﻿//
// LibraryModule.cs: Configuration module for Ninject.
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

using MPfm.Library.Gateway;
using MPfm.MVP.Helpers;
using MPfm.MVP.Presenters;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services;
using MPfm.MVP.Services.Interfaces;
using TinyMessenger;

namespace MPfm.MVP.Bootstrapper
{
	/// <summary>
	/// Configuration module for Ninject.
	/// </summary>
	public class LibraryModule : Ninject.Modules.NinjectModule
	{
		/// <summary>
		/// Loads Ninject configuration.
		/// </summary>
		public override void Load()
		{			
			Bind<IMPfmGateway>().To<MPfmGateway>().WithConstructorArgument("databaseFilePath", ConfigurationHelper.DatabaseFilePath);
			
			Bind<IInitializationService>().To<InitializationService>().InSingletonScope();
            Bind<IPlayerService>().To<PlayerService>().InSingletonScope();
			Bind<ILibraryService>().To<LibraryService>().InSingletonScope();
			Bind<IAudioFileCacheService>().To<AudioFileCacheService>().InSingletonScope();		
            Bind<IUpdateLibraryService>().To<UpdateLibraryService>().InSingletonScope();
			
            Bind<ISplashPresenter>().To<SplashPresenter>().InSingletonScope();
            Bind<IMainPresenter>().To<MainPresenter>();
			Bind<IPlayerPresenter>().To<PlayerPresenter>().InSingletonScope();
			Bind<ISongBrowserPresenter>().To<SongBrowserPresenter>().InSingletonScope();
			Bind<ILibraryBrowserPresenter>().To<LibraryBrowserPresenter>().InSingletonScope();
			Bind<IUpdateLibraryPresenter>().To<UpdateLibraryPresenter>().InSingletonScope();
            Bind<IEffectsPresenter>().To<EffectsPresenter>().InSingletonScope();
            Bind<IPreferencesPresenter>().To<PreferencesPresenter>().InSingletonScope();
            Bind<IPlaylistPresenter>().To<PlaylistPresenter>().InSingletonScope();

            Bind<ITinyMessengerHub>().To<TinyMessengerHub>().InSingletonScope();
		}		
	}
}
