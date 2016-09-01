#region using

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

#endregion

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards
{
    public class CardsAdapter : CardStackAdapter
    {
        //TODO with RecycleAdapter
  /*      public class CardViewHolder : RecyclerView.ViewHolder
        {
            public ImageButton btnLike;
            public ImageButton btnDislike;
            public ImageView imgProduct;

            public CardViewHolder(View itemView) : base(itemView)
            {
                btnLike = itemView.FindViewById<ImageButton>(Resource.Id.btnLike);
                btnDislike = itemView.FindViewById<ImageButton>(Resource.Id.btnDislike);
                imgProduct = itemView.FindViewById<ImageView>(Resource.Id.imgProduct);
            }

            public void Bind(CardModel cm)
            {
                imgProduct.SetImageResource(cm.ImgResId);
            }
        }*/

        public CardsAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public CardsAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
        }

        public CardsAdapter(Context context, int resource, int textViewResourceId)
            : base(context, resource, textViewResourceId)
        {
        }

        public CardsAdapter(Context context, int textViewResourceId, Object[] objects)
            : base(context, textViewResourceId, objects)
        {
        }

        public CardsAdapter(Context context, int resource, int textViewResourceId, Object[] objects)
            : base(context, resource, textViewResourceId, objects)
        {
        }

        public CardsAdapter(Context context, int textViewResourceId, IList<Object> objects)
            : base(context, textViewResourceId, objects)
        {
        }

        public CardsAdapter(Context context, int resource, int textViewResourceId, IList<Object> objects)
            : base(context, resource, textViewResourceId, objects)
        {
        }

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
}