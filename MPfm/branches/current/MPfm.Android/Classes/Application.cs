using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using MPfm.Android.Classes.Fragments;
using MPfm.MVP;
using MPfm.MVP.Bootstrapper;
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
            container.Register<NavigationManager, AndroidNavigationManager>().AsSingleton();
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