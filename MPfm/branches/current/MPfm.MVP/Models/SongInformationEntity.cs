//
// SongInformationEntity.cs: Data structure repesenting song information.
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
using System.IO;
using System.Reflection;
using MPfm.Sound;

namespace MPfm.MVP
{
    /// <summary>
    /// Data structure repesenting song information.
    /// </summary>
    public class SongInformationEntity
    {
		public string ArtistName { get; set; }
		public string AlbumTitle { get; set; }
		public string Title { get; set; }
		public string FilePath { get; set; }
		public AudioFileFormat FileType { get; set; }
		
		public string Length { get; set; }
		public string Position { get; set; }		
		
		public int Bitrate { get; set; }
		public int BitsPerSample { get; set; }
		public int SampleRate { get; set; }
		
		public string FileTypeString { get; set; }
		public string BitrateString { get; set; }
		public string BitsPerSampleString { get; set; }
		public string SampleRateString { get; set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.MVP.SongInformationEntity"/> class.
		/// </summary>
		public SongInformationEntity()
		{
            ArtistName = string.Empty;
            AlbumTitle = string.Empty;
            Title = string.Empty;
            FilePath = string.Empty;
            FileType = AudioFileFormat.All;

            Length = "0:00.000";
            Position = "0:00.000";

            Bitrate = 128;
            BitsPerSample = 16;
            SampleRate = 44100;

            FileTypeString = string.Empty;
            BitrateString = string.Empty;
            BitsPerSampleString = string.Empty;
            SampleRateString = string.Empty;
		}
	}
}

