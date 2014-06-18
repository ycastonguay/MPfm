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
using Sessions.MVP.Messages;
using Sessions.MVP.Models;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.BassNetWrapper;
using Sessions.Sound.Playlists;

namespace Sessions.MVP.Services.Interfaces
{
    /// <summary>
    /// Interface for the PlayerService class.
    /// </summary>
    public interface IPlayerService
    {
        bool IsInitialized { get; }
        bool IsSettingPosition { get; }
        bool IsPlaying { get; }
        bool IsPaused { get; }
        bool IsEQBypassed { get; }
        bool IsEQEnabled { get; }
        bool UseFloatingPoint { get; }

        PlayerStatusType Status { get; }
        EQPreset EQPreset { get; }
        Loop Loop { get; }
        RepeatType RepeatType { get; }
        float TimeShifting { get; }
        int PitchShifting { get; }
        float Volume { get; set; }

        PlaylistItem CurrentPlaylistItem { get; }
        Playlist CurrentPlaylist { get; }
        Playlist CurrentQueue { get; }

        event PlayerService.BPMDetected OnBPMDetected;

        void Initialize(Device device, int sampleRate, int bufferSize, int updatePeriod);
        void Dispose();

        void Play();        
        void Play(IEnumerable<string> filePaths);
        void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath, double initialPosition, bool startPaused, bool waitingToStart);
        void PlayQueue();
        void Stop();
        void Pause();
        void Next();
        void Previous();
        void Resume();
        void GoTo(int index);
        void GoTo(Guid playlistItemId);
        void ToggleRepeatType();

        int GetDataAvailable();
        Tuple<short[], short[]> GetMixerData(double seconds);
        Tuple<float[], float[]> GetFloatingPointMixerData(double seconds);
        PlayerPositionEntity GetPosition();

        void SetPosition(double percentage);
        void SetPosition(long bytes);
        void SetTimeShifting(float timeShifting);
        void SetPitchShifting(int pitchShifting);

        void GoToMarker(Marker marker);
        void StartLoop(Loop loop);
        void StopLoop();        

        void BypassEQ();
        void ResetEQ();
        void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue);
        void ApplyEQPreset(EQPreset preset);
    }
}
