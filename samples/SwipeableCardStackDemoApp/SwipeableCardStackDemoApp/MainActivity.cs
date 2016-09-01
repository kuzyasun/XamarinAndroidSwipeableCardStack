#region using

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

#endregion

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards
{
    [Activity(Label = "SwipeableCardStackDemoApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private CardStack _cardStack;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            SetContentView(Resource.Layout.activity_main);

            _cardStack = FindViewById<CardStack>(Resource.Id.container);
            _cardStack.ContentResource = Resource.Layout.card;
            //you can adjust swipe behaviour with your custom swipe listener
            _cardStack.CardEventListener = new MyCard.CardSwipeListener(Dp2Px(this, 100), _cardStack);
            //cardStack.setStackMargin(20);
            var cardAdapter = new CardsAdapter(ApplicationContext, Resource.Layout.card);
            cardAdapter.Add(new CardModel {ImgResId = Resource.Drawable.android_img_1});
            cardAdapter.Add(new CardModel {ImgResId = Resource.Drawable.android_img_2});
            cardAdapter.Add(new CardModel {ImgResId = Resource.Drawable.android_img_3});
            cardAdapter.Add(new CardModel {ImgResId = Resource.Drawable.android_img_4});
            cardAdapter.Add(new CardModel {ImgResId = Resource.Drawable.android_img_5});
            cardAdapter.Add(new CardModel {ImgResId = Resource.Drawable.android_img_6});
            cardAdapter.Add(new CardModel {ImgResId = Resource.Drawable.android_img_7});
            cardAdapter.Add(new CardModel {ImgResId = Resource.Drawable.apple_vs_android_02});

            cardAdapter.OnTapButtonsEvent += OnButtonTap;
            cardAdapter.OnCardSwipeActionEvent += OnCardSwipeActionEvent;

            _cardStack.Adapter = cardAdapter;
        }

        private void OnCardSwipeActionEvent(string action)
        {
            Toast.MakeText(this, action, ToastLength.Short).Show();
        }

        private void OnButtonTap(string action)
        {
            Toast.MakeText(this, action, ToastLength.Short).Show();
            var direction = (action == "like") ? 3 : 2;
            Task.Delay(250).ContinueWith(o => { RunOnUiThread(() => _cardStack.DiscardTop(direction, 700)); });
        }

        public static int Dp2Px(Context context, int dip)
        {
            DisplayMetrics displayMetrics = context.Resources.DisplayMetrics;
            return (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, dip, displayMetrics);
        }
    }
}