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
using Sessions.MVP.Presenters;
using Sessions.Player.Objects;

namespace Sessions.MVP.Views
{
	/// <summary>
	/// Markers view interface.
	/// </summary>
	public interface IMarkersView : IBaseView
	{
        Action OnAddMarker { get; set; }
        Action<MarkerTemplateNameType> OnAddMarkerWithTemplate { get; set; }
        Action<Marker> OnEditMarker { get; set; }
        Action<Marker> OnSelectMarker { get; set; }
        Action<Marker> OnDeleteMarker { get; set; }
        Action<Marker> OnUpdateMarker { get; set; }
        Action<Guid> OnPunchInMarker { get; set; }
        Action<Guid> OnUndoMarker { get; set; }
        Action<Guid> OnSetActiveMarker { get; set; }
        Action<Guid, string> OnChangeMarkerName { get; set; }
        Action<Guid, float> OnChangeMarkerPosition { get; set; }
        Action<Guid, float> OnSetMarkerPosition { get; set; }

        void MarkerError(Exception ex);
        void RefreshMarkers(List<Marker> markers);
        void RefreshMarkerPosition(Marker marker, int newIndex);
	}
}
