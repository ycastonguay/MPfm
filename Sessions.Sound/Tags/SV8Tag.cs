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
    /// Data structure for SV8/MPC tags.
    /// </summary>
    #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
    [TypeConverter(typeof(ExpandableObjectConverter))]
    #endif
    public class SV8Tag
    {
        #region Stream Header Packet
        
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
        /// Private value for the LengthSamples property.
        /// </summary>
        private long lengthSamples = 0;
        /// <summary>
        /// Defines the audio file length in samples.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Audio file length (in samples).")]
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
        /// Private value for the BeginningSilence property.
        /// </summary>
        private long beginningSilence = 0;
        /// <summary>
        /// Defines the number of samples to skip at the beginning of the stream (silence).
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Number of samples to skip at the beginning of the stream (silence).")]
        #endif
        public long BeginningSilence
        {
            get
            {
                return beginningSilence;
            }
            set
            {
                beginningSilence = value;
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
        /// Private value for the AudioBlockFrames property.
        /// </summary>
        private int audioBlockFrames = 0;
        /// <summary>
        /// Defines the number of frames per audio packet.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Number of frames per audio packet.")]
        #endif
        public int AudioBlockFrames
        {
            get
            {
                return audioBlockFrames;
            }
            set
            {
                audioBlockFrames = value;
            }
        }

        /// <summary>
        /// Private value for the MaxUsedBands property.
        /// </summary>
        private int maxUsedBands = 0;
        /// <summary>
        /// Defines the maximum number of bands used in the audio file.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Maximum number of bands used in the audio file.")]
        #endif
        public int MaxUsedBands
        {
            get
            {
                return maxUsedBands;
            }
            set
            {
                maxUsedBands = value;
            }
        }

        #endregion

        #region Replay Gain Packet
        
        /// <summary>
        /// Private value for the ReplayGainVersion property.
        /// </summary>
        private int replayGainVersion = 0;
        /// <summary>
        /// Defines the Replay Gain version.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Replay Gain version.")]
        #endif
        public int ReplayGainVersion
        {
            get
            {
                return replayGainVersion;
            }
            set
            {
                replayGainVersion = value;
            }
        }

        /// <summary>
        /// Private value for the TitleGain property.
        /// </summary>
        private int titleGain = 0;
        /// <summary>
        /// The loudness calculated for the title, and not the gain that the player must apply.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] The loudness calculated for the title, and not the gain that the player must apply.")]
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
        /// The gain calculated for the title.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] The gain calculated for the title.")]
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
        /// The loudness calculated for the album.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] The loudness calculated for the album.")]
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
        /// The gain calculated for the album.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] The gain calculated for the album.")]
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

        #region Encoder Information Packet

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
        /// Private value for the EncoderPNSTool property.
        /// </summary>
        private bool encoderPNSTool = false;
        /// <summary>
        /// Encoder PNS tool enabled.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] PNS tool enabled.")]
        #endif
        public bool EncoderPNSTool
        {
            get
            {
                return encoderPNSTool;
            }
            set
            {
                encoderPNSTool = value;
            }
        }

        /// <summary>
        /// Private value for the EncoderMajor property.
        /// </summary>
        private int encoderMajor = 0;
        /// <summary>
        /// Encoder major version.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Encoder major version.")]
        #endif
        public int EncoderMajor
        {
            get
            {
                return encoderMajor;
            }
            set
            {
                encoderMajor = value;
            }
        }

        /// <summary>
        /// Private value for the EncoderMinor property.
        /// </summary>
        private int encoderMinor = 0;
        /// <summary>
        /// Encoder minor version.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Encoder minor version.")]
        #endif
        public int EncoderMinor
        {
            get
            {
                return encoderMinor;
            }
            set
            {
                encoderMinor = value;
            }
        }

        /// <summary>
        /// Private value for the EncoderBuild property.
        /// </summary>
        private int encoderBuild = 0;
        /// <summary>
        /// Encoder build version.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Encoder build version.")]
        #endif
        public int EncoderBuild
        {
            get
            {
                return encoderBuild;
            }
            set
            {
                encoderBuild = value;
            }
        }

        #endregion

        #region Seek Table Offset Packet

        /// <summary>
        /// Private value for the SeekTableOffset property.
        /// </summary>
        private long seekTableOffset = 0;
        /// <summary>
        /// Offset from this packet to the seek table packet.
        /// </summary>
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        [Category("Seek Table Offset"), Browsable(true), ReadOnly(true), Description("[Seek Table Offset] Offset from this packet to the seek table packet.")]
        #endif
        public long SeekTableOffset
        {
            get
            {
                return seekTableOffset;
            }
            set
            {
                seekTableOffset = value;
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
        /// Default constructor for the SV8Tag class.
        /// </summary>
        public SV8Tag()
        {
        }
    }
}
