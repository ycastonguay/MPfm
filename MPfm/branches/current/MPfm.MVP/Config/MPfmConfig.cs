//
// MPfmConfig.cs: Singleton containing all settings for MPfm.
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
using System.Collections.Generic;
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;

namespace MPfm.MVP
{
    /// <summary>
    /// Singleton containing all settings for MPfm.
    /// </summary>
    public class MPfmConfig
    {
        #region Singleton

        private static MPfmConfig instance;
        /// <summary>
        /// MPfmConfig instance.
        /// </summary>
        public static MPfmConfig Instance
        {
            get
            {
                if(instance == null)
                    instance = new MPfmConfig();
                return instance;
            }
        }

        #endregion

        #region Properties

        public bool IsFirstRun { get; set; }
        public bool ShowTooltips { get; set; }
        public AudioFileFormat FilterSoundFormat { get; set; }

        public int WindowSplitterDistance { get; set; }
        public int PositionUpdateFrequency { get; set; }
        public int OutputMeterUpdateFrequency { get; set; }

        public bool PeakFileUseCustomDirectory { get; set; }
        public string PeakFileCustomDirectory { get; set; }
        public bool PeakFileIsDisplayWarning { get; set; }
        public int PeakFileWarningThreshold { get; set; }

        public MPfmAudioConfig Audio { get; set; }
        public MPfmControlsConfig Controls { get; set; }       
        public MPfmWindowsConfig Windows { get; set; }

        #endregion

        public MPfmConfig()
        {
            // Set defaults
            IsFirstRun = true;
            ShowTooltips = true;
            PositionUpdateFrequency = 10;
            OutputMeterUpdateFrequency = 10;

            Audio = new MPfmAudioConfig();
            Controls = new MPfmControlsConfig();
            Windows = new MPfmWindowsConfig();
        }

        public void Load()
        {
            instance = ConfigurationHelper.Load(ConfigurationHelper.ConfigurationFilePath);
        }
    }
}
