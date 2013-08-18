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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Player.Events;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.BassNetWrapper;
using MPfm.Sound.PeakFiles;
using MPfm.WindowsControls;

namespace MPfm.Windows.Classes.Forms
{
    /// <summary>
    /// Main form for the MPfm application. Contains the Artist Browser, Song Browser,
    /// Current Song Panel, Playback controls, etc.
    /// </summary>
    public partial class frmMain : BaseForm, IMainView
    {
        #region Initialization

        public frmMain(Action<IBaseView> onViewReady) : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }    

        /// <summary>
        /// Fires when the main form is first loaded.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            this.Text = "MPfm: Music Player for Musicians - " + assembly.GetName().Version.ToString();

            ShowUpdateLibraryProgress(false);

            // TODO: Determine first run with InitializationService
            // TODO: When starting player, try to reuse the last configured device with DeviceHelper.FindOutputDevice
            // TODO: Apply configuration
            // TODO: Expand nodes to what was opened in the last session

            // Populate the supported formats
            comboSoundFormat.Items.Clear();
            comboSoundFormat.Items.Add("All");
            comboSoundFormat.Items.Add("APE");
            comboSoundFormat.Items.Add("FLAC");
            comboSoundFormat.Items.Add("MP3");
            comboSoundFormat.Items.Add("MPC");
            comboSoundFormat.Items.Add("OGG");
            comboSoundFormat.Items.Add("WMA");
            comboSoundFormat.Items.Add("WV");
            comboSoundFormat.SelectedIndex = 0;
            lblPeakFileWarning.Visible = false;

            //// Load other configuration options                
            //notifyIcon.Visible = Config.GetKeyValueGeneric<bool>("ShowTray").HasValue ? Config.GetKeyValueGeneric<bool>("ShowTray").Value : false;
            //EnableTooltips(showTooltips);

            //// Update peak file warning if necessary
            //RefreshPeakFileDirectorySizeWarning();
        }

        /// <summary>
        /// Loads the window configuration (window position, size, columns, etc.) from the configuration file.
        /// </summary>
        public void LoadWindowConfiguration()
        {
            //// Main window
            //WindowConfiguration windowMain = Config.Windows.Windows.FirstOrDefault(x => x.Name.ToUpper() == "MAIN");
            //if(windowMain != null)
            //{
            //    // Set size
            //    Width = windowMain.Width;
            //    Height = windowMain.Height;

            //    // Set splitter distance
            //    splitFirst.SplitterDistance = Config.GetKeyValueGeneric<int>("WindowSplitterDistance") ?? 175;

            //    // Check if the window needs to use the default position (center screen)
            //    if(windowMain.UseDefaultPosition)
            //    {
            //        // No configuration for window position; center window in screen
            //        StartPosition = FormStartPosition.CenterScreen;
            //    }
            //    else
            //    {
            //        int x = 0;
            //        int y = 0;

            //        // Make sure the window X isn't negative
            //        if (windowMain.X < 0)
            //        {
            //            x = 0;
            //        }
            //        else
            //        {
            //            x = windowMain.X;
            //        }
            //        // Make sure the window Y isn't negative
            //        if (windowMain.Y < 0)
            //        {
            //            y = 0;
            //        }
            //        else
            //        {
            //            y = windowMain.Y;
            //        }

            //        Location = new Point(x, y);
            //    }

            //    // Set maximized state
            //    if (windowMain.Maximized)
            //    {
            //        WindowState = FormWindowState.Maximized;
            //    }
            //    else
            //    {
            //        WindowState = FormWindowState.Normal;
            //    }

            //    // Load song browser column widths
            //    foreach (SongGridViewColumn column in Config.Controls.SongGridView.Columns)
            //    {
            //        SongGridViewColumn columnGrid = viewSongs2.Columns.FirstOrDefault(x => x.Title == column.Title);
            //        if (columnGrid != null)
            //        {
            //            columnGrid.Visible = column.Visible;
            //            columnGrid.Width = column.Width;
            //            columnGrid.Order = column.Order;
            //        }
            //    }
            //}           

            //// Playlist window
            //WindowConfiguration windowPlaylist = Config.Windows.Windows.FirstOrDefault(x => x.Name.ToUpper() == "PLAYLIST");            
            //if (formPlaylist != null && windowPlaylist != null)
            //{
            //    // Check if the window needs to use the default position (center screen)
            //    if (windowPlaylist.UseDefaultPosition)
            //    {
            //        // No configuration for window position; center window in screen
            //        formPlaylist.StartPosition = FormStartPosition.CenterScreen;                    
            //    }
            //    else
            //    {
            //        formPlaylist.Location = new Point(windowPlaylist.X, windowPlaylist.Y);
            //    }

            //    formPlaylist.Width = windowPlaylist.Width;
            //    formPlaylist.Height = windowPlaylist.Height;

            //    if (windowPlaylist.Maximized)
            //    {
            //        formPlaylist.WindowState = FormWindowState.Maximized;
            //    }
            //    else
            //    {
            //        formPlaylist.WindowState = FormWindowState.Normal;
            //    }

            //    formPlaylist.Visible = windowPlaylist.Visible;

            //    // Load playlist column widths                
            //    foreach (SongGridViewColumn column in Config.Controls.PlaylistGridView.Columns)
            //    {
            //        SongGridViewColumn columnGrid = formPlaylist.viewSongs2.Columns.FirstOrDefault(x => x.Title == column.Title);
            //        if (columnGrid != null)
            //        {
            //            columnGrid.Visible = column.Visible;
            //            columnGrid.Width = column.Width;
            //            columnGrid.Order = column.Order;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Saves the window configuration (window position, size, columns, etc.) into the configuration file.
        /// </summary>
        public void SaveWindowConfiguration()
        {
            //// Save window position and size
            //WindowConfiguration windowMain = Config.Windows.Windows.FirstOrDefault(x => x.Name.ToUpper() == "MAIN");
            //if(windowMain != null)
            //{
            //    // Set position and size
            //    windowMain.X = Location.X;
            //    windowMain.Y = Location.Y;
            //    windowMain.Width = Width;
            //    windowMain.Height = Height;
            //    windowMain.UseDefaultPosition = false;

            //    // Set maximized and visible
            //    bool isMaximized = false;
            //    if (WindowState == FormWindowState.Maximized)
            //    {
            //        isMaximized = true;
            //    }
            //    windowMain.Maximized = isMaximized;
            //    windowMain.Visible = true;

            //    // Set splitter position
            //    Config.SetKeyValue<int>("WindowSplitterDistance", splitFirst.SplitterDistance);

            //    // Add columns
            //    Config.Controls.SongGridView.Columns = viewSongs2.Columns;
            //}

            //// Save playlist window position and size
            //if (formPlaylist != null)
            //{
            //    WindowConfiguration windowPlaylist = Config.Windows.Windows.FirstOrDefault(x => x.Name.ToUpper() == "PLAYLIST");
            //    if (windowPlaylist != null)
            //    {
            //        windowPlaylist.X = formPlaylist.Location.X;
            //        windowPlaylist.Y = formPlaylist.Location.Y;
            //        windowPlaylist.Width = formPlaylist.Width;
            //        windowPlaylist.Height = formPlaylist.Height;
            //        windowPlaylist.Visible = formPlaylist.Visible;
            //        windowPlaylist.UseDefaultPosition = false;

            //        bool isMaximized = false;
            //        if (formPlaylist.WindowState == FormWindowState.Maximized)
            //        {
            //            isMaximized = true;
            //        }
            //        windowPlaylist.Maximized = isMaximized;

            //        // Add columns
            //        Config.Controls.PlaylistGridView.Columns = formPlaylist.viewSongs2.Columns;
            //    }
            //}

            //// Save configuration
            //Config.Save();
        }

        #endregion

        /// <summary>
        /// Occurs when the timer for the output meter is expired. This forces the
        /// output meter to refresh itself every 10ms.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void timerUpdateOutputMeter_Tick(object sender, EventArgs e)
        {            
            //// Check for valid objects
            //if (player == null || !player.IsPlaying ||
            //    player.Playlist == null || player.Playlist.CurrentItem == null || player.Playlist.CurrentItem.Channel == null)
            //{
            //    return;
            //}

            //float maxL = 0f;
            //float maxR = 0f;

            //// length of a 20ms window in bytes
            //int length20ms = (int)player.MixerChannel.Seconds2Bytes(0.02);   //(int)Bass.BASS_ChannelSeconds2Bytes(channel, 0.02);
            //// the number of 32-bit floats required (since length is in bytes!)
            //int l4 = length20ms / 4; // 32-bit = 4 bytes

            //// create a data buffer as needed
            //float[] sampleData = new float[l4];

            //int length = 0;
            //if (Player.Device.DriverType != DriverType.DirectSound)
            //{
            //    // Use the GetMixerData method instead, so we don't "steal" data from the decode buffer
            //    //length = player.MainChannel.GetMixerData(sampleData, length20ms);
            //    length = player.FXChannel.GetMixerData(sampleData, length20ms);
            //}
            //else
            //{
            //    length = player.MixerChannel.GetData(sampleData, length20ms);
            //}
            

            //// the number of 32-bit floats received
            //// as less data might be returned by BASS_ChannelGetData as requested
            //l4 = length / 4;

            //float[] left = new float[l4 / 2];
            //float[] right = new float[l4 / 2];
            //for (int a = 0; a < l4; a++)
            //{
            //    float absLevel = Math.Abs(sampleData[a]);

            //    // decide on L/R channel
            //    if (a % 2 == 0)
            //    {                    
            //        // Left channel
            //        left[a/2] = sampleData[a];
            //        if (absLevel > maxL)
            //            maxL = absLevel;
            //    }
            //    else
            //    {
            //        // Right channel
            //        right[a/2] = sampleData[a];
            //        if (absLevel > maxR)
            //            maxR = absLevel;
            //    }
            //}

            ////// limit the maximum peak levels to +6bB = 65535 = 0xFFFF
            ////// the peak levels will be int values, where 32767 = 0dB
            ////// and a float value of 1.0 also represents 0db.
            ////peakL = (int)Math.Round(32767f * maxL) & 0xFFFF;
            ////peakR = (int)Math.Round(32767f * maxR) & 0xFFFF;

            ////// convert the level to dB
            ////double dBlevelL = Base.LevelToDB_16Bit(peakL);
            ////double dBlevelR = Base.LevelToDB_16Bit(peakR);

            ////lblBitsPerSampleTitle.Text = dBlevelL.ToString("0.000");

            //outputMeter.AddWaveDataBlock(left, right);
            //outputMeter.Refresh();

            //// Get min max info from wave block
            //if (AudioTools.CheckForDistortion(left, right, true, -3.0f))
            //{
            //    // Show distortion warning "LED"
            //    //picDistortionWarning.Visible = true;
            //}
        }

        /// <summary>
        /// Occurs when the user tries to close the form, using the X button or the
        /// Close button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (e.CloseReason == CloseReason.FormOwnerClosing ||
            //    e.CloseReason == CloseReason.UserClosing)
            //{
            //    // Check configuration values
            //    if (Config.GetKeyValueGeneric<bool>("ShowTray") == true &&
            //        Config.GetKeyValueGeneric<bool>("HideTray") == true)
            //    {
            //        e.Cancel = true;
            //        this.Hide();
            //        return;
            //    }
            //}

            //// Save configuration
            //SaveWindowConfiguration();
            e.Cancel = false;

            //// Close player if not null
            //if (player != null)
            //{
            //    // Stop playback if necessary
            //    if (player.IsPlaying)
            //    {
            //        // Stop playback
            //        player.Stop();
            //    }

            //    // Check if a wave form is generating
            //    if (waveFormMarkersLoops.IsLoading)
            //    {
            //        // Cancel loading
            //        waveFormMarkersLoops.CancelWaveFormLoading();
            //    }

            //    // Release the sound system from memory
            //    player.Dispose();
            //}
        }
        
        /// <summary>
        /// Saves the configuration and exits the application.
        /// </summary>
        public void ExitApplication()
        {
            SaveWindowConfiguration();
            Application.Exit();
        }

        #region Main Menu Events

        /// <summary>
        /// Occurs when the user clicks on the "File -> Add file(s) to library" menu item.
        /// Pops an open file dialog to let the user select the file(s) to add.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miFileAddFiles_Click(object sender, EventArgs e)
        {
            if (dialogAddFiles.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            OnAddFilesToLibrary(dialogAddFiles.FileNames.ToList());
        }

        /// <summary>
        /// Occurs when the user clicks on the "File -> Add folder to library" menu item.
        /// Pops an open folder dialog to let the user select the folder to add.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miFileAddFolder_Click(object sender, EventArgs e)
        {
            if (dialogAddFolder.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            OnAddFolderToLibrary(dialogAddFolder.SelectedPath);
        }

        /// <summary>
        /// Occurs when the user clicks on the "File -> Open an audio files" menu item.
        /// Pops an open file dialog to let the user select the file to play.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miFileOpenAudioFiles_Click(object sender, EventArgs e)
        {
            if (dialogOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            //// Declare variables
            //Dictionary<string, string> failedAudioFiles = new Dictionary<string, string>();

            //// Remove play icon on song browser and playlist
            //RefreshSongBrowserPlayIcon(Guid.Empty);
            //formPlaylist.RefreshPlaylistPlayIcon(Guid.Empty);

            //// Determine if this is a playlist file or an audio file
            //if (dialogOpenFile.FileName.ToUpper().Contains(".M3U") ||
            //    dialogOpenFile.FileName.ToUpper().Contains(".M3U8") ||
            //    dialogOpenFile.FileName.ToUpper().Contains(".PLS") ||
            //    dialogOpenFile.FileName.ToUpper().Contains(".XSPF"))
            //{
            //    // Load playlist
            //    formPlaylist.LoadPlaylist(dialogOpenFile.FileName);
            //}
            //else
            //{
            //    // Load audio files
            //    List<AudioFile> audioFiles = new List<AudioFile>();                
            //    foreach (string fileName in dialogOpenFile.FileNames)
            //    {
            //        try
            //        {
            //            // Create audio file and add to list
            //            AudioFile audioFile = new AudioFile(fileName);
            //            audioFiles.Add(audioFile);
            //        }
            //        catch(Exception ex)
            //        {
            //            // Add to list of failed playback files
            //            failedAudioFiles.Add(fileName, ex.Message);
            //        }
            //    }

            //    // Check if there is at least one file to load
            //    if (audioFiles.Count == 0)
            //    {
            //        // Build list
            //        StringBuilder sbMessage = new StringBuilder();
            //        sbMessage.AppendLine("Error: The playlist is empty. No files could not be loaded:\n");
            //        foreach (KeyValuePair<string, string> fileName in failedAudioFiles)
            //        {
            //            sbMessage.AppendLine(fileName.Key + "\nReason: " + fileName.Value);
            //        }

            //        // Display message box
            //        MessageBox.Show(sbMessage.ToString(), "Error loading files", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }

            //    // Add items to playlist
            //    Player.Playlist.Clear();
            //    //Player.Playlist.AddItems(dialogOpenFile.FileNames.ToList());
            //    Player.Playlist.AddItems(audioFiles);
            //    Player.Playlist.First();
            //}

            //// Display the list of failed songs to the user
            //if (failedAudioFiles.Count > 0)
            //{
            //    // Build list
            //    StringBuilder sbMessage = new StringBuilder();
            //    sbMessage.AppendLine("Some files could not be loaded:\n");
            //    foreach (KeyValuePair<string, string> fileName in failedAudioFiles)
            //    {
            //        sbMessage.AppendLine(fileName.Key +"\nReason: " + fileName.Value);
            //    }

            //    // Display warning
            //    MessageBox.Show(sbMessage.ToString(), "Some files could not be loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }

        /// <summary>
        /// Occurs when the user clicks on the "File -> Exit" menu item. This
        /// exits the application.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miFileExit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> View Help Contents..." menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpHelp_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "MPfm_User_Manual.chm");
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> View License..." menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpLicense_Click(object sender, EventArgs e)
        {
            try
            {
                // Display license in favorite text editor
                Process.Start("license.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace, "Error opening license file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> Go to: MPfm Web Site..." menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpWebsite_Click(object sender, EventArgs e)
        {
            try
            {
                // Open website in default browser
                Process.Start("http://www.mp4m.org");                        
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace, "Error opening web browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> Go to: MPfm Blog..." menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpBlog_Click(object sender, EventArgs e)
        {
            try
            {
                // Open website in default browser
                Process.Start("http://www.mp4m.org/blog");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace, "Error opening web browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> Go to: MPfm SourceForge Home Page..." menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpSourceForge_Click(object sender, EventArgs e)
        {
            try
            {
                // Open website in default browser
                Process.Start("https://sourceforge.net/projects/mp4m/");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace, "Error opening web browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> Download latest version of MPfm directly from SourceForge..." menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpDownload_Click(object sender, EventArgs e)
        {
            try
            {
                // Open website in default browser
                Process.Start("https://sourceforge.net/projects/mp4m/files/latest/download");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace, "Error opening web browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> Go to: MPfm Bug Tracker - Roadmap..." menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpRoadmap_Click(object sender, EventArgs e)
        {
            try
            {
                // Open website in default browser
                Process.Start("http://www.mp4m.org/mantis/roadmap_page.php");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace, "Error opening web browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> Go to: MPfm Bug Tracker - Change Log..." menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpChangeLog_Click(object sender, EventArgs e)
        {
            try
            {
                // Open website in default browser
                Process.Start("http://www.mp4m.org/mantis/changelog_page.php");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace, "Error opening web browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> Go to: MPfm Bug Tracker - Report a new bug..." menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpReportBug_Click(object sender, EventArgs e)
        {
            try
            {
                // Open website in default browser
                if (MessageBox.Show("Thank you for taking the time to report a bug. It is truly appreciated.\n\nTo report a bug in the Mantis bug tracker, you need to login or register a new account. You can only submit bugs in the MPfm/Support project.\n\nFor more information, consult this web page: http://www.mp4m.org/support.", "Report a new bug", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
                    Process.Start("http://www.mp4m.org/mantis/login_page.php");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace, "Error opening web browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Help -> About" menu item.
        /// This displays the "About" screen.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miHelpAbout_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region Main Toolbar Events

        /// <summary>
        /// Occurs when the user clicks on the "Update Library" button on the main form toolbar.
        /// Displays the Update Library Status window and updates the whole library.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnUpdateLibrary_Click(object sender, EventArgs e)
        {
            OnUpdateLibrary();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Play" button on the main form toolbar.
        /// Plays the selected song query in the Song Browser.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPlay_Click(object sender, EventArgs e)
        {
            OnPlayerPause();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Next song" button on the main form toolbar.
        /// Skips to the next song in the playlist.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnNextSong_Click(object sender, EventArgs e)
        {
            OnPlayerNext();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Previous song" button on the main form toolbar.
        /// Skips to the previous song in the playlist.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPreviousSong_Click(object sender, EventArgs e)
        {
            OnPlayerPrevious();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Repeat Type" button on the main form toolbar.
        /// Iterates through the different repeat types.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnRepeat_Click(object sender, EventArgs e)
        {
            //// Cycle through the repeat types
            //if (player.RepeatType == RepeatType.Off)
            //{
            //    player.RepeatType = RepeatType.Playlist;
            //}
            //else if (player.RepeatType == RepeatType.Playlist)
            //{
            //    player.RepeatType = RepeatType.Song;
            //}
            //else
            //{
            //    player.RepeatType = RepeatType.Off;
            //}

            //// Update repeat button
            //RefreshRepeatButton();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Shuffle" button on the main form toolbar.
        /// Enables of disables shuffling.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnShuffle_Click(object sender, EventArgs e)
        {
            // Set button check
            btnShuffle.Checked = !btnShuffle.Checked;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Playlist" button on the main form toolbar.
        /// Opens or closes the Playlist window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPlaylist_Click(object sender, EventArgs e)
        {
            OnOpenPlaylistWindow();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Effects" button on the main form toolbar.
        /// Opens or closes the Effects window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnEffects_Click(object sender, EventArgs e)
        {
            OnOpenEffectsWindow();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Visualizer" button on the main form toolbar.
        /// Opens or closes the Visualizer window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnVisualizer_Click(object sender, EventArgs e)
        {
            //if (formVisualizer.Visible)
            //{
            //    formVisualizer.Close();
            //    btnVisualizer.Checked = false;
            //}
            //else
            //{
            //    formVisualizer.Show(this);
            //    btnVisualizer.Checked = true;
            //}
        }

        /// <summary>
        /// Occurs when the user clicks on the "Themes" button on the main form toolbar.
        /// Opens or closes the Themes window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnThemes_Click(object sender, EventArgs e)
        {
            //if (formThemes.Visible)
            //{
            //    formThemes.Close();
            //    btnThemes.Checked = false;
            //}
            //else
            //{
            //    formThemes.Show(this);
            //    btnThemes.Checked = true;
            //}
        }

        /// <summary>
        /// Occurs when the user clicks on the "Settings" button on the main form toolbar.
        /// Opens or closes the Settings window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSettings_Click(object sender, EventArgs e)
        {
            OnOpenPreferencesWindow();
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            OnOpenSyncWindow();
        }

        #endregion

        #region Refresh Methods

        /// <summary>
        /// Refreshes the peak file directory size warning.
        /// </summary>
        public void RefreshPeakFileDirectorySizeWarning()
        {
            //// Get configuration
            //int? peakFileDisplayWarningThreshold = Config.GetKeyValueGeneric<int>("PeakFile_DisplayWarningThreshold");
            //bool? peakFileDisplayWarning = Config.GetKeyValueGeneric<bool>("PeakFile_DisplayWarning");

            //// Check if the warning needs to be displayed
            //if (peakFileDisplayWarning.HasValue && peakFileDisplayWarning.Value && peakFileDisplayWarningThreshold.HasValue)
            //{
            //    // Check peak file directory size
            //    long size = PeakFileService.CheckDirectorySize(peakFileFolderPath);

            //    // Check free space
            //    long freeSpace = 0;
            //    try
            //    {
            //        // Get drive info
            //        DriveInfo driveInfo = new DriveInfo(peakFileFolderPath[0] + ":");
            //        freeSpace = driveInfo.AvailableFreeSpace;
            //    }
            //    catch
            //    {
            //        // Set value to indicate this has failed
            //        freeSpace = -1;
            //    }

            //    // Compare size with threshold
            //    if (size > peakFileDisplayWarningThreshold.Value * 1000000)
            //    {
            //        // Display warning
            //        lblPeakFileWarning.Text = "Warning: The peak file directory size exceeds " + peakFileDisplayWarningThreshold.Value.ToString() + " MB. Total peak file directory size: " + ((float)size / 1000000).ToString("0") + " MB. Free space on disk: " + ((float)freeSpace / 1000000).ToString("0") + " MB.";
            //        lblPeakFileWarning.Visible = true;
            //    }
            //    else
            //    {
            //        // Hide warning
            //        lblPeakFileWarning.Visible = false;
            //    }
            //}
            //else
            //{
            //    // Hide warning
            //    lblPeakFileWarning.Visible = false;
            //}
        }

        /// <summary>
        /// Refreshes the controls in the "Current Song" panel and the different
        /// playback buttons. By default, does not update the playlist window.
        /// </summary>
        public void RefreshSongControls()
        {
            RefreshSongControls(false);
        }

        /// <summary>
        /// Refreshes the controls in the "Current Song" panel and the different
        /// playback buttons. Can update the playlist window or not.
        /// </summary>
        public void RefreshSongControls(bool refreshPlaylistWindow)
        {
            //// Is the player playing?
            //if (Player.IsPlaying)
            //{
            //    // Enable/disable buttons
            //    btnStop.Enabled = true;
            //    btnPause.Enabled = true;
            //    btnPlay.Enabled = false;
            //    btnNextSong.Enabled = (Player.Playlist.CurrentItemIndex < Player.Playlist.Items.Count - 1) ? true : false;
            //    btnPreviousSong.Enabled = (Player.Playlist.CurrentItemIndex == 0) ? false : true;
            //    btnPause.Checked = false;
            //    trackPosition.Enabled = true;
            //    trackTimeShifting.Enabled = true;

            //    // Set the play icon in the song browser                
            //    RefreshSongBrowserPlayIcon(Player.Playlist.CurrentItem.AudioFile.Id);

            //    // Refresh playlist window
            //    if (refreshPlaylistWindow)
            //    {
            //        formPlaylist.RefreshPlaylist();
            //    }

            //    // Refresh playlist icon
            //    formPlaylist.RefreshPlaylistPlayIcon(Player.Playlist.CurrentItem.Id);
            //}
            //else
            //{
            //    // Enable/disable buttons
            //    btnStop.Enabled = false;
            //    btnPause.Enabled = false;
            //    btnNextSong.Enabled = false;
            //    btnPreviousSong.Enabled = false;
            //    btnPlay.Enabled = true;
            //    trackPosition.Enabled = false;
            //    trackTimeShifting.Enabled = false;                

            //    // Nothing is playing, then display "stop" song information                
            //    lblCurrentArtistName.Text = string.Empty;
            //    lblCurrentAlbumTitle.Text = string.Empty;
            //    lblCurrentSongTitle.Text = string.Empty;
            //    lblCurrentFilePath.Text = string.Empty;
            //    lblCurrentPosition.Text = "0:00.000";
            //    lblLength.Text = "0:00.000";
            //    lblSoundFormat.Text = string.Empty;
            //    lblBitsPerSample.Text = string.Empty;
            //    lblBitrate.Text = string.Empty;
            //    lblFrequency.Text = string.Empty;
            //    picAlbum.Image = null;
            //    btnPause.Checked = false;
            //    lblSongPosition.Text = "0:00.000";
            //    lblSongPercentage.Text = "0 %";
            //    trackPosition.Value = 0;

            //    // Empty output meter history and refresh
            //    outputMeter.WaveDataHistory.Clear();
            //    outputMeter.Refresh();

            //    // Refresh play icon
            //    RefreshSongBrowserPlayIcon(Guid.Empty);
            //    formPlaylist.RefreshPlaylistPlayIcon(Guid.Empty);
            //    viewSongs2.ClearSelectedItems();
            //}
        }

        /// <summary>
        /// Refreshes the Markers grid view.
        /// </summary>
        public void RefreshMarkers()
        {
            //// Clear items
            //viewMarkers.Items.Clear();

            //// Set marker buttons
            //btnEditMarker.Enabled = false;
            //btnRemoveMarker.Enabled = false;
            //btnGoToMarker.Enabled = false;

            //// Check if a song is currently playing
            //if (!Player.IsPlaying)
            //{
            //    // Reset buttons
            //    btnAddMarker.Enabled = false;
            //    return;
            //}

            //// Set button
            //btnAddMarker.Enabled = true;

            //// Fetch markers from database
            //List<Marker> markers = Library.Facade.SelectMarkers(Player.Playlist.CurrentItem.AudioFile.Id);

            //// Update grid view
            //foreach (Marker marker in markers)
            //{
            //    // Create grid view item
            //    ListViewItem item = viewMarkers.Items.Add(marker.Name);
            //    item.Tag = marker.MarkerId;
            //    item.SubItems.Add(marker.Position);
            //    item.SubItems.Add(marker.Comments);
            //    item.SubItems.Add(marker.PositionBytes.ToString());
            //}
        }

        /// <summary>
        /// Refreshes the Loops grid view.
        /// </summary>
        public void RefreshLoops()
        {
            //// Clear items
            //viewLoops.Items.Clear();

            //// Set buttons
            //btnEditLoop.Enabled = false;
            //btnRemoveLoop.Enabled = false;
            //btnPlayLoop.Enabled = false;
            //btnStopLoop.Enabled = false;

            //// Check if a song is currently playing
            //if (!Player.IsPlaying)
            //{
            //    // Reset buttons
            //    btnAddLoop.Enabled = false;
            //    return;
            //}

            //// Set button
            //btnAddLoop.Enabled = true;

            //// Fetch loops from database
            //List<Loop> loops = Library.Facade.SelectLoops(Player.Playlist.CurrentItem.AudioFile.Id);
            //List<Marker> markers = Library.Facade.SelectMarkers(Player.Playlist.CurrentItem.AudioFile.Id);

            //// Update grid view
            //foreach (Loop loop in loops)
            //{
            //    // Create grid view item
            //    ListViewItem item = viewLoops.Items.Add("");
            //    item.Tag = loop.LoopId;
            //    item.SubItems.Add(loop.Name);
            //    item.SubItems.Add(loop.Length);

            //    // Update marker subitems
            //    item.SubItems.Add(loop.StartPosition);
            //    item.SubItems.Add(loop.EndPosition);

            //    // Check if this is the currently playing loop
            //    if (Player.Loop != null && Player.Loop.Name == loop.Name)
            //    {
            //        item.ImageIndex = 7;
            //    }
            //}
        }

        /// <summary>
        /// Refreshes the "Repeat" button in the main form toolbar.
        /// </summary>
        public void RefreshRepeatButton()
        {
            //string repeatOff = "Repeat (Off)";
            //string repeatPlaylist = "Repeat (Playlist)";
            //string repeatSong = "Repeat (Song)";

            //// Display the repeat type
            //if (player.RepeatType == RepeatType.Playlist)
            //{
            //    btnRepeat.Text = repeatPlaylist;
            //    btnRepeat.Checked = true;

            //    miTrayRepeat.Text = repeatPlaylist;
            //    miTrayRepeat.Checked = true;
            //}
            //else if (player.RepeatType == RepeatType.Song)
            //{
            //    btnRepeat.Text = repeatSong;
            //    btnRepeat.Checked = true;

            //    miTrayRepeat.Text = repeatSong;
            //    miTrayRepeat.Checked = true;
            //}
            //else
            //{
            //    btnRepeat.Text = repeatOff;
            //    btnRepeat.Checked = false;

            //    miTrayRepeat.Text = repeatOff;
            //    miTrayRepeat.Checked = false;
            //}
        }

        #endregion

        #region Current Song Panel Events

        /// <summary>
        /// Occurs when the user holds a mouse button on the Song Position trackbar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackPosition_MouseDown(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Occurs when the user releases a mouse button on the Song Position trackbar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackPosition_MouseUp(object sender, MouseEventArgs e)
        {
            //try
            //{
            //    // Get ratio and set position
            //    double ratio = (double)trackPosition.Value / 1000;

            //    // Get length
            //    int positionBytes = (int)(ratio * (double)player.Playlist.CurrentItem.LengthBytes);
            //    long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
            //    long positionMS = ConvertAudio.ToMS(positionSamples, (uint)player.Playlist.CurrentItem.AudioFile.SampleRate);

            //    // Set player position
            //    player.SetPosition(positionBytes);

            //    // Set UI
            //    lblSongPosition.Text = Conversion.MillisecondsToTimeString((ulong)positionMS);                
            //    lblSongPercentage.Text = (ratio * 100).ToString("00.00");

            //    // Set flags
            //    songPositionChanging = false;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message.ToString());
            //}
        }

        /// <summary>
        /// Occurs when the mouse cursor moves on the Song Position trackbar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackPosition_MouseMove(object sender, MouseEventArgs e)
        {
            //// Get ratio
            //double ratio = (double)trackPosition.Value / 1000;

            //// Check if the left mouse button was clicked
            //if (e.Button == MouseButtons.Left)
            //{
            //    // Get time                
            //    lblSongPosition.Text = Conversion.MillisecondsToTimeString(Convert.ToUInt32((ratio * (double)player.Playlist.CurrentItem.LengthMilliseconds)));
            //    lblSongPercentage.Text = (ratio * 100).ToString("0.00") + " %";
            //}
        }

        /// <summary>
        /// Fires when the user scrolls the time shifting slider.
        /// </summary>
        private void trackTimeShiftingNew_OnTrackBarValueChanged()
        {
            //double multiplier = 1 / ((double)trackTimeShifting.Value / 100);
            //lblTimeShifting.Text = trackTimeShifting.Value.ToString() + " %";
            //Player.TimeShifting = trackTimeShifting.Value;
        }

        /// <summary>
        /// Fires when the user releases the mouse button on the Volume slider. Saves the final value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackVolume_MouseUp(object sender, MouseEventArgs e)
        {
            //Config["Volume"] = trackVolume.Value.ToString();
            //Config.Audio.Mixer.Volume = faderVolume.Value;
        }

        /// <summary>
        /// This button resets the time shifting value to 0%.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void linkResetTimeShifting_Click(object sender, EventArgs e)
        {
            trackTimeShifting.Value = 0;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Edit Song Metadata" link in the "Actions" panel.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>        
        private void linkEditSongMetadata_Click(object sender, EventArgs e)
        {
            //// Check for null
            //if (!Player.IsPlaying)
            //{
            //    return;
            //}

            //// Open window
            //EditSongMetadata(Player.Playlist.CurrentItem.AudioFile.FilePath);
        }

        /// <summary>
        /// Fires when the volume slider value has changed. Updates the player volume.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void faderVolume_OnFaderValueChanged(object sender, EventArgs e)
        {
            //// Check if the form has finished loading
            //if (!IsInitDone)
            //{
            //    return;
            //}

            //// Set volume and update label            
            //player.Volume = (float)faderVolume.Value / 100;
            //lblVolume.Text = faderVolume.Value.ToString() + " %";
            //Config.Audio.Mixer.Volume = faderVolume.Value;
            //Config.Save();
        }

        /// <summary>
        /// Occurs when the user clicks on the distortion warning "LED". This clears
        /// the distortion warning and hides the control.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void picDistortionWarning_Click(object sender, EventArgs e)
        {
            // Hide distortion warning
            picDistortionWarning.Visible = false;
        }

        #region Search Links

        /// <summary>
        /// Occurs when the user clicks on the "Guitar tabs" link in the "Actions" panel.
        /// Opens the default browser and searches for guitar tabs featuring the current song.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>        
        private void linkSearchGuitarTabs_Click(object sender, EventArgs e)
        {
            //// Make sure the player is playing
            //if (Player != null && Player.IsPlaying)
            //{
            //    Process.Start("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.Title) + "+guitar+tab");
            //}
        }

        /// <summary>
        /// Occurs when the user clicks on the "Bass tabs" link in the "Actions" panel.
        /// Opens the default browser and searches for bass tabs featuring the current song.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>        
        private void linkSearchBassTabs_Click(object sender, EventArgs e)
        {
            //// Make sure the player is playing
            //if (Player != null && Player.IsPlaying)
            //{
            //    Process.Start("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.Title) + "+bass+tab");
            //}
        }

        /// <summary>
        /// Occurs when the user clicks on the "Lyrics" link in the "Actions" panel.
        /// Opens the default browser and searches for lyrics featuring the current song.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>6
        private void linkSearchLyrics_Click(object sender, EventArgs e)
        {
            //// Make sure the player is playing
            //if (Player != null && Player.IsPlaying)
            //{
            //    Process.Start("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.Title) + "+lyrics");
            //}
        }

        /// <summary>
        /// Occurs when the user clicks on the album art picture box in the "Current Song" panel.
        /// Opens the default browser and searches for album art featuring the current album.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void picAlbum_MouseClick(object sender, MouseEventArgs e)
        {
            //// Make sure the player is playing
            //if (Player != null && Player.IsPlaying)
            //{
            //    Process.Start("http://www.google.ca/imghp?q=" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.AlbumTitle));
            //}
        }

        #endregion  

        #endregion

        #region Song Browser Events

        /// <summary>
        /// Occurs when the user right clicks on an item of the Song Browser.
        /// Opens up a contextual menu if at least an item is selected.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void menuSongBrowser_Opening(object sender, CancelEventArgs e)
        {
            if (viewSongs2.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        /// <summary>
        /// Occurs when the user changes the selection on the Song Browser.
        /// </summary>
        /// <param name="data">Event data</param>
        private void viewSongs2_OnSelectedIndexChanged(SongGridViewSelectedIndexChangedData data)
        {
            // Check if a selection has been made
            bool enabled = viewSongs2.SelectedItems.Count != 0;

            // Set buttons
            if (btnPlaySelectedSong.Enabled != enabled)
                btnPlaySelectedSong.Enabled = enabled;
            if (btnEditSongMetadata.Enabled != enabled)
                btnEditSongMetadata.Enabled = enabled;
            if (btnAddSongToPlaylist.Enabled != enabled)
                btnAddSongToPlaylist.Enabled = enabled;

            // Set selected song in config
            if (viewSongs2.SelectedItems.Count > 0)
            {                
                //Config.Controls.SongGridView.Query.AudioFileId = viewSongs2.SelectedItems[0].AudioFile.Id;
            }
        }

        /// <summary>
        /// Occurs when the user double clicks on an item of the Song Browser.
        /// Plays the selected song query.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void viewSongs2_DoubleClick(object sender, EventArgs e)
        {
            if (viewSongs2.SelectedItems.Count == 0)
                return;

            OnTableRowDoubleClicked(viewSongs2.SelectedItems[0].AudioFile);
        }

        /// <summary>
        /// Occurs when the user clicks on the "Edit Song Metadata" button on the Song Browser toolbar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnEditSongMetadata_Click(object sender, EventArgs e)
        {
            //// Check if at least one item is selected
            //if (viewSongs2.SelectedItems.Count == 0)
            //    return;

            //// Get audio file from item metadata (check for null)
            //AudioFile audioFile = viewSongs2.SelectedItems[0].AudioFile;
            //if (audioFile == null)
            //    return;

            //// Open window
            //EditSongMetadata(audioFile.FilePath);
        }

        #endregion

        #region Artist/Album Browser Events (Tree Library)

        /// <summary>
        /// Occurs when the user clicks on the expand button of a tree node.
        /// This fires a background worker to fetch the data needed for displaying the
        /// artists, albums, playlists, etc.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void treeLibrary_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            Console.WriteLine("frmMain - treeLibrary_BeforeExpand");
            if (e == null || e.Node == null)
                return;

            var entity = (LibraryBrowserEntity) e.Node.Tag;
            OnTreeNodeExpanded(entity, e.Node);
        }

        /// <summary>
        /// Occurs just before the user tries to select an item in the Artist/Album browser.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void treeLibrary_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
            if (e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.Unknown)
                e.Cancel = false;
        }

        /// <summary>
        /// Occurs just after the user selected an item in the Artist/Album browser.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void treeLibrary_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Console.WriteLine("frmMain - treeLibrary_AfterSelect");
            if (e == null || e.Node == null)
                return;
            
            if (e.Action == TreeViewAction.ByMouse)
            {
                var entity = (LibraryBrowserEntity)e.Node.Tag;
                OnTreeNodeSelected(entity);
            }
        }

        /// <summary>
        /// Occurs when the user clicks on a node on the Artist/Album browser.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void treeLibrary_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                treeLibraryBrowser.SelectedNode = e.Node;
        }

        /// <summary>
        /// Occurs when the user double clicks on a node on the Artist/Album browser.
        /// Plays the songs from the artist, album or playlist from the item metadata.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void treeLibrary_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Node == null)
                return;

            // Cast metadata
            //TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)e.Node.Tag;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Play songs" menu item of the Artist/Album browser contextual menu.
        /// Plays the songs from the artist, album or playlist from the item metadata.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miTreeLibraryPlaySongs_Click(object sender, EventArgs e)
        {
            
        }  

        #endregion

        #region Background worker for Album Art

        /// <summary>
        /// Occurs when the background worker for fetching the album art is starting its work.
        /// Gets the album art from the ID3 tags, or folder.jpg, and converts it to a smooth
        /// resized image.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void workerAlbumArt_DoWork(object sender, DoWorkEventArgs e)
        {
            string songPath = (string)e.Argument;

            // Get image from library
            Image image = MPfm.Library.Library.GetAlbumArtFromID3OrFolder(songPath);

            // Resize image with quality AA
            if (image != null)
                image = ImageManipulation.ResizeImage(image, picAlbum.Size.Width, picAlbum.Size.Height);

            e.Result = image;
        }

        /// <summary>
        /// Occurs when the background worker for fetching the album art has finished its work.
        /// Updates the picture box with the album art.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void workerAlbumArt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate {
                Image image = (Image)e.Result;
                picAlbum.Image = image ?? null;
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
        }

        #endregion

        #region System Tray Management

        /// <summary>
        /// Occurs when the user double clicks on the application tray icon.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowApplication();
        }

        /// <summary>
        /// Shows the application or minimizes the application to the tray.
        /// </summary>
        private void ShowApplication()
        {
            if (!Visible)
            {
                Show();
                Focus();
            }
            else
            {
                if (WindowState == FormWindowState.Minimized)
                    WindowState = FormWindowState.Normal;

                TopMost = true;
                TopMost = false;
                Focus();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Show MPfm" menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miTrayShowApplication_Click(object sender, EventArgs e)
        {
            ShowApplication();
        }

        #endregion

        /// <summary>
        /// Occurs when the user right clicks on an item of the Artist/Album browser.
        /// Opens up a contextual menu if the user right clicked on a valid item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void menuLibrary_Opening(object sender, CancelEventArgs e)
        {
            //// Get the point where the mouse cursor is
            //Point pt = treeLibraryBrowser.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));

            //// Get the tree node at that point
            //TreeNode node = treeLibraryBrowser.GetNodeAt(pt.X, pt.Y);

            //// Is the node null?
            //if (node == null)
            //{
            //    // Set cancel flag
            //    e.Cancel = true;
            //    return;
            //}

            //// Get metadata
            //TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)node.Tag;
            //if (metadata != null)
            //{
            //    // Check the node type
            //    if (metadata.NodeType == TreeLibraryNodeType.Playlist)
            //    {
            //        // Show Delete playlist option
            //        miTreeLibraryDeletePlaylist.Visible = true;
            //    }
            //    else
            //    {
            //        miTreeLibraryDeletePlaylist.Visible = false;
            //    }
            //}

            //// Set metadata for later
            //miTreeLibraryDeletePlaylist.Tag = metadata;
        }

        /// <summary>
        /// Occurs when the user changes the sound format filter using the Sound Format combobox.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void comboSoundFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            //// Get audio file format
            //AudioFileFormat audioFileFormat = AudioFileFormat.Unknown;
            //Enum.TryParse<AudioFileFormat>(comboSoundFormat.Text, out audioFileFormat);

            //// Set filter
            //filterAudioFileFormat = audioFileFormat;
                        
            //// Reset Library Browser tree view selection and Song Browser query
            //treeLibraryBrowser.SelectedNode = null;
            //querySongBrowser = new SongQuery();

            //// Check if init is done
            //if (IsInitDone)
            //{
            //    // Set configuration                
            //    Config.SetKeyValue("FilterSoundFormat", audioFileFormat.ToString());                

            //    // Refresh all controls
            //    RefreshAll();
            //}
        }

        /// <summary>
        /// Occurs when the user clicks on the "Add songs to playlist" menu item of
        /// the Library contextual menu.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miTreeLibraryAddSongsToPlaylist_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Occurs when the user types something in the txtSearch textbox. Disables the Search
        /// button when the search query is empty.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Refresh song browser
            //RefreshSongBrowser();
        }

        /// <summary>
        /// Occurs when the user clicks on the Add songs to playlist button or menu item in the Song Browser.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnAddSongToPlaylist_Click(object sender, EventArgs e)
        {
            //// Loop through selected items
            //for (int a = 0; a < viewSongs2.SelectedItems.Count; a++)
            //{
            //    // Get the song from the tag of the item
            //    AudioFile audioFile = viewSongs2.SelectedItems[a].AudioFile;

            //    // Check for null
            //    if (audioFile != null)
            //    {
            //        // Add to playlist                    
            //        player.Playlist.AddItem(audioFile.FilePath);
            //    }
            //}

            //// Refresh playlists (if there was at least one selected item)
            //if (viewSongs2.SelectedItems.Count > 0)
            //{
            //    formPlaylist.RefreshPlaylist();
            //}
        }  

        private void panelLoopsMarkers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (e.Button != System.Windows.Forms.MouseButtons.Left)
            //{
            //    return;
            //}

            //if (splitLoopsMarkersSongBrowser.SplitterDistance != 22)
            //{
            //    splitLoopsMarkersSongBrowser.SplitterDistance = 22;
            //}
            //else
            //{
            //    splitLoopsMarkersSongBrowser.SplitterDistance = panelLoopsMarkers.Height;
            //}
        }

        /// <summary>
        /// Occurs when the user has clicked on the Markers and Loops Wave Form Display control and
        /// wants to skip to a specific position of the song.
        /// </summary>
        /// <param name="data">Event Data</param>
        private void waveFormMarkersLoops_OnPositionChanged(PositionChangedData data)
        {
            //// Check if data is valid
            //if (data == null)
            //{
            //    return;
            //}            

            //// Stop timer for updating position
            ////m_timerSongPosition.Stop();

            //// Set new position
            //player.SetPosition(data.Percentage);

            ////m_timerSongPosition.Start();

            ////// Set new position
            ////uint newPosition = Player.SetPositionSentenceMS(data.Percentage);

            ////// Update song position
            ////string time = Conversion.MillisecondsToTimeString(Convert.ToUInt32((data.Percentage * (double)Player.currentSongLength) / 100));
            ////lblSongPosition.Text = time;
            ////lblSongPercentage.Text = newPosition.ToString();
        }

        #region Markers Button and GridView Events
        
        /// <summary>
        /// Occurs when the user has clicked on the Add Marker button.
        /// Opens the Add/Edit Marker window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnAddMarker_Click(object sender, EventArgs e)
        {
            //// Check if the wave data is loaded
            //if (waveFormMarkersLoops.WaveDataHistory.Count > 0)
            //{
            //    // Create window and show as dialog                
            //    formAddEditMarker = new frmAddEditMarker(this, AddEditMarkerWindowMode.Add, Player.Playlist.CurrentItem.AudioFile, Guid.Empty);
            //    formAddEditMarker.toolTip.Active = Config.GetKeyValueGeneric<bool>("ShowTooltips").HasValue ? Config.GetKeyValueGeneric<bool>("ShowTooltips").Value : true;
            //    formAddEditMarker.ShowDialog(this);
            //}
        }

        /// <summary>
        /// Occurs when the user has clicked on the Edit Marker button.
        /// Opens the Add/Edit Marker window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnEditMarker_Click(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewMarkers.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Get selected markerId
            //Guid markerId = new Guid(viewMarkers.SelectedItems[0].Tag.ToString());

            //// Create window and show as dialog
            //formAddEditMarker = new frmAddEditMarker(this, AddEditMarkerWindowMode.Edit, Player.Playlist.CurrentItem.AudioFile, markerId);
            //formAddEditMarker.ShowDialog(this);
        }

        /// <summary>
        /// Occurs when the user has clicked on the Remove Marker button.
        /// Confirms with the user the deletion of the marker.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnRemoveMarker_Click(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewMarkers.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Confirm with the user
            //if (MessageBox.Show("Are you sure you wish to remove the '" + viewMarkers.SelectedItems[0].Text + "' marker?", "Remove marker confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            //{
            //    // Get selected markerId
            //    Guid markerId = new Guid(viewMarkers.SelectedItems[0].Tag.ToString());

            //    // Remove marker and refresh list                
            //    Library.Facade.DeleteMarker(markerId);
            //    RefreshMarkers();
            //}
        }

        /// <summary>
        /// Occurs when the user has clicked on the Go To Marker button.
        /// Sets the player position as the currently selecter marker position.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnGoToMarker_Click(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewMarkers.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Get selected markerId
            //Guid markerId = new Guid(viewMarkers.SelectedItems[0].Tag.ToString());

            //// Get PCM position
            //uint position = 0;
            //uint.TryParse(viewMarkers.SelectedItems[0].SubItems[3].Text, out position);

            //// Set player position            
            //player.SetPosition(position);
        }

        /// <summary>
        /// Occurs when the user double-clicks on the Marker grid view.
        /// Sets the player position as the currently selecter marker position.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void viewMarkers_DoubleClick(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewMarkers.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Get selected markerId
            //Guid markerId = new Guid(viewMarkers.SelectedItems[0].Tag.ToString());

            //// Get PCM position
            //uint position = 0;
            //uint.TryParse(viewMarkers.SelectedItems[0].SubItems[3].Text, out position);

            //// Set player position            
            //player.SetPosition(position);
        }

        /// <summary>
        /// Occurs when the user changes the selection on the Markers grid view.
        /// Enables/disables marker buttons.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void viewMarkers_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //// Enable/disable marker buttons
            //if (viewMarkers.SelectedItems.Count == 0)
            //{
            //    btnEditMarker.Enabled = false;
            //    btnRemoveMarker.Enabled = false;
            //    btnGoToMarker.Enabled = false;
            //}
            //else
            //{
            //    btnEditMarker.Enabled = true;
            //    btnRemoveMarker.Enabled = true;
            //    btnGoToMarker.Enabled = true;
            //}
        }

        #endregion

        #region Loops Button and GridView Events

        /// <summary>
        /// Occurs when the user has clicked on the Add Loop button.
        /// Opens the Add/Edit Loop window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnAddLoop_Click(object sender, EventArgs e)
        {
            //// Check if the wave data is loaded
            //if (waveFormMarkersLoops.WaveDataHistory.Count == 0)
            //{
            //    return;
            //}

            ////// Check if there are at least two markers
            ////if (viewMarkers.Items.Count < 2)
            ////{
            ////    // Display message
            ////    MessageBox.Show("You must add at least two markers before adding a loop.", "Error adding loop", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ////    return;
            ////}

            ////// Create window and show as dialog
            ////formAddEditLoop = new frmAddEditLoop(this, AddEditLoopWindowMode.Add, Player.Playlist.CurrentItem.AudioFile, Guid.Empty);            
            ////formAddEditLoop.toolTip.Active = Config.GetKeyValueGeneric<bool>("ShowTooltips").HasValue ? Config.GetKeyValueGeneric<bool>("ShowTooltips").Value : true;
            ////formAddEditLoop.ShowDialog(this);            
        }

        /// <summary>
        /// Occurs when the user has clicked on the Edit Loop button.
        /// Opens the Add/Edit Loop window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnEditLoop_Click(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewLoops.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Get selected loopId
            //Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

            //// Create window and show as dialog
            //formAddEditLoop = new frmAddEditLoop(this, AddEditLoopWindowMode.Edit, Player.Playlist.CurrentItem.AudioFile, loopId);
            //formAddEditLoop.ShowDialog(this);
        }

        /// <summary>
        /// Occurs when the user has clicked on the Remove Loop button.
        /// Confirms with the user the deletion of the loop.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnRemoveLoop_Click(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewLoops.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Confirm with the user
            //if (MessageBox.Show("Are you sure you wish to remove the '" + viewLoops.SelectedItems[0].SubItems[1].Text + "' loop?", "Remove loop confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            //{
            //    // Get selected loopId
            //    Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

            //    // Delete loop and refresh list                
            //    Library.Facade.DeleteLoop(loopId);
            //    RefreshLoops();
            //}
        }

        /// <summary>
        /// Occurs when the user has clicked on the Play Loop button.
        /// Starts the playback of a loop.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPlayLoop_Click(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewLoops.SelectedItems.Count == 0)
            //{
            //    return;
            //}            

            //// Get selected loopId
            //Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

            //// Fetch loop from database
            //Loop loop = Library.Facade.SelectLoop(loopId);

            //// Check if the loop is valid
            //if (loop == null)
            //{
            //    return;
            //}

            //// Set current loop in player            
            //Player.StartLoop(loop);

            //// Set currently playing loop icon
            //for (int a = 0; a < viewLoops.Items.Count; a++)
            //{
            //    // Check if the loop is currently playing
            //    if (new Guid(viewLoops.Items[a].Tag.ToString()) == loop.LoopId)
            //    {
            //        // Set flag
            //        viewLoops.Items[a].ImageIndex = 23;
            //    }
            //    else
            //    {
            //        // Reset flag
            //        viewLoops.Items[a].ImageIndex = -1;
            //    }
            //}

            //// Reset buttons
            //btnPlayLoop.Enabled = false;
            //btnStopLoop.Enabled = true;
        }

        /// <summary>
        /// Occurs when the user has clicked on the Stop Loop button.
        /// Stops the playback of a loop.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnStopLoop_Click(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewLoops.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Reset loop
            //Player.StopLoop();

            //// Refresh loops
            //RefreshLoops();

            //// Reset buttons
            //btnPlayLoop.Enabled = true;
            //btnStopLoop.Enabled = false;
        }

        /// <summary>
        /// Occurs when the user changes the selection on the Loops grid view.
        /// Enables/disables loop buttons.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void viewLoops_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //// Enable/disable loop buttons
            //if (viewLoops.SelectedItems.Count == 0)
            //{
            //    btnPlayLoop.Enabled = false;
            //    btnEditLoop.Enabled = false;
            //    btnRemoveLoop.Enabled = false;
            //}
            //else
            //{
            //    // At least one item is selected.                
            //    btnPlayLoop.Enabled = true;
            //    btnStopLoop.Enabled = false;
            //    btnEditLoop.Enabled = true;
            //    btnRemoveLoop.Enabled = true;

            //    // Check if the loop is currently playing
            //    if (Player.Loop != null)
            //    {
            //        // Check if the loop matches
            //        if (new Guid(viewLoops.SelectedItems[0].Tag.ToString()) == Player.Loop.LoopId)
            //        {
            //            // Set buttons
            //            btnPlayLoop.Enabled = false;
            //            btnStopLoop.Enabled = true;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Occurs when the user double-clicks on the Loops grid view.
        /// Sets the current loop as the currently selected item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void viewLoops_DoubleClick(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewLoops.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Check if the player is already playing a loop
            //if (Player.Loop != null)
            //{
            //    return;
            //}

            //// Get selected loopId
            //Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

            //// Fetch loop from database
            //Loop loop = Library.Facade.SelectLoop(loopId);

            //// Check if the loop is valid
            //if (loop == null)
            //{
            //    return;
            //}

            //// Set current loop in player            
            //Player.StartLoop(loop);

            //// Set currently playing loop icon
            //for (int a = 0; a < viewLoops.Items.Count; a++)
            //{
            //    // Check if the loop is currently playing
            //    if (new Guid(viewLoops.Items[a].Tag.ToString()) == loop.LoopId)
            //    {
            //        // Set flag
            //        viewLoops.Items[a].ImageIndex = 23;
            //    }
            //    else
            //    {
            //        // Reset flag
            //        viewLoops.Items[a].ImageIndex = -1;
            //    }
            //}

            //// Reset buttons
            //btnPlayLoop.Enabled = false;
            //btnStopLoop.Enabled = true;
        }

        #endregion

        /// <summary>
        /// Occurs when the user clicks on one of the columns of the song gridview.
        /// </summary>
        /// <param name="data">Event data</param>
        private void viewSongs2_OnColumnClick(SongGridViewColumnClickData data)
        {
            // Refresh browser
            //RefreshSongBrowser();
        }

        /// <summary>
        /// Displays the "Update Library" panel for updating progress.
        /// </summary>
        /// <param name="show">If true, the panel will be shown.</param>
        public void ShowUpdateLibraryProgress(bool show)
        {
            // Check if the panel needs to be shown
            if (show)
            {
                // The update library progress panel is 102 pixels high.
                treeLibraryBrowser.Height -= 102;
                panelUpdateLibraryProgress.Visible = true;
            }
            else
            {
                treeLibraryBrowser.Height += 102;
                panelUpdateLibraryProgress.Visible = false;
            }
        }

        /// <summary>
        /// Enables/disables tooltips.
        /// </summary>
        /// <param name="enable">If true, tooltips will be enabled.</param>
        public void EnableTooltips(bool enable)
        {
            //// Show/hide tooltips
            //if (formAddEditLoop != null)
            //    formAddEditLoop.toolTip.Active = enable;
            //if (formAddEditMarker != null)
            //    formAddEditMarker.toolTip.Active = enable;
            //if (formEditSongMetadata != null)
            //    formEditSongMetadata.toolTip.Active = enable;
            //if (formEffects != null)
            //    formEffects.toolTip.Active = enable;
            //if (formPlaylist != null)
            //    formPlaylist.toolTip.Active = enable;
            //if (formSettings != null)
            //    formSettings.toolTip.Active = enable;
            //if (formUpdateLibraryStatus != null)
            //    formUpdateLibraryStatus.toolTip.Active = enable;
        }

        #region IMainView implementation

        public Action OnOpenPreferencesWindow { get; set; }
        public Action OnOpenEffectsWindow { get; set; }
        public Action OnOpenPlaylistWindow { get; set; }
        public Action OnOpenSyncWindow { get; set; }
        public Action<List<string>> OnAddFilesToLibrary { get; set; }
        public Action<string> OnAddFolderToLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }

        #endregion

        #region ILibraryBrowserView implementation

        public Action<AudioFileFormat> OnAudioFileFormatFilterChanged { get; set; }
        public Action<LibraryBrowserEntity> OnTreeNodeSelected { get; set; }
        public Action<LibraryBrowserEntity> OnTreeNodeDoubleClicked { get; set; }
        public Action<LibraryBrowserEntity, object> OnTreeNodeExpanded { get; set; }
        public Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable { get; set; }

        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                treeLibraryBrowser.BeginUpdate();
                treeLibraryBrowser.Nodes.Clear();

                foreach (var entity in entities)
                {
                    var node = new TreeNode(entity.Title);
                    node.Tag = entity;
                    switch (entity.Type)
                    {
                        case LibraryBrowserEntityType.AllSongs:
                            node.ImageIndex = 12;
                            node.SelectedImageIndex = 12;
                            break;
                        case LibraryBrowserEntityType.Artists:
                            node.ImageIndex = 16;
                            node.SelectedImageIndex = 16;
                            break;
                        case LibraryBrowserEntityType.Albums:
                            node.ImageIndex = 17;
                            node.SelectedImageIndex = 17;
                            break;
                    }

                    if (entity.Type != LibraryBrowserEntityType.AllSongs)
                        node.Nodes.Add("dummy", "dummy");

                    treeLibraryBrowser.Nodes.Add(node);
                }

                treeLibraryBrowser.EndUpdate();
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
        {
            Console.WriteLine("frmMain - RefreshLibraryBrowserNode - entities.Count: {0}", entities.Count());
            MethodInvoker methodUIUpdate = delegate
            {
                var node = (TreeNode)userData;
                treeLibraryBrowser.BeginUpdate();

                node.Nodes.Clear();

                foreach (var childEntity in entities)
                {
                    var childNode = new TreeNode(childEntity.Title);
                    childNode.Tag = childEntity;
                    switch (childEntity.Type)
                    {
                        case LibraryBrowserEntityType.Artist:
                            childNode.ImageIndex = 16;
                            childNode.SelectedImageIndex = 16;
                            break;
                        case LibraryBrowserEntityType.Album:
                        case LibraryBrowserEntityType.ArtistAlbum:
                            childNode.ImageIndex = 17;
                            childNode.SelectedImageIndex = 17;
                            break;
                    }

                    if (childEntity.Type != LibraryBrowserEntityType.Album)
                        childNode.Nodes.Add("dummy", "dummy");

                    node.Nodes.Add(childNode);
                }

                treeLibraryBrowser.EndUpdate();
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        #endregion

        #region ISongBrowserView implementation

        public Action<AudioFile> OnTableRowDoubleClicked { get; set; }

        public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
        {
            Console.WriteLine("frmMain - RefreshSongBrowser - audioFiles.Count: {0}", audioFiles.Count());
            MethodInvoker methodUIUpdate = delegate
            {
                //string orderBy = viewSongs2.OrderByFieldName;
                //bool orderByAscending = viewSongs2.OrderByAscending;
                viewSongs2.ImportAudioFiles(audioFiles.ToList());
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        #endregion

        #region IPlayerView implementation

        public Action OnPlayerPlay { get; set; }
        public Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        public Action OnPlayerPause { get; set; }
        public Action OnPlayerStop { get; set; }
        public Action OnPlayerPrevious { get; set; }
        public Action OnPlayerNext { get; set; }
        public Action<float> OnPlayerSetVolume { get; set; }
        public Action<float> OnPlayerSetPitchShifting { get; set; }
        public Action<float> OnPlayerSetTimeShifting { get; set; }
        public Action<float> OnPlayerSetPosition { get; set; }
        public Func<float, PlayerPositionEntity> OnPlayerRequestPosition { get; set; }

        public void PlayerError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate {
                MessageBox.Show(string.Format("An error occured in Player: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }
        
        public void RefreshPlayerStatus(PlayerStatusType status)
        {
            MethodInvoker methodUIUpdate = delegate {
                if (status == PlayerStatusType.Playing)
                {
                    btnPlay.Text = "Pause";
                    btnPlay.Image = MPfm.Windows.Properties.Resources.control_pause;
                }
                else
                {
                    btnPlay.Text = "Play";
                    btnPlay.Image = MPfm.Windows.Properties.Resources.control_play;
                }
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            MethodInvoker methodUIUpdate = delegate {
                lblCurrentPosition.Text = entity.Position;
                lblSongPosition.Text = entity.Position;
                trackPosition.Value = (int)entity.PositionPercentage * 10;

                //    // Set UI            
                //    lblCurrentPosition.Text = position;                
                //    miTraySongPosition.Text = "[ " + position + " / " + player.Playlist.CurrentItem.LengthString + " ]";

                //    // Set position in the wave form display
                //    if (!waveFormMarkersLoops.IsLoading)
                //    {
                //        waveFormMarkersLoops.SetPosition(positionBytes, position);
                //    }

                //    // Update the song position
                //    if (!songPositionChanging)
                //    {
                //        // Get ratio
                //        float ratio = (float)positionSamples / (float)player.Playlist.CurrentItem.LengthSamples;
                //        trackPosition.Value = Convert.ToInt32(ratio * 1000);

                //        // Set time on seek control
                //        lblSongPosition.Text = position;
                //        lblSongPercentage.Text = (ratio * 100).ToString("0.00") + " %";
                //    }
                //}
                //catch
                //{
                //    // Just don't do anything, this might be because the playlist items are now gone.
                //}
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            MethodInvoker methodUIUpdate = delegate {
                if (audioFile == null)
                {
                    lblCurrentArtistName.Text = string.Empty;
                    lblCurrentAlbumTitle.Text = string.Empty;
                    lblCurrentSongTitle.Text = string.Empty;
                    lblCurrentFilePath.Text = string.Empty;
                    lblFrequency.Text = string.Empty;
                    lblBitrate.Text = string.Empty;
                    lblBitsPerSample.Text = string.Empty;
                    lblSoundFormat.Text = string.Empty;
                }
                else
                {
                    lblCurrentArtistName.Text = audioFile.ArtistName;
                    lblCurrentAlbumTitle.Text = audioFile.AlbumTitle;
                    lblCurrentSongTitle.Text = audioFile.Title;
                    lblCurrentFilePath.Text = audioFile.FilePath;
                    lblLength.Text = audioFile.Length;
                    lblFrequency.Text = string.Format("{0} Hz", audioFile.SampleRate);
                    lblBitrate.Text = string.Format("{0} kbps", audioFile.Bitrate);
                    lblBitsPerSample.Text = string.Format("{0} bits", audioFile.BitsPerSample);
                    lblSoundFormat.Text = audioFile.FileType.ToString();

                    miTrayArtistName.Text = audioFile.ArtistName;
                    miTrayAlbumTitle.Text = audioFile.AlbumTitle;
                    miTraySongTitle.Text = audioFile.Title;

                    viewSongs2.NowPlayingAudioFileId = audioFile.Id;
                    viewSongs2.Refresh();

                    try
                    {
                        // Update the album art in an another thread
                        workerAlbumArt.RunWorkerAsync(audioFile.FilePath);
                    }
                    catch
                    {
                        // Just do nothing if thread is busy
                    }

                    // Configure wave form
                    //waveFormMarkersLoops.Length = lengthBytes;
                    //waveFormMarkersLoops.LoadWaveForm(audioFile.FilePath);
                }
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshMarkers(IEnumerable<Marker> markers)
        {
        }

        public void RefreshLoops(IEnumerable<Loop> loops)
        {
        }

        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
        {
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {
        }
        
        #endregion

        #region IMarkersView implementation

        public Action<MarkerTemplateNameType> OnAddMarker { get; set; }
        public Action<Marker> OnEditMarker { get; set; }
        public Action<Marker> OnSelectMarker { get; set; }

        public void MarkerError(Exception ex)
        {
        }

        public void RefreshMarkers(List<Marker> markers)
        {
        }

        #endregion

        #region ILoopsView implementation

        public Action OnAddLoop { get; set; }
        public Action<Loop> OnEditLoop { get; set; }
        public void LoopError(Exception ex)
        {
        }

        public void RefreshLoops(List<Loop> loops)
        {
        }

        #endregion
    }
}
