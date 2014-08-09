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
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;

namespace Sessions.MVP.Views
{
	public interface ILoopDetailsView : IBaseView
	{
        Action OnAddSegment { get; set; }
        Action<Loop> OnUpdateLoopDetails { get; set; }
        Action<Segment> OnPunchInSegment { get; set; }
        Action<Segment, Guid> OnLinkSegmentToMarker { get; set; }
        Action<Segment, float> OnChangingSegmentPosition { get; set; }
        Action<Segment, float> OnChangedSegmentPosition { get; set; }

        // Legacy
        Action<Guid, int> OnAddSegmentFromMarker { get; set; }
        Action<Segment> OnEditSegment { get; set; }
        Action<Segment> OnDeleteSegment { get; set; }
        Action<Segment, int> OnChangeSegmentOrder { get; set; }

        void LoopDetailsError(Exception ex);
        void RefreshLoopDetails(Loop loop, AudioFile audioFile, long audioFileLength);
	    void RefreshLoopDetailsSegment(Segment segment);
        void RefreshLoopMarkers(IEnumerable<Marker> markers);
	}
}
