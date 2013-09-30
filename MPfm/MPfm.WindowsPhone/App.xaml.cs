﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MPfm.Library;
using MPfm.Library.Database;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Helpers;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.WindowsPhone.Classes;
using MPfm.WindowsPhone.Classes.Navigation;
using MPfm.WindowsPhone.Classes.Pages;
using MPfm.WindowsPhone.Resources;
using TinyIoC;

namespace MPfm.WindowsPhone
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();
            InitializePhoneApplication();
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //var stuff = new DatabaseFacade("");
            //string path = ConfigurationHelper.DatabaseFilePath;
            BootstrapApp();

            // Instead of using WMAppManifest to automatically launch a page, do it in code.
            RootFrame.Navigate(new Uri("/Classes/Pages/MainPage.xaml", UriKind.Relative));
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;

            // Set theme colors
            (Resources["PhoneAccentBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 231, 76, 60);
            (Resources["PhoneForegroundBrush"] as SolidColorBrush).Color = Colors.White;
            (Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 32, 40, 46);
            SetApplicationTile();
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        private void BootstrapApp()
        {
            TinyIoCContainer container = Bootstrapper.GetContainer();
            container.Register<ISyncDeviceSpecifications, WindowsPhoneSyncDeviceSpecifications>().AsSingleton();
            container.Register<MobileNavigationManager, WindowsPhoneNavigationManager>().AsSingleton();
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
            //container.Register<ISyncDownloadView, SyncDownloadActivity>().AsMultiInstance();
            //container.Register<ISyncMenuView, SyncMenuActivity>().AsMultiInstance();
            container.Register<ISyncWebBrowserView, SyncWebBrowserPage>().AsMultiInstance();
            //container.Register<IEqualizerPresetsView, EqualizerPresetsActivity>().AsMultiInstance();
            //container.Register<IEqualizerPresetDetailsView, EqualizerPresetDetailsActivity>().AsMultiInstance();
            container.Register<IPreferencesView, PreferencesPage>().AsMultiInstance();
            //container.Register<IAudioPreferencesView, AudioPreferencesFragment>().AsMultiInstance();
            //container.Register<IGeneralPreferencesView, GeneralPreferencesFragment>().AsMultiInstance();
            //container.Register<ILibraryPreferencesView, LibraryPreferencesFragment>().AsMultiInstance();
            //container.Register<IAboutView, AboutActivity>().AsMultiInstance();            
        }

        private void SetApplicationTile()
        {
            //// Application Tile is always the first Tile, even if it is not pinned to Start.
            //ShellTile TileToFind = ShellTile.ActiveTiles.First();

            //// Application should always be found
            //if (TileToFind != null)
            //{
            //    IconicTileData TileData = new IconicTileData()
            //    {
            //        Title = "My App title",
            //        WideContent1 = "New Wide Content 1",
            //        WideContent2 = "New Wide Content 2",
            //        WideContent3 = "New Wide Content 3",
            //        //Count = 2,
            //        //BackgroundColor = Colors.Blue, 
            //        //BackgroundColor = new Color { A = 255, R = 200, G = 148, B = 255 },
            //        //BackgroundColor = Color.FromArgb(255, 200, 148, 55),
            //        //BackgroundColor = (Color)Application.Current.Resources["PhoneAccentColor"],
            //        BackgroundColor = Color.FromArgb(255, 200, 148, 55), //Colors.Blue, // HexToColor("#FF7A3B3F"),
            //        IconImage = new Uri("Assets/Tiles/IconicTileMediumLarge.png", UriKind.Relative),
            //        SmallIconImage = new Uri("Assets/Tiles/IconicTileSmall.png", UriKind.Relative),
            //    };

            //    // Update the Application Tile
            //    TileToFind.Update(TileData);
            //}
        }
    }
}