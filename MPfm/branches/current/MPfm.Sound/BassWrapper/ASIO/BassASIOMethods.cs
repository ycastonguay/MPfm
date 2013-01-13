//
// BassASIOMethods.cs: This file contains methods for the P/Invoke wrapper of the BASS ASIO audio library.
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

namespace MPfm.Sound.BassWrapper.ASIO
{
    [SuppressUnmanagedCodeSecurity]
    public sealed class BassAsio
    {
        public const int BASSASIOVERSION = 257;
        private static int a;
        private static string b;
        //static BassAsio()
        //{
        //    //BassAsio.a = 0;
        //    //BassAsio.b = "bassasio.dll";
        //    //if (!BassNet.OmitCheckVersion)
        //    //{
        //    //    BassAsio.a();
        //    //}
        //}
        //private BassAsio()
        //{
        //}
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern BASSError BASS_ASIO_ErrorGetCode();

        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern bool BASS_ASIO_GetDeviceInfo(int device, ref BASS_ASIO_DEVICEINFO info);

        //[DllImport("bassasio.dll", CharSet = CharSet.Auto, EntryPoint = "BASS_ASIO_GetDeviceInfo")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool BASS_ASIO_GetDeviceInfoInternal([In] int A_0, [In] [Out] ref Un4seen.BassAsio.a A_1);
        //public static bool BASS_ASIO_GetDeviceInfo(int device, BASS_ASIO_DEVICEINFO info)
        //{
        //    bool flag = BassAsio.BASS_ASIO_GetDeviceInfoInternal(device, ref info.a);
        //    if (flag)
        //    {
        //        info.name = Marshal.PtrToStringAnsi(info.a.a);
        //        info.driver = Marshal.PtrToStringAnsi(info.a.b);
        //    }
        //    return flag;
        //}
        //public static BASS_ASIO_DEVICEINFO BASS_ASIO_GetDeviceInfo(int device)
        //{
        //    BASS_ASIO_DEVICEINFO bASS_ASIO_DEVICEINFO = new BASS_ASIO_DEVICEINFO();
        //    if (BassAsio.BASS_ASIO_GetDeviceInfo(device, bASS_ASIO_DEVICEINFO))
        //    {
        //        return bASS_ASIO_DEVICEINFO;
        //    }
        //    return null;
        //}
        public static BASS_ASIO_DEVICEINFO[] BASS_ASIO_GetDeviceInfos()
        {
            List<BASS_ASIO_DEVICEINFO> list = new List<BASS_ASIO_DEVICEINFO>();
            int num = 0;
            BASS_ASIO_DEVICEINFO item = new BASS_ASIO_DEVICEINFO();
            while ((BassAsio.BASS_ASIO_GetDeviceInfo(num, ref item)) != null)
            {
                list.Add(item);
                num++;
            }
            BassAsio.BASS_ASIO_GetCPU();
            return list.ToArray();
        }
        //public static int BASS_ASIO_GetDeviceCount()
        //{
        //    BASS_ASIO_DEVICEINFO info = new BASS_ASIO_DEVICEINFO();
        //    int num = 0;
        //    while (BassAsio.BASS_ASIO_GetDeviceInfo(num, info))
        //    {
        //        num++;
        //    }
        //    BassAsio.BASS_ASIO_GetCPU();
        //    return num;
        //}
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_SetDevice(int device);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_ASIO_GetDevice();
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_Init(int device, BASSASIOInit flags);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_Free();
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_ASIO_GetVersion();
        public static Version BASS_ASIO_GetVersion(int fieldcount)
        {
            if (fieldcount < 1)
            {
                fieldcount = 1;
            }
            if (fieldcount > 4)
            {
                fieldcount = 4;
            }
            int num = BassAsio.BASS_ASIO_GetVersion();
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
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_Stop();
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_GetInfo([In] [Out] BASS_ASIO_INFO info);
        public static BASS_ASIO_INFO BASS_ASIO_GetInfo()
        {
            BASS_ASIO_INFO bASS_ASIO_INFO = new BASS_ASIO_INFO();
            if (BassAsio.BASS_ASIO_GetInfo(bASS_ASIO_INFO))
            {
                return bASS_ASIO_INFO;
            }
            return null;
        }
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_SetRate(double rate);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_Start(int buflen);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_IsStarted();
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ControlPanel();
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern double BASS_ASIO_GetRate();
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern int BASS_ASIO_GetLatency([MarshalAs(UnmanagedType.Bool)] bool input);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern float BASS_ASIO_GetCPU();
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_Monitor(int input, int output, int gain, int state, int pan);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_SetNotify(ASIONOTIFYPROC proc, IntPtr user);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ChannelGetInfo([MarshalAs(UnmanagedType.Bool)] bool input, int channel, [In] [Out] BASS_ASIO_CHANNELINFO info);
        public static BASS_ASIO_CHANNELINFO BASS_ASIO_ChannelGetInfo(bool input, int channel)
        {
            BASS_ASIO_CHANNELINFO bASS_ASIO_CHANNELINFO = new BASS_ASIO_CHANNELINFO();
            if (BassAsio.BASS_ASIO_ChannelGetInfo(input, channel, bASS_ASIO_CHANNELINFO))
            {
                return bASS_ASIO_CHANNELINFO;
            }
            return null;
        }
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ChannelEnable([MarshalAs(UnmanagedType.Bool)] bool input, int channel, ASIOPROC proc, IntPtr user);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ChannelJoin([MarshalAs(UnmanagedType.Bool)] bool input, int channel, int channel2);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ChannelSetFormat([MarshalAs(UnmanagedType.Bool)] bool input, int channel, BASSASIOFormat format);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ChannelSetRate([MarshalAs(UnmanagedType.Bool)] bool input, int channel, double rate);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ChannelReset([MarshalAs(UnmanagedType.Bool)] bool input, int channel, BASSASIOReset flags);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ChannelPause([MarshalAs(UnmanagedType.Bool)] bool input, int channel);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern BASSASIOActive BASS_ASIO_ChannelIsActive([MarshalAs(UnmanagedType.Bool)] bool input, int channel);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern BASSASIOFormat BASS_ASIO_ChannelGetFormat([MarshalAs(UnmanagedType.Bool)] bool input, int channel);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern double BASS_ASIO_ChannelGetRate([MarshalAs(UnmanagedType.Bool)] bool input, int channel);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern float BASS_ASIO_ChannelGetLevel([MarshalAs(UnmanagedType.Bool)] bool input, int channel);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ChannelEnableMirror(int channel, [MarshalAs(UnmanagedType.Bool)] bool input2, int channel2);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ASIO_ChannelSetVolume([MarshalAs(UnmanagedType.Bool)] bool input, int channel, float volume);
        [DllImport("bassasio.dll", CharSet = CharSet.Auto)]
        public static extern float BASS_ASIO_ChannelGetVolume([MarshalAs(UnmanagedType.Bool)] bool input, int channel);
    //    public static bool LoadMe()
    //    {
    //        bool result = Utils.a(BassAsio.b, ref BassAsio.a);
    //        if (!BassNet.OmitCheckVersion)
    //        {
    //            BassAsio.a();
    //        }
    //        return result;
    //    }
    //    public static bool LoadMe(string path)
    //    {
    //        bool result = Utils.a(Path.Combine(path, BassAsio.b), ref BassAsio.a);
    //        if (!BassNet.OmitCheckVersion)
    //        {
    //            BassAsio.a();
    //        }
    //        return result;
    //    }
    //    public static bool FreeMe()
    //    {
    //        return Utils.a(ref BassAsio.a);
    //    }
    //    private static void a()
    //    {
    //        try
    //        {
    //            if (Utils.HighWord(BassAsio.BASS_ASIO_GetVersion()) != 257)
    //            {
    //                Version version = BassAsio.BASS_ASIO_GetVersion(2);
    //                Version version2 = new Version(1, 1);
    //                FileVersionInfo fileVersionInfo = null;
    //                ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
    //                for (int i = modules.Count - 1; i >= 0; i--)
    //                {
    //                    ProcessModule processModule = modules[i];
    //                    if (processModule.ModuleName.ToLower().Equals(BassAsio.b.ToLower()))
    //                    {
    //                        fileVersionInfo = processModule.FileVersionInfo;
    //                        break;
    //                    }
    //                }
    //                if (fileVersionInfo != null)
    //                {
    //                    MessageBox.Show(string.Format("An incorrect version of BASSASIO was loaded!\r\n\r\nVersion loaded: {0}.{1}\r\nVersion expected: {2}.{3}\r\n\r\nFile: {4}\r\nFileVersion: {5}\r\nDescription: {6}\r\nCompany: {7}\r\nLanguage: {8}", new object[]
    //                    {
    //                        version.Major,
    //                        version.Minor,
    //                        version2.Major,
    //                        version2.Minor,
    //                        fileVersionInfo.FileName,
    //                        fileVersionInfo.FileVersion,
    //                        fileVersionInfo.FileDescription,
    //                        fileVersionInfo.CompanyName + " " + fileVersionInfo.LegalCopyright,
    //                        fileVersionInfo.Language
    //                    }), "Incorrect BASSASIO Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    //                }
    //                else
    //                {
    //                    MessageBox.Show(string.Format("An incorrect version of BASSASIO was loaded!\r\n\r\nVersion loaded: {0}.{1}\r\nVersion expected: {2}.{3}", new object[]
    //                    {
    //                        version.Major,
    //                        version.Minor,
    //                        version2.Major,
    //                        version2.Minor
    //                    }), "Incorrect BASSASIO Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    //                }
    //            }
    //        }
    //        catch
    //        {
    //        }
    //    }
    }
}
