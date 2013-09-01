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
using MPfm.MVP;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using System.Collections.Generic;

namespace MPfm.GTK.Windows
{
	/// <summary>
	/// Playlist window.
	/// </summary>
	public partial class PlaylistWindow : BaseWindow, IPlaylistView
	{
		public PlaylistWindow(Action<IBaseView> onViewReady) : 
				base(Gtk.WindowType.Toplevel, onViewReady)
		{
			this.Build();
            onViewReady(this);
            treePlaylistBrowser.GrabFocus(); // the list view changes color when focused by default. it annoys me!
            this.Center();
            this.Show();
		}
		
		protected void OnActionNewPlaylistActivated(object sender, System.EventArgs e)
		{
		}

		protected void OnActionOpenPlaylistActivated(object sender, System.EventArgs e)
		{
		}

		protected void OnActionSavePlaylistActivated(object sender, System.EventArgs e)
		{
		}

		protected void OnActionSavePlaylistAsActivated(object sender, System.EventArgs e)
		{
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
        }

        public void RefreshPlaylist(Playlist playlist)
        {
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
        }

        #endregion

	}
}
