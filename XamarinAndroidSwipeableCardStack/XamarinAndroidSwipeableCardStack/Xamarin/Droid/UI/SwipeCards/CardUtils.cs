#region using

using System;
using Android.Views;
using Android.Widget;

#endregion

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards
{
    public class CardUtils
    {
        internal const int DirectionTopLeft = 0;
        internal const int DirectionTopRight = 1;
        internal const int DirectionBottomLeft = 2;
        internal const int DirectionBottomRight = 3;

        public static void Scale(View v, int pixel)
        {
            RelativeLayout.LayoutParams @params = (RelativeLayout.LayoutParams) v.LayoutParameters;
            @params.LeftMargin -= pixel;
            @params.RightMargin -= pixel;
            @params.TopMargin -= pixel;
            @params.BottomMargin -= pixel;
            v.LayoutParameters = @params;
        }

        public static RelativeLayout.LayoutParams GetMoveParams(View v, int upDown, int leftRight)
        {
            RelativeLayout.LayoutParams original = (RelativeLayout.LayoutParams) v.LayoutParameters;
            //RelativeLayout.LayoutParams params = new RelativeLayout.LayoutParams(original);
            RelativeLayout.LayoutParams @params = CloneParams(original);
            @params.LeftMargin += leftRight;
            @params.RightMargin -= leftRight;
            @params.TopMargin -= upDown;
            @params.BottomMargin += upDown;
            return @params;
        }

        public static void Move(View v, int upDown, int leftRight)
        {
            RelativeLayout.LayoutParams @params = GetMoveParams(v, upDown, leftRight);
            v.LayoutParameters = @params;
        }

        public static RelativeLayout.LayoutParams ScaleFrom(View v, RelativeLayout.LayoutParams @params, int pixel)
        {
            @params = CloneParams(@params);
            @params.LeftMargin -= pixel;
            @params.RightMargin -= pixel;
            @params.TopMargin -= pixel;
            @params.BottomMargin -= pixel;

            v.LayoutParameters = @params;

            return @params;
        }

        public static RelativeLayout.LayoutParams MoveFrom(View v, RelativeLayout.LayoutParams @params, int leftRight,
            int upDown)
        {
            @params = CloneParams(@params);
            @params.LeftMargin += leftRight;
            @params.RightMargin -= leftRight;
            @params.TopMargin -= upDown;
            @params.BottomMargin += upDown;
            v.LayoutParameters = @params;

            return @params;
        }

        //a copy method for RelativeLayout.LayoutParams for backward compartibility
        public static RelativeLayout.LayoutParams CloneParams(RelativeLayout.LayoutParams @params)
        {
            RelativeLayout.LayoutParams copy = new RelativeLayout.LayoutParams(@params.Width, @params.Height);
            copy.LeftMargin = @params.LeftMargin;
            copy.TopMargin = @params.TopMargin;
            copy.RightMargin = @params.RightMargin;
            copy.BottomMargin = @params.BottomMargin;
            int[] rules = @params.GetRules();

            for (int i = 0; i < rules.Length; i++)
                copy.AddRule((LayoutRules) i, rules[i]);

            //copy.setMarginStart(params.getMarginStart());
            //copy.setMarginEnd(params.getMarginEnd());

            return copy;
        }

        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return (float) Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public static int Direction(float x1, float y1, float x2, float y2)
        {
            if (x2 > x1)
            {
                //RIGHT
                if (y2 > y1)
                {
                    //BOTTOM
                    return DirectionBottomRight;
                }
                else
                {
                    //TOP
                    return DirectionTopRight;
                }
            }
            else
            {
                //LEFT
                if (y2 > y1)
                {
                    //BOTTOM
                    return DirectionBottomLeft;
                }
                else
                {
                    //TOP
                    return DirectionTopLeft;
                }
            }
        }
    }
}