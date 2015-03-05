//// Copyright © 2011-2013 Yanick Castonguay
////
//// This file is part of Sessions.
////
//// Sessions is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
////
//// Sessions is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with Sessions. If not, see <http://www.gnu.org/licenses/>.
//
//using System;
//using System.Collections.Generic;
//using Un4seen.Bass;
//using Un4seen.Bass.AddOn.Enc;
//
//namespace Sessions.Sound.BassNetWrapper
//{
//    /// <summary>
//    /// The Base class contains methods for the Bassenc library.
//    /// </summary>
//    public static class BaseEnc
//    {
//        public static int COCOA_AUDIOUNIT_ADTS = 1633973363; // 'adts'
//        public static int COCOA_AUDIOUNIT_MP4F = 1836069990; // 'mp4f'
//        public static int COCOA_AUDIOUNIT_M4AF = 1832149350; // 'm4af'
//        public static int COCOA_AUDIOUNIT_AAC = 1633772320; // 'aac '
//        public static int COCOA_AUDIOUNIT_ALAC = 1634492771; // 'alac'
//
//        public static int GetVersion()
//        {
//            return BassEnc.BASS_Encode_GetVersion();
//        }
//
//        public static int EncodeStart(int handle, string cmdline, BASSEncode flags, ENCODEPROC encodeproc, IntPtr user)
//        {
//            int result = BassEnc.BASS_Encode_Start(handle, cmdline, flags, encodeproc, user);
//            if (result == 0)
//                Base.CheckForError();
//
//            return result;
//        }
//
//        public static void EncodeStop(int handle)
//        {
//            bool result = BassEnc.BASS_Encode_Stop(handle);
//            if (!result)
//                Base.CheckForError();
//        }
//
//        public static BASSActive EncodeIsActive(int handle)
//        {
//            var active = BassEnc.BASS_Encode_IsActive(handle);
//            return active;
//        }
//
//        public static void SetNotify(int handle, ENCODENOTIFYPROC proc, IntPtr user)
//        {
//            bool result = BassEnc.BASS_Encode_SetNotify(handle, proc, user);
//            if (!result)
//                Base.CheckForError();
//        }
//
//        #if MACOSX || IOS
//
//        public static int EncodeStartCA(int handle, int ftype, int atype, BASSEncode flags, int bitrate, ENCODEPROCEX encodeproc, IntPtr user)
//        {
//            int result = BassEnc.BASS_Encode_StartCA(handle, ftype, atype, flags, bitrate, encodeproc, user);
//            if (result == 0)
//                Base.CheckForError();
//
//            return result;
//        }
//
//        #endif
//    }
//}
