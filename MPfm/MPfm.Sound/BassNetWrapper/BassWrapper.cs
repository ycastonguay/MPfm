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
using System.Runtime.InteropServices;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// The BassWrapper class contains P/Invoke methods for the BASS audio library.
    /// This only contains a few methods that aren't currently supported in BASS.NET.
    /// </summary>
    public static class BassWrapper
    {
        #if IOS
        public const string DllImportValue = "__Internal";
        public const string DllImportValueFx = "__Internal";
        #elif ANDROID
        public const string DllImportValue = "libbass.so";
        public const string DllImportValueFx = "libbass_fx.so";
        #endif

        [DllImport(DllImportValue, CharSet = CharSet.Auto, EntryPoint = "BASS_FXGetParameters")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FXGetParametersPeakEQ(int handle, [In, Out] BASS_BFX_PEAKEQ par);
        
        [DllImport(DllImportValue, CharSet = CharSet.Auto, EntryPoint = "BASS_FXSetParameters")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FXSetParametersPeakEQ(int handle, [In] BASS_BFX_PEAKEQ par);

    }
}
