//
// frmAddEditLoop.cs: Add/Edit Loop window. This is where the user can add or edit 
//                    loops for an audio file.
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
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Add/Edit Loop window. This is where the user can add or edit loops for an audio file.
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
        private uint m_loopLengthBytes = 0;
        private uint m_loopLengthSamples = 0;        
        private long m_startPositionBytes = 0;
        private long m_endPositionBytes = 0;

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
            lblSongValue.Text = m_audioFile.Title + " (" + m_audioFile.ArtistName + ")";

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
                Loop loop = Main.Library.Gateway.SelectLoop(m_loopId);

                // Check if the loop was found
                if (loop == null)
                {
                    return;
                }

                // Update fields
                txtName.Text = loop.Name;
                txtStartPosition.Text = loop.StartPosition;
                txtEndPosition.Text = loop.EndPosition;
                m_startPositionBytes = loop.StartPositionBytes;
                m_endPositionBytes = loop.EndPositionBytes;
            }

            // Validate form
            ValidateForm();

            // Check if a marker matches
            CheckForRelatedPositionMarkers();
        }

        /// <summary>
        /// Refreshes the list of markers in both comboboxes.
        /// </summary>
        public void RefreshMarkers()
        {
            // Fetch markers from database
            m_markers = Main.Library.Gateway.SelectMarkers(m_main.Player.Playlist.CurrentItem.AudioFile.Id);
            m_markers.Insert(0, new Marker());

            // Set combo box items for A
            comboStartPositionMarker.DataSource = m_markers;

            // Set combo box items for B (refetch data because data binding the same objects make both combo box value change at the same time...)
            m_markers = Main.Library.Gateway.SelectMarkers(m_main.Player.Playlist.CurrentItem.AudioFile.Id);
            m_markers.Insert(0, new Marker());
            comboEndPositionMarker.DataSource = m_markers;  
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
            // Get loop length
            int length = 0;
            int.TryParse(lblLoopLengthValue.Text, out length);

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
            loop.Length = Conversion.MillisecondsToTimeString((ulong)m_loopLengthMS);
            loop.LengthBytes = m_loopLengthBytes;
            loop.LengthSamples = m_loopLengthSamples;
            loop.StartPosition = txtStartPosition.Text;
            loop.StartPositionBytes = (uint)m_startPositionBytes;            
            loop.EndPosition = txtEndPosition.Text;
            loop.EndPositionBytes = (uint)m_endPositionBytes;

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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // Validate form
            ValidateForm();
        }

        /// <summary>
        /// Occurs when the user changes the start position marker selection.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void comboStartPositionMarker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get marker and display position
            Marker marker = (Marker)comboStartPositionMarker.SelectedItem;
            txtStartPosition.Text = marker.Position;
            m_startPositionBytes = marker.PositionBytes;

            // Validate form
            ValidateForm();
        }

        /// <summary>
        /// Occurs when the user changes the end position marker selection.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void comboEndPositionMarker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get marker and display position
            Marker marker = (Marker)comboEndPositionMarker.SelectedItem;
            txtEndPosition.Text = marker.Position;
            m_endPositionBytes = marker.PositionBytes;

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

            // Valiudate loop length
            if (m_endPositionBytes <= m_startPositionBytes)
            {
                isValid = false;
                warningMessage = "The loop length must be positive.";
            }
            else
            {
                // Convert values                
                m_loopLengthBytes = (uint)(m_endPositionBytes - m_startPositionBytes);
                m_loopLengthSamples = (uint)ConvertAudio.ToPCM((long)m_loopLengthBytes, 16, 2);
                m_loopLengthMS = (uint)ConvertAudio.ToMS((long)m_loopLengthSamples, 44100);

                // Update loop length
                lblLoopLengthValue.Text = MPfm.Core.Conversion.MillisecondsToTimeString((ulong)m_loopLengthMS);
                lblLoopLengthPCMBytesValue.Text = m_loopLengthBytes.ToString();
                lblLoopLengthPCMValue.Text = m_loopLengthSamples.ToString();                
            }

            // Set warning
            panelWarning.Visible = !isValid;
            lblWarning.Text = warningMessage;

            // Enable/disable save button
            btnSave.Enabled = isValid;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Punch in" button in the Start Position section.
        /// Sets the start position to the current playback position.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnStartPositionPunchIn_Click(object sender, EventArgs e)
        {
            // Check if the player is currently playing
            if (!Main.Player.IsPlaying)
            {
                return;
            }

            // Get position
            m_startPositionBytes = Main.Player.Playlist.CurrentItem.Channel.GetPosition();
            string position = ConvertAudio.ToTimeString(m_startPositionBytes, 16, 2, 44100);

            // Update controls
            txtStartPosition.Text = position;

            // Check if a marker matches
            CheckForRelatedPositionMarkers();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Punch in" button in the End Position section.
        /// Sets the end position to the current playback position.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnEndPositionPunchIn_Click(object sender, EventArgs e)
        {
            // Check if the player is currently playing
            if (!Main.Player.IsPlaying)
            {
                return;
            }

            // Get position
            m_endPositionBytes = Main.Player.Playlist.CurrentItem.Channel.GetPosition();
            string position = ConvertAudio.ToTimeString(m_endPositionBytes, 16, 2, 44100);

            // Update controls
            txtEndPosition.Text = position;

            // Check if a marker matches
            CheckForRelatedPositionMarkers();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Go to" button in the Start Position section.
        /// Sets the player position to the loop start position.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnStartPositionGoTo_Click(object sender, EventArgs e)
        {
            // Check if the player is currently playing
            if (!Main.Player.IsPlaying)
            {
                return;
            }

            // Set position            
            Main.Player.SetPosition(m_startPositionBytes);
        }

        /// <summary>
        /// Occurs when the user clicks on the "Go to" button in the End Position section.
        /// Sets the player position to the loop end position.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnEndPositionGoTo_Click(object sender, EventArgs e)
        {
            // Check if the player is currently playing
            if (!Main.Player.IsPlaying)
            {
                return;
            }

            // Set position            
            Main.Player.SetPosition(m_endPositionBytes);
        }

        /// <summary>
        /// Occurs when the user changes the start position textbox.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void txtStartPosition_TextChanged(object sender, EventArgs e)
        {
            // Check if a marker matches
            CheckForRelatedPositionMarkers();

            // Validate form
            ValidateForm();
        }

        /// <summary>
        /// Occurs when the user changes the end position textbox.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void txtEndPosition_TextChanged(object sender, EventArgs e)
        {
            // Check if a marker matches
            CheckForRelatedPositionMarkers();

            // Validate form
            ValidateForm();
        }

        /// <summary>
        /// Finds any related marker based on the start or end position.
        /// </summary>
        public void CheckForRelatedPositionMarkers()
        {
            // Loop through markers to check if the position matches
            foreach (Marker marker in m_markers)
            {
                // Skip any unnamed markers
                if (!String.IsNullOrEmpty(marker.Name))
                {
                    // Check if the start position matches
                    if (marker.PositionBytes == m_startPositionBytes)
                    {
                        // Set start position selected item
                        comboStartPositionMarker.SelectedItem = marker;
                    }

                    // Check if the end position matches
                    if (marker.PositionBytes == m_endPositionBytes)
                    {
                        // Set end position selected item
                        comboEndPositionMarker.SelectedItem = marker;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Defines the mode of the AddEditLoop window.
    /// </summary>
    public enum AddEditLoopWindowMode
    {
        /// <summary>
        /// The window is in "Add" mode.
        /// </summary>
        Add = 0, 
        /// <summary>
        /// The window is in "Edit" mode.
        /// </summary>
        Edit = 1
    }
}
