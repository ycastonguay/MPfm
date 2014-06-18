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

using System.ComponentModel;

namespace Sessions.Sound.Tags
{  
    /// <summary>
    /// Data structure for SV7/MPC tags. 
    /// This is a older MusePack format (earlier than 2009).
    /// </summary>
    #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
    [TypeConverter(typeof(ExpandableObjectConverter))]
    #endif
    public class SV7Tag
    {
        #region Stream Header
        
        /// <summary>
        /// Private value for the SampleRate property.
        /// </summary>
        private int sampleRate = 44100;
        /// <summary>
        /// Audio file sample rate. Can be 44100, 48000, 37800 or 32000 Hz.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Audio file sample rate.")]
        #endif
        public int SampleRate
        {
            get
            {
                return sampleRate;
            }
            set
            {
                sampleRate = value;
            }
        }

        /// <summary>
        /// Private value for the FrameCount property.
        /// </summary>
        private int frameCount = 0;
        /// <summary>
        /// Number of frames, every frame contains 1152 samples per channel, the last frame contains 
        /// 1 to 1152 samples per channel. Furthermore, one has to consider the latency of the analysis 
        /// and the synthesis filterbank of 481 samples.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Number of frames, every frame contains 1152 samples per channel, the last frame contains 1 to 1152 samples per channel. Furthermore, one has to consider the latency of the analysis and the synthesis filterbank of 481 samples.")]
        #endif
        public int FrameCount
        {
            get
            {
                return frameCount;
            }
            set
            {
                frameCount = value;
            }
        }

        /// <summary>
        /// Private value for the LastFrameLength property.
        /// </summary>
        private int lastFrameLength = 0;
        /// <summary>
        /// Defines the used samples for the last frame. If true gapless, always 0.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Used samples for the last frame. If true gapless, always 0.")]
        #endif
        public int LastFrameLength
        {
            get
            {
                return lastFrameLength;
            }
            set
            {
                lastFrameLength = value;
            }
        }

        /// <summary>
        /// Private value for the MaxLevel property.
        /// </summary>
        private int maxLevel = 0;
        /// <summary>
        /// Defines the maximum level of the coded PCM input signal.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Maximum level of the coded PCM input signal.")]
        #endif
        public int MaxLevel
        {
            get
            {
                return maxLevel;
            }
            set
            {
                maxLevel = value;
            }
        }

        /// <summary>
        /// Private value for the MaxLevel property.
        /// </summary>
        private int maxBand = 0;
        /// <summary>
        /// Defines the last subband used in the whole file.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Last subband used in the whole file.")]
        #endif
        public int MaxBand
        {
            get
            {
                return maxBand;
            }
            set
            {
                maxBand = value;
            }
        }

        /// <summary>
        /// Private value for the AudioChannels property.
        /// </summary>
        private int audioChannels = 2;
        /// <summary>
        /// Defines the number of audio channels used for this audio file.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Number of audio channels.")]
        #endif
        public int AudioChannels
        {
            get
            {
                return audioChannels;
            }
            set
            {
                audioChannels = value;
            }
        }

        /// <summary>
        /// Private value for the IntensityStereo property.
        /// </summary>
        private bool intensityStereo = false;
        /// <summary>
        /// Defines if intensity stereo coding (IS) was used.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Defines if intensity stereo coding (IS) was used.")]
        #endif
        public bool IntensityStereo
        {
            get
            {
                return intensityStereo;
            }
            set
            {
                intensityStereo = value;
            }
        }

        /// <summary>
        /// Private value for the MidSideStereoEnabled property.
        /// </summary>
        private bool midSideStereoEnabled = false;
        /// <summary>
        /// Defines if the Mid Side Stereo mode is enabled.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Defines if the Mid Side Stereo mode is enabled.")]
        #endif
        public bool MidSideStereoEnabled
        {
            get
            {
                return midSideStereoEnabled;
            }
            set
            {
                midSideStereoEnabled = value;
            }
        }

        /// <summary>
        /// Private value for the TrueGapless property.
        /// </summary>
        private bool trueGapless = false;
        /// <summary>
        /// Defines if true gapless is used.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Defines if true gapless is used.")]
        #endif
        public bool TrueGapless
        {
            get
            {
                return trueGapless;
            }
            set
            {
                trueGapless = value;
            }
        }

        #endregion

        #region Encoder Info

        /// <summary>
        /// Private value for the EncoderProfile property.
        /// </summary>
        private int encoderProfile = 0;
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
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Profile (ranges from 0 to 15. Q0 starts at 5, where Q10 = 15).")]
        #endif
        public int EncoderProfile
        {
            get
            {
                return encoderProfile;
            }
            set
            {
                encoderProfile = value;
            }
        }

        /// <summary>
        /// Private value for the EncoderVersion property.
        /// </summary>
        private string encoderVersion = string.Empty;
        /// <summary>
        /// Defines the encoder version (ex: mppenc Beta 1.16).
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Encoder version (ex: mppenc Beta 1.16).")]
        #endif
        public string EncoderVersion
        {
            get
            {
                return encoderVersion;
            }
            set
            {
                encoderVersion = value;
            }
        }

        #endregion

        #region Replay Gain

        /// <summary>
        /// Private value for the TitleGain property.
        /// </summary>
        private int titleGain = 0;
        /// <summary>
        /// Change in the replay level. Value is treated as signed 16 bit value and the level is changed by that many mB (Millibel). 
        /// Thus level changes of -327.68 dB to +327.67 dB are possible.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Change in the replay level. Value is treated as signed 16 bit value and the level is changed by that many mB (Millibel). Thus level changes of -327.68 dB to +327.67 dB are possible.")]
        #endif
        public int TitleGain
        {
            get
            {
                return titleGain;
            }
            set
            {
                titleGain = value;
            }
        }

        /// <summary>
        /// Private value for the TitlePeak property.
        /// </summary>
        private int titlePeak = 0;
        /// <summary>
        /// Maximum level of the decoded title:
        /// 16422: -6 dB
        /// 32767:  0 dB
        /// 65379: +6 dB
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Maximum level of the decoded title: 16422=-6dB, 32767=0dB, 65379=+6dB.")]
        #endif
        public int TitlePeak
        {
            get
            {
                return titlePeak;
            }
            set
            {
                titlePeak = value;
            }
        }

        /// <summary>
        /// Private value for the AlbumGain property.
        /// </summary>
        private int albumGain = 0;
        /// <summary>
        /// Change in the replay level if the whole cd is supposed to be played with the same level change for all tracks. 
        /// Value is treated as signed 16 bit value and the level is attenuated by that many mB (Millibel). 
        /// Thus, level changes of -327.68 dB to +327.67 dB are possible.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Change in the replay level if the whole cd is supposed to be played with the same level change for all tracks. Value is treated as signed 16 bit value and the level is attenuated by that many mB (Millibel). Thus, level changes of -327.68 dB to +327.67 dB are possible.")]
        #endif
        public int AlbumGain
        {
            get
            {
                return albumGain;
            }
            set
            {
                albumGain = value;
            }
        }

        /// <summary>
        /// Private value for the AlbumPeak property.
        /// </summary>
        private int albumPeak = 0;
        /// <summary>
        /// Maximum level of the whole decoded CD:
        /// 16422: -6 dB
        /// 32767:  0 dB
        /// 65379: +6 dB
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Maximum level of the whole decoded CD: 16422=-6dB, 32767=0dB, 65379=+6dB.")]
        #endif
        public int AlbumPeak
        {
            get
            {
                return albumPeak;
            }
            set
            {
                albumPeak = value;
            }
        }

        #endregion

        #region Metadata

        /// <summary>
        /// Private value for the Bitrate property.
        /// </summary>
        private int bitrate = 0;
        /// <summary>
        /// Audio file bitrate. This value is not part of the SV7 header.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Metadata"), Browsable(true), ReadOnly(true), Description("[Metadata] Audio file bitrate. This value is not part of the SV7 header.")]
        #endif
        public int Bitrate
        {
            get
            {
                return bitrate;
            }
            set
            {
                bitrate = value;
            }
        }

        /// <summary>
        /// Private value for the LengthSamples property.
        /// </summary>
        private long lengthSamples = 0;
        /// <summary>
        /// Audio file length (in samples). This value is not part of the SV7 header.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Metadata"), Browsable(true), ReadOnly(true), Description("[Metadata] Audio file length (in samples). This value is not part of the SV7 header.")]
        #endif
        public long LengthSamples
        {
            get
            {
                return lengthSamples;
            }
            set
            {
                lengthSamples = value;
            }
        }

        /// <summary>
        /// Private value for the LengthMS property.
        /// </summary>
        private long lengthMS = 0;
        /// <summary>
        /// Audio file length (in milliseconds). This value is not part of the SV7 header.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Metadata"), Browsable(true), ReadOnly(true), Description("[Metadata] Audio file length (in milliseconds). This value is not part of the SV7 header.")]
        #endif
        public long LengthMS
        {
            get
            {
                return lengthMS;
            }
            set
            {
                lengthMS = value;
            }
        }

        /// <summary>
        /// Private value for the Length property.
        /// </summary>
        private string length = string.Empty;
        /// <summary>
        /// Audio file length (in time string format, 00:00.000). This value is not part of the SV7 header.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Metadata"), Browsable(true), ReadOnly(true), Description("[Metadata] Audio file length (in time string format, 00:00.000). This value is not part of the SV7 header.")]
        #endif
        public string Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
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
