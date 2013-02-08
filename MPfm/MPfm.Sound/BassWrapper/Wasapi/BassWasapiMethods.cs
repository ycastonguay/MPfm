//
// BassWasapiMethods.cs: This file contains methods for the P/Invoke wrapper of the BASS Wasapi audio library.
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

namespace MPfm.Sound.BassWrapper.Wasapi
{
    [SuppressUnmanagedCodeSecurity]
    public sealed class BassWasapi
    {
        public const int BASSWASAPIVERSION = 0;
        private static int a;
        private static string b;
        static BassWasapi()
        {
            //BassWasapi.a = 0;
            //BassWasapi.b = "basswasapi.dll";
            //if (!BassNet.OmitCheckVersion)
            //{
            //    BassWasapi.a();
            //}
        }
        private BassWasapi()
        {
        }
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_WASAPI_GetVersion();
        public static Version BASS_WASAPI_GetVersion(int fieldcount)
        {
            if (fieldcount < 1)
            {
                fieldcount = 1;
            }
            if (fieldcount > 4)
            {
                fieldcount = 4;
            }
            int num = BassWasapi.BASS_WASAPI_GetVersion();
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
        //[DllImport("basswasapi.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_WASAPI_GetDeviceInfo")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool BASS_WASAPI_GetDeviceInfoInternal([In] int A_0, [In] [Out] ref Un4seen.BassWasapi.a A_1);
        //public static bool BASS_WASAPI_GetDeviceInfo(int device, BASS_WASAPI_DEVICEINFO info)
        //{
        //    bool result;
        //    try
        //    {
        //        bool flag = BassWasapi.BASS_WASAPI_GetDeviceInfoInternal(device, ref info.a);
        //        if (flag)
        //        {
        //            if (info.a.a != IntPtr.Zero)
        //            {
        //                info.name = Marshal.PtrToStringAnsi(info.a.a);
        //            }
        //            else
        //            {
        //                info.name = string.Empty;
        //            }
        //            if (info.a.b != IntPtr.Zero)
        //            {
        //                info.id = Marshal.PtrToStringAnsi(info.a.b);
        //            }
        //            else
        //            {
        //                info.id = string.Empty;
        //            }
        //            info.type = info.a.c;
        //            info.flags = info.a.d;
        //            info.minperiod = info.a.e;
        //            info.defperiod = info.a.f;
        //            info.mixfreq = info.a.g;
        //            info.mixchans = info.a.h;
        //        }
        //        result = flag;
        //    }
        //    catch
        //    {
        //        result = false;
        //    }
        //    return result;
        //}
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern bool BASS_WASAPI_GetDeviceInfo(int device, ref BASS_WASAPI_DEVICEINFO info);
        public static BASS_WASAPI_DEVICEINFO[] BASS_WASAPI_GetDeviceInfos()
        {
            List<BASS_WASAPI_DEVICEINFO> list = new List<BASS_WASAPI_DEVICEINFO>();
            int num = 0;
            BASS_WASAPI_DEVICEINFO item = new BASS_WASAPI_DEVICEINFO();
            while ((BassWasapi.BASS_WASAPI_GetDeviceInfo(num, ref item)) != null)
            {
                list.Add(item);
                num++;
            }
            BassWasapi.BASS_WASAPI_GetCPU();
            return list.ToArray();
        }
        public static int BASS_WASAPI_GetDeviceCount()
        {
            BASS_WASAPI_DEVICEINFO info = new BASS_WASAPI_DEVICEINFO();
            int num = 0;
            while (BassWasapi.BASS_WASAPI_GetDeviceInfo(num, ref info))
            {
                num++;
            }
            BassWasapi.BASS_WASAPI_GetCPU();
            return num;
        }
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_SetDevice(int device);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_WASAPI_GetDevice();
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern BASSWASAPIFormat BASS_WASAPI_CheckFormat(int device, int freq, int chans, BASSWASAPIInit flags);
        //public static BASSWASAPIFormat BASS_WASAPI_CheckFormat(int device, int freq, int chans, BASSWASAPIFormat format)
        //{
        //    return BassWasapi.BASS_WASAPI_CheckFormat(device, freq, chans, (BASSWASAPIInit)Utils.MakeLong(1, (int)format));
        //}
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_Init(int device, int freq, int chans, BASSWASAPIInit flags, float buffer, float period, WASAPIPROC proc, IntPtr user);
        //public static bool BASS_WASAPI_Init(int device, int freq, int chans, BASSWASAPIInit flags, BASSWASAPIFormat format, float buffer, float period, WASAPIPROC proc, IntPtr user)
        //{
        //    flags |= BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE;
        //    return BassWasapi.BASS_WASAPI_Init(device, freq, chans, (BASSWASAPIInit)Utils.MakeLong((int)flags, (int)format), buffer, period, proc, user);
        //}
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_Free();
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_GetInfo([In] [Out] BASS_WASAPI_INFO info);
        public static BASS_WASAPI_INFO BASS_WASAPI_GetInfo()
        {
            BASS_WASAPI_INFO bASS_WASAPI_INFO = new BASS_WASAPI_INFO();
            if (BassWasapi.BASS_WASAPI_GetInfo(bASS_WASAPI_INFO))
            {
                return bASS_WASAPI_INFO;
            }
            return null;
        }
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_Start();
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_Stop([MarshalAs(UnmanagedType.Bool)] bool reset);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_IsStarted();
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern float BASS_WASAPI_GetCPU();
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern float BASS_WASAPI_Lock([MarshalAs(UnmanagedType.Bool)] bool state);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_SetVolume([MarshalAs(UnmanagedType.Bool)] bool linear, float volume);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern float BASS_WASAPI_GetVolume([MarshalAs(UnmanagedType.Bool)] bool linear);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_WASAPI_PutData(IntPtr buffer, int length);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_WASAPI_PutData(float[] buffer, int length);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_WASAPI_GetLevel();
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_WASAPI_GetData(IntPtr buffer, int length);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_WASAPI_GetData([In] [Out] float[] buffer, int length);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        public static extern float BASS_WASAPI_GetDeviceLevel(int device, int chan);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_SetMute([MarshalAs(UnmanagedType.Bool)] bool mute);
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_GetMute();
        [DllImport("basswasapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_WASAPI_SetNotify(WASAPINOTIFYPROC proc, IntPtr user);
        //public static bool LoadMe()
        //{
        //    bool result = Utils.a(BassWasapi.b, ref BassWasapi.a);
        //    if (!BassNet.OmitCheckVersion)
        //    {
        //        BassWasapi.a();
        //    }
        //    return result;
        //}
        //public static bool LoadMe(string path)
        //{
        //    bool result = Utils.a(Path.Combine(path, BassWasapi.b), ref BassWasapi.a);
        //    if (!BassNet.OmitCheckVersion)
        //    {
        //        BassWasapi.a();
        //    }
        //    return result;
        //}
        //public static bool FreeMe()
        //{
        //    return Utils.a(ref BassWasapi.a);
        //}
        //private static void a()
        //{
        //    try
        //    {
        //        if (Utils.HighWord(BassWasapi.BASS_WASAPI_GetVersion()) != 0)
        //        {
        //            Version version = BassWasapi.BASS_WASAPI_GetVersion(2);
        //            Version version2 = new Version(0, 0);
        //            FileVersionInfo fileVersionInfo = null;
        //            ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
        //            for (int i = modules.Count - 1; i >= 0; i--)
        //            {
        //                ProcessModule processModule = modules[i];
        //                if (processModule.ModuleName.ToLower().Equals(BassWasapi.b.ToLower()))
        //                {
        //                    fileVersionInfo = processModule.FileVersionInfo;
        //                    break;
        //                }
        //            }
        //            if (fileVersionInfo != null)
        //            {
        //                MessageBox.Show(string.Format("An incorrect version of BASSWASAPI was loaded!\r\n\r\nVersion loaded: {0}.{1}\r\nVersion expected: {2}.{3}\r\n\r\nFile: {4}\r\nFileVersion: {5}\r\nDescription: {6}\r\nCompany: {7}\r\nLanguage: {8}", new object[]
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
        //                }), "Incorrect BASSWASAPI Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            }
        //            else
        //            {
        //                MessageBox.Show(string.Format("An incorrect version of BASSWASAPI was loaded!\r\n\r\nVersion loaded: {0}.{1}\r\nVersion expected: {2}.{3}", new object[]
        //                {
        //                    version.Major,
        //                    version.Minor,
        //                    version2.Major,
        //                    version2.Minor
        //                }), "Incorrect BASSWASAPI Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}
    }
}
