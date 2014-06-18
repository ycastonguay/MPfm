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
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using org.sessionsapp.android;

namespace MPfm.Android.Classes.Adapters
{
    public class PlaylistItemListAdapter : BaseAdapter<AudioFile>, View.IOnClickListener, View.IOnTouchListener
    {
        readonly PlaylistActivity _context;
        readonly CustomListView _listView;
        Playlist _playlist;
        int _nowPlayingRowPosition;
        int _editingRowPosition;
        Guid _nowPlayingAudioFileId;

        public bool IsEditingRow { get; private set; }

        public PlaylistItemListAdapter(PlaylistActivity context, CustomListView listView, Playlist playlist)
        {
            _context = context;
            _listView = listView;
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
                view = _context.LayoutInflater.Inflate(Resource.Layout.PlaylistItemCell, null);

            if (item == null)
                return view;

            view.Tag = position;
            //view.SetOnTouchListener(this);

            var index = view.FindViewById<TextView>(Resource.Id.playlistItemCell_lblIndex);
            var title = view.FindViewById<TextView>(Resource.Id.playlistItemCell_lblTitle);
            var subtitle = view.FindViewById<TextView>(Resource.Id.playlistItemCell_lblSubtitle);
            var imageNowPlaying = view.FindViewById<ImageView>(Resource.Id.playlistItemCell_imageNowPlaying);
            
            //index.Text = item.AudioFile.TrackNumber.ToString();
            index.Text = (position+1).ToString();
            title.Text = item.AudioFile.ArtistName + " / " + item.AudioFile.Title;
            subtitle.Text = item.AudioFile.Length;

            if (item.AudioFile != null && item.AudioFile.Id == _nowPlayingAudioFileId)
                imageNowPlaying.Visibility = ViewStates.Visible;
            else
                imageNowPlaying.Visibility = ViewStates.Gone;

            return view;
        }

        public void SetNowPlayingRow(int position, AudioFile audioFile)
        {
            int oldPosition = _nowPlayingRowPosition;
            _nowPlayingAudioFileId = audioFile == null ? Guid.Empty : audioFile.Id;
            _nowPlayingRowPosition = audioFile == null ? -1 : position;

            var viewOldPosition = _listView.GetChildAt(oldPosition - _listView.FirstVisiblePosition);
            if (viewOldPosition != null)
            {
                var imageNowPlaying = viewOldPosition.FindViewById<ImageView>(Resource.Id.playlistItemCell_imageNowPlaying);
                imageNowPlaying.Visibility = ViewStates.Gone;
            }

            var view = _listView.GetChildAt(position - _listView.FirstVisiblePosition);
            if (view == null)
                return;

            if (_playlist.Items[position].AudioFile != null && _playlist.Items[position].AudioFile.Id == _nowPlayingAudioFileId)
            {
                var imageNowPlaying = view.FindViewById<ImageView>(Resource.Id.playlistItemCell_imageNowPlaying);
                imageNowPlaying.Visibility = ViewStates.Visible;
            }
        }

        public void SetEditingRow(int position)
        {
            int visibleCellIndex = position - _listView.FirstVisiblePosition;
            var view = _listView.GetChildAt(visibleCellIndex);
            if (view == null)
                return;

            var imagePlay = view.FindViewById<ImageView>(Resource.Id.playlistItemCell_imagePlay);
            var imageDelete = view.FindViewById<ImageView>(Resource.Id.playlistItemCell_imageDelete);

            int oldPosition = _editingRowPosition;
            _editingRowPosition = position;

            if (IsEditingRow && oldPosition == position)
            {
                // Fade out the controls
                Animation anim = AnimationUtils.LoadAnimation(_context, Resource.Animation.listviewoptions_fade_out);
                anim.AnimationEnd += (sender, args) =>
                {
                    imagePlay.Visibility = ViewStates.Gone;
                    imageDelete.Visibility = ViewStates.Gone;
                };
                imagePlay.StartAnimation(anim);
                imageDelete.StartAnimation(anim);

                _editingRowPosition = -1;
                IsEditingRow = false;
            }
            else if (IsEditingRow && oldPosition >= 0)
            {
                // Fade in the new controls
                imagePlay.Visibility = ViewStates.Visible;
                imageDelete.Visibility = ViewStates.Visible;
                Animation anim = AnimationUtils.LoadAnimation(_context, Resource.Animation.listviewoptions_fade_in);
                imagePlay.StartAnimation(anim);
                imageDelete.StartAnimation(anim);

                // Fade out the older controls
                int oldPositionVisibleCellIndex = oldPosition - _listView.FirstVisiblePosition;
                var viewOldPosition = _listView.GetChildAt(oldPositionVisibleCellIndex);
                if (viewOldPosition != null)
                {
                    var imagePlayOld = viewOldPosition.FindViewById<ImageView>(Resource.Id.playlistItemCell_imagePlay);
                    var imageDeleteOld = viewOldPosition.FindViewById<ImageView>(Resource.Id.playlistItemCell_imageDelete);

                    // Fade out the controls
                    Animation animOld = AnimationUtils.LoadAnimation(_context, Resource.Animation.listviewoptions_fade_out);
                    animOld.AnimationEnd += (sender, args) =>
                    {
                        imagePlayOld.Visibility = ViewStates.Gone;
                        imageDeleteOld.Visibility = ViewStates.Gone;
                    };
                    imagePlayOld.StartAnimation(animOld);
                    imageDeleteOld.StartAnimation(animOld);
                }

                IsEditingRow = true;
            }
            else if (!IsEditingRow)
            {
                // Fade in the controls
                imagePlay.Visibility = ViewStates.Visible;
                imageDelete.Visibility = ViewStates.Visible;
                Animation anim = AnimationUtils.LoadAnimation(_context, Resource.Animation.listviewoptions_fade_in);
                imagePlay.StartAnimation(anim);
                imageDelete.StartAnimation(anim);

                IsEditingRow = true;
            }
        }

        public void OnClick(View v)
        {
            int position = (int)v.Tag;
            switch (v.Id)
            {
                case Resource.Id.playlistItemCell_imagePlay:
                    Console.WriteLine("playlistItemCell - Play - position: {0}", position);
                    //_fragment.OnPlayItem(position);
                    break;
                case Resource.Id.playlistItemCell_imageDelete:
                    Console.WriteLine("playlistItemCell - Delete - position: {0}", position);
                    //AlertDialog ad = new AlertDialog.Builder(_context)
                    //    .SetIconAttribute(global::Android.Resource.Attribute.AlertDialogIcon)
                    //    .SetTitle("Delete confirmation")
                    //    .SetMessage(string.Format("Are you sure you wish to delete {0}?", _items[position].Title))
                    //    .SetCancelable(true)
                    //    .SetPositiveButton("OK", (sender, args) => _fragment.OnDeleteItem(position))
                    //    .SetNegativeButton("Cancel", (sender, args) => { })
                    //    .Create();
                    //ad.Show();
                    break;
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            //Console.WriteLine("PlaylistItemListAdapter - OnTouch - action: {0} buttonState: {1} downTime: {2} eventTime: {3} x: {4} y: {5}", e.Action, e.ButtonState, e.DownTime, e.EventTime, e.GetX(), e.GetY());

            //float x = e.GetX();
            //float y = e.GetY();

            //// Keep cancel on top because the flag can contain both move and cancel.
            //if (e.Action.HasFlag(MotionEventActions.Cancel))
            //{
            //    Console.WriteLine("PlaylistItemListAdapter - OnTouch - Cancel - (x,y): ({0},{1})", x, y);
            //    _listView.IsScrollable = true;
            //}
            //else if (e.Action.HasFlag(MotionEventActions.Move))
            //{
            //    Console.WriteLine("PlaylistItemListAdapter - OnTouch - Move - (x,y): ({0},{1})", x, y);
            //    _listView.IsScrollable = false;
            //}

            ////Console.WriteLine("PlaylistItemListAdapter - OnTouch - Current cell - height: {0} - position: {1}", v.Height, (int)v.Tag);

            return true;
        }        
    }
}
