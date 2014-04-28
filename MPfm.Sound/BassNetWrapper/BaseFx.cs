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

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// The Base class contains methods for Bass FX.
    /// </summary>
    public static class BaseFx
    {
        public static void BPM_CallbackSet(int handle, BPMPROC proc, double period, int minMaxBPM, BASSFXBpm flags, IntPtr user)
        {
            if(!BassFx.BASS_FX_BPM_CallbackSet(handle, proc, period, minMaxBPM, flags, user))
                Base.CheckForError();
        }

        public static void BPM_CallbackReset(int handle)
        {
            if (!BassFx.BASS_FX_BPM_CallbackReset(handle))
                Base.CheckForError();
        }

        public static void BPM_Free(int handle)
        {
            if (!BassFx.BASS_FX_BPM_Free(handle))
                Base.CheckForError();
        }

        public static void BPM_BeatCallbackSet(int handle, BPMBEATPROC proc, IntPtr user)
        {
            if(!BassFx.BASS_FX_BPM_BeatCallbackSet(handle, proc, user))
                Base.CheckForError();
        }

        public static void BPM_BeatCallbackReset(int handle)
        {
            if (!BassFx.BASS_FX_BPM_BeatCallbackReset(handle))
                Base.CheckForError();
        }

        public static void BPM_BeatFree(int handle)
        {
            if (!BassFx.BASS_FX_BPM_BeatFree(handle))
                Base.CheckForError();
        }

        public static float BPM_Translate(int handle, float val2tran, BASSFXBpmTrans trans)
        {
            float value = BassFx.BASS_FX_BPM_Translate(handle, val2tran, trans);
            if(value == -1)
                Base.CheckForError();
            return value;
        }
    }
}
