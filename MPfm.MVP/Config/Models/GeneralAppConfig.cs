// Copyright © 2011-2013 Yanick Castonguay
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

using System.Collections.Generic;
using MPfm.Library.Objects;

namespace MPfm.MVP.Config.Models
{
    /// <summary>
    /// Class containing all general settings.
    /// </summary>
    public class GeneralAppConfig : IAppConfig
    {
        public bool UseCustomPeakFileFolder { get; set; }
        public string CustomPeakFileFolder { get; set; }
        public int MaximumPeakFolderSize { get; set; }

        public int SongPositionUpdateFrequency { get; set; }
        public int OutputMeterUpdateFrequency { get; set; }

        public bool ShowTooltips { get; set; }
        public bool ShowAppInSystemTray { get; set; }
        public bool MinimizeAppInSystemTray { get; set; }

        public GeneralAppConfig()
        {
            MaximumPeakFolderSize = 100;
            SongPositionUpdateFrequency = 20;
            OutputMeterUpdateFrequency = 20;
            ShowTooltips = true;
        }        
    }
}
