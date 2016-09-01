#region using

using System;
using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.Content.Res;
using Android.Util;
using Android.Views;
using Android.Widget;

#endregion

/*
 * Port of Android Java project https://github.com/wenchaojiang/AndroidSwipeableCardStack
 */

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards
{
    //TODO refactor to RecyclerView.Adapter
    public class CardStack : RelativeLayout
    {
        public event Action<int, int> DiscardedEvent;
        public event EventHandler OnCardSwiped;

        public const int DefaultStackMargin = 20;
        private int _stackMargin = DefaultStackMargin;
        private int _color = -1;
        private int _index;
        private int _numVisible = 4;
        private bool _canSwipe = true;
        private CardStackAdapter _adapter;
        private IOnTouchListener _onTouchListener;
        private CardAnimator _cardAnimator;

        public List<View> CardViews = new List<View>();

        private ICardEventListener _cardEventListener = new DefaultStackEventListener(300);
        private int _contentResource;

        public interface ICardEventListener
        {
            //section
            // 0 | 1
            //--------
            // 2 | 3
            // swipe distance, most likely be used with height and width of a view ;

            bool SwipeEnd(int section, float x1, float y1, float x2, float y2);
            bool SwipeStart(int section, float x1, float y1, float x2, float y2);
            bool SwipeContinue(int section, float x1, float y1, float x2, float y2);
            void Discarded(int mIndex, int direction);
            void TopCardTapped();
        }

        public void DiscardTop(int direction, int duration)
        {
            _cardAnimator.Discard(direction, new AnimatorListenerAdapterAnonymousInnerClass(this, direction, TopView),
                duration);
        }

        private class AnimatorListenerAdapterAnonymousInnerClass : AnimatorListenerAdapter
        {
            private readonly CardStack _outerInstance;

            private readonly int _direction;
            private readonly View _topView;

            public AnimatorListenerAdapterAnonymousInnerClass(CardStack outerInstance, int direction, View topView)
            {
                _outerInstance = outerInstance;
                _direction = direction;
                _topView = topView;
            }

            public override void OnAnimationEnd(Animator arg0)
            {
                _outerInstance._cardAnimator.InitLayout();

                _outerInstance._index++;
                _outerInstance.LoadLast(_topView);
                _outerInstance.RecycleView(_topView);

                _outerInstance.ViewCollection[0].SetOnTouchListener(null);
                _outerInstance.ViewCollection[_outerInstance.ViewCollection.Count - 1].SetOnTouchListener(
                    _outerInstance._onTouchListener);
                _outerInstance._cardEventListener.Discarded(_outerInstance._index - 1, _direction);
                _outerInstance.DiscardedEvent?.Invoke(_outerInstance._index - 1, _direction);
            }
        }

        public int CurrIndex
        {
            get
            {
//sync?
                return _index;
            }
        }

        //only necessary when I need the attrs from xml, this will be used when inflating layout
        public CardStack(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            if (attrs != null)
            {
                TypedArray array = context.ObtainStyledAttributes(attrs, Resource.Styleable.CardStack);
                _color = array.GetColor(Resource.Styleable.CardStack_backgroundColor, _color);
                _stackMargin = array.GetInteger(Resource.Styleable.CardStack_stackMargin, _stackMargin);
                array.Recycle();
            }

            //get attrs assign minVisiableNum
            for (int i = 0; i < _numVisible; i++)
                AddContainerViews();

            SetupAnimation();
        }

        private void AddContainerViews()
        {
            FrameLayout v = new FrameLayout(Context);

            ViewCollection.Add(v);
            AddView(v);
        }

        public int StackMargin
        {
            set
            {
                _stackMargin = value;
                _cardAnimator.StackMargin = _stackMargin;
                _cardAnimator.InitLayout();
            }
        }

        public int ContentResource
        {
            set { _contentResource = value; }
        }

        public bool CanSwipe
        {
            set { _canSwipe = value; }
        }

        public void Reset(bool resetIndex)
        {
            if (resetIndex)
                _index = 0;

            RemoveAllViews();
            ViewCollection.Clear();

            for (int i = 0; i < _numVisible; i++)
                AddContainerViews();

            SetupAnimation();
            LoadData();
        }

        public int VisibleCardNum
        {
            set
            {
                _numVisible = value;
                Reset(false);
            }
        }

        public int Threshold
        {
            set { _cardEventListener = new DefaultStackEventListener(value); }
        }

        public ICardEventListener CardEventListener
        {
            get { return _cardEventListener; }
            set { _cardEventListener = value; }
        }

        private void SetupAnimation()
        {
            View cardView = ViewCollection[ViewCollection.Count - 1];
            _cardAnimator = new CardAnimator(
                ViewCollection,
                _color,
                _stackMargin,
                Context.Resources.DisplayMetrics.HeightPixels);

            _cardAnimator.InitLayout();
            DragGestureDetector dd = new DragGestureDetector(Context, new DragListener(this));

            _onTouchListener = new OnTouchListener(this, dd);

            cardView.SetOnTouchListener(_onTouchListener);

            Clickable = true;
            cardView.Clickable = true;
        }

        private sealed class DragListener : DragGestureDetector.IDragListener
        {
            private readonly CardStack _outerInstance;

            public DragListener(CardStack outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public bool OnDragStart(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                if (_outerInstance._canSwipe)
                    _outerInstance._cardAnimator.Drag(e1, e2, distanceX, distanceY);

                float x1 = e1.RawX;
                float y1 = e1.RawY;
                float x2 = e2.RawX;
                float y2 = e2.RawY;

                int direction = CardUtils.Direction(x1, y1, x2, y2);

                _outerInstance._cardEventListener.SwipeStart(direction, x1, y1, x2, y2);

                return true;
            }

            public bool OnDragContinue(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                float x1 = e1.RawX;
                float y1 = e1.RawY;
                float x2 = e2.RawX;
                float y2 = e2.RawY;
                int direction = CardUtils.Direction(x1, y1, x2, y2);
                if (_outerInstance._canSwipe)
                    _outerInstance._cardAnimator.Drag(e1, e2, distanceX, distanceY);

                _outerInstance._cardEventListener.SwipeContinue(direction, x1, y1, x2, y2);

                return true;
            }

            public bool OnDragEnd(MotionEvent e1, MotionEvent e2)
            {
                //reverse(e1,e2);
                float x1 = e1.RawX;
                float y1 = e1.RawY;
                float x2 = e2.RawX;
                float y2 = e2.RawY;

                int direction = CardUtils.Direction(x1, y1, x2, y2);

                bool discard = _outerInstance._cardEventListener.SwipeEnd(direction, x1, y1, x2, y2);

                if (discard)
                {
                    if (_outerInstance._canSwipe)
                        _outerInstance._cardAnimator.Discard(direction,
                            new AnimatorListenerAdapterAnonymousInnerClass2(this, direction, _outerInstance.TopView));
                }
                else
                {
                    if (_outerInstance._canSwipe)
                        _outerInstance._cardAnimator.Reverse(e1, e2);
                }

                _outerInstance.OnCardSwiped?.Invoke(_outerInstance, EventArgs.Empty);

                return true;
            }

            private class AnimatorListenerAdapterAnonymousInnerClass2 : AnimatorListenerAdapter
            {
                private readonly DragListener _outerInstance;

                private readonly int _direction;
                private readonly View _topView;

                public AnimatorListenerAdapterAnonymousInnerClass2(DragListener outerInstance, int direction,
                    View topView)
                {
                    _outerInstance = outerInstance;
                    _direction = direction;
                    _topView = topView;
                }

                public override void OnAnimationEnd(Animator arg0)
                {
                    _outerInstance._outerInstance._cardAnimator.InitLayout();
                    _outerInstance._outerInstance._index++;
                    _outerInstance._outerInstance._cardEventListener.Discarded(_outerInstance._outerInstance._index,
                        _direction);
                    _outerInstance._outerInstance.DiscardedEvent?.Invoke(_outerInstance._outerInstance._index,
                        _direction);

                    //_index = _index%_adapter.getCount();
                    _outerInstance._outerInstance.RecycleView(_topView);
                    _outerInstance._outerInstance.LoadLast(_topView);

                    _outerInstance._outerInstance.ViewCollection[0].SetOnTouchListener(null);
                    _outerInstance._outerInstance.ViewCollection[_outerInstance._outerInstance.ViewCollection.Count - 1]
                        .SetOnTouchListener(_outerInstance._outerInstance._onTouchListener);
                }
            }

            public bool OnTapUp()
            {
                _outerInstance._cardEventListener.TopCardTapped();
                return true;
            }
        }

        private class OnTouchListener : Java.Lang.Object, IOnTouchListener
        {
            private readonly CardStack _outerInstance;

            private DragGestureDetector _dd;


            public OnTouchListener(CardStack outerInstance, DragGestureDetector dd)
            {
                _outerInstance = outerInstance;
                _dd = dd;
            }

            public bool OnTouch(View arg0, MotionEvent @event)
            {
                _dd.OnTouchEvent(@event);
                return true;
            }
        }

        private Android.Database.DataSetObserver _dataObserver;

        private class DataSetObserver : Android.Database.DataSetObserver
        {
            private readonly CardStack _stack;

            public DataSetObserver(CardStack stack)
            {
                _stack = stack;
            }

            public override void OnChanged()
            {
                _stack.Reset(false);
            }
        }

        //ArrayList

        internal List<View> ViewCollection = new List<View>();

        public CardStack(Context context) : base(context)
        {
        }

        public CardStackAdapter Adapter
        {
            set
            {
                _adapter?.UnregisterDataSetObserver(_dataObserver);

                if (_dataObserver == null)
                    _dataObserver = new DataSetObserver(this);

                _adapter = value;
                value.RegisterDataSetObserver(_dataObserver);

                LoadData();
            }

            get { return _adapter; }
        }

        public View TopView
        {
            get { return ((ViewGroup) ViewCollection[ViewCollection.Count - 1]).GetChildAt(0); }
        }

        private void LoadData()
        {
            for (int i = _numVisible - 1; i >= 0; i--)
            {
                ViewGroup parent = (ViewGroup) ViewCollection[i];
                int index = (_index + _numVisible - 1) - i;
                if (index > _adapter.Count - 1)
                {
                    parent.Visibility = ViewStates.Gone;
                }
                else
                {
                    View child = _adapter.GetView(index, ContentView, this);
                    _adapter.BindView(index, child, this);
                    parent.AddView(child);
                    parent.Visibility = ViewStates.Visible;
                }
            }
        }

        private View ContentView
        {
            get
            {
                View contentView = null;
                if (_contentResource != 0)
                {
                    LayoutInflater lf = LayoutInflater.From(Context);
                    contentView = lf.Inflate(_contentResource, null);
                    CardViews.Add(contentView);
                }
                return contentView;
            }
        }

        private void LoadLast(View topView)
        {
            ViewGroup parent = (ViewGroup) ViewCollection[0];

            int lastIndex = (_numVisible - 1) + _index;
            if (lastIndex > _adapter.Count - 1)
            {
                parent.Visibility = ViewStates.Gone;
                return;
            }
            _adapter.BindView(lastIndex, topView, parent);
            //            View child = _adapter.GetView(lastIndex, topView, parent);
            //            parent.RemoveAllViews();
            ////            ViewGroup group = (ViewGroup) child.Parent;
            ////            group?.RemoveView(child);
            //            parent.AddView(child);
        }

        private void RecycleView(View topView)
        {
        }

        public int StackSize
        {
            get { return _numVisible; }
        }
    }
}