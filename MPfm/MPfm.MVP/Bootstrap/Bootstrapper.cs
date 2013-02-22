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

using MPfm.Library.Database;
using MPfm.Library.Database.Interfaces;
using MPfm.MVP.Services;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Presenters;
using MPfm.MVP.Helpers;
using MPfm.MVP.Services.Interfaces;

namespace MPfm.MVP.Bootstrap
{
    /// <summary>
    /// Singleton static class for bootstrapping the application.
    /// Configures AutoMapper and Ninject.
    /// </summary>
    public static class Bootstrapper
    {
        /// <summary>
        /// Constructor for the Bootstrapper static class.
        /// </summary>
        static Bootstrapper()
        {
            // Get IoC container
            var container = TinyIoC.TinyIoCContainer.Current;

            // Register services
            container.Register<IDatabaseFacade>(new DatabaseFacade(ConfigurationHelper.DatabaseFilePath));
            container.Register<IInitializationService, InitializationService>().AsSingleton();
            container.Register<IPlayerService, PlayerService>().AsSingleton();
            container.Register<ILibraryService, LibraryService>().AsSingleton();
            container.Register<IAudioFileCacheService, AudioFileCacheService>().AsSingleton();
            container.Register<IUpdateLibraryService, UpdateLibraryService>().AsSingleton();

            // Register presenters
            container.Register<ISplashPresenter, SplashPresenter>().AsSingleton();
            container.Register<IMainPresenter, MainPresenter>().AsSingleton();
            container.Register<IPlayerPresenter, PlayerPresenter>().AsSingleton();
            container.Register<ISongBrowserPresenter, SongBrowserPresenter>().AsSingleton();
            container.Register<IMobileLibraryBrowserPresenter, MobileLibraryBrowserPresenter>().AsMultiInstance();
            container.Register<IMobileOptionsMenuPresenter, MobileOptionsMenuPresenter>().AsSingleton();
            container.Register<ILibraryBrowserPresenter, LibraryBrowserPresenter>().AsSingleton();
            container.Register<IUpdateLibraryPresenter, UpdateLibraryPresenter>().AsSingleton();
            container.Register<IEffectsPresenter, EffectsPresenter>().AsSingleton();
            container.Register<IAudioPreferencesPresenter, AudioPreferencesPresenter>().AsSingleton();
            container.Register<IGeneralPreferencesPresenter, GeneralPreferencesPresenter>().AsSingleton();
            container.Register<ILibraryPreferencesPresenter, LibraryPreferencesPresenter>().AsSingleton();
            container.Register<IPlaylistPresenter, PlaylistPresenter>().AsSingleton();
            container.Register<ILoopsPresenter, LoopsPresenter>().AsSingleton();
            container.Register<IMarkersPresenter, MarkersPresenter>().AsSingleton();
            container.Register<ITimeShiftingPresenter, TimeShiftingPresenter>().AsSingleton();
            container.Register<IPitchShiftingPresenter, PitchShiftingPresenter>().AsSingleton();
            container.Register<IPlayerMetadataPresenter, PlayerMetadataPresenter>().AsSingleton();
        }
        
        /// <summary>
        /// Returns the IoC container.
        /// </summary>
        /// <returns>IoC container</returns>
        public static TinyIoC.TinyIoCContainer GetContainer()
        {
            return TinyIoC.TinyIoCContainer.Current;
        }
    }
}

