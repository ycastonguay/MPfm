﻿//
// SV7Tag.cs: Data structure for SV7/MPC tags.
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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MPfm.Sound
{  
    /// <summary>
    /// Data structure for SV7/MPC tags. 
    /// This is a older MusePack format (earlier than 2009).
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SV7Tag
    {
        #region Stream Header
        
        /// <summary>
        /// Private value for the SampleRate property.
        /// </summary>
        private int m_sampleRate = 44100;
        /// <summary>
        /// Audio file sample rate. Can be 44100, 48000, 37800 or 32000 Hz.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Audio file sample rate.")]
        public int SampleRate
        {
            get
            {
                return m_sampleRate;
            }
            set
            {
                m_sampleRate = value;
            }
        }

        /// <summary>
        /// Private value for the FrameCount property.
        /// </summary>
        private int m_frameCount = 0;
        /// <summary>
        /// Number of frames, every frame contains 1152 samples per channel, the last frame contains 
        /// 1 to 1152 samples per channel. Furthermore, one has to consider the latency of the analysis 
        /// and the synthesis filterbank of 481 samples.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Number of frames, every frame contains 1152 samples per channel, the last frame contains 1 to 1152 samples per channel. Furthermore, one has to consider the latency of the analysis and the synthesis filterbank of 481 samples.")]
        public int FrameCount
        {
            get
            {
                return m_frameCount;
            }
            set
            {
                m_frameCount = value;
            }
        }

        /// <summary>
        /// Private value for the LastFrameLength property.
        /// </summary>
        private int m_lastFrameLength = 0;
        /// <summary>
        /// Defines the used samples for the last frame. If true gapless, always 0.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Used samples for the last frame. If true gapless, always 0.")]
        public int LastFrameLength
        {
            get
            {
                return m_lastFrameLength;
            }
            set
            {
                m_lastFrameLength = value;
            }
        }

        /// <summary>
        /// Private value for the MaxLevel property.
        /// </summary>
        private int m_maxLevel = 0;
        /// <summary>
        /// Defines the maximum level of the coded PCM input signal.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Maximum level of the coded PCM input signal.")]
        public int MaxLevel
        {
            get
            {
                return m_maxLevel;
            }
            set
            {
                m_maxLevel = value;
            }
        }

        /// <summary>
        /// Private value for the MaxLevel property.
        /// </summary>
        private int m_maxBand = 0;
        /// <summary>
        /// Defines the last subband used in the whole file.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Last subband used in the whole file.")]
        public int MaxBand
        {
            get
            {
                return m_maxBand;
            }
            set
            {
                m_maxBand = value;
            }
        }

        /// <summary>
        /// Private value for the AudioChannels property.
        /// </summary>
        private int m_audioChannels = 2;
        /// <summary>
        /// Defines the number of audio channels used for this audio file.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Number of audio channels.")]
        public int AudioChannels
        {
            get
            {
                return m_audioChannels;
            }
            set
            {
                m_audioChannels = value;
            }
        }

        /// <summary>
        /// Private value for the IntensityStereo property.
        /// </summary>
        private bool m_intensityStereo = false;
        /// <summary>
        /// Defines if intensity stereo coding (IS) was used.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Defines if intensity stereo coding (IS) was used.")]
        public bool IntensityStereo
        {
            get
            {
                return m_intensityStereo;
            }
            set
            {
                m_intensityStereo = value;
            }
        }

        /// <summary>
        /// Private value for the MidSideStereoEnabled property.
        /// </summary>
        private bool m_midSideStereoEnabled = false;
        /// <summary>
        /// Defines if the Mid Side Stereo mode is enabled.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Defines if the Mid Side Stereo mode is enabled.")]
        public bool MidSideStereoEnabled
        {
            get
            {
                return m_midSideStereoEnabled;
            }
            set
            {
                m_midSideStereoEnabled = value;
            }
        }

        /// <summary>
        /// Private value for the TrueGapless property.
        /// </summary>
        private bool m_trueGapless = false;
        /// <summary>
        /// Defines if true gapless is used.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Defines if true gapless is used.")]
        public bool TrueGapless
        {
            get
            {
                return m_trueGapless;
            }
            set
            {
                m_trueGapless = value;
            }
        }

        #endregion

        #region Encoder Info

        /// <summary>
        /// Private value for the EncoderProfile property.
        /// </summary>
        private int m_encoderProfile = 0;
        /// <summary>
        /// Encoder quality; one of the following values:
        /// 0: no profile
        /// 1: Unstable/Experimental
        /// 2: unused
        /// 3: unused
        /// 4: unused
        /// 5: below Telephone (q= 0.0)
        /// 6: below Telephone (q= 1.0)
        /// 7: Telephone       (q= 2.0)
        /// 8: Thumb           (q= 3.0)
        /// 9: Radio           (q= 4.0)
        /// 10: Standard        (q= 5.0)
        /// 11: Xtreme          (q= 6.0)
        /// 12: Insane          (q= 7.0)
        /// 13: BrainDead       (q= 8.0)
        /// 14: above BrainDead (q= 9.0)
        /// 15: above BrainDead (q=10.0)
        /// </summary>
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Profile (ranges from 0 to 15. Q0 starts at 5, where Q10 = 15).")]
        public int EncoderProfile
        {
            get
            {
                return m_encoderProfile;
            }
            set
            {
                m_encoderProfile = value;
            }
        }

        /// <summary>
        /// Private value for the EncoderVersion property.
        /// </summary>
        private string m_encoderVersion = string.Empty;
        /// <summary>
        /// Defines the encoder version (ex: mppenc Beta 1.16).
        /// </summary>
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Encoder version (ex: mppenc Beta 1.16).")]
        public string EncoderVersion
        {
            get
            {
                return m_encoderVersion;
            }
            set
            {
                m_encoderVersion = value;
            }
        }

        #endregion

        #region Replay Gain

        /// <summary>
        /// Private value for the TitleGain property.
        /// </summary>
        private int m_titleGain = 0;
        /// <summary>
        /// Change in the replay level. Value is treated as signed 16 bit value and the level is changed by that many mB (Millibel). 
        /// Thus level changes of -327.68 dB to +327.67 dB are possible.
        /// </summary>
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Change in the replay level. Value is treated as signed 16 bit value and the level is changed by that many mB (Millibel). Thus level changes of -327.68 dB to +327.67 dB are possible.")]
        public int TitleGain
        {
            get
            {
                return m_titleGain;
            }
            set
            {
                m_titleGain = value;
            }
        }

        /// <summary>
        /// Private value for the TitlePeak property.
        /// </summary>
        private int m_titlePeak = 0;
        /// <summary>
        /// Maximum level of the decoded title:
        /// 16422: -6 dB
        /// 32767:  0 dB
        /// 65379: +6 dB
        /// </summary>
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Maximum level of the decoded title: 16422=-6dB, 32767=0dB, 65379=+6dB.")]
        public int TitlePeak
        {
            get
            {
                return m_titlePeak;
            }
            set
            {
                m_titlePeak = value;
            }
        }

        /// <summary>
        /// Private value for the AlbumGain property.
        /// </summary>
        private int m_albumGain = 0;
        /// <summary>
        /// Change in the replay level if the whole cd is supposed to be played with the same level change for all tracks. 
        /// Value is treated as signed 16 bit value and the level is attenuated by that many mB (Millibel). 
        /// Thus, level changes of -327.68 dB to +327.67 dB are possible.
        /// </summary>
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Change in the replay level if the whole cd is supposed to be played with the same level change for all tracks. Value is treated as signed 16 bit value and the level is attenuated by that many mB (Millibel). Thus, level changes of -327.68 dB to +327.67 dB are possible.")]
        public int AlbumGain
        {
            get
            {
                return m_albumGain;
            }
            set
            {
                m_albumGain = value;
            }
        }

        /// <summary>
        /// Private value for the AlbumPeak property.
        /// </summary>
        private int m_albumPeak = 0;
        /// <summary>
        /// Maximum level of the whole decoded CD:
        /// 16422: -6 dB
        /// 32767:  0 dB
        /// 65379: +6 dB
        /// </summary>
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Maximum level of the whole decoded CD: 16422=-6dB, 32767=0dB, 65379=+6dB.")]
        public int AlbumPeak
        {
            get
            {
                return m_albumPeak;
            }
            set
            {
                m_albumPeak = value;
            }
        }

        #endregion

        #region Metadata

        /// <summary>
        /// Private value for the Bitrate property.
        /// </summary>
        private int m_bitrate = 0;
        /// <summary>
        /// Audio file bitrate. This value is not part of the SV7 header.
        /// </summary>
        [Category("Metadata"), Browsable(true), ReadOnly(true), Description("[Metadata] Audio file bitrate. This value is not part of the SV7 header.")]
        public int Bitrate
        {
            get
            {
                return m_bitrate;
            }
            set
            {
                m_bitrate = value;
            }
        }

        /// <summary>
        /// Private value for the LengthSamples property.
        /// </summary>
        private long m_lengthSamples = 0;
        /// <summary>
        /// Audio file length (in samples). This value is not part of the SV7 header.
        /// </summary>
        [Category("Metadata"), Browsable(true), ReadOnly(true), Description("[Metadata] Audio file length (in samples). This value is not part of the SV7 header.")]
        public long LengthSamples
        {
            get
            {
                return m_lengthSamples;
            }
            set
            {
                m_lengthSamples = value;
            }
        }

        /// <summary>
        /// Private value for the LengthMS property.
        /// </summary>
        private long m_lengthMS = 0;
        /// <summary>
        /// Audio file length (in milliseconds). This value is not part of the SV7 header.
        /// </summary>
        [Category("Metadata"), Browsable(true), ReadOnly(true), Description("[Metadata] Audio file length (in milliseconds). This value is not part of the SV7 header.")]
        public long LengthMS
        {
            get
            {
                return m_lengthMS;
            }
            set
            {
                m_lengthMS = value;
            }
        }

        /// <summary>
        /// Private value for the Length property.
        /// </summary>
        private string m_length = string.Empty;
        /// <summary>
        /// Audio file length (in time string format, 00:00.000). This value is not part of the SV7 header.
        /// </summary>
        [Category("Metadata"), Browsable(true), ReadOnly(true), Description("[Metadata] Audio file length (in time string format, 00:00.000). This value is not part of the SV7 header.")]
        public string Length
        {
            get
            {
                return m_length;
            }
            set
            {
                m_length = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for the SV7Tag class.
        /// </summary>
        public SV7Tag()
        {
        }
    }
}
