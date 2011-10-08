//
// frmEditSongMetadata.cs: Edit Song Metadata window. This is where the user can modify the ID3 and other
//                         tags for the media files.
//
// Copyright © 2011 Yanick Castonguay
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
using System.Text;
using System.Windows.Forms;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Edit Song Metadata window. This is where the user can modify the ID3 and other
    /// tags for the media files.
    /// </summary>
    public partial class frmAddEditMarker : MPfm.WindowsControls.Form
    {
        // Private variables
        private AddEditMarkerWindowMode m_mode = AddEditMarkerWindowMode.Add;
        private frmMain m_main = null;
        private List<string> m_filePaths = null;

        /// <summary>
        /// Hook to the main form.
        /// </summary>
        public frmMain Main
        {
            get
            {
                return m_main;
            }
        }

        /// <summary>
        /// Constructor for Edit Song Metadata window. Requires a hook to the main form and
        /// the window mode must be specified.
        /// </summary>
        /// <param name="main">Hook to the main window</param>
        /// <param name="mode">Window mode</param>
        public frmAddEditMarker(frmMain main, AddEditMarkerWindowMode mode)
        {
            InitializeComponent();
            m_main = main;

            // Set labels depending on mode
            if (mode == AddEditMarkerWindowMode.Add)
            {
                panelEditMarker.HeaderTitle = "Add marker";
                Text = "Add marker";
            }
            else if (mode == AddEditMarkerWindowMode.Edit)
            {
                panelEditMarker.HeaderTitle = "Edit marker";
                Text = "Edit marker";
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Close" button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Hide the form
            this.Close();
        }
        /// <summary>
        /// Occurs when the user clicks on the "Save" button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
        }
    }

    /// <summary>
    /// Defines the mode of the AddEditMarker window.
    /// </summary>
    public enum AddEditMarkerWindowMode
    {
        Add = 0, Edit = 1
    }
}