#region using

using System;

#endregion

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards
{
    public class DefaultStackEventListener : CardStack.ICardEventListener
    {
        private readonly float _threshold;

        public DefaultStackEventListener(int i)
        {
            _threshold = i;
        }

        public bool SwipeEnd(int section, float x1, float y1, float x2, float y2)
        {
            var distance = CardUtils.Distance(x1, y1, x2, y2);
            return distance > _threshold;
        }

        public bool SwipeStart(int section, float x1, float y1, float x2, float y2)
        {
            return false;
        }

        public bool SwipeContinue(int section, float x1, float y1, float x2, float y2)
        {
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