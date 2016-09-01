#region using

using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;

#endregion

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards
{
    public class MyCard : CardView
    {
        public MyCard(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MyCard(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public MyCard(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public MyCard(Context context) : base(context)
        {
        }

        public event Action<string> OnCardSwipeActionEvent;

        internal class CardSwipeListener : CardStack.ICardEventListener
        {
            private readonly int _discardDistancePx;
            private readonly CardStack _cardStack;

            public CardSwipeListener(int discardDistancePx, CardStack cardStack)
            {
                _discardDistancePx = discardDistancePx;
                _cardStack = cardStack;
            }

            public bool SwipeEnd(int section, float x1, float y1, float x2, float y2)
            {
                //var distance = CardUtils.Distance(x1, y1, x2, y2);
                //Discard card only if user moves card to RIGHT/LEFT
                var discard = Math.Abs(x2 - x1) > _discardDistancePx;
                var cardView = _cardStack.TopView as MyCard;
                if (discard)
                {
                    var action = (x2 < x1) ? "dislike" : "like";
                    cardView.OnCardSwipeActionEvent?.Invoke(action);
                }
                ;
                return discard;
            }

            public bool SwipeStart(int section, float x1, float y1, float x2, float y2)
            {
                //var cardView = _cardStack.TopView as ProductCard;
                return false;
            }

            public bool SwipeContinue(int section, float x1, float y1, float x2, float y2)
            {
                // var cardView = _cardStack.TopView as ProductCard;
                //cardView.ProgressToDiscad = (x2 - x1) / _discardDistancePx;

                return false;
            }

            public void Discarded(int mIndex, int direction)
            {
            }

            public void TopCardTapped()
            {
            }
        }
    }
}