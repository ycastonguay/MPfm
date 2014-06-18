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
using MPfm.Library.Objects;
using MPfm.MVP.Models;
using MPfm.Player.Objects;

namespace MPfm.Android.Classes.Adapters
{
    public class ResumePlaybackListAdapter : BaseAdapter<ResumePlaybackEntity>
    {
        readonly Activity _context;
        List<ResumePlaybackEntity> _items;

        public ResumePlaybackListAdapter(Activity context, List<ResumePlaybackEntity> items)
        {
            _context = context;
            _items = items;
        }

        public void SetData(IEnumerable<ResumePlaybackEntity> items)
        {
            _items = items.ToList();
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override ResumePlaybackEntity this[int position]
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
                view = _context.LayoutInflater.Inflate(Resource.Layout.ResumePlaybackCell, null);

            var lblTitle = view.FindViewById<TextView>(Resource.Id.resumePlaybackCell_lblTitle);
            var lblSubtitle = view.FindViewById<TextView>(Resource.Id.resumePlaybackCell_lblSubtitle);
            var lblArtistName = view.FindViewById<TextView>(Resource.Id.resumePlaybackCell_lblArtistName);
            var lblAlbumTitle = view.FindViewById<TextView>(Resource.Id.resumePlaybackCell_lblAlbumTitle);
            var lblSongTitle = view.FindViewById<TextView>(Resource.Id.resumePlaybackCell_lblSongTitle);
            var lblTimestamp = view.FindViewById<TextView>(Resource.Id.resumePlaybackCell_lblTimestamp);
            var imageAlbum = view.FindViewById<ImageView>(Resource.Id.resumePlaybackCell_imageAlbum);

            lblTitle.Text = item.DeviceInfo.DeviceName;
            lblSubtitle.Text = "On-the-fly playlist";
            lblArtistName.Text = item.DeviceInfo.ArtistName;
            lblAlbumTitle.Text = item.DeviceInfo.AlbumTitle;
            lblSongTitle.Text = item.DeviceInfo.SongTitle;
            lblTimestamp.Text = string.Format("Last updated on {0}", item.DeviceInfo.Timestamp);

            return view;
        }
    }
}
