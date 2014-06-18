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
using Sessions.MVP.Models;
using Sessions.Sound.AudioFiles;

namespace Sessions.MVP.Views
{
	/// <summary>
	/// Library browser view interface for mobile devices.
	/// </summary>
    public interface IMobileLibraryBrowserView : IBaseView
	{
        Action<MobileLibraryBrowserType> OnChangeBrowserType { get; set; }
        Action<Guid> OnItemClick { get; set; }
        Action<Guid> OnDeleteItem { get; set; }
        Action<Guid> OnPlayItem { get; set; }
        Action<Guid> OnAddItemToPlaylist { get; set; }
        Action<Guid> OnAddRemoveItemQueue { get; set; }
        Action<string, string, object> OnRequestAlbumArt { get; set; }
        Func<string, string, byte[]> OnRequestAlbumArtSynchronously { get; set; }

        void MobileLibraryBrowserError(Exception ex);
        void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle, string navigationBarSubtitle, string breadcrumb, bool isPopBackstack, bool isBackstackEmpty);
        void RefreshCurrentlyPlayingSong(Guid id, AudioFile audioFile);
        void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData, object userData);
	    void NotifyNewPlaylistItems(string text);
	}

    public enum MobileLibraryBrowserType
    {
        Playlists = 0,
        Artists = 1,
        Albums = 2,
        Songs = 3
    }
}
