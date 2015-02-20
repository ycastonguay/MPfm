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

using Sessions.Sound.AudioFiles;
using MonoTouch.UIKit;
using MonoTouch.MediaPlayer;

namespace Sessions.iOS.Classes.Services
{
    /// <summary>
    /// This service is used to keep the state of the now playing info for AirPlay and remotes.
    /// Since the NowPlaying structure needs to be recreated every time to update the metadata,
    /// we need a place to store this data.
    /// </summary>
	public class NowPlayingInfoService
	{
        public AudioFile AudioFile { get; set; }
        public long PositionMS { get; set; }
        public long LengthMS { get; set; }
        public int PlaylistIndex { get; set; }
        public int PlaylistCount { get; set; }
        public UIImage AlbumArtImage { get; set; }

        public void ResetAndUpdateInfo()
        {
            AudioFile = null;
            PositionMS = 0;
            LengthMS = 0;
            PlaylistIndex = 0;
            PlaylistCount = 0;
            AlbumArtImage = null;

            if(MPNowPlayingInfoCenter.DefaultCenter != null)
                MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = null;
        }

        public void UpdateInfo()
        {
            if (MPNowPlayingInfoCenter.DefaultCenter == null)
                return;

            if (AudioFile == null)
            {
                ResetAndUpdateInfo();
                return;
            }

            MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = new MPNowPlayingInfo() {
                Artist = AudioFile.ArtistName,
                AlbumTitle = AudioFile.AlbumTitle,
                Title = AudioFile.Title,
                PlaybackDuration = LengthMS / 1000,
                ElapsedPlaybackTime = PositionMS / 1000,
                PlaybackQueueIndex = PlaylistIndex,
                PlaybackQueueCount = PlaylistCount,
                Artwork = (AlbumArtImage != null) ? new MPMediaItemArtwork(AlbumArtImage) : null
            };
        }
	}
}
