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
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MPfm.Core;
using MPfm.Player.Objects;

namespace MPfm.Android.Classes.Adapters
{
    public class MarkersListAdapter : BaseAdapter<Marker>
    {
        readonly Activity _context;
        List<Marker> _items;

        public MarkersListAdapter(Activity context, List<Marker> items)
        {
            _context = context;
            _items = items;
        }

        public void SetData(IEnumerable<Marker> items)
        {
            _items = items.ToList();
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Marker this[int position]
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
                view = _context.LayoutInflater.Inflate(Resource.Layout.MarkerCell, null);

            var txtName = view.FindViewById<TextView>(Resource.Id.markerCell_name);
            var txtPosition = view.FindViewById<TextView>(Resource.Id.markerCell_position);
            var txtLetter = view.FindViewById<TextView>(Resource.Id.markerCell_letter);
            txtName.Text = item.Name;
            txtPosition.Text = item.Position;
            txtLetter.Text = Conversion.IndexToLetter(position).ToString();

            return view;
        }
    }
}
