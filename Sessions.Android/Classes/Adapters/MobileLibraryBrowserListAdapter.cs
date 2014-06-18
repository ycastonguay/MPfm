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
using System.Linq;
using System.Net.Mime;
using Android.App;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using MPfm.Android.Classes.Fragments;
using MPfm.MVP.Models;
using MPfm.Sound.AudioFiles;

namespace MPfm.Android.Classes.Adapters
{
    public class MobileLibraryBrowserListAdapter : BaseAdapter<LibraryBrowserEntity>, View.IOnClickListener
    {
        readonly Activity _context;
        readonly MobileLibraryBrowserFragment _fragment;
        readonly ListView _listView;
        List<LibraryBrowserEntity> _items;
        int _editingRowPosition = -1;
        int _nowPlayingRowPosition = -1;
        Guid _nowPlayingAudioFileId = Guid.Empty;

        public bool IsEditingRow { get; private set; }

        public MobileLibraryBrowserListAdapter(Activity context, MobileLibraryBrowserFragment fragment, ListView listView, List<LibraryBrowserEntity> items)
        {
            _context = context;
            _fragment = fragment;
            _listView = listView;
            _items = items;
        }

        public void SetData(IEnumerable<LibraryBrowserEntity> items)
        {
            _editingRowPosition = -1;
            IsEditingRow = false;
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

            var layoutAlbums = view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowserCell_layoutAlbums);
            var layoutSubtitle = view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowserCell_layoutSubtitle);
            var layoutMenu = view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowserCell_layoutMenu);
            var lblTitle = view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowserCell_lblTitle);
            var lblTitleWithSubtitle = view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowserCell_lblTitleWithSubtitle);
            var lblSubtitle = view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowserCell_lblSubtitle);
            var index = view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowserCell_index);
            var imageNowPlaying = view.FindViewById<ImageView>(Resource.Id.mobileLibraryBrowserCell_imageNowPlaying);
            var btnAdd = view.FindViewById<ImageButton>(Resource.Id.mobileLibraryBrowserCell_imageAdd);
            var btnPlay = view.FindViewById<ImageButton>(Resource.Id.mobileLibraryBrowserCell_imagePlay);
            var btnDelete = view.FindViewById<ImageButton>(Resource.Id.mobileLibraryBrowserCell_imageDelete);
            var layoutAlbumCount = view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowserCell_layoutAlbumCount);
            var lblAlbumCount = view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowserCell_lblAlbumCount);
            var imageAlbum1 = view.FindViewById<ImageView>(Resource.Id.mobileLibraryBrowserCell_imageAlbum1);
            var imageAlbum2 = view.FindViewById<ImageView>(Resource.Id.mobileLibraryBrowserCell_imageAlbum2);
            var imageAlbum3 = view.FindViewById<ImageView>(Resource.Id.mobileLibraryBrowserCell_imageAlbum3);

            if(item.AudioFile != null && item.AudioFile.Id == _nowPlayingAudioFileId)
                imageNowPlaying.Visibility = ViewStates.Visible;
            else
                imageNowPlaying.Visibility = ViewStates.Gone;

            btnAdd.Tag = position;
            btnPlay.Tag = position;
            btnDelete.Tag = position;
            btnAdd.SetOnClickListener(this);
            btnPlay.SetOnClickListener(this);
            btnDelete.SetOnClickListener(this);

            if (IsEditingRow && position == _editingRowPosition)
                layoutMenu.Visibility = ViewStates.Visible;
            else
                layoutMenu.Visibility = ViewStates.Gone;

            lblTitle.Text = item.Title;
            lblTitleWithSubtitle.Text = item.Title;
            lblSubtitle.Text = string.Empty;
            if (item.AudioFile != null)
            {
                index.Text = item.AudioFile.TrackNumber.ToString();
                lblSubtitle.Text = item.AudioFile.Length;
            }

            switch (item.EntityType)
            {
                case LibraryBrowserEntityType.Song:
                    layoutAlbums.Visibility = ViewStates.Gone;
                    index.Visibility = ViewStates.Visible;
                    lblTitle.Visibility = ViewStates.Gone;            
                    layoutSubtitle.Visibility = ViewStates.Visible;
                    break;
                default:
                    layoutAlbums.Visibility = ViewStates.Visible;
                    index.Visibility = ViewStates.Gone;
                    lblTitle.Visibility = ViewStates.Visible;            
                    layoutSubtitle.Visibility = ViewStates.Gone;
                    break;
            }

            imageAlbum1.SetImageBitmap(null);
            imageAlbum2.SetImageBitmap(null);
            imageAlbum3.SetImageBitmap(null);

            lblAlbumCount.Text = string.Format("+{0}", item.AlbumTitles.Count - 3);
            imageAlbum1.Visibility = item.AlbumTitles.Count >= 1 ? ViewStates.Visible : ViewStates.Gone;
            imageAlbum2.Visibility = item.AlbumTitles.Count >= 2 ? ViewStates.Visible : ViewStates.Gone;
            imageAlbum3.Visibility = item.AlbumTitles.Count >= 3 ? ViewStates.Visible : ViewStates.Gone;
            layoutAlbumCount.Visibility = item.AlbumTitles.Count >= 4 ? ViewStates.Visible : ViewStates.Gone;

            int albumFetchCount = item.AlbumTitles.Count >= 3 ? 3 : item.AlbumTitles.Count;
            for (int a = 0; a < albumFetchCount; a++)
            {
                ImageView imageAlbum = null;
                if (a == 0)
                    imageAlbum = imageAlbum1;
                else if (a == 1)
                    imageAlbum = imageAlbum2;
                else if (a == 2)
                    imageAlbum = imageAlbum3;

                string bitmapKey = item.Query.ArtistName + "_" + item.AlbumTitles[a];
                imageAlbum.Tag = bitmapKey;

                var viewHolder = new ListAlbumCellViewHolder();
                viewHolder.imageView = imageAlbum;
                viewHolder.position = position;
                viewHolder.key = bitmapKey;
                //view.Tag = viewHolder;

                Console.WriteLine("MLBLA - GetView - bitmapKey: {0}", bitmapKey);
                if (_fragment.SmallBitmapCache.KeyExists(bitmapKey))
                    imageAlbum.SetImageBitmap(_fragment.SmallBitmapCache.GetBitmapFromMemoryCache(bitmapKey));
                else
                    _fragment.OnRequestAlbumArt(item.Query.ArtistName, item.AlbumTitles[a], viewHolder);
            }

            return view;
        }

        public void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData, object userData)
        {
            try
            {
                var viewHolder = (ListAlbumCellViewHolder) userData;
                Console.WriteLine("MobileLibraryBrowserListAdapter - RefreshAlbumArtCell - key: {0}/{1} - imageView.Tag: {2}", artistName, albumTitle, viewHolder.imageView.Tag.ToString());
                if (viewHolder.imageView.Tag.ToString() == artistName + "_" + albumTitle)
                    _fragment.SmallBitmapCache.LoadBitmapFromByteArray(albumArtData, artistName + "_" + albumTitle, viewHolder.imageView);
                else
                    Console.WriteLine("MobileLibraryBrowserListAdapter - RefreshAlbumArtCell - ITEM DID NOT MATCH - key: {0}/{1} - imageView.Tag: {2}", artistName, albumTitle, viewHolder.imageView.Tag.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("MobileLibraryBrowserListAdapter - Failed to load album art: {0}", ex);
            }
        }

        public void SetNowPlayingRow(int position, AudioFile audioFile)
        {
            int oldPosition = _nowPlayingRowPosition;
            _nowPlayingAudioFileId = audioFile == null ? Guid.Empty : audioFile.Id;
            _nowPlayingRowPosition = audioFile == null ? -1 : position;

            var viewOldPosition = _listView.GetChildAt(oldPosition - _listView.FirstVisiblePosition);
            if (viewOldPosition != null)
            {
                var imageNowPlaying = viewOldPosition.FindViewById<ImageView>(Resource.Id.mobileLibraryBrowserCell_imageNowPlaying);
                imageNowPlaying.Visibility = ViewStates.Gone;
            }
            
            var view = _listView.GetChildAt(position - _listView.FirstVisiblePosition);
            if (view == null)
                return;
            
            if (_items[position].AudioFile != null && _items[position].AudioFile.Id == _nowPlayingAudioFileId)
            {
                var imageNowPlaying = view.FindViewById<ImageView>(Resource.Id.mobileLibraryBrowserCell_imageNowPlaying);
                imageNowPlaying.Visibility = ViewStates.Visible;
            }
        }

        public void ResetEditingRow()
        {
            int visibleCellIndex = _editingRowPosition - _listView.FirstVisiblePosition;
            var view = _listView.GetChildAt(visibleCellIndex);
            if (view == null)
                return;

            var layoutMenu = view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowserCell_layoutMenu);

            // Fade out the controls
            Animation anim = AnimationUtils.LoadAnimation(_context, Resource.Animation.listviewoptions_fade_out);
            anim.AnimationEnd += (sender, args) => {
                layoutMenu.Visibility = ViewStates.Gone;
            };
            layoutMenu.StartAnimation(anim);

            _editingRowPosition = -1;
            IsEditingRow = false;
        }

        public void SetEditingRow(int position)
        {
            int visibleCellIndex = position - _listView.FirstVisiblePosition;
            var view = _listView.GetChildAt(visibleCellIndex);
            if (view == null)
                return;

            var layoutMenu = view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowserCell_layoutMenu);
            int oldPosition = _editingRowPosition;
            _editingRowPosition = position;

            if(IsEditingRow && oldPosition == position)
            {
                ResetEditingRow();
            }
            else if (IsEditingRow && oldPosition >= 0)
            {
                // Fade in the new controls
                layoutMenu.Visibility = ViewStates.Visible;
                Animation anim = AnimationUtils.LoadAnimation(_context, Resource.Animation.listviewoptions_fade_in);
                layoutMenu.StartAnimation(anim);

                // Fade out the older controls
                int oldPositionVisibleCellIndex = oldPosition - _listView.FirstVisiblePosition;
                var viewOldPosition = _listView.GetChildAt(oldPositionVisibleCellIndex);
                if (viewOldPosition != null)
                {
                    var layoutMenuOld = viewOldPosition.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowserCell_layoutMenu);

                    // Fade out the controls
                    Animation animOld = AnimationUtils.LoadAnimation(_context, Resource.Animation.listviewoptions_fade_out);
                    animOld.AnimationEnd += (sender, args) => {
                        layoutMenuOld.Visibility = ViewStates.Gone;
                    };
                    layoutMenuOld.StartAnimation(animOld);
                }

                IsEditingRow = true;
            }
            else if (!IsEditingRow)
            {
                // Fade in the controls
                layoutMenu.Visibility = ViewStates.Visible;
                Animation anim = AnimationUtils.LoadAnimation(_context, Resource.Animation.listviewoptions_fade_in);
                layoutMenu.StartAnimation(anim);

                IsEditingRow = true;
            }
        }

        public void OnClick(View v)
        {
            int position = (int)v.Tag;
            switch(v.Id)
            {
                case Resource.Id.mobileLibraryBrowserCell_imageAdd:
                    Console.WriteLine("MLBLA - ADD - position: {0}", position);
                    _fragment.AddItemToPlaylist(position);
                    break;
                case Resource.Id.mobileLibraryBrowserCell_imagePlay:
                    Console.WriteLine("MLBLA - PLAY - position: {0}", position);
                    _fragment.OnPlayItem(_items[position].Id);
                    break;
                case Resource.Id.mobileLibraryBrowserCell_imageDelete:
                    Console.WriteLine("MLBLA - DELETE - position: {0}", position);
                    AlertDialog ad = new AlertDialog.Builder(_context)
                        .SetIconAttribute(global::Android.Resource.Attribute.AlertDialogIcon)
                        .SetTitle("Delete confirmation")
                        .SetMessage(string.Format("Are you sure you wish to delete {0}?", _items[position].Title))
                        .SetCancelable(true)
                        .SetPositiveButton("OK", (sender, args) => _fragment.OnDeleteItem(_items[position].Id))
                        .SetNegativeButton("Cancel", (sender, args) => {})
                        .Create();
                    ad.Show();
                    break;
            }

            ResetEditingRow();
        }
    }

    public class ListAlbumCellViewHolder : Java.Lang.Object
    {
        public ImageView imageView;
        public LibraryBrowserEntity entity;
        public int position;
        public string key;
    }
}
