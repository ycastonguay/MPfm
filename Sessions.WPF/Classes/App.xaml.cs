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

using System.Diagnostics;
using System.Windows;
using Sessions.WPF.Classes.Controls.Graphics;
using Sessions.WPF.Classes.Navigation;
using Sessions.WPF.Classes.Specifications;
using Sessions.WPF.Classes.Windows;
using Sessions.Core;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Services;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.Library;
using Sessions.Library.Services;
using Sessions.Library.Services.Interfaces;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Config;
using Sessions.MVP.Config.Providers;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;

namespace Sessions.WPF.Classes
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
            Bootstrapper.GetContainer().Register<IDisposableImageFactory, DisposableImageFactory>().AsSingleton();
            Bootstrapper.GetContainer().Register<ICloudService, DropboxCoreService>().AsSingleton();
            Bootstrapper.GetContainer().Register<IWaveFormEngineService, WaveFormEngineService>().AsSingleton();
            Bootstrapper.GetContainer().Register<IWaveFormRenderingService, WaveFormRenderingService>().AsSingleton();
            Bootstrapper.GetContainer().Register<IWaveFormRequestService, WaveFormRequestService>().AsSingleton();
            Bootstrapper.GetContainer().Register<ITileCacheService, TileCacheService>().AsSingleton();
            Bootstrapper.GetContainer().Register<IAppConfigProvider, XmlAppConfigProvider>().AsSingleton();
            Bootstrapper.GetContainer().Register<NavigationManager, WindowsNavigationManager>().AsSingleton();
            Bootstrapper.GetContainer().Register<ISplashView, SplashWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IMainView, MainWindow>().AsMultiInstance();
            //Bootstrapper.GetContainer().Register<ILoopDetailsView, frmLoopDetails>().AsMultiInstance();
            //Bootstrapper.GetContainer().Register<IMarkerDetailsView, frmMarkerDetails>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IEditSongMetadataView, EditSongMetadataWindow>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IAboutView, AboutWindow>().AsMultiInstance();
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
