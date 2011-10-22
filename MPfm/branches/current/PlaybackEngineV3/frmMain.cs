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
using MPfm.Library;

namespace PlaybackEngineV3
{
    /// <summary>
    /// Main application form.
    /// </summary>
    public partial class frmMain : Form
    {
        // Private variables
        private string ConfigKey_LastUsedDirectory = "LastUsedDirectory";
        //private PlayerV3 playerV3 = null;
        private PlayerV4 playerV4 = null;
        private List<string> soundFiles = null;
        private TextWriterTraceListener textTraceListener = null;
        private bool isSongPositionChanging = false;
        private long m_currentSongLength = 0;

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

                // Get last used directory from configuration
                string directory = string.Empty;
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[ConfigKey_LastUsedDirectory] != null)
                {
                    directory = config.AppSettings.Settings[ConfigKey_LastUsedDirectory].Value;
                }

                // Set directory
                txtPath.Text = directory;

                // Load the playlist if the path is valid
                if (!String.IsNullOrEmpty(directory))
                {
                    LoadPlaylist();
                }

                // Load player
                //player = new PlayerV3(V3DriverType.WavWriter, "");
                //playerV3 = new PlayerV3();
                //playerV3.Volume = trackVolume.Value;
                playerV4 = new PlayerV4();
                playerV4.OnSongFinished += new PlayerV4.SongFinished(playerV4_OnSongFinished);

                // Refresh status bar
                RefreshStatusBar();

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
            if (playerV4 != null)
            {
                // Close the player and set to null
                playerV4.Dispose();
                playerV4 = null;
            }
        }

        /// <summary>
        /// Occurs when the current song has finished playing.
        /// </summary>
        /// <param name="data">Song Finished Data</param>
        protected void playerV4_OnSongFinished(PlayerV4SongFinishedData data)
        {
            if (playerV4.CurrentSubChannel == null)
            {
                return;
            }

            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Set metadata
                lblCurrentArtist.Text = playerV4.CurrentSubChannel.FileProperties.ArtistName;
                lblCurrentAlbum.Text = playerV4.CurrentSubChannel.FileProperties.AlbumTitle;
                lblCurrentTitle.Text = playerV4.CurrentSubChannel.FileProperties.Title;
                lblCurrentPath.Text = playerV4.CurrentSubChannel.FileProperties.FilePath;

                // Check if this was the last song
                if (data.IsPlaybackStopped)
                {
                    btnStop.PerformClick();
                }

                // Check if the previous/next buttons need to be updated
                btnNext.Enabled = (playerV4.CurrentSubChannelIndex + 1 < playerV4.FilePaths.Count);
                btnPrev.Enabled = (playerV4.CurrentSubChannelIndex > 0);

                m_currentSongLength = playerV4.CurrentSubChannel.Channel.GetLength();
                lblCurrentLength.Text = BytesToTime(m_currentSongLength);
                lblCurrentLengthPCM.Text = m_currentSongLength.ToString();
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

                // Clear playlist
                listBoxPlaylist.Items.Clear();

                // The user has clicked OK; set the current path
                txtPath.Text = dialogFolderBrowser.SelectedPath;

                // Record path in configuration file for next use
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[ConfigKey_LastUsedDirectory] != null)
                {
                    config.AppSettings.Settings.Remove(ConfigKey_LastUsedDirectory);
                }
                config.AppSettings.Settings.Add(ConfigKey_LastUsedDirectory, txtPath.Text);
                config.Save();

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
            //playerV3.Stop();
            playerV4.Stop();

            // Enable/disable buttons
            btnPlay.Enabled = true;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
            btnPrev.Enabled = false;
            btnNext.Enabled = false;
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

            // Play set of files            
            playerV4.PlayFiles(soundFiles);
        }

        /// <summary>
        /// Occurs when the user clicks on the Pause button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!playerV4.IsPaused)
            {
                btnPause.BackColor = SystemColors.ControlDark;
            }
            else
            {
                btnPause.BackColor = SystemColors.Control;
            }
            playerV4.Pause();                 
        }

        /// <summary>
        /// Occurs when the user clicks on the Prev (previous song) button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPrev_Click(object sender, EventArgs e)
        {
            playerV4.Previous();
        }

        /// <summary>
        /// Occurs when the user clicks on the Next (next song) button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            playerV4.Next();
        }

        /// <summary>
        /// Occurs when the user clicks on the Repeat (repeat type) button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnRepeat_Click(object sender, EventArgs e)
        {
            // Cycle through repeat types
            if (playerV4.RepeatType == RepeatType.Off)
            {
                playerV4.RepeatType = RepeatType.Playlist;
            }
            else if (playerV4.RepeatType == RepeatType.Playlist)
            {
                playerV4.RepeatType = RepeatType.Song;
            }
            else
            {
                playerV4.RepeatType = RepeatType.Off;
            }

            // Update status bar
            RefreshStatusBar();
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
            if (playerV4 == null || !playerV4.IsPlaying)
            {
                return;
            }

            if (playerV4.CurrentSubChannel == null)
            {
                return;
            }

            // Get position (in bytes)
            long positionBytes = playerV4.CurrentSubChannel.Channel.GetPosition();

            // Set position labels
            lblCurrentPositionPCM.Text = positionBytes.ToString();

            // Refresh status bar
            RefreshStatusBar();

            // Set position
            lblCurrentPosition.Text = BytesToTime(positionBytes);            

            // Set the metadata for the first time (initial value == [Artist])
            if (lblCurrentArtist.Text == "[Artist]")
            {
                lblCurrentArtist.Text = playerV4.CurrentSubChannel.FileProperties.ArtistName;
                lblCurrentAlbum.Text = playerV4.CurrentSubChannel.FileProperties.AlbumTitle;
                lblCurrentTitle.Text = playerV4.CurrentSubChannel.FileProperties.Title;
                lblCurrentPath.Text = playerV4.CurrentSubChannel.FileProperties.FilePath;

                m_currentSongLength = playerV4.CurrentSubChannel.Channel.GetLength();
                lblCurrentLength.Text = BytesToTime(m_currentSongLength);
                lblCurrentLengthPCM.Text = m_currentSongLength.ToString();

                //long length = playerV4.CurrentSubChannel.FileProperties.LastBlockPosition - playerV4.CurrentSubChannel.FileProperties.FirstBlockPosition;
            }

            if (!isSongPositionChanging)
            {
                trackPosition.Maximum = (int)m_currentSongLength;
                if (positionBytes > m_currentSongLength)
                {
                    trackPosition.Value = (int)m_currentSongLength;
                }
                else
                {
                    trackPosition.Value = (int)positionBytes;
                }
            }

            //    // Set status bar
            //    lblStatus.Text = playerV3.NumberOfChannelsPlaying.ToString() + " channel(s) playing // Current channel index: " + playerV3.CurrentAudioFileIndex.ToString() + " // Output mixer frequency: " + playerV3.OutputFormatMixer.sampleRate.ToString() + " Hz";
            //}
        }

        /// <summary>
        /// Refreshes the status bar.
        /// </summary>
        private void RefreshStatusBar()
        {
            // Build string
            StringBuilder sb = new StringBuilder();
            sb.Append("Current channel: " + playerV4.CurrentSubChannelIndex.ToString());
            sb.Append(" // ");
            sb.Append("Mixer sample rate: " + playerV4.MixerSampleRate.ToString());
            sb.Append(" // ");
            sb.Append("Repeat type: " + playerV4.RepeatType.ToString());            

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
            playerV4.Volume = (float)trackVolume.Value / 100;
            lblVolumeValue.Text = (playerV4.Volume * 100).ToString("0") + "%";            
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

        }

        private void listBoxPlaylist_DoubleClick(object sender, EventArgs e)
        {
            // Skip to this song
            playerV4.GoTo(listBoxPlaylist.SelectedIndex);
        }

        private string BytesToTime(long bytes)
        {
            long samples = bytes * 8 / 16 / 2;
            ulong ms = (ulong)samples * 1000 / 44100;
            return MPfm.Core.Conversion.MillisecondsToTimeString(ms);
        }

        private void trackPosition_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (playerV4.CurrentSubChannel != null)
            {
                playerV4.CurrentSubChannel.Channel.SetPosition(trackPosition.Value);
            }
        }
    }
}
