//
// Bootstrapper.cs: Singleton static class for bootstrapping the application.
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
	/// Singleton static class for bootstrapping the application.
	/// Configures AutoMapper and Ninject.
	/// </summary>
	public static class Bootstrapper
	{
		/// <summary>
		/// Instance of the Ninject StandardKernel.
		/// </summary>
		private static IKernel kernel;
		
		/// <summary>
		/// Constructor for the Bootstrapper static class.
		/// </summary>
		static Bootstrapper()
		{
			kernel = new StandardKernel(new LibraryModule());
			
			// Configure automapper
			Mapper.CreateMap<AudioFile, SongInformationEntity>();
		}
		
		/// <summary>
		/// Returns the instance of the StandardKernel for resolving dependencies.
		/// </summary>
		/// <returns>StandardKernel</returns>		
		public static IKernel GetServiceLocator()
		{
			return kernel;
		}
	}
}

