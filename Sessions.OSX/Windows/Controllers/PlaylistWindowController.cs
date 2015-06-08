// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.AppKit;
using Sessions.MVP.Views;
using Sessions.OSX.Classes.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.OSX.Classes.Helpers;
using Sessions.Sound.Player;

namespace Sessions.OSX
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
            ShowWindowCentered();
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            LoadFontsAndImages();
            OnViewReady(this);
        }

        private void LoadFontsAndImages()
        {
            viewTitle.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewTitle.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewToolbar.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewToolbar.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;

            lblTitle.Font = NSFont.FromFontName("Roboto Light", 16f);

            btnToolbarNew.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_new");
            btnToolbarOpen.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_open");
            btnToolbarSave.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_save");
            btnToolbarSaveAs.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_save");
        }

        #region IPlaylistView implementation

        public Action<int, int> OnChangePlaylistItemOrder { get; set; }
        public Action<int> OnSelectPlaylistItem { get; set; }
        public Action<List<int>> OnRemovePlaylistItems { get; set; }
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
            _playlist = playlist;
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
//                        var customCell = (SessionsTableViewCell)cell;
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
