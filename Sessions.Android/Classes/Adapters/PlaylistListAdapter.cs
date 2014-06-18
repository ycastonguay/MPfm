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
using Android.App;
using Android.Views;
using Android.Widget;
using Sessions.MVP.Models;

namespace MPfm.Android.Classes.Adapters
{
    public class PlaylistListAdapter : BaseAdapter<PlaylistEntity>
    {
        readonly Activity _context;
        readonly ListView _listView;
        List<PlaylistEntity> _playlists;

        public PlaylistListAdapter(Activity context, ListView listView, List<PlaylistEntity> playlists)
        {
            _context = context;
            _listView = listView;
            _playlists = playlists;
        }

        public void SetData(List<PlaylistEntity> playlists)
        {
            _playlists = playlists;
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override PlaylistEntity this[int position]
        {
            get { return _playlists[position]; }
        }

        public override int Count
        {
            get { return _playlists.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _playlists[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.PlaylistCell, null);

            var title = view.FindViewById<TextView>(Resource.Id.playlistcell_title);
            var count = view.FindViewById<TextView>(Resource.Id.playlistcell_count);
            title.Text = _playlists[position].Name;
            count.Text = string.Format("{0}", _playlists[position].ItemCount);

            return view;
        }
    }
}
