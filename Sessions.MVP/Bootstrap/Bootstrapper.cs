﻿// Copyright © 2011-2013 Yanick Castonguay
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

using Sessions.MVP.Presenters;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services;
using Sessions.MVP.Services.Interfaces;
using Sessions.Core.Helpers;
using Sessions.Library.Database;
using Sessions.Library.Database.Interfaces;
using Sessions.Library.Services;
using Sessions.Library.Services.Interfaces;
using Sessions.Sound.PeakFiles;
using TinyMessenger;
using Sessions.Core.Network;

namespace Sessions.MVP.Bootstrap
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
            container.Register<IDatabaseFacade>(new DatabaseFacade(PathHelper.DatabaseFilePath));
            container.Register<ITinyMessengerHub, TinyMessengerHub>().AsSingleton(); 
            container.Register<IInitializationService, InitializationService>().AsSingleton();
            container.Register<IPlayerService, PlayerService>().AsSingleton();            
            container.Register<ILibraryService, LibraryService>().AsSingleton();
            container.Register<IAudioFileCacheService, AudioFileCacheService>().AsSingleton();
            container.Register<IUpdateLibraryService, UpdateLibraryService>().AsSingleton();
            container.Register<IPeakFileService, PeakFileService>().AsSingleton();
            container.Register<ILifecycleService, LifecycleService>().AsSingleton();
            container.Register<IResumePlaybackService, ResumePlaybackService>().AsSingleton();
            container.Register<ISyncClientService, SyncClientService>().AsMultiInstance();
            container.Register<ISyncListenerService, SyncListenerService>().AsSingleton();
            container.Register<ISyncDiscoveryService, SyncDiscoveryService>().AsSingleton();
            container.Register<ISyncDeviceManagerService, SyncDeviceManagerService>().AsSingleton();
            container.Register<ICloudLibraryService, CloudLibraryService>().AsSingleton();
            container.Register<IHttpService, HttpService>().AsSingleton();
            container.Register<IHttpServiceFactory, HttpServiceFactory>().AsSingleton();

            // Register presenters
            container.Register<ISplashPresenter, SplashPresenter>().AsSingleton();
            container.Register<IMainPresenter, MainPresenter>().AsSingleton();
            container.Register<IPlayerPresenter, PlayerPresenter>().AsSingleton();
            container.Register<ISongBrowserPresenter, SongBrowserPresenter>().AsSingleton();
            container.Register<IMobileLibraryBrowserPresenter, MobileLibraryBrowserPresenter>().AsMultiInstance();
            container.Register<IMobileOptionsMenuPresenter, MobileOptionsMenuPresenter>().AsSingleton();
            container.Register<ILibraryBrowserPresenter, LibraryBrowserPresenter>().AsSingleton();
            container.Register<IUpdateLibraryPresenter, UpdateLibraryPresenter>().AsSingleton();
            container.Register<IEqualizerPresetsPresenter, EqualizerPresetsPresenter>().AsMultiInstance();
            container.Register<IEqualizerPresetDetailsPresenter, EqualizerPresetDetailsPresenter>().AsMultiInstance();
            container.Register<IPreferencesPresenter, PreferencesPresenter>().AsMultiInstance();
            container.Register<IAudioPreferencesPresenter, AudioPreferencesPresenter>().AsMultiInstance();
            container.Register<ICloudPreferencesPresenter, CloudPreferencesPresenter>().AsMultiInstance();
            container.Register<IGeneralPreferencesPresenter, GeneralPreferencesPresenter>().AsMultiInstance();
            container.Register<ILibraryPreferencesPresenter, LibraryPreferencesPresenter>().AsMultiInstance();
            container.Register<ICloudConnectPresenter, CloudConnectPresenter>().AsMultiInstance();
            container.Register<IPlaylistPresenter, PlaylistPresenter>().AsMultiInstance();
            container.Register<ILoopsPresenter, LoopsPresenter>().AsMultiInstance();
            container.Register<ILoopDetailsPresenter, LoopDetailsPresenter>().AsMultiInstance();
            container.Register<ISegmentDetailsPresenter, SegmentDetailsPresenter>().AsMultiInstance();
            container.Register<IMarkersPresenter, MarkersPresenter>().AsMultiInstance();
            container.Register<IMarkerDetailsPresenter, MarkerDetailsPresenter>().AsMultiInstance();
            container.Register<ITimeShiftingPresenter, TimeShiftingPresenter>().AsMultiInstance();
            container.Register<IPitchShiftingPresenter, PitchShiftingPresenter>().AsMultiInstance();
            container.Register<IPlayerMetadataPresenter, PlayerMetadataPresenter>().AsMultiInstance();
            container.Register<IPlayerStatusPresenter, PlayerStatusPresenter>().AsMultiInstance();
            container.Register<ISyncPresenter, SyncPresenter>().AsMultiInstance();
            container.Register<ISyncConnectManualPresenter, SyncConnectManualPresenter>().AsMultiInstance();
            container.Register<ISyncCloudPresenter, SyncCloudPresenter>().AsMultiInstance();
            container.Register<ISyncWebBrowserPresenter, SyncWebBrowserPresenter>().AsMultiInstance();
            container.Register<ISyncMenuPresenter, SyncMenuPresenter>().AsMultiInstance();
            container.Register<ISyncDownloadPresenter, SyncDownloadPresenter>().AsMultiInstance();
            container.Register<IEditSongMetadataPresenter, EditSongMetadataPresenter>().AsSingleton();
            container.Register<IMobileFirstRunPresenter, MobileFirstRunPresenter>().AsSingleton();
            container.Register<IAboutPresenter, AboutPresenter>().AsSingleton();
            container.Register<ISelectFoldersPresenter, SelectFoldersPresenter>().AsMultiInstance();
            container.Register<ISelectPlaylistPresenter, SelectPlaylistPresenter>().AsMultiInstance();
            container.Register<IAddPlaylistPresenter, AddPlaylistPresenter>().AsMultiInstance();
            container.Register<IAddMarkerPresenter, AddMarkerPresenter>().AsMultiInstance();
            container.Register<IResumePlaybackPresenter, ResumePlaybackPresenter>().AsMultiInstance();
            container.Register<IStartResumePlaybackPresenter, StartResumePlaybackPresenter>().AsMultiInstance();
            container.Register<IFirstRunPresenter, FirstRunPresenter>().AsMultiInstance();
            container.Register<IMobileMainPresenter, MobileMainPresenter>().AsMultiInstance();       
            container.Register<IQueuePresenter, QueuePresenter>().AsMultiInstance();
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

