//
// ParamEQDSP.cs: FMOD Wrapper parametric equalizer DSP class.
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

namespace MPfm.Sound
{
    /// <summary>
    /// This class wraps up the parametric equalizer DSP effect
    /// of the FMOD library. It is based on the DSP wrapper class.
    /// </summary>
    public class ParamEQDSP : DSP
    {
        public ParamEQDSP(MPfm.Sound.System system, FMOD.DSP baseDSP)
            : base(system, baseDSP)
        {            
            //SetPitch(pitch);
        }

        private float m_center;
        public float Center
        {
            get
            {
                return m_center;
            }
            set
            {
                SetCenter(value);
                m_center = value;                
            }
        }

        public void SetCenter(float value)
        {
            m_center = value;
            baseDSP.setParameter((int)FMOD.DSP_PARAMEQ.CENTER, value);
        }

        private float m_bandwidth;
        public float Bandwidth
        {
            get
            {
                return m_bandwidth;
            }
            set
            {
                SetBandwidth(value);
                m_bandwidth = value;
            }
        }

        public void SetBandwidth(float value)
        {
            m_bandwidth = value;
            baseDSP.setParameter((int)FMOD.DSP_PARAMEQ.BANDWIDTH, value);
        }

        private float m_gain;
        public float Gain
        {
            get
            {
                return m_gain;
            }
            set
            {
                SetGain(value);
                m_gain = value;
            }
        }

        public void SetGain(float value)
        {
            m_gain = value;
            baseDSP.setParameter((int)FMOD.DSP_PARAMEQ.GAIN, value);
        }


    }
}
