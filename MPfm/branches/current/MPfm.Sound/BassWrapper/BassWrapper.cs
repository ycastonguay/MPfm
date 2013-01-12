//
// Bass.cs: This file contains a P/Invoke wrapper for the BASS audio library.
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
    public static class BASS
    {
#if IOS
        public const string DllImportValue = "__Internal";
#elif ANDROID || LINUX
        public const string DllImportValue = "libbass.so";
#elif MAC
        public const string DllImportValue = "libbass.dylib";
#else
        public const string DllImportValue = "bass.dll";
#endif

        [DllImport (DllImportValue)]
        public static extern int BASS_ErrorGetCode();
        [DllImport (DllImportValue)]
        public static extern int BASS_Init(int device, int frequency);
        [DllImport (DllImportValue)]
        public static extern int BASS_Free();
        [DllImport (DllImportValue)]
        public static extern bool BASS_Start();
        [DllImport (DllImportValue)]
        public static extern bool BASS_Pause();
        [DllImport (DllImportValue)]
        public static extern bool BASS_Stop();
        
        //[DllImport(DllImportValue)]
        //public static extern int BASS_GetDeviceCount();
        [DllImport(DllImportValue)]
        public static extern bool BASS_GetDeviceInfo(int device, ref BASS_DEVICEINFO info);
        
        [DllImport (DllImportValue)]
        public static extern bool BASS_RecordInit(int device);
        [DllImport (DllImportValue)]
        public static extern bool BASS_RecordFree();
        [DllImport (DllImportValue)]
        public static extern string BASS_RecordGetInputName(int input);
        [DllImport (DllImportValue)]
        public static extern bool BASS_RecordSetDevice(int device);
        [DllImport (DllImportValue)]
        public static extern bool BASS_RecordSetInput(int input, int flags, float volume);
        [DllImport (DllImportValue)]
        public static extern int BASS_RecordStart(int frequency, int channels, int flags, RecordProcDelegate callback, IntPtr user);
        
        [DllImport (DllImportValue)]
        public static extern int BASS_GetConfig(int option);
        [DllImport (DllImportValue)]
        public static extern bool BASS_SetConfig(int option, int value);
        [DllImport (DllImportValue)]
        public static extern IntPtr BASS_GetConfigPtr(int option);
        [DllImport (DllImportValue)]
        public static extern bool BASS_SetConfigPtr(int option, IntPtr value);
        
        [DllImport (DllImportValue)]
        public static extern bool BASS_StreamFree(int handle);
        [DllImport (DllImportValue)]
        public static extern int BASS_StreamCreateURL(string url, int offset, int flags, DownloadProcDelegate callback, IntPtr user);
        [DllImport (DllImportValue)]
        public static extern int BASS_StreamCreateFile(bool fromMemory, string filePath, long offset, long length, int flags);
        [DllImport (DllImportValue)]
        public static extern long BASS_StreamGetFilePosition(int handle, int mode);
        
        [DllImport (DllImportValue)]
        public static extern int BASS_ChannelIsActive(int handle);
        [DllImport (DllImportValue)]
        public static extern bool BASS_ChannelPlay(int handle, bool restart);
        [DllImport (DllImportValue)]
        public static extern int BASS_ChannelStop(int handle);
        [DllImport (DllImportValue)]
        public static extern bool BASS_ChannelPause(int handle);
        [DllImport (DllImportValue)]
        public static extern long BASS_ChannelGetPosition(int handle, int mode);
        [DllImport (DllImportValue)]
        public static extern long BASS_ChannelGetLength(int handle, int mode);
        [DllImport (DllImportValue)]
        public static extern bool BASS_ChannelSetAttribute(int handle, int attrib, float value);        
        
        [DllImport (DllImportValue)]
        public static extern int BASS_Encode_GetVersion();
        [DllImport (DllImportValue)]
        public static extern int BASS_Encode_StartCA(int handle, int ftype, int atype, int flags, int bitrate, EncodeProcExDelegate callback, IntPtr user);
        [DllImport (DllImportValue)]
        public static extern int BASS_Encode_StartCAFile(int handle, int ftype, int atype, int flags, int bitrate, string file);
        [DllImport (DllImportValue)]
        public static extern bool BASS_Encode_Stop(int handle);
        
        // BASS_ChannelGetLength/GetPosition/SetPosition modes
        public static int BASS_POS_BYTE          = 0;       // byte position
        public static int BASS_POS_MUSIC_ORDER   = 1;       // order.row position, MAKELONG(order,row)
        public static int BASS_POS_DECODE        = 0x10000000; // flag: get the decoding (not playing) position
        public static int BASS_POS_DECODETO      = 0x20000000; // flag: decode to the position instead of seeking
        
        public static int COCOA_AUDIOUNIT_ADTS = 1633973363; // 'adts'
        public static int COCOA_AUDIOUNIT_MP4F = 1836069990; // 'mp4f'
        public static int COCOA_AUDIOUNIT_M4AF = 1832149350; // 'm4af'
        public static int COCOA_AUDIOUNIT_AAC = 1633772320; // 'aac '
        public static int COCOA_AUDIOUNIT_ALAC = 1634492771; // 'alac'
        
        public static int BASS_OK             = 0;   // all is OK
        public static int BASS_ERROR_MEM      = 1;   // memory error
        public static int BASS_ERROR_FILEOPEN = 2;   // can't open the file
        public static int BASS_ERROR_DRIVER   = 3;   // can't find a free/valid driver
        public static int BASS_ERROR_BUFLOST  = 4;   // the sample buffer was lost
        public static int BASS_ERROR_HANDLE   = 5;   // invalid handle
        public static int BASS_ERROR_FORMAT   = 6;   // unsupported sample format
        public static int BASS_ERROR_POSITION = 7;   // invalid position
        public static int BASS_ERROR_INIT     = 8;   // BASS_Init has not been successfully called
        public static int BASS_ERROR_START    = 9;   // BASS_Start has not been successfully called
        public static int BASS_ERROR_ALREADY  = 14;  // already initialized/paused/whatever
        public static int BASS_ERROR_NOCHAN   = 18;  // can't get a free channel
        public static int BASS_ERROR_ILLTYPE  = 19;  // an illegal type was specified
        public static int BASS_ERROR_ILLPARAM = 20;  // an illegal parameter was specified
        public static int BASS_ERROR_NO3D     = 21;  // no 3D support
        public static int BASS_ERROR_NOEAX    = 22;  // no EAX support
        public static int BASS_ERROR_DEVICE   = 23;  // illegal device number
        public static int BASS_ERROR_NOPLAY   = 24;  // not playing
        public static int BASS_ERROR_FREQ     = 25;  // illegal sample rate
        public static int BASS_ERROR_NOTFILE  = 27;  // the stream is not a file stream
        public static int BASS_ERROR_NOHW     = 29;  // no hardware voices available
        public static int BASS_ERROR_EMPTY    = 31;  // the MOD music has no sequence data
        public static int BASS_ERROR_NONET    = 32;  // no internet connection could be opened
        public static int BASS_ERROR_CREATE   = 33;  // couldn't create the file
        public static int BASS_ERROR_NOFX     = 34;  // effects are not available
        public static int BASS_ERROR_NOTAVAIL = 37;  // requested data is not available
        public static int BASS_ERROR_DECODE   = 38;  // the channel is a "decoding channel"
        public static int BASS_ERROR_DX       = 39;  // a sufficient DirectX version is not installed
        public static int BASS_ERROR_TIMEOUT  = 40;  // connection timedout
        public static int BASS_ERROR_FILEFORM = 41;  // unsupported file format
        public static int BASS_ERROR_SPEAKER  = 42;  // unavailable speaker
        public static int BASS_ERROR_VERSION  = 43;  // invalid BASS version (used by add-ons)
        public static int BASS_ERROR_CODEC    = 44;  // codec is not available/supported
        public static int BASS_ERROR_ENDED    = 45;  // the channel/file has ended
        public static int BASS_ERROR_BUSY     = 46;  // the device is busy
        
        // BASS_SetConfig options
        public static int BASS_CONFIG_BUFFER          = 0;
        public static int BASS_CONFIG_UPDATEPERIOD    = 1;
        public static int BASS_CONFIG_GVOL_SAMPLE     = 4;
        public static int BASS_CONFIG_GVOL_STREAM     = 5;
        public static int BASS_CONFIG_GVOL_MUSIC      = 6;
        public static int BASS_CONFIG_CURVE_VOL       = 7;
        public static int BASS_CONFIG_CURVE_PAN       = 8;
        public static int BASS_CONFIG_FLOATDSP        = 9;
        public static int BASS_CONFIG_3DALGORITHM     = 10;
        public static int BASS_CONFIG_NET_TIMEOUT     = 11;
        public static int BASS_CONFIG_NET_BUFFER      = 12;
        public static int BASS_CONFIG_PAUSE_NOPLAY    = 13;
        public static int BASS_CONFIG_NET_PREBUF      = 15;
        public static int BASS_CONFIG_NET_PASSIVE     = 18;
        public static int BASS_CONFIG_REC_BUFFER      = 19;
        public static int BASS_CONFIG_NET_PLAYLIST    = 21;
        public static int BASS_CONFIG_MUSIC_VIRTUAL   = 22;
        public static int BASS_CONFIG_VERIFY          = 23;
        public static int BASS_CONFIG_UPDATETHREADS   = 24;
        public static int BASS_CONFIG_DEV_BUFFER      = 27;
        public static int BASS_CONFIG_IOS_MIXAUDIO    = 34;
        public static int BASS_CONFIG_DEV_DEFAULT     = 36;
        public static int BASS_CONFIG_NET_READTIMEOUT = 37;
        public static int BASS_CONFIG_IOS_SPEAKER     = 39;
        public static int BASS_CONFIG_IOS_NOTIFY      = 46;
        
        public static int BASS_STREAM_PRESCAN     = 0x20000; // enable pin-point seeking/length (MP3/MP2/MP1)
        public static int BASS_MP3_SETPOS         = BASS_STREAM_PRESCAN;
        public static int BASS_STREAM_AUTOFREE    = 0x40000; // automatically free the stream when it stop/ends
        public static int BASS_STREAM_RESTRATE    = 0x80000; // restrict the download rate of internet file streams
        public static int BASS_STREAM_BLOCK       = 0x100000; // download/play internet file stream in small blocks
        public static int BASS_STREAM_DECODE      = 0x200000; // don't play the stream, only decode (BASS_ChannelGetData)
        public static int BASS_STREAM_STATUS      = 0x800000; // give server status info (HTTP/ICY tags) in DOWNLOADPROC
        
        // Channel attributes
        public static int BASS_ATTRIB_FREQ             = 1;
        public static int BASS_ATTRIB_VOL              = 2;
        public static int BASS_ATTRIB_PAN              = 3;
        public static int BASS_ATTRIB_EAXMIX           = 4;
        public static int BASS_ATTRIB_NOBUFFER         = 5;
        public static int BASS_ATTRIB_CPU              = 7;
        public static int BASS_ATTRIB_MUSIC_AMPLIFY    = 0x100;
        public static int BASS_ATTRIB_MUSIC_PANSEP     = 0x101;
        public static int BASS_ATTRIB_MUSIC_PSCALER    = 0x102;
        public static int BASS_ATTRIB_MUSIC_BPM        = 0x103;
        public static int BASS_ATTRIB_MUSIC_SPEED      = 0x104;
        public static int BASS_ATTRIB_MUSIC_VOL_GLOBAL = 0x105;
        public static int BASS_ATTRIB_MUSIC_VOL_CHAN   = 0x200; // + channel #
        public static int BASS_ATTRIB_MUSIC_VOL_INST   = 0x300; // + instrument #
        
        // Active attributes
        public static int BASS_ACTIVE_STOPPED = 0;
        public static int BASS_ACTIVE_PLAYING = 1;
        public static int BASS_ACTIVE_STALLED = 2;
        public static int BASS_ACTIVE_PAUSED = 3;
        
        // File position attributes
        public static int BASS_FILEPOS_CURRENT   = 0;
        public static int BASS_FILEPOS_DECODE    = BASS_FILEPOS_CURRENT;
        public static int BASS_FILEPOS_DOWNLOAD  = 1;
        public static int BASS_FILEPOS_END       = 2;
        public static int BASS_FILEPOS_START     = 3;
        public static int BASS_FILEPOS_CONNECTED = 4;
        public static int BASS_FILEPOS_BUFFER    = 5;
        public static int BASS_FILEPOS_SOCKET    = 6;
        
        public static int BASS_SAMPLE_8BITS      = 1;   // 8 bit
        public static int BASS_SAMPLE_FLOAT      = 256; // 32-bit floating-point
        public static int BASS_SAMPLE_MONO       = 2;   // mono
        public static int BASS_SAMPLE_LOOP       = 4;   // looped
        public static int BASS_SAMPLE_3D         = 8;   // 3D functionality
        public static int BASS_SAMPLE_SOFTWARE   = 16;  // not using hardware mixing
        public static int BASS_SAMPLE_MUTEMAX    = 32;  // mute at max distance (3D only)
        public static int BASS_SAMPLE_VAM        = 64;  // DX7 voice allocation & management
        public static int BASS_SAMPLE_FX         = 128; // old implementation of DX8 effects
        public static int BASS_SAMPLE_OVER_VOL   = 0x10000; // override lowest volume
        public static int BASS_SAMPLE_OVER_POS   = 0x20000; // override longest playing
        public static int BASS_SAMPLE_OVER_DIST  = 0x30000; // override furthest from listener (3D only)
        
        // BASS_RecordSetInput flags
        public static int BASS_INPUT_OFF     = 0x10000;
        public static int BASS_INPUT_ON      = 0x20000;
        
        // BASS_Encode_Start flags
        public static int BASS_ENCODE_NOHEAD       = 1;   // don't send a WAV header to the encoder
        public static int BASS_ENCODE_FP_8BIT      = 2;   // convert floating-point sample data to 8-bit integer
        public static int BASS_ENCODE_FP_16BIT     = 4;   // convert floating-point sample data to 16-bit integer
        public static int BASS_ENCODE_FP_24BIT     = 6;   // convert floating-point sample data to 24-bit integer
        public static int BASS_ENCODE_FP_32BIT     = 8;   // convert floating-point sample data to 32-bit integer
        public static int BASS_ENCODE_BIGEND       = 16;  // big-endian sample data
        public static int BASS_ENCODE_PAUSE        = 32;  // start encording paused
        public static int BASS_ENCODE_PCM          = 64;  // write PCM sample data (no encoder)
        public static int BASS_ENCODE_RF64         = 128; // send an RF64 header
        public static int BASS_ENCODE_MONO         = 256; // convert to mono (if not already)
        public static int BASS_ENCODE_QUEUE        = 512; // queue data to feed encoder asynchronously
        public static int BASS_ENCODE_CAST_NOLIMIT = 0x1000; // don't limit casting data rate
        public static int BASS_ENCODE_LIMIT        = 0x2000;  // limit data rate to real-time
        public static int BASS_ENCODE_AUTOFREE     = 0x40000; // free the encoder when the channel is freed
        
        public static int BASS_RECORD_PAUSE        = 0x8000;
        
        public const int BASS_IOSNOTIFY_INTERRUPT     = 1;
        public const int BASS_IOSNOTIFY_INTERRUPT_END = 2;        
    }
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DownloadProcDelegate(IntPtr buffer, int length, IntPtr user);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool RecordProcDelegate(int handle, IntPtr buffer, int length, IntPtr user);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EncodeProcExDelegate(int handle, int channel, IntPtr buffer, int length, int offset, IntPtr user);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IOSNotifyProcDelegate(int status);
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct BASS_DEVICEINFO
    {
        public string name;
        public string driver;
        public uint flags;
    }
}
