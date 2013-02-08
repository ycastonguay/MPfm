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

namespace MPfm.Sound.BassWrapper.FX
{
    public enum BASSBFXBQF
    {
        BASS_BFX_BQF_LOWPASS,
        BASS_BFX_BQF_HIGHPASS,
        BASS_BFX_BQF_BANDPASS,
        BASS_BFX_BQF_BANDPASS_Q,
        BASS_BFX_BQF_NOTCH,
        BASS_BFX_BQF_ALLPASS,
        BASS_BFX_BQF_PEAKINGEQ,
        BASS_BFX_BQF_LOWSHELF,
        BASS_BFX_BQF_HIGHSHELF
    }

    public enum BASSFXBpm
    {
        BASS_FX_BPM_DEFAULT,
        BASS_FX_BPM_BKGRND,
        BASS_FX_BPM_MULT2,
        BASS_FX_FREESOURCE = 65536
    }

    public enum BASSFXBpmTrans
    {
        BASS_FX_BPM_TRAN_X2,
        BASS_FX_BPM_TRAN_2FREQ,
        BASS_FX_BPM_TRAN_FREQ2,
        BASS_FX_BPM_TRAN_2PERCENT,
        BASS_FX_BPM_TRAN_PERCENT2
    }

    [Flags]
    public enum BASSFXChan
    {
        BASS_BFX_CHANALL = -1,
        BASS_BFX_CHANNONE = 0,
        BASS_BFX_CHAN1 = 1,
        BASS_BFX_CHAN2 = 2,
        BASS_BFX_CHAN3 = 4,
        BASS_BFX_CHAN4 = 8,
        BASS_BFX_CHAN5 = 16,
        BASS_BFX_CHAN6 = 32,
        BASS_BFX_CHAN7 = 64,
        BASS_BFX_CHAN8 = 128,
        BASS_BFX_CHAN9 = 256,
        BASS_BFX_CHAN10 = 512,
        BASS_BFX_CHAN11 = 1024,
        BASS_BFX_CHAN12 = 2048,
        BASS_BFX_CHAN13 = 4096,
        BASS_BFX_CHAN14 = 8192,
        BASS_BFX_CHAN15 = 16384,
        BASS_BFX_CHAN16 = 32768,
        BASS_BFX_CHAN17 = 65536,
        BASS_BFX_CHAN18 = 131072,
        BASS_BFX_CHAN19 = 262144,
        BASS_BFX_CHAN20 = 524288,
        BASS_BFX_CHAN21 = 1048576,
        BASS_BFX_CHAN22 = 2097152,
        BASS_BFX_CHAN23 = 4194304,
        BASS_BFX_CHAN24 = 8388608,
        BASS_BFX_CHAN25 = 16777216,
        BASS_BFX_CHAN26 = 33554432,
        BASS_BFX_CHAN27 = 67108864,
        BASS_BFX_CHAN28 = 134217728,
        BASS_BFX_CHAN29 = 268435456,
        BASS_BFX_CHAN30 = 536870912
    }

    public enum BASSFXReverse
    {
        BASS_FX_RVS_REVERSE = -1,
        BASS_FX_RVS_FORWARD = 1
    }
}
