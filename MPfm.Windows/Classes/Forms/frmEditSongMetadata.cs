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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;

namespace MPfm.Windows.Classes.Forms
{
    /// <summary>
    /// Edit Song Metadata window. This is where the user can modify the ID3 and other
    /// tags for the media files.
    /// </summary>
    public partial class frmEditSongMetadata : BaseForm, IEditSongMetadataView
    {
        private AudioFile _audioFile;

        public frmEditSongMetadata(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Close" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {            
            this.Close();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            OnSaveAudioFile(_audioFile);

            //    // Check if the player is playing
            //    if (Main.Player.IsPlaying)
            //    {
            //        // Check if the file is currently playing
            //        if (Main.Player.Playlist.CurrentItem.AudioFile.FilePath == audioFile.FilePath)
            //        {
            //            // Warn user that this will stop playback.
            //            if (MessageBox.Show("This audio file is currently playing. Do you wish to stop the playback to save this audio file metadata?", "Must stop playback to save audio file metadata", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
            //            {
            //                // Stop playback
            //                Main.Stop();
            //            }
            //            else
            //            {
            //                // Cancel operation
            //                return;
            //            }
            //        }
            //    }
        }

        #region IEditSongMetadataView implementation

        public Action<AudioFile> OnSaveAudioFile { get; set; }

        public void EditSongMetadataError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                MessageBox.Show(string.Format("An error occured in EditSongMetadata: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshAudioFile(AudioFile audioFile)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                _audioFile = audioFile;
                lblEditing.Text = String.Format("Editing {0}", audioFile.FilePath);
                propertyGridTags.SelectedObject = audioFile;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        #endregion

    }
}
