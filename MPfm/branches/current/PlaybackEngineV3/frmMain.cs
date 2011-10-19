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
        private PlayerV3 playerV3 = null;
        private PlayerV4 playerV4 = null;
        private List<string> soundFiles = null;
        private TextWriterTraceListener textTraceListener = null;
        private bool isSongPositionChanging = false;

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
            //// Make sure the player isn't null
            //if (playerV3 != null)
            //{
            //    // Close the player and set to null
            //    playerV3.Close();
            //    playerV3 = null;
            //}
        }

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

            // Play set of files
            //playerV3.PlayFiles(soundFiles);
            playerV4.PlayFiles(soundFiles);
        }

        /// <summary>
        /// Occurs when the user clicks on the Pause button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPause_Click(object sender, EventArgs e)
        {
            playerV4.Pause();

            //if (!playerV3.IsPaused)
            //{
            //    btnPause.BackColor = SystemColors.ControlDark;
            //}
            //else
            //{
            //    btnPause.BackColor = SystemColors.Control;
            //}
            //playerV3.Pause();            
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

            long positionBytes = playerV4.SubChannels[playerV4.CurrentChannel].Channel.GetPosition();

            lblCurrentPositionPCM.Text = positionBytes.ToString();

            lblStatus.Text = "Current channel: " + playerV4.CurrentChannel.ToString();

            //// Check if the player needs to be updated
            //if (playerV3 != null && playerV3.IsPlaying)
            //{
            //    // Update the player audio stream
            //    playerV3.Update();

            //    // Check if there's a currently playing channel/sound object
            //    if (playerV3.CurrentAudioFile == null || !playerV3.IsPlaying || playerV3.IsPaused)
            //    {
            //        return;
            //    }

            //    // Set metadata           
            //    lblCurrentArtist.Text = playerV3.CurrentAudioFile.ArtistName;
            //    lblCurrentAlbum.Text = playerV3.CurrentAudioFile.AlbumTitle;
            //    lblCurrentTitle.Text = playerV3.CurrentAudioFile.Title;
            //    lblCurrentPath.Text = playerV3.CurrentAudioFile.FilePath;
            //    lblCurrentPosition.Text = playerV3.CurrentChannel.Position;
            //    lblCurrentPositionPCM.Text = playerV3.CurrentChannel.PositionPCM.ToString();
            //    lblCurrentLength.Text = playerV3.CurrentSound.Length;
            //    lblCurrentLengthPCM.Text = playerV3.CurrentSound.LengthPCM.ToString();

            //    if (!isSongPositionChanging)
            //    {
            //        trackPosition.Maximum = (int)playerV3.CurrentSound.LengthPCMBytes;
            //        trackPosition.Value = (int)playerV3.CurrentChannel.PositionPCMBytes;
            //    }

            //    // Set status bar
            //    lblStatus.Text = playerV3.NumberOfChannelsPlaying.ToString() + " channel(s) playing // Current channel index: " + playerV3.CurrentAudioFileIndex.ToString() + " // Output mixer frequency: " + playerV3.OutputFormatMixer.sampleRate.ToString() + " Hz";
            //}
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
            // Check if player is valid
            if (playerV3 != null)
            {
                // Set volume and update label
                playerV3.Volume = trackVolume.Value;
                lblVolumeValue.Text = playerV3.Volume.ToString() + "%";
            }
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
    }
}
