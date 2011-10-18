//
// PitchShiftDSP.cs: FMOD Wrapper pitch shift DSP class.
//
// Copyright © 2011 Yanick Castonguay
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
using System.Text;

namespace MPfm.Sound.FMODWrapper
{
    /// <summary>
    /// This class wraps up the pitch shift DSP effect
    /// of the FMOD library. It is based on the DSP wrapper class.
    /// </summary>
    public class PitchShiftDSP : DSP
    {
        public PitchShiftDSP(MPfm.Sound.FMODWrapper.System system, FMOD.DSP baseDSP, float pitch) 
            : base(system, baseDSP)
        {            
            SetPitch(pitch);
        }

        public void SetPitch(float value)
        {
            baseDSP.setParameter((int)FMOD.DSP_PITCHSHIFT.PITCH, value);
        }

        public void SetFFTSize(float value)
        {
            baseDSP.setParameter((int)FMOD.DSP_PITCHSHIFT.FFTSIZE, value);
        }
    
    }
}
