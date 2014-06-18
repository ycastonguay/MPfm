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
using System.Collections.Generic;
using Sessions.MVP.Views;

namespace Sessions.MVP.Models
{
	public class MobileOptionsMenuEntity
	{
        public string Title { get; set; }
        public string HeaderTitle { get; set; }
        public MobileOptionsMenuType MenuType { get; set; }

        public MobileOptionsMenuEntity()
        {
        }
    }

    public enum MobileOptionsMenuType
    {
        About = 0,
        Preferences = 1,
        EqualizerPresets = 2,
        UpdateLibrary = 3,
        SyncLibrary = 4,
        SyncLibraryFileSharing = 5,
        SyncLibraryWebBrowser = 6,
        SyncLibraryCloud = 7,
        ResumePlayback = 8,
        AudioPreferences = 9,
        CloudPreferences = 10,
        GeneralPreferences = 11,
        LibraryPreferences = 12
    }
}
