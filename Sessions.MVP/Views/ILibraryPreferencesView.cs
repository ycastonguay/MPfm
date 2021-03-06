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
using Sessions.MVP.Config.Models;
using Sessions.Library.Objects;

namespace Sessions.MVP.Views
{
	/// <summary>
    /// Library preferences view interface.
	/// </summary>
    public interface ILibraryPreferencesView : IBaseView
	{
        Action<LibraryAppConfig> OnSetLibraryPreferences { get; set; }
        Action OnSelectFolders { get; set; }
        Action OnResetLibrary { get; set; }
        Action OnUpdateLibrary { get; set; }
        Action<string, bool> OnAddFolder { get; set; }
        Action<IEnumerable<Folder>, bool> OnRemoveFolders { get; set; }
        Action<bool> OnEnableSyncListener { get; set; }
        Action<int> OnSetSyncListenerPort { get; set; }

        void LibraryPreferencesError(Exception ex);
        void RefreshLibraryPreferences(LibraryAppConfig config, string librarySize);
	}
}
