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

using Sessions.Sound.BassNetWrapper;
using org.sessionsapp.player;

namespace Sessions.MVP.Config.Models
{
    /// <summary>
    /// Class containing all audio settings for Sessions.
    /// </summary>
    public class AudioAppConfig : IAppConfig
    {
        public SSPDevice AudioDevice { get; set; }
        public int SampleRate { get; set; }
        public float Volume { get; set; }
        public int BufferSize { get; set; }
        public int UpdatePeriod { get; set; }
        public bool IsEQEnabled { get; set; }
        public string EQPreset { get; set; }

        public AudioAppConfig()
        {
            AudioDevice = new SSPDevice();
            SampleRate = 44100;
            Volume = 1;
            BufferSize = 1000;
            UpdatePeriod = 100;
        }        
    }
}
