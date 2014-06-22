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
using Sessions.MVP.Presenters;

namespace Sessions.MVP.Views
{
	/// <summary>
	/// Time shifting view interface.
	/// </summary>
	public interface ITimeShiftingView : IBaseView
	{
        Action<float> OnSetTimeShifting { get; set; }
        Action OnResetTimeShifting { get; set; }
        Action OnUseDetectedTempo { get; set; }
        Action OnIncrementTempo { get; set; }
        Action OnDecrementTempo { get; set; }

        void TimeShiftingError(Exception ex);
        void RefreshTimeShifting(PlayerTimeShiftingEntity entity);
	}
}