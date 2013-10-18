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
using System.IO;
using System.Reflection;
using Gtk;
using MPfm.MVP;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Services;
using MPfm.MVP.Views;
using MPfm.GTK.Navigation;
using MPfm.GTK.Windows;
using MPfm.Library;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Config.Providers;

namespace MPfm.GTK.Classes
{
	public class MainClass
	{
		static NavigationManager navigationManager;
		
		public static void Main(string[] args)
        {
            // Add view implementations to IoC
            Application.Init();
            Bootstrapper.GetContainer().Register<NavigationManager, GtkNavigationManager>().AsSingleton();
            Bootstrapper.GetContainer().Register<ISyncDeviceSpecifications, LinuxSyncDeviceSpecifications>().AsSingleton();
            Bootstrapper.GetContainer().Register<IAppConfigProvider, XmlAppConfigProvider>().AsSingleton();
            Bootstrapper.GetContainer().Register<IDropboxService, MPfm.Library.Services.DropboxCoreService>().AsSingleton();
			Bootstrapper.GetContainer().Register<ISplashView, SplashWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IMainView, MainWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IUpdateLibraryView, UpdateLibraryWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IDesktopPreferencesView, PreferencesWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IDesktopEffectsView, EffectsWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IPlaylistView, PlaylistWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncView, SyncWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncMenuView, SyncMenuWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncDownloadView, SyncDownloadWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncCloudView, SyncCloudWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncWebBrowserView, SyncWebBrowserWindow>().AsMultiInstance();
			
			// Create and start navigation manager
			navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
			navigationManager.CreateSplashView();
			Application.Run();
		}
	}
}
