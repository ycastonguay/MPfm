//
// BassWasapiStructs.cs: This file contains structs for the P/Invoke wrapper of the BASS WASAPI audio library.
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

namespace MPfm.Sound.BassWrapper.Wasapi
{
	[Serializable]
	public sealed class BASS_WASAPI_DEVICEINFO
	{
		//internal a a;
		public string name = string.Empty;
		public string id = string.Empty;
		public BASSWASAPIDeviceType type = BASSWASAPIDeviceType.BASS_WASAPI_TYPE_UNKNOWN;
		public BASSWASAPIDeviceInfo flags;
		public float minperiod;
		public float defperiod;
		public int mixfreq;
		public int mixchans;
		public bool IsEnabled
		{
			get
			{
				return (this.flags & BASSWASAPIDeviceInfo.BASS_DEVICE_ENABLED) != BASSWASAPIDeviceInfo.BASS_DEVICE_UNKNOWN;
			}
		}
		public bool IsDefault
		{
			get
			{
				return (this.flags & BASSWASAPIDeviceInfo.BASS_DEVICE_DEFAULT) != BASSWASAPIDeviceInfo.BASS_DEVICE_UNKNOWN;
			}
		}
		public bool IsInitialized
		{
			get
			{
				return (this.flags & BASSWASAPIDeviceInfo.BASS_DEVICE_INIT) != BASSWASAPIDeviceInfo.BASS_DEVICE_UNKNOWN;
			}
		}
		public bool IsLoopback
		{
			get
			{
				return (this.flags & BASSWASAPIDeviceInfo.BASS_DEVICE_LOOPBACK) != BASSWASAPIDeviceInfo.BASS_DEVICE_UNKNOWN;
			}
		}
		public bool IsInput
		{
			get
			{
				return (this.flags & BASSWASAPIDeviceInfo.BASS_DEVICE_INPUT) != BASSWASAPIDeviceInfo.BASS_DEVICE_UNKNOWN;
			}
		}
		public bool SupportsRecording
		{
			get
			{
				return (this.flags & BASSWASAPIDeviceInfo.BASS_DEVICE_INPUT) != BASSWASAPIDeviceInfo.BASS_DEVICE_UNKNOWN || (this.flags & BASSWASAPIDeviceInfo.BASS_DEVICE_LOOPBACK) != BASSWASAPIDeviceInfo.BASS_DEVICE_UNKNOWN;
			}
		}
		public override string ToString()
		{
			return this.name;
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class BASS_WASAPI_INFO
	{
		public BASSWASAPIInit initflags;
		public int freq;
		public int chans;
		public BASSWASAPIFormat format;
		public int buflen;
		private float a;
		private float b;
		private float c;
		public bool IsExclusive
		{
			get
			{
				return (this.initflags & BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE) != BASSWASAPIInit.BASS_WASAPI_SHARED;
			}
		}
        //public override string ToString()
        //{
        //    return string.Format("{0}, {1}Hz, {2}", (this.format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_FLOAT || this.format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_32BIT) ? "32-bit" : ((this.format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_24BIT) ? "24-bit" : ((this.format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_16BIT) ? "16-bit" : ((this.format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_8BIT) ? "8-bit" : "Unknown"))), this.freq, Utils.ChannelNumberToString(this.chans));
        //}
	}

    //[SuppressUnmanagedCodeSecurity]
    //public class BassWasapiHandler : IDisposable
    //{
    //    public delegate void BassWasapiHandlerEventHandler(object sender, BassWasapiHandlerEventArgs e);
    //    private bool a;
    //    private BASS_WASAPI_DEVICEINFO b = new BASS_WASAPI_DEVICEINFO();
    //    private WASAPIPROC c;
    //    private bool d;
    //    private volatile float e = 1f;
    //    private volatile float f = 1f;
    //    private int g = -1;
    //    private bool h;
    //    private bool i = true;
    //    private int j = -1;
    //    private int k = 48000;
    //    private int l = 2;
    //    private float m;
    //    private float n;
    //    private int o;
    //    private int p;
    //    private float q = 1f;
    //    private float r;
    //    private int s;
    //    private bool t;
    //    private volatile bool u;
    //    private BassWasapiHandler.BassWasapiHandlerEventHandler v;
    //    public event BassWasapiHandler.BassWasapiHandlerEventHandler Notification
    //    {
    //        [MethodImpl(MethodImplOptions.Synchronized)]
    //        add
    //        {
    //            this.v = (BassWasapiHandler.BassWasapiHandlerEventHandler)Delegate.Combine(this.v, value);
    //        }
    //        [MethodImpl(MethodImplOptions.Synchronized)]
    //        remove
    //        {
    //            this.v = (BassWasapiHandler.BassWasapiHandlerEventHandler)Delegate.Remove(this.v, value);
    //        }
    //    }
    //    public WASAPIPROC InternalWasapiProc
    //    {
    //        get
    //        {
    //            return this.c;
    //        }
    //    }
    //    public bool IsInput
    //    {
    //        get
    //        {
    //            return this.b.SupportsRecording;
    //        }
    //    }
    //    public bool Exclusive
    //    {
    //        get
    //        {
    //            return this.i;
    //        }
    //    }
    //    public int Device
    //    {
    //        get
    //        {
    //            return this.j;
    //        }
    //    }
    //    public double SampleRate
    //    {
    //        get
    //        {
    //            return (double)this.k;
    //        }
    //    }
    //    public int NumChans
    //    {
    //        get
    //        {
    //            return this.l;
    //        }
    //    }
    //    public float BufferLength
    //    {
    //        get
    //        {
    //            return this.m;
    //        }
    //    }
    //    public float UpdatePeriod
    //    {
    //        get
    //        {
    //            return this.n;
    //        }
    //    }
    //    public int InternalMixer
    //    {
    //        get
    //        {
    //            return this.o;
    //        }
    //    }
    //    public int OutputChannel
    //    {
    //        get
    //        {
    //            return this.p;
    //        }
    //    }
    //    public float Volume
    //    {
    //        get
    //        {
    //            return this.q;
    //        }
    //        set
    //        {
    //            if (this.q == value)
    //            {
    //                return;
    //            }
    //            if (value < 0f)
    //            {
    //                this.q = 0f;
    //            }
    //            else
    //            {
    //                this.q = value;
    //            }
    //            this.a(this.q, this.r);
    //        }
    //    }
    //    public float Pan
    //    {
    //        get
    //        {
    //            return this.r;
    //        }
    //        set
    //        {
    //            if (this.r == value)
    //            {
    //                return;
    //            }
    //            if (value < -1f)
    //            {
    //                this.r = -1f;
    //            }
    //            else
    //            {
    //                if (value > 1f)
    //                {
    //                    this.r = 1f;
    //                }
    //                else
    //                {
    //                    this.r = value;
    //                }
    //            }
    //            this.a(this.q, this.r);
    //        }
    //    }
    //    //public float DeviceVolume
    //    //{
    //    //    get
    //    //    {
    //    //        int num = BassWasapi.BASS_WASAPI_GetDevice();
    //    //        if (num != this.j)
    //    //        {
    //    //            BassWasapi.BASS_WASAPI_SetDevice(this.j);
    //    //        }
    //    //        float result = BassWasapi.BASS_WASAPI_GetVolume(true);
    //    //        BassWasapi.BASS_WASAPI_SetDevice(num);
    //    //        return result;
    //    //    }
    //    //    set
    //    //    {
    //    //        if (value < 0f)
    //    //        {
    //    //            value = 0f;
    //    //        }
    //    //        else
    //    //        {
    //    //            if (value > 1f)
    //    //            {
    //    //                value = 1f;
    //    //            }
    //    //        }
    //    //        int num = BassWasapi.BASS_WASAPI_GetDevice();
    //    //        if (num != this.j)
    //    //        {
    //    //            BassWasapi.BASS_WASAPI_SetDevice(this.j);
    //    //        }
    //    //        BassWasapi.BASS_WASAPI_SetVolume(true, value);
    //    //        BassWasapi.BASS_WASAPI_SetDevice(num);
    //    //    }
    //    //}
    //    //public bool DeviceMute
    //    //{
    //    //    get
    //    //    {
    //    //        int num = BassWasapi.BASS_WASAPI_GetDevice();
    //    //        if (num != this.j)
    //    //        {
    //    //            BassWasapi.BASS_WASAPI_SetDevice(this.j);
    //    //        }
    //    //        bool result = BassWasapi.BASS_WASAPI_GetMute();
    //    //        BassWasapi.BASS_WASAPI_SetDevice(num);
    //    //        return result;
    //    //    }
    //    //    set
    //    //    {
    //    //        int num = BassWasapi.BASS_WASAPI_GetDevice();
    //    //        if (num != this.j)
    //    //        {
    //    //            BassWasapi.BASS_WASAPI_SetDevice(this.j);
    //    //        }
    //    //        BassWasapi.BASS_WASAPI_SetMute(value);
    //    //        BassWasapi.BASS_WASAPI_SetDevice(num);
    //    //    }
    //    //}
    //    public int InputChannel
    //    {
    //        get
    //        {
    //            return this.s;
    //        }
    //    }
    //    public bool UseInput
    //    {
    //        get
    //        {
    //            return this.t;
    //        }
    //        set
    //        {
    //            if (!this.IsInput)
    //            {
    //                this.t = false;
    //                return;
    //            }
    //            this.t = value;
    //            if (this.s != 0)
    //            {
    //                Bass.BASS_StreamFree(this.s);
    //                this.s = 0;
    //            }
    //            if (this.t)
    //            {
    //                this.s = Bass.BASS_StreamCreateDummy(this.k, this.l, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, IntPtr.Zero);
    //            }
    //        }
    //    }
    //    public bool IsInputFullDuplex
    //    {
    //        get
    //        {
    //            return this.IsInput && this.u;
    //        }
    //    }
    //    public bool BypassFullDuplex
    //    {
    //        get
    //        {
    //            return this.h;
    //        }
    //        set
    //        {
    //            this.h = value;
    //        }
    //    }
    //    public BassWasapiHandler(int device, bool exclusive, int freq, int chans, float buffer, float period)
    //    {
    //        this.j = device;
    //        this.i = exclusive;
    //        if (!BassWasapi.BASS_WASAPI_GetDeviceInfo(device, this.b))
    //        {
    //            throw new ArgumentException("Invalid device: " + Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));
    //        }
    //        if (exclusive)
    //        {
    //            this.l = chans;
    //        }
    //        else
    //        {
    //            this.l = this.b.mixchans;
    //        }
    //        if (exclusive)
    //        {
    //            this.k = freq;
    //        }
    //        else
    //        {
    //            this.k = this.b.mixfreq;
    //        }
    //        this.n = period;
    //        if (buffer == 0f)
    //        {
    //            this.m = ((this.n == 0f) ? this.b.defperiod : this.n) * 4f;
    //        }
    //        else
    //        {
    //            this.m = buffer;
    //        }
    //        if (this.IsInput)
    //        {
    //            this.UseInput = true;
    //            return;
    //        }
    //        this.o = BassMix.BASS_Mixer_StreamCreate(this.k, this.l, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_RESUME);
    //        if (this.o == 0)
    //        {
    //            throw new NotSupportedException("Internal Mixer: " + Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));
    //        }
    //    }
    //    public void Dispose()
    //    {
    //        this.a(true);
    //        GC.SuppressFinalize(this);
    //    }
    //    //private void a(bool A_0)
    //    //{
    //    //    if (!this.a)
    //    //    {
    //    //        if (A_0)
    //    //        {
    //    //            int num = BassWasapi.BASS_WASAPI_GetDevice();
    //    //            if (num != this.j)
    //    //            {
    //    //                BassWasapi.BASS_WASAPI_SetDevice(this.j);
    //    //            }
    //    //            this.RemoveFullDuplex();
    //    //            BassWasapi.BASS_WASAPI_Stop(true);
    //    //            Bass.BASS_StreamFree(this.o);
    //    //            this.o = 0;
    //    //            BassWasapi.BASS_WASAPI_Free();
    //    //            BassWasapi.BASS_WASAPI_SetDevice(num);
    //    //        }
    //    //        if (this.p != 0)
    //    //        {
    //    //            Bass.BASS_StreamFree(this.p);
    //    //            this.p = 0;
    //    //        }
    //    //        if (this.s != 0)
    //    //        {
    //    //            Bass.BASS_StreamFree(this.s);
    //    //            this.s = 0;
    //    //        }
    //    //    }
    //    //    this.a = true;
    //    //}
    //    ~BassWasapiHandler()
    //    {
    //        this.a(false);
    //    }
    //    public bool Init()
    //    {
    //        if (this.IsInput)
    //        {
    //            this.c = new WASAPIPROC(this.WasapiInputCallback);
    //            return BassWasapi.BASS_WASAPI_Init(this.j, this.k, this.l, this.i ? BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE : BASSWASAPIInit.BASS_WASAPI_SHARED, this.m, this.n, this.c, IntPtr.Zero);
    //        }
    //        this.c = new WASAPIPROC(this.WasapiOutputCallback);
    //        return BassWasapi.BASS_WASAPI_Init(this.j, this.k, this.l, this.i ? BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE : BASSWASAPIInit.BASS_WASAPI_SHARED, this.m, this.n, this.c, IntPtr.Zero);
    //    }
    //    public bool Start()
    //    {
    //        bool flag = true;
    //        int num = BassWasapi.BASS_WASAPI_GetDevice();
    //        if (num != this.j)
    //        {
    //            flag &= BassWasapi.BASS_WASAPI_SetDevice(this.j);
    //        }
    //        if (flag && !BassWasapi.BASS_WASAPI_IsStarted())
    //        {
    //            flag &= BassWasapi.BASS_WASAPI_Start();
    //        }
    //        BassWasapi.BASS_WASAPI_SetDevice(num);
    //        return flag;
    //    }
    //    public bool Stop()
    //    {
    //        bool flag = true;
    //        int num = BassWasapi.BASS_WASAPI_GetDevice();
    //        if (num != this.j)
    //        {
    //            flag &= BassWasapi.BASS_WASAPI_SetDevice(this.j);
    //        }
    //        if (flag && BassWasapi.BASS_WASAPI_IsStarted())
    //        {
    //            flag &= BassWasapi.BASS_WASAPI_Stop(true);
    //        }
    //        BassWasapi.BASS_WASAPI_SetDevice(num);
    //        return flag;
    //    }
    //    public bool Pause(bool pause)
    //    {
    //        bool flag = true;
    //        int num = BassWasapi.BASS_WASAPI_GetDevice();
    //        if (pause)
    //        {
    //            if (num != this.j)
    //            {
    //                flag &= BassWasapi.BASS_WASAPI_SetDevice(this.j);
    //            }
    //            if (flag)
    //            {
    //                flag &= BassWasapi.BASS_WASAPI_Stop(false);
    //            }
    //        }
    //        else
    //        {
    //            if (flag)
    //            {
    //                flag &= BassWasapi.BASS_WASAPI_SetDevice(this.j);
    //                if (flag)
    //                {
    //                    flag &= BassWasapi.BASS_WASAPI_Start();
    //                }
    //            }
    //        }
    //        BassWasapi.BASS_WASAPI_SetDevice(num);
    //        return flag;
    //    }
    //    public bool AddOutputSource(int channel, BASSFlag flags)
    //    {
    //        BASS_CHANNELINFO bASS_CHANNELINFO = Bass.BASS_ChannelGetInfo(channel);
    //        if (bASS_CHANNELINFO == null)
    //        {
    //            return false;
    //        }
    //        if (!bASS_CHANNELINFO.IsDecodingChannel && bASS_CHANNELINFO.ctype != BASSChannelType.BASS_CTYPE_RECORD)
    //        {
    //            return false;
    //        }
    //        if (flags < BASSFlag.BASS_SPEAKER_FRONT)
    //        {
    //            flags |= BASSFlag.BASS_WV_STEREO;
    //        }
    //        if (bASS_CHANNELINFO.freq != this.k)
    //        {
    //            flags |= BASSFlag.BASS_MIXER_RESUME;
    //        }
    //        return BassMix.BASS_Mixer_StreamAddChannel(this.o, channel, flags);
    //    }
    //    public bool SetFullDuplex(int bassDevice, BASSFlag flags, bool buffered)
    //    {
    //        if (!this.IsInput)
    //        {
    //            return false;
    //        }
    //        bool flag = true;
    //        int num = Bass.BASS_GetDevice();
    //        if (num != bassDevice)
    //        {
    //            flag &= Bass.BASS_SetDevice(bassDevice);
    //        }
    //        if (flag)
    //        {
    //            if (this.p != 0)
    //            {
    //                Bass.BASS_StreamFree(this.p);
    //            }
    //            flags &= ~BASSFlag.BASS_SAMPLE_8BITS;
    //            flags |= BASSFlag.BASS_SAMPLE_FLOAT;
    //            this.p = Bass.BASS_StreamCreatePush(this.k, this.l, flags, IntPtr.Zero);
    //            if (this.p != 0)
    //            {
    //                if (buffered)
    //                {
    //                    this.g = (int)Bass.BASS_ChannelSeconds2Bytes(this.p, (double)Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD) / 500.0);
    //                }
    //                else
    //                {
    //                    this.g = -1;
    //                }
    //                if (!buffered && (flags & BASSFlag.BASS_STREAM_DECODE) == BASSFlag.BASS_DEFAULT)
    //                {
    //                    Bass.BASS_ChannelPlay(this.p, false);
    //                }
    //                this.u = true;
    //            }
    //        }
    //        Bass.BASS_SetDevice(num);
    //        return flag;
    //    }
    //    public bool RemoveFullDuplex()
    //    {
    //        if (!this.IsInputFullDuplex)
    //        {
    //            return false;
    //        }
    //        this.u = false;
    //        Bass.BASS_StreamFree(this.p);
    //        this.p = 0;
    //        this.BypassFullDuplex = false;
    //        return true;
    //    }
    //    public unsafe virtual int WasapiOutputCallback(IntPtr buffer, int length, IntPtr user)
    //    {
    //        if (this.o == 0)
    //        {
    //            return 0;
    //        }
    //        int num = Bass.BASS_ChannelGetData(this.o, buffer, length);
    //        if (num <= 0)
    //        {
    //            num = 0;
    //            if (!this.d)
    //            {
    //                this.d = true;
    //                this.a(BassWasapiHandlerSyncType.SourceStalled, this.o);
    //            }
    //        }
    //        else
    //        {
    //            if (this.d)
    //            {
    //                this.d = false;
    //                this.a(BassWasapiHandlerSyncType.SourceResumed, this.o);
    //            }
    //        }
    //        if (num > 0 && (this.e != 1f || this.f != 1f))
    //        {
    //            float* ptr = (float*)((void*)buffer);
    //            for (int i = 0; i < length / 4; i++)
    //            {
    //                if (i % 2 == 0)
    //                {
    //                    ptr[(IntPtr)i] = *(ptr + (IntPtr)i) * this.e;
    //                }
    //                else
    //                {
    //                    ptr[(IntPtr)i] = *(ptr + (IntPtr)i) * this.f;
    //                }
    //            }
    //        }
    //        return num;
    //    }
    //    public unsafe virtual int WasapiInputCallback(IntPtr buffer, int length, IntPtr user)
    //    {
    //        if (length > 0 && (this.e != 1f || this.f != 1f))
    //        {
    //            float* ptr = (float*)((void*)buffer);
    //            for (int i = 0; i < length / 4; i++)
    //            {
    //                if (i % 2 == 0)
    //                {
    //                    ptr[(IntPtr)i] = *(ptr + (IntPtr)i) * this.e;
    //                }
    //                else
    //                {
    //                    ptr[(IntPtr)i] = *(ptr + (IntPtr)i) * this.f;
    //                }
    //            }
    //        }
    //        if (this.s != 0)
    //        {
    //            Bass.BASS_ChannelGetData(this.s, buffer, length);
    //        }
    //        if (this.u && !this.h)
    //        {
    //            Bass.BASS_ChannelLock(this.p, true);
    //            int num = Bass.BASS_StreamPutData(this.p, buffer, length);
    //            if (num > 1536000)
    //            {
    //                Bass.BASS_ChannelGetData(this.p, buffer, num - 1536000);
    //            }
    //            Bass.BASS_ChannelLock(this.p, false);
    //            if (this.g > 0 && Bass.BASS_StreamPutData(this.p, IntPtr.Zero, 0) >= this.g)
    //            {
    //                Bass.BASS_ChannelPlay(this.p, false);
    //                this.g = -1;
    //            }
    //        }
    //        return 1;
    //    }
    //    //private void a(float A_0, float A_1)
    //    //{
    //    //    this.e = A_0;
    //    //    this.f = A_0;
    //    //    if (this.l > 1)
    //    //    {
    //    //        if (A_1 < 0f)
    //    //        {
    //    //            this.f = (1f + A_1) * A_0;
    //    //            return;
    //    //        }
    //    //        if (A_1 > 0f)
    //    //        {
    //    //            this.e = (1f - A_1) * A_0;
    //    //        }
    //    //    }
    //    //}
    //    //private void a(BassWasapiHandlerSyncType A_0, int A_1)
    //    //{
    //    //    if (this.v != null)
    //    //    {
    //    //        this.v(this, new BassWasapiHandlerEventArgs(A_0, A_1));
    //    //    }
    //    //}
    //}

	[Serializable]
	public class BassWasapiHandlerEventArgs : EventArgs
	{
		private readonly BassWasapiHandlerSyncType a = BassWasapiHandlerSyncType.SourceResumed;
		private readonly int b;
		public BassWasapiHandlerSyncType SyncType
		{
			get
			{
				return this.a;
			}
		}
		public int Data
		{
			get
			{
				return this.b;
			}
		}
		public BassWasapiHandlerEventArgs(BassWasapiHandlerSyncType syncType, int data)
		{
			this.a = syncType;
		}
	}
}
