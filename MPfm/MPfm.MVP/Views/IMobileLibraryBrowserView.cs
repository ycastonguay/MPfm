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
using MPfm.MVP.Models;

namespace MPfm.MVP.Views
{
	/// <summary>
	/// Library browser view interface for mobile devices.
	/// </summary>
    public interface IMobileLibraryBrowserView : IBaseView
	{
        Action<MobileLibraryBrowserType> OnChangeBrowserType { get; set; }
        Action<int> OnItemClick { get; set; }
        Action<int> OnDeleteItem { get; set; }
		Action<int> OnPlayItem { get; set; }
        Action<string, string> OnRequestAlbumArt { get; set; }
        Func<string, string, byte[]> OnRequestAlbumArtSynchronously { get; set; }

        void MobileLibraryBrowserError(Exception ex);
        void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle, string navigationBarSubtitle, string breadcrumb, bool isPopBackstack, bool isBackstackEmpty);
        void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile);
        void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData);
	}

    public enum MobileLibraryBrowserType
    {
        Playlists = 0,
        Artists = 1,
        Albums = 2,
        Songs = 3
    }
}

