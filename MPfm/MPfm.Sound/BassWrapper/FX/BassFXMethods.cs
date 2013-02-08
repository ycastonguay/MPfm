//
// BassFXMethods.cs: This file contains methods for the P/Invoke wrapper of the BASS FX audio library.
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
using System.Security;

namespace MPfm.Sound.BassWrapper.FX
{
    [SuppressUnmanagedCodeSecurity]
    public sealed class BassFx
    {
        public const int BASSFXVERSION = 516;
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        public static extern int BASS_FX_GetVersion();
        public static Version BASS_FX_GetVersion(int fieldcount)
        {
            if (fieldcount < 1)
            {
                fieldcount = 1;
            }
            if (fieldcount > 4)
            {
                fieldcount = 4;
            }
            int num = BassFx.BASS_FX_GetVersion();
            Version result = new Version(2, 4);
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
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        public static extern int BASS_FX_TempoCreate(int channel, BASSFlag flags);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        public static extern int BASS_FX_TempoGetSource(int channel);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        public static extern float BASS_FX_TempoGetRateRatio(int chan);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        public static extern int BASS_FX_ReverseCreate(int channel, float dec_block, BASSFlag flags);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        public static extern int BASS_FX_ReverseGetSource(int channel);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        public static extern float BASS_FX_BPM_DecodeGet(int channel, double startSec, double endSec, int minMaxBPM, BASSFXBpm flags, BPMPROCESSPROC proc);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_BeatDecodeGet(int channel, double startSec, double endSec, BASSFXBpm flags, BPMBEATPROC proc, IntPtr user);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        public static extern float BASS_FX_BPM_Translate(int handle, float val2tran, BASSFXBpmTrans trans);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_Free(int handle);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_CallbackSet(int handle, BPMPROC proc, double period, int minMaxBPM, BASSFXBpm flags, IntPtr user);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_CallbackReset(int handle);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_BeatCallbackSet(int handle, BPMBEATPROC proc, IntPtr user);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_BeatCallbackReset(int handle);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_BeatSetParameters(int handle, float bandwidth, float centerfreq, float beat_rtime);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_BeatGetParameters(int handle, ref float bandwidth, ref float centerfreq, ref float beat_rtime);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_BeatGetParameters(int handle, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object bandwidth, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object centerfreq, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object beat_rtime);
        [DllImport(BassWrapperGlobals.DllImportValue_BassFX, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FX_BPM_BeatFree(int handle);
    }
}
