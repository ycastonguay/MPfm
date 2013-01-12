//
// BassCallbacks.cs: This file contains callbacks for the P/Invoke wrapper of the BASS audio library.
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
    public delegate void DOWNLOADPROC(IntPtr buffer, int length, IntPtr user);
    public delegate void DSPPROC(int handle, int channel, IntPtr buffer, int length, IntPtr user);
    public delegate void FILECLOSEPROC(IntPtr user);
    public delegate long FILELENPROC(IntPtr user);
    public delegate int FILEREADPROC(IntPtr buffer, int length, IntPtr user);
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool FILESEEKPROC(long offset, IntPtr user);
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool RECORDPROC(int handle, IntPtr buffer, int length, IntPtr user);
    public delegate int STREAMPROC(int handle, IntPtr buffer, int length, IntPtr user);
    public delegate void SYNCPROC(int handle, int channel, int data, IntPtr user);

    public delegate void ENCODEPROC(int handle, int channel, IntPtr buffer, int length, IntPtr user);
}
