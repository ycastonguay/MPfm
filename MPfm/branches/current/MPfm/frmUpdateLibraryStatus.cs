//
// frmUpdateLibraryStatus.cs: Update Library window. This window is displayed when the application is updating the library.
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MPfm.Sound;
using MPfm.Library;
using MPfm.Core;

namespace MPfm
{
    /// <summary>
    /// Update Library window. This window is displayed when the application 
    /// is updating the library.
    /// </summary>
    public partial class frmUpdateLibraryStatus : Form
    {
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

        // Private variables        
        private UpdateLibraryMode mode;
        private StringBuilder sbLog = new StringBuilder();
        private DateTime startTime;
        private DateTime startTimeAddFiles;
        private TimeSpan timeElapsed;        
        private List<string> listAlbums = null;
        private bool timerEnabled = false;

        private List<string> m_filePaths = null;
        public List<string> FilePaths
        {
            get
            {
                return m_filePaths;
            }
        }

        private string m_folderPath = string.Empty;
        public string FolderPath
        {
            get
            {
                return m_folderPath;
            }
        }

        /// <summary>
        /// Default constructor for the Update Library Status window.
        /// This updates the whole library.
        /// </summary>
        /// <param name="main">Hook to main form</param>        
        public frmUpdateLibraryStatus(frmMain main)
        {
            // Initialize the form
            InitializeForm(main, UpdateLibraryMode.WholeLibrary, null, null);
        }

        /// <summary>
        /// Constructor for the Update Library Status window.
        /// This is the constructor for the SpecificFiles mode. You must pass
        /// the files to update in parameter as a string List.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        /// <param name="filePaths">Files to update</param>
        public frmUpdateLibraryStatus(frmMain main, List<string> filePaths)
        {
            // Initialize the form
            InitializeForm(main, UpdateLibraryMode.SpecificFiles, filePaths, null);
        }

        /// <summary>
        /// Constructor for the Update Library Status window.
        /// This is the constructor for the SpecificFolder mode. You must pass
        /// the folder to update in parameter.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        /// <param name="folderPath">Folder path</param>
        public frmUpdateLibraryStatus(frmMain main, string folderPath)
        {
            // Initialize the form
            InitializeForm(main, UpdateLibraryMode.SpecificFolder, null, folderPath);
        }

        /// <summary>
        /// Initializes the form. Needs a handle to the main form and the mode must be specified
        /// with the accompagnied parameters.
        /// </summary>
        /// <param name="main">Handle to the main form</param>
        /// <param name="mode">Update Library Status Window mode</param>
        /// <param name="filePaths">File paths (for the SpecificFiles mode)</param>
        /// <param name="folderPath">Folder path (for the SpecificFolder mode)</param>
        private void InitializeForm(frmMain main, UpdateLibraryMode mode, List<string> filePaths, string folderPath)
        {
            // Initialize private variables
            InitializeComponent();
            m_main = main;
            this.mode = mode;
            m_filePaths = filePaths;
            m_folderPath = folderPath;

            // Update UI
            lblTitle.Text = "Updating";
            lblMessage.Text = "Calculating...";
            lblArtist.Text = string.Empty;
            lblAlbum.Text = string.Empty;
            lblSongTitle.Text = string.Empty;

            // Set update library events
            main.Library.OnUpdateLibraryProgress += new MPfm.Library.Library.UpdateLibraryProgress(Library_OnUpdateLibraryProgress);
            main.Library.OnUpdateLibraryFinished += new MPfm.Library.Library.UpdateLibraryFinished(Library_OnUpdateLibraryFinished);
        }

        /// <summary>
        /// Fires when the form is shown. The form is shown when the user has started the update process.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void frmUpdateLibraryStatus_Shown(object sender, EventArgs e)
        {
            // Stop any song playing
            //Main.Player.Stop();

            // Reset buttons
            btnCancel.Enabled = true;
            btnOK.Enabled = false;
            linkSaveLog.Visible = false;

            // Reset log
            sbLog = new StringBuilder(1000);

            // Start timer
            timerEnabled = true;
            workerTimer.RunWorkerAsync();

            // Reset variables
            startTime = DateTime.Now;
            startTimeAddFiles = DateTime.MinValue;
            listAlbums = new List<string>();

            // Update UI
            lblEstimatedTimeLeft.Text = "Estimated time left: Calculating...";

            // Start the update process
            Main.Library.UpdateLibrary(mode, FilePaths, FolderPath);
        }

        #region Control Events

        /// <summary>
        /// Fires when the user presses the OK button.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Refresh controls on the main form
            Cursor.Current = Cursors.WaitCursor;
            Main.RefreshAll();
            //main.RefreshSongBrowser();
            //main.RefreshTreeLibrary();
            Cursor.Current = Cursors.Default;

            // Close this form
            this.Close();
        }

        /// <summary>
        /// Fires when the user presses the Cancel button.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Set the cancel update library flag to true
            Main.Library.CancelUpdateLibrary = true;

            // Reset buttons
            btnOK.Enabled = true;
            btnCancel.Enabled = false;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save Log" link.
        /// Displays a "Save file to" dialog and saves the log as a text file.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void linkSaveLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Generate the name of the log
            saveLogDialog.FileName = "MPfm_UpdateLibraryLog_" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + ".txt";

            // Display the save dialog
            if (saveLogDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    // Open text writer
                    TextWriter tw = new StreamWriter(saveLogDialog.FileName);

                    foreach (String item in lbLog.Items)
                    {
                        tw.WriteLine(item);
                    }

                    tw.Close();
                }
                catch (Exception ex)
                {
                    // Display error
                    MessageBox.Show("Failed to save the file to " + saveLogDialog.FileName + "!\n\nException:\n" + ex.Message + "\n" + ex.StackTrace, "Failed to save the file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Player Events

        /// <summary>
        /// Fires when the update library process is over.
        /// </summary>
        private void Library_OnUpdateLibraryFinished(UpdateLibraryFinishedData data)
        {
            if (data.Successful)
            {
                // Success
                lblProgress.Text = "100%";
                progressBar.Value = 100;
                lblMessage.Text = "The update library process has finished successfully.";
                lblTitle.Text = "Update library successful";
            }
            else if (data.Cancelled)
            {
                // Cancel
                lblProgress.Text = "0%";
                progressBar.Value = 0;
                lblMessage.Text = "The update library process was cancelled by the user.";
                lblTitle.Text = "Update library process cancelled";
            }
            else if (!data.Successful)
            {
                // Fail
                lblProgress.Text = "0%";
                progressBar.Value = 0;
                lblMessage.Text = "The update library process has failed.";
                lblTitle.Text = "Update library failed";
            }

            lblEstimatedTimeLeft.Text = "Estimated time left: N/A";

            // Stop timer
            timerEnabled = false;
            //workerTimer.CancelAsync();

            // Refresh song cache
            Main.Library.RefreshCache();

            // Set buttons
            btnCancel.Enabled = false;
            btnOK.Enabled = true;
            linkSaveLog.Visible = true;
        }

        /// <summary>
        /// Fires when the update library process sends a progress changed message.
        /// </summary>
        /// <param name="data">Update data</param>
        private void Library_OnUpdateLibraryProgress(UpdateLibraryProgressData data)
        {                         
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Check if there is an error
                if (data.Error != null)
                {
                    //string errorMessage = "Error: Cannot add this file to the library - " + status.errorInformation + "\n\nFile Path: " + status.filePath + "\nReason: " + status.error.Message;
                    string errorMessage;

                    if (!String.IsNullOrEmpty(data.FilePath))
                    {
                        errorMessage = data.FilePath + "\n";
                    }
                    else
                    {
                        errorMessage = "====================\n";
                    }

                    errorMessage += "   Error: " + data.Error.Message;

                    if (data.Error.InnerException != null)
                    {
                        errorMessage += "\n   " + data.Error.InnerException.Message;
                    }
                    errorMessage += "\n";

                    // Log for file
                    sbLog.Insert(0, errorMessage);

                    // Add to log screen
                    lbLog.Items.Insert(0, "Error reading " + Path.GetFileName(data.FilePath) + ": " + data.Error.Message);

                    lblMessage.Text = data.Message;
                    lblProgress.Text = data.Percentage.ToString("0.000") + " %";
                    progressBar.Value = Convert.ToInt32(data.Percentage);
                }
                else
                {
                    // Update UI                
                    if (data.CurrentFilePosition > 0 && data.TotalNumberOfFiles > 0)
                    {
                        // Set start time when the process has finished finding the files and is ready to add files into library
                        if (startTimeAddFiles == DateTime.MinValue)
                        {
                            startTimeAddFiles = DateTime.Now;
                        }

                        // Calculate time elapsed
                        TimeSpan timeElapsed = DateTime.Now.Subtract(startTimeAddFiles);

                        // Update title
                        lblTitle.Text = data.Title + " (file " + data.CurrentFilePosition.ToString() + " of " + data.TotalNumberOfFiles.ToString() + ")";

                        // Calculate time remaining
                        double msPerFile = timeElapsed.TotalMilliseconds / data.CurrentFilePosition;
                        double remainingTime = (data.TotalNumberOfFiles - data.CurrentFilePosition) * msPerFile;
                        TimeSpan timeRemaining = new TimeSpan(0, 0, 0, 0, (int)remainingTime);

                        // Update estimated time left (from more precise to more vague)
                        if (timeRemaining.TotalSeconds == 0)
                        {
                            lblEstimatedTimeLeft.Text = "Estimated time left : N/A";
                        }
                        else if (timeRemaining.Minutes == 1)
                        {
                            lblEstimatedTimeLeft.Text = "Estimated time left : 1 minute";
                        }
                        else if (timeRemaining.TotalSeconds <= 10)
                        {
                            lblEstimatedTimeLeft.Text = "Estimated time left : A few seconds";
                        }
                        else if (timeRemaining.TotalSeconds <= 30)
                        {
                            lblEstimatedTimeLeft.Text = "Estimated time left : Less than 30 seconds";
                        }
                        else if (timeRemaining.TotalSeconds <= 60)
                        {
                            lblEstimatedTimeLeft.Text = "Estimated time left : Less than a minute";
                        }
                        else
                        {
                            lblEstimatedTimeLeft.Text = "Estimated time left : " + timeRemaining.Minutes.ToString() + " minutes";
                        }
                    }
                    else
                    {
                        lblTitle.Text = data.Title;
                    }
                    lblMessage.Text = data.Message;
                    lblProgress.Text = data.Percentage.ToString("0.000") + " %";
                    progressBar.Value = Convert.ToInt32(data.Percentage);

                    if (!String.IsNullOrEmpty(data.LogEntry))
                    {
                        sbLog.Insert(0, data.LogEntry + "\n");
                    }

                    // Update song if exists
                    if (data.Song != null)
                    {
                        lblArtist.Text = data.Song.ArtistName;
                        lblAlbum.Text = data.Song.AlbumTitle;
                        lblSongTitle.Text = data.Song.SongTitle;

                        // Check if the folder has already been treated (we want to get the ID3
                        // image just once per album. This speeds up the update library process a lot.)

                        //string path = Path.GetDirectoryName(data.FilePath);
                        string artistAlbum = data.Song.ArtistName + " - " + data.Song.AlbumTitle;
                        if (!listAlbums.Contains(artistAlbum))
                        {
                            try
                            {
                                // Get album art from ID3 tag or folder.jpg
                                Image image = MPfm.Library.Library.GetAlbumArtFromID3OrFolder(data.FilePath);

                                // If image is null...
                                if (image == null)
                                {
                                    // Display nothing
                                    picAlbum.Image = null;
                                }
                                else
                                {
                                    // Update the album art                                
                                    picAlbum.Image = ImageManipulation.ResizeImage(image, picAlbum.Size.Width, picAlbum.Size.Height);
                                }

                                // Add the folder in the list of folders
                                listAlbums.Add(artistAlbum);
                            }
                            catch (Exception ex)
                            {
                                // Do nothing since this is only album art.
                            }
                        }

                        lbLog.Items.Insert(0, data.LogEntry);
                        if (lbLog.Items.Count > 1000)
                        {
                            lbLog.Items.RemoveAt(lbLog.Items.Count - 1);
                        }
                    }
                    else
                    {
                        lblArtist.Text = string.Empty;
                        lblAlbum.Text = string.Empty;
                        lblSongTitle.Text = string.Empty;
                        picAlbum.Image = null;
                    }
                }
            };

            // Check if invoking is necessary
            if (InvokeRequired)
            {
                BeginInvoke(methodUIUpdate);
            }
            else
            {
                methodUIUpdate.Invoke();
            }

            //if (data.ProgressUndefined)
            //{
            //    progressBar.Style = ProgressBarStyle.Continuous;
            //}
            //else
            //{
            //    progressBar.Style = ProgressBarStyle.Marquee;
            //    lblProgress.Text = data.Percentage.ToString("0.00") + " %";
            //    progressBar.Value = Convert.ToInt32(data.Percentage);
            //}
        }

        #endregion

        #region Background Worker

        /// <summary>
        /// Occurs when the background worker for the Update Library is started.
        /// This is the method that refreshes the UI.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void workerTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            // Loop until timer is over
            while(timerEnabled)
            {
                System.Threading.Thread.Sleep(50);

                // Invoke UI updates
                MethodInvoker methodUIUpdate = delegate
                {
                    // Make sure the worker is still operational
                    if (workerTimer.IsBusy)
                    {
                        try
                        {
                            workerTimer.ReportProgress(0);
                        }
                        catch (Exception ex)
                        {
                            // Do nothing. There are so many updates that missing one is not very important.
                        }
                    }
                };

                // Check if invoking is necessary
                if (InvokeRequired)
                {
                    BeginInvoke(methodUIUpdate);
                }
                else
                {
                    methodUIUpdate.Invoke();
                }

            };            
        }

        /// <summary>
        /// Occurs when the background worker receives an update for the progress.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void workerTimer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update time
            timeElapsed = DateTime.Now.Subtract(startTime);
            string mins = string.Empty;
            if (timeElapsed.Minutes >= 2)
            {
                mins = "minutes";
            }
            else
            {
                mins = "minute";
            }

            lblTimeElapsed.Text = "Time elapsed : " + timeElapsed.Minutes.ToString() + ":" + timeElapsed.Seconds.ToString("00") + " " + mins;
        }

        #endregion
    }
}