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

using System;
using System.Collections.Generic;
using MPfm.Sound.AudioFiles;
using MPfm.Library.Objects;
using MPfm.MVP.Presenters;

namespace MPfm.MVP.Models
{
	public class SyncMenuItemEntity
	{
        public SyncMenuItemEntityType ItemType { get; set; }
        public bool IsExpanded { get; set; }
        public StateSelectionType Selection { get; set; }
        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }
        public AudioFile Song { get; set; }

	    public string Title
	    {
	        get
	        {
	            switch (ItemType)
	            {
	                case SyncMenuItemEntityType.Artist:
	                    return ArtistName;
	                    break;
	                case SyncMenuItemEntityType.Album:
	                    return AlbumTitle;
	                    break;
	                case SyncMenuItemEntityType.Song:
	                    if (Song != null)
	                        return Song.Title;
	                    break;
	            }
	            return string.Empty;
	        }
	    }
	}
	
    public enum SyncMenuItemEntityType
	{
		Artist = 0, Album = 1, Song = 2
	}

    public enum StateSelectionType
    {
        None = 0,
        Selected = 1,
        PartlySelected = 2
    }
}
