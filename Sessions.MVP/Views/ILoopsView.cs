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
using org.sessionsapp.player;

namespace Sessions.MVP.Views
{
	public interface ILoopsView : IBaseView
	{
        Action OnAddLoop { get; set; }
        Action<SSPLoop> OnEditLoop { get; set; }
        Action<SSPLoop> OnSelectLoop { get; set; }
        Action<SSPLoop> OnDeleteLoop { get; set; }
        Action<SSPLoop> OnPlayLoop { get; set; }
        Action<SSPLoop> OnUpdateLoop { get; set; }

        Action<SSPLoopSegmentType> OnPunchInLoopSegment { get; set; }
        Action<SSPLoopSegmentType, float> OnChangingLoopSegmentPosition { get; set; }
        Action<SSPLoopSegmentType, float> OnChangedLoopSegmentPosition { get; set; }

        Action<Guid, string> OnChangeLoopName { get; set; }

        void LoopError(Exception ex);
        void RefreshPlayingLoop(SSPLoop loop, bool isPlaying);
        void RefreshCurrentlyEditedLoop(SSPLoop loop);
        void RefreshLoops(List<SSPLoop> loops);
	}
}
