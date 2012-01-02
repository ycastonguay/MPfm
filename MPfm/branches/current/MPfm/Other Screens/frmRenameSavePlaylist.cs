//
// frmRenameSavePlaylist.cs: Popup window displayed in modal when the user clicked on the
//                           Save Playlist or Rename Playlist buttons in the Playlist window.
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
using MPfm.Library;

namespace MPfm
{
    /// <summary>
    /// Popup window displayed in modal when the user clicked on the
    /// Save Playlist or Rename Playlist buttons in the Playlist window.
    /// </summary>
    public partial class frmRenameSavePlaylist : MPfm.WindowsControls.Form
    {
        // Private variables
        private RenameSavePlaylistWindowMode mode;
        
        private frmMain m_main = null;
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
        /// Constructor for this class. Needs a hook to the main window.
        /// The user must also specify the window mode (RenamePlaylist, SavePlaylist)
        /// </summary>
        /// <param name="main">Hook to the Main Window instance</param>
        /// <param name="mode">Window Mode (RenamePlaylist, SavePlaylist)</param>
        public frmRenameSavePlaylist(frmMain main, RenameSavePlaylistWindowMode mode)
        {
            InitializeComponent();
            this.m_main = main;
            this.mode = mode;

            // Refresh controls based on window mode
            if (mode == RenameSavePlaylistWindowMode.RenamePlaylist)
            {
                Text = "Rename playlist";
            }
            else if (mode == RenameSavePlaylistWindowMode.SavePlaylist)
            {
                Text = "Save playlist as";
            }
        }

        #region Control Events

        /// <summary>
        /// Occurs when the user clicks on the OK button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            //// Check if name is already taken
            //if (DataAccess.PlaylistExists(txtName.Text))
            //{
            //    // Show message box and cancel the form close
            //    MessageBox.Show("A playlist named \"" + txtName.Text + "\" already exists in the database!\nPlease select another name.", "Playlist already exists in database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //// Check window mode
            //if (mode == RenameSavePlaylistWindowMode.SavePlaylist)
            //{
            //    // Rename playlist and reset its id (save AS)
            //    Main.Player.Playlist.Name = txtName.Text;                
            //}
            //else if (mode == RenameSavePlaylistWindowMode.RenamePlaylist)
            //{
            //    // Rename playlist
            //    Main.Player.Playlist.Name = txtName.Text;
            //}

            // Set result
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            // Refresh the window title
            Main.formPlaylist.RefreshTitle();

            // Close form
            this.Close();          
        }

        /// <summary>
        /// Occurs when the user clicks on the Cancel button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Set result
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            // Close form
            this.Close();
        }

        /// <summary>
        /// Occurs when the user types something into the txtName textbox.
        /// Disables the OK button if the name is empty.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // Check if name is empty
            if (txtName.Text.Length > 0)
            {
                btnOK.Enabled = true;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the mode for the RenameSavePlaylist form.
    /// </summary>
    public enum RenameSavePlaylistWindowMode
    {
        RenamePlaylist = 0, SavePlaylist = 1
    }
}