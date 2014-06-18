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

namespace Sessions.MVP.Views
{
	/// <summary>
	/// Main view interface for desktop platforms. Combines several views together. 
	/// Unfortunately it is not easy/possible to create subviews on all desktop platforms like on mobile devices (i.e. GTK).
	/// </summary>
    public interface IMainView : ILibraryBrowserView, ISongBrowserView, IPlayerView, IMarkersView, IMarkerDetailsView, ILoopsView, ILoopDetailsView, ILoopPlaybackView, ISegmentDetailsView, ITimeShiftingView, IPitchShiftingView, IUpdateLibraryView, IQueueView
	{
        Action OnOpenAboutWindow { get; set; }
        Action OnOpenPreferencesWindow { get; set; }
        Action OnOpenEffectsWindow { get; set; }
        Action OnOpenPlaylistWindow { get; set; }
        Action OnOpenSyncWindow { get; set; }
        Action OnOpenSyncCloudWindow { get; set; }
        Action OnOpenSyncWebBrowserWindow { get; set; }
        Action OnOpenResumePlayback { get; set; }
	}
}
