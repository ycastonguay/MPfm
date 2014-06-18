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

using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Sessions.MVP.Models;

namespace MPfm.Android.Classes.Adapters
{
    public class SyncMenuListAdapter : BaseAdapter<SyncMenuItemEntity>, View.IOnClickListener
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
            //Console.WriteLine("SyncMenuListAdapter - GetView - position {0}", position);
            var item = _items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.SyncMenuCell, null);

            var title = view.FindViewById<TextView>(Resource.Id.syncMenuCell_title);
            var index = view.FindViewById<TextView>(Resource.Id.syncMenuCell_index);
            var image = view.FindViewById<ImageView>(Resource.Id.syncMenuCell_image);            
            var checkmark = view.FindViewById<ImageView>(Resource.Id.syncMenuCell_checkmark);
            checkmark.Tag = position;
            checkmark.SetOnClickListener(this);

            if (item.Selection == StateSelectionType.Selected)
                checkmark.SetImageResource(Resource.Drawable.checkbox_checked);
            else if (item.Selection == StateSelectionType.PartlySelected)
                checkmark.SetImageResource(Resource.Drawable.checkbox_partial);
            else
                checkmark.SetImageResource(Resource.Drawable.checkbox_unchecked);

            switch (_items[position].ItemType)
            {
                case SyncMenuItemEntityType.Artist:
                    title.Text = item.ArtistName;
                    title.SetTextSize(ComplexUnitType.Sp, 16);
                    index.Visibility = ViewStates.Gone;                                   
                    image.SetImageResource(Resource.Drawable.icon_artist);
                    image.Visibility = ViewStates.Visible;
                    break;
                case SyncMenuItemEntityType.Album:                    
                    title.Text = item.AlbumTitle;
                    title.SetTextSize(ComplexUnitType.Sp, 14);
                    index.Visibility = ViewStates.Gone;
                    image.SetImageResource(Resource.Drawable.icon_vinyl);
                    image.Visibility = ViewStates.Visible;
                    break;
                case SyncMenuItemEntityType.Song:
                    title.Text = item.Song.Title;
                    title.SetTextSize(ComplexUnitType.Sp, 14);
                    index.Visibility = ViewStates.Visible;
                    index.Text = item.Song.TrackNumber.ToString();
                    image.SetImageResource(0);
                    image.Visibility = ViewStates.Gone;
                    break;
            }

            return view;
        }

        public void OnClick(View v)
        {
            int position = (int)v.Tag;
            var item = _items[position];
            var checkmark = (ImageView) v;
            Console.WriteLine("Click on checkmark {0} tag {1}", position, v.Tag);
            _context.OnSelectItems(new List<SyncMenuItemEntity>(){ item });

            if (item.Selection == StateSelectionType.Selected)
                checkmark.SetImageResource(Resource.Drawable.checkbox_checked);
            else if (item.Selection == StateSelectionType.PartlySelected)
                checkmark.SetImageResource(Resource.Drawable.checkbox_partial);
            else
                checkmark.SetImageResource(Resource.Drawable.checkbox_unchecked);
        }
    }
}
