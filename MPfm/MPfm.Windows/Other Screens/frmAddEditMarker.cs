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
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm
{
    /// <summary>
    /// Add/Edit Markers window. This is where the user can add or edit markers for an audio file.
    /// </summary>
    public partial class frmAddEditMarker : MPfm.WindowsControls.Form
    {
        // Private variables
        private AddEditMarkerWindowMode mode = AddEditMarkerWindowMode.Add;
        private frmMain main = null;        
        private AudioFile audioFile = null;
        private Guid markerId = Guid.Empty;

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
        /// Constructor for Add/Edit Marker window. Requires a hook to the main form and
        /// the window mode must be specified.
        /// </summary>
        /// <param name="main">Hook to the main window</param>
        /// <param name="mode">Window mode</param>
        /// <param name="audioFile">AudioFile linked to the marker</param>
        /// <param name="markerId">Identifier of the marker (if it exists)</param>
        public frmAddEditMarker(frmMain main, AddEditMarkerWindowMode mode, AudioFile audioFile, Guid markerId)
        {
            InitializeComponent();
            this.main = main;
            this.mode = mode;
            this.audioFile = audioFile;
            this.markerId = markerId;

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

            // Set labels depending on mode
            if (mode == AddEditMarkerWindowMode.Add)
            {
                panelEditMarker.HeaderTitle = "Add Marker";
                Text = "Add Marker";
            }
            else if (mode == AddEditMarkerWindowMode.Edit)
            {
                panelEditMarker.HeaderTitle = "Edit Marker";
                panelEditMarker.Refresh();
                Text = "Edit Marker";

                // Fetch marker from database                
                Marker marker = Main.Library.Facade.SelectMarker(markerId);

                // Check if the marker was found
                if(marker == null)
                {
                    return;
                }

                // Update fields
                txtName.Text = marker.Name;
                txtComments.Text = marker.Comments;
                txtPosition.Text = marker.Position;
                lblMarkerPositionSamplesValue.Text = marker.PositionSamples.ToString();
                lblMarkerPositionBytesValue.Text = marker.PositionBytes.ToString();
            }

            // Validate form
            ValidateForm();
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
            // Get PCM and PCM bytes values
            long pcm = 0;
            long.TryParse(lblMarkerPositionSamplesValue.Text, out pcm);
            long pcmBytes = 0;
            long.TryParse(lblMarkerPositionBytesValue.Text, out pcmBytes);

            // Create a new marker or fetch the existing marker from the database            
            Marker marker = null;
            if (mode == AddEditMarkerWindowMode.Add)
            {
                // Insert the new marker into the database
                marker = new Marker();
                marker.MarkerId = Guid.NewGuid();
            }
            else if (mode == AddEditMarkerWindowMode.Edit)
            {
                // Select the existing marker from the database
                marker = Main.Library.Facade.SelectMarker(markerId);
            }

            // Set properties            
            marker.AudioFileId = audioFile.Id;
            marker.Name = txtName.Text;
            marker.Comments = txtComments.Text;
            marker.Position = txtPosition.Text;
            marker.PositionBytes = pcm;
            marker.PositionBytes = pcmBytes;

            // Determine if an INSERT or an UPDATE is necessary
            if (mode == AddEditMarkerWindowMode.Add)
            {
                // Insert marker
                Main.Library.Facade.InsertMarker(marker);

                // Refresh window as Edit Marker
                markerId = marker.MarkerId;
                mode = AddEditMarkerWindowMode.Edit;
                Initialize();
            }
            else if (mode == AddEditMarkerWindowMode.Edit)
            {
                // Update marker
                Main.Library.Facade.UpdateMarker(marker);
            }

            // Refresh main window marker list
            Main.RefreshMarkers();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Punch in" button.
        /// Sets the marker position to the current playback position.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPunchIn_Click(object sender, EventArgs e)
        {
            // Check if the player is currently playing
            // test
            if (!Main.Player.IsPlaying)
            {
                return;
            }

            // Get position
            //long positionBytes = Main.Player.Playlist.CurrentItem.Channel.GetPosition();
            //long positionSamples = ConvertAudio.ToPCM(positionBytes, 16, 2);
            //string position = ConvertAudio.ToTimeString(positionBytes, 16, 2, 44100);

            // Get position
            long positionBytes = Main.Player.GetPosition();
            long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)Main.Player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
            string position = ConvertAudio.ToTimeString(positionBytes, (uint)Main.Player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2, (uint)Main.Player.Playlist.CurrentItem.AudioFile.SampleRate);

            // Update controls
            txtPosition.Text = position;
            lblMarkerPositionSamplesValue.Text = positionSamples.ToString();
            lblMarkerPositionBytesValue.Text = positionBytes.ToString();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Go to" button.
        /// Sets the player position to the current marker position.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnGoTo_Click(object sender, EventArgs e)
        {
            // Check if the player is currently playing
            if (!Main.Player.IsPlaying)
            {
                return;
            }

            // Set position
            uint position = 0;
            uint.TryParse(lblMarkerPositionBytesValue.Text, out position);            
            Main.Player.SetPosition(position);
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
        /// Occurs when the user changes the position value.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void txtPosition_TextChanged(object sender, EventArgs e)
        {
            // Convert 0:00.000 to MS
            uint totalMS = ConvertAudio.ToMS(txtPosition.Text);
            uint samples = ConvertAudio.ToPCM(totalMS, 44100); // Sample rate of the song, not of the mixer!
            uint bytes = ConvertAudio.ToPCMBytes(samples, 16, 2);

            // Set new values
            lblMarkerPositionMSValue.Text = totalMS.ToString();
            lblMarkerPositionSamplesValue.Text = samples.ToString();
            lblMarkerPositionBytesValue.Text = bytes.ToString();

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
                warningMessage = "The marker must have a valid name.";
            }

            // Get song length in MS
            uint msTotal = ConvertAudio.ToMS(audioFile.Length);
            uint msMarker = ConvertAudio.ToMS(txtPosition.Text);

            // Check if the position exceeds the song length
            if (msMarker > msTotal)
            {
                isValid = false;
                warningMessage = "The marker position cannot exceed the audio file length (" + audioFile.Length + ").";
            }

            // Set warning
            panelWarning.Visible = !isValid;
            lblWarning.Text = warningMessage;

            // Enable/disable save button
            btnSave.Enabled = isValid;
        }
    }

    /// <summary>
    /// Defines the mode of the AddEditMarker window.
    /// </summary>
    public enum AddEditMarkerWindowMode
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