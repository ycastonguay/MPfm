//
// Program.cs: Main program class.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MPfm
{
    /// <summary>
    /// Main program class.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Check if an instance of the application is already running
                string proc = Process.GetCurrentProcess().ProcessName;
                Process[] processes = Process.GetProcessesByName(proc);

                // If the number of processes is greater or equal to 2 (one instance already running + this new instance)
                if (processes.Length >= 2)
                {
                    // Ask the user if it allows another instance of the application
                    if (MessageBox.Show("At least one other instance of MPfm is already running.\n\nClick OK to continue or Cancel to exit the application.", "Multiple instances of MPfm running", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    {
                        // The user wants to exit the application
                        return;
                    }                    
                }
            }
            catch
            {
                throw;
            }

            // Set application defaults
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show splash screen (in a different thread)
            frmSplash.ShowSplash();

            // Start the main window
            Application.Run(new frmMain());

            // Last code to be run when application exits here.
        }
    }
}