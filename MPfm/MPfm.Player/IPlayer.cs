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
using System.Collections.Generic;
using MPfm.Player.Objects;
using MPfm.Sound;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Player
{
    /// <summary>
    /// Interface for the Player class.
    /// </summary>
    public interface IPlayer
    {
        int BufferSize { get; set; }
        EQPreset CurrentEQPreset { get; set; }
        Loop CurrentLoop { get; }
        Device Device { get; }
        Channel FXChannel { get; }
        bool IsSettingPosition { get; }
        bool IsDeviceInitialized { get; }
        bool IsEQBypassed { get; }
        bool IsEQEnabled { get; }
        bool IsPaused { get; }
        bool IsPlaying { get; }
        List<Loop> Loops { get; }
        List<Marker> Markers { get; }
        MixerChannel MixerChannel { get; }
        int MixerSampleRate { get; }
        Playlist Playlist { get; }
        RepeatType RepeatType { get; set; }
        float TimeShifting { get; set; }
        int UpdatePeriod { get; set; }
        int UpdateThreads { get; set; }
        float Volume { get; set; }

        event Player.PlaylistIndexChanged OnPlaylistIndexChanged;
        event Player.AudioInterrupted OnAudioInterrupted;

        void ApplyEQPreset(EQPreset preset);        
        void BypassEQ();
        void Dispose();
        void FreeDevice();
        void FreePlugins();
        BASS_BFX_PEAKEQ GetEQParams(int band);
        long GetPosition();
        void GoTo(int index);
        void GoToMarker(Marker marker);
        void InitializeDevice();
        void InitializeDevice(Device device, int mixerSampleRate);
        void LoadPlugins();
        void Next();
        void Pause();
        void Play();
        void PlayFiles(List<AudioFile> audioFiles);
        void PlayFiles(List<string> filePaths);        
        void Previous();
        void ResetEQ();
        void SetPosition(double percentage);
        void SetPosition(long bytes);
        void StartLoop(Loop loop);
        void Stop();
        void StopLoop();
        void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue);
    }
}
