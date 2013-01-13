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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace MPfm.Sound.BassWrapper
{
	[SuppressUnmanagedCodeSecurity]
    public sealed class Bass
    {
		public const int BASSVERSION = 516;
        //public const int FALSE = 0;
        //public const int TRUE = 1;
        //public const int ERROR = -1;
        //public static string SupportedStreamExtensions;
        //public static string SupportedStreamName;
        //public static string SupportedMusicExtensions;
        //private static int a;
        //private static string b;
        //static Bass()
        //{
        //    Bass.SupportedStreamExtensions = "*.mp3;*.ogg;*.wav;*.mp2;*.mp1;*.aiff;*.m2a;*.mpa;*.m1a;*.mpg;*.mpeg;*.aif;*.mp3pro;*.bwf;*.mus";
        //    Bass.SupportedStreamName = "WAV/AIFF/MP3/MP2/MP1/OGG";
        //    Bass.SupportedMusicExtensions = "*.mod;*.mo3;*.s3m;*.xm;*.it;*.mtm;*.umx;*.mdz;*.s3z;*.itz;*.xmz";
        //    Bass.a = 0;
        //    Bass.b = "bass.dll";
        //    if (!BassNet.OmitCheckVersion)
        //    {
        //        Bass.a();
        //    }
        //    if (!BassNet.IsRegistered)
        //    {
        //        ThreadPool.QueueUserWorkItem(new WaitCallback(Bass.a), null);
        //    }
        //}
        //private static void a(object A_0)
        //{
        //    Un4seen.Bass.a a = new Un4seen.Bass.a(true, 0);
        //    a.Show();
        //    Application.DoEvents();
        //    Thread.Sleep(a.j);
        //    if (!a.IsDisposed)
        //    {
        //        a.Invoke(new MethodInvoker(a.Close));
        //    }
        //    a.Dispose();
        //}
		[DllImport(BassWrapperGlobals.DllImportValue_Bass, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win, Guid clsid);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool BASS_Init(int A_0, int A_1, BASSInit A_2, IntPtr A_3, IntPtr A_4);
		public static bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win)
		{
			return Bass.BASS_Init(device, freq, flags, win, IntPtr.Zero);
		}
        //[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_GetDeviceInfo")]
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

        [DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_GetDeviceInfo")]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern BASSError BASS_ErrorGetCode();
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Stop();
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Free();
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetDevice(int device);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_GetDevice();
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr BASS_GetDSoundObject(int handle);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr BASS_GetDSoundObject(BASSDirectSound dsobject);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Update(int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern float BASS_GetCPU();
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Start();
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Pause();
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetVolume(float volume);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern float BASS_GetVolume();
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetConfig(BASSConfig option, int newvalue);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetConfig(BASSConfig option, [MarshalAs(UnmanagedType.Bool)] [In] bool newvalue);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SetConfigPtr(BASSConfig option, IntPtr newvalue);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_GetConfig(BASSConfig option);
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_GetConfig")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_GetConfigBool(BASSConfig option);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_PluginLoad")]
		private static extern int BASS_PluginLoadUnicode([MarshalAs(UnmanagedType.LPWStr)] [In] string A_0, BASSFlag A_1);
		public static int BASS_PluginLoad(string file)
		{
			return Bass.BASS_PluginLoadUnicode(file, BASSFlag.BASS_UNICODE);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_PluginFree(int handle);
        //public static Dictionary<int, string> BASS_PluginLoadDirectory(string dir)
        //{
        //    Dictionary<int, string> dictionary = new Dictionary<int, string>();
        //    string[] files = Directory.GetFiles(dir, "bass*.dll");
        //    if (files == null || files.Length == 0)
        //    {
        //        files = Directory.GetFiles(dir, "libbass*.so");
        //    }
        //    if (files == null || files.Length == 0)
        //    {
        //        files = Directory.GetFiles(dir, "libbass*.dylib");
        //    }
        //    if (files != null)
        //    {
        //        string[] array = files;
        //        for (int i = 0; i < array.Length; i++)
        //        {
        //            string text = array[i];
        //            int num = Bass.BASS_PluginLoad(text);
        //            if (num > 0)
        //            {
        //                dictionary.Add(num, text);
        //            }
        //        }
        //    }
        //    Bass.BASS_GetCPU();
        //    if (dictionary.Count > 0)
        //    {
        //        return dictionary;
        //    }
        //    return null;
        //}
        //[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_PluginGetInfo")]
        //private static extern IntPtr BASS_PluginGetInfoPtr(int A_0);
        //public static BASS_PLUGININFO BASS_PluginGetInfo(int handle)
        //{
        //    if (handle == 0)
        //    {
        //        BASS_PLUGINFORM[] a_ = new BASS_PLUGINFORM[]
        //        {
        //            new BASS_PLUGINFORM("WAVE Audio", "*.wav", BASSChannelType.BASS_CTYPE_STREAM_WAV),
        //            new BASS_PLUGINFORM("Ogg Vorbis", "*.ogg", BASSChannelType.BASS_CTYPE_STREAM_OGG),
        //            new BASS_PLUGINFORM("MPEG Layer 1", "*.mp1;*.m1a", BASSChannelType.BASS_CTYPE_STREAM_MP1),
        //            new BASS_PLUGINFORM("MPEG Layer 2", "*.mp2;*.m2a;*.mpa", BASSChannelType.BASS_CTYPE_STREAM_MP2),
        //            new BASS_PLUGINFORM("MPEG Layer 3", "*.mp3;*.mpg;*.mpeg;*.mp3pro", BASSChannelType.BASS_CTYPE_STREAM_MP3),
        //            new BASS_PLUGINFORM("Audio IFF", "*.aif;*.aiff", BASSChannelType.BASS_CTYPE_STREAM_AIFF),
        //            new BASS_PLUGINFORM("Broadcast Wave", "*.bwf", BASSChannelType.BASS_CTYPE_STREAM_WAV)
        //        };
        //        return new BASS_PLUGININFO(Bass.BASS_GetVersion(), a_);
        //    }
        //    IntPtr intPtr = Bass.BASS_PluginGetInfoPtr(handle);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        c c = (c)Marshal.PtrToStructure(intPtr, typeof(c));
        //        return new BASS_PLUGININFO(c.a, c.b, c.c);
        //    }
        //    return null;
        //}
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool BASS_GetEAXParameters(ref EAXEnvironment env, ref float vol, ref float decay, ref float damp);
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool BASS_GetEAXParameters([MarshalAs(UnmanagedType.AsAny)] [In] [Out] object env, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object vol, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object decay, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object damp);
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool BASS_SetEAXParameters(EAXEnvironment env, float vol, float decay, float damp);
        //public static bool BASS_SetEAXParameters(EAXPreset preset)
        //{
        //    bool result = false;
        //    switch (preset)
        //    {
        //    case EAXPreset.EAX_PRESET_GENERIC:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_GENERIC, 0.5f, 1.493f, 0.5f);
        //        break;
        //    case EAXPreset.EAX_PRESET_PADDEDCELL:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_PADDEDCELL, 0.25f, 0.1f, 0f);
        //        break;
        //    case EAXPreset.EAX_PRESET_ROOM:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_ROOM, 0.417f, 0.4f, 0.666f);
        //        break;
        //    case EAXPreset.EAX_PRESET_BATHROOM:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_BATHROOM, 0.653f, 1.499f, 0.166f);
        //        break;
        //    case EAXPreset.EAX_PRESET_LIVINGROOM:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_LIVINGROOM, 0.208f, 0.478f, 0f);
        //        break;
        //    case EAXPreset.EAX_PRESET_STONEROOM:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_STONEROOM, 0.5f, 2.309f, 0.888f);
        //        break;
        //    case EAXPreset.EAX_PRESET_AUDITORIUM:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_AUDITORIUM, 0.403f, 4.279f, 0.5f);
        //        break;
        //    case EAXPreset.EAX_PRESET_CONCERTHALL:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_CONCERTHALL, 0.5f, 3.961f, 0.5f);
        //        break;
        //    case EAXPreset.EAX_PRESET_CAVE:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_CAVE, 0.5f, 2.886f, 1.304f);
        //        break;
        //    case EAXPreset.EAX_PRESET_ARENA:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_ARENA, 0.361f, 7.284f, 0.332f);
        //        break;
        //    case EAXPreset.EAX_PRESET_HANGAR:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_HANGAR, 0.5f, 10f, 0.3f);
        //        break;
        //    case EAXPreset.EAX_PRESET_CARPETEDHALLWAY:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_CARPETEDHALLWAY, 0.153f, 0.259f, 2f);
        //        break;
        //    case EAXPreset.EAX_PRESET_HALLWAY:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_HALLWAY, 0.361f, 1.493f, 0f);
        //        break;
        //    case EAXPreset.EAX_PRESET_STONECORRIDOR:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_STONECORRIDOR, 0.444f, 2.697f, 0.638f);
        //        break;
        //    case EAXPreset.EAX_PRESET_ALLEY:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_ALLEY, 0.25f, 1.752f, 0.776f);
        //        break;
        //    case EAXPreset.EAX_PRESET_FOREST:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_FOREST, 0.111f, 3.145f, 0.472f);
        //        break;
        //    case EAXPreset.EAX_PRESET_CITY:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_CITY, 0.111f, 2.767f, 0.224f);
        //        break;
        //    case EAXPreset.EAX_PRESET_MOUNTAINS:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_MOUNTAINS, 0.194f, 7.841f, 0.472f);
        //        break;
        //    case EAXPreset.EAX_PRESET_QUARRY:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_QUARRY, 1f, 1.499f, 0.5f);
        //        break;
        //    case EAXPreset.EAX_PRESET_PLAIN:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_PLAIN, 0.097f, 2.767f, 0.224f);
        //        break;
        //    case EAXPreset.EAX_PRESET_PARKINGLOT:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_PARKINGLOT, 0.208f, 1.652f, 1.5f);
        //        break;
        //    case EAXPreset.EAX_PRESET_SEWERPIPE:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_SEWERPIPE, 0.652f, 2.886f, 0.25f);
        //        break;
        //    case EAXPreset.EAX_PRESET_UNDERWATER:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_UNDERWATER, 1f, 1.499f, 0f);
        //        break;
        //    case EAXPreset.EAX_PRESET_DRUGGED:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_DRUGGED, 0.875f, 8.392f, 1.388f);
        //        break;
        //    case EAXPreset.EAX_PRESET_DIZZY:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_DIZZY, 0.139f, 17.234f, 0.666f);
        //        break;
        //    case EAXPreset.EAX_PRESET_PSYCHOTIC:
        //        result = Bass.BASS_SetEAXParameters(EAXEnvironment.EAX_ENVIRONMENT_PSYCHOTIC, 0.486f, 7.563f, 0.806f);
        //        break;
        //    }
        //    return result;
        //}
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //public static extern void BASS_Apply3D();
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool BASS_Set3DFactors(float distf, float rollf, float doppf);
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool BASS_Get3DFactors(ref float distf, ref float rollf, ref float doppf);
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool BASS_Get3DFactors([MarshalAs(UnmanagedType.AsAny)] [In] [Out] object distf, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object rollf, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object doppf);
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool BASS_Set3DPosition([In] BASS_3DVECTOR pos, [In] BASS_3DVECTOR vel, [In] BASS_3DVECTOR front, [In] BASS_3DVECTOR top);
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool BASS_Get3DPosition([In] [Out] BASS_3DVECTOR pos, [In] [Out] BASS_3DVECTOR vel, [In] [Out] BASS_3DVECTOR front, [In] [Out] BASS_3DVECTOR top);
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_SampleLoad")]
		private static extern int BASS_SampleLoadUnicode([MarshalAs(UnmanagedType.Bool)] bool A_0, [MarshalAs(UnmanagedType.LPWStr)] [In] string A_1, long A_2, int A_3, int A_4, BASSFlag A_5);
		public static int BASS_SampleLoad(string file, long offset, int length, int max, BASSFlag flags)
		{
			flags |= BASSFlag.BASS_UNICODE;
			return Bass.BASS_SampleLoadUnicode(false, file, offset, length, max, flags);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_SampleLoad")]
		private static extern int BASS_SampleLoadMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, IntPtr A_1, long A_2, int A_3, int A_4, BASSFlag A_5);
		public static int BASS_SampleLoad(IntPtr memory, long offset, int length, int max, BASSFlag flags)
		{
			return Bass.BASS_SampleLoadMemory(true, memory, offset, length, max, flags);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_SampleLoad")]
		private static extern int BASS_SampleLoadMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, byte[] A_1, long A_2, int A_3, int A_4, BASSFlag A_5);
		public static int BASS_SampleLoad(byte[] memory, long offset, int length, int max, BASSFlag flags)
		{
			return Bass.BASS_SampleLoadMemory(true, memory, offset, length, max, flags);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_SampleCreate(int length, int freq, int chans, int max, BASSFlag flags);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, IntPtr buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, float[] buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, int[] buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, short[] buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetData(int handle, byte[] buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, IntPtr buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, float[] buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, int[] buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, short[] buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleGetData(int handle, byte[] buffer);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleFree(int handle);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleSetInfo(int handle, [In] BASS_SAMPLE info);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_SampleGetChannel(int handle, [MarshalAs(UnmanagedType.Bool)] bool onlynew);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_SampleStop(int handle);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamCreate(int freq, int chans, BASSFlag flags, STREAMPROC proc, IntPtr user);
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreate")]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreateFile")]
		private static extern int BASS_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool A_0, [MarshalAs(UnmanagedType.LPWStr)] [In] string A_1, long A_2, long A_3, BASSFlag A_4);
		public static int BASS_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
		{
			flags |= BASSFlag.BASS_UNICODE;
			return Bass.BASS_StreamCreateFileUnicode(false, file, offset, length, flags);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreateFile")]
		private static extern int BASS_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, IntPtr A_1, long A_2, long A_3, BASSFlag A_4);
		public static int BASS_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
		{
			return Bass.BASS_StreamCreateFileMemory(true, memory, offset, length, flags);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreateURL")]
		private static extern int BASS_StreamCreateURLUnicode([MarshalAs(UnmanagedType.LPWStr)] [In] string A_0, int A_1, BASSFlag A_2, DOWNLOADPROC A_3, IntPtr A_4);
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_StreamCreateURL")]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern long BASS_StreamGetFilePosition(int handle, BASSStreamFilePosition mode);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_StreamFree(int handle);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, IntPtr buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, float[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, int[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, short[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutData(int handle, byte[] buffer, int length);
        //public unsafe static int BASS_StreamPutData(int handle, byte[] buffer, int startIdx, int length)
        //{
        //    return Bass.BASS_StreamPutData(handle, new IntPtr((void*)(&buffer[startIdx])), length);
        //}
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, IntPtr buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, float[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, int[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, short[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_StreamPutFileData(int handle, byte[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_MusicLoad")]
		private static extern int BASS_MusicLoadUnicode([MarshalAs(UnmanagedType.Bool)] bool A_0, [MarshalAs(UnmanagedType.LPWStr)] [In] string A_1, long A_2, int A_3, BASSFlag A_4, int A_5);
		public static int BASS_MusicLoad(string file, long offset, int length, BASSFlag flags, int freq)
		{
			flags |= BASSFlag.BASS_UNICODE;
			return Bass.BASS_MusicLoadUnicode(false, file, offset, length, flags, freq);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_MusicLoad")]
		private static extern int BASS_MusicLoadMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, IntPtr A_1, long A_2, int A_3, BASSFlag A_4, int A_5);
		public static int BASS_MusicLoad(IntPtr memory, long offset, int length, BASSFlag flags, int freq)
		{
			return Bass.BASS_MusicLoadMemory(true, memory, offset, length, flags, freq);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_MusicLoad")]
		private static extern int BASS_MusicLoadMemory([MarshalAs(UnmanagedType.Bool)] bool A_0, byte[] A_1, long A_2, int A_3, BASSFlag A_4, int A_5);
		public static int BASS_MusicLoad(byte[] memory, long offset, int length, BASSFlag flags, int freq)
		{
			return Bass.BASS_MusicLoadMemory(true, memory, offset, length, flags, freq);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_MusicFree(int handle);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_RecordInit(int device);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_RecordStart(int freq, int chans, BASSFlag flags, RECORDPROC proc, IntPtr user);
        //public static int BASS_RecordStart(int freq, int chans, BASSFlag flags, int period, RECORDPROC proc, IntPtr user)
        //{
        //    return Bass.BASS_RecordStart(freq, chans, (BASSFlag)Utils.MakeLong((int)flags, period), proc, user);
        //}
        //[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_RecordGetDeviceInfo")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool BASS_RecordGetDeviceInfoInternal([In] int A_0, [In] [Out] ref d A_1);
        //public static bool BASS_RecordGetDeviceInfo(int device, BASS_DEVICEINFO info)
        //{
        //    bool flag = Bass.BASS_RecordGetDeviceInfoInternal(device, ref info.a);
        //    if (flag)
        //    {
        //        info.name = Marshal.PtrToStringAnsi(info.a.a);
        //        info.driver = Marshal.PtrToStringAnsi(info.a.b);
        //        info.flags = info.a.c;
        //    }
        //    return flag;
        //}
        //public static BASS_DEVICEINFO BASS_RecordGetDeviceInfo(int device)
        //{
        //    //BASS_DEVICEINFO bASS_DEVICEINFO = new BASS_DEVICEINFO();
        //    //if (Bass.BASS_RecordGetDeviceInfo(device, bASS_DEVICEINFO))
        //    //{
        //    //    return bASS_DEVICEINFO;
        //    //}
        //    return null;
        //}
        //public static BASS_DEVICEINFO[] BASS_RecordGetDeviceInfos()
        //{
        //    List<BASS_DEVICEINFO> list = new List<BASS_DEVICEINFO>();
        //    int num = 0;
        //    BASS_DEVICEINFO item;
        //    while ((item = Bass.BASS_RecordGetDeviceInfo(num)) != null)
        //    {
        //        list.Add(item);
        //        num++;
        //    }
        //    Bass.BASS_GetCPU();
        //    return list.ToArray();
        //}
        //public static int BASS_RecordGetDeviceCount()
        //{
        //    BASS_DEVICEINFO info = new BASS_DEVICEINFO();
        //    int num = 0;
        //    //while (Bass.BASS_RecordGetDeviceInfo(num, info))
        //    //{
        //    //    num++;
        //    //}
        //    //Bass.BASS_GetCPU();
        //    return num;
        //}
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_RecordSetDevice(int device);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_RecordGetDevice();
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_RecordGetInputName")]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_RecordSetInput(int input, BASSInput setting, float volume);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_RecordGetInput")]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_RecordFree();

        [DllImport("bass.dll")]
        public static extern bool BASS_ChannelGetInfo(int device, [In] [Out] ref BASS_CHANNELINFO info);

        //[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_ChannelGetInfo")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool BASS_ChannelGetInfoInternal(int A_0, [In] [Out] ref b A_1);
        //public static bool BASS_ChannelGetInfo(int handle, BASS_CHANNELINFO info)
        //{
        //    bool flag = Bass.BASS_ChannelGetInfoInternal(handle, ref info.a);
        //    if (flag)
        //    {
        //        info.chans = info.a.b;
        //        info.ctype = info.a.d;
        //        info.flags = info.a.c;
        //        info.freq = info.a.a;
        //        info.origres = info.a.e;
        //        info.plugin = info.a.f;
        //        info.sample = info.a.g;
        //        if ((info.flags & BASSFlag.BASS_UNICODE) != BASSFlag.BASS_DEFAULT)
        //        {
        //            info.filename = Marshal.PtrToStringUni(info.a.h);
        //        }
        //        else
        //        {
        //            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
        //            {
        //                int num;
        //                info.filename = Utils.IntPtrAsStringUtf8(info.a.h, out num);
        //            }
        //            else
        //            {
        //                info.filename = Marshal.PtrToStringAnsi(info.a.h);
        //            }
        //        }
        //    }
        //    return flag;
        //}
        //public static BASS_CHANNELINFO BASS_ChannelGetInfo(int handle)
        //{
        //    BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
        //    if (Bass.BASS_ChannelGetInfo(handle, bASS_CHANNELINFO))
        //    {
        //        return bASS_CHANNELINFO;
        //    }
        //    return null;
        //}
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelPlay(int handle, [MarshalAs(UnmanagedType.Bool)] bool restart);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelSetDSP(int handle, DSPPROC proc, IntPtr user, int priority);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, IntPtr buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, [In] [Out] float[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, [In] [Out] short[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, [In] [Out] int[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetData(int handle, [In] [Out] byte[] buffer, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern long BASS_ChannelSeconds2Bytes(int handle, double pos);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern double BASS_ChannelBytes2Seconds(int handle, long pos);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern BASSActive BASS_ChannelIsActive(int handle);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelLock(int handle, [MarshalAs(UnmanagedType.Bool)] bool state);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern long BASS_ChannelGetLength(int handle, BASSMode mode);
		public static long BASS_ChannelGetLength(int handle)
		{
			return Bass.BASS_ChannelGetLength(handle, BASSMode.BASS_POS_BYTES);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelSetSync(int handle, BASSSync type, long param, SYNCPROC proc, IntPtr user);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelSetFX(int handle, BASSFXType type, int priority);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern int BASS_ChannelGetDevice(int handle);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSetDevice(int handle, int device);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelStop(int handle);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelPause(int handle);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSetAttribute(int handle, BASSAttribute attrib, float value);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelGetAttribute(int handle, BASSAttribute attrib, ref float value);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern BASSFlag BASS_ChannelFlags(int handle, BASSFlag flags, BASSFlag mask);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelUpdate(int handle, int length);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelIsSliding(int handle, BASSAttribute attrib);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSlideAttribute(int handle, BASSAttribute attrib, float value, int time);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSet3DAttributes(int handle, BASS3DMode mode, float min, float max, int iangle, int oangle, int outvol);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelGet3DAttributes(int handle, ref BASS3DMode mode, ref float min, ref float max, ref int iangle, ref int oangle, ref int outvol);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelGet3DAttributes(int handle, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object mode, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object min, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object max, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object iangle, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object oangle, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object outvol);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSet3DPosition(int handle, [In] BASS_3DVECTOR pos, [In] BASS_3DVECTOR orient, [In] BASS_3DVECTOR vel);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelGet3DPosition(int handle, [In] [Out] BASS_3DVECTOR pos, [In] [Out] BASS_3DVECTOR orient, [In] [Out] BASS_3DVECTOR vel);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		public static extern long BASS_ChannelGetPosition(int handle, BASSMode mode);
		public static long BASS_ChannelGetPosition(int handle)
		{
			return Bass.BASS_ChannelGetPosition(handle, BASSMode.BASS_POS_BYTES);
		}
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
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
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelRemoveSync(int handle, int sync);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelRemoveDSP(int handle, int dsp);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelRemoveFX(int handle, int fx);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelSetLink(int handle, int chan);
		[DllImport("bass.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_ChannelRemoveLink(int handle, int chan);
        //[DllImport("bass.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr BASS_ChannelGetTags(int handle, BASSTag tags);
        //public static string[] BASS_ChannelGetTagsArrayNullTermAnsi(int handle, BASSTag format)
        //{
        //    return Utils.IntPtrToArrayNullTermAnsi(Bass.BASS_ChannelGetTags(handle, format));
        //}
        //public static string[] BASS_ChannelGetTagsArrayNullTermUtf8(int handle, BASSTag format)
        //{
        //    return Utils.IntPtrToArrayNullTermUtf8(Bass.BASS_ChannelGetTags(handle, format));
        //}
        //public static string[] BASS_ChannelGetTagsID3V1(int handle)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_ID3);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        string[] array = new string[7];
        //        try
        //        {
        //            BASS_TAG_ID3 bASS_TAG_ID = (BASS_TAG_ID3)Marshal.PtrToStructure(intPtr, typeof(BASS_TAG_ID3));
        //            array[0] = bASS_TAG_ID.Title;
        //            array[1] = bASS_TAG_ID.Artist;
        //            array[2] = bASS_TAG_ID.Album;
        //            array[3] = bASS_TAG_ID.Year;
        //            array[4] = bASS_TAG_ID.Comment;
        //            array[5] = bASS_TAG_ID.Genre.ToString();
        //            if (bASS_TAG_ID.g == 0)
        //            {
        //                array[6] = bASS_TAG_ID.Track.ToString();
        //            }
        //        }
        //        catch
        //        {
        //        }
        //        return array;
        //    }
        //    return null;
        //}
        //public static string[] BASS_ChannelGetTagsBWF(int handle)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_RIFF_BEXT);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        string[] array = new string[9];
        //        try
        //        {
        //            BASS_TAG_BEXT bASS_TAG_BEXT = (BASS_TAG_BEXT)Marshal.PtrToStructure(intPtr, typeof(BASS_TAG_BEXT));
        //            array[0] = bASS_TAG_BEXT.Description;
        //            array[1] = bASS_TAG_BEXT.Originator;
        //            array[2] = bASS_TAG_BEXT.OriginatorReference;
        //            array[3] = bASS_TAG_BEXT.OriginationDate;
        //            array[4] = bASS_TAG_BEXT.OriginationTime;
        //            array[5] = bASS_TAG_BEXT.TimeReference.ToString();
        //            array[6] = bASS_TAG_BEXT.Version.ToString();
        //            array[7] = bASS_TAG_BEXT.UMID;
        //            array[8] = bASS_TAG_BEXT.GetCodingHistory(intPtr);
        //        }
        //        catch
        //        {
        //        }
        //        return array;
        //    }
        //    return null;
        //}
        //public static BASS_TAG_CACODEC BASS_ChannelGetTagsCA(int handle)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_WMA_META);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        return new BASS_TAG_CACODEC(intPtr);
        //    }
        //    return null;
        //}
        //public static string[] BASS_ChannelGetTagsID3V2(int handle)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_ID3V2);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        try
        //        {
        //            List<string> list = new List<string>();
        //            Un4seen.Bass.AddOn.Tags.a a = new Un4seen.Bass.AddOn.Tags.a(intPtr);
        //            int num = 0;
        //            while (a.l())
        //            {
        //                string text = a.h();
        //                object obj = a.j();
        //                if (text.Length > 0 && obj is string)
        //                {
        //                    list.Add(string.Format("{0}={1}", text, obj));
        //                }
        //                else
        //                {
        //                    if ((text == "POPM" || text == "POP") && obj is byte)
        //                    {
        //                        if (num == 0)
        //                        {
        //                            list.Add(string.Format("POPM={0}", obj));
        //                        }
        //                        num++;
        //                    }
        //                }
        //            }
        //            a.m();
        //            string[] result;
        //            if (list.Count > 0)
        //            {
        //                result = list.ToArray();
        //                return result;
        //            }
        //            result = null;
        //            return result;
        //        }
        //        catch
        //        {
        //            string[] result = null;
        //            return result;
        //        }
        //    }
        //    return null;
        //}
        //public static string[] BASS_ChannelGetTagsAPE(int handle)
        //{
        //    return Bass.BASS_ChannelGetTagsArrayNullTermUtf8(handle, BASSTag.BASS_TAG_APE);
        //}
        //public static BASS_TAG_APE_BINARY[] BASS_ChannelGetTagsAPEBinary(int handle)
        //{
        //    List<BASS_TAG_APE_BINARY> list = new List<BASS_TAG_APE_BINARY>();
        //    int num = 0;
        //    BASS_TAG_APE_BINARY tag;
        //    while ((tag = BASS_TAG_APE_BINARY.GetTag(handle, num)) != null)
        //    {
        //        list.Add(tag);
        //        num++;
        //    }
        //    if (list.Count > 0)
        //    {
        //        return list.ToArray();
        //    }
        //    return null;
        //}
        //public static TagPicture[] BASS_ChannelGetTagsAPEPictures(int handle)
        //{
        //    List<TagPicture> list = new List<TagPicture>();
        //    int num = 0;
        //    BASS_TAG_APE_BINARY tag;
        //    while ((tag = BASS_TAG_APE_BINARY.GetTag(handle, num)) != null)
        //    {
        //        if (tag.Key != null && tag.Key.ToLower().StartsWith("cover art"))
        //        {
        //            list.Add(new TagPicture(tag.Data, 2));
        //        }
        //        num++;
        //    }
        //    if (list.Count > 0)
        //    {
        //        return list.ToArray();
        //    }
        //    return null;
        //}
        //public static string[] BASS_ChannelGetTagsWMA(int handle)
        //{
        //    return Bass.BASS_ChannelGetTagsArrayNullTermUtf8(handle, BASSTag.BASS_TAG_WMA);
        //}
        //public static string[] BASS_ChannelGetTagsMP4(int handle)
        //{
        //    return Bass.BASS_ChannelGetTagsArrayNullTermUtf8(handle, BASSTag.BASS_TAG_MP4);
        //}
        //public static string[] BASS_ChannelGetTagsMF(int handle)
        //{
        //    return Bass.BASS_ChannelGetTagsArrayNullTermUtf8(handle, BASSTag.BASS_TAG_MF);
        //}
        //public static WAVEFORMATEXT BASS_ChannelGetTagsWAVEFORMAT(int handle)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_WAVEFORMAT);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        return new WAVEFORMATEXT(intPtr);
        //    }
        //    return null;
        //}
        //public static BASS_TAG_FLAC_PICTURE[] BASS_ChannelGetTagsFLACPictures(int handle)
        //{
        //    List<BASS_TAG_FLAC_PICTURE> list = new List<BASS_TAG_FLAC_PICTURE>();
        //    int num = 0;
        //    BASS_TAG_FLAC_PICTURE tag;
        //    while ((tag = BASS_TAG_FLAC_PICTURE.GetTag(handle, num)) != null)
        //    {
        //        list.Add(tag);
        //        num++;
        //    }
        //    if (list.Count > 0)
        //    {
        //        return list.ToArray();
        //    }
        //    return null;
        //}
        //public static BASS_TAG_FLAC_CUE BASS_ChannelGetTagsFLACCuesheet(int handle)
        //{
        //    return BASS_TAG_FLAC_CUE.GetTag(handle);
        //}
        //public static string[] BASS_ChannelGetTagsHTTP(int handle)
        //{
        //    return Bass.BASS_ChannelGetTagsArrayNullTermAnsi(handle, BASSTag.BASS_TAG_HTTP);
        //}
        //public static string[] BASS_ChannelGetTagsICY(int handle)
        //{
        //    return Bass.BASS_ChannelGetTagsArrayNullTermAnsi(handle, BASSTag.BASS_TAG_ICY);
        //}
        //public static string[] BASS_ChannelGetTagsOGG(int handle)
        //{
        //    return Bass.BASS_ChannelGetTagsArrayNullTermUtf8(handle, BASSTag.BASS_TAG_OGG);
        //}
        //public static string[] BASS_ChannelGetTagsRIFF(int handle)
        //{
        //    return Bass.BASS_ChannelGetTagsArrayNullTermAnsi(handle, BASSTag.BASS_TAG_RIFF_INFO);
        //}
        //public static string[] BASS_ChannelGetTagsMETA(int handle)
        //{
        //    return Utils.IntPtrToArrayNullTermAnsi(Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_META));
        //}
        //public static string BASS_ChannelGetTagLyrics3v2(int handle)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_LYRICS3);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        return Marshal.PtrToStringAnsi(intPtr);
        //    }
        //    return null;
        //}
        //public static string BASS_ChannelGetMusicName(int handle)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_MUSIC_NAME);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        return Marshal.PtrToStringAnsi(intPtr);
        //    }
        //    return null;
        //}
        //public static string BASS_ChannelGetMusicMessage(int handle)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_MUSIC_MESSAGE);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        return Marshal.PtrToStringAnsi(intPtr);
        //    }
        //    return null;
        //}
        //public static string BASS_ChannelGetMusicInstrument(int handle, int instrument)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_MUSIC_INST + instrument);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        return Marshal.PtrToStringAnsi(intPtr);
        //    }
        //    return null;
        //}
        //public static string BASS_ChannelGetMusicSample(int handle, int sample)
        //{
        //    IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_MUSIC_SAMPLE + sample);
        //    if (intPtr != IntPtr.Zero)
        //    {
        //        return Marshal.PtrToStringAnsi(intPtr);
        //    }
        //    return null;
        //}
        //public static string[] BASS_ChannelGetMidiTrackText(int handle, int track)
        //{
        //    if (track >= 0)
        //    {
        //        return Utils.IntPtrToArrayNullTermAnsi(Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_MIDI_TRACK + track));
        //    }
        //    List<string> list = new List<string>();
        //    track = 0;
        //    while (true)
        //    {
        //        IntPtr intPtr = Bass.BASS_ChannelGetTags(handle, BASSTag.BASS_TAG_MIDI_TRACK + track);
        //        if (!(intPtr != IntPtr.Zero))
        //        {
        //            break;
        //        }
        //        string[] array = Utils.IntPtrToArrayNullTermAnsi(intPtr);
        //        if (array != null && array.Length > 0)
        //        {
        //            list.AddRange(array);
        //        }
        //        track++;
        //    }
        //    if (list.Count > 0)
        //    {
        //        return list.ToArray();
        //    }
        //    return null;
        //}
        //[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_FXSetParameters")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool BASS_FXSetParametersExt(int A_0, [MarshalAs(UnmanagedType.AsAny)] [In] object A_1);
        //public static bool BASS_FXSetParameters(int handle, object par)
        //{
        //    if (par is BASS_BFX_MIX)
        //    {
        //        ((BASS_BFX_MIX)par).a();
        //    }
        //    else
        //    {
        //        if (par is BASS_BFX_VOLUME_ENV)
        //        {
        //            ((BASS_BFX_VOLUME_ENV)par).a();
        //        }
        //    }
        //    return Bass.BASS_FXSetParametersExt(handle, par);
        //}
        //[DllImport("bass.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_FXGetParameters")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool BASS_FXGetParametersExt(int A_0, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object A_1);
        //public static bool BASS_FXGetParameters(int handle, object par)
        //{
        //    bool result = Bass.BASS_FXGetParametersExt(handle, par);
        //    if (par is BASS_BFX_MIX)
        //    {
        //        ((BASS_BFX_MIX)par).b();
        //    }
        //    else
        //    {
        //        if (par is BASS_BFX_VOLUME_ENV)
        //        {
        //            ((BASS_BFX_VOLUME_ENV)par).b();
        //        }
        //    }
        //    return result;
        //}

	    [DllImport("bass.dll", CharSet = CharSet.Auto)]
	    public static extern bool BASS_FXGetParameters(int handle, IntPtr param);
        [DllImport("bass.dll", CharSet = CharSet.Auto)]
        public static extern bool BASS_FXSetParameters(int handle, IntPtr param);
        [DllImport("bass.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FXReset(int handle);
        //public static bool LoadMe()
        //{
        //    bool result = Utils.a(Bass.b, ref Bass.a);
        //    if (!BassNet.OmitCheckVersion)
        //    {
        //        Bass.a();
        //    }
        //    return result;
        //}
        //public static bool LoadMe(string path)
        //{
        //    bool result = Utils.a(Path.Combine(path, Bass.b), ref Bass.a);
        //    if (!BassNet.OmitCheckVersion)
        //    {
        //        Bass.a();
        //    }
        //    return result;
        //}
        //public static bool FreeMe()
        //{
        //    return Utils.a(ref Bass.a);
        //}
        //private static void a()
        //{
        //    try
        //    {
        //        if (Utils.HighWord(Bass.BASS_GetVersion()) != 516)
        //        {
        //            Version version = Bass.BASS_GetVersion(2);
        //            Version version2 = new Version(2, 4);
        //            FileVersionInfo fileVersionInfo = null;
        //            ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
        //            for (int i = modules.Count - 1; i >= 0; i--)
        //            {
        //                ProcessModule processModule = modules[i];
        //                if (processModule.ModuleName.ToLower().Equals(Bass.b.ToLower()))
        //                {
        //                    fileVersionInfo = processModule.FileVersionInfo;
        //                    break;
        //                }
        //            }
        //            if (fileVersionInfo != null)
        //            {
        //                MessageBox.Show(string.Format("An incorrect version of BASS was loaded!\r\n\r\nVersion loaded: {0}.{1}\r\nVersion expected: {2}.{3}\r\n\r\nFile: {4}\r\nFileVersion: {5}\r\nDescription: {6}\r\nCompany: {7}\r\nLanguage: {8}", new object[]
        //                {
        //                    version.Major,
        //                    version.Minor,
        //                    version2.Major,
        //                    version2.Minor,
        //                    fileVersionInfo.FileName,
        //                    fileVersionInfo.FileVersion,
        //                    fileVersionInfo.FileDescription,
        //                    fileVersionInfo.CompanyName + " " + fileVersionInfo.LegalCopyright,
        //                    fileVersionInfo.Language
        //                }), "Incorrect BASS Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            }
        //            else
        //            {
        //                MessageBox.Show(string.Format("An incorrect version of BASS was loaded!\r\n\r\nBASS Version loaded: {0}.{1}\r\nBASS Version expected: {2}.{3}", new object[]
        //                {
        //                    version.Major,
        //                    version.Minor,
        //                    version2.Major,
        //                    version2.Minor
        //                }), "Incorrect BASS Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //[DllImport(DllImportValue)]
        //public static extern int BASS_ErrorGetCode();

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_Init(int device, int frequency);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_Init(int device, int frequency, BASSInit flags, IntPtr win);

        //[DllImport(DllImportValue)]
        //public static extern int BASS_Free();

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_Start();

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_Pause();

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_Stop();

        ////[DllImport(DllImportValue)]
        ////public static extern int BASS_GetDeviceCount();
        //[DllImport(DllImportValue)]
        //public static extern bool BASS_GetDeviceInfo(int device, ref BASSDeviceInfo info);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_RecordInit(int device);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_RecordFree();

        //[DllImport(DllImportValue)]
        //public static extern string BASS_RecordGetInputName(int input);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_RecordSetDevice(int device);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_RecordSetInput(int input, int flags, float volume);

        //[DllImport(DllImportValue)]
        //public static extern int BASS_RecordStart(int frequency, int channels, int flags, RECORDPROC callback,
        //                                          IntPtr user);

        //[DllImport(DllImportValue)]
        //public static extern int BASS_GetConfig(int option);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_SetConfig(int option, int value);

        //[DllImport(DllImportValue)]
        //public static extern IntPtr BASS_GetConfigPtr(int option);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_SetConfigPtr(int option, IntPtr value);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_StreamFree(int handle);

        //[DllImport(DllImportValue)]
        //public static extern int BASS_StreamCreateURL(string url, int offset, int flags, DOWNLOADPROC callback,
        //                                              IntPtr user);

        //[DllImport(DllImportValue)]
        //public static extern int BASS_StreamCreateFile(bool fromMemory, string filePath, long offset, long length,
        //                                               int flags);

        //[DllImport(DllImportValue)]
        //public static extern long BASS_StreamGetFilePosition(int handle, int mode);

        //[DllImport(DllImportValue)]
        //public static extern int BASS_ChannelIsActive(int handle);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_ChannelPlay(int handle, bool restart);

        //[DllImport(DllImportValue)]
        //public static extern int BASS_ChannelStop(int handle);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_ChannelPause(int handle);

        //[DllImport(DllImportValue)]
        //public static extern long BASS_ChannelGetPosition(int handle, int mode);

        //[DllImport(DllImportValue)]
        //public static extern long BASS_ChannelGetLength(int handle, int mode);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_ChannelSetAttribute(int handle, int attrib, float value);

        //[DllImport(DllImportValue)]
        //public static extern int BASS_Encode_GetVersion();

        //[DllImport(DllImportValue)]
        //public static extern int BASS_Encode_StartCA(int handle, int ftype, int atype, int flags, int bitrate,
        //                                             ENCODEPROC callback, IntPtr user);

        //[DllImport(DllImportValue)]
        //public static extern int BASS_Encode_StartCAFile(int handle, int ftype, int atype, int flags, int bitrate,
        //                                                 string file);

        //[DllImport(DllImportValue)]
        //public static extern bool BASS_Encode_Stop(int handle);
    }
}
