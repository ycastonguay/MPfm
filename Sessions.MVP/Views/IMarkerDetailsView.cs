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
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;

namespace Sessions.MVP.Views
{
	/// <summary>
	/// Marker details view interface.
	/// </summary>
	public interface IMarkerDetailsView : IBaseView
	{
        Action<float> OnChangePositionMarkerDetails { get; set; }
        Action<Marker> OnUpdateMarkerDetails { get; set; }
        Action OnDeleteMarkerDetails { get; set; }
        Action OnPunchInMarkerDetails { get; set; }

        void MarkerDetailsError(Exception ex);
        void DismissMarkerDetailsView();
        void RefreshMarker(Marker marker, AudioFile audioFile);
        void RefreshMarkerPosition(string position, float positionPercentage);
	}
}
