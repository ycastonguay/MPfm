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
using MPfm.Sound.AudioFiles;

namespace MPfm.Library.Objects
{
    /// <summary>
    /// Data structure repesenting a syncable device.
    /// </summary>
	public class SyncDevice
	{
        public SyncDeviceType DeviceType { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<AudioFile> AudioFiles { get; set; }

		public SyncDevice()
		{
            AudioFiles = new List<AudioFile>();
		}
	}
	
	/// <summary>
	/// Syncable device type.
	/// </summary>
	public enum SyncDeviceType
	{
        Unknown = 0,
		Linux = 1, 
        OSX = 2, 
        Windows = 3, 
        iOS = 4, 
        Android = 5
	}
}

