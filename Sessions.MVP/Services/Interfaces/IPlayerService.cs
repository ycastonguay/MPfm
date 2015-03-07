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
using org.sessionsapp.player;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Player;
using Sessions.Sound.Objects;

namespace Sessions.MVP.Services.Interfaces
{
    /// <summary>
    /// Interface for the PlayerService class.
    /// </summary>
    public interface IPlayerService
    {
        SSPPlayerState State { get; }
        bool IsSettingPosition { get; }
        bool IsPlayingLoop { get; }
        bool IsShuffleEnabled { get; set; }
        bool IsEQEnabled { get; }

        SSPMixer Mixer { get; }
        SSPEQPreset EQPreset { get; }
        SSPLoop Loop { get; }
        SSPRepeatType RepeatType { get; }
        float TimeShifting { get; }
        int PitchShifting { get; }
        float Volume { get; set; }

        //PlaylistItem CurrentPlaylistItem { get; }
        AudioFile CurrentAudioFile { get; }
        SSPPlaylist Playlist { get; }
        //SSPPlaylist CurrentQueue { get; }

        event LoopPlaybackStartedDelegate OnLoopPlaybackStarted;
        event LoopPlaybackStoppedDelegate OnLoopPlaybackStopped;
        event BPMDetectedDelegate OnBPMDetected;

        void InitDevice(SSPDevice device, int sampleRate, int bufferSize, int updatePeriod);
        void Dispose();

        void SetBufferSize(int bufferSize);
        void SetUpdatePeriod(int updatePeriod);

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

        long GetDataAvailable();
        Tuple<short[], short[]> GetMixerData(double seconds);
        Tuple<float[], float[]> GetFloatingPointMixerData(double seconds);
        SSPPosition GetPosition();

        void SetPosition(double percentage);
        void SetPosition(long bytes);
        void SetTimeShifting(float timeShifting);
        void SetPitchShifting(int pitchShifting);

        void GoToMarker(Marker marker);
        void StartLoop(SSPLoop loop);
        void UpdateLoop(SSPLoop loop);
        void StopLoop();        

        void EnableEQ(bool enabled);
        void ResetEQ();
        void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue);
        void ApplyEQPreset(SSPEQPreset preset);

        IEnumerable<SSPDevice> GetOutputDevices();
    }
}
