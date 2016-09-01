#region using

using System;
using System.Collections.Generic;
using Android.Animation;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Gemslibe.Xamarin.Droid.UI.SwipeCards.Animation;

#endregion

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards
{
    public class CardAnimator
    {
        private int _remoteDistance;
        private readonly int _mBackgroundColor;
        private readonly RelativeLayout.LayoutParams[] _mRemoteLayouts = new RelativeLayout.LayoutParams[4];
        private RelativeLayout.LayoutParams _baseLayout;
        private Dictionary<View, RelativeLayout.LayoutParams> _mLayoutsMap;
        private float _mRotation;
        private int _mStackMargin;
        public List<View> MCardCollection;

        public CardAnimator(List<View> viewCollection, int backgroundColor, int stackMargin, int remoteDistance)
        {
            MCardCollection = viewCollection;
            _remoteDistance = remoteDistance;
            _mBackgroundColor = backgroundColor;
            _mStackMargin = stackMargin;
            Setup();
        }

        private View TopView
        {
            get { return MCardCollection[MCardCollection.Count - 1]; }
        }

        public virtual int StackMargin
        {
            set
            {
                _mStackMargin = value;
                InitLayout();
            }
        }

        private void Setup()
        {
            _mLayoutsMap = new Dictionary<View, RelativeLayout.LayoutParams>();

            foreach (var v in MCardCollection)
            {
                //setup basic layout
                var @params = (RelativeLayout.LayoutParams) v.LayoutParameters;
                @params.AddRule(LayoutRules.AlignParentTop);
                @params.Width = ViewGroup.LayoutParams.MatchParent;
                @params.Height = ViewGroup.LayoutParams.MatchParent;

                if (_mBackgroundColor != -1)
                {
                    v.SetBackgroundColor(new Color(_mBackgroundColor));
                }

                v.LayoutParameters = @params;
            }

            _baseLayout = (RelativeLayout.LayoutParams) MCardCollection[0].LayoutParameters;
            _baseLayout = CardUtils.CloneParams(_baseLayout);

            InitLayout();

            foreach (var v in MCardCollection)
            {
                var @params = (RelativeLayout.LayoutParams) v.LayoutParameters;
                var paramsCopy = CardUtils.CloneParams(@params);
                _mLayoutsMap.Add(v, paramsCopy);
            }

            SetupRemotes();
        }

        public virtual void InitLayout()
        {
            var size = MCardCollection.Count;
            foreach (var v in MCardCollection)
            {
                var index = MCardCollection.IndexOf(v);
                if (index != 0)
                {
                    index -= 1;
                }
                var @params = CardUtils.CloneParams(_baseLayout);
                v.LayoutParameters = @params;

                CardUtils.Scale(v, -(size - index - 1) * 5);
                CardUtils.Move(v, index * _mStackMargin, 0);
                v.Rotation = 0;
            }
        }

        private void SetupRemotes()
        {
            var topView = TopView;
            _mRemoteLayouts[0] = CardUtils.GetMoveParams(topView, _remoteDistance, -_remoteDistance);
            _mRemoteLayouts[1] = CardUtils.GetMoveParams(topView, _remoteDistance, _remoteDistance);
            _mRemoteLayouts[2] = CardUtils.GetMoveParams(topView, -_remoteDistance, -_remoteDistance);
            _mRemoteLayouts[3] = CardUtils.GetMoveParams(topView, -_remoteDistance, _remoteDistance);
        }

        private void MoveToBack(View child)
        {
            var parent = (ViewGroup) child.Parent;
            if (null != parent)
            {
                parent.RemoveView(child);
                parent.AddView(child, 0);
            }
        }

        private void Reorder()
        {
            var temp = TopView;
            //RelativeLayout.LayoutParameters tempLp = mLayoutsMap.get(mCardCollection.get(0));
            //mLayoutsMap.put(temp,tempLp);
            MoveToBack(temp);

            for (var i = MCardCollection.Count - 1; i > 0; i--)
            {
                //View next = mCardCollection.get(i);
                //RelativeLayout.LayoutParameters lp = mLayoutsMap.get(next);
                //mLayoutsMap.remove(next);
                var current = MCardCollection[i - 1];
                //current replace next
                MCardCollection[i] = current;
                //mLayoutsMap.put(current,lp);
            }
            MCardCollection[0] = temp;

            temp = TopView;
        }

        public virtual void Discard(int direction, Animator.IAnimatorListener al, int duration = 250)
        {
            var @as = new AnimatorSet();
            var aCollection = new List<Animator>();

            var topView = TopView;
            var topParams = (RelativeLayout.LayoutParams) topView.LayoutParameters;
            var layout = CardUtils.CloneParams(topParams);
            var discardAnim = ValueAnimator.OfObject(new RelativeLayoutParamsEvaluator(), layout,
                _mRemoteLayouts[direction]);

            discardAnim.Update +=
                (sender, args) =>
                {
                    topView.LayoutParameters = (RelativeLayout.LayoutParams) args.Animation.AnimatedValue;
                };

            discardAnim.SetDuration(duration);
            aCollection.Add(discardAnim);

            for (var i = 0; i < MCardCollection.Count; i++)
            {
                var v = MCardCollection[i];

                if (v == topView) continue;

                var nv = MCardCollection[i + 1];
                var layoutParams = (RelativeLayout.LayoutParams) v.LayoutParameters;
                var endLayout = CardUtils.CloneParams(layoutParams);
                RelativeLayout.LayoutParams viewParams;
                _mLayoutsMap.TryGetValue(nv, out viewParams);
                var layoutAnim = ValueAnimator.OfObject(new RelativeLayoutParamsEvaluator(), endLayout, viewParams);
                layoutAnim.SetDuration(duration);
                layoutAnim.Update +=
                    (sender, args) =>
                    {
                        v.LayoutParameters = (RelativeLayout.LayoutParams) args.Animation.AnimatedValue;
                    };
                aCollection.Add(layoutAnim);
            }

            @as.AddListener(new DiscardAnimationEndListener(this, al));
            @as.PlayTogether(aCollection);
            @as.Start();
        }

        public virtual void Reverse(MotionEvent e1, MotionEvent e2)
        {
            var topView = TopView;
            var rotationAnim = ValueAnimator.OfFloat(_mRotation, 0f);
            rotationAnim.SetDuration(250);
            rotationAnim.Update +=
                (sender, args) => { topView.Rotation = ((float?) args.Animation.AnimatedValue).Value; };

            rotationAnim.Start();

            foreach (var v in MCardCollection)
            {
                var layoutParams = (RelativeLayout.LayoutParams) v.LayoutParameters;
                var endLayout = CardUtils.CloneParams(layoutParams);
                RelativeLayout.LayoutParams viewParams;
                _mLayoutsMap.TryGetValue(v, out viewParams);
                var layoutAnim = ValueAnimator.OfObject(new RelativeLayoutParamsEvaluator(), endLayout, viewParams);
                layoutAnim.SetDuration(250);
                layoutAnim.Update +=
                    (sender, args) =>
                    {
                        v.LayoutParameters = (RelativeLayout.LayoutParams) args.Animation.AnimatedValue;
                    };
                layoutAnim.Start();
            }
        }

        public virtual void Drag(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            var topView = TopView;

            var rotationCoefficient = 20f;

            var layoutParams = (RelativeLayout.LayoutParams) topView.LayoutParameters;
            RelativeLayout.LayoutParams topViewLayouts;
            _mLayoutsMap.TryGetValue(topView, out topViewLayouts);
            var xDiff = (int) (e2.RawX - e1.RawX);
            var yDiff = (int) (e2.RawY - e1.RawY);

            layoutParams.LeftMargin = topViewLayouts.LeftMargin + xDiff;
            layoutParams.RightMargin = topViewLayouts.RightMargin - xDiff;
            layoutParams.TopMargin = topViewLayouts.TopMargin + yDiff;
            layoutParams.BottomMargin = topViewLayouts.BottomMargin - yDiff;

            _mRotation = xDiff / rotationCoefficient;
            topView.Rotation = _mRotation;
            topView.LayoutParameters = layoutParams;

            //animate secondary views.
            foreach (var v in MCardCollection)
            {
                var index = MCardCollection.IndexOf(v);
                if (v != TopView && index != 0)
                {
                    RelativeLayout.LayoutParams viewParams;
                    _mLayoutsMap.TryGetValue(v, out viewParams);
                    var l = CardUtils.ScaleFrom(v, viewParams, (int) (Math.Abs(xDiff) * 0.05));
                    CardUtils.MoveFrom(v, l, 0, (int) (Math.Abs(xDiff) * 0.1));
                }
            }
        }

        private class DiscardAnimationEndListener : AnimatorListenerAdapter
        {
            private readonly Animator.IAnimatorListener _al;
            private readonly CardAnimator _outerInstance;

            public DiscardAnimationEndListener(CardAnimator outerInstance, Animator.IAnimatorListener al)
            {
                _outerInstance = outerInstance;
                _al = al;
            }

            public override void OnAnimationEnd(Animator animation)
            {
                _outerInstance.Reorder();
                _al?.OnAnimationEnd(animation);
                _outerInstance._mLayoutsMap = new Dictionary<View, RelativeLayout.LayoutParams>();

                foreach (var v in _outerInstance.MCardCollection)
                {
                    var @params = (RelativeLayout.LayoutParams) v.LayoutParameters;
                    var paramsCopy = CardUtils.CloneParams(@params);
                    _outerInstance._mLayoutsMap.Add(v, paramsCopy);
                }
            }
        }
    }
}