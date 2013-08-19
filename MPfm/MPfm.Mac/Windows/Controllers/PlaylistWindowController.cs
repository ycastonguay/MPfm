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
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.MVP.Views;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.Mac.Classes.Objects;
using MPfm.Sound.Playlists;
using MPfm.Sound.AudioFiles;
using MPfm.Mac.Classes.Helpers;

namespace MPfm.Mac
{
    public partial class PlaylistWindowController : BaseWindowController, IPlaylistView
    {
        Guid _currentlyPlayingSongId;
        Playlist _playlist = new Playlist();

        public PlaylistWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        public PlaylistWindowController(Action<IBaseView> onViewReady)
            : base ("PlaylistWindow", onViewReady)
        {
            Initialize();
        }
        
        private void Initialize()
        {
            this.Window.Center();
            this.Window.MakeKeyAndOrderFront(this);
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            tableView.WeakDelegate = this;
            tableView.WeakDataSource = this;
            LoadImages();
            OnViewReady.Invoke(this);
        }

        private void LoadImages()
        {
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarNewPlaylist").Image = ImageResources.images32x32[11];
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarLoadPlaylist").Image = ImageResources.images32x32[0];
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarSavePlaylist").Image = ImageResources.images32x32[12];
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarSaveAsPlaylist").Image = ImageResources.images32x32[13];
        }

        partial void actionNewPlaylist(NSObject sender)
        {
        }

        partial void actionLoadPlaylist(NSObject sender)
        {
        }

        partial void actionSavePlaylist(NSObject sender)
        {
        }

        partial void actionSaveAsPlaylist(NSObject sender)
        {
        }

        [Export ("numberOfRowsInTableView:")]
        public int GetRowCount(NSTableView tableView)
        {
            return _playlist.Items.Count;
        }

        [Export ("tableView:dataCellForTableColumn:row:")]
        public NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            return new NSString();
        }

        [Export ("tableView:viewForTableColumn:row:")]
        public NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            NSTableCellView view = null;
            if(tableColumn.Identifier.ToString() == "columnNowPlaying")
            {
                view = (NSTableCellView)tableView.MakeView("cellNowPlaying", this);
                view.TextField.StringValue = string.Empty;
            }
            else if(tableColumn.Identifier.ToString() == "columnTitle")
            {
                view = (NSTableCellView)tableView.MakeView("cellTitle", this);
                view.TextField.StringValue = _playlist.Items[row].AudioFile.Title;
            }
            else if(tableColumn.Identifier.ToString() == "columnLength")
            {
                view = (NSTableCellView)tableView.MakeView("cellLength", this);
                view.TextField.StringValue = _playlist.Items[row].LengthString;
            }
            else if(tableColumn.Identifier.ToString() == "columnArtistName")
            {
                view = (NSTableCellView)tableView.MakeView("cellArtistName", this);
                view.TextField.StringValue = _playlist.Items[row].AudioFile.ArtistName;
            }
            else if(tableColumn.Identifier.ToString() == "columnAlbumTitle")
            {
                view = (NSTableCellView)tableView.MakeView("cellAlbumTitle", this);
                view.TextField.StringValue = _playlist.Items[row].AudioFile.AlbumTitle;
            }

            view.TextField.Font = NSFont.FromFontName("Junction", 11);
//            if (view.ImageView != null)
//                view.ImageView.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_android");

            return view;
        }

        #region IPlaylistView implementation

        public Action<Guid, int> OnChangePlaylistItemOrder { get; set; }
        public Action<Guid> OnSelectPlaylistItem { get; set; }
        public Action<Guid> OnRemovePlaylistItem { get; set; }
        public Action OnNewPlaylist { get; set; }
        public Action<string> OnLoadPlaylist { get; set; }
        public Action OnSavePlaylist { get; set; }
        public Action OnShufflePlaylist { get; set; }

        public void PlaylistError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in Playlist: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshPlaylist(Playlist playlist)
        {
            Console.WriteLine("PlaylistWindowController - RefreshPlaylist");
            InvokeOnMainThread(() => {
                _playlist = playlist;
                tableView.ReloadData();
            });
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
            Console.WriteLine("PlaylistWindowController - RefreshCurrentlyPlayingSong index: {0} audioFile: {1}", index, audioFile.FilePath);

            if (audioFile != null)
                _currentlyPlayingSongId = audioFile.Id;
            else
                _currentlyPlayingSongId = Guid.Empty;

            InvokeOnMainThread(() => {
//                foreach(var cell in tableView.VisibleCells)
//                {
//                    if(_playlist.Items[cell.Tag].AudioFile != null)
//                    {
//                        var id = _playlist.Items[cell.Tag].AudioFile.Id;
//                        var customCell = (MPfmTableViewCell)cell;
//                        if(id == audioFile.Id)
//                            customCell.RightImage.Hidden = false;
//                        else
//                            customCell.RightImage.Hidden = true;
//                    }
//                }
            });
        }

        #endregion

    }
}
