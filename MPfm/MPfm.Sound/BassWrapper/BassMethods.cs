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
using System.Runtime.InteropServices;
using System.Security;

namespace MPfm.Sound.BassWrapper
{
	[SuppressUnmanagedCodeSecurity]
    public sealed class Bass
    {
		public const int BASSVERSION = 516;
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win, Guid clsid);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool BASS_Init(int A_0, int A_1, BASSInit A_2, IntPtr A_3, IntPtr A_4);
		public static bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win)
		{
			return Bass.BASS_Init(device, freq, flags, win, IntPtr.Zero);
		}
        //[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_GetDeviceInfo")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool BASS_GetDeviceInfoInternal([In] int A_0, [In] [Out] ref d A_1);
        //public static bool BASS_GetDeviceInfo(int device, BASS_DEVICEINFO info)
        //{
        //    bool flag = Bass.BASS_GetDeviceInfoInternal(device, ref info.a);
        //    if (flag)
        //    {
        //        info.name = Marshal.PtrToStringAnsi(info.a.a);
        //        info.driver = Marshal.PtrToStringAnsi(info.a.b);
        //        info.flags = info.a.c;
        //    }
        //    return flag;
        //}
        //public static BASS_DEVICEINFO BASS_GetDeviceInfo(int device)
        //{
        //    //BASS_DEVICEINFO bASS_DEVICEINFO = new BASS_DEVICEINFO();
        //    //if (Bass.BASS_GetDeviceInfo(device, bASS_DEVICEINFO))
        //    //{
        //    //    return bASS_DEVICEINFO;
        //    //}
        //    return null;
        //}

        [DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_GetDeviceInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BASS_GetDeviceInfoInternal([In] int A_0, [In] [Out] ref BASS_DEVICEINFO_TEMP A_1);
        public static bool BASS_GetDeviceInfo(int device, ref BASS_DEVICEINFO info)
        {
            BASS_DEVICEINFO_TEMP temp = new BASS_DEVICEINFO_TEMP();
            bool success = Bass.BASS_GetDeviceInfoInternal(device, ref temp);
            if (success)
            {
                info.name = Marshal.PtrToStringAnsi(temp.a);
                info.driver = Marshal.PtrToStringAnsi(temp.b);
                info.flags = temp.c;
            }
            return success;
        }
        public static BASS_DEVICEINFO[] BASS_GetDeviceInfos()
        {
            List<BASS_DEVICEINFO> list = new List<BASS_DEVICEINFO>();
            int num = 0;
            BASS_DEVICEINFO tempItem = new BASS_DEVICEINFO();
            while (true)
            {
                bool success = Bass.BASS_GetDeviceInfo(num, ref tempItem);
                if(!success)
                    break;

                // Create a new item (or the list will be full of the same item reference)
                BASS_DEVICEINFO info = new BASS_DEVICEINFO();
                info.name = tempItem.name;
                info.driver = tempItem.driver;
                info.flags = tempItem.flags;
                list.Add(info);
                num++;
            }
            Bass.BASS_GetCPU();
            return list.ToArray();
        }
        //public static int BASS_GetDeviceCount()
        //{
        //    BASS_DEVICEINFO info = new BASS_DEVICEINFO();
        //    int num = 0;
        //    //while (Bass.BASS_GetDeviceInfo(num, info))
        //    //{
        //    //    num++;
        //    //}
        //    //Bass.BASS_GetCPU();
        //    return num;
        //}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_GetInfo([In] [Out] BASS_INFO info);
		public static BASS_INFO BASS_GetInfo()
		{
			BASS_INFO bASS_INFO = new BASS_INFO();
			if (Bass.BASS_GetInfo(bASS_INFO))
			{
				return bASS_INFO;
			}
			return null;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern BASSError BASS_ErrorGetCode();
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Stop();
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Free();
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_GetVersion();
		public static Version BASS_GetVersion(int fieldcount)
		{
			if (fieldcount < 1)
			{
				fieldcount = 1;
			}
			if (fieldcount > 4)
			{
				fieldcount = 4;
			}
			int num = Bass.BASS_GetVersion();
			Version result = new Version(2, 3);
			switch (fieldcount)
			{
			case 1:
				result = new Version(num >> 24 & 255, 0);
				break;
			case 2:
				result = new Version(num >> 24 & 255, num >> 16 & 255);
				break;
			case 3:
				result = new Version(num >> 24 & 255, num >> 16 & 255, num >> 8 & 255);
				break;
			case 4:
				result = new Version(num >> 24 & 255, num >> 16 & 255, num >> 8 & 255, num & 255);
				break;
			}
			return result;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetDevice(int device);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_GetDevice();
//		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
//		public static extern IntPtr BASS_GetDSoundObject(int handle);
//		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
//		public static extern IntPtr BASS_GetDSoundObject(BASSDirectSound dsobject);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Update(int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern float BASS_GetCPU();
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Start();
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Pause();
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetVolume(float volume);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern float BASS_GetVolume();
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetConfig(BASSConfig option, int newvalue);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetConfig(BASSConfig option, [MarshalAs(UnmanagedType.Bool)] [In] bool newvalue);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetConfigPtr(BASSConfig option, IntPtr newvalue);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_GetConfig(BASSConfig option);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_GetConfig")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_GetConfigBool(BASSConfig option);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern IntPtr BASS_GetConfigPtr(BASSConfig option);
		public static string BASS_GetConfigString(BASSConfig option)
		{
			IntPtr intPtr = Bass.BASS_GetConfigPtr(option);
			if (intPtr != IntPtr.Zero)
			{
				return Marshal.PtrToStringAnsi(intPtr);
			}
			return null;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_PluginLoad")]
		private static extern int BASS_PluginLoadUnicode([MarshalAs(UnmanagedType.LPWStr)] [In] string A_0, BASSFlag A_1);
		public static int BASS_PluginLoad(string file)
		{
			return Bass.BASS_PluginLoadUnicode(file, BASSFlag.BASS_UNICODE);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_PluginFree(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_SampleLoad")]
		private static extern int BASS_SampleLoadUnicode([MarshalAs(UnmanagedType.Bool)] bool A_0, [MarshalAs(UnmanagedType.LPWStr)] [In] string A_1, long A_2, int A_3, int A_4, BASSFlag A_5);
		public static int BASS_SampleLoad(string file, long offset, int length, int max, BASSFlag flags)
		{
			flags |= BASSFlag.BASS_UNICODE;
			return Bass.BASS_SampleLoadUnicode(false, file, offset, length, max, flags);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_SampleLoad")]
		private static extern int BASS_SampleLoadMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, IntPtr A_1, long A_2, int A_3, int A_4, BASSFlag A_5);
		public static int BASS_SampleLoad(IntPtr memory, long offset, int length, int max, BASSFlag flags)
		{
			return Bass.BASS_SampleLoadMemory(true, memory, offset, length, max, flags);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_SampleLoad")]
		private static extern int BASS_SampleLoadMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, byte[] A_1, long A_2, int A_3, int A_4, BASSFlag A_5);
		public static int BASS_SampleLoad(byte[] memory, long offset, int length, int max, BASSFlag flags)
		{
			return Bass.BASS_SampleLoadMemory(true, memory, offset, length, max, flags);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_SampleCreate(int length, int freq, int chans, int max, BASSFlag flags);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, IntPtr buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, float[] buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, int[] buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, short[] buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, byte[] buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, IntPtr buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, float[] buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, int[] buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, short[] buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, byte[] buffer);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleFree(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetInfo(int handle, [In] [Out] BASS_SAMPLE info);
		public static BASS_SAMPLE BASS_SampleGetInfo(int handle)
		{
			BASS_SAMPLE bASS_SAMPLE = new BASS_SAMPLE();
			if (Bass.BASS_SampleGetInfo(handle, bASS_SAMPLE))
			{
				return bASS_SAMPLE;
			}
			return null;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetInfo(int handle, [In] BASS_SAMPLE info);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_SampleGetChannel(int handle, [MarshalAs(UnmanagedType.Bool)] bool onlynew);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_SampleGetChannels(int handle, int[] channels);
		public static int[] BASS_SampleGetChannels(int handle)
		{
			BASS_SAMPLE bASS_SAMPLE = Bass.BASS_SampleGetInfo(handle);
			int[] array = new int[bASS_SAMPLE.max];
			int num = Bass.BASS_SampleGetChannels(handle, array);
			if (num >= 0)
			{
				int[] array2 = new int[num];
				Array.Copy(array, array2, num);
				return array2;
			}
			return null;
		}
		public static int BASS_SampleGetChannelCount(int handle)
		{
			return Bass.BASS_SampleGetChannels(handle, null);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleStop(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamCreate(int freq, int chans, BASSFlag flags, STREAMPROC proc, IntPtr user);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreate")]
		private static extern int BASS_StreamCreatePtr(int A_0, int A_1, BASSFlag A_2, IntPtr A_3, IntPtr A_4);
		public static int BASS_StreamCreate(int freq, int chans, BASSFlag flags, BASSStreamProc proc)
		{
			return Bass.BASS_StreamCreatePtr(freq, chans, flags, new IntPtr((int)proc), IntPtr.Zero);
		}
		public static int BASS_StreamCreateDummy(int freq, int chans, BASSFlag flags, IntPtr user)
		{
			return Bass.BASS_StreamCreatePtr(freq, chans, flags, IntPtr.Zero, user);
		}
		public static int BASS_StreamCreatePush(int freq, int chans, BASSFlag flags, IntPtr user)
		{
			return Bass.BASS_StreamCreatePtr(freq, chans, flags, new IntPtr(-1), user);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreateFile")]
		private static extern int BASS_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool A_0, [MarshalAs(UnmanagedType.LPWStr)] [In] string A_1, long A_2, long A_3, BASSFlag A_4);
		public static int BASS_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
		{
			flags |= BASSFlag.BASS_UNICODE;
			return Bass.BASS_StreamCreateFileUnicode(false, file, offset, length, flags);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreateFile")]
		private static extern int BASS_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, IntPtr A_1, long A_2, long A_3, BASSFlag A_4);
		public static int BASS_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
		{
			return Bass.BASS_StreamCreateFileMemory(true, memory, offset, length, flags);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreateURL")]
		private static extern int BASS_StreamCreateURLUnicode([MarshalAs(UnmanagedType.LPWStr)] [In] string A_0, int A_1, BASSFlag A_2, DOWNLOADPROC A_3, IntPtr A_4);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreateURL")]
		private static extern int BASS_StreamCreateURLAscii([MarshalAs(UnmanagedType.LPStr)] [In] string A_0, int A_1, BASSFlag A_2, DOWNLOADPROC A_3, IntPtr A_4);
		public static int BASS_StreamCreateURL(string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user)
		{
			if (Bass.BASS_GetVersion() >= 33817870)
			{
				flags |= BASSFlag.BASS_UNICODE;
				int num = Bass.BASS_StreamCreateURLUnicode(url, offset, flags, proc, user);
				if (num == 0)
				{
					flags &= (BASSFlag)2147483647;
					num = Bass.BASS_StreamCreateURLAscii(url, offset, flags, proc, user);
				}
				return num;
			}
			flags &= (BASSFlag)2147483647;
			return Bass.BASS_StreamCreateURLAscii(url, offset, flags, proc, user);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern long BASS_StreamGetFilePosition(int handle, BASSStreamFilePosition mode);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_StreamFree(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, IntPtr buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, float[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, int[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, short[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, byte[] buffer, int length);
        //public unsafe static int BASS_StreamPutData(int handle, byte[] buffer, int startIdx, int length)
        //{
        //    return Bass.BASS_StreamPutData(handle, new IntPtr((void*)(&buffer[startIdx])), length);
        //}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, IntPtr buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, float[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, int[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, short[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, byte[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_MusicLoad")]
		private static extern int BASS_MusicLoadUnicode([MarshalAs(UnmanagedType.Bool)] bool A_0, [MarshalAs(UnmanagedType.LPWStr)] [In] string A_1, long A_2, int A_3, BASSFlag A_4, int A_5);
		public static int BASS_MusicLoad(string file, long offset, int length, BASSFlag flags, int freq)
		{
			flags |= BASSFlag.BASS_UNICODE;
			return Bass.BASS_MusicLoadUnicode(false, file, offset, length, flags, freq);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_MusicLoad")]
		private static extern int BASS_MusicLoadMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, IntPtr A_1, long A_2, int A_3, BASSFlag A_4, int A_5);
		public static int BASS_MusicLoad(IntPtr memory, long offset, int length, BASSFlag flags, int freq)
		{
			return Bass.BASS_MusicLoadMemory(true, memory, offset, length, flags, freq);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_MusicLoad")]
		private static extern int BASS_MusicLoadMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, byte[] A_1, long A_2, int A_3, BASSFlag A_4, int A_5);
		public static int BASS_MusicLoad(byte[] memory, long offset, int length, BASSFlag flags, int freq)
		{
			return Bass.BASS_MusicLoadMemory(true, memory, offset, length, flags, freq);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_MusicFree(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_RecordInit(int device);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_RecordStart(int freq, int chans, BASSFlag flags, RECORDPROC proc, IntPtr user);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_RecordSetDevice(int device);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_RecordGetDevice();
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_RecordGetInfo([In] [Out] BASS_RECORDINFO info);
		public static BASS_RECORDINFO BASS_RecordGetInfo()
		{
			BASS_RECORDINFO bASS_RECORDINFO = new BASS_RECORDINFO();
			if (Bass.BASS_RecordGetInfo(bASS_RECORDINFO))
			{
				return bASS_RECORDINFO;
			}
			return null;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_RecordGetInputName")]
		private static extern IntPtr BASS_RecordGetInputNamePtr(int A_0);
		public static string BASS_RecordGetInputName(int input)
		{
			IntPtr intPtr = Bass.BASS_RecordGetInputNamePtr(input);
			if (intPtr != IntPtr.Zero)
			{
				return Marshal.PtrToStringAnsi(intPtr);
			}
			return null;
		}
		public static string[] BASS_RecordGetInputNames()
		{
			List<string> list = new List<string>();
			int num = 0;
			string item;
			while ((item = Bass.BASS_RecordGetInputName(num)) != null)
			{
				list.Add(item);
				num++;
			}
			Bass.BASS_GetCPU();
			return list.ToArray();
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_RecordSetInput(int input, BASSInput setting, float volume);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_RecordGetInput(int input, ref float volume);
		public static BASSInput BASS_RecordGetInput(int input)
		{
			int num = Bass.BASS_RecordGetInputPtr(input, IntPtr.Zero);
			if (num != -1)
			{
				return (BASSInput)(num & 16711680);
			}
			return BASSInput.BASS_INPUT_NONE;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_RecordGetInput")]
		private static extern int BASS_RecordGetInputPtr(int A_0, IntPtr A_1);
		public static BASSInputType BASS_RecordGetInputType(int input)
		{
			int num = Bass.BASS_RecordGetInputPtr(input, IntPtr.Zero);
			if (num != -1)
			{
				return (BASSInputType)(num & -16777216);
			}
			return BASSInputType.BASS_INPUT_TYPE_ERROR;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_RecordFree();

        [DllImport(BassWrapperGlobals.DllImportValue_Bass)]
        public static extern bool BASS_ChannelGetInfo(int device, [In] [Out] ref BASS_CHANNELINFO info);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelPlay(int handle, [MarshalAs(UnmanagedType.Bool)] bool restart);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelSetDSP(int handle, DSPPROC proc, IntPtr user, int priority);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, IntPtr buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, [In] [Out] float[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, [In] [Out] short[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, [In] [Out] int[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, [In] [Out] byte[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern long BASS_ChannelSeconds2Bytes(int handle, double pos);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern double BASS_ChannelBytes2Seconds(int handle, long pos);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern BASSActive BASS_ChannelIsActive(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelLock(int handle, [MarshalAs(UnmanagedType.Bool)] bool state);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern long BASS_ChannelGetLength(int handle, BASSMode mode);
		public static long BASS_ChannelGetLength(int handle)
		{
			return Bass.BASS_ChannelGetLength(handle, BASSMode.BASS_POS_BYTES);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelSetSync(int handle, BASSSync type, long param, SYNCPROC proc, IntPtr user);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelSetFX(int handle, BASSFXType type, int priority);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetDevice(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSetDevice(int handle, int device);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelStop(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelPause(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSetAttribute(int handle, BASSAttribute attrib, float value);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelGetAttribute(int handle, BASSAttribute attrib, ref float value);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern BASSFlag BASS_ChannelFlags(int handle, BASSFlag flags, BASSFlag mask);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelUpdate(int handle, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelIsSliding(int handle, BASSAttribute attrib);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSlideAttribute(int handle, BASSAttribute attrib, float value, int time);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSet3DAttributes(int handle, BASS3DMode mode, float min, float max, int iangle, int oangle, int outvol);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelGet3DAttributes(int handle, ref BASS3DMode mode, ref float min, ref float max, ref int iangle, ref int oangle, ref int outvol);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelGet3DAttributes(int handle, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object mode, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object min, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object max, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object iangle, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object oangle, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object outvol);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSet3DPosition(int handle, [In] BASS_3DVECTOR pos, [In] BASS_3DVECTOR orient, [In] BASS_3DVECTOR vel);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelGet3DPosition(int handle, [In] [Out] BASS_3DVECTOR pos, [In] [Out] BASS_3DVECTOR orient, [In] [Out] BASS_3DVECTOR vel);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSetPosition(int handle, long pos, BASSMode mode);
		public static bool BASS_ChannelSetPosition(int handle, long pos)
		{
			return Bass.BASS_ChannelSetPosition(handle, pos, BASSMode.BASS_POS_BYTES);
		}
		public static bool BASS_ChannelSetPosition(int handle, double seconds)
		{
			return Bass.BASS_ChannelSetPosition(handle, Bass.BASS_ChannelSeconds2Bytes(handle, seconds), BASSMode.BASS_POS_BYTES);
		}
        //public static bool BASS_ChannelSetPosition(int handle, int order, int row)
        //{
        //    return Bass.BASS_ChannelSetPosition(handle, (long)Utils.MakeLong(order, row), BASSMode.BASS_POS_MUSIC_ORDERS);
        //}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern long BASS_ChannelGetPosition(int handle, BASSMode mode);
		public static long BASS_ChannelGetPosition(int handle)
		{
			return Bass.BASS_ChannelGetPosition(handle, BASSMode.BASS_POS_BYTES);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetLevel(int handle);
		public static bool BASS_ChannelGetLevel(int handle, float[] level)
		{
			if (level.Length <= 0)
			{
				return false;
			}
			Array.Clear(level, 0, level.Length);
			int num = (int)Bass.BASS_ChannelSeconds2Bytes(handle, 0.02);
			if (num > 0)
			{
				float[] array = new float[num / 4];
				num = Bass.BASS_ChannelGetData(handle, array, num | 1073741824);
				num /= 4;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					float num3 = Math.Abs(array[i]);
					if (num3 > level[num2])
					{
						level[num2] = num3;
					}
					num2++;
					if (num2 >= level.Length)
					{
						num2 = 0;
					}
				}
				return true;
			}
			return false;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelRemoveSync(int handle, int sync);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelRemoveDSP(int handle, int dsp);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelRemoveFX(int handle, int fx);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSetLink(int handle, int chan);
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelRemoveLink(int handle, int chan);

        [DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FXGetParameters(int handle, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object par);
        [DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FXSetParameters(int handle, [MarshalAs(UnmanagedType.AsAny)] [In] object par);
        [DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FXReset(int handle);
    }
}
