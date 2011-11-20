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
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Player.PlayerV4;
using MPfm.Sound;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Edit Song Metadata window. This is where the user can modify the ID3 and other
    /// tags for the media files.
    /// </summary>
    public partial class frmAddEditLoop : MPfm.WindowsControls.Form
    {
        // Private variables
        private AddEditLoopWindowMode m_mode = AddEditLoopWindowMode.Add;
        private frmMain m_main = null;        
        private AudioFile m_audioFile = null;
        private Guid m_loopId = Guid.Empty;
        private List<Marker> m_markers = null;
        private uint m_loopLengthMS = 0;
        private uint m_loopLengthPCM = 0;
        private uint m_loopLengthPCMBytes = 0;

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
        /// Constructor for Add/Edit Loop window. Requires a hook to the main form and
        /// the window mode must be specified.
        /// </summary>
        /// <param name="main">Hook to the main window</param>
        /// <param name="mode">Window mode</param>
        /// <param name="audioFile">AudioFile linked to the marker</param>
        /// <param name="loopId">Identifier of the loop (if it exists)</param>
        public frmAddEditLoop(frmMain main, AddEditLoopWindowMode mode, AudioFile audioFile, Guid loopId)
        {
            InitializeComponent();
            m_main = main;
            m_mode = mode;
            m_audioFile = audioFile;
            m_loopId = loopId;

            // Initialize controls
            Initialize();
        }

        /// <summary>
        /// Initializes the controls depending on the window mode.
        /// </summary>
        private void Initialize()
        {
            // Set song labels
            lblArtistName.Text = m_audioFile.ArtistName;
            lblAlbumTitle.Text = m_audioFile.AlbumTitle;
            lblSongTitle.Text = m_audioFile.Title;

            // Refresh markers
            RefreshMarkers();

            // Set labels depending on mode
            if (m_mode == AddEditLoopWindowMode.Add)
            {
                panelEditLoop.HeaderTitle = "Add loop";
                Text = "Add loop";
            }
            else if (m_mode == AddEditLoopWindowMode.Edit)
            {
                panelEditLoop.HeaderTitle = "Edit loop";
                panelEditLoop.Refresh();
                Text = "Edit loop";

                // Fetch loop from database
                //MPfm.Library.Data.Loop loop = DataAccess.SelectLoop(m_loopId);

                //// Check if the loop was found
                //if (loop == null)
                //{
                //    return;
                //}

                //// Update fields
                //txtName.Text = loop.Name;
                //comboMarkerA.SelectedValue = loop.MarkerAId;
                //comboMarkerB.SelectedValue = loop.MarkerBId;
            }
        }

        /// <summary>
        /// Refreshes the list of markers in both comboboxes.
        /// </summary>
        public void RefreshMarkers()
        {
            // Fetch markers from database
            m_markers = Main.Library.Gateway.SelectMarkers(m_main.Player.Playlist.CurrentItem.AudioFile.Id);

            // Set combo box items for A
            comboMarkerA.DataSource = m_markers;

            // Set combo box items for B (refetch data because data binding the same objects make both combo box value change at the same time...)
            m_markers = Main.Library.Gateway.SelectMarkers(m_main.Player.Playlist.CurrentItem.AudioFile.Id);
            comboMarkerB.DataSource = m_markers;  
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
            // Get loop length
            int length = 0;
            int.TryParse(lblLoopLengthValue.Text, out length);

            // Get selected markers
            Marker markerA = (Marker)comboMarkerA.SelectedItem;
            Marker markerB = (Marker)comboMarkerB.SelectedItem;

            // Create a new loop or fetch the existing loop from the database
            Loop loop = null;
            if (m_mode == AddEditLoopWindowMode.Add)
            {
                // Insert the new loop into the database
                loop = new Loop();
                loop.LoopId = Guid.NewGuid();
            }
            else if (m_mode == AddEditLoopWindowMode.Edit)
            {
                // Select the existing loop from the database
                loop = Main.Library.Gateway.SelectLoop(m_loopId);
            }

            // Set properties    
            loop.Name = txtName.Text;
            loop.AudioFileId = m_audioFile.Id;
            loop.MarkerA = markerA;
            loop.MarkerB = markerB;
            loop.Length = Conversion.MillisecondsToTimeString((ulong)m_loopLengthMS);
            loop.LengthBytes = m_loopLengthPCMBytes;
            loop.LengthSamples = m_loopLengthPCM;

            // Determine if an INSERT or an UPDATE is necessary
            if (m_mode == AddEditLoopWindowMode.Add)
            {
                // Insert loop
                Main.Library.Gateway.InsertLoop(loop);

                // Refresh window as Edit Loop
                m_loopId = loop.LoopId;
                m_mode = AddEditLoopWindowMode.Edit;
                Initialize();
            }
            else if (m_mode == AddEditLoopWindowMode.Edit)
            {
                // Update loop
                Main.Library.Gateway.UpdateLoop(loop);
            }

            // Refresh main window loop list
            Main.RefreshLoops();
        }

        /// <summary>
        /// Occurs when the user changes the name of the marker.
        /// Enables/disables the Save button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // Validate form
            ValidateForm();
        }

        /// <summary>
        /// Occurs when the user changes the marker A selection.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void comboMarkerA_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get marker and display position
            Marker marker = (Marker)comboMarkerA.SelectedItem;
            lblMarkerAPosition.Text = marker.Position;

            // Validate form
            ValidateForm();
        }

        /// <summary>
        /// Occurs when the user changes the marker B selection.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void comboMarkerB_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get marker and display position
            Marker marker = (Marker)comboMarkerB.SelectedItem;
            lblMarkerBPosition.Text = marker.Position;

            // Validate form
            ValidateForm();
        }

        /// <summary>
        /// Validates the form and displays warning if needed.
        /// </summary>
        public void ValidateForm()
        {
            // Declare variables
            bool isValid = true;
            string warningMessage = string.Empty;

            // Check if name is empty
            if (String.IsNullOrEmpty(txtName.Text))
            {
                isValid = false;
                warningMessage = "The loop must have a valid name.";                
            }

            // Check if the loop length is negative or zero
            if (String.IsNullOrEmpty(lblMarkerAPosition.Text) ||
                String.IsNullOrEmpty(lblMarkerBPosition.Text))
            {
                isValid = false;
                warningMessage = "The loop length must be positive.";
            }
            else
            {
                // Get delta ms
                uint msMarkerA = ConvertAudio.ToMS(lblMarkerAPosition.Text);
                uint msMarkerB = ConvertAudio.ToMS(lblMarkerBPosition.Text);

                // Check if the loop length is negative or zero
                if (msMarkerB < msMarkerA || msMarkerA == msMarkerB)
                {
                    isValid = false;
                    warningMessage = "The loop length must be positive.";
                }
                else
                {
                    // Convert values
                    m_loopLengthMS = msMarkerB - msMarkerA;
                    m_loopLengthPCM = ConvertAudio.ToPCM(m_loopLengthMS, 44100);
                    m_loopLengthPCMBytes = ConvertAudio.ToPCMBytes(m_loopLengthPCM, 16, 2);

                    // Update loop length
                    lblLoopLengthValue.Text = MPfm.Core.Conversion.MillisecondsToTimeString((ulong)m_loopLengthMS);
                    lblLoopLengthPCMValue.Text = m_loopLengthPCM.ToString();
                    lblLoopLengthPCMBytesValue.Text = m_loopLengthPCMBytes.ToString();
                }
            }

            // Set warning
            panelWarning.Visible = !isValid;
            lblWarning.Text = warningMessage;

            // Enable/disable save button
            btnSave.Enabled = isValid;
        }
    }

    /// <summary>
    /// Defines the mode of the AddEditLoop window.
    /// </summary>
    public enum AddEditLoopWindowMode
    {
        Add = 0, Edit = 1
    }
}
