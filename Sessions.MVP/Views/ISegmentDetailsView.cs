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
using Sessions.Player.Objects;

namespace MPfm.MVP.Views
{
    public interface ISegmentDetailsView : IBaseView
	{
        Action<float> OnChangePositionSegmentDetails { get; set; }
        Action OnPunchInPositionSegmentDetails { get; set; }
        Action<Segment> OnUpdateSegmentDetails { get; set; }
        Action<Guid> OnLinkToMarkerSegmentDetails { get; set; }

        void SegmentDetailsError(Exception ex);
        void RefreshSegmentDetails(Segment segment, long audioFileLength);        
        void RefreshSegmentPosition(string position, float positionPercentage);
        void RefreshSegmentMarkers(IEnumerable<Marker> markers);
	}
}
