//
// frmEditSongMetadata.cs: Edit Song Metadata window. This is where the user can modify the ID3 and other
//                         tags for the media files.
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
using System.Text;
using System.Windows.Forms;
using MPfm.Sound;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Edit Song Metadata window. This is where the user can modify the ID3 and other
    /// tags for the media files.
    /// </summary>
    public partial class frmEditSongMetadata : MPfm.WindowsControls.Form
    {
        // Private variables
        private frmMain main = null;
        private List<string> filePaths = null;
        private AudioFile audioFile = null;

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
        /// Constructor for Edit Song Metadata window. Requires a hook to the main form and
        /// the file path(s) to the files to edit.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        /// <param name="filePaths">List of file paths</param>
        public frmEditSongMetadata(frmMain main, List<string> filePaths)
        {
            InitializeComponent();
            this.main = main;
            this.filePaths = filePaths;

            // Get TagLib information about the file
            if (filePaths.Count > 0)
            {
                // Get TagLib information                
                audioFile = new AudioFile(filePaths[0]);

                // Update property grid
                propertyGridTags.SelectedObject = audioFile;
                lblEditing.Text = "Editing " + filePaths[0];
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Close" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Hide the form
            this.Close();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Check if the player is playing
            if (Main.Player.IsPlaying)
            {
                // Check if the file is currently playing
                if (Main.Player.Playlist.CurrentItem.AudioFile.FilePath == audioFile.FilePath)
                {
                    // Warn user that this will stop playback.
                    if (MessageBox.Show("This audio file is currently playing. Do you wish to stop the playback to save this audio file metadata?", "Must stop playback to save audio file metadata", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
                    {
                        // Stop playback
                        Main.Stop();
                    }
                    else
                    {
                        // Cancel operation
                        return;
                    }
                }
            }

            // Save metadata
            audioFile.SaveMetadata();
        }
    }
}