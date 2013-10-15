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
using MPfm.Library;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.WindowsStore.Classes;
using MPfm.WindowsStore.Classes.Navigation;
using MPfm.WindowsStore.Classes.Pages;
using MPfm.WindowsStore.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TinyIoC;

// The Split App template is documented at http://go.microsoft.com/fwlink/?LinkId=234228

namespace MPfm.WindowsStore
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private WindowsStoreNavigationManager _navigationManager;

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Debug.WriteLine("App - Ctor");
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Debug.WriteLine("App - OnLaunched");
            //Frame rootFrame = Window.Current.Content as Frame;

            //// Do not repeat app initialization when the Window already has content,
            //// just ensure that the window is active
            //if (rootFrame == null)
            //{
            //    BootstrapApp();

            //    // Create a Frame to act as the navigation context and navigate to the first page
            //    rootFrame = new Frame();
            //    // Associate the frame with a SuspensionManager key                                
            //    SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

            //    if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            //    {
            //        // Restore the saved session state only when appropriate
            //        try
            //        {
            //            Debug.WriteLine("App - SuspensionManager.RestoreAsync");
            //            await SuspensionManager.RestoreAsync();
            //        }
            //        catch (SuspensionManagerException)
            //        {
            //            //Something went wrong restoring state.
            //            //Assume there is no state and continue
            //        }
            //    }

            //    // Place the frame in the current Window
            //    Window.Current.Content = rootFrame;
            //}
            //if (rootFrame.Content == null)
            //{
            //    // When the navigation stack isn't restored navigate to the first page,
            //    // configuring the new page by passing required information as a navigation
            //    // parameter
            //    Debug.WriteLine("RootFrame.Content == null; trying to navigate to MainPage");
            //    if(!rootFrame.Navigate(typeof(MainPage), "AllGroups"))
            //    {
            //        throw new Exception("Failed to create initial page");
            //    }
            //}
            // Ensure the current window is active
            //Window.Current.Activate();

            BootstrapApp();
            _navigationManager = (WindowsStoreNavigationManager)Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            _navigationManager.SplashImageLocation = args.SplashScreen.ImageLocation;
            //_navigationManager.BindOptionsMenuView(this);
            _navigationManager.Start();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            Debug.WriteLine("App - OnSuspending");
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        private void BootstrapApp()
        {
            TinyIoCContainer container = Bootstrapper.GetContainer();
            container.Register<ISyncDeviceSpecifications, WindowsStoreSyncDeviceSpecifications>().AsSingleton();
            container.Register<MobileNavigationManager, WindowsStoreNavigationManager>().AsSingleton();
            container.Register<IMobileOptionsMenuView, MainPage>().AsMultiInstance();
            container.Register<ISplashView, SplashPage>().AsMultiInstance();
            //container.Register<IPlayerView, PlayerActivity>().AsMultiInstance();
            //container.Register<IPlayerMetadataView, PlayerMetadataFragment>().AsMultiInstance();
            //container.Register<IMarkersView, MarkersFragment>().AsMultiInstance();
            //container.Register<IMarkerDetailsView, MarkerDetailsActivity>().AsMultiInstance();
            //container.Register<ILoopsView, LoopsFragment>().AsMultiInstance();
            //container.Register<ITimeShiftingView, TimeShiftingFragment>().AsMultiInstance();
            //container.Register<IPitchShiftingView, PitchShiftingFragment>().AsMultiInstance();
            //container.Register<IUpdateLibraryView, UpdateLibraryFragment>().AsMultiInstance();
            //container.Register<IMobileLibraryBrowserView, MobileLibraryBrowserFragment>().AsMultiInstance();
            //container.Register<IPlaylistView, PlaylistActivity>().AsMultiInstance();
            container.Register<ISyncView, SyncPage>().AsMultiInstance();
            container.Register<ISyncDownloadView, SyncDownloadPage>().AsMultiInstance();
            container.Register<ISyncMenuView, SyncMenuPage>().AsMultiInstance();
            //container.Register<ISyncWebBrowserView, SyncWebBrowserActivity>().AsMultiInstance();
            //container.Register<IEqualizerPresetsView, EqualizerPresetsActivity>().AsMultiInstance();
            //container.Register<IEqualizerPresetDetailsView, EqualizerPresetDetailsActivity>().AsMultiInstance();
            //container.Register<IPreferencesView, PreferencesActivity>().AsMultiInstance();
            //container.Register<IAudioPreferencesView, AudioPreferencesFragment>().AsMultiInstance();
            //container.Register<IGeneralPreferencesView, GeneralPreferencesFragment>().AsMultiInstance();
            //container.Register<ILibraryPreferencesView, LibraryPreferencesFragment>().AsMultiInstance();
            //container.Register<IAboutView, AboutActivity>().AsMultiInstance();

        }
    }
}
