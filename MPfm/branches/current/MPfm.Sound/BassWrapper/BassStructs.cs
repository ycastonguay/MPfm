//
// BassStructs.cs: This file contains structs for the P/Invoke wrapper of the BASS audio library.
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
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_INFO
    {
        public BASSInfo flags;
        public int hwsize;
        public int hwfree;
        public int freesam;
        public int free3d;
        public int minrate;
        public int maxrate;
        public bool eax;
        public int minbuf = 500;
        public int dsver;
        public int latency;
        public BASSInit initflags;
        public int speakers;
        public int freq;
        public bool SupportsContinuousRate
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_CONTINUOUSRATE) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool SupportsDirectSound
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_EMULDRIVER) == BASSInfo.DSCAPS_NONE;
            }
        }
        public bool IsCertified
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_CERTIFIED) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool SupportsMonoSamples
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_SECONDARYMONO) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool SupportsStereoSamples
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_SECONDARYSTEREO) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool Supports8BitSamples
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_SECONDARY8BIT) != BASSInfo.DSCAPS_NONE;
            }
        }
        public bool Supports16BitSamples
        {
            get
            {
                return (this.flags & BASSInfo.DSCAPS_SECONDARY16BIT) != BASSInfo.DSCAPS_NONE;
            }
        }
        public override string ToString()
        {
            return string.Format("Speakers={0}, MinRate={1}, MaxRate={2}, DX={3}, EAX={4}", new object[]
		    {
			    this.speakers,
			    this.minrate,
			    this.maxrate,
			    this.dsver,
			    this.eax
		    });
        }
    }
}
