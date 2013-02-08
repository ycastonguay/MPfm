using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Objects;

namespace MPfm.Android.Classes.Adapters
{
    public class GenericListAdapter : BaseAdapter<GenericListItem>
    {
        readonly Activity _context;
        readonly IList<GenericListItem> _items;

        public GenericListAdapter(Activity context, IList<GenericListItem> items)
        {
            this._context = context;
            this._items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override GenericListItem this[int position]
        {
            get { return _items[position]; }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.GenericCell, null);

            //view.SetBackgroundColor(Color.White);

            var title = view.FindViewById<TextView>(Resource.Id.genericcell_title);
            title.Text = _items[position].Title;

            var image = view.FindViewById<ImageView>(Resource.Id.genericcell_image);           
            image.SetBackgroundColor(Color.White);

            return view;
        }
    }
}