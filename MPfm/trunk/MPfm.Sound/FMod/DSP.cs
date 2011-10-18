//
// DSP.cs: FMOD Wrapper base DSP class.
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
    /// This class wraps up the DSP class of the FMOD library.
    /// </summary>
    public class DSP
    {
        protected MPfm.Sound.FMODWrapper.System system = null;
        public FMOD.DSP baseDSP = null;

        public DSP()
        {
            baseDSP = new FMOD.DSP();
        }

        public DSP(MPfm.Sound.FMODWrapper.System system, FMOD.DSP baseDSP)
        {
            this.system = system;
            this.baseDSP = baseDSP;
        }

        public bool Bypassed
        {
            get
            {
                FMOD.RESULT result;
                bool bypassed = false;

                if (baseDSP != null)
                {
                    result = baseDSP.getBypass(ref bypassed);
                    System.CheckForError(result);
                }

                return bypassed;
            }
            set
            {
                FMOD.RESULT result;                

                if (baseDSP != null)
                {
                    result = baseDSP.setBypass(value);
                    System.CheckForError(result);
                }
            }
        }
    }
}
