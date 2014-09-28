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
using Sessions.Player.Events;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.BassNetWrapper;
using Sessions.Sound.Playlists;
#if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
using Un4seen.Bass.AddOn.Fx;
#endif

namespace Sessions.Player
{
    public delegate void PlaylistEnded();
    public delegate void SegmentIndexChanged(int segmentIndex);
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
        bool IsPlayingLoop { get; }
        bool IsShuffleEnabled { get; set; }
        bool UseFloatingPoint { get; }
        int MixerSampleRate { get; }
        int CurrentSegmentIndex { get; }
        ShufflePlaylist Playlist { get; }
        RepeatType RepeatType { get; set; }
        float TimeShifting { get; set; }
        int PitchShifting { get; set; }
        int UpdatePeriod { get; set; }
        int UpdateThreads { get; set; }
        float Volume { get; set; }

        event PlaylistEnded OnPlaylistEnded;
        event PlaylistIndexChanged OnPlaylistIndexChanged;
        event AudioInterrupted OnAudioInterrupted;
        event BPMDetected OnBPMDetected;
        event SegmentIndexChanged OnSegmentIndexChanged;

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
