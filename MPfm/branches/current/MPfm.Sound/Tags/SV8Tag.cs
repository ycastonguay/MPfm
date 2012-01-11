﻿//
// SV8Tag.cs: Data structure for SV8/MPC tags.
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
    /// Data structure for SV8/MPC tags.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SV8Tag
    {
        #region Stream Header Packet
        
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
        /// Private value for the Length property.
        /// </summary>
        private long m_length = 0;
        /// <summary>
        /// Defines the audio file length in samples.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Audio file length (in samples).")]
        public long Length
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

        /// <summary>
        /// Private value for the BeginningSilence property.
        /// </summary>
        private long m_beginningSilence = 0;
        /// <summary>
        /// Defines the number of samples to skip at the beginning of the stream (silence).
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Number of samples to skip at the beginning of the stream (silence).")]
        public long BeginningSilence
        {
            get
            {
                return m_beginningSilence;
            }
            set
            {
                m_beginningSilence = value;
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
        /// Private value for the AudioBlockFrames property.
        /// </summary>
        private int m_audioBlockFrames = 0;
        /// <summary>
        /// Defines the number of frames per audio packet.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Number of frames per audio packet.")]
        public int AudioBlockFrames
        {
            get
            {
                return m_audioBlockFrames;
            }
            set
            {
                m_audioBlockFrames = value;
            }
        }

        /// <summary>
        /// Private value for the MaxUsedBands property.
        /// </summary>
        private int m_maxUsedBands = 0;
        /// <summary>
        /// Defines the maximum number of bands used in the audio file.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("[Stream Header] Maximum number of bands used in the audio file.")]
        public int MaxUsedBands
        {
            get
            {
                return m_maxUsedBands;
            }
            set
            {
                m_maxUsedBands = value;
            }
        }

        #endregion

        #region Replay Gain Packet
        
        /// <summary>
        /// Private value for the ReplayGainVersion property.
        /// </summary>
        private int m_replayGainVersion = 0;
        /// <summary>
        /// Defines the Replay Gain version.
        /// </summary>
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] Replay Gain version.")]
        public int ReplayGainVersion
        {
            get
            {
                return m_replayGainVersion;
            }
            set
            {
                m_replayGainVersion = value;
            }
        }

        /// <summary>
        /// Private value for the TitleGain property.
        /// </summary>
        private int m_titleGain = 0;
        /// <summary>
        /// The loudness calculated for the title, and not the gain that the player must apply.
        /// </summary>
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] The loudness calculated for the title, and not the gain that the player must apply.")]
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
        /// The gain calculated for the title.
        /// </summary>
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] The gain calculated for the title.")]
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
        /// The loudness calculated for the album.
        /// </summary>
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] The loudness calculated for the album.")]
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
        /// The gain calculated for the album.
        /// </summary>
        [Category("Replay Gain"), Browsable(true), ReadOnly(true), Description("[Replay Gain] The gain calculated for the album.")]
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

        #region Encoder Information Packet

        /// <summary>
        /// Private value for the EncoderProfile property.
        /// </summary>
        private int m_encoderProfile = 0;
        /// <summary>
        /// Encoder quality in 4.3 format.
        /// </summary>
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Quality in 4.3 format.")]
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
        /// Private value for the EncoderPNSTool property.
        /// </summary>
        private bool m_encoderPNSTool = false;
        /// <summary>
        /// Encoder PNS tool enabled.
        /// </summary>
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] PNS tool enabled.")]
        public bool EncoderPNSTool
        {
            get
            {
                return m_encoderPNSTool;
            }
            set
            {
                m_encoderPNSTool = value;
            }
        }

        /// <summary>
        /// Private value for the EncoderMajor property.
        /// </summary>
        private int m_encoderMajor = 0;
        /// <summary>
        /// Encoder major version.
        /// </summary>
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Encoder major version.")]
        public int EncoderMajor
        {
            get
            {
                return m_encoderMajor;
            }
            set
            {
                m_encoderMajor = value;
            }
        }

        /// <summary>
        /// Private value for the EncoderMinor property.
        /// </summary>
        private int m_encoderMinor = 0;
        /// <summary>
        /// Encoder minor version.
        /// </summary>
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Encoder minor version.")]
        public int EncoderMinor
        {
            get
            {
                return m_encoderMinor;
            }
            set
            {
                m_encoderMinor = value;
            }
        }

        /// <summary>
        /// Private value for the EncoderBuild property.
        /// </summary>
        private int m_encoderBuild = 0;
        /// <summary>
        /// Encoder build version.
        /// </summary>
        [Category("Encoder Information"), Browsable(true), ReadOnly(true), Description("[Encoder Info] Encoder build version.")]
        public int EncoderBuild
        {
            get
            {
                return m_encoderBuild;
            }
            set
            {
                m_encoderBuild = value;
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