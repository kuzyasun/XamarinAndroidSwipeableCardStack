#region using

using System;
using Android.Content;
using Android.Views;
using GestureDetectorCompat = Android.Support.V4.View;

#endregion

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards
{
    //detect both tap and drag
    public class DragGestureDetector
    {
        public static string DebugTag = "DragGestureDetector";
        private readonly GestureDetectorCompat.GestureDetectorCompat _gestureDetector;
        private readonly IDragListener _listener;
        private bool _started;
        private MotionEvent _originalEvent;

        public interface IDragListener
        {
            bool OnDragStart(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY);
            bool OnDragContinue(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY);
            bool OnDragEnd(MotionEvent e1, MotionEvent e2);
            bool OnTapUp();
        }

        public DragGestureDetector(Context context, IDragListener myDragListener)
        {
            _gestureDetector = new GestureDetectorCompat.GestureDetectorCompat(context, new MyGestureListener(this));
            _listener = myDragListener;
        }

        public void OnTouchEvent(MotionEvent @event)
        {
            _gestureDetector.OnTouchEvent(@event);
            int action = GestureDetectorCompat.MotionEventCompat.GetActionMasked(@event);

            switch (action)
            {
                case ((int) MotionEventActions.Up):

                    if (_started)
                        _listener.OnDragEnd(_originalEvent, @event);

                    _started = false;
                    break;
                case ((int) MotionEventActions.Down):
                    //need to set this, quick tap will not generate drap event, so the
                    //originalEvent may be null for case action_up
                    //which lead to null pointer
                    _originalEvent = @event;
                    break;
            }
        }

        internal class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private readonly DragGestureDetector _outerInstance;

            public MyGestureListener(DragGestureDetector outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                if (_outerInstance._listener == null)
                    return true;

                if (!_outerInstance._started)
                {
                    _outerInstance._listener.OnDragStart(e1, e2, distanceX, distanceY);
                    _outerInstance._started = true;
                }
                else
                {
                    _outerInstance._listener.OnDragContinue(e1, e2, distanceX, distanceY);
                }
                _outerInstance._originalEvent = e1;
                return true;
            }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                return _outerInstance._listener.OnTapUp();
            }
        }
    }
}