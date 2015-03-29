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
using org.sessionsapp.player;
using System.Collections.Generic;

namespace Sessions.Sound.Player
{
    /// <summary>
    /// Interface for the Player class.
    /// </summary>
    public interface ISSPPlayer : IDisposable
    {
        // This will replace IPlayer, but should it replace IPlayerService? 
        // IPlayerService manages AppConfigManager but also requires ICloudService and ILibraryService.
        event LogDelegate Log;
        event StateChangedDelegate StateChanged;
        event PlaylistIndexChangedDelegate PlaylistIndexChanged;

        Playlist Playlist { get; }

        // Initialization
        void Init();
        void InitDevice(int device, int sampleRate, int bufferSize, int updatePeriod, bool useFloatingPoint);
        void FreeDevice();
        void Free();

        float GetCPU();
        UInt32 GetBufferDataAvailable();

        void SetBufferSize(int bufferSize);
        void SetUpdatePeriod(int updatePeriod);

        int Version { get; }
        SSPPlayerState State { get; }
        SSPDevice Device { get; }
        SSPMixer Mixer { get; }
        SSPEQPreset EQPreset { get; }
        SSPLoop Loop { get; }
        bool EQEnabled { get; set; }

        // Playhead
        bool IsShuffle { get; set; }
        SSPRepeatType RepeatType { get; set; }
        float Volume { get; set; }
        float TimeShifting { get; set; }
        int PitchShifting { get; set; }

        bool IsSettingPosition { get; }
        bool IsPlayingLoop { get; }

        void ToggleRepeatType();

        // EQ
        void SetEQPreset(SSPEQPreset preset);
        void SetEQPresetBand(int band, float gain);
        void ResetEQ();
        void NormalizeEQ();

        // Loops
        void StartLoop(SSPLoop loop);
        void UpdateLoop(SSPLoop loop);
        void StopLoop();

        // Playback
        void Play();
        void Play(int startIndex, long startPosition, bool startPaused);
        void Pause();
        void Stop();
        void Previous();
        void Next();
        void GoTo(int index);

        // Position
        SSPPosition GetPosition();
        SSPPosition GetPositionFromPercentage(double percentage);
        void SetPosition(long position);
        void SetPosition(float position);

        // Data
        long Seconds2Bytes(double value);
        int GetMixerData(int length, float[] sampleData);
        int GetMixerData(int length, int[] sampleData);
        long GetDataAvailable();

        // Encoder
        void StartEncode(SSPEncoderType encoder);
        void StopEncode();
        void StartCast(SSPCastServer server);
        void StopCast();

        // Output devices
        IEnumerable<SSPDevice> GetOutputDevices();
    }
}
