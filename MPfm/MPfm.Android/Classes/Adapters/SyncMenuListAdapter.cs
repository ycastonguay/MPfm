// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MPfm.MVP.Models;

namespace MPfm.Android.Classes.Adapters
{
    public class SyncMenuListAdapter : BaseAdapter<SyncMenuItemEntity>
    {
        private readonly SyncMenuActivity _context;
        private List<SyncMenuItemEntity> _items;

        public SyncMenuListAdapter(SyncMenuActivity context, List<SyncMenuItemEntity> items)
        {
            _context = context;
            _items = items;
        }

        public void SetData(List<SyncMenuItemEntity> items)
        {
            _items = items;
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override SyncMenuItemEntity this[int position]
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

            var title = view.FindViewById<TextView>(Resource.Id.genericcell_title);
            title.Text = _items[position].ArtistName;

            var image = view.FindViewById<ImageView>(Resource.Id.genericcell_image);
            image.SetBackgroundColor(Color.White);

            return view;
        }
    }
}
