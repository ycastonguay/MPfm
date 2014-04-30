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
using MPfm.Player.Events;
using MPfm.Player.Objects;
using MPfm.Sound;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.BassNetWrapper;
using MPfm.Sound.Playlists;

#if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
using Un4seen.Bass.AddOn.Fx;
#endif

namespace MPfm.Player
{
    public delegate void PlaylistIndexChanged(PlayerPlaylistIndexChangedData data);
    public delegate void AudioInterrupted(AudioInterruptedData data);
    public delegate void BPMDetected(float bpm);

    /// <summary>
    /// Interface for the Player class.
    /// </summary>
    public interface IPlayer
    {
        int BufferSize { get; set; }
        EQPreset EQPreset { get; set; }
        Loop Loop { get; }
        Device Device { get; }
        bool IsSettingPosition { get; }
        bool IsDeviceInitialized { get; }
        bool IsEQBypassed { get; }
        bool IsEQEnabled { get; }
        bool IsPaused { get; }
        bool IsPlaying { get; }
        bool UseFloatingPoint { get; }
        int MixerSampleRate { get; }
        Playlist Playlist { get; }
        RepeatType RepeatType { get; set; }
        float TimeShifting { get; set; }
        int PitchShifting { get; set; }
        int UpdatePeriod { get; set; }
        int UpdateThreads { get; set; }
        float Volume { get; set; }

        event PlaylistIndexChanged OnPlaylistIndexChanged;
        event AudioInterrupted OnAudioInterrupted;
        event BPMDetected OnBPMDetected;

        void InitializeDevice();
        void InitializeDevice(Device device, int mixerSampleRate);
        void Dispose();
        void FreeDevice();
        void FreePlugins();

        void Play();
        void Play(double initialPosition, bool startPaused);
        void PlayFiles(List<AudioFile> audioFiles);
        void PlayFiles(List<string> filePaths);        
        void Pause();
        void Stop();
        void Previous();
        void Next();
        void GoTo(int index);
        void GoTo(Guid playlistItemId);

        void StartEncode(Player.EncoderType encoderType);
        void StopEncode();
        void StartCast(CastServerParams serverParams);
        void StopCast();

        int GetDataAvailable();
        long Seconds2Bytes(double value);
        int GetMixerData(int length, float[] sampleData);
        int GetMixerData(int length, int[] sampleData);
        long GetPosition();
        void SetPosition(double percentage);
        void SetPosition(long bytes);       

        void ApplyEQPreset(EQPreset preset);        
        void BypassEQ();
        void ResetEQ();
        void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue);

        void GoToMarker(Marker marker);
        void StartLoop(Loop loop);
        void StopLoop();
    }
}