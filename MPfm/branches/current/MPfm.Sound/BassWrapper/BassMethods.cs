//
// BassMethods.cs: This file contains methods for the P/Invoke wrapper of the BASS audio library.
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

        [DllImport(DllImportValue)]
        public static extern int BASS_ErrorGetCode();

        [DllImport(DllImportValue)]
        public static extern int BASS_Init(int device, int frequency);

        [DllImport(DllImportValue)]
        public static extern int BASS_Free();

        [DllImport(DllImportValue)]
        public static extern bool BASS_Start();

        [DllImport(DllImportValue)]
        public static extern bool BASS_Pause();

        [DllImport(DllImportValue)]
        public static extern bool BASS_Stop();

        //[DllImport(DllImportValue)]
        //public static extern int BASS_GetDeviceCount();
        [DllImport(DllImportValue)]
        public static extern bool BASS_GetDeviceInfo(int device, ref BASSDeviceInfo info);

        [DllImport(DllImportValue)]
        public static extern bool BASS_RecordInit(int device);

        [DllImport(DllImportValue)]
        public static extern bool BASS_RecordFree();

        [DllImport(DllImportValue)]
        public static extern string BASS_RecordGetInputName(int input);

        [DllImport(DllImportValue)]
        public static extern bool BASS_RecordSetDevice(int device);

        [DllImport(DllImportValue)]
        public static extern bool BASS_RecordSetInput(int input, int flags, float volume);

        [DllImport(DllImportValue)]
        public static extern int BASS_RecordStart(int frequency, int channels, int flags, RECORDPROC callback,
                                                  IntPtr user);

        [DllImport(DllImportValue)]
        public static extern int BASS_GetConfig(int option);

        [DllImport(DllImportValue)]
        public static extern bool BASS_SetConfig(int option, int value);

        [DllImport(DllImportValue)]
        public static extern IntPtr BASS_GetConfigPtr(int option);

        [DllImport(DllImportValue)]
        public static extern bool BASS_SetConfigPtr(int option, IntPtr value);

        [DllImport(DllImportValue)]
        public static extern bool BASS_StreamFree(int handle);

        [DllImport(DllImportValue)]
        public static extern int BASS_StreamCreateURL(string url, int offset, int flags, DOWNLOADPROC callback,
                                                      IntPtr user);

        [DllImport(DllImportValue)]
        public static extern int BASS_StreamCreateFile(bool fromMemory, string filePath, long offset, long length,
                                                       int flags);

        [DllImport(DllImportValue)]
        public static extern long BASS_StreamGetFilePosition(int handle, int mode);

        [DllImport(DllImportValue)]
        public static extern int BASS_ChannelIsActive(int handle);

        [DllImport(DllImportValue)]
        public static extern bool BASS_ChannelPlay(int handle, bool restart);

        [DllImport(DllImportValue)]
        public static extern int BASS_ChannelStop(int handle);

        [DllImport(DllImportValue)]
        public static extern bool BASS_ChannelPause(int handle);

        [DllImport(DllImportValue)]
        public static extern long BASS_ChannelGetPosition(int handle, int mode);

        [DllImport(DllImportValue)]
        public static extern long BASS_ChannelGetLength(int handle, int mode);

        [DllImport(DllImportValue)]
        public static extern bool BASS_ChannelSetAttribute(int handle, int attrib, float value);

        [DllImport(DllImportValue)]
        public static extern int BASS_Encode_GetVersion();

        [DllImport(DllImportValue)]
        public static extern int BASS_Encode_StartCA(int handle, int ftype, int atype, int flags, int bitrate,
                                                     ENCODEPROC callback, IntPtr user);

        [DllImport(DllImportValue)]
        public static extern int BASS_Encode_StartCAFile(int handle, int ftype, int atype, int flags, int bitrate,
                                                         string file);

        [DllImport(DllImportValue)]
        public static extern bool BASS_Encode_Stop(int handle);
    }
}
