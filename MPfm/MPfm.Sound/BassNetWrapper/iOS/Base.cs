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

namespace MPfm.Sound.BassNetWrapper.iOS
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IOSNOTIFY(int status);

    [Flags]
    public enum BASSIOSNotify
    {
        BASS_IOSNOTIFY_INTERRUPT = 1,
        BASS_IOSNOTIFY_INTERRUPT_END = 2
    }

    public enum BASSIOSConfig
    {
        BASS_CONFIG_IOS_MIXAUDIO = 34,
        BASS_CONFIG_IOS_SPEAKER = 39,
        BASS_CONFIG_IOS_NOTIFY = 46
    }
}
