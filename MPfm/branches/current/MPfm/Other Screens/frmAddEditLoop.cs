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
using MPfm.Core;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm
{
    /// <summary>
    /// Add/Edit Loop window. This is where the user can add or edit loops for an audio file.
    /// </summary>
    public partial class frmAddEditLoop : MPfm.WindowsControls.Form
    {
        // Private variables
        private AddEditLoopWindowMode mode = AddEditLoopWindowMode.Add;
        private frmMain main = null;        
        private AudioFile audioFile = null;
        private Guid loopId = Guid.Empty;
        private List<Marker> markers = null;
        private uint loopLengthMS = 0;
        private uint loopLengthBytes = 0;
        private uint loopLengthSamples = 0;        
        private long startPositionBytes = 0;
        private long endPositionBytes = 0;

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
            this.main = main;
            this.mode = mode;
            this.audioFile = audioFile;
            this.loopId = loopId;

            // Initialize controls
            Initialize();
        }

        /// <summary>
        /// Initializes the controls depending on the window mode.
        /// </summary>
        private void Initialize()
        {
            // Set song labels
            lblArtistNameValue.Text = audioFile.ArtistName;
            lblAlbumTitleValue.Text = audioFile.AlbumTitle;
            lblSongTitleValue.Text = audioFile.Title;

            // Refresh markers
            RefreshMarkers();

            // Set labels depending on mode
            if (mode == AddEditLoopWindowMode.Add)
            {
                panelEditLoop.HeaderTitle = "Add Loop";
                Text = "Add Loop";
            }
            else if (mode == AddEditLoopWindowMode.Edit)
            {
                panelEditLoop.HeaderTitle = "Edit Loop";
                panelEditLoop.Refresh();
                Text = "Edit Loop";

                // Fetch loop from database                
                Loop loop = Main.Library.Facade.SelectLoop(loopId);

                // Check if the loop was found
                if (loop == null)
                {
                    return;
                }

                // Update fields
                txtName.Text = loop.Name;
                txtStartPosition.Text = loop.StartPosition;
                txtEndPosition.Text = loop.EndPosition;
                startPositionBytes = loop.StartPositionBytes;
                endPositionBytes = loop.EndPositionBytes;
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
            markers = Main.Library.Facade.SelectMarkers(main.Player.Playlist.CurrentItem.AudioFile.Id);
            markers.Insert(0, new Marker());

            // Set combo box items for A
            comboStartPositionMarker.DataSource = markers;

            // Set combo box items for B (refetch data because data binding the same objects make both combo box value change at the same time...)
            markers = Main.Library.Facade.SelectMarkers(main.Player.Playlist.CurrentItem.AudioFile.Id);
            markers.Insert(0, new Marker());
            comboEndPositionMarker.DataSource = markers;  
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
            if (mode == AddEditLoopWindowMode.Add)
            {
                // Insert the new loop into the database
                loop = new Loop();
                loop.LoopId = Guid.NewGuid();
            }
            else if (mode == AddEditLoopWindowMode.Edit)
            {
                // Select the existing loop from the database
                loop = Main.Library.Facade.SelectLoop(loopId);
            }

            // Set properties    
            loop.Name = txtName.Text;
            loop.AudioFileId = audioFile.Id;
            loop.Length = Conversion.MillisecondsToTimeString((ulong)loopLengthMS);
            loop.LengthBytes = loopLengthBytes;
            loop.LengthSamples = loopLengthSamples;
            loop.StartPosition = txtStartPosition.Text;
            loop.StartPositionBytes = (uint)startPositionBytes;            
            loop.EndPosition = txtEndPosition.Text;
            loop.EndPositionBytes = (uint)endPositionBytes;

            // Determine if an INSERT or an UPDATE is necessary
            if (mode == AddEditLoopWindowMode.Add)
            {
                // Insert loop
                Main.Library.Facade.InsertLoop(loop);

                // Refresh window as Edit Loop
                loopId = loop.LoopId;
                mode = AddEditLoopWindowMode.Edit;
                Initialize();
            }
            else if (mode == AddEditLoopWindowMode.Edit)
            {
                // Update loop
                Main.Library.Facade.UpdateLoop(loop);
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
            startPositionBytes = marker.PositionBytes;

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
            endPositionBytes = marker.PositionBytes;

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
            if (endPositionBytes <= startPositionBytes)
            {
                isValid = false;
                warningMessage = "The loop length must be positive.";
            }
            else
            {
                // Convert values                
                loopLengthBytes = (uint)(endPositionBytes - startPositionBytes);
                loopLengthSamples = (uint)ConvertAudio.ToPCM((long)loopLengthBytes, 16, 2);
                loopLengthMS = (uint)ConvertAudio.ToMS((long)loopLengthSamples, 44100);

                // Update loop length
                lblLoopLengthValue.Text = MPfm.Core.Conversion.MillisecondsToTimeString((ulong)loopLengthMS);
                lblLoopLengthPCMBytesValue.Text = loopLengthBytes.ToString();
                lblLoopLengthPCMValue.Text = loopLengthSamples.ToString();                
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
            startPositionBytes = Main.Player.GetPosition();            
            string position = ConvertAudio.ToTimeString(startPositionBytes, (uint)Main.Player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2, (uint)Main.Player.Playlist.CurrentItem.AudioFile.SampleRate);

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
            endPositionBytes = Main.Player.GetPosition();
            string position = ConvertAudio.ToTimeString(endPositionBytes, (uint)Main.Player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2, (uint)Main.Player.Playlist.CurrentItem.AudioFile.SampleRate);

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
            Main.Player.SetPosition(startPositionBytes);
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
            Main.Player.SetPosition(endPositionBytes);
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
            foreach (Marker marker in markers)
            {
                // Skip any unnamed markers
                if (!String.IsNullOrEmpty(marker.Name))
                {
                    // Check if the start position matches
                    if (marker.PositionBytes == startPositionBytes)
                    {
                        // Set start position selected item
                        comboStartPositionMarker.SelectedItem = marker;
                    }

                    // Check if the end position matches
                    if (marker.PositionBytes == endPositionBytes)
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
