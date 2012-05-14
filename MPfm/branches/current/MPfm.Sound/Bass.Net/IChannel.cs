//
// IChannel.cs: Interface for the Channel class.
//
// Copyright © 2011-2012 Yanick Castonguay
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

namespace MPfm.Sound.BassNetWrapper
{
    interface IChannel
    {
        ChannelType ChannelType { get; }
        int Handle { get; }
        bool IsDecode { get; }
        bool IsFloatingPoint { get; }
        int SampleRate { get; }
        float Volume { get; set; }
        
        void AddChannel(int channelHandle);       
        double Bytes2Seconds(long position);        
        void Free();
        void GetAttribute(Un4seen.Bass.BASSAttribute attribute, ref float value);
        int GetData(byte[] buffer, int length);
        int GetData(IntPtr buffer, int length);
        int GetData(float[] buffer, int length);
        long GetLength();
        int GetMixerData(float[] buffer, int length);
        long GetPosition();
        int GetSampleRate();        
        Un4seen.Bass.BASSActive IsActive();        
        void Lock(bool state);
        void Pause();
        void Play(bool restart);
        void RemoveFX(int handleFX);
        void RemoveSync(int syncHandle);
        void ResetFX(int handleFX);
        long Seconds2Bytes2(double position);
        void SetAttribute(Un4seen.Bass.BASSAttribute attribute, float value);
        Un4seen.Bass.BASSFlag SetFlags(Un4seen.Bass.BASSFlag flags, Un4seen.Bass.BASSFlag mask);
        int SetFX(Un4seen.Bass.BASSFXType type, int priority);
        void SetPosition(long position);
        void SetSampleRate(int sampleRate);
        int SetSync(Un4seen.Bass.BASSSync type, long param, Un4seen.Bass.SYNCPROC syncProc);
        void SetTempo(float tempo);
        void Stop();

    }
}
