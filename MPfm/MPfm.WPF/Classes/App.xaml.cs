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

using System.Diagnostics;
using System.Windows;
using MPfm.Core;
using MPfm.GenericControls.Graphics;
using MPfm.Library;
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Config;
using MPfm.MVP.Config.Providers;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.WPF.Classes.Controls.Graphics;
using MPfm.WPF.Classes.Navigation;
using MPfm.WPF.Classes.Specifications;
using MPfm.WPF.Classes.Windows;

namespace MPfm.WPF.Classes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NavigationManager _navigationManager;

        //protected override void OnStartup(StartupEventArgs e)
        protected void App_Startup(object sender, StartupEventArgs e)
        {
            Tracing.Log("App - OnStartup");
            CheckForOtherInstances();
            RegisterIoC();

            AppConfigManager.Instance.Load();
            Tracing.Log("App - OnStartup - isFirstRun: {0}", AppConfigManager.Instance.Root.IsFirstRun);
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();            
            if (AppConfigManager.Instance.Root.IsFirstRun)
                _navigationManager.CreateFirstRunView();
            else
                _navigationManager.CreateSplashView();
        }

        private void CheckForOtherInstances()
        {
            // Check if an instance of the application is already running
            string proc = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(proc);

            // Ask the user if it allows another instance of the application
            if (processes.Length >= 2)
                if (MessageBox.Show("At least one other instance of Sessions is already running.\n\nClick OK to continue or Cancel to exit the application.", "Multiple instances of Sessions running", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    return;
        }

        private void RegisterIoC()
        {
            // Finish IoC registration
            Bootstrapper.GetContainer().Register<ISyncDeviceSpecifications, WindowsSyncDeviceSpecifications>().AsSingleton();
            Bootstrapper.GetContainer().Register<IMemoryGraphicsContextFactory, MemoryGraphicsContextFactory>().AsSingleton();
            Bootstrapper.GetContainer().Register<ICloudService, DropboxCoreService>().AsSingleton();            
            Bootstrapper.GetContainer().Register<IAppConfigProvider, XmlAppConfigProvider>().AsSingleton();
            Bootstrapper.GetContainer().Register<NavigationManager, WindowsNavigationManager>().AsSingleton();
            Bootstrapper.GetContainer().Register<ISplashView, SplashWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IMainView, MainWindow>().AsMultiInstance();
            //Bootstrapper.GetContainer().Register<ILoopDetailsView, frmLoopDetails>().AsMultiInstance();
            //Bootstrapper.GetContainer().Register<IMarkerDetailsView, frmMarkerDetails>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IEditSongMetadataView, EditSongMetadataWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IFirstRunView, FirstRunWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IUpdateLibraryView, UpdateLibraryWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IPlaylistView, PlaylistWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IDesktopEffectsView, EffectsWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IDesktopPreferencesView, PreferencesWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ICloudConnectView, CloudConnectWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncView, SyncWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncCloudView, SyncCloudWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncMenuView, SyncMenuWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncDownloadView, SyncDownloadWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncWebBrowserView, SyncWebBrowserWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IResumePlaybackView, ResumePlaybackWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IStartResumePlaybackView, StartResumePlaybackWindow>().AsMultiInstance();
        }
    }
}
