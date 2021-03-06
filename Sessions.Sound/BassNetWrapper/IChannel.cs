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
using Un4seen.Bass;

namespace Sessions.Sound.BassNetWrapper
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
        void GetAttribute(BASSAttribute attribute, ref float value);
        int GetData(byte[] buffer, int length);
        int GetData(IntPtr buffer, int length);
        int GetData(int[] buffer, int length);
        int GetData(float[] buffer, int length);
        long GetLength();
        int GetMixerData(float[] buffer, int length);
        long GetPosition();
        int GetSampleRate();        
        BASSActive IsActive();        
        void Lock(bool state);
        void Pause();
        void Play(bool restart);
        void RemoveFX(int handleFX);
        void RemoveSync(int syncHandle);
        void ResetFX(int handleFX);
        long Seconds2Bytes(double position);
        void SetAttribute(BASSAttribute attribute, float value);
        BASSFlag SetFlags(BASSFlag flags, BASSFlag mask);
        int SetFX(BASSFXType type, int priority);
        void SetPosition(long position);
        void SetSampleRate(int sampleRate);
        int SetSync(BASSSync type, long param, SYNCPROC syncProc);
        void SetTempo(float tempo);
        void Stop();

    }
}
