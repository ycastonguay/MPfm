﻿//
// BassASIOStructs.cs: This file contains structs for the P/Invoke wrapper of the BASS ASIO audio library.
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

namespace MPfm.Sound.BassWrapper.ASIO
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class BASS_ASIO_CHANNELINFO
    {
        public int group;
        public BASSASIOFormat format = BASSASIOFormat.BASS_ASIO_FORMAT_UNKNOWN;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name = string.Empty;
        public override string ToString()
        {
            return this.name;
        }
    }

    [Serializable]
    public sealed class BASS_ASIO_DEVICEINFO
    {
        //internal a a;
        public string name = string.Empty;
        public string driver = string.Empty;
        public override string ToString()
        {
            return this.name;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class BASS_ASIO_INFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name = string.Empty;
        public int version;
        public int inputs;
        public int outputs;
        public int bufmin;
        public int bufmax;
        public int bufpref;
        public int bufgran;
        public int initflags;
        public override string ToString()
        {
            return this.name;
        }
    }
}
