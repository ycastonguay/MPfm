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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Sessions.Player;
using Sessions.WPF.Classes.Windows.Base;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;

namespace Sessions.WPF.Classes.Windows
{
    public partial class PlaylistWindow : BaseWindow, IPlaylistView
    {
        private SSPPlaylist _playlist;

        public PlaylistWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        #region IPlaylistView implementation

        public Action<Guid, int> OnChangePlaylistItemOrder { get; set; }
        public Action<Guid> OnSelectPlaylistItem { get; set; }
        public Action<List<Guid>> OnRemovePlaylistItems { get; set; }
        public Action OnNewPlaylist { get; set; }
        public Action<string> OnLoadPlaylist { get; set; }
        public Action OnSavePlaylist { get; set; }
        public Action OnShufflePlaylist { get; set; }
        
        public void PlaylistError(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in Playlist: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void RefreshPlaylist(SSPPlaylist playlist)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                _playlist = playlist;
                listViewPlaylist.Items.Clear();
                listViewPlaylistAlbumArt.Items.Clear();
                string currentAlbum = string.Empty;
                int songCount = 0;
                //foreach (var item in _playlist.Items)
                //{
                //    // Add only one row per album (to do: expose row height in viewmodel)
                //    songCount++;                                            
                //    string album = string.Format("{0}_{1}", item.AudioFile.ArtistName, item.AudioFile.AlbumTitle).ToUpper();
                //    if (string.IsNullOrEmpty(currentAlbum))
                //    {
                //        currentAlbum = album;
                //    }
                //    else if (album != currentAlbum)
                //    {
                //        //Console.WriteLine("PlaylistWindow - RefreshPlaylists - Album: {0} SongCount: {1}", album, songCount);
                //        var listViewItem = new ListViewItem();
                //        listViewItem.Background = new LinearGradientBrush(Colors.HotPink, Colors.Yellow, 90);
                //        listViewItem.Height = (songCount - 1) * 24;
                //        listViewItem.Content = string.Format("{0}/{1}", (songCount - 1), currentAlbum);
                //        listViewPlaylistAlbumArt.Items.Add(listViewItem);
                //        currentAlbum = album;
                //        songCount = 1;
                //    }

                //    listViewPlaylist.Items.Add(item);
                //}
            }));
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
        }

        #endregion

        private void ListViewPlaylist_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //Console.WriteLine("SCROLL CHANGE {0} {1} {2} {3}", e.ExtentHeight, e.ExtentHeightChange, e.ViewportHeight, e.ViewportHeightChange);
            Console.WriteLine("SCROLL CHANGE {0} {1} {2} {3}", e.HorizontalChange, e.HorizontalOffset, e.VerticalChange,
                e.VerticalOffset);

            // e.VerticalChange = +/- number of rows gone
            //listViewPlaylistAlbumArt.ScrollIntoView();   
        }

        private void ListViewPlaylistAlbumArt_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Console.WriteLine("SCROLL CHANGE {0} {1} {2} {3}", e.ExtentHeight, e.ExtentHeightChange, e.ViewportHeight, e.ViewportHeightChange);
            //listViewPlaylist.ScrollIntoView();
        }
    }
}
