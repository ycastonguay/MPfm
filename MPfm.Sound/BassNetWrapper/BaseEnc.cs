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
using MPfm.Core;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Enc;

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// The Base class contains methods for the Bassenc library.
    /// </summary>
    public static class BaseEnc
    {
        public static void CastInit()
        {
            //BassEnc.BASS_Encode_CastInit(
        }

        public static void EncodeStart(int handle, string cmdline, BASSEncode flags, ENCODEPROC encodeproc, IntPtr user)
        {
            //int result = BassEnc.BASS_Encode_Start(handle, cmdline, flags, encodeproc, user);
        }

        #if MACOSX // || IOS

        public static void EncodeStartCA(int handle, string cmdline, BASSEncode flags, ENCODEPROC encodeproc, IntPtr user)
        {
            //BassEnc.BASS_Encode_StartCA(
        }

        #endif
    }
}
