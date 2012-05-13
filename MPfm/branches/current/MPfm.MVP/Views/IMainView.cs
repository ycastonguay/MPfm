//
// IMainView.cs: Main window view interface.
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MPfm.Core;
using MPfm.Sound;

namespace MPfm.MVP
{
	/// <summary>
	/// Main window view interface.
	/// </summary>
	public interface IMainView
	{
		/// <summary>
		/// This method is called when the song position needs to be refreshed.
		/// </summary>
		/// <param name='entity'>
		/// Player position entity.
		/// </param>
		void RefreshPlayerPosition(PlayerPositionEntity entity);
		/// <summary>
		/// This method is called when the song information needs to be refreshed.
		/// </summary>
		/// <param name='entity'>
		/// Song information entity.
		/// </param>
		void RefreshSongInformation(SongInformationEntity entity);
	}
}

