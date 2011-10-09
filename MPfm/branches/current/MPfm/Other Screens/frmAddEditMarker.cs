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
using MPfm.Library;
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
        private SongDTO m_song = null;
        private Guid m_markerId = Guid.Empty;

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
        /// <param name="song">Song linked to the marker</param>
        /// <param name="markerId">Identifier of the marker (if it exists)</param>
        public frmAddEditMarker(frmMain main, AddEditMarkerWindowMode mode, SongDTO song, Guid markerId)
        {
            InitializeComponent();
            m_main = main;
            m_mode = mode;
            m_song = song;
            m_markerId = markerId;

            // Initialize controls
            Initialize();
        }

        /// <summary>
        /// Initializes the controls depending on the window mode.
        /// </summary>
        private void Initialize()
        {
            // Set song labels
            lblArtistName.Text = m_song.ArtistName;
            lblAlbumTitle.Text = m_song.AlbumTitle;
            lblSongTitle.Text = m_song.Title;

            // Set labels depending on mode
            if (m_mode == AddEditMarkerWindowMode.Add)
            {
                panelEditMarker.HeaderTitle = "Add marker";
                Text = "Add marker";
            }
            else if (m_mode == AddEditMarkerWindowMode.Edit)
            {
                panelEditMarker.HeaderTitle = "Edit marker";
                panelEditMarker.Refresh();
                Text = "Edit marker";

                // Fetch marker from database
                MPfm.Library.Data.Marker marker = DataAccess.SelectMarker(m_markerId);

                // Check if the marker was found
                if(marker == null)
                {
                    return;
                }

                // Update fields
                txtName.Text = marker.Name;
                txtPosition.Text = marker.Position;
                lblPositionPCMValue.Text = marker.PositionPCM.ToString();
                lblPositionPCMBytesValue.Text = marker.PositionPCMBytes.ToString();
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
            // Get PCM and PCM bytes values
            long pcm = 0;
            long.TryParse(lblPositionPCMValue.Text, out pcm);
            long pcmBytes = 0;
            long.TryParse(lblPositionPCMBytesValue.Text, out pcmBytes);

            // Create a new marker or fetch the existing marker from the database
            MPfm.Library.Data.Marker marker = null;
            if (m_mode == AddEditMarkerWindowMode.Add)
            {
                // Insert the new marker into the database
                marker = new Library.Data.Marker();
                marker.MarkerId = Guid.NewGuid().ToString();
            }
            else if (m_mode == AddEditMarkerWindowMode.Edit)
            {
                // Select the existing marker from the database
                marker = DataAccess.SelectMarker(m_markerId);
            }

            // Set properties            
            marker.SongId = Main.Player.CurrentSong.SongId.ToString();
            marker.Name = txtName.Text;
            marker.Position = txtPosition.Text;
            marker.PositionPCM = pcm;
            marker.PositionPCMBytes = pcmBytes;

            // Determine if an INSERT or an UPDATE is necessary
            if (m_mode == AddEditMarkerWindowMode.Add)
            {
                // Insert marker
                DataAccess.InsertMarker(marker);

                // Refresh window as Edit Marker
                m_markerId = new Guid(marker.MarkerId);
                m_mode = AddEditMarkerWindowMode.Edit;
                Initialize();
            }
            else if (m_mode == AddEditMarkerWindowMode.Edit)
            {
                // Update marker
                DataAccess.UpdateMarker(marker);
            }

            // Refresh main window marker list
            Main.RefreshMarkers();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Punch in" button.
        /// Sets the marker position to the current playback position.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnPunchIn_Click(object sender, EventArgs e)
        {
            // Check if the player is currently playing
            if (!Main.Player.IsPlaying)
            {
                return;
            }

            // Update controls
            txtPosition.Text = Main.Player.MainChannel.Position;
            lblPositionPCMValue.Text = Main.Player.MainChannel.PositionSentencePCM.ToString();
            lblPositionPCMBytesValue.Text = Main.Player.MainChannel.PositionSentencePCMBytes.ToString();            
        }

        /// <summary>
        /// Occurs when the user clicks on the "Go to" button.
        /// Sets the player position t the current marker position.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnGoTo_Click(object sender, EventArgs e)
        {
            // Check if the player is currently playing
            if (!Main.Player.IsPlaying)
            {
                return;
            }

            // Set position
            uint position = 0;
            uint.TryParse(lblPositionPCMValue.Text, out position);
            Main.Player.MainChannel.SetPosition(position, FMOD.TIMEUNIT.SENTENCE_PCM);
        }

        /// <summary>
        /// Occurs when the user changes the name of the marker.
        /// Enables/disables the Save button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // Check if name is empty
            if (String.IsNullOrEmpty(txtName.Text))
            {
                btnSave.Enabled = false;
            }
            else
            {
                btnSave.Enabled = true;
            }
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