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
    /// Load playlist window. This is a small window displaying the loading progress
    /// of a playlist.
    /// </summary>
    public partial class frmLoadPlaylist : MPfm.WindowsControls.Form
    {
        // Private variables
        private frmMain m_main = null;
        private string m_playlistFilePath = string.Empty;

        /// <summary>
        /// Private value for the AudioFiles property.
        /// </summary>
        private List<AudioFile> m_audioFiles = null;
        /// <summary>
        /// List of AudioFiles that have been scanned by the background worker.
        /// </summary>
        public List<AudioFile> AudioFiles
        {
            get
            {
                return m_audioFiles;
            }
        }

        /// <summary>
        /// Private value for the FailedAudioFilePaths property.
        /// </summary>
        private List<string> m_failedAudioFilePaths = null;
        /// <summary>
        /// List of audio file paths that could not be loaded successfully.
        /// </summary>
        public List<string> FailedAudioFilePaths
        {
            get
            {
                return m_failedAudioFilePaths;
            }
        }

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
        /// Constructor for Load playlist window. Requires the playlist file path and
        /// a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        /// <param name="playlistFilePath">Playlist file path</param>
        public frmLoadPlaylist(frmMain main, string playlistFilePath)
        {
            InitializeComponent();
            m_main = main;
            m_playlistFilePath = playlistFilePath;
        }

        /// <summary>
        /// Occurs when the form is loading for the first time.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmLoadPlaylist_Load(object sender, EventArgs e)
        {
            // Start worker
            worker.RunWorkerAsync(m_playlistFilePath);
        }

        /// <summary>
        /// Occurs when the user clicks on the "Cancel" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Check if the background worker is still running
            if (worker.IsBusy && !worker.CancellationPending)
            {
                // Cancel background worker
                worker.CancelAsync();
            }
        }

        /// <summary>
        /// Callback for the background worker that does the main work.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get argument
            string playlistFilePath = (string)e.Argument;

            // Report progress
            worker.ReportProgress(0, new WorkerLoadPlaylistProgressData() { Message = "Analyzing playlist file..." });

            List<string> filePaths = new List<string>();
            try
            {
                // Load playlist file paths
                filePaths = PlaylistTools.LoadPlaylist(playlistFilePath);
            }
            catch (Exception ex)
            {
                // Stop process
                e.Result = ex;
                return;
            }

            // Create completed event data
            WorkerLoadPlaylistCompleteData complete = new WorkerLoadPlaylistCompleteData();            

            // Loop through files
            for (int a = 0; a < filePaths.Count; a++)
            {
                // Check for cancel
                if (worker.CancellationPending)
                {                    
                    e.Cancel = true;
                    return;
                }

                //System.Threading.Thread.Sleep(10);

                // Try to get audio file
                WorkerLoadPlaylistProgressData data = new WorkerLoadPlaylistProgressData();
                AudioFile audioFile = null;
                try
                {
                    // Add audio file
                    audioFile = new AudioFile(filePaths[a]);
                }
                catch (Exception ex)
                {
                    // Keep the audio file null, we don't load this file
                    data.Exception = ex;
                    complete.FailedAudioFilePaths.Add(filePaths[a]);
                }

                // Report progress
                data.Message = "Loading playlist :";
                data.FilePath = filePaths[a];
                int percentage = (int)(((float)a / (float)filePaths.Count) * 100);
                worker.ReportProgress(percentage, data);

                // Add to list
                if (audioFile != null)
                {
                    // Add audio file
                    complete.AudioFiles.Add(audioFile);
                }
            }

            // Report completed progress
            e.Result = complete;
        }

        /// <summary>
        /// Occurs when the background worker needs to report progress.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Get data structure
            WorkerLoadPlaylistProgressData data = (WorkerLoadPlaylistProgressData)e.UserState;

            // Report progress
            lblFilePath.Text = data.FilePath;
            lblPercentage.Text = e.ProgressPercentage.ToString() + "%";
            progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Occurs when the background worker has completed its work (canceled or not).
        /// Closes the work.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check if the result is an exception
            if (e.Result is Exception)
            {
                // Cast exception
                Exception ex = (Exception)e.Result;

                // Show message box
                MessageBox.Show("There was an error while loading the playlist:\n" + ex.Message + "\n" + ex.StackTrace, "Error loading playlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Check if the operation was canceled
                if (e.Cancelled)
                {
                    // Set dialog result
                    DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else
                {
                    // Get data structure
                    WorkerLoadPlaylistCompleteData data = (WorkerLoadPlaylistCompleteData)e.Result;

                    // Set dialog result
                    m_audioFiles = data.AudioFiles;
                    m_failedAudioFilePaths = data.FailedAudioFilePaths;
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }

            // Close form
            this.Close();
        }

    }

    /// <summary>
    /// Data structure used for reporting progress for the Load Playlist background worker.
    /// </summary>
    public class WorkerLoadPlaylistProgressData
    {
        /// <summary>
        /// Defines the exception related to the audio file loading.
        /// If null, it means the file has loaded successfully.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Message to display.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Audio file path currently loading
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Default constructor for the WorkerLoadPlaylistProgressData class.
        /// </summary>
        public WorkerLoadPlaylistProgressData()
        {
            // Set default values
            Exception = null;
            Message = string.Empty;
            FilePath = string.Empty;
        }
    }

    /// <summary>
    /// Data structure used for reporting completed progress for the Load Playlist background worker.
    /// </summary>
    public class WorkerLoadPlaylistCompleteData
    {
        /// <summary>
        /// List of audio files to add to the playlist.
        /// </summary>
        public List<AudioFile> AudioFiles { get; set; }

        /// <summary>
        /// List of audio file paths that could not be loaded.
        /// </summary>
        public List<string> FailedAudioFilePaths { get; set; }

        /// <summary>
        /// Default constructor for the WorkerLoadPlaylistCompleteData class.
        /// </summary>
        public WorkerLoadPlaylistCompleteData()
        {
            // Set default values
            AudioFiles = new List<AudioFile>();
            FailedAudioFilePaths = new List<string>();
        }
    }

}