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
using MPfm.MVP.Models;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Views
{
	/// <summary>
	/// Library browser view interface for mobile devices.
	/// </summary>
    public interface IMobileLibraryBrowserView : IBaseView
	{
        MobileLibraryBrowserType BrowserType { get; set; }
        string Filter { get; set; }

	    //IBaseView PushView(); // Can push Player or MobileLibraryBrowserView instance

	    //Action<AudioFileFormat> OnAudioFileFormatFilterChanged { get; set; }
	    //Action<LibraryBrowserEntity> OnTreeNodeSelected { get; set; }        
	    //Action<LibraryBrowserEntity> OnTreeNodeDoubleClicked { get; set; }
	    //Action<LibraryBrowserEntity, object> OnTreeNodeExpanded { get; set; }
	    //Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable { get; set; }

	    //void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities);
	    //void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData);
	}

    public enum MobileLibraryBrowserType
    {
        Playlists = 0,
        Artists = 1,
        Albums = 2,
        Songs = 3
    }
}
