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
using MPfm.Sound.AudioFiles;
using MPfm.Sound.BassNetWrapper;
using MPfm.Sound.Playlists;

namespace MPfm.Player
{
    public class Player : IPlayer
    {
        public int BufferSize { get; set; }
        public EQPreset EQPreset { get; set; }
        public Loop Loop { get; private set; }
        public Device Device { get; private set; }
        public bool IsSettingPosition { get; private set; }
        public bool IsDeviceInitialized { get; private set; }
        public bool IsEQBypassed { get; private set; }
        public bool IsEQEnabled { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsPlaying { get; private set; }
        public int MixerSampleRate { get; private set; }
        public Playlist Playlist { get; private set; }
        public RepeatType RepeatType { get; set; }
        public float TimeShifting { get; set; }
        public int PitchShifting { get; set; }
        public int UpdatePeriod { get; set; }
        public int UpdateThreads { get; set; }
        public float Volume { get; set; }
        public event PlaylistIndexChanged OnPlaylistIndexChanged;
        public event AudioInterrupted OnAudioInterrupted;
        public event BPMDetected OnBPMDetected;

        public Player(Device device, int mixerSampleRate, int bufferSize, int updatePeriod, bool initializeDevice)        
        {
        }

        public void InitializeDevice()
        {
        }

        public void InitializeDevice(Device device, int mixerSampleRate)
        {
        }

        public void Dispose()
        {
        }

        public void FreeDevice()
        {
        }

        public void FreePlugins()
        {
        }

        public void Play()
        {
        }

        public void PlayFiles(List<AudioFile> audioFiles)
        {
        }

        public void PlayFiles(List<string> filePaths)
        {
        }

        public void Pause()
        {
        }

        public void Stop()
        {
        }

        public void Previous()
        {
        }

        public void Next()
        {
        }

        public void GoTo(int index)
        {
        }

        public void GoTo(Guid playlistItemId)
        {
        }

        public int GetDataAvailable()
        {
            return 0;
        }

        public long Seconds2Bytes(double value)
        {
            return 0;
        }

        public int GetMixerData(int length, float[] sampleData)
        {
            return 0;
        }

        public long GetPosition()
        {
            return 0;
        }

        public void SetPosition(double percentage)
        {
        }

        public void SetPosition(long bytes)
        {
        }

        public void ApplyEQPreset(EQPreset preset)
        {
        }

        public void BypassEQ()
        {
        }

        public void ResetEQ()
        {
        }

        public void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue)
        {
        }

        public void GoToMarker(Marker marker)
        {
        }

        public void StartLoop(Loop loop)
        {
        }

        public void StopLoop()
        {
        }
    }
}