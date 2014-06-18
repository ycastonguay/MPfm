// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Runtime.InteropServices;
using Un4seen.Bass.AddOn.Fx;

namespace Sessions.Sound.BassNetWrapper
{
    /// <summary>
    /// The BassWrapper class contains P/Invoke methods for the BASS audio library.
    /// This only contains a few methods that aren't currently supported in BASS.NET.
    /// </summary>
    public static class BassWrapper
    {
        #if IOS
        public const string DllImportValue_Bass = "__Internal";
        public const string DllImportValue_BassFX = "__Internal";
        public const string DllImportValue_BassMix = "__Internal";
        #elif ANDROID || LINUX
        public const string DllImportValue_Bass = "libbass.so";
        public const string DllImportValue_BassFX = "libbass_fx.so";
        public const string DllImportValue_BassMix = "libbassmix.so";
        #elif MAC
        public const string DllImportValue_Bass = "libbass.dylib";
        public const string DllImportValue_BassFX = "libbass_fx.dylib";
        public const string DllImportValue_BassMix = "libbassmix.dylib";
        public const string DllImportValue_BassASIO = "libbassasio.dylib";
        public const string DllImportValue_BassWASAPI = "libbasswasapi.dylib";
        #else
        public const string DllImportValue_Bass = "bass.dll";
        public const string DllImportValue_BassFX = "bass_fx.dll";
        public const string DllImportValue_BassMix = "bassmix.dll";
        public const string DllImportValue_BassASIO = "bassasio.dll";
        public const string DllImportValue_BassWASAPI = "basswasapi.dll";
        #endif

        [DllImport(DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_FXGetParameters")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FXGetParametersPeakEQ(int handle, [In, Out] BASS_BFX_PEAKEQ par);
        
        [DllImport(DllImportValue_Bass, CharSet = CharSet.Auto, EntryPoint = "BASS_FXSetParameters")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_FXSetParametersPeakEQ(int handle, [In] BASS_BFX_PEAKEQ par);

    }
}
