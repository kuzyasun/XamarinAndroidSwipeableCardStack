#region using

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

#endregion

namespace Gemslibe.Xamarin.Droid.UI.SwipeCards
{
    //TODO refactor to RecyclerView.Adapter
    public abstract class CardStackAdapter : ArrayAdapter<Object>
    {
        public abstract void BindView(int position, View convertView, ViewGroup parent);

        public CardStackAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public CardStackAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
        }

        public CardStackAdapter(Context context, int resource, int textViewResourceId)
            : base(context, resource, textViewResourceId)
        {
        }

        public CardStackAdapter(Context context, int textViewResourceId, Object[] objects)
            : base(context, textViewResourceId, objects)
        {
        }

        public CardStackAdapter(Context context, int resource, int textViewResourceId, Object[] objects)
            : base(context, resource, textViewResourceId, objects)
        {
        }

        public CardStackAdapter(Context context, int textViewResourceId, IList<Object> objects)
            : base(context, textViewResourceId, objects)
        {
        }

        public CardStackAdapter(Context context, int resource, int textViewResourceId, IList<Object> objects)
            : base(context, resource, textViewResourceId, objects)
        {
        }
    }
}