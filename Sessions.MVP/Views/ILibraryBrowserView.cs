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
	/// Library browser view interface.
	/// </summary>
    public interface ILibraryBrowserView : IBaseView
	{
        Action<AudioFileFormat> OnAudioFileFormatFilterChanged { get; set; }
        Action<LibraryBrowserEntity> OnTreeNodeSelected { get; set; }        
        Action<LibraryBrowserEntity> OnTreeNodeDoubleClicked { get; set; }
        Action<LibraryBrowserEntity, object> OnTreeNodeExpanded { get; set; }
        Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable { get; set; }
        Action<LibraryBrowserEntity> OnAddToPlaylist { get; set; }
        Action<LibraryBrowserEntity> OnRemoveFromLibrary { get; set; }
        Action<LibraryBrowserEntity> OnDeleteFromHardDisk { get; set; }

        void LibraryBrowserError(Exception ex);
		void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities);
		void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData);
        void RefreshLibraryBrowserSelectedNode(LibraryBrowserEntity entity);
        void NotifyLibraryBrowserNewNode(int position, LibraryBrowserEntity entity);
        void NotifyLibraryBrowserRemovedNode(int position);
	}
}
