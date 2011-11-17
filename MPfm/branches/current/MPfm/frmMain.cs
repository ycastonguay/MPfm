//
// frmMain.cs: Main form for the MPfm application. Contains the Artist Browser, Song Browser,
//             Current Song Panel, Playback controls, etc.
//
// Copyright � 2011 Yanick Castonguay
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
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.WindowsControls;
using MPfm.Library;
using MPfm.Player;
using MPfm.Player.PlayerV4;

namespace MPfm
{
    /// <summary>
    /// Main form for the MPfm application. Contains the Artist Browser, Song Browser,
    /// Current Song Panel, Playback controls, etc.
    /// </summary>
    public partial class frmMain : MPfm.WindowsControls.Form
    {
        #region Properties

        // Tracing
        private string fileTracingName = "MPfm.log";
        private Stream fileTracing = null;
        private TextWriterTraceListener textTraceListener = null;

        // Initialisation
        public bool IsInitDone { get; set; }        
        public string InitOpenNodeArtist { get; set; }        
        public string InitOpenNodeArtistAlbum { get; set; }        
        public string InitOpenNodeAlbum { get; set; }
        public Guid InitOpenNodePlaylistId { get; set; }
        public Guid InitCurrentSongId { get; set; }

        public FilterSoundFormat FilterSoundFormat { get; set; }
        public bool songPositionChanging = false;
        public SongQuery QuerySongBrowser { get; set; }

        // Forms
        public frmUpdateLibraryStatus formUpdateLibraryStatus = null;        
        public frmEffects formEffects = null;
        public frmSettings formSettings = null;
        public frmPlaylist formPlaylist = null;
        public frmEditSongMetadata formEditSongMetadata = null;
        public frmAddEditMarker formAddEditMarker = null;
        public frmAddEditLoop formAddEditLoop = null;
        public frmVisualizer formVisualizer = null;

        // Tree library nodes
        public TreeNode nodeAllSongs = null;
        public TreeNode nodeAllArtists = null;
        public TreeNode nodeAllAlbums = null;
        public TreeNode nodeAllPlaylists = null;
        public TreeNode nodeRecentlyPlayed = null;

        // Timer for updating song position
        public System.Windows.Forms.Timer m_timerSongPosition = null;

        private MPfm.Library.Library m_library = null;
        public MPfm.Library.Library Library
        {
            get
            {
                return m_library;
            }
        }

        private MPfm.Player.PlayerV4.Player m_playerV4 = null;
        public MPfm.Player.PlayerV4.Player PlayerV4
        {
            get
            {
                return m_playerV4;
            }
        }

        // Configuration
        private MPFMConfig m_config = null;
        public MPFMConfig Config
        {
            get
            {
                return m_config;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor for the main form.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }    

        /// <summary>
        /// Fires when the main form is first loaded.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Set initialization boolean
            IsInitDone = false;

            // Initialize tracing
            frmSplash.SetStatus("Initializing tracing...");
            
            // Check if trace file exists
            if (!File.Exists(fileTracingName))
            {
                // Create file
                fileTracing = File.Create(fileTracingName);
            }
            else
            {
                try
                {
                    // Open file
                    fileTracing = File.Open(fileTracingName, FileMode.Append);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
                
            // Configure trace
            textTraceListener = new TextWriterTraceListener(fileTracing);
            Trace.Listeners.Add(textTraceListener);

            // Start log
            Tracing.LogWithoutTimeStamp("");
            Tracing.LogWithoutTimeStamp("******************************************************************************");
            Tracing.LogWithoutTimeStamp("MPfm: Music Player for Musicians - Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Tracing.LogWithoutTimeStamp("Started at: " + DateTime.Now.ToString());
            Tracing.LogWithoutTimeStamp("");

            // Load configuration
            try
            {                               
                // Register BASS.NET with key      
                Tracing.Log("Registering BASS.NET...");
                Base.Register("yanick.castonguay@gmail.com", "2X3433427152222");

                Tracing.Log("Loading configuration...");
                frmSplash.SetStatus("Loading configuration...");

                // Get config
                m_config = new MPFMConfig();                

                // Get assembly version
                Assembly assembly = Assembly.GetExecutingAssembly();
                string title = "MPfm: Music Player for Musicians";
                this.Text = title + " - " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

                //// Verify if the database path is in the configuration file 
                //bool databaseFileFound = false;
                //string connectionString = string.Empty;
                //if (m_config.Config.ConnectionStrings.ConnectionStrings.Count > 0)
                //{
                //    // Loop through connection strings
                //    foreach (ConnectionStringSettings connectionStringSetting in m_config.Config.ConnectionStrings.ConnectionStrings)
                //    {
                //        // Check for connection string name
                //        if (connectionStringSetting.Name.ToUpper() == "MPFM_EF")
                //        {
                //            // Set connection string
                //            connectionString = connectionStringSetting.ConnectionString;                            
                //        }
                //    }
                //}

                //// If a connection string was found, check if the file exists
                //if (!String.IsNullOrEmpty(connectionString))
                //{
                //    // Extract file path
                //    string[] stuff = Regex.Split(connectionString.ToUpper(), "\"DATA SOURCE=");
                //    if (stuff.Length == 2)
                //    {
                //        // Get file path
                //        string filePath = stuff[1].Replace("\"", "");

                //        try
                //        {
                //            // Check if the database file exists
                //            if (File.Exists(filePath))
                //            {
                //                // We found the database file!
                //                databaseFileFound = true;
                //            }
                //        }
                //        catch
                //        {
                //            // Do nothing; the database file found flag is already false
                //        }
                //    }
                //}

                //// Check if the database file was found
                //if(!databaseFileFound)
                //{
                //    // Ask user if he/she wants to create a new database file, or to point to another one
                //    frmSplash.HideSplash();
                //    MessageBox.Show("The database file was not found. Check the database file path in the MPfm.exe.config file.", "Error: Could not find database file", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //    //DialogResult resultDatabaseFile = MessageBox.Show(this, "Error: The database file was not found.\n\nTo create a new database, click on the Yes button.\nTo select an existing database, click on the No button.\nTo exit the application, click on the Cancel button.", "Error: Database file not found", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                //    //if (resultDatabaseFile == System.Windows.Forms.DialogResult.Yes)
                //    //{

                //    //}
                    
                //    // Actually... show the Select database or create a new one in the 2nd part of the first run window.

                //    // Create a new database from script... is that possible? 

                //    //dialogOpenFile.ShowDialog();
                //    Application.Exit();
                //}

            }
            catch (Exception ex)
            {
                Tracing.Log("Configuration error:" + ex.Message);
                frmSplash.SetError("Configuration error: " + ex.Message);
            }

            // Check if it's the first time the user runs the application
            if (Config.FirstRun)
            {
                // Display the first run wizard
                frmFirstRun formFirstRun = new frmFirstRun(this);
                DialogResult dialogResultFirstRun = formFirstRun.ShowDialog();

                // Evaluate user response
                if (dialogResultFirstRun == System.Windows.Forms.DialogResult.Cancel)
                {
                    // User clicked cancel; exit the application immediately
                    Application.Exit();
                    return;                    
                }
                else
                {
                    // Wizard is done: set first run to false
                    Config.FirstRun = false;
                }
            }
            
            // Create player
            try
            {
                Device device = null;
                Tracing.Log("Loading player...");
                frmSplash.SetStatus("Loading player...");
                
                // Get configuration values
                DriverType driverType = Config.Driver;
                string deviceName = Config.OutputDevice;                

                // Check configured driver type
                if (driverType == DriverType.DirectSound)
                {
                    // Try to find the configured device
                    device = DeviceHelper.FindOutputDevice(DriverType.DirectSound, deviceName);
                }
                else if (driverType == DriverType.ASIO)
                {
                    // Try to find the configured device
                    device = DeviceHelper.FindOutputDevice(DriverType.ASIO, deviceName);
                }
                else if (driverType == DriverType.WASAPI)
                {
                    // Try to find the configured device
                    device = DeviceHelper.FindOutputDevice(DriverType.WASAPI, deviceName);
                }

                // Check if the device was found
                if (device == null)
                {
                    // Select default device instead (DirectSound, default device)
                    device = new Device();
                }

                // Create player
                m_playerV4 = new MPfm.Player.PlayerV4.Player(device, 96000, 100, 10);
                m_playerV4.OnSongFinished += new Player.PlayerV4.Player.SongFinished(m_playerV4_OnSongFinished);
                m_playerV4.OnStreamCallbackCalled += new MPfm.Player.PlayerV4.Player.StreamCallbackCalled(m_playerV4_OnStreamCallbackCalled);

                // Create timer
                m_timerSongPosition = new System.Windows.Forms.Timer();
                m_timerSongPosition.Interval = 10;
                m_timerSongPosition.Tick += new EventHandler(m_timerSongPosition_Tick);
                m_timerSongPosition.Enabled = true;

                // Load library
                Tracing.Log("Loading library...");
                frmSplash.SetStatus("Loading library...");
                string databaseFilePath = AppDomain.CurrentDomain.BaseDirectory + "MPfm.db";
                m_library = new Library.Library(databaseFilePath);
            }
            catch (Exception ex)
            {
                // Set error in splash and hide splash
                frmSplash.SetStatus("Error initializing player!");
                frmSplash.HideSplash();

                // Display message box with error
                this.TopMost = true;
                MessageBox.Show("There was an error while initializing the player.\nYou can remove the appSettings node from MPFM.exe.config to reset the configuration and display the First Run screen.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error initializing player!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracing.Log("Player init error: " + ex.Message + "\nStack trace: " + ex.StackTrace);
                
                // Exit application
                Application.Exit();
                return;
            }

            // Load UI
            try
            {
                Tracing.Log("Loading UI...");
                frmSplash.SetStatus("Loading UI...");

                Tracing.Log("Loading UI - Effects...");
                formEffects = new frmEffects(this);

                Tracing.Log("Loading UI - Settings...");
                formSettings = new frmSettings(this);

                Tracing.Log("Loading UI - Playlist...");
                formPlaylist = new frmPlaylist(this);

                Tracing.Log("Loading UI - Visualizer...");
                formVisualizer = new frmVisualizer(this);
            }
            catch (Exception ex)
            {
                // Set error in splash and hide splash
                frmSplash.SetStatus("Error initializing UI!");
                frmSplash.HideSplash();

                // Display message box with error
                this.TopMost = true;
                MessageBox.Show("There was an error while initializing the UI.\nYou can remove the appSettings node from MPFM.exe.config to reset the configuration and display the First Run screen.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error initializing UI!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracing.Log("UI error: " + ex.Message + "\nStack trace: " + ex.StackTrace);

                // Exit application
                Application.Exit();
                return;
            }

            // Resetting display
            lblCurrentAlbumTitle.Text = "";
            lblCurrentArtistName.Text = "";
            lblCurrentSongTitle.Text = "";
            lblCurrentFilePath.Text = "";
            lblBitsPerSample.Text = "";
            lblSoundFormat.Text = "";
            lblFrequency.Text = "";

            Tracing.Log("Refreshing library cache...");
            frmSplash.SetStatus("Refreshing library cache...");

            // Load window configuration (position, size, column sizes, etc.)
            LoadWindowConfiguration();

            // Reset song query
            QuerySongBrowser = new SongQuery();

            // Get media type filter configuration and set media type before refreshing the tree library
            comboSoundFormat.SelectedItem = Config.FilterSoundFormat;
            RefreshTreeLibrary();

            //RefreshPlayControls();

            //if (config.GetBoolean("PlaylistVisible"))
            //{
            //    btnPlaylist.Checked = true;
            //    formPlaylist.Show(this);
            //}            

            //if (currentSong != null)
            //{
            //    //PlayCurrentSong(true, config.GetInteger("CurrentSongPosition"));
            //}

            Tracing.Log("Applying configuration...");
            frmSplash.SetStatus("Applying configuration...");

            // Reset init settings
            InitOpenNodeAlbum = string.Empty;
            InitOpenNodeArtist = string.Empty;
            InitOpenNodeArtistAlbum = string.Empty;
            InitOpenNodePlaylistId = Guid.Empty;
            InitCurrentSongId = Guid.Empty;

            // Set volume
            faderVolume.Value = Config.Volume;

            // Set tray options
            notifyIcon.Visible = Config.ShowTray;

            // Set EQ options           
            // (automatic)

            // Set query if available
            string queryArtistName = Config.SongQueryArtistName;
            string queryAlbumTitle = Config.SongQueryAlbumTitle;
            string queryPlaylistId = Config.SongQueryPlaylistId;
            string querySongId = Config.SongQuerySongId;
            string currentNodeType = Config.CurrentNodeType;

            // Set Init current song Id
            if (!string.IsNullOrEmpty(querySongId))
            {
                // Make sure the application doesn't crash if it tries to convert a string into Guid
                try
                {
                    InitCurrentSongId = new Guid(querySongId);
                }
                catch(Exception ex)
                {
                    // Do nothing
                }               
            }

            // Set default current node type
            if (String.IsNullOrEmpty(currentNodeType))
            {
                currentNodeType = "AllSongs";
            }

            // Set selected node depending on configuration
            // AllArtists: No background worker required
            if (currentNodeType == "AllArtists")
            {
                // Set selected node
                treeLibrary.SelectedNode = nodeAllArtists;

                // Refresh song browser
                RefreshSongBrowser();

                // Declare init done!
                SetInitDone();
            }
            // AllAlbums: No background worker required
            else if (currentNodeType == "AllAlbums")
            {
                // Set selected node
                treeLibrary.SelectedNode = nodeAllAlbums;

                // Refresh song browser
                RefreshSongBrowser();

                // Declare init done!
                SetInitDone();
            }
            // nodeAllSongs: No background worker required
            else if (currentNodeType == "AllSongs")
            {
                // Set selected node
                treeLibrary.SelectedNode = nodeAllSongs;

                // Refresh song browser
                RefreshSongBrowser();

                // Declare init done!
                SetInitDone();
            }
            // AllPlaylists: No background worker required
            else if (currentNodeType == "AllPlaylists")
            {
                // Set selected node
                treeLibrary.SelectedNode = nodeAllPlaylists;

                // Refresh song browser
                RefreshSongBrowser();

                // Declare init done!
                SetInitDone();
            }
            // Artist: Background worker required
            else if (currentNodeType == "Artist")
            {
                // Expand the AllArtists node
                InitOpenNodeArtist = queryArtistName;                
                nodeAllArtists.Expand();

                // Can't declare init done yet since background thread is running
            }
            else if (currentNodeType == "Album")
            {

            }
            else if (currentNodeType == "ArtistAlbum")
            {
                // Expand the AllArtists node
                InitOpenNodeArtist = queryArtistName;
                InitOpenNodeArtistAlbum = queryAlbumTitle;
                nodeAllArtists.Expand();

                // Can't declare init done yet since background thread is running
            }
            else if (currentNodeType == "Playlist")
            {
                // Expand the playlist node                
                if (!String.IsNullOrEmpty(queryPlaylistId))
                {                    
                    try
                    {
                        InitOpenNodePlaylistId = new Guid(queryPlaylistId);
                    }
                    catch
                    {
                        InitOpenNodePlaylistId = Guid.Empty;
                    }
                }                
                nodeAllPlaylists.Expand();
            }
        }

        /// <summary>
        /// Loads the window configuration (window position, size, columns, etc.) from the configuration file.
        /// </summary>
        public void LoadWindowConfiguration()
        {
            if (!Config.KeyExists(MPFMConfig.Key_WindowX))
            {
                // No configuration for window position; center window in screen
                StartPosition = FormStartPosition.CenterScreen;
                return;
            }
            else
            {
                int x = 0;
                int y = 0;

                // Make sure the window X isn't negative
                if (Config.WindowX < 0)
                {
                    x = 0;
                }
                else
                {
                    x = Config.WindowX;
                }
                // Make sure the window Y isn't negative
                if (Config.WindowY < 0)
                {
                    y = 0;
                }
                else
                {
                    y = Config.WindowY;
                }

                Location = new Point(x, y);
            }
           
            Width = Config.WindowWidth;
            Height = Config.WindowHeight;

            if (Config.WindowMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }

            splitFirst.SplitterDistance = Config.WindowSplitterDistance;

            //// Load song browser column widths
            //viewSongs.Columns[0].Width = Config.SongBrowserCol1Width;
            //viewSongs.Columns[1].Width = Config.SongBrowserCol2Width;
            //viewSongs.Columns[2].Width = Config.SongBrowserCol3Width;
            //viewSongs.Columns[3].Width = Config.SongBrowserCol4Width;
            //viewSongs.Columns[4].Width = Config.SongBrowserCol5Width;
            //viewSongs.Columns[5].Width = Config.SongBrowserCol6Width;
            //viewSongs.Columns[6].Width = Config.SongBrowserCol7Width;
            //viewSongs.Columns[7].Width = Config.SongBrowserCol8Width;

            if (formPlaylist != null)
            {
                // Load playlist window position and size
                if (!Config.KeyExists(MPFMConfig.Key_PlaylistX))
                {
                    // No configuration for window position; center window in screen
                    formPlaylist.StartPosition = FormStartPosition.CenterScreen;
                    return;
                }
                else
                {
                    formPlaylist.Location = new Point(Config.PlaylistX, Config.PlaylistY);
                }

                formPlaylist.Width = Config.PlaylistWidth;
                formPlaylist.Height = Config.PlaylistHeight;

                if (Config.PlaylistMaximized)
                {
                    formPlaylist.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    formPlaylist.WindowState = FormWindowState.Normal;
                }

                formPlaylist.Visible = Config.PlaylistVisible;

                // Load playlist column widths
                formPlaylist.viewSongs.Columns[0].Width = Config.PlaylistCol1Width;
                formPlaylist.viewSongs.Columns[1].Width = Config.PlaylistCol2Width;
                formPlaylist.viewSongs.Columns[2].Width = Config.PlaylistCol3Width;
                formPlaylist.viewSongs.Columns[3].Width = Config.PlaylistCol4Width;
                formPlaylist.viewSongs.Columns[4].Width = Config.PlaylistCol5Width;
                formPlaylist.viewSongs.Columns[5].Width = Config.PlaylistCol6Width;
                formPlaylist.viewSongs.Columns[6].Width = Config.PlaylistCol7Width;
                formPlaylist.viewSongs.Columns[7].Width = Config.PlaylistCol8Width;
            }
        }

        /// <summary>
        /// Saves the window configuration (window position, size, columns, etc.) into the configuration file.
        /// </summary>
        public void SaveWindowConfiguration()
        {
            // Save window position and size
            Config.WindowX = Location.X;
            Config.WindowY = Location.Y;
            Config.WindowWidth = Width;
            Config.WindowHeight = Height;
            Config.WindowSplitterDistance = splitFirst.SplitterDistance;

            bool isMaximized = false;
            if (WindowState == FormWindowState.Maximized)
            {
                isMaximized = true;
            }
            Config.WindowMaximized = isMaximized;

            //// Save song browser column widths
            //Config.SongBrowserCol1Width = viewSongs.Columns[0].Width;
            //Config.SongBrowserCol2Width = viewSongs.Columns[1].Width;
            //Config.SongBrowserCol3Width = viewSongs.Columns[2].Width;
            //Config.SongBrowserCol4Width = viewSongs.Columns[3].Width;
            //Config.SongBrowserCol5Width = viewSongs.Columns[4].Width;
            //Config.SongBrowserCol6Width = viewSongs.Columns[5].Width;
            //Config.SongBrowserCol7Width = viewSongs.Columns[6].Width;
            //Config.SongBrowserCol8Width = viewSongs.Columns[7].Width;

            // Save playlist window position and size
            if (formPlaylist != null)
            {
                Config.PlaylistX = formPlaylist.Location.X;
                Config.PlaylistY = formPlaylist.Location.Y;
                Config.PlaylistWidth = formPlaylist.Width;
                Config.PlaylistHeight = formPlaylist.Height;
                Config.PlaylistVisible = formPlaylist.Visible;

                if (formPlaylist.WindowState == FormWindowState.Maximized)
                {
                    isMaximized = true;
                }
                Config.PlaylistMaximized = isMaximized;

                // Save playlist column widths
                Config.PlaylistCol1Width = formPlaylist.viewSongs.Columns[0].Width;
                Config.PlaylistCol2Width = formPlaylist.viewSongs.Columns[1].Width;
                Config.PlaylistCol3Width = formPlaylist.viewSongs.Columns[2].Width;
                Config.PlaylistCol4Width = formPlaylist.viewSongs.Columns[3].Width;
                Config.PlaylistCol5Width = formPlaylist.viewSongs.Columns[4].Width;
                Config.PlaylistCol6Width = formPlaylist.viewSongs.Columns[5].Width;
                Config.PlaylistCol7Width = formPlaylist.viewSongs.Columns[6].Width;
                Config.PlaylistCol8Width = formPlaylist.viewSongs.Columns[7].Width;
            }
        }

        /// <summary>
        /// Sets the initialization phase to done. Closes the splash screen.
        /// </summary>
        public void SetInitDone()
        {
            // Set initialization boolean
            IsInitDone = true;

            Tracing.Log("Initialization successful!");
            frmSplash.SetStatus("Initialization successful!");

            this.BringToFront();
            this.Activate();
            frmSplash.CloseFormWithFadeOut();            
        }

        #endregion

        #region Player Events

        public void m_playerV4_OnStreamCallbackCalled(Player.PlayerV4.StreamCallbackData data)
        {
            // Check for valid objects
            if (m_playerV4 == null || !m_playerV4.IsPlaying ||
                m_playerV4.Playlist == null || m_playerV4.Playlist.CurrentItem == null || m_playerV4.Playlist.CurrentItem.Channel == null)
            {
                return;
            }

        }

        /// <summary>
        /// Occurs when the timer for the output meter is expired. This forces the
        /// output meter to refresh itself every 10ms.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void timerUpdateOutputMeter_Tick(object sender, EventArgs e)
        {
            // Check for valid objects
            if (m_playerV4 == null || !m_playerV4.IsPlaying ||
                m_playerV4.Playlist == null || m_playerV4.Playlist.CurrentItem == null || m_playerV4.Playlist.CurrentItem.Channel == null)
            {
                return;
            }

            int peakL = 0;
            int peakR = 0;
            float maxL = 0f;
            float maxR = 0f;

            // length of a 20ms window in bytes
            int length20ms = (int)m_playerV4.MainChannel.Seconds2Bytes2(0.02);   //(int)Bass.BASS_ChannelSeconds2Bytes(channel, 0.02);
            // the number of 32-bit floats required (since length is in bytes!)
            int l4 = length20ms / 4; // 32-bit = 4 bytes

            // create a data buffer as needed
            float[] sampleData = new float[l4];

            //int length = Bass.BASS_ChannelGetData(channel, sampleData, length20ms);
            int length = m_playerV4.MainChannel.GetData(sampleData, length20ms);

            // the number of 32-bit floats received
            // as less data might be returned by BASS_ChannelGetData as requested
            l4 = length / 4;

            float[] left = new float[l4 / 2];
            float[] right = new float[l4 / 2];
            for (int a = 0; a < l4; a++)
            {
                float absLevel = Math.Abs(sampleData[a]);

                // decide on L/R channel
                if (a % 2 == 0)
                {                    
                    // Left channel
                    left[a/2] = sampleData[a];
                    if (absLevel > maxL)
                        maxL = absLevel;
                }
                else
                {
                    // Right channel
                    right[a/2] = sampleData[a];
                    if (absLevel > maxR)
                        maxR = absLevel;
                }
            }

            // limit the maximum peak levels to +6bB = 65535 = 0xFFFF
            // the peak levels will be int values, where 32767 = 0dB
            // and a float value of 1.0 also represents 0db.
            peakL = (int)Math.Round(32767f * maxL) & 0xFFFF;
            peakR = (int)Math.Round(32767f * maxR) & 0xFFFF;

            // convert the level to dB
            double dBlevelL = Base.LevelToDB_16Bit(peakL);
            double dBlevelR = Base.LevelToDB_16Bit(peakR);

            lblBitsPerSampleTitle.Text = dBlevelL.ToString("0.000");

            outputMeter.AddWaveDataBlock(left, right);
            outputMeter.Refresh();

            // Get min max info from wave block
            if (AudioTools.CheckForDistortion(left, right, true, 0.0f))
            {
                // Show distortion warning "LED"
                picDistortionWarning.Visible = true;
            }
        }

        /// <summary>
        /// Occurs when the timer for updating the song position has expired.
        /// Updates the song position UI and other things.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public void m_timerSongPosition_Tick(object sender, EventArgs e)
        {
            // Check for valid objects
            if (m_playerV4 == null || !m_playerV4.IsPlaying ||
                m_playerV4.Playlist == null || m_playerV4.Playlist.CurrentItem == null || m_playerV4.Playlist.CurrentItem.Channel == null)
            {
                return;
            }

            // Get position
            //long positionBytes = m_playerV4.Playlist.CurrentItem.Channel.GetPosition();
            long positionBytes = m_playerV4.GetPosition();

            // For some reason this works instead of using the 96000 Hz and 24 bit values in the following equations.
            float ratioPosition = (float)44100 / (float)m_playerV4.Playlist.CurrentItem.AudioFile.SampleRate;
            positionBytes = (int)((float)positionBytes * ratioPosition);
            
            //string position = ConvertAudio.ToTimeString(positionBytes, (uint)m_playerV4.Playlist.CurrentItem.AudioFile.BitsPerSample, 2, (uint)m_playerV4.Playlist.CurrentItem.Channel.SampleRate);
            //long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)m_playerV4.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
            long positionSamples = ConvertAudio.ToPCM(positionBytes, 16, 2);
            long positionMS = (int)ConvertAudio.ToMS(positionSamples, 44100);
            string position = Conversion.MillisecondsToTimeString((ulong)positionMS);
            //long positionSamples = ConvertAudio.ToPCM(positionBytes, 16, 2);

            // Set UI            
            lblCurrentTime.Text = position;
            lblTotalTime.Text = m_playerV4.Playlist.CurrentItem.LengthString;
            //waveFormMarkersLoops.Position = positionBytes;
            //waveFormMarkersLoops.PositionTime = position;
            waveFormMarkersLoops.SetPosition(positionBytes, position);

            // Update the song position
            if (!songPositionChanging)
            {
                // Get ratio
                float ratio = (float)positionSamples / (float)m_playerV4.Playlist.CurrentItem.LengthSamples;

                // Do not go beyong 99% or the song might end before!
                if (ratio <= 0.99f)
                {
                    trackPosition.Value = Convert.ToInt32(ratio * 1000);
                }

                // Set time on seek control
                lblSongPosition.Text = position;
                lblSongPercentage.Text = (ratio * 100).ToString("0.00") + " %";
            }

            //// Check if player is playing
            //if (!Player.IsPlaying)
            //{
            //    // Nothing to update
            //    return;
            //}

            //// debug
            ////lblBitrateTitle.Text = data.Debug;
            ////lblFrequencyTitle.Text = data.Debug2;
            ////lblBitrateTitle.Text = Player.CurrentPlaylist.Position.ToString();

            //// Update current time               
            //string currentTime = Conversion.MillisecondsToTimeString(data.SongPositionMilliseconds);
            //lblCurrentTime.Text = currentTime;

            ////lblBitsPerSampleTitle.Text = Player.SoundSystem.NumberOfChannelsPlaying.ToString();

            //// Update data for Loops & Markers wave form display
            //waveFormMarkersLoops.CurrentPositionPCMBytes = data.SongPositionSentencePCMBytes;
            //waveFormMarkersLoops.CurrentPositionMS = data.SongPositionMilliseconds;

            //// Update wave form control
            //formVisualizer.waveForm.AddWaveDataBlock(data.WaveDataLeft, data.WaveDataRight);

            //// Get minmax data from wave data
            ////WaveDataMinMax minMax = AudioTools.GetMinMaxFromWaveData(data.WaveDataLeft, data.WaveDataRight, true);

            //// Add raw wave data to control
            //outputMeter.AddWaveDataBlock(data.WaveDataLeft, data.WaveDataRight);

            //// Get min max info from wave block
            //if (AudioTools.CheckForDistortion(data.WaveDataLeft, data.WaveDataRight, true, 0.0f))
            //{
            //    // Show distortion warning "LED"
            //    picDistortionWarning.Visible = true;
            //}

            ////double rms = System.Math.Sqrt(doubleSum / 256);

            //// Update the song position
            //if (!songPositionChanging)
            //{
            //    // Do not go beyong 99% or the song might end before!
            //    if (data.SongPositionPercentage <= 99)
            //    {                        
            //        trackPosition.Value = Convert.ToInt32(data.SongPositionPercentage * 10);
            //    }

            //    // Set time on seek control
            //    lblSongPosition.Text = currentTime;
            //    lblSongPercentage.Text = data.SongPositionPercentage.ToString("0.00") + " %";
            //}
        }

        /// <summary>
        /// Occurs when the player has finished playing a song.
        /// Updates the UI.
        /// </summary>
        /// <param name="data">Song finished data</param>
        public void m_playerV4_OnSongFinished(Player.PlayerV4.SongFinishedData data)
        {
            // If the initialization isn't finished, exit this event
            if (!IsInitDone)
            {
                return;
            }

            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Refresh song information                    
                RefreshSongInformation();

                // Set the play icon in the song browser
                //RefreshSongBrowserPlayIcon(m_playerV4.Playlist.CurrentItem.Song.SongId);
                RefreshSongBrowserPlayIcon(m_playerV4.Playlist.CurrentItem.AudioFile.Id);

                // Refresh play icon in playlist
                //formPlaylist.RefreshPlaylistPlayIcon(data.NextSong.PlaylistSongId);

                // Set next song in configuration                    
                //Config.SongQuerySongId = m_playerV4.Playlist.CurrentItem.Song.SongId.ToString();
                Config.SongQuerySongId = m_playerV4.Playlist.CurrentItem.AudioFile.Id.ToString();

                // Refresh loops & markers
                RefreshMarkers();
                RefreshLoops();

                // Refresh play count
                //SongGridViewItem item = viewSongs2.Items.FirstOrDefault(x => x.Song.SongId == m_playerV4.Playlist.CurrentItem.Song.SongId);
                SongGridViewItem item = viewSongs2.Items.FirstOrDefault(x => x.Song.SongId == m_playerV4.Playlist.CurrentItem.AudioFile.Id);
                if (item != null)
                {
                    // Set updated data
                    //SongDTO updatedSong = Library.SelectSong(m_playerV4.Playlist.CurrentItem.Song.SongId);
                    SongDTO updatedSong = Library.SelectSong(m_playerV4.Playlist.CurrentItem.AudioFile.Id);
                    item.Song = updatedSong;
                }

                // Refresh icon
                viewSongs2.Refresh();
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

        #region Form Events

        /// <summary>
        /// Occurs when the user tries to close the form, using the X button or the
        /// Close button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Tracing.Log("Closing MPfm...");

            // Save configuration
            SaveWindowConfiguration();
            e.Cancel = false;

            // Close player if not null
            if (m_playerV4 != null)
            {
                // Release the sound system from memory
                m_playerV4.Dispose();
            }
        }
        
        /// <summary>
        /// Saves the configuration and exits the application.
        /// </summary>
        public void ExitApplication()
        {
            SaveWindowConfiguration();
            Application.Exit();
        }

        #endregion

        #region Main Menu Events

        /// <summary>
        /// Occurs when the user clicks on the "File -> Add file(s) to library" menu item.
        /// Pops an open file dialog to let the user select the file(s) to add.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miFileAddFile_Click(object sender, EventArgs e)
        {
            // Display dialog
            if (dialogAddFiles.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                // The user has cancelled the operation
                return;
            }

            // Update the library using the specified folder            
            formUpdateLibraryStatus = new frmUpdateLibraryStatus(this, dialogAddFiles.FileNames.ToList());
            formUpdateLibraryStatus.ShowDialog(this);
        }

        /// <summary>
        /// Occurs when the user clicks on the "File -> Add folder to library" menu item.
        /// Pops an open folder dialog to let the user select the folder to add.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miFileAddFolder_Click(object sender, EventArgs e)
        {
            // Display dialog
            if (dialogAddFolder.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                // The user has cancelled the operation
                return;
            }

            // Update the library using the specified folder            
            formUpdateLibraryStatus = new frmUpdateLibraryStatus(this, dialogAddFolder.SelectedPath);
            formUpdateLibraryStatus.ShowDialog(this);
        }

        /// <summary>
        /// Occurs when the user clicks on the "File -> Open an audio file" menu item.
        /// Pops an open file dialog to let the user select the file to play.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miFileOpenAudioFile_Click(object sender, EventArgs e)
        {
            //// Display dialog
            //if (dialogOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            //{
            //    // The user has cancelled the operation
            //    return;
            //}

            //// MISSING: Update play controls with total length, bitrate, frequency, etc.
            //// Doesn't seem to crash already, that's good news. Output Meter and song position works.

            //// Get the song info
            //RefreshSongInformation(dialogOpenFile.FileName);

            //// Remove play icon on song browser and playlist
            //RefreshSongBrowserPlayIcon(Guid.Empty);
            //formPlaylist.RefreshPlaylistPlayIcon(Guid.Empty);

            //// Play the audio file
            //Player.PlayFile(dialogOpenFile.FileName);
        }

        /// <summary>
        /// Occurs when the user clicks on the "File -> Exit" menu item. This
        /// exits the application.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miFileExit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }


        /// <summary>
        /// Occurs when the user clicks the "Help -> Go to website" menu item.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miHelpWebsite_Click(object sender, EventArgs e)
        {
            // Open website in default browser
            Process.Start("http://www.mp4m.org");
        }

        /// <summary>
        /// Occurs when the user clicks the "Help -> Report a bug" menu item.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miHelpReportBug_Click(object sender, EventArgs e)
        {
            // Open website in default browser
            Process.Start("http://www.mp4m.org/mantis");
        }

        /// <summary>
        /// Occurs when the user clicks on the "Help -> About" menu item.
        /// This displays the "About" screen.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miHelpAbout_Click(object sender, EventArgs e)
        {
            frmSplash splash = new frmSplash(false, true);
            splash.Show(this);
        }

        #endregion

        #region Main Toolbar Events

        /// <summary>
        /// Occurs when the user clicks on the "Update Library" button on the main form toolbar.
        /// Displays the Update Library Status window and updates the whole library.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnUpdateLibrary_Click(object sender, EventArgs e)
        {
            // Update the whole library
            UpdateLibrary();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Play" button on the main form toolbar.
        /// Plays the selected song query in the Song Browser.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlaySelectedSongQuery();            
        }

        /// <summary>
        /// Occurs when the user clicks on the "Pause" button on the main form toolbar.
        /// Pauses the playback on the player.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnPause_Click(object sender, EventArgs e)
        {
            // Validate player
            if (m_playerV4 == null || m_playerV4.Playlist == null || !m_playerV4.IsPlaying)
            {
                return;
            }

            // Check pause status
            if (m_playerV4.IsPaused)
            {
                btnPause.Checked = false;
                miTrayPause.Checked = false;
            }
            else
            {
                btnPause.Checked = true;
                miTrayPause.Checked = true;
            }

            // Set pause
            m_playerV4.Pause();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Stop" button on the main form toolbar.
        /// Stops the playback of the player.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Next song" button on the main form toolbar.
        /// Skips to the next song in the playlist.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnNextSong_Click(object sender, EventArgs e)
        {
            // Validate player
            if (m_playerV4 == null || m_playerV4.Playlist == null || !m_playerV4.IsPlaying)
            {
                return;
            }

            // Skip to next song in player
            m_playerV4.Next();

            // Refresh controls
            RefreshSongControls();
            RefreshMarkers();
            RefreshLoops();            
        }

        /// <summary>
        /// Occurs when the user clicks on the "Previous song" button on the main form toolbar.
        /// Skips to the previous song in the playlist.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnPreviousSong_Click(object sender, EventArgs e)
        {
            // Validate player
            if (m_playerV4 == null || m_playerV4.Playlist == null || !m_playerV4.IsPlaying)
            {
                return;
            }

            // Go to previous song in player
            m_playerV4.Previous();

            // Refresh controls
            RefreshSongControls();
            RefreshMarkers();
            RefreshLoops();            
        }

        /// <summary>
        /// Occurs when the user clicks on the "Repeat Type" button on the main form toolbar.
        /// Iterates through the different repeat types.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnRepeat_Click(object sender, EventArgs e)
        {
            // Cycle through the repeat types
            if (m_playerV4.RepeatType == RepeatType.Off)
            {
                m_playerV4.RepeatType = RepeatType.Playlist;
            }
            else if (m_playerV4.RepeatType == RepeatType.Playlist)
            {
                m_playerV4.RepeatType = RepeatType.Song;
            }
            else
            {
                m_playerV4.RepeatType = RepeatType.Off;
            }

            // Update repeat button
            RefreshRepeatButton();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Playlist" button on the main form toolbar.
        /// Opens or closes the Playlist window.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnPlaylist_Click(object sender, EventArgs e)
        {           
            if (formPlaylist.Visible)
            {                                
                formPlaylist.Close();
                btnPlaylist.Checked = false;
            }
            else
            {
                formPlaylist.Show(this);
                btnPlaylist.Checked = true;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Effects" button on the main form toolbar.
        /// Opens or closes the Effects window.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnEffects_Click(object sender, EventArgs e)
        {
            if (formEffects.Visible)
            {
                formEffects.Close();
                btnEffects.Checked = false;
            }
            else
            {
                formEffects.Show(this);
                btnEffects.Checked = true;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Visualizer" button on the main form toolbar.
        /// Opens or closes the Visualizer window.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnVisualizer_Click(object sender, EventArgs e)
        {
            if (formVisualizer.Visible)
            {
                formVisualizer.Close();
                btnVisualizer.Checked = false;
            }
            else
            {
                formVisualizer.Show(this);
                btnVisualizer.Checked = true;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Settings" button on the main form toolbar.
        /// Opens or closes the Settings window.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnSettings_Click(object sender, EventArgs e)
        {
            formSettings.ShowDialog(this);
        }

        #endregion

        #region Refresh Methods

        /// <summary>
        /// Refreshes all the controls in the main form.
        /// </summary>
        public void RefreshAll()
        {
            RefreshTreeLibrary();
            RefreshSongBrowser();            
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
            // Is the player playing?
            if (PlayerV4.IsPlaying)
            {
                // Mantis 0000042
                // Reset the pause button icon
                btnPause.Checked = false;

                // Update song information                
                //RefreshSongInformation();

                // Set the play icon in the song browser
                //RefreshSongBrowserPlayIcon(PlayerV4.Playlist.CurrentItem.Song.SongId);
                RefreshSongBrowserPlayIcon(PlayerV4.Playlist.CurrentItem.AudioFile.Id);

                // Refresh playlist window
                if (refreshPlaylistWindow)
                {
                    formPlaylist.RefreshPlaylist();
                }
                //formPlaylist.RefreshPlaylistPlayIcon(Player.CurrentSong.SongId);
            }
            else
            {
                // Nothing is playing, then display "stop" song information                
                lblCurrentArtistName.Text = string.Empty;
                lblCurrentAlbumTitle.Text = string.Empty;
                lblCurrentSongTitle.Text = string.Empty;
                lblCurrentFilePath.Text = string.Empty;
                lblCurrentTime.Text = "0:00.000";
                lblTotalTime.Text = "0:00.000";
                lblSoundFormat.Text = string.Empty;
                lblBitsPerSample.Text = string.Empty;
                lblFrequency.Text = string.Empty;
                picAlbum.Image = null;
                btnPause.Checked = false;
                lblSongPosition.Text = "0:00.000";
                lblSongPercentage.Text = "0 %";
                trackPosition.Value = 0;

                // Empty output meter history and refresh
                outputMeter.WaveDataHistory.Clear();
                outputMeter.Refresh();

                // Refresh play icon
                RefreshSongBrowserPlayIcon(Guid.Empty);
                formPlaylist.RefreshPlaylistPlayIcon(Guid.Empty);
                viewSongs2.ClearSelectedItems();
            }
        }

        /// <summary>
        /// Updates the play icon in the Song Browser. Sets the icon to the
        /// specified song in the newSongId parameter. If the Guid is empty,
        /// the icon will be removed.
        /// </summary>
        /// <param name="newSongId">Song identifier</param>
        public void RefreshSongBrowserPlayIcon(Guid newSongId)
        {
            // Set currently playing song
            viewSongs2.NowPlayingSongId = newSongId;
            viewSongs2.Refresh();
        }

        /// <summary>
        /// Refreshes the Song Browser using the selected query in the Artist/Album browser.
        /// </summary>
        public void RefreshSongBrowser()
        {
            // If no node has been selected
            if (treeLibrary.SelectedNode == null)
            {
                // Filter all songs
                RefreshSongBrowser(new SongQuery());
                return;
            }
           
            // Cast the tree node metadata            
            TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)treeLibrary.SelectedNode.Tag;           

            // Set the current song browser query from the selected node metadata            
            QuerySongBrowser = metadata.Query;

            // Set config
            Config.SongQueryArtistName = QuerySongBrowser.ArtistName;
            Config.SongQueryAlbumTitle = QuerySongBrowser.AlbumTitle;
            Config.SongQueryPlaylistId = QuerySongBrowser.PlaylistId.ToString();

            // Refresh song browser
            RefreshSongBrowser(QuerySongBrowser);
        }

        /// <summary>
        /// Refreshes the Song Browser using the query specified in parameter.
        /// </summary>
        /// <param name="query">Query for Song Browser</param>
        public void RefreshSongBrowser(SongQuery query)
        {
            // Create the list of songs for the browser
            List<SongDTO> songs = null;
            string orderBy = viewSongs2.OrderByFieldName;
            bool orderByAscending = viewSongs2.OrderByAscending;

            // Get query type
            if (query.Type == SongQueryType.Album)
            {
                songs = Library.SelectSongs(FilterSoundFormat, orderBy, orderByAscending, query.ArtistName, query.AlbumTitle);
            }
            else if (query.Type == SongQueryType.Artist)
            {
                songs = Library.SelectSongs(FilterSoundFormat, orderBy, orderByAscending, query.ArtistName);
            }
            else if (query.Type == SongQueryType.Playlist)
            {
                //songs = Library.SelectSongs(query.PlaylistId);
            }
            else if (query.Type == SongQueryType.All)
            {
                songs = Library.SelectSongs(FilterSoundFormat, orderBy, orderByAscending);
            }
            else if (query.Type == SongQueryType.None)
            {
                songs = new List<SongDTO>();
            }

            // Filter songs by media type
            if (comboSoundFormat.Text.ToLower() == "mp3")
            {
                songs = songs.Where(x => x.SoundFormat == "MP3").ToList();
            }
            else if (comboSoundFormat.Text.ToLower() == "flac")
            {
                songs = songs.Where(x => x.SoundFormat == "FLAC").ToList();
            }
            else if (comboSoundFormat.Text.ToLower() == "ogg")
            {
                songs = songs.Where(x => x.SoundFormat == "OGG").ToList();
            }

            // Clear view
            //viewSongs.Items.Clear();
            //viewSongs.Groups.Clear();

            // Make sure the song list is valid
            int a = 0;
            if (songs == null)
            {
                return;
            }

            // Import list of songs into song grid view
            viewSongs2.ImportSongs(songs);            
        }

        /// <summary>
        /// Refreshes the song information in the main window (artist name, album title, song title, etc.)
        /// </summary>        
        public void RefreshSongInformation()
        {
            try
            {
                try
                {
                    // Update the album art in an another thread
                    //workerAlbumArt.RunWorkerAsync(m_playerV4.Playlist.CurrentItem.FilePath);
                    workerAlbumArt.RunWorkerAsync(m_playerV4.Playlist.CurrentItem.AudioFile.FilePath);
                }
                catch
                {
                    // Just do nothing if thread is busy
                }

                //// Check the player playback mode 
                //if (Player.PlaybackMode == PlaybackMode.Playlist)
                //{
                //    // Fetch song from database
                //    MPfm.Library.Data.Song song = DataAccess.SelectSong(filePath);

                //    // Check if song exists
                //    if (song != null)
                //    {
                //        // Update song from tags
                //        song = Player.Library.UpdateSongFromTags(song, true);
                //    }
                //}

                // Set metadata and file path labels
                lblCurrentArtistName.Text = m_playerV4.Playlist.CurrentItem.AudioFile.ArtistName;
                lblCurrentAlbumTitle.Text = m_playerV4.Playlist.CurrentItem.AudioFile.AlbumTitle;
                lblCurrentSongTitle.Text = m_playerV4.Playlist.CurrentItem.AudioFile.Title;
                lblCurrentFilePath.Text = m_playerV4.Playlist.CurrentItem.AudioFile.FilePath;

                // Set format labels
                lblSoundFormat.Text = Path.GetExtension(m_playerV4.Playlist.CurrentItem.AudioFile.FilePath).Replace(".", "").ToUpper();
                lblBitsPerSample.Text = m_playerV4.Playlist.CurrentItem.AudioFile.Bitrate.ToString();
                lblFrequency.Text = m_playerV4.Playlist.CurrentItem.AudioFile.SampleRate.ToString();

                // Set the song length for the Loops & Markers wave form display control
                //waveFormMarkersLoops.Position = m_playerV4.Playlist.CurrentItem.Channel.GetPosition();
                waveFormMarkersLoops.Length = m_playerV4.Playlist.CurrentItem.Channel.GetLength();

                // Load the wave form                
                waveFormMarkersLoops.LoadWaveForm(m_playerV4.Playlist.CurrentItem.AudioFile.FilePath);

                //// Update wave form loops & markers control
                //waveFormMarkersLoops.WaveDataHistory.Clear();               
                //if (workerWaveFormMarkerLoops.IsBusy)
                //{
                //    workerWaveFormMarkerLoops.CancelAsync();
                //}
                //workerWaveFormMarkerLoops.RunWorkerAsync(filePath);
                //waveFormMarkersLoops.IsLoading = true;
                //timerWaveFormLoopsMarkers.Enabled = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            //if (Player.CurrentSong == null)
            //{
            //    // Reset buttons
            //    btnAddMarker.Enabled = false;
            //    return;
            //}

            //// Set button
            //btnAddMarker.Enabled = true;
            
            //// Fetch markers from database
            //List<MPfm.Library.Data.Marker> markers = DataAccess.SelectSongMarkers(Player.CurrentSong.SongId);

            //// Update grid view
            //foreach (MPfm.Library.Data.Marker marker in markers)
            //{
            //    // Create grid view item
            //    ListViewItem item = viewMarkers.Items.Add(marker.Name);
            //    item.Tag = marker.MarkerId;
            //    item.SubItems.Add(marker.Position);
            //    item.SubItems.Add(marker.Comments);
            //    item.SubItems.Add(marker.PositionPCM.ToString());
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
            //if (Player.CurrentSong == null)
            //{
            //    // Reset buttons
            //    btnAddLoop.Enabled = false;
            //    return;
            //}

            //// Set button
            //btnAddLoop.Enabled = true;

            //// Fetch loops from database
            //List<MPfm.Library.Data.Loop> loops = DataAccess.SelectSongLoops(Player.CurrentSong.SongId);
            //List<MPfm.Library.Data.Marker> markers = DataAccess.SelectSongMarkers(Player.CurrentSong.SongId);

            //// Update grid view
            //foreach (MPfm.Library.Data.Loop loop in loops)
            //{
            //    // Create grid view item
            //    ListViewItem item = viewLoops.Items.Add("");
            //    item.Tag = loop.LoopId;
            //    item.SubItems.Add(loop.Name);
            //    item.SubItems.Add(Conversion.MillisecondsToTimeString((ulong)loop.Length));

            //    // Find the markers
            //    MPfm.Library.Data.Marker markerA = markers.FirstOrDefault(x => x.MarkerId == loop.MarkerAId);
            //    MPfm.Library.Data.Marker markerB = markers.FirstOrDefault(x => x.MarkerId == loop.MarkerBId);

            //    // Update marker subitems
            //    item.SubItems.Add(markerA.Position);
            //    item.SubItems.Add(markerB.Position);

            //    // Check if this is the currently playing loop
            //    if (Player.CurrentLoop != null && Player.CurrentLoop.Name == loop.Name)
            //    {
            //        item.ImageIndex = 7;
            //    }
            //}
        }

        /// <summary>
        /// Refreshes the tree view control presenting the library.
        /// </summary>
        public void RefreshTreeLibrary()
        {
            // Declare the selected node
            TreeNode selectedNode = null;

            // Supress repainting the TreeView until we're done (to prevent flicker)
            treeLibrary.BeginUpdate();

            // Make sure the tree is empty
            treeLibrary.Nodes.Clear();

            // Create the main nodes
            nodeAllSongs = new TreeNode("All Songs");
            nodeAllSongs.ImageIndex = 12;
            nodeAllSongs.SelectedImageIndex = 12;
            nodeAllSongs.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.AllSongs, new SongQuery());

            if (QuerySongBrowser.Type == SongQueryType.None)
            {
                selectedNode = nodeAllSongs;
            }

            // Create the artist list node
            nodeAllArtists = new TreeNode("Artists");
            nodeAllArtists.ImageIndex = 16;
            nodeAllArtists.SelectedImageIndex = 16;
            nodeAllArtists.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.AllArtists, new SongQuery());
            nodeAllArtists.Nodes.Add("dummy", "dummy");

            nodeAllAlbums = new TreeNode("Albums");
            nodeAllAlbums.ImageIndex = 17;
            nodeAllAlbums.SelectedImageIndex = 17;
            nodeAllAlbums.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.AllAlbums, new SongQuery());
            nodeAllAlbums.Nodes.Add("dummy", "dummy");

            nodeAllPlaylists = new TreeNode("Playlists");
            nodeAllPlaylists.ImageIndex = 4;
            nodeAllPlaylists.SelectedImageIndex = 4;
            nodeAllPlaylists.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.AllPlaylists, new SongQuery(SongQueryType.None));
            nodeAllPlaylists.Nodes.Add("dummy", "dummy");

            nodeRecentlyPlayed = new TreeNode("Recently Played");
            nodeRecentlyPlayed.ImageIndex = 18;
            nodeRecentlyPlayed.SelectedImageIndex = 18;
            nodeRecentlyPlayed.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.RecentlyPlayed, new SongQuery());

            //if (this.currentSongBrowserQueryType == "RecentlyPlayed")
            //{
            //    selectedNode = nodeRecentlyPlayed;
            //}

            // Add main nodes to the treeview
            treeLibrary.Nodes.Add(nodeAllSongs);
            treeLibrary.Nodes.Add(nodeAllArtists);
            treeLibrary.Nodes.Add(nodeAllAlbums);
            treeLibrary.Nodes.Add(nodeAllPlaylists);
            treeLibrary.Nodes.Add(nodeRecentlyPlayed);

            // Set selected node
            treeLibrary.SelectedNode = selectedNode;

            // Set update done
            treeLibrary.EndUpdate();
        }

        /// <summary>
        /// Refreshes the Playlists node of the tree library, if already expanded.
        /// </summary>
        public void RefreshTreeLibraryPlaylists()
        {
            ////// Check for nulls
            ////if (nodeAllPlaylists == null)
            ////{
            ////    return;
            ////}

            ////// If the node isn't expanded, no need to refresh that node
            ////if (!nodeAllPlaylists.IsExpanded && nodeAllPlaylists.Nodes.Count > 0)
            ////{
            ////    return;
            ////}

            ////// Fetch playlists
            ////List<PlaylistDTO> playlists = Player.Library.SelectPlaylists(false);

            ////// Clear nodes
            ////nodeAllPlaylists.Nodes.Clear();

            ////// For each playlist
            ////foreach (PlaylistDTO playlist in playlists)
            ////{
            ////    // Create tree node
            ////    TreeNode nodePlaylist = new TreeNode();
            ////    nodePlaylist.Text = playlist.PlaylistName;
            ////    nodePlaylist.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.Playlist, new SongQuery(playlist.PlaylistId));
            ////    nodePlaylist.ImageIndex = 11;
            ////    nodePlaylist.SelectedImageIndex = 11;

            ////    // Add node to tree
            ////    nodeAllPlaylists.Nodes.Add(nodePlaylist);

            ////    // If the form is initializing and setting the initial opened node from history...
            ////    if (!IsInitDone && playlist.PlaylistId == InitOpenNodePlaylistId)
            ////    {
            ////        // Set node as selected
            ////        treeLibrary.SelectedNode = nodePlaylist;
            ////    }
            ////}

        }

        /// <summary>
        /// Refreshes the "Repeat" button in the main form toolbar.
        /// </summary>
        public void RefreshRepeatButton()
        {
            string repeatOff = "Repeat (Off)";
            string repeatPlaylist = "Repeat (Playlist)";
            string repeatSong = "Repeat (Song)";

            // Display the repeat type
            if (m_playerV4.RepeatType == RepeatType.Playlist)
            {
                btnRepeat.Text = repeatPlaylist;
                btnRepeat.Checked = true;

                miTrayRepeat.Text = repeatPlaylist;
                miTrayRepeat.Checked = true;
            }
            else if (m_playerV4.RepeatType == RepeatType.Song)
            {
                btnRepeat.Text = repeatSong;
                btnRepeat.Checked = true;

                miTrayRepeat.Text = repeatSong;
                miTrayRepeat.Checked = true;
            }
            else
            {
                btnRepeat.Text = repeatOff;
                btnRepeat.Checked = false;

                miTrayRepeat.Text = repeatOff;
                miTrayRepeat.Checked = false;
            }
        }

        #endregion

        #region Playback Methods

        /// <summary>
        /// Plays the current view in the Artist/Album Browser, starting from the first song of the playlist.
        /// </summary>
        public void PlaySelectedView()
        {
            // Is there at least one item?
            if (viewSongs2.Items.Count > 0)
            {
                // Select the first song
                //viewSongs2.SelectedItems = null;
                viewSongs2.ClearSelectedItems();
                viewSongs2.Items[0].IsSelected = true;

                // Play newly selected song
                PlaySelectedSongQuery();
            }
        }

        /// <summary>
        /// Plays the selected song query in the Song Browser. Refreshes UI controls.
        /// </summary>
        public void PlaySelectedSongQuery()
        {
            PlaySelectedSongQuery(false, 0);
        } 

        /// <summary>
        /// Plays the selected song query in the Song Browser. The playback can be paused to seeked to a specific 
        /// position before playing. Refreshes UI controls.
        /// </summary>
        private void PlaySelectedSongQuery(bool paused, int position)
        {
            // Make sure there is a selected song
            if (viewSongs2.SelectedItems.Count == 0)
            {
                return;
            }

            try
            {
                // Get the song from the tag of the selected item                
                SongDTO currentSong = viewSongs2.SelectedItems[0].Song;

                // Check if song is null
                if (currentSong != null)
                {
                    // Set playback depending on the query in the song browser
                    List<SongDTO> songs = null;
                    if (QuerySongBrowser.Type == SongQueryType.Album)
                    {
                        // Generate an artist/album playlist and start playback
                        songs = Library.SelectSongs(FilterSoundFormat, string.Empty, true, QuerySongBrowser.ArtistName, QuerySongBrowser.AlbumTitle);
                        //Player.PlayAlbum(FilterSoundFormat, QuerySongBrowser.ArtistName, QuerySongBrowser.AlbumTitle, currentSong.SongId);
                    }
                    else if (QuerySongBrowser.Type == SongQueryType.Artist)
                    {
                        // Generate an artist playlist and start playback                                                                        
                        songs = Library.SelectSongs(FilterSoundFormat, string.Empty, true, QuerySongBrowser.ArtistName);
                        //Player.PlayArtist(FilterSoundFormat, QuerySongBrowser.ArtistName, currentSong.SongId);
                    }
                    else if (QuerySongBrowser.Type == SongQueryType.Playlist)
                    {
                        // Play playlist
                        //Player.PlayPlaylist(QuerySongBrowser.PlaylistId);
                    }
                    else if (QuerySongBrowser.Type == SongQueryType.All)
                    {
                        // Generate a playlist with all the library and start playaback
                        songs = Library.SelectSongs(FilterSoundFormat);
                        //Player.PlayAll(FilterSoundFormat, currentSong.SongId);
                    }

                    List<AudioFile> audioFiles = new List<AudioFile>();
                    foreach (SongDTO song in songs)
                    {
                        AudioFile audioFile = new AudioFile(song.FilePath, song.SongId, false);
                        audioFile.ArtistName = song.ArtistName;
                        audioFile.AlbumTitle = song.AlbumTitle;
                        audioFile.Title = song.Time;
                        audioFile.TrackNumber = (uint)song.TrackNumber;
                        audioFile.DiscNumber = (uint)song.DiscNumber;

                        audioFiles.Add(audioFile);
                    }

                    // Clear playlist and add songs
                    m_playerV4.Playlist.Clear();
                    m_playerV4.Playlist.AddItems(audioFiles);
                    //m_playerV4.Playlist.GoTo(currentSong.SongId);
                    m_playerV4.Playlist.GoTo(currentSong.FilePath);
                    m_playerV4.Play();

                    // Refresh song information
                    RefreshSongInformation();

                    // Refresh controls after song playback
                    RefreshSongControls();

                    // Refresh loop and marker controls
                    RefreshMarkers();
                    RefreshLoops();
                    btnAddMarker.Enabled = true;
                    btnAddLoop.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured while loading audio files:\n" + ex.Message, "An error has occured while loading audio files.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Stops playback and refreshes UI controls.
        /// </summary>
        public void Stop()
        {
            // Validate player
            if(m_playerV4 == null || m_playerV4.Playlist == null || !m_playerV4.IsPlaying)
            {
                return;
            }

            // Stop song, wait a little
            m_playerV4.Stop();
            //System.Threading.Thread.Sleep(100);

            // Refresh controls
            btnAddMarker.Enabled = false;
            waveFormMarkersLoops.Clear();
            RefreshSongControls();
            RefreshMarkers();
            RefreshLoops();
            formPlaylist.RefreshPlaylistPlayIcon(Guid.Empty);            
        }

        #endregion

        #region Current Song Panel Events

        /// <summary>
        /// Occurs when the user holds a mouse button on the Song Position trackbar.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void trackPosition_MouseDown(object sender, MouseEventArgs e)
        {            
            songPositionChanging = true;
        }

        /// <summary>
        /// Occurs when the user releases a mouse button on the Song Position trackbar.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void trackPosition_MouseUp(object sender, MouseEventArgs e)
        {
            // Validate player
            if (m_playerV4 == null || !m_playerV4.IsPlaying ||
                m_playerV4.Playlist == null || m_playerV4.Playlist.CurrentItem == null)
            {
                return;
            }

            try
            {
                // Get ratio and set position
                double ratio = (double)trackPosition.Value / 1000;

                // Get length
                int positionBytes = (int)(ratio * (double)m_playerV4.Playlist.CurrentItem.LengthBytes);
                long positionSamples = ConvertAudio.ToPCM(positionBytes, 16, 2);
                long positionMS = ConvertAudio.ToMS(positionSamples, 44100);

                // Set player position
                m_playerV4.SetPosition(positionBytes);

                // Set UI
                lblSongPosition.Text = Conversion.MillisecondsToTimeString((ulong)positionMS);                
                lblSongPercentage.Text = (ratio * 100).ToString("00.00");

                // Set flags
                songPositionChanging = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Occurs when the mouse cursor moves on the Song Position trackbar.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void trackPosition_MouseMove(object sender, MouseEventArgs e)
        {
            // Validate player
            if (m_playerV4 == null || !m_playerV4.IsPlaying ||
                m_playerV4.Playlist == null || m_playerV4.Playlist.CurrentItem == null)
            {
                return;
            }

            // Get ratio
            double ratio = (double)trackPosition.Value / 1000;

            // Check if the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Get time                
                lblSongPosition.Text = Conversion.MillisecondsToTimeString(Convert.ToUInt32((ratio * (double)m_playerV4.Playlist.CurrentItem.LengthMilliseconds)));
                lblSongPercentage.Text = (ratio * 100).ToString("0.00") + " %";
            }
        }

        /// <summary>
        /// Fires when the user scrolls the time shifting slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackTimeShiftingNew_OnTrackBarValueChanged()
        {
            double multiplier = 1 / ((double)trackTimeShifting.Value / 100);

            lblTimeShifting.Text = trackTimeShifting.Value.ToString() + " %";

            PlayerV4.TimeShifting = trackTimeShifting.Value;
        }


        /// <summary>
        /// Fires when the user releases the mouse button on the Volume slider. Saves the final value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackVolume_MouseUp(object sender, MouseEventArgs e)
        {
            //Config["Volume"] = trackVolume.Value.ToString();
            Config.Volume = faderVolume.Value;
        }

        /// <summary>
        /// This button resets the time shifting value to 0%.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void linkResetTimeShifting_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            trackTimeShifting.Value = 0;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Edit Song Metadata" link in the "Actions" panel.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void linkEditSongMetadata_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Check for null
            if (!PlayerV4.IsPlaying)
            {
                return;
            }

            // Open window
            EditSongMetadata(PlayerV4.Playlist.CurrentItem.AudioFile.FilePath);
        }

        /// <summary>
        /// Fires when the volume slider value has changed. Updates the player volume.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void faderVolume_OnFaderValueChanged(object sender, EventArgs e)
        {
            // Set volume and update label            
            m_playerV4.Volume = (float)faderVolume.Value / 100;
            lblVolume.Text = faderVolume.Value.ToString() + " %";
            Config.Volume = faderVolume.Value;

            //Player.Volume = faderVolume.Value;
            //lblVolume.Text = faderVolume.Value.ToString() + " %";
            //Config.Volume = faderVolume.Value;
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
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void linkSearchGuitarTabs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Make sure the player is playing
            if (PlayerV4 != null && PlayerV4.IsPlaying)
            {
                Process.Start("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(PlayerV4.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(PlayerV4.Playlist.CurrentItem.AudioFile.Title) + "+guitar+tab");
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Bass tabs" link in the "Actions" panel.
        /// Opens the default browser and searches for bass tabs featuring the current song.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void linkSearchBassTabs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Make sure the player is playing
            if (PlayerV4 != null && PlayerV4.IsPlaying)
            {
                Process.Start("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(PlayerV4.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(PlayerV4.Playlist.CurrentItem.AudioFile.Title) + "+bass+tab");
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Lyrics" link in the "Actions" panel.
        /// Opens the default browser and searches for lyrics featuring the current song.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void linkSearchLyrics_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Make sure the player is playing
            if (PlayerV4 != null && PlayerV4.IsPlaying)
            {
                Process.Start("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(PlayerV4.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(PlayerV4.Playlist.CurrentItem.AudioFile.Title) + "+lyrics");
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the album art picture box in the "Current Song" panel.
        /// Opens the default browser and searches for album art featuring the current album.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void picAlbum_MouseClick(object sender, MouseEventArgs e)
        {
            // Make sure the player is playing
            if (PlayerV4 != null && PlayerV4.IsPlaying)
            {
                Process.Start("http://www.google.ca/imghp?q=" + HttpUtility.UrlEncode(PlayerV4.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(PlayerV4.Playlist.CurrentItem.AudioFile.AlbumTitle));
            }
        }

        #endregion  

        #endregion

        #region Bookmarks Panel Events

        //private void btnAddBookmark_Click(object sender, EventArgs e)
        //{            
        //    //formEditBookmark.SetEditingMode(false);
        //    //formEditBookmark.ShowDialog(this);
        //}

        #endregion

        #region Song Browser Events

        /// <summary>
        /// Occurs when the user right clicks on an item of the Song Browser.
        /// Opens up a contextual menu if at least an item is selected.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void menuSongBrowser_Opening(object sender, CancelEventArgs e)
        {
            if (viewSongs2.SelectedItems.Count == 0)
            {
                e.Cancel = true;
            }
        }

        ///// <summary>
        ///// Occurs when the user changes the selection on the Song Browser.
        ///// </summary>
        ///// <param name="sender">Event Sender</param>
        ///// <param name="e">Event Arguments</param>
        //private void viewSongs_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    // Check if a selection has been made
        //    bool enabled = true;
        //    if (viewSongs2.SelectedIndices.Count == 0)
        //    {
        //        enabled = false;
        //    }

        //    // Set buttons
        //    if (btnPlaySelectedSong.Enabled != enabled)
        //        btnPlaySelectedSong.Enabled = enabled;
        //    if (btnEditSongMetadata.Enabled != enabled)
        //        btnEditSongMetadata.Enabled = enabled;
        //    if (btnAddSongToPlaylist.Enabled != enabled)
        //        btnAddSongToPlaylist.Enabled = enabled;

        //    // Set selected song in config
        //    if (viewSongs.SelectedIndices.Count > 0)
        //    {
        //        SongDTO song = (SongDTO)viewSongs.SelectedItems[0].Tag;
        //        if (song != null)
        //        {
        //            Config.SongQuerySongId = song.SongId.ToString();
        //        }
        //    }
        //}

        private void viewSongs2_OnSelectedIndexChanged(SongGridViewSelectedIndexChangedData data)
        {
            // Check if a selection has been made
            bool enabled = true;
            if (viewSongs2.SelectedItems.Count == 0)
            {
                enabled = false;
            }

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
                Config.SongQuerySongId = viewSongs2.SelectedItems[0].Song.SongId.ToString();
            }
        }

        /// <summary>
        /// Occurs when the user tries to change the width of a column of the Song Browser.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewSongs_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Occurs when the user double clicks on an item of the Song Browser.
        /// Plays the selected song query.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewSongs_DoubleClick(object sender, EventArgs e)
        {
            PlaySelectedSongQuery();
        }

        private void viewSongs2_DoubleClick(object sender, EventArgs e)
        {
            PlaySelectedSongQuery();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Edit Song Metadata" button on the Song Browser toolbar.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnEditSongMetadata_Click(object sender, EventArgs e)
        {
            // Check if at least one item is selected
            if (viewSongs2.SelectedItems.Count == 0)
            {
                return;
            }

            // Get song from item metadata (check for null)
            SongDTO song = viewSongs2.SelectedItems[0].Song;
            if (song == null)
            {
                return;
            }

            // Open window
            EditSongMetadata(song.FilePath);
        }

        #endregion

        #region Artist/Album Browser Events (Tree Library)

        #region Background worker

        /// <summary>
        /// Fires when the tree library background worker process is started. This process fetches data
        /// from the library.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void workerTreeLibrary_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get arguments
            WorkerTreeLibraryArgs args = (WorkerTreeLibraryArgs)e.Argument;

            // Create result
            WorkerTreeLibraryResult result = new WorkerTreeLibraryResult();
            result.OperationType = args.OperationType;
            result.TreeNodeToUpdate = args.TreeNodeToUpdate;
            result.ArtistName = args.ArtistName;

            // Check what operation needs to be done
            if (args.OperationType == WorkerTreeLibraryOperationType.GetArtistAlbums)
            {
                // Select all albums from artist
                result.Albums = Library.SelectArtistAlbumTitles(args.ArtistName, FilterSoundFormat);
            }
            else if (args.OperationType == WorkerTreeLibraryOperationType.GetArtists)
            {
                // Select all artists
                result.Artists = Library.SelectArtistNames(FilterSoundFormat);
            }
            else if (args.OperationType == WorkerTreeLibraryOperationType.GetAlbums)
            {
                // Select all albums
                result.AllAlbums = Library.SelectAlbumTitles(FilterSoundFormat);
            }
            else if (args.OperationType == WorkerTreeLibraryOperationType.GetPlaylists)
            {
                // Select playlists
                //result.Playlists = Library.SelectPlaylists(false);
            }

            e.Result = result;
        }

        /// <summary>
        /// Fires when the tree library background worker process has finished. It needs to update
        /// the UI to remove the "expanding" message.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void workerTreeLibrary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Get result 
            WorkerTreeLibraryResult result = (WorkerTreeLibraryResult)e.Result;

            // Check if result is valid
            if (result != null && result.TreeNodeToUpdate != null)
            {
                // Supress repainting the TreeView until we're done (to prevent flicker)
                treeLibrary.BeginUpdate();

                // Remove the "expanding" notice and remove the dummy tree node
                result.TreeNodeToUpdate.Text = result.TreeNodeToUpdate.Text.Replace(" (expanding...)", string.Empty);
                result.TreeNodeToUpdate.Nodes.Clear();

                // Check which peration type to do
                if (result.OperationType == WorkerTreeLibraryOperationType.GetArtistAlbums)
                {
                    // For each album
                    foreach (string albumTitle in result.Albums)
                    {
                        // Check if string is valid
                        if (!String.IsNullOrEmpty(albumTitle))
                        {
                            // Create query
                            SongQuery songQuery = new SongQuery(result.ArtistName, albumTitle);

                            // Create tree node
                            TreeNode nodeAlbum = new TreeNode();
                            nodeAlbum.Text = albumTitle;
                            nodeAlbum.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.ArtistAlbum, songQuery);
                            nodeAlbum.ImageIndex = 2;
                            nodeAlbum.SelectedImageIndex = 2;

                            // Add node to tree
                            result.TreeNodeToUpdate.Nodes.Add(nodeAlbum);

                            // If the form is initializing and setting the initial opened node from history...
                            if (!IsInitDone && result.ArtistName == InitOpenNodeArtist && albumTitle == InitOpenNodeArtistAlbum)
                            {
                                // Set node as selected
                                treeLibrary.SelectedNode = nodeAlbum;
                            }
                        }
                    }

                    // Check if init wasn't done yet
                    if (!IsInitDone)
                    {
                        // The ArtistAlbum node needed to be selected. We're done!
                        RefreshSongBrowser();
                        SetInitDone();
                    }
                }
                else if (result.OperationType == WorkerTreeLibraryOperationType.GetArtists)
                {
                    // For each artist
                    foreach (string artistName in result.Artists)
                    {
                        // Check if string is valid
                        if (!String.IsNullOrEmpty(artistName))
                        {
                            // Create query
                            SongQuery songQuery = new SongQuery(artistName);

                            // Create tree node
                            TreeNode nodeArtist = new TreeNode();
                            nodeArtist.Text = artistName;
                            nodeArtist.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.Artist, songQuery);
                            nodeArtist.ImageIndex = 13;
                            nodeArtist.SelectedImageIndex = 13;

                            // Add dummy node
                            nodeArtist.Nodes.Add("dummy", "dummy");

                            // Add node to tree
                            result.TreeNodeToUpdate.Nodes.Add(nodeArtist);

                            // If the form is initializing and setting the initial opened node from history...
                            if (!IsInitDone && artistName == InitOpenNodeArtist)
                            {
                                // Set node as selected
                                treeLibrary.SelectedNode = nodeArtist;
                            }
                        }
                    }

                    // Check if an ArtistAlbum child node needs to be opened
                    if (!IsInitDone && !String.IsNullOrEmpty(InitOpenNodeArtistAlbum))
                    {
                        // The artist node must be expanded
                        treeLibrary.SelectedNode.Expand();
                    }
                    else if (!IsInitDone)
                    {
                        // Only the artist node needed to be selected. We're done!
                        RefreshSongBrowser();
                        SetInitDone();
                    }
                }
                else if (result.OperationType == WorkerTreeLibraryOperationType.GetAlbums)
                {
                    List<string> albums = new List<string>();

                    // For each song                    
                    foreach (KeyValuePair<string, List<string>> keyValue in result.AllAlbums)
                    {
                        foreach (string albumTitle in keyValue.Value)
                        {
                            albums.Add(albumTitle);
                        }
                    }

                    // Order the albums by title
                    albums = albums.OrderBy(x => x).ToList();

                    // For each album
                    foreach (string albumTitle in albums)
                    {
                        // Create query
                        SongQuery songQuery = new SongQuery(string.Empty, albumTitle);

                        // Create tree node
                        TreeNode nodeAlbum = new TreeNode();
                        nodeAlbum.Text = albumTitle;
                        nodeAlbum.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.Album, songQuery);
                        nodeAlbum.ImageIndex = 2;
                        nodeAlbum.SelectedImageIndex = 2;

                        // Add node to tree
                        result.TreeNodeToUpdate.Nodes.Add(nodeAlbum);
                    }

                }
                else if (result.OperationType == WorkerTreeLibraryOperationType.GetPlaylists)
                {
                    // Check result
                    if (result.Playlists != null)
                    {                    
                        // For each playlist
                        foreach (PlaylistDTO playlist in result.Playlists)
                        {
                            // Create tree node
                            TreeNode nodePlaylist = new TreeNode();
                            nodePlaylist.Text = playlist.PlaylistName;
                            nodePlaylist.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.Playlist, new SongQuery(playlist.PlaylistId));
                            nodePlaylist.ImageIndex = 11;
                            nodePlaylist.SelectedImageIndex = 11;

                            // Add node to tree
                            result.TreeNodeToUpdate.Nodes.Add(nodePlaylist);

                            // If the form is initializing and setting the initial opened node from history...
                            if (!IsInitDone && playlist.PlaylistId == InitOpenNodePlaylistId)
                            {
                                // Set node as selected
                                treeLibrary.SelectedNode = nodePlaylist;
                            }
                        }
                    }
                }

                // Expand the updated node
                result.TreeNodeToUpdate.Expand();

                // Check if init wasn't done yet
                if (!IsInitDone)
                {
                    // The ArtistAlbum node needed to be selected. We're done!
                    RefreshSongBrowser();
                    SetInitDone();
                }
            }

            // Set update to end
            treeLibrary.EndUpdate();
        }

        #endregion

        /// <summary>
        /// Occurs when the user clicks on the expand button of a tree node.
        /// This fires a background worker to fetch the data needed for displaying the
        /// artists, albums, playlists, etc.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void treeLibrary_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Check if the arguments are valid
            if (e == null || e.Node == null)
            {
                return;
            }

            // Detect if the child node is a dummy node (indicating we have to fetch the data)            
            if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Text != "dummy")
            {
                // The child nodes have been generated or are static
                return;
            }

            // Cast the tree node metadata
            TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)e.Node.Tag;

            // Check if the metadata is valid
            if (metadata == null)
            {
                return;
            }

            // Is the worker already busy fetching other information?
            if (workerTreeLibrary.IsBusy)
            {
                MessageBox.Show("Error fetch data for the tree library item. A process is already running.\nPlease wait until the process is done before expanding another node.", "Error fetching tree library items!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            // Cancel the expand since we're getting the objects in a background thread
            e.Cancel = true;

            // Set node title to expand
            e.Node.Text = e.Node.Text + " (expanding...)";

            // Create arguments
            WorkerTreeLibraryArgs args = new WorkerTreeLibraryArgs();
            args.TreeNodeToUpdate = e.Node;

            // Check the node type                    
            if (metadata.NodeType == TreeLibraryNodeType.Artist)
            {
                // Fill arguments
                args.OperationType = WorkerTreeLibraryOperationType.GetArtistAlbums;
                args.ArtistName = metadata.Query.ArtistName;
            }
            else if (metadata.NodeType == TreeLibraryNodeType.AllArtists)
            {
                // Fill arguments
                args.OperationType = WorkerTreeLibraryOperationType.GetArtists;
            }
            else if (metadata.NodeType == TreeLibraryNodeType.AllAlbums)
            {
                // Fill arguments
                args.OperationType = WorkerTreeLibraryOperationType.GetAlbums;
            }
            else if (metadata.NodeType == TreeLibraryNodeType.AllPlaylists)
            {
                // Fill arguments
                args.OperationType = WorkerTreeLibraryOperationType.GetPlaylists;
            }

            // Start background worker process
            workerTreeLibrary.RunWorkerAsync(args);
        }

        /// <summary>
        /// Occurs just before the user tries to select an item in the Artist/Album browser.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void treeLibrary_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;

            if (e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.Unknown)
            {
                e.Cancel = false;
            }
        }

        /// <summary>
        /// Occurs just after the user selected an item in the Artist/Album browser.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void treeLibrary_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //if (e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.Unknown)
            if (e.Action == TreeViewAction.ByMouse)
            {
                // Refresh song browser
                RefreshSongBrowser();

                // Set current tree node type in config
                TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)e.Node.Tag;

                if (metadata != null)
                {
                    Config.CurrentNodeType = metadata.NodeType.ToString();
                }
            }
        }

        /// <summary>
        /// Occurs when the user clicks on a node on the Artist/Album browser.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void treeLibrary_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeLibrary.SelectedNode = e.Node;
            }
        }

        /// <summary>
        /// Occurs when the user double clicks on a node on the Artist/Album browser.
        /// Plays the songs from the artist, album or playlist from the item metadata.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void treeLibrary_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Check for null
            if(e.Node == null)
            {
                return;
            }

            // Cast metadata
            TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)e.Node.Tag;

            PlaySelectedView();
            
        }

        /// <summary>
        /// Occurs when the user clicks on the "Play songs" menu item of the Artist/Album browser contextual menu.
        /// Plays the songs from the artist, album or playlist from the item metadata.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miTreeLibraryPlaySongs_Click(object sender, EventArgs e)
        {
            PlaySelectedView();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Delete playlist" menu item of the Artist/Album browser contextual menu.
        /// Deletes the playlist represented by the node.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miTreeLibraryDeletePlaylist_Click(object sender, EventArgs e)
        {
            // Check if the tag is null
            if (miTreeLibraryDeletePlaylist.Tag == null)
            {
                return;
            }

            // Get metadata from tag
            TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)miTreeLibraryDeletePlaylist.Tag;

            // Confirm with user
            if (MessageBox.Show("Are you sure you wish to delete this playlist?", "Delete playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
            {
                // The user has cancelled
                return;
            }

            // Delete playlist
            //DataAccess.DeletePlaylist(metadata.Query.PlaylistId);

            // Refresh playlists
            RefreshTreeLibraryPlaylists();
        }     

        #endregion

        #region Background worker for Album Art

        /// <summary>
        /// Occurs when the background worker for fetching the album art is starting its work.
        /// Gets the album art from the ID3 tags, or folder.jpg, and converts it to a smooth
        /// resized image.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void workerAlbumArt_DoWork(object sender, DoWorkEventArgs e)
        {
            string songPath = (string)e.Argument;

            // Get image from library
            Image image = MPfm.Library.Library.GetAlbumArtFromID3OrFolder(songPath);

            // Check if image is null
            if (image != null)
            {
                // Resize image with quality AA
                image = ImageManipulation.ResizeImage(image, picAlbum.Size.Width, picAlbum.Size.Height);
            }

            e.Result = image;
        }

        /// <summary>
        /// Occurs when the background worker for fetching the album art has finished its work.
        /// Updates the picture box with the album art.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void workerAlbumArt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Get image 
                Image image = (Image)e.Result;

                // Check if image is null
                if (image != null)
                {
                    picAlbum.Image = image;
                }
                else
                {
                    picAlbum.Image = null;
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
        }

        #endregion

        #region System Tray Management

        /// <summary>
        /// Occurs when the user double clicks on the application tray icon.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
                {
                    WindowState = FormWindowState.Normal;
                }

                TopMost = true;
                TopMost = false;
                Focus();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Show MPfm" menu item.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miTrayShowApplication_Click(object sender, EventArgs e)
        {
            ShowApplication();
        }

        #endregion

        /// <summary>
        /// Occurs when the user right clicks on an item of the Artist/Album browser.
        /// Opens up a contextual menu if the user right clicked on a valid item.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void menuLibrary_Opening(object sender, CancelEventArgs e)
        {
            // Get the point where the mouse cursor is
            Point pt = treeLibrary.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));

            // Get the tree node at that point
            TreeNode node = treeLibrary.GetNodeAt(pt.X, pt.Y);

            // Is the node null?
            if (node == null)
            {
                // Set cancel flag
                e.Cancel = true;
                return;
            }

            // Get metadata
            TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)node.Tag;
            if (metadata != null)
            {
                // Check the node type
                if (metadata.NodeType == TreeLibraryNodeType.Playlist)
                {
                    // Show Delete playlist option
                    miTreeLibraryDeletePlaylist.Visible = true;
                }
                else
                {
                    miTreeLibraryDeletePlaylist.Visible = false;
                }
            }

            // Set metadata for later
            miTreeLibraryDeletePlaylist.Tag = metadata;
        }

        /// <summary>
        /// Displays the Update Library Status window and updates the library
        /// using the mode passed in parameter.
        /// </summary>
        public void UpdateLibrary()
        {
            // Create window and display as dialog
            formUpdateLibraryStatus = new frmUpdateLibraryStatus(this);
            formUpdateLibraryStatus.ShowDialog(this);           
        }

        /// <summary>
        /// Occurs when the user changes the sound format filter using the Sound Format combobox.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void comboSoundFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set filter media type
            if (comboSoundFormat.Text == "FLAC")
            {
                FilterSoundFormat = FilterSoundFormat.FLAC;
            }
            else if (comboSoundFormat.Text == "MP3")
            {
                FilterSoundFormat = FilterSoundFormat.MP3;
            }
            else if (comboSoundFormat.Text == "OGG")
            {
                FilterSoundFormat = FilterSoundFormat.OGG;
            }

            // Check if init is done
            if (IsInitDone)
            {
                // Set configuration                
                Config.FilterSoundFormat = FilterSoundFormat.ToString();

                // Refresh all controls
                RefreshAll();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Add songs to playlist" menu item of
        /// the Library contextual menu.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miTreeLibraryAddSongsToPlaylist_Click(object sender, EventArgs e)
        {
            //// Get selected node
            //TreeNode node = treeLibrary.SelectedNode;


            //// Check for null
            //if (node == null)
            //{
            //    return;
            //}

            //// Get the node metadata
            //TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)node.Tag;

            //// Check for null
            //if (metadata == null)
            //{
            //    return;
            //}

            //// Build a playlist based on node type
            //PlaylistDTO playlist = null;
            //if (metadata.NodeType == TreeLibraryNodeType.Artist)
            //{
            //    // Generate playlist from library
            //    playlist = Player.Library.GeneratePlaylistFromArtist(FilterSoundFormat, metadata.Query.ArtistName);
            //}
            //else if (metadata.NodeType == TreeLibraryNodeType.ArtistAlbum)
            //{
            //    // Generate playlist from library
            //    playlist = Player.Library.GeneratePlaylistFromAlbum(FilterSoundFormat, metadata.Query.ArtistName, metadata.Query.AlbumTitle);
            //}

            //// If the playlist is valid, add songs to current playlist
            //if (playlist != null)
            //{
            //    // Add each song to current playlist
            //    //foreach (SongDTO song in playlist.Songs)
            //    foreach(PlaylistSongDTO playlistSong in playlist.Songs)
            //    {
            //        // Add song to playlist
            //        Player.AddSongToPlaylist(playlistSong.Song.SongId);
            //    }

            //    // Refresh playlist window
            //    formPlaylist.RefreshPlaylist();
            //}
        }

        /// <summary>
        /// Occurs when the user types something in the txtSearch textbox. Disables the Search
        /// button when the search query is empty.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                //btnSearch.Enabled = false;
                return;
            }

            //btnSearch.Enabled = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the Add songs to playlist button or menu item in the Song Browser.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnAddSongToPlaylist_Click(object sender, EventArgs e)
        {
            //// Loop through selected items
            //for (int a = 0; a < viewSongs2.SelectedItems.Count; a++)
            //{
            //    // Get the song from the tag of the item
            //    SongDTO currentSong = viewSongs2.SelectedItems[a].Song;

            //    // Check for null
            //    if (currentSong != null)
            //    {
            //        // Add to playlist
            //        Player.AddSongToPlaylist(currentSong.SongId);
            //    }
            //}

            //// Refresh playlists (if there was at least one selected item)
            //if (viewSongs2.SelectedItems.Count > 0)
            //{
            //    formPlaylist.RefreshPlaylist();
            //}
        }  

        /// <summary>
        /// Opens the Edit Song Metadata window with the specified file path to modify.
        /// </summary>
        /// <param name="filePath">File Path</param>
        public void EditSongMetadata(string filePath)
        {
            // Create window and show as dialog
            formEditSongMetadata = new frmEditSongMetadata(this, new List<string>() { filePath });            
            formEditSongMetadata.ShowDialog(this);
        }

        private void panelLoopsMarkers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
            {
                return;
            }

            if (splitLoopsMarkersSongBrowser.SplitterDistance != 22)
            {
                splitLoopsMarkersSongBrowser.SplitterDistance = 22;
            }
            else
            {
                splitLoopsMarkersSongBrowser.SplitterDistance = panelLoopsMarkers.Height;
            }
        }

        /// <summary>
        /// Occurs when the user has clicked on the Markers & Loops Wave Form Display control and
        /// wants to skip to a specific position of the song.
        /// </summary>
        /// <param name="data">Event Data</param>
        private void waveFormMarkersLoops_OnPositionChanged(PositionChangedData data)
        {
            // Check if data is valid
            if (data == null)
            {
                return;
            }

            // Set new position
            m_playerV4.SetPosition(data.Percentage);

            //// Set new position
            //uint newPosition = Player.SetPositionSentenceMS(data.Percentage);

            //// Update song position
            //string time = Conversion.MillisecondsToTimeString(Convert.ToUInt32((data.Percentage * (double)Player.currentSongLength) / 100));
            //lblSongPosition.Text = time;
            //lblSongPercentage.Text = newPosition.ToString();
        }

        #region Markers Button and GridView Events
        
        /// <summary>
        /// Occurs when the user has clicked on the Add Marker button.
        /// Opens the Add/Edit Marker window.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnAddMarker_Click(object sender, EventArgs e)
        {
            // Check if the wave data is loaded
            if (waveFormMarkersLoops.WaveDataHistory.Count > 0)
            {
                // Create window and show as dialog                
                formAddEditMarker = new frmAddEditMarker(this, AddEditMarkerWindowMode.Add, PlayerV4.Playlist.CurrentItem, Guid.Empty);
                formAddEditMarker.ShowDialog(this);
            }
        }

        /// <summary>
        /// Occurs when the user has clicked on the Edit Marker button.
        /// Opens the Add/Edit Marker window.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
            //formAddEditMarker = new frmAddEditMarker(this, AddEditMarkerWindowMode.Edit, Player.CurrentSong, markerId);
            //formAddEditMarker.ShowDialog(this);
        }

        /// <summary>
        /// Occurs when the user has clicked on the Remove Marker button.
        /// Confirms with the user the deletion of the marker.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnRemoveMarker_Click(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (viewMarkers.SelectedItems.Count == 0)
            {
                return;
            }

            // Confirm with the user
            if (MessageBox.Show("Are you sure you wish to remove the '" + viewMarkers.SelectedItems[0].Text + "' marker?", "Remove marker confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                // Get selected markerId
                Guid markerId = new Guid(viewMarkers.SelectedItems[0].Tag.ToString());

                // Remove marker and refresh list                
                //DataAccess.DeleteMarker(markerId);
                RefreshMarkers();
            }
        }

        /// <summary>
        /// Occurs when the user has clicked on the Go To Marker button.
        /// Sets the player position as the currently selecter marker position.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
            //uint.TryParse(viewMarkers.SelectedItems[0].SubItems[2].Text, out position);

            //// Set player position
            //m_player.MainChannel.SetPosition(position, FMOD.TIMEUNIT.SENTENCE_PCM);
        }

        /// <summary>
        /// Occurs when the user double-clicks on the Marker grid view.
        /// Sets the player position as the currently selecter marker position.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
            //m_player.MainChannel.SetPosition(position, FMOD.TIMEUNIT.SENTENCE_PCM);
        }

        /// <summary>
        /// Occurs when the user changes the selection on the Markers grid view.
        /// Enables/disables marker buttons.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewMarkers_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // Enable/disable marker buttons
            if (viewMarkers.SelectedItems.Count == 0)
            {
                btnEditMarker.Enabled = false;
                btnRemoveMarker.Enabled = false;
                btnGoToMarker.Enabled = false;
            }
            else
            {
                btnEditMarker.Enabled = true;
                btnRemoveMarker.Enabled = true;
                btnGoToMarker.Enabled = true;
            }
        }

        #endregion

        #region Loops Button and GridView Events

        /// <summary>
        /// Occurs when the user has clicked on the Add Loop button.
        /// Opens the Add/Edit Loop window.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnAddLoop_Click(object sender, EventArgs e)
        {
            //// Check if the wave data is loaded
            //if (waveFormMarkersLoops.WaveDataHistory.Count == 0)
            //{
            //    return;
            //}

            //// Check if there are at least two markers
            //if (viewMarkers.Items.Count < 2)
            //{
            //    // Display message
            //    MessageBox.Show("You must add at least two markers before adding a loop.", "Error adding loop", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //// Create window and show as dialog
            //formAddEditLoop = new frmAddEditLoop(this, AddEditLoopWindowMode.Add, Player.CurrentSong, Guid.Empty);
            //formAddEditLoop.ShowDialog(this);            
        }

        /// <summary>
        /// Occurs when the user has clicked on the Edit Loop button.
        /// Opens the Add/Edit Loop window.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
            //formAddEditLoop = new frmAddEditLoop(this, AddEditLoopWindowMode.Edit, Player.CurrentSong, loopId);
            //formAddEditLoop.ShowDialog(this);
        }

        /// <summary>
        /// Occurs when the user has clicked on the Remove Loop button.
        /// Confirms with the user the deletion of the loop.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnRemoveLoop_Click(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (viewLoops.SelectedItems.Count == 0)
            {
                return;
            }

            // Confirm with the user
            if (MessageBox.Show("Are you sure you wish to remove the '" + viewLoops.SelectedItems[0].SubItems[1].Text + "' loop?", "Remove loop confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                // Get selected markerId
                Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

                // Remove marker and refresh list                
                //DataAccess.DeleteLoop(loopId);
                RefreshLoops();
            }
        }

        /// <summary>
        /// Occurs when the user has clicked on the Play Loop button.
        /// Starts the playback of a loop.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
            //MPfm.Library.Data.Loop loop = DataAccess.SelectLoop(loopId);

            //// Check if the loop is valid
            //if (loop == null)
            //{
            //    return;
            //}

            //// Set current loop in player
            //Player.CurrentLoop = loop;

            //// Set currently playing loop icon
            //for (int a = 0; a < viewLoops.Items.Count; a++)
            //{
            //    // Check if the loop is currently playing
            //    if (viewLoops.Items[a].Tag.ToString() == loop.LoopId)
            //    {
            //        // Set flag
            //        viewLoops.Items[a].ImageIndex = 7;
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
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnStopLoop_Click(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewLoops.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Reset loop
            //Player.CurrentLoop = null;

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
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
            //    if(Player.CurrentLoop != null)
            //    {
            //        // Check if the loop matches
            //        if (viewLoops.SelectedItems[0].Tag.ToString() == Player.CurrentLoop.LoopId)
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
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewLoops_DoubleClick(object sender, EventArgs e)
        {
            //// Check if an item is selected
            //if (viewLoops.SelectedItems.Count == 0)
            //{
            //    return;
            //}

            //// Get selected loopId
            //Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

            //// Fetch loop from database
            //MPfm.Library.Data.Loop loop = DataAccess.SelectLoop(loopId);

            //// Check if the loop is valid
            //if (loop == null)
            //{
            //    return;
            //}

            //// Set current loop in player
            //Player.CurrentLoop = loop;
            
            //// Set currently playing loop icon
            //for (int a = 0; a < viewLoops.Items.Count; a++)
            //{
            //    // Check if the loop is currently playing
            //    if (viewLoops.Items[a].Tag.ToString() == loop.LoopId)
            //    {
            //        // Set flag
            //        viewLoops.Items[a].ImageIndex = 7;
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

        private void viewSongs2_OnColumnClick(SongGridViewColumnClickData data)
        {


            RefreshSongBrowser();
        }
    }

    #region Legacy

    #region Bookmarks (legacy)

    //public void RefreshBookmarks()
    //{
    //    //List<BookmarkDTO> bookmarks = null;

    //    //bookmarks = db.SelectBookmarksBySong(currentSong.SongId);

    //    //viewBookmarks.Items.Clear();

    //    //if (bookmarks != null)
    //    //{
    //    //    foreach (BookmarkDTO bookmark in bookmarks)
    //    //    {
    //    //        ListViewItem item = new ListViewItem(bookmark.Name);
    //    //        item.Tag = bookmark.BookmarkId.ToString();
    //    //        item.SubItems.Add(bookmark.Time);
    //    //        item.SubItems.Add(bookmark.AbsoluteTime.ToString() + " ms");
    //    //        item.SubItems.Add(bookmark.Comments);

    //    //        viewBookmarks.Items.Add(item);
    //    //    }
    //    //}
    //}

    //private void EditBookmark()
    //{
    //    //if(viewBookmarks.SelectedItems.Count == 0)
    //    //{
    //    //    return;
    //    //}

    //    //BookmarkDTO bookmark = db.SelectBookmark(Convert.ToInt32(viewBookmarks.SelectedItems[0].Tag));

    //    //if (bookmark == null)
    //    //{
    //    //    return;
    //    //}

    //    //formEditBookmark.SetEditingMode(true);
    //    //formEditBookmark.LoadBookmark(bookmark);
    //    //formEditBookmark.Show();
    //}

    //public void GoToSelectedBookmark()
    //{
    //    //BookmarkDTO bookmark = null;

    //    //if (viewBookmarks.SelectedItems.Count > 0)
    //    //{
    //    //    bookmark = db.SelectBookmark(Convert.ToInt32(viewBookmarks.SelectedItems[0].Tag));

    //    //    if (bookmark != null)
    //    //    {
    //    //        if (soundSystem.MainChannel.IsInitialized && soundSystem.MainChannel.IsPlaying)
    //    //        {
    //    //            soundSystem.MainChannel.PositionAbsoluteMilliseconds = (uint)bookmark.AbsoluteTime;
    //    //        }
    //    //    }
    //    //}
    //}

    //private void DeleteSelectedBookmarks()
    //{
    //    //if (viewBookmarks.SelectedItems.Count == 0)
    //    //{
    //    //    return;
    //    //}

    //    //if (MessageBox.Show("Are you sure you wish to delete the selected bookmark(s)?", "Deleting selected bookmark(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
    //    //{
    //    //    return;
    //    //}

    //    //foreach (ListViewItem item in viewBookmarks.SelectedItems)
    //    //{
    //    //    db.DeleteBookmark(Convert.ToInt32(item.Tag));                
    //    //}

    //    //RefreshBookmarks();
    //    //RefreshBookmarksToolbar();
    //}

    //private void SetTime()
    //{
    //    //BookmarkDTO bookmark = null;

    //    //if (viewBookmarks.SelectedItems.Count > 0)
    //    //{
    //    //    bookmark = db.SelectBookmark(Convert.ToInt32(viewBookmarks.SelectedItems[0].Tag));

    //    //    if (bookmark != null)
    //    //    {
    //    //        bookmark.AbsoluteTime = Convert.ToInt32(soundSystem.MainChannel.PositionAbsoluteMilliseconds);
    //    //        bookmark.Time = soundSystem.MainChannel.Position;
    //    //        db.UpdateBookmark(bookmark);

    //    //        viewBookmarks.SelectedItems[0].SubItems[1].Text = bookmark.Time;
    //    //        viewBookmarks.SelectedItems[0].SubItems[2].Text = bookmark.AbsoluteTime.ToString() + " ms";
    //    //    }
    //    //}
    //}

    #endregion

    #region Tabs and Lyrics (legacy)

    //private void btnAddTab_Click(object sender, EventArgs e)
    //{
    //    //formEditTab.SetEditingMode(false);
    //    //formEditTab.Show(this);
    //}

    //private void btnEditTab_Click(object sender, EventArgs e)
    //{
    //    EditTab();
    //}

    //private void miEditTab_Click(object sender, EventArgs e)
    //{
    //    EditTab();
    //}

    //private void viewTabs_MouseDoubleClick(object sender, MouseEventArgs e)
    //{
    //    EditTab();
    //}

    //private void EditTab()
    //{
    //    //if (viewTabs.SelectedItems.Count == 0)
    //    //{
    //    //    return;
    //    //}

    //    //TabDTO tab = db.SelectTab(Convert.ToInt32(viewTabs.SelectedItems[0].Tag));

    //    //if (tab == null)
    //    //{
    //    //    return;
    //    //}

    //    //formEditTab.SetEditingMode(true);
    //    //formEditTab.LoadTab(tab);
    //    //formEditTab.Show(this);
    //}

    //private void btnShowTab_Click(object sender, EventArgs e)
    //{

    //}

    //public void RefreshTabs()
    //{
    //    //List<TabDTO> tabs = null;

    //    //tabs = db.SelectTabsBySong(currentSong.SongId);

    //    //viewTabs.Items.Clear();

    //    //if (tabs != null)
    //    //{
    //    //    foreach (TabDTO tab in tabs)
    //    //    {
    //    //        ListViewItem item = new ListViewItem(tab.TabName);
    //    //        item.Tag = tab.TabId.ToString();

    //    //        if (tab.TabType == 1)
    //    //        {
    //    //            item.SubItems.Add("Guitar Tab");
    //    //        }
    //    //        else if (tab.TabType == 2)
    //    //        {
    //    //            item.SubItems.Add("Bass Tab");
    //    //        }
    //    //        else if (tab.TabType == 3)
    //    //        {
    //    //            item.SubItems.Add("Drum Tab");
    //    //        }
    //    //        else
    //    //        {
    //    //            item.SubItems.Add("Lyrics");
    //    //        }

    //    //        item.ImageIndex = 9;

    //    //        viewTabs.Items.Add(item);
    //    //    }
    //    //}
    //}

    //public void RefreshTabsToolbar()
    //{
    //    //bool enabled = true;
    //    //if (viewTabs.SelectedIndices.Count == 0)
    //    //{
    //    //    enabled = false;
    //    //}

    //    //btnEditTab.Enabled = enabled;
    //    //btnDeleteTab.Enabled = enabled;
    //}

    //private void viewTabs_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    RefreshTabsToolbar();               
    //}

    //private void miDeleteTab_Click(object sender, EventArgs e)
    //{
    //    DeleteSelectedTabs();
    //}

    //private void btnDeleteTab_Click(object sender, EventArgs e)
    //{
    //    DeleteSelectedTabs();
    //}

    //private void DeleteSelectedTabs()
    //{
    //    //if (viewTabs.SelectedItems.Count == 0)
    //    //{
    //    //    return;
    //    //}

    //    //if (MessageBox.Show("Are you sure you wish to delete the selected tab(s)?", "Deleting selected tab(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
    //    //{
    //    //    return;
    //    //}

    //    //foreach (ListViewItem item in viewTabs.SelectedItems)
    //    //{
    //    //    db.DeleteTab(Convert.ToInt32(item.Tag));
    //    //}

    //    //RefreshTabs();
    //    //RefreshTabsToolbar();
    //}

    //private void menuTabs_Opening(object sender, CancelEventArgs e)
    //{
    //    //if (viewTabs.SelectedItems.Count == 0)
    //    //{
    //    //    e.Cancel = true;
    //    //}
    //}

    #endregion

    #region Loops (legacy)

    //private void btnAddLoop_Click(object sender, EventArgs e)
    //{
    //    //formEditLoop.SetEditingMode(false);
    //    //formEditLoop.Show(this);
    //}

    //private void btnEditLoop_Click(object sender, EventArgs e)
    //{

    //}

    //private void btnDeleteLoop_Click(object sender, EventArgs e)
    //{

    //}

    //public void RefreshLoops()
    //{
    //}

    #endregion

    #endregion

    #region Classes and enums

    /// <summary>
    /// Defines the data structure for reporting progress when generating a wave form
    /// for the Loops & Markers UI.
    /// </summary>
    public class WorkerWaveFormLoopsMarkersReportProgress
    {
        public uint BytesRead { get; set; }
        public uint TotalBytes { get; set; }
        public float PercentageDone { get; set; }
        public WaveDataMinMax WaveDataMinMax { get; set; }
    }

    /// <summary>
    /// Defines the arguments passed to the background worker of the tree library. This
    /// allows the background worker to get the type of operation it needs to do.
    /// </summary>
    public class WorkerTreeLibraryArgs
    {
        public WorkerTreeLibraryOperationType OperationType { get; set; }
        public TreeNode TreeNodeToUpdate { get; set; }
        public string ArtistName { get; set; }
    }
    /// <summary>
    /// Defines the results coming out of the background worker of the tree library.
    /// </summary>
    public class WorkerTreeLibraryResult
    {
        public WorkerTreeLibraryOperationType OperationType { get; set; }
        public TreeNode TreeNodeToUpdate { get; set; }
        public string ArtistName { get; set; }
        public List<string> Albums { get; set; }
        public List<string> Artists { get; set; }
        public List<PlaylistDTO> Playlists { get; set; }
        // Key = ArtistName -- Values = albums
        public Dictionary<string, List<string>> AllAlbums { get; set; }
    }
    /// <summary>
    /// Defines what kind of operation the background worker process needs to do.
    /// </summary>
    public enum WorkerTreeLibraryOperationType
    {
        GetArtists = 0, GetArtistAlbums = 1, GetAlbums = 2, GetPlaylists = 3
    }

    /// <summary>
    /// Defines what the tree library node represents (artist, album, playlist, etc.)
    /// </summary>
    public enum TreeLibraryNodeType
    {
        All = 0, AllSongs = 1, AllArtists = 2, AllAlbums = 3, AllPlaylists = 4, Artist = 5, Album = 6, ArtistAlbum = 7, Playlist = 8, RecentlyPlayed = 9
    }

    /// <summary>
    /// Data structure used with the Tag property of the tree library TreeNode object. 
    /// Contains the type of node and its query.
    /// </summary>
    public class TreeLibraryNodeMetadata
    {
        public TreeLibraryNodeType NodeType { get; set; }
        public SongQuery Query { get; set; }       

        /// <summary>
        /// Default constructor for the TreeLibraryNodeMetadata class.
        /// </summary>
        public TreeLibraryNodeMetadata()
        {
        }

        /// <summary>
        /// Constructor for the TreeLibraryNodeMetadata class. Requires the
        /// node type and query.
        /// </summary>
        /// <param name="nodeType">Node type</param>
        /// <param name="query">Query</param>
        public TreeLibraryNodeMetadata(TreeLibraryNodeType nodeType, SongQuery query)
        {
            NodeType = nodeType;
            Query = query;
        }
    }

    #endregion

}