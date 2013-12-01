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

using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Config.Models
{
    public class RootAppConfig : IAppConfig
    {
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

        public AudioAppConfig Audio { get; set; }
        public CloudAppConfig Cloud { get; set; }
        public LibraryAppConfig Library { get; set; }
        public GeneralAppConfig General { get; set; }
        public ControlsAppConfig Controls { get; set; }       
        public WindowsAppConfig Windows { get; set; }

        public ResumePlaybackAppConfig ResumePlayback { get; set; }

        public RootAppConfig()
        {
            // Set defaults
            IsFirstRun = true;
            ShowTooltips = true;
            PositionUpdateFrequency = 10;
            OutputMeterUpdateFrequency = 10;

            Audio = new AudioAppConfig();
            Cloud = new CloudAppConfig();
            Controls = new ControlsAppConfig();
            Library = new LibraryAppConfig();     
            General = new GeneralAppConfig();       
            Windows = new WindowsAppConfig();

            ResumePlayback = new ResumePlaybackAppConfig();
        }
    }
}
