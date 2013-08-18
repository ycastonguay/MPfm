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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MPfm.Library.Objects;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    /// <summary>
    /// Update Library window. This window is displayed when the application 
    /// is updating the library.
    /// </summary>
    public partial class frmUpdateLibrary : BaseForm, IUpdateLibraryView
    {
        public frmUpdateLibrary(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();

            //lblTitle.Text = "Updating";
            //lblMessage.Text = "Calculating...";
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
            btnSaveLog.Enabled = false;

            //// Reset log
            //sbLog = new StringBuilder(1000);

            //// Start timer
            //timerEnabled = true;
            //workerTimer.RunWorkerAsync();

            //// Reset variables
            //startTime = DateTime.Now;
            //startTimeAddFiles = DateTime.MinValue;
            //listAlbums = new List<string>();

            // Update UI
            lblEstimatedTimeLeft.Text = "Estimated time left: Calculating...";

            // Start the update process
            //Main.Library.UpdateLibrary(mode, FilePaths, FolderPath);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Refresh controls on the main form
            Cursor.Current = Cursors.WaitCursor;

            // Reset query and refresh all controls
            //Main.ResetQuery();
            //Main.RefreshAll();            

            Cursor.Current = Cursors.Default;

            // Close this form
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //// Set the cancel update library flag to true
            //Main.Library.CancelUpdateLibrary = true;

            //// Reset buttons
            //btnOK.Enabled = true;
            //btnCancel.Enabled = false;
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            //// Generate the name of the log
            //saveLogDialog.FileName = "MPfm_UpdateLibraryLog_" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + ".txt";

            //// Display the save dialog
            //if (saveLogDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    TextWriter tw = null;
            //    try
            //    {
            //        // Open text writer
            //        tw = new StreamWriter(saveLogDialog.FileName);

            //        foreach (String item in lbLog.Items)
            //        {
            //            tw.WriteLine(item);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        // Display error
            //        MessageBox.Show("Failed to save the file to " + saveLogDialog.FileName + "!\n\nException:\n" + ex.Message + "\n" + ex.StackTrace, "Failed to save the file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    finally
            //    {
            //        tw.Close();
            //    }
            //}
        }

        ///// <summary>
        ///// Fires when the update library process sends a progress changed message.
        ///// </summary>
        ///// <param name="data">Update data</param>
        //private void Library_OnUpdateLibraryProgress(UpdateLibraryProgressData data)
        //{                         
        //    //// Invoke UI updates
        //    //MethodInvoker methodUIUpdate = delegate
        //    //{
        //    //    // Check if there is an error
        //    //    if (data.Error != null)
        //    //    {
        //    //        //string errorMessage = "Error: Cannot add this file to the library - " + status.errorInformation + "\n\nFile Path: " + status.filePath + "\nReason: " + status.error.Message;
        //    //        string errorMessage;

        //    //        if (!String.IsNullOrEmpty(data.FilePath))
        //    //        {
        //    //            errorMessage = data.FilePath + "\n";
        //    //        }
        //    //        else
        //    //        {
        //    //            errorMessage = "====================\n";
        //    //        }

        //    //        errorMessage += "   Error: " + data.Error.Message;

        //    //        if (data.Error.InnerException != null)
        //    //        {
        //    //            errorMessage += "\n   " + data.Error.InnerException.Message;
        //    //        }
        //    //        errorMessage += "\n";

        //    //        // Log for file
        //    //        sbLog.Insert(0, errorMessage);

        //    //        // Add to log screen
        //    //        lbLog.Items.Insert(0, "Error reading " + Path.GetFileName(data.FilePath) + ": " + data.Error.Message);

        //    //        lblMessage.Text = data.Message;
        //    //        lblProgress.Text = data.Percentage.ToString("0.000") + " %";
        //    //        progressBar.Value = Convert.ToInt32(data.Percentage);
        //    //    }
        //    //    else
        //    //    {
        //    //        // Update UI                
        //    //        if (data.CurrentFilePosition > 0 && data.TotalNumberOfFiles > 0)
        //    //        {
        //    //            // Set start time when the process has finished finding the files and is ready to add files into library
        //    //            if (startTimeAddFiles == DateTime.MinValue)
        //    //            {
        //    //                startTimeAddFiles = DateTime.Now;
        //    //            }

        //    //            // Calculate time elapsed
        //    //            TimeSpan timeElapsed = DateTime.Now.Subtract(startTimeAddFiles);

        //    //            // Update title
        //    //            lblTitle.Text = data.Title + " (file " + data.CurrentFilePosition.ToString() + " of " + data.TotalNumberOfFiles.ToString() + ")";

        //    //            // Calculate time remaining
        //    //            double msPerFile = timeElapsed.TotalMilliseconds / data.CurrentFilePosition;
        //    //            double remainingTime = (data.TotalNumberOfFiles - data.CurrentFilePosition) * msPerFile;
        //    //            TimeSpan timeRemaining = new TimeSpan(0, 0, 0, 0, (int)remainingTime);

        //    //            // Update estimated time left (from more precise to more vague)
        //    //            if (timeRemaining.TotalSeconds == 0)
        //    //            {
        //    //                lblEstimatedTimeLeft.Text = "Estimated time left : N/A";
        //    //            }
        //    //            else if (timeRemaining.Minutes == 1)
        //    //            {
        //    //                lblEstimatedTimeLeft.Text = "Estimated time left : 1 minute";
        //    //            }
        //    //            else if (timeRemaining.TotalSeconds <= 10)
        //    //            {
        //    //                lblEstimatedTimeLeft.Text = "Estimated time left : A few seconds";
        //    //            }
        //    //            else if (timeRemaining.TotalSeconds <= 30)
        //    //            {
        //    //                lblEstimatedTimeLeft.Text = "Estimated time left : Less than 30 seconds";
        //    //            }
        //    //            else if (timeRemaining.TotalSeconds <= 60)
        //    //            {
        //    //                lblEstimatedTimeLeft.Text = "Estimated time left : Less than a minute";
        //    //            }
        //    //            else
        //    //            {
        //    //                lblEstimatedTimeLeft.Text = "Estimated time left : " + timeRemaining.Minutes.ToString() + " minutes";
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            lblTitle.Text = data.Title;
        //    //        }
        //    //        lblMessage.Text = data.Message;
        //    //        lblProgress.Text = data.Percentage.ToString("0.000") + " %";
        //    //        progressBar.Value = Convert.ToInt32(data.Percentage);

        //    //        if (!String.IsNullOrEmpty(data.LogEntry))
        //    //        {
        //    //            sbLog.Insert(0, data.LogEntry + "\n");
        //    //        }

        //    //        // Update song if exists
        //    //        if (data.Song != null)
        //    //        {
        //    //            //lblArtist.Text = data.Song.ArtistName;
        //    //            //lblAlbum.Text = data.Song.AlbumTitle;
        //    //            //lblSongTitle.Text = data.Song.SongTitle;

        //    //            // Check if the folder has already been treated (we want to get the ID3
        //    //            // image just once per album. This speeds up the update library process a lot.)

        //    //            ////string path = Path.GetDirectoryName(data.FilePath);
        //    //            //string artistAlbum = data.Song.ArtistName + " - " + data.Song.AlbumTitle;
        //    //            //if (!listAlbums.Contains(artistAlbum))
        //    //            //{
        //    //            //    try
        //    //            //    {
        //    //            //        // Get album art from ID3 tag or folder.jpg
        //    //            //        Image image = MPfm.Library.Library.GetAlbumArtFromID3OrFolder(data.FilePath);

        //    //            //        // If image is null...
        //    //            //        if (image == null)
        //    //            //        {
        //    //            //            // Display nothing
        //    //            //            picAlbum.Image = null;
        //    //            //        }
        //    //            //        else
        //    //            //        {
        //    //            //            // Update the album art                                
        //    //            //            picAlbum.Image = ImageManipulation.ResizeImage(image, picAlbum.Size.Width, picAlbum.Size.Height);
        //    //            //        }

        //    //            //        // Add the folder in the list of folders
        //    //            //        listAlbums.Add(artistAlbum);
        //    //            //    }
        //    //            //    catch (Exception ex)
        //    //            //    {
        //    //            //        // Do nothing since this is only album art.
        //    //            //    }
        //    //            //}

        //    //            //lbLog.Items.Insert(0, data.LogEntry);
        //    //            //if (lbLog.Items.Count > 1000)
        //    //            //{
        //    //            //    lbLog.Items.RemoveAt(lbLog.Items.Count - 1);
        //    //            //}
        //    //        }
        //    //        else
        //    //        {
        //    //            //lblArtist.Text = string.Empty;
        //    //            //lblAlbum.Text = string.Empty;
        //    //            //lblSongTitle.Text = string.Empty;
        //    //            //picAlbum.Image = null;
        //    //        }
        //    //    }
        //    //};

        //    //// Check if invoking is necessary
        //    //if (InvokeRequired)
        //    //{
        //    //    BeginInvoke(methodUIUpdate);
        //    //}
        //    //else
        //    //{
        //    //    methodUIUpdate.Invoke();
        //    //}

        //    ////if (data.ProgressUndefined)
        //    ////{
        //    ////    progressBar.Style = ProgressBarStyle.Continuous;
        //    ////}
        //    ////else
        //    ////{
        //    ////    progressBar.Style = ProgressBarStyle.Marquee;
        //    ////    lblProgress.Text = data.Percentage.ToString("0.00") + " %";
        //    ////    progressBar.Value = Convert.ToInt32(data.Percentage);
        //    ////}
        //}

        #region IUpdateLibraryView implementation

        public Action<UpdateLibraryMode, List<string>, string> OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }

        public void RefreshStatus(UpdateLibraryEntity entity)
        {
        }

        public void AddToLog(string entry)
        {
        }

        public void ProcessEnded(bool canceled)
        {
        }

        #endregion

    }
}
