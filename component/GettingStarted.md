Configuration
-----


Put CardStack in your layout file

```xml
     <gemslibe.xamarin.droid.ui.swipecards.CardStack
        android:id="@+id/container"
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        android:clipChildren="false"
        android:clipToPadding="false"
        android:layout_margin="-15dp" />
```

Create your card view layout file.

Example: card_layout.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<gemslibe.xamarin.droid.ui.swipecards.MyCard xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:card_view="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@android:color/white"
    card_view:cardCornerRadius="20dp"
    card_view:cardElevation="10dp">
    <RelativeLayout
        android:layout_width="match_parent"
        android:layout_height="match_parent">
        <ImageView
            android:id="@+id/imgProduct"
            android:layout_width="match_parent"
            android:layout_height="match_parent"
            android:layout_above="@+id/btnDislike"
            android:layout_alignParentLeft="true"
            android:layout_alignParentStart="true"
            android:padding="100dp"
            android:scaleType="fitCenter"
            android:src="@drawable/android_img_1" />
        <ImageButton
            android:id="@+id/btnDislike"
            android:layout_width="100dp"
            android:layout_height="100dp"
            android:layout_alignParentLeft="true"
            android:layout_alignParentStart="true"
            android:layout_alignTop="@+id/btnLike"
            android:layout_marginLeft="50dp"
            android:layout_marginStart="50dp"
            android:background="@drawable/xamarin_icon" />
        <ImageButton
            android:id="@+id/btnLike"
            android:layout_width="100dp"
            android:layout_height="100dp"
            android:layout_alignParentBottom="true"
            android:layout_alignParentEnd="true"
            android:layout_alignParentRight="true"
            android:layout_marginBottom="50dp"
            android:layout_marginEnd="50dp"
            android:layout_marginRight="50dp"
            android:background="@drawable/xamarin_icon" />
    </RelativeLayout>
</gemslibe.xamarin.droid.ui.swipecards.MyCard>
```

Extend and implement your own adapter for the card stack. The CardStack will accept ArrayAdapter.
The Following example extends a simple ArrayAdapter<String>, overriding ```GetView()``` and ```BindView()``` to supply your customized card layout

```cs
 public class CardsAdapter : CardStackAdapter
    {
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = ((MyCard)convertView);
            var btnLike = convertView.FindViewById<ImageButton>(Resource.Id.btnLike);
            var btnDislike = convertView.FindViewById<ImageButton>(Resource.Id.btnDislike);
            btnLike.Click += (sender, args) => { OnTapButtonsEvent?.Invoke("like"); };

            btnDislike.Click += (sender, args) => { OnTapButtonsEvent?.Invoke("dislike"); };

            view.OnCardSwipeActionEvent += action => { OnCardSwipeActionEvent?.Invoke(action); };

            return convertView;
        }

        public event Action<string> OnTapButtonsEvent;
        public event Action<string> OnCardSwipeActionEvent;

        public override void BindView(int position, View convertView, ViewGroup parent)
        {
            var cm = (CardModel)GetItem(position);
            var imgProduct = convertView.FindViewById<ImageView>(Resource.Id.imgProduct);

            imgProduct.SetImageResource(cm.ImgResId);
        }
    }
```
Create your card model class
```cs
	 public class CardModel
    {
        public int ImgResId { get; set; }
    }
```
Get the CardStack instance in your activity and set the adapter

```cs
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
```

Listening to card stack event 
----
implement CardStack.CardEventListener, and set it as listener
```cs
	_cardStack.CardEventListener = new MyCard.CardSwipeListener(Dp2Px(this, 100), _cardStack);
```
```cs
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
                
                return discard;
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
```


---
See demo project