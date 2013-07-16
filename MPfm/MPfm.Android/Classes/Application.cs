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
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Navigation;
using MPfm.Library;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Android.Classes.Helpers;

namespace MPfm.Android.Classes
{
#if DEBUG
    [Application(Name = "my.App", Debuggable = true, Label = "Sessions")]
#else
    [Application (Name="my.App", Debuggable=false, Label="Sessions")]
#endif
    public class MPfmApplication : Application
    {
        private static Context _context;

        public MPfmApplication(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer) 
        { 
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _context = ApplicationContext;

            // Complete IoC configuration
            TinyIoC.TinyIoCContainer container = Bootstrapper.GetContainer();
            container.Register<ISyncDeviceSpecifications, AndroidSyncDeviceSpecifications>().AsSingleton();
            container.Register<MobileNavigationManager, AndroidNavigationManager>().AsSingleton();
            container.Register<IMobileOptionsMenuView, MainActivity>().AsMultiInstance();
            container.Register<ISplashView, SplashFragment>().AsMultiInstance();
            container.Register<IPlayerView, PlayerActivity>().AsMultiInstance();
            container.Register<IPlayerMetadataView, PlayerMetadataFragment>().AsMultiInstance();
            container.Register<IMarkersView, MarkersFragment>().AsMultiInstance();
            container.Register<IMarkerDetailsView, MarkerDetailsActivity>().AsMultiInstance();
            container.Register<ILoopsView, LoopsFragment>().AsMultiInstance();
            container.Register<ITimeShiftingView, TimeShiftingFragment>().AsMultiInstance();
            container.Register<IPitchShiftingView, PitchShiftingFragment>().AsMultiInstance();
            container.Register<IUpdateLibraryView, UpdateLibraryFragment>().AsMultiInstance();
            container.Register<IMobileLibraryBrowserView, MobileLibraryBrowserFragment>().AsMultiInstance();
            container.Register<ISyncView, SyncActivity>().AsMultiInstance();
            container.Register<ISyncDownloadView, SyncDownloadActivity>().AsMultiInstance();
            container.Register<ISyncMenuView, SyncMenuActivity>().AsMultiInstance();
            container.Register<ISyncWebBrowserView, SyncWebBrowserActivity>().AsMultiInstance();
            container.Register<IEqualizerPresetsView, EqualizerPresetsActivity>().AsMultiInstance();
            container.Register<IEqualizerPresetDetailsView, EqualizerPresetDetailsActivity>().AsMultiInstance();
            container.Register<IPreferencesView, PreferencesActivity>().AsMultiInstance();
            container.Register<IAudioPreferencesView, AudioPreferencesFragment>().AsMultiInstance();
            container.Register<IGeneralPreferencesView, GeneralPreferencesFragment>().AsMultiInstance();
            container.Register<ILibraryPreferencesView, LibraryPreferencesFragment>().AsMultiInstance();

            // Set player plugin directory path
            ApplicationInfo appInfo = PackageManager.GetApplicationInfo(PackageName, 0);
            Player.Player.PluginDirectoryPath = appInfo.NativeLibraryDir;
        }

        public override void OnTerminate()
        {
            base.OnTerminate();

            // Stop and clean up player
            if (MPfm.Player.Player.CurrentPlayer.IsPlaying)
                MPfm.Player.Player.CurrentPlayer.Stop();
            MPfm.Player.Player.CurrentPlayer.Dispose();
        }

        public static Context GetApplicationContext()
        {
            return _context;
        }
    }
}
