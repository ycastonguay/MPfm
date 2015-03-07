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
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Player;

namespace Sessions.MVP.Views
{
	/// <summary>
	/// Playlist view interface.
	/// </summary>
    public interface IPlaylistView : IBaseView
	{
        // Note: These actions use the PlaylistItemId, not AudioFileId!
        Action<Guid, int> OnChangePlaylistItemOrder { get; set; }
        Action<Guid> OnSelectPlaylistItem { get; set; }
        Action<List<Guid>> OnRemovePlaylistItems { get; set; }
        Action OnNewPlaylist { get; set; }
        Action<string> OnLoadPlaylist { get; set; }
        Action OnSavePlaylist { get; set; }
        Action OnShufflePlaylist { get; set; }

        void PlaylistError(Exception ex);
        void RefreshPlaylist(Playlist playlist);
        void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile);
	}
}
