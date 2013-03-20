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

using System.Collections.Generic;
using MPfm.Player;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Bass.Net;
using MPfm.Sound.Playlists;

namespace MPfm.MVP.Services.Interfaces
{
    /// <summary>
    /// Interface for the PlayerService class.
    /// </summary>
    public interface IPlayerService
    {
        bool IsSettingPosition { get; }
        bool IsPaused { get; }
        PlaylistItem CurrentPlaylistItem { get; }
        float Volume { get; }

        void Initialize(Device device, int sampleRate, int bufferSize, int updatePeriod);
        void Dispose();

        void Play();
        void Play(IEnumerable<AudioFile> audioFiles);
        void Play(IEnumerable<string> filePaths);
        void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath);
        void Stop();
        void Pause();
        void Next();
        void Previous();
        void RepeatType();

        int GetDataAvailable();
        long GetPosition();

        void SetPosition(double percentage);
        void SetPosition(long bytes);
        void SetVolume(float volume);
        void SetTimeShifting(float timeShifting);

        void GoToMarker(Marker marker);

        void BypassEQ();
        void ResetEQ();
        void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue);
    }
}
