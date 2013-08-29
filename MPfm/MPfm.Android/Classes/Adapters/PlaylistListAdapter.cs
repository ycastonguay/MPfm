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
using Android.Views.Animations;
using Android.Widget;
using MPfm.Library.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;

namespace MPfm.Android.Classes.Adapters
{
    public class PlaylistListAdapter : BaseAdapter<AudioFile>
    {
        readonly PlaylistActivity _context;
        Playlist _playlist;

        public PlaylistListAdapter(PlaylistActivity context, Playlist playlist)
        {
            _context = context;
            _playlist = playlist;
        }

        public void SetData(Playlist playlist)
        {
            _playlist = playlist;
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override AudioFile this[int position]
        {
            get { return _playlist.Items[position].AudioFile; }
        }

        public override int Count
        {
            get { return _playlist.Items.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _playlist.Items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.PlaylistCell, null);

            if (item == null)
                return view;

            var index = view.FindViewById<TextView>(Resource.Id.playlistCell_lblIndex);
            var title = view.FindViewById<TextView>(Resource.Id.playlistCell_lblTitle);
            var subtitle = view.FindViewById<TextView>(Resource.Id.playlistCell_lblSubtitle);
            
            //index.Text = item.AudioFile.TrackNumber.ToString();
            index.Text = (position+1).ToString();
            title.Text = item.AudioFile.ArtistName + " / " + item.AudioFile.Title;
            subtitle.Text = item.AudioFile.Length;

            return view;
        }
    }
}
