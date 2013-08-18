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
using System.Diagnostics;
using System.Windows.Forms;
using MPfm.Library;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Windows.Classes.Forms;
using MPfm.Windows.Classes.Navigation;
using MPfm.Windows.Classes.Specifications;

namespace MPfm.Windows.Classes
{
    /// <summary>
    /// Main program class.
    /// </summary>
    static class Program
    {
        private static NavigationManager _navigationManager;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Check if an instance of the application is already running
            string proc = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(proc);
            
            // Ask the user if it allows another instance of the application
            if (processes.Length >= 2)
                if (MessageBox.Show("At least one other instance of MPfm is already running.\n\nClick OK to continue or Cancel to exit the application.", "Multiple instances of MPfm running", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    return;

            // Set application defaults
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Finish IoC registration
            Bootstrapper.GetContainer().Register<ISyncDeviceSpecifications, WindowsSyncDeviceSpecifications>().AsSingleton();
            Bootstrapper.GetContainer().Register<NavigationManager, WindowsNavigationManager>().AsSingleton();
            Bootstrapper.GetContainer().Register<ISplashView, frmSplash>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IMainView, frmMain>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IUpdateLibraryView, frmUpdateLibrary>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IPlaylistView, frmPlaylist>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IDesktopEffectsView, frmEffects>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<IDesktopPreferencesView, frmPreferences>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncView, frmSync>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncMenuView, frmSyncMenu>().AsMultiInstance();
            Bootstrapper.GetContainer().Register<ISyncDownloadView, frmSyncDownload>().AsMultiInstance();

            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
            _navigationManager.CreateSplashView();
            Application.Run();
        }
    }
}
