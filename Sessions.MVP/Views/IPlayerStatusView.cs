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
using Sessions.MVP.Models;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;
using org.sessionsapp.player;
using Sessions.Sound.Player;

namespace Sessions.MVP.Views
{
	/// <summary>
	/// Player status view interface.
	/// </summary>
	public interface IPlayerStatusView : IBaseView
	{
        Action OnPlayerPlayPause { get; set; }
        Action OnPlayerPrevious { get; set; }
        Action OnPlayerNext { get; set; }
        Action OnPlayerShuffle { get; set; }
        Action OnPlayerRepeat { get; set; }
        Action OnOpenPlaylist { get; set; }

	    void RefreshPlayerState(SSPPlayerState state);
	    void RefreshAudioFile(AudioFile audioFile);
	    void RefreshPlaylist(Playlist playlist);
	    void RefreshPlaylists(List<PlaylistEntity> playlists, Guid selectedPlaylistId);
	}
}
