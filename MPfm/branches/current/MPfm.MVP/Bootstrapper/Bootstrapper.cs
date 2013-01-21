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

using MPfm.MVP.Models;
using AutoMapper;
using MPfm.Sound.AudioFiles;
using Ninject;

namespace MPfm.MVP.Bootstrapper
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

        static LibraryModule module;
		
		/// <summary>
		/// Constructor for the Bootstrapper static class.
		/// </summary>
		static Bootstrapper()
		{
			// Create Ninject kernel
            module = new LibraryModule();
			kernel = new StandardKernel(module);
			
			// Configure Automapper
			Mapper.CreateMap<AudioFile, SongInformationEntity>();
		}
		
		/// <summary>
		/// Returns the instance of the StandardKernel for resolving dependencies.
		/// </summary>
		/// <returns>StandardKernel</returns>		
		public static IKernel GetKernel()
		{
			return kernel;
		}
	}
}

