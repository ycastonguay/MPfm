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
using Android.Content.PM;
using Android.Runtime;
using MPfm.Android.Classes.Fragments;
using MPfm.MVP;
using MPfm.MVP.Bootstrapper;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player;
using MPfm.Sound.Bass.Net;

namespace MPfm.Android.Classes
{
    [Application (Name="my.App", Debuggable=true, Label="MPfm: Music Player for Musicians")]
    public class MPfmApplication : Application
    {
        private IPlayer _player;

        public MPfmApplication(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer) 
        { 
        }

        public override void OnCreate()
        {
            base.OnCreate();

            // Complete IoC configuration
            TinyIoC.TinyIoCContainer container = Bootstrapper.GetContainer();
            container.Register<MobileNavigationManager, AndroidNavigationManager>().AsSingleton();
            container.Register<ISplashView, SplashFragment>().AsMultiInstance();
            container.Register<IUpdateLibraryView, UpdateLibraryFragment>().AsMultiInstance();
            container.Register<IMobileLibraryBrowserView, MobileLibraryBrowserFragment>().AsMultiInstance();
            container.Register<IAudioPreferencesView, AudioPreferencesFragment>().AsMultiInstance();
            container.Register<IGeneralPreferencesView, GeneralPreferencesFragment>().AsMultiInstance();
            container.Register<ILibraryPreferencesView, LibraryPreferencesFragment>().AsMultiInstance();

            // Set player plugin directory path
            ApplicationInfo appInfo = PackageManager.GetApplicationInfo(PackageName, 0);
            string nativeDir = appInfo.NativeLibraryDir;
            Player.Player.PluginDirectoryPath = appInfo.NativeLibraryDir;

            // Initialize player
            Device device = new Device()
            {
                DriverType = DriverType.DirectSound,
                Id = -1
            };
            _player = new MPfm.Player.Player(device, 44100, 5000, 100, true);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();

            // Clean up player
            if (MPfm.Player.Player.CurrentPlayer.IsPlaying)
            {
                MPfm.Player.Player.CurrentPlayer.Stop();
            }
            MPfm.Player.Player.CurrentPlayer.Dispose();
        }
    }
}