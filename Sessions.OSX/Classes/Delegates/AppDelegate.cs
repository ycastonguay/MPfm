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

using Sessions.Library;
using Sessions.Library.Services;
using Sessions.Library.Services.Interfaces;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Sessions.OSX.Classes.Navigation;
using Sessions.MVP.Config.Providers;
using Sessions.GenericControls.Graphics;
using Sessions.OSX.Classes.Controls.Graphics;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services;

namespace Sessions.OSX.Classes.Delegates
{
    /// <summary>
    /// App delegate. Uses TinyIoC to create the MainWindow.
    /// </summary>
	public partial class AppDelegate : NSApplicationDelegate
	{
        NavigationManager _navigationManager;
		
		public AppDelegate()
		{
		}

		public override void FinishedLaunching(NSObject notification)
		{
            Bootstrapper.GetContainer().Register<ISyncDeviceSpecifications, MacSyncDeviceSpecifications>().AsSingleton();   
            Bootstrapper.GetContainer().Register<IAppConfigProvider, XmlAppConfigProvider>().AsSingleton();
            Bootstrapper.GetContainer().Register<IMemoryGraphicsContextFactory, MemoryGraphicsContextFactory>().AsSingleton();
            Bootstrapper.GetContainer().Register<IDisposableImageFactory, DisposableImageFactory>().AsSingleton();
            Bootstrapper.GetContainer().Register<ICloudService, DropboxCoreService>().AsSingleton();   

            Bootstrapper.GetContainer().Register<ITileCacheService, TileCacheService>().AsSingleton();   
            Bootstrapper.GetContainer().Register<IWaveFormEngineService, WaveFormEngineService>().AsSingleton();   
            Bootstrapper.GetContainer().Register<IWaveFormRenderingService, WaveFormRenderingService>().AsSingleton();   
            Bootstrapper.GetContainer().Register<IWaveFormRequestService, WaveFormRequestService>().AsSingleton();   

            Bootstrapper.GetContainer().Register<NavigationManager, MacNavigationManager>().AsSingleton();
            Bootstrapper.GetContainer().Register<ISplashView, SplashWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IMainView, MainWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IUpdateLibraryView, UpdateLibraryWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IPlaylistView, PlaylistWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ICloudConnectView, CloudConnectWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IDesktopEffectsView, EffectsWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IDesktopPreferencesView, PreferencesWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IStartResumePlaybackView, StartResumePlaybackWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ILibraryBrowserView, MainWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISongBrowserView, MainWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IEditSongMetadataView, EditSongMetadataWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ILoopsView, MainWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ILoopDetailsView, MainWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISegmentDetailsView, MainWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IMarkersView, MainWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IMarkerDetailsView, MainWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncView, SyncWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncMenuView, SyncMenuWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncDownloadView, SyncDownloadWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncCloudView, SyncCloudWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncWebBrowserView, SyncWebBrowserWindowController>().AsMultiInstance();

            // Create and start navigation manager
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
            _navigationManager.CreateSplashView();
        }
	}
}
