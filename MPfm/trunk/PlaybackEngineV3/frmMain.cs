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
        private PlayerV3 player = null;
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
            // Configure trace
            textTraceListener = new TextWriterTraceListener(@"PEV3_Log.txt");
            Trace.Listeners.Add(textTraceListener);

            // Write trace init
            Tracing.Log("======================================================");
            Tracing.Log("MPfm - Playback Engine V3 Demo");
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
            player = new PlayerV3();
            player.Volume = trackVolume.Value;
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
                player.Close();
                player = null;
            }
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
            player.Stop();

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
            player.PlayFiles(soundFiles);
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
            // Check if the player needs to be updated
            if (player != null && player.IsPlaying)
            {
                // Update the player audio stream
                player.Update();

                // Check if there's a currently playing channel/sound object
                if (player.CurrentAudioFile == null || !player.IsPlaying || player.IsPaused)
                {
                    return;
                }

                // Set metadata           
                lblCurrentArtist.Text = player.CurrentAudioFile.ArtistName;
                lblCurrentAlbum.Text = player.CurrentAudioFile.AlbumTitle;
                lblCurrentTitle.Text = player.CurrentAudioFile.Title;
                lblCurrentPath.Text = player.CurrentAudioFile.FilePath;                
                lblCurrentPosition.Text = player.CurrentChannel.Position;
                lblCurrentPositionPCM.Text = player.CurrentChannel.PositionPCM.ToString();
                lblCurrentLength.Text = player.CurrentSound.Length;
                lblCurrentLengthPCM.Text = player.CurrentSound.LengthPCM.ToString();

                if (!isSongPositionChanging)
                {
                    trackPosition.Maximum = (int)player.CurrentSound.LengthPCMBytes;
                    trackPosition.Value = (int)player.CurrentChannel.PositionPCMBytes;
                }

                // Set status bar
                lblStatus.Text = player.NumberOfChannelsPlaying.ToString() + " channel(s) playing // Current channel index: " + player.CurrentAudioFileIndex.ToString() + " // Output mixer frequency: " + player.OutputFormatMixer.sampleRate.ToString() + " Hz";
            }
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
            if (player != null)
            {
                // Set volume and update label
                player.Volume = trackVolume.Value;
                lblVolumeValue.Text = player.Volume.ToString() + "%";
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
