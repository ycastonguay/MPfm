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
using Sessions.Player.Objects;

namespace Sessions.MVP.Views
{
	/// <summary>
	/// Pitch shifting view interface.
	/// </summary>
	public interface IPitchShiftingView : IBaseView
	{
        Action<int> OnChangeKey { get; set; }
        Action<int> OnSetInterval { get; set; }
        Action OnResetInterval { get; set; }
        Action OnIncrementInterval { get; set; }
        Action OnDecrementInterval { get; set; }

        void PitchShiftingError(Exception ex);
        void RefreshKeys(List<Tuple<int, string>> keys);
        void RefreshPitchShifting(PlayerPitchShifting entity);
	}
}
