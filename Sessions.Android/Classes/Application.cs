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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using MPfm.Android.Classes.Controls.Graphics;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Navigation;
using MPfm.Android.Classes.Providers;
using MPfm.Android.Classes.Receivers;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Services;
using MPfm.GenericControls.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Config.Providers;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Android.Classes.Services;
using org.sessionsapp.android;
using Sessions.Library;
using Sessions.Library.Services.Interfaces;
using Sessions.Player;

namespace MPfm.Android.Classes
{
#if DEBUG
    [Application(Name = "my.App", Debuggable = true, Label = "Sessions")]
#else
    [Application (Name="my.App", Debuggable=false, Label="Sessions")]
#endif
    public class MPfmApplication : Application
    {
        static Context _context;
        ConnectionChangeReceiver _connectionChangeReceiver;
        private LockReceiver _lockReceiver;

        //private IntentFilter _intentFilter;
        //private WifiP2pManager _wifiManager;
        //private WifiP2pManager.Channel _wifiChannel;
        //private WifiDirectReceiver _wifiDirectReceiver;
        //private ActionListener _actionListener;

#if __ANDROID_16__
        private AndroidDiscoveryService _discoveryService;
#endif

        public MPfmApplication(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer) 
        { 
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _context = ApplicationContext; // TODO: Probably creates a memory leak.
            BootstrapApp();

            // Set player plugin directory path
            ApplicationInfo appInfo = PackageManager.GetApplicationInfo(PackageName, 0);
            Sessions.Player.Player.PluginDirectoryPath = appInfo.NativeLibraryDir;

            try
            {
                Console.WriteLine("Application - Registering ConnectionChangeReceiver...");
                _connectionChangeReceiver = new ConnectionChangeReceiver();
                IntentFilter filter = new IntentFilter("android.net.conn.CONNECTIVITY_CHANGE");
                filter.AddCategory(Intent.CategoryDefault);
                RegisterReceiver(_connectionChangeReceiver, filter);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application - Error: Failed to setup connection change receiver! {0}", ex);
            }

            try
            {
                Console.WriteLine("Application - Registering LockReceiver...");
                var intentFilter = new IntentFilter();
                intentFilter.AddAction(Intent.ActionScreenOff);
                intentFilter.AddAction(Intent.ActionScreenOn);
                intentFilter.AddAction(Intent.ActionUserPresent);
                intentFilter.AddCategory(Intent.CategoryDefault);
                _lockReceiver = new LockReceiver();
                RegisterReceiver(_lockReceiver, intentFilter);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application - Error: Failed to setup lock receiver! {0}", ex);
            }

//#if __ANDROID_16__
//            if (((int)global::Android.OS.Build.VERSION.SdkInt) >= 16) {
//                _discoveryService = new AndroidDiscoveryService();
//                _discoveryService.StartDiscovery();
//                _discoveryService.DiscoverPeers();
//            }
//#endif
        }

        public override void OnTerminate()
        {
            base.OnTerminate();

            // Stop and clean up player
            if (Sessions.Player.Player.CurrentPlayer.IsPlaying)
                Sessions.Player.Player.CurrentPlayer.Stop();
            Sessions.Player.Player.CurrentPlayer.Dispose();

//#if __ANDROID_16__
//            if (((int)global::Android.OS.Build.VERSION.SdkInt) >= 16) {
//                _discoveryService.Dispose();
//            }
//#endif
        }

        //private void SetupWifiDirect()
        //{
        //    _intentFilter = new IntentFilter();
        //    _intentFilter.AddAction(WifiP2pManager.WifiP2pStateChangedAction);
        //    _intentFilter.AddAction(WifiP2pManager.WifiP2pPeersChangedAction);
        //    _intentFilter.AddAction(WifiP2pManager.WifiP2pConnectionChangedAction);
        //    _intentFilter.AddAction(WifiP2pManager.WifiP2pThisDeviceChangedAction);

        //    _actionListener = new ActionListener();
        //    _wifiManager = (WifiP2pManager) GetSystemService(Context.WifiP2pService);
        //    _wifiChannel = _wifiManager.Initialize(this, MainLooper, null);
        //    _wifiDirectReceiver = new WifiDirectReceiver();
        //    RegisterReceiver(_wifiDirectReceiver, _intentFilter);

        //    _wifiManager.DiscoverPeers(_wifiChannel, _actionListener);
        //}

        public static Context GetApplicationContext()
        {
            return _context;
        }

        private void BootstrapApp()
        {
            // Complete IoC configuration
            TinyIoC.TinyIoCContainer container = Bootstrapper.GetContainer();
            container.Register<IMemoryGraphicsContextFactory, MemoryGraphicsContextFactory>().AsSingleton();
            container.Register<ISyncDeviceSpecifications, AndroidSyncDeviceSpecifications>().AsSingleton();
            container.Register<ICloudService, AndroidDropboxService>().AsSingleton();
            container.Register<IWaveFormCacheService, WaveFormCacheService>().AsSingleton();
            container.Register<IWaveFormRenderingService, WaveFormRenderingService>().AsSingleton();
            container.Register<IAppConfigProvider, AndroidAppConfigProvider>().AsSingleton();
            container.Register<MobileNavigationManager, AndroidNavigationManager>().AsSingleton();
            container.Register<IMobileOptionsMenuView, MainActivity>().AsMultiInstance();
            container.Register<ISplashView, SplashActivity>().AsMultiInstance();
            container.Register<IMobileMainView, MainActivity>().AsMultiInstance();
            container.Register<IPlayerView, PlayerActivity>().AsMultiInstance();
            container.Register<IPlayerMetadataView, PlayerMetadataFragment>().AsMultiInstance();
            container.Register<IMarkersView, MarkersFragment>().AsMultiInstance();
            container.Register<IMarkerDetailsView, MarkerDetailsActivity>().AsMultiInstance();
            container.Register<ILoopsView, LoopsFragment>().AsMultiInstance();
            container.Register<ITimeShiftingView, TimeShiftingFragment>().AsMultiInstance();
            container.Register<IPitchShiftingView, PitchShiftingFragment>().AsMultiInstance();
            container.Register<IUpdateLibraryView, UpdateLibraryFragment>().AsMultiInstance();
            container.Register<IMobileLibraryBrowserView, MobileLibraryBrowserFragment>().AsMultiInstance();
            container.Register<IPlaylistView, PlaylistActivity>().AsMultiInstance();
            container.Register<ISyncView, SyncActivity>().AsMultiInstance();
            container.Register<ISyncDownloadView, SyncDownloadActivity>().AsMultiInstance();
            container.Register<ISyncMenuView, SyncMenuActivity>().AsMultiInstance();
            container.Register<ISyncWebBrowserView, SyncWebBrowserActivity>().AsMultiInstance();
            container.Register<ISyncCloudView, SyncCloudActivity>().AsMultiInstance();
            container.Register<ICloudConnectView, CloudConnectFragment>().AsMultiInstance();
            container.Register<IEqualizerPresetsView, EqualizerPresetsActivity>().AsMultiInstance();
            container.Register<IEqualizerPresetDetailsView, EqualizerPresetDetailsActivity>().AsMultiInstance();
            container.Register<IPreferencesView, PreferencesActivity>().AsMultiInstance();
            container.Register<IAudioPreferencesView, AudioPreferencesFragment>().AsMultiInstance();
            container.Register<IGeneralPreferencesView, GeneralPreferencesFragment>().AsMultiInstance();
            container.Register<ILibraryPreferencesView, LibraryPreferencesFragment>().AsMultiInstance();
            container.Register<IAboutView, AboutActivity>().AsMultiInstance();
            container.Register<ISelectFoldersView, SelectFoldersFragment>().AsMultiInstance();
            container.Register<ISelectPlaylistView, SelectPlaylistFragment>().AsMultiInstance();
            container.Register<IAddPlaylistView, AddPlaylistFragment>().AsMultiInstance();
            container.Register<IAddMarkerView, AddMarkerFragment>().AsMultiInstance();
            container.Register<IResumePlaybackView, ResumePlaybackActivity>().AsMultiInstance();
            container.Register<IStartResumePlaybackView, StartResumePlaybackFragment>().AsMultiInstance();
            container.Register<IFirstRunView, FirstRunActivity>().AsMultiInstance();
        }
    }
}
