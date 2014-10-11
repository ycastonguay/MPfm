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
using Sessions.MVP.Messages;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;

namespace Sessions.MVP.Views
{
	/// <summary>
	/// Player view interface.
	/// </summary>
    public interface IPlayerView : IBaseView
	{
        bool IsOutputMeterEnabled { get; }

        Action OnPlayerPlay { get; set; }
        Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        Action OnPlayerPause { get; set; }
        Action OnPlayerStop { get; set; }
        Action OnPlayerPrevious { get; set; }
        Action OnPlayerNext { get; set; }
        Action OnPlayerShuffle { get; set; }
        Action OnPlayerRepeat { get; set; }
        Action<float> OnPlayerSetVolume { get; set; }
        Action<float> OnPlayerSetPitchShifting { get; set; }
        Action<float> OnPlayerSetTimeShifting { get; set; }
        Action<float> OnPlayerSetPosition { get; set; }
        Func<float, PlayerPosition> OnPlayerRequestPosition { get; set; }
        Action OnEditSongMetadata { get; set; }
        Action OnOpenPlaylist { get; set; }
        Action OnOpenEffects { get; set; }
        Action OnOpenSelectAlbumArt { get; set; }
        Action OnPlayerViewAppeared { get; set; }
        Action<byte[]> OnApplyAlbumArtToSong { get; set; }
        Action<byte[]> OnApplyAlbumArtToAlbum { get; set; }

        void PlayerError(Exception ex);
	    void PushSubView(IBaseView view);
        void RefreshPlayerStatus(PlayerStatusType status, RepeatType repeatType, bool isShuffleEnabled);
		void RefreshPlayerPosition(PlayerPosition entity);
        void RefreshPlaylist(Playlist playlist);
		void RefreshSongInformation(AudioFile audioFile, Guid playlistItemId, long lengthBytes, int playlistIndex, int playlistCount);
        void RefreshMarkers(IEnumerable<Marker> markers);
        void RefreshActiveMarker(Guid markerId);
        void RefreshMarkerPosition(Marker marker);
        void RefreshLoops(IEnumerable<Loop> loops);
        void RefreshPlayerVolume(PlayerVolume entity);
        void RefreshPlayerTimeShifting(PlayerTimeShifting entity);
        void RefreshOutputMeter(float[] dataLeft, float[] dataRight);
	}
}

