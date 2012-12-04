//
// IPlayerView.cs: Player view interface.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MPfm.Core;
using MPfm.Sound;

namespace MPfm.MVP
{
	/// <summary>
	/// Player view interface.
	/// </summary>
    public interface IPlayerView : IBaseView
	{
        Action OnPlayerPlay { get; set; }
        Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        Action OnPlayerPause { get; set; }
        Action OnPlayerStop { get; set; }
        Action OnPlayerPrevious { get; set; }
        Action OnPlayerNext { get; set; }
        Action<float> OnPlayerSetVolume { get; set; }
        Action<float> OnPlayerSetPitchShifting { get; set; }
        Action<float> OnPlayerSetTimeShifting { get; set; }
        Action<float> OnPlayerSetPosition { get; set; }
        
		void RefreshPlayerPosition(PlayerPositionEntity entity);
		void RefreshSongInformation(SongInformationEntity entity);
        void RefreshPlayerVolume(PlayerVolumeEntity entity);
        void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity);
        
        void PlayerError(Exception ex);
	}
}

