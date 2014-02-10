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
using System.Drawing;
using MPfm.Library;
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreFoundation;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using TinyIoC;
using MPfm.Mac.Classes.Navigation;
using MPfm.MVP.Config.Providers;

namespace MPfm.Mac.Classes.Delegates
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
            Bootstrapper.GetContainer().Register<IAppConfigProvider, XmlAppConfigProvider>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ICloudService, DropboxCoreService>().AsSingleton();   
            Bootstrapper.GetContainer().Register<NavigationManager, MacNavigationManager>().AsSingleton();
            Bootstrapper.GetContainer().Register<ISplashView, SplashWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IMainView, MainWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IUpdateLibraryView, UpdateLibraryWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IPlaylistView, PlaylistWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IDesktopEffectsView, EffectsWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IDesktopPreferencesView, PreferencesWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncView, SyncWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncMenuView, SyncMenuWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncDownloadView, SyncDownloadWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncCloudView, SyncCloudWindowController>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncWebBrowserView, SyncWebBrowserWindowController>().AsMultiInstance();

            // Create and start navigation manager
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
            //navigationManager.Start();
            _navigationManager.CreateSplashView();
        }
	}
}
