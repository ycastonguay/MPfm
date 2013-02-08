//
// BassMixEnums.cs: This file contains enums for the P/Invoke wrapper of the BASS Mix audio library.
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

namespace MPfm.Sound.BassWrapper.Mix
{
    [Flags]
    public enum BASSMIXEnvelope
    {
        BASS_MIXER_ENV_FREQ = 1,
        BASS_MIXER_ENV_VOL = 2,
        BASS_MIXER_ENV_PAN = 3,
        BASS_MIXER_ENV_LOOP = 65536
    }
}
