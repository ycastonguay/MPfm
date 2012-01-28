//
// frmMain.cs: This class is part of the PlaybackEngineV4 demo application.
//             This is the main form of the application.
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.Library;
using MPfm.Player;

namespace MPfm.Player.Demo
{
    /// <summary>
    /// Main application form.
    /// </summary>
    public partial class frmMain : Form
    {
        // Private variables        
        public Player player = null;
        private List<string> soundFiles = null;
        private TextWriterTraceListener textTraceListener = null;
        private bool isSongPositionChanging = false;
        private bool isNewPlaylist = false;
        private long m_currentSongLength = 0;
        public ConfigData m_configData = null;
        public Device m_device = null;

        /// <summary>
        /// Main form constructor.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when the form loads its contents.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                // Configure trace
                textTraceListener = new TextWriterTraceListener(@"PEV4_Log.txt");
                Trace.Listeners.Add(textTraceListener);

                // Write trace init
                Tracing.Log("======================================================");
                Tracing.Log("MPfm - Playback Engine V4 Demo");
                Tracing.Log("Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

                // Set version label
                lblVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Tracing.Log(ex.Message);
                Tracing.Log(ex.StackTrace);
                Application.Exit();
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            try
            {
                // Register BASS.NET with key                
                Base.Register("yanick.castonguay@gmail.com", "2X3433427152222");  

                // Get last used directory from configuration
                string directory = Config.Load("LastUsedDirectory");

                // Get configuration values
                Tracing.Log("Loading configuration file...");
                m_configData = new ConfigData();
                Tracing.Log("BufferSize: " + m_configData.bufferSize.ToString());
                Tracing.Log("UpdatePeriod: " + m_configData.updatePeriod.ToString());
                Tracing.Log("UpdateThreads: " + m_configData.updateThreads.ToString());
                Tracing.Log("DriverType: " + m_configData.driverType);
                Tracing.Log("DeviceId: " + m_configData.deviceId.ToString());
                Tracing.Log("DeviceName: " + m_configData.deviceName);

                // Set directory
                txtPath.Text = directory;

                // Load the playlist if the path is valid
                if (!String.IsNullOrEmpty(directory))
                {
                    // Load playlist
                    Tracing.Log("Loading playlist...");
                    LoadPlaylist();
                }             

                // Check configured driver type
                if (m_configData.driverType.ToUpper() == "DIRECTSOUND")
                {
                    // Try to find the configured device
                    m_device = DeviceHelper.FindOutputDevice(DriverType.DirectSound, m_configData.deviceName);
                }
                else if (m_configData.driverType.ToUpper() == "ASIO")
                {
                    // Try to find the configured device
                    m_device = DeviceHelper.FindOutputDevice(DriverType.ASIO, m_configData.deviceName);
                }
                else if (m_configData.driverType.ToUpper() == "WASAPI")
                {
                    // Try to find the configured device
                    m_device = DeviceHelper.FindOutputDevice(DriverType.WASAPI, m_configData.deviceName);
                }

                // Check if the device was found
                if (m_device == null)
                {
                    // Select default device instead (DirectSound, default device)
                    m_device = new Device();
                }

                // Load player using configuration values
                Tracing.Log("Initializing player...");
                player = new MPfm.Player.Player(m_device, 44100, m_configData.bufferSize, m_configData.updatePeriod, true);
                player.UpdateThreads = m_configData.updateThreads;
                player.OnPlaylistIndexChanged += new MPfm.Player.Player.PlaylistIndexChanged(player_OnSongFinished);
                
                // Refresh status bar
                Tracing.Log("Refreshing UI...");
                RefreshStatusBar();

                // Set EQ bands combo box
                List<EQBandComboBoxItem> items = new List<EQBandComboBoxItem>();
                for (int a = 0; a < player.CurrentEQPreset.Bands.Count; a++)
                {
                    // Get current band
                    EQPresetBand currentBand = player.CurrentEQPreset.Bands[a];

                    // Add combo box item
                    items.Add(new EQBandComboBoxItem() { Band = a + 1, Center = currentBand.Center, Text = "B" + (a + 1).ToString() + " (" + currentBand.Center.ToString("0") + "Hz)" });
                }

                // Set combo box items
                comboEQBands.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Tracing.Log(ex.Message);
                Tracing.Log(ex.StackTrace);
                Application.Exit();
            }
        }

        /// <summary>
        /// Occurs when the user closes the application.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Make sure the player isn't null
            if (player != null)
            {
                // Close the player and set to null
                player.Dispose();
                player = null;
            }
        }

        /// <summary>
        /// Occurs when the current song has finished playing.
        /// </summary>
        /// <param name="data">Song Finished Data</param>
        protected void player_OnSongFinished(MPfm.Player.PlayerPlaylistIndexChangedData data)
        {
            // Check if playlist exists
            if (player.Playlist == null || player.Playlist.CurrentItem == null)
            {
                return;
            }

            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Set metadata
                lblCurrentArtist.Text = player.Playlist.CurrentItem.AudioFile.ArtistName;
                lblCurrentAlbum.Text = player.Playlist.CurrentItem.AudioFile.AlbumTitle;
                lblCurrentTitle.Text = player.Playlist.CurrentItem.AudioFile.Title;
                lblCurrentPath.Text = player.Playlist.CurrentItem.AudioFile.FilePath;

                // Check if this was the last song
                if (data.IsPlaybackStopped)
                {
                    btnStop.PerformClick();
                    return;
                }

                // Check if the previous/next buttons need to be updated
                btnNext.Enabled = (player.Playlist.CurrentItemIndex + 1 < player.Playlist.Items.Count);
                btnPrev.Enabled = (player.Playlist.CurrentItemIndex > 0);

                m_currentSongLength = player.Playlist.CurrentItem.Channel.GetLength();
                lblCurrentLength.Text = BytesToTime(m_currentSongLength);
                lblCurrentLengthPCM.Text = m_currentSongLength.ToString();

                // Set list box position
                SetListBoxPosition(player.Playlist.CurrentItemIndex);
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

        /// <summary>
        /// Sets a ==> marker on the currently playing song.
        /// </summary>
        /// <param name="position">Playlist index</param>
        public void SetListBoxPosition(int position)
        {
            // If there are no items, just return
            if (listBoxPlaylist.Items.Count == 0)
            {
                return;
            }

            // Reset previous statuses
            for (int a = 0; a < listBoxPlaylist.Items.Count; a++)
            {
                listBoxPlaylist.Items[a] = listBoxPlaylist.Items[a].ToString().Replace("==> ", "");
            }

            // Make sure position is at least zero
            if (position >= 0)
            {
                // Update list box
                string stuff = listBoxPlaylist.Items[position].ToString();
                listBoxPlaylist.Items[position] = "==> " + stuff;
            }
        }

        #region Button Events
        
        /// <summary>
        /// Occurs when the user clicks on the Browse button.
        /// Opens a Select Folder dialog.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // Set selected path
            dialogFolderBrowser.SelectedPath = txtPath.Text;

            // Show dialog
            if (dialogFolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Make sure controls are disabled
                btnStop.Enabled = false;
                btnPlay.Enabled = false;
                btnPause.Enabled = false;
                btnPrev.Enabled = false;
                btnNext.Enabled = false;
                btnPlayLoop.Enabled = false;
                btnStopLoop.Enabled = false;

                // Clear playlist
                listBoxPlaylist.Items.Clear();

                // The user has clicked OK; set the current path
                txtPath.Text = dialogFolderBrowser.SelectedPath;

                // Save last used directory
                Config.Save("LastUsedDirectory", txtPath.Text);

                //// Record path in configuration file for next use
                //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //if (config.AppSettings.Settings[ConfigKey_LastUsedDirectory] != null)
                //{
                //    config.AppSettings.Settings.Remove(ConfigKey_LastUsedDirectory);
                //}
                //config.AppSettings.Settings.Add(ConfigKey_LastUsedDirectory, txtPath.Text);
                //config.Save();

                // Load playlist
                LoadPlaylist();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the Stop button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            // Stop playback            
            player.Stop();
            SetListBoxPosition(-1);

            // Enable/disable buttons
            btnPlay.Enabled = true;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
            btnPrev.Enabled = false;
            btnNext.Enabled = false;
            btnPlayLoop.Enabled = false;
            btnStopLoop.Enabled = false;
        }

        /// <summary>
        /// Occurs when the user clicks on the Play button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPlay_Click(object sender, EventArgs e)
        {
            // Enable/disable buttons
            btnPlay.Enabled = false;
            btnPause.Enabled = true;
            btnStop.Enabled = true;
            btnPrev.Enabled = false; // by default we start on the first song!
            btnNext.Enabled = true;
            btnPlayLoop.Enabled = true;
            btnStopLoop.Enabled = false;

            // Play set of files            
            player.PlayFiles(soundFiles);
            isNewPlaylist = true;

            // Set list box position
            SetListBoxPosition(0);
        }

        /// <summary>
        /// Occurs when the user clicks on the Pause button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!player.IsPaused)
            {
                btnPause.BackColor = SystemColors.ControlDark;
            }
            else
            {
                btnPause.BackColor = SystemColors.Control;
            }
            player.Pause();                 
        }

        /// <summary>
        /// Occurs when the user clicks on the Prev (previous song) button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPrev_Click(object sender, EventArgs e)
        {
            player.Previous();
        }

        /// <summary>
        /// Occurs when the user clicks on the Next (next song) button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            player.Next();
        }

        /// <summary>
        /// Occurs when the user clicks on the Repeat (repeat type) button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnRepeat_Click(object sender, EventArgs e)
        {
            // Cycle through repeat types
            if (player.RepeatType == RepeatType.Off)
            {
                player.RepeatType = RepeatType.Playlist;
            }
            else if (player.RepeatType == RepeatType.Playlist)
            {
                player.RepeatType = RepeatType.Song;
            }
            else
            {
                player.RepeatType = RepeatType.Off;
            }

            // Update status bar
            RefreshStatusBar();
        }

        /// <summary>
        /// Occurs when the user clicks on the Play Loop button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPlayLoop_Click(object sender, EventArgs e)
        {
            // Get values
            uint positionStart = 0;
            uint.TryParse(txtLoopStart.Text, out positionStart);
            uint positionEnd = 0;
            uint.TryParse(txtLoopEnd.Text, out positionEnd);

            // Create and start loop
            Loop loop = new Loop();
            loop.StartPositionBytes = positionStart;
            loop.EndPositionBytes = positionEnd;
            player.StartLoop(loop);

            // Set button enable
            btnPlayLoop.Enabled = false;
            btnStopLoop.Enabled = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the Stop Loop button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnStopLoop_Click(object sender, EventArgs e)
        {
            // Stop loop
            player.StopLoop();

            // Set button enable
            btnPlayLoop.Enabled = true;
            btnStopLoop.Enabled = false;
        }

        /// <summary>
        /// Occurs when the user clicks on the Set EQ button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSetEQ_Click(object sender, EventArgs e)
        {
            float gain = 0.0f;
            float.TryParse(txtEQGain.Text, out gain);
            player.UpdateEQBand(comboEQBands.SelectedIndex, gain, true);
        }

        /// <summary>
        /// Occurs when the user clicks on the Reset EQ button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnResetEQ_Click(object sender, EventArgs e)
        {
            // Reset EQ
            player.ResetEQ();

            // Set current gain
            txtEQGain.Text = "0000";
        }

        /// <summary>
        /// Occurs when the user clicks on the Exit button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            // Exit the application
            Application.Exit();
        }

        #endregion

        /// <summary>
        /// Occurs when the timer for updating the sound system has expired.
        /// This happens every 10 ms. This is necessary to fill the audio stream.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void timerUpdateSoundSystem_Tick(object sender, EventArgs e)
        {
            // Check if the player exists
            if (player == null || !player.IsPlaying)
            {
                return;
            }

            if (player.Playlist == null || player.Playlist.CurrentItem == null)
            {
                return;
            }

            // Get position (in bytes)
            long positionBytes = player.Playlist.CurrentItem.Channel.GetPosition();

            // Set position labels
            lblCurrentPositionPCM.Text = positionBytes.ToString();

            // Refresh status bar
            RefreshStatusBar();

            // Set position
            lblCurrentPosition.Text = BytesToTime(positionBytes);            

            // Set the metadata when loading a new playlist
            if (isNewPlaylist)
            {
                isNewPlaylist = !isNewPlaylist;

                lblCurrentArtist.Text = player.Playlist.CurrentItem.AudioFile.ArtistName;
                lblCurrentAlbum.Text = player.Playlist.CurrentItem.AudioFile.AlbumTitle;
                lblCurrentTitle.Text = player.Playlist.CurrentItem.AudioFile.Title;
                lblCurrentPath.Text = player.Playlist.CurrentItem.AudioFile.FilePath;

                m_currentSongLength = player.Playlist.CurrentItem.Channel.GetLength();
                lblCurrentLength.Text = BytesToTime(m_currentSongLength);
                lblCurrentLengthPCM.Text = m_currentSongLength.ToString();

                //long length = player.CurrentSubChannel.FileProperties.LastBlockPosition - player.CurrentSubChannel.FileProperties.FirstBlockPosition;
            }

            if (!isSongPositionChanging)
            {
                trackPosition.Maximum = (int)m_currentSongLength - 1;
                if (positionBytes > m_currentSongLength - 1)
                {
                    trackPosition.Value = (int)m_currentSongLength - 1;
                }
                else if (positionBytes <= 0)
                {
                    trackPosition.Value = 0;
                }
                else
                {
                    trackPosition.Value = (int)positionBytes;
                }
            }
        }

        /// <summary>
        /// Refreshes the status bar.
        /// </summary>
        private void RefreshStatusBar()
        {
            // Build string
            StringBuilder sb = new StringBuilder();
            sb.Append("Current song index: " + player.Playlist.CurrentItemIndex.ToString());
            sb.Append(" // ");
            sb.Append("Mixer sample rate: " + player.MixerSampleRate.ToString());
            sb.Append(" // ");
            sb.Append("Repeat type: " + player.RepeatType.ToString());            

            lblStatus.Text = sb.ToString();
        }

        /// <summary>
        /// Loads the current playlist.
        /// </summary>
        private void LoadPlaylist()
        {
            // Search for audio files in the specified path
            soundFiles = AudioTools.SearchAudioFilesRecursive(txtPath.Text, "MP3;OGG;FLAC;WAV");

            // Check if there's at least one file in the playlist
            if (soundFiles.Count == 0)
            {
                // Display message
                MessageBox.Show("The player did not find MP3 files in the specified directory:\n" + txtPath.Text, "Playlist creation failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // Enable controls
            btnStop.Enabled = false;
            btnPlay.Enabled = true;
            btnPause.Enabled = false;
            btnPrev.Enabled = false;
            btnNext.Enabled = false;            

            // Fill playlist                
            foreach (string filePath in soundFiles)
            {
                listBoxPlaylist.Items.Add(filePath);
            }
        }

        /// <summary>
        /// Occurs when the user changes the volume using the track bar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackVolume_Scroll(object sender, EventArgs e)
        {
            // Set volume and update label
            player.Volume = (float)trackVolume.Value / 100;
            lblVolumeValue.Text = (player.Volume * 100).ToString("0") + "%";            
        }

        /// <summary>
        /// Occurs when the user holds a mouse button on the Song Position trackbar.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void trackPosition_MouseDown(object sender, MouseEventArgs e)
        {
            isSongPositionChanging = true;
        }

        /// <summary>
        /// Occurs when the user releases a mouse button on the Song Position trackbar.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void trackPosition_MouseUp(object sender, MouseEventArgs e)
        {
            isSongPositionChanging = false;
        }

        /// <summary>
        /// Occurs when the user changes the time shifting using the track bar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackTimeShifting_Scroll(object sender, EventArgs e)
        {
            // Set time shifting and update label
            player.TimeShifting = (float)trackTimeShifting.Value;
            lblTimeShiftingValue.Text = trackTimeShifting.Value.ToString("0") + "%";
        }

        /// <summary>
        /// Occurs when the user changes the position of the song position trackbar.
        /// This is fired only when the mouse button has been released.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackPosition_MouseCaptureChanged(object sender, EventArgs e)
        {
            // Make sure the channel exists
            if (player.Playlist != null && player.Playlist.CurrentItem != null)
            {
                // Set channel position
                player.Playlist.CurrentItem.Channel.SetPosition(trackPosition.Value);
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the Reset time shifting link. Sets time shifting to 0%.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void linkResetTimeShifting_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Reset time shifting (0%)
            trackTimeShifting.Value = 0;
            player.TimeShifting = 0.0f;
            lblTimeShiftingValue.Text = "0%";
        }

        /// <summary>
        /// Occurs when the user double clicks on an item of the listbox playlist.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void listBoxPlaylist_DoubleClick(object sender, EventArgs e)
        {
            // Skip to this song
            player.GoTo(listBoxPlaylist.SelectedIndex);
        }

        /// <summary>
        /// Occurs when the user changes the EQ band selection.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void comboEQBands_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load band
            EQPresetBand currentBand = player.CurrentEQPreset.Bands[comboEQBands.SelectedIndex];
            txtEQBandwidth.Text = currentBand.Bandwidth.ToString("00.00");
            txtEQQ.Text = currentBand.Q.ToString("00.00");
            txtEQGain.Text = currentBand.Gain.ToString("00.00");
        }

        /// <summary>
        /// Converts bytes to time string.
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Time string</returns>
        private string BytesToTime(long bytes)
        {
            long samples = bytes * 8 / 16 / 2;
            ulong ms = (ulong)samples * 1000 / 44100;
            return MPfm.Core.Conversion.MillisecondsToTimeString(ms);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings(this);
            settings.ShowDialog();
        }

        private void btnInsertFile_Click(object sender, EventArgs e)
        {
            // Check if the user has selected something
            if (listBoxPlaylist.SelectedIndex < 0)
            {
                return;
            }            

            // Ask user for file
            if (dialogOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Insert file into playlist
                player.Playlist.InsertItem(dialogOpenFile.FileName, listBoxPlaylist.SelectedIndex);

                // Insert file in list box
                soundFiles.Insert(listBoxPlaylist.SelectedIndex, dialogOpenFile.FileName);
                listBoxPlaylist.Items.Insert(listBoxPlaylist.SelectedIndex, dialogOpenFile.FileName);                
            }            
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            // Check if the user has selected something
            if (listBoxPlaylist.SelectedIndex < 0)
            {
                return;
            } 

            try
            {
                // Remove file from playlist
                player.Playlist.RemoveItem(listBoxPlaylist.SelectedIndex);

                // Remove file from listbox
                soundFiles.RemoveAt(listBoxPlaylist.SelectedIndex);
                listBoxPlaylist.Items.RemoveAt(listBoxPlaylist.SelectedIndex);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }

    /// <summary>
    /// Defines an item of the EQ band combo box.
    /// </summary>
    public class EQBandComboBoxItem
    {
        public int Band { get; set; }
        public float Center { get; set; }
        public string Text { get; set; }
    }
}
