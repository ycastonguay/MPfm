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

using System;

namespace Sessions.Library.Objects
{
    /// <summary>
	/// Object representing a device with informations; simplified for cloud storage.
    /// </summary>
	public class CloudDeviceInfo
	{
		public Guid AudioFileId { get; set; }
		public string ArtistName { get; set; }
		public string AlbumTitle { get; set; }
		public string SongTitle { get; set; }
		public string Position { get; set; }
		public long PositionBytes { get; set; }
		public string DeviceType { get; set; }
		public string DeviceName { get; set; }
		public string DeviceId { get; set; }
		public string IPAddress { get; set; }
		public DateTime Timestamp { get; set; }   
	}
}
