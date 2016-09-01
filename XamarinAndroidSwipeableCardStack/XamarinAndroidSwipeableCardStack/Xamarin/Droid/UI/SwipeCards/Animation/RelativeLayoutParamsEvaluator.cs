#region using

using System;
using Android.Animation;
using Android.Widget;
using Object = Java.Lang.Object;

#endregion

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards.Animation
{
    public class RelativeLayoutParamsEvaluator : Object, ITypeEvaluator
    {
        public Object Evaluate(float fraction, Object startValue, Object endValue)
        {
            var start = (RelativeLayout.LayoutParams) startValue;
            var end = (RelativeLayout.LayoutParams) endValue;
            RelativeLayout.LayoutParams result = CardUtils.CloneParams(start);
            result.LeftMargin += (int) ((end.LeftMargin - start.LeftMargin) * fraction);
            result.RightMargin += (int) ((end.RightMargin - start.RightMargin) * fraction);
            result.TopMargin += (int) ((end.TopMargin - start.TopMargin) * fraction);
            result.BottomMargin += (int) ((end.BottomMargin - start.BottomMargin) * fraction);
            return result;
        }
    }
}