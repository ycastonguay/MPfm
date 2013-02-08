//
// frmVisualizer.cs: Visualizer window. This is where the user can visualize effects such as
//                   wave forms, spectrum analyzers, and more. 
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Sound;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Visualizer window. This is where the user can visualize effects such as
    /// wave forms, spectrum analyzers, and more.
    /// </summary>
    public partial class frmVisualizer : MPfm.WindowsControls.Form
    {
        private frmMain main = null;
        /// <summary>
        /// Hook to the main form.
        /// </summary>
        public frmMain Main
        {
            get
            {
                return main;
            }
        }

        /// <summary>
        /// Constructor for the Visualizer form. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmVisualizer(frmMain main)
        {            
            InitializeComponent();
            this.main = main;
        }

        /// <summary>
        /// Occurs when the user tries to close the form.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmVisualizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                this.Hide();
                Main.btnVisualizer.Checked = false;
            }
        }

        /// <summary>
        /// Occurs when the timer updates. This refreshes the wave form control.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            //// Make sure the player is valid and something plays
            //if (Main.Player != null && Main.Player.IsPlaying)
            //{
            //    waveForm.Refresh();
            //}
        }
    }
}
