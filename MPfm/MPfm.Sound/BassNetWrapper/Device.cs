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

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// Defines an audio device that can be used with the Player.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Defines the driver type (DirectSound, ASIO or WASAPI).
        /// </summary>
        public DriverType DriverType { get; set; }

        /// <summary>
        /// True if the device is the default device for this driver type.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Device id (needed for initialization).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Device name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Device driver identifier.
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// Default constructor for the Device class.
        /// </summary>
        public Device()
        {
            // Set default values
            DriverType = DriverType.DirectSound;
            IsDefault = true;
            Id = -1; // default id for default device in BASS
            Name = "Default device";
            Driver = string.Empty;
        }
    }
}
