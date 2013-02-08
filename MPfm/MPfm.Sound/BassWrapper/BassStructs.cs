//
// BassStructs.cs: This file contains structs for the P/Invoke wrapper of the BASS audio library.
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
using System.Runtime.InteropServices;

namespace MPfm.Sound.BassWrapper
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_3DVECTOR
    {
        public float x;
        public float y;
        public float z;
        public BASS_3DVECTOR()
        {
        }
        public BASS_3DVECTOR(float X, float Y, float Z)
        {
            this.x = X;
            this.y = Y;
            this.z = Z;
        }
        //public override string ToString()
        //{
        //    return string.Format("X={0}, Y={1}, Z={2}", this.x, this.y, this.z);
        //}
    }

    [Serializable]
    public sealed class BASS_CHANNELINFO
    {
        public int freq;
        public int chans;
        public BASSFlag flags;
        public BASSChannelType ctype;
        public int origres;
        public int plugin;
        public int sample;
        public string filename = string.Empty;
        public bool IsDecodingChannel
        {
            get
            {
                return (this.flags & BASSFlag.BASS_STREAM_DECODE) != BASSFlag.BASS_DEFAULT;
            }
        }
        public bool Is32bit
        {
            get
            {
                return (this.flags & BASSFlag.BASS_SAMPLE_FLOAT) != BASSFlag.BASS_DEFAULT;
            }
        }
        public bool Is8bit
        {
            get
            {
                return (this.flags & BASSFlag.BASS_SAMPLE_8BITS) != BASSFlag.BASS_DEFAULT;
            }
        }
        //public override string ToString()
        //{
        //    return string.Format("{0}, {1}Hz, {2}, {3}bit", new object[]
        //    {
        //        Utils.BASSChannelTypeToString(this.ctype),
        //        this.freq,
        //        Utils.ChannelNumberToString(this.chans),
        //        (this.origres == 0) ? (this.Is32bit ? 32 : (this.Is8bit ? 8 : 16)) : this.origres
        //    });
        //}
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct BASS_DEVICEINFO_TEMP
    {
        public IntPtr a;
        public IntPtr b;
        public BASSDeviceInfo c;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class BASS_DEVICEINFO
    {
        public string name;// = string.Empty;
        public string driver;// = string.Empty;
        public BASSDeviceInfo flags;
        public bool IsEnabled
        {
            get
            {
                return (this.flags & BASSDeviceInfo.BASS_DEVICE_ENABLED) != BASSDeviceInfo.BASS_DEVICE_NONE;
            }
        }
        public bool IsDefault
        {
            get
            {
                return (this.flags & BASSDeviceInfo.BASS_DEVICE_DEFAULT) != BASSDeviceInfo.BASS_DEVICE_NONE;
            }
        }
        public bool IsInitialized
        {
            get
            {
                return (this.flags & BASSDeviceInfo.BASS_DEVICE_INIT) != BASSDeviceInfo.BASS_DEVICE_NONE;
            }
        }
        public override string ToString()
        {
            return this.name;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_FILEPROCS
    {
        public FILECLOSEPROC close;
        public FILELENPROC length;
        public FILEREADPROC read;
        public FILESEEKPROC seek;
        public BASS_FILEPROCS(FILECLOSEPROC closeCallback, FILELENPROC lengthCallback, FILEREADPROC readCallback, FILESEEKPROC seekCallback)
        {
            this.close = closeCallback;
            this.length = lengthCallback;
            this.read = readCallback;
            this.seek = seekCallback;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_INFO
    {
        public BASSInfo flags;
        public int hwsize;
        public int hwfree;
        public int freesam;
        public int free3d;
        public int minrate;
        public int maxrate;
        public bool eax;
        public int minbuf = 500;
        public int dsver;
        public int latency;
        public BASSInit initflags;
        public int speakers;
        public int freq;
        public bool SupportsContinuousRate
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_CONTINUOUSRATE) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool SupportsDirectSound
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_EMULDRIVER) == BASSInfo.DSCAPS_NONE;
            }
        }
        public bool IsCertified
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_CERTIFIED) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool SupportsMonoSamples
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_SECONDARYMONO) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool SupportsStereoSamples
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_SECONDARYSTEREO) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool Supports8BitSamples
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_SECONDARY8BIT) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool Supports16BitSamples
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_SECONDARY16BIT) != BASSInfo.DSCAPS_NONE;
            }
        }
        public override string ToString()
        {
            return string.Format("Speakers={0}, MinRate={1}, MaxRate={2}, DX={3}, EAX={4}", new object[]
		    {
			    this.speakers,
			    this.minrate,
			    this.maxrate,
			    this.dsver,
			    this.eax
		    });
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class BASS_PLUGINFORM
    {
        public BASSChannelType ctype;
        [MarshalAs(UnmanagedType.LPStr)]
        public string name = string.Empty;
        [MarshalAs(UnmanagedType.LPStr)]
        public string exts = string.Empty;
        public BASS_PLUGINFORM()
        {
        }
        public BASS_PLUGINFORM(string Name, string Extensions, BASSChannelType ChannelType)
        {
            this.ctype = ChannelType;
            this.name = Name;
            this.exts = Extensions;
        }
        public override string ToString()
        {
            return string.Format("{0}|{1}", this.name, this.exts);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_RECORDINFO
    {
        public BASSRecordInfo flags;
        public BASSRecordFormat formats;
        public int inputs;
        public bool singlein;
        public int freq;
        public bool SupportsDirectSound
        {
            get
            {
                return (this.flags & BASSRecordInfo.DSCAPS_EMULDRIVER) == BASSRecordInfo.DSCAPS_NONE;
            }
        }
        public bool IsCertified
        {
            get
            {
                return (this.flags & BASSRecordInfo.DSCAPS_CERTIFIED) != BASSRecordInfo.DSCAPS_NONE;
            }
        }
        public override string ToString()
        {
            return string.Format("Inputs={0}, SingleIn={1}", this.inputs, this.singlein);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_SAMPLE
    {
        public int freq = 44100;
        public float volume = 1f;
        public float pan;
        public BASSFlag flags;
        public int length;
        public int max = 1;
        public int origres;
        public int chans = 2;
        public int mingap;
        public BASS3DMode mode3d;
        public float mindist;
        public float maxdist;
        public int iangle;
        public int oangle;
        public float outvol = 1f;
        public BASSVam vam = BASSVam.BASS_VAM_HARDWARE;
        public int priority;
        public BASS_SAMPLE()
        {
        }
        public BASS_SAMPLE(int Freq, float Volume, float Pan, BASSFlag Flags, int Length, int Max, int OrigRes, int Chans, int MinGap, BASS3DMode Flag3D, float MinDist, float MaxDist, int IAngle, int OAngle, float OutVol, BASSVam FlagsVam, int Priority)
        {
            this.freq = Freq;
            this.volume = Volume;
            this.pan = Pan;
            this.flags = Flags;
            this.length = Length;
            this.max = Max;
            this.origres = OrigRes;
            this.chans = Chans;
            this.mingap = MinGap;
            this.mode3d = Flag3D;
            this.mindist = MinDist;
            this.maxdist = MaxDist;
            this.iangle = IAngle;
            this.oangle = OAngle;
            this.outvol = OutVol;
            this.vam = FlagsVam;
            this.priority = Priority;
        }
        public override string ToString()
        {
            return string.Format("Frequency={0}, Volume={1}, Pan={2}", this.freq, this.volume, this.pan);
        }
    }

}
