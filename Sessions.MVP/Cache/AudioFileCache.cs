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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using MPfm.Core;
using MPfm.Library;
using MPfm.MVP;
using MPfm.Player;
using MPfm.Sound;
using AutoMapper;
using Ninject;

namespace MPfm.MVP
{
	/// <summary>
	/// Singleton for caching audio file metadata.
	/// </summary>
	public static class AudioFileCache
	{
		private static AudioFileCacheService service;
	
		static AudioFileCache()
		{			
			service = Bootstrapper.GetKernel().Get<AudioFileCacheService>();
		}
		
		public static AudioFileCacheService GetService()
		{
			return service;
		}
	}
}

