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
using Android.Views;
using Android.Widget;
using MPfm.MVP.Models;

namespace MPfm.Android.Classes.Adapters
{
    public class MobileLibraryBrowserListAdapter : BaseAdapter<LibraryBrowserEntity>
    {
        readonly Activity _context;
        List<LibraryBrowserEntity> _items;

        public MobileLibraryBrowserListAdapter(Activity context, List<LibraryBrowserEntity> items)
        {
            _context = context;
            _items = items;
        }

        public void SetData(IEnumerable<LibraryBrowserEntity> items)
        {
            _items = items.ToList();
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override LibraryBrowserEntity this[int position]
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
                view = _context.LayoutInflater.Inflate(Resource.Layout.MobileLibraryBrowserCell, null);

            var layoutSubtitle = view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowserCell_layoutSubtitle);
            var lblTitle = view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowserCell_lblTitle);
            var lblTitleWithSubtitle = view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowserCell_lblTitleWithSubtitle);
            var lblSubtitle = view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowserCell_lblSubtitle);
            var index = view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowserCell_index);
            var imageNowPlaying = view.FindViewById<ImageView>(Resource.Id.mobileLibraryBrowserCell_imageNowPlaying);
            imageNowPlaying.Visibility = ViewStates.Invisible;

            lblTitle.Text = item.Title;
            lblTitleWithSubtitle.Text = item.Title;
            lblSubtitle.Text = string.Empty;
            if (item.AudioFile != null)
            {
                index.Text = item.AudioFile.TrackNumber.ToString();
                lblSubtitle.Text = item.AudioFile.Length;
            }

            switch (item.Type)
            {
                case LibraryBrowserEntityType.Song:
                    index.Visibility = ViewStates.Visible;
                    lblTitle.Visibility = ViewStates.Gone;            
                    layoutSubtitle.Visibility = ViewStates.Visible;
                    break;
                default:
                    index.Visibility = ViewStates.Gone;
                    lblTitle.Visibility = ViewStates.Visible;            
                    layoutSubtitle.Visibility = ViewStates.Gone;
                    break;
            }

            return view;
        }
    }
}
