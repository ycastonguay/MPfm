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

namespace MPfm.MVP.Views
{
	/// <summary>
	/// Splash screen view interface.
	/// </summary>
    public interface IMainView : ILibraryBrowserView, ISongBrowserView, IPlayerView, IMarkersView, ILoopsView, ITimeShiftingView, IPitchShiftingView
	{
        Action OnOpenPreferencesWindow { get; set; }
        Action OnOpenEffectsWindow { get; set; }
        Action OnOpenPlaylistWindow { get; set; }
        Action OnOpenSyncWindow { get; set; }

        Action<List<string>> OnAddFilesToLibrary { get; set; }
        Action<string> OnAddFolderToLibrary { get; set; }
        Action OnUpdateLibrary { get; set; }
	}
}
