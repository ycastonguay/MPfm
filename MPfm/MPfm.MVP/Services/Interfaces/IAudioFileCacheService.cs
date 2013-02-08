//
// IAudioFileCacheService.cs: Interface for the AudioFileCacheService class.
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

using System.Collections.Generic;
using MPfm.MVP.Models;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Services.Interfaces
{
	/// <summary>
	/// Interface for the AudioFileCacheService class.
	/// </summary>
	public interface IAudioFileCacheService
	{
		void RefreshCache();
		IEnumerable<AudioFile> SelectAudioFiles(SongBrowserQueryEntity query);    
	}
}

