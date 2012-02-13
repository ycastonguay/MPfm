//
// frmMain.cs: Main form for the MPfm application. Contains the Artist Browser, Song Browser,
//             Current Song Panel, Playback controls, etc.
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
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Library;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.WindowsControls;
using MPfm.Player;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Threading;

namespace MPfm
{
    /// <summary>
    /// Main form for the MPfm application. Contains the Artist Browser, Song Browser,
    /// Current Song Panel, Playback controls, etc.
    /// </summary>
    public partial class frmMain : MPfm.WindowsControls.Form
    {
        // Private variables
        private Stream fileTracing = null;
        private TextWriterTraceListener textTraceListener = null;
        private string m_configurationFilePath = string.Empty;
        private string m_databaseFilePath = string.Empty;
        private string m_logFilePath = string.Empty;
        private string m_initOpenNodeArtist = string.Empty;
        private string m_initOpenNodeArtistAlbum = string.Empty;
        private string m_initOpenNodeAlbum = string.Empty;
        private AudioFileFormat m_filterAudioFileFormat = AudioFileFormat.Unknown;
        private bool m_songPositionChanging = false;
        private SongQuery m_querySongBrowser = null;

        #region Properties
         
        /// <summary>
        /// Private value for the ApplicationDataFolderPath property.
        /// </summary>
        private string m_applicationDataFolderPath = string.Empty;
        /// <summary>
        /// Indicates the application data folder path.
        /// </summary>
        public string ApplicationDataFolderPath
        {
            get
            {
                return m_applicationDataFolderPath;
            }
        }

        /// <summary>
        /// Private value for the PeakFileFolderPath property.
        /// </summary>
        private string m_peakFileFolderPath = string.Empty;
        /// <summary>
        /// Indicates the peak file folder path.
        /// </summary>
        public string PeakFileFolderPath
        {
            get
            {
                return m_peakFileFolderPath;
            }
            set
            {
                m_peakFileFolderPath = value;
                waveFormMarkersLoops.PeakFileDirectory = value;
            }
        }

        /// <summary>
        /// Private value for the IsInitDone property.
        /// </summary>
        private bool m_isInitDone = false;
        /// <summary>
        /// Defines if the initialization process is done.
        /// </summary>
        public bool IsInitDone
        {
            get
            {
                return m_isInitDone;
            }
        }
        
        /// <summary>
        /// "Update Library Status" form.
        /// </summary>
        public frmUpdateLibraryStatus formUpdateLibraryStatus = null;        
        /// <summary>
        /// "Effects" form.
        /// </summary>
        public frmEffects formEffects = null;
        /// <summary>
        /// "Settings" form.
        /// </summary>
        public frmSettings formSettings = null;
        /// <summary>
        /// "Playlist" form.
        /// </summary>
        public frmPlaylist formPlaylist = null;
        /// <summary>
        /// "Edit Song Metadata" form.
        /// </summary>
        public frmEditSongMetadata formEditSongMetadata = null;
        /// <summary>
        /// "Add/Edit Marker" form.
        /// </summary>
        public frmAddEditMarker formAddEditMarker = null;
        /// <summary>
        /// "Add/Edit Loop" form.
        /// </summary>
        public frmAddEditLoop formAddEditLoop = null;
        /// <summary>
        /// "Visualizer" form.
        /// </summary>
        public frmVisualizer formVisualizer = null;

        /// <summary>
        /// "All songs" library tree node.
        /// </summary>
        public TreeNode nodeAllSongs = null;
        /// <summary>
        /// "All artists" library tree node.
        /// </summary>
        public TreeNode nodeAllArtists = null;
        /// <summary>
        /// "All albums" library tree node.
        /// </summary>
        public TreeNode nodeAllAlbums = null;
        /// <summary>
        /// "All playlists" library tree node.
        /// </summary>
        public TreeNode nodeAllPlaylists = null;
        /// <summary>
        /// "Recently played" library tree node.
        /// </summary>
        public TreeNode nodeRecentlyPlayed = null;

        /// <summary>
        /// Timer for updating song position.
        /// </summary>
        public System.Windows.Forms.Timer m_timerSongPosition = null;

        /// <summary>
        /// Private value for the Library property.
        /// </summary>
        private MPfm.Library.Library m_library = null;
        /// <summary>
        /// The Library property contains the audio file library cache and updates the library.
        /// </summary>
        public MPfm.Library.Library Library
        {
            get
            {
                return m_library;
            }
        }

        /// <summary>
        /// Private value for the Player property.
        /// </summary>
        private MPfm.Player.Player m_player = null;
        /// <summary>
        /// This is the playback engine for MPfm.
        /// </summary>
        public MPfm.Player.Player Player
        {
            get
            {
                return m_player;
            }
        }

        /// <summary>
        /// Private value for the Config property.
        /// </summary>
        private MPfmConfiguration m_config = null;
        /// <summary>
        /// This contains the configuration values for MPfm.
        /// </summary>
        public MPfmConfiguration Config
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Load configuration
            try
            {
                // Get assembly version
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Set form title                
                this.Text = "MPfm: Music Player for Musicians - " + assembly.GetName().Version.ToString();

                // Get application data folder path
                // Vista/Windows7: C:\Users\%username%\AppData\Roaming\
                // XP: C:\Documents and Settings\%username%\Application Data\
                m_applicationDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MPfm";

                // Check if the folder exists
                if (!Directory.Exists(m_applicationDataFolderPath))
                {
                    // Create directory                    
                    frmSplash.SetStatus("Creating application data folder...");
                    Directory.CreateDirectory(m_applicationDataFolderPath);
                }

                // Set paths
                m_configurationFilePath = m_applicationDataFolderPath + "\\MPfm.Configuration.xml";
                m_databaseFilePath = m_applicationDataFolderPath + "\\MPfm.Database.db";
                m_logFilePath = m_applicationDataFolderPath + "\\MPfm.Log.txt";

                // Set control paths
                waveFormMarkersLoops.PeakFileDirectory = m_peakFileFolderPath + "\\";

                // Initialize tracing
                frmSplash.SetStatus("Main form init -- Initializing tracing...");
            
                // Check if trace file exists
                if (!File.Exists(m_logFilePath))
                {
                    // Create file
                    fileTracing = File.Create(m_logFilePath);
                }
                else
                {
                    try
                    {
                        // Open file
                        fileTracing = File.Open(m_logFilePath, FileMode.Append);
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

                // Output paths
                Tracing.Log("Main form init -- Application data folder: " + m_applicationDataFolderPath);
                Tracing.Log("Main form init -- Configuration file path: " + m_configurationFilePath);
                Tracing.Log("Main form init -- Database file path: " + m_databaseFilePath);                
                Tracing.Log("Main form init -- Log file path: " + m_logFilePath);                

                // Register BASS.NET with key
                Tracing.Log("Main form init -- Registering BASS.NET...");
                Base.Register("yanick.castonguay@gmail.com", "2X3433427152222");
               
                // Create configuration with default settings\
                Tracing.Log("Main form init -- Loading configuration...");
                frmSplash.SetStatus("Loading configuration...");                
                m_config = new MPfmConfiguration(m_configurationFilePath);

                // Check if the configuration file exists
                if (File.Exists(m_configurationFilePath))
                {
                    // Load configuration values
                    m_config.Load();

                    // Load peak file options
                    bool? peakFileUseCustomDirectory = Config.GetKeyValueGeneric<bool>("PeakFile_UseCustomDirectory");
                    string peakFileCustomDirectory = Config.GetKeyValue("PeakFile_CustomDirectory");

                    // Set peak file directory
                    if (peakFileUseCustomDirectory.HasValue && peakFileUseCustomDirectory.Value)
                    {
                        // Set custom peak file directory
                        PeakFileFolderPath = peakFileCustomDirectory;                        
                    }
                    else
                    {
                        // Set default peak file directory
                        PeakFileFolderPath = m_applicationDataFolderPath + "\\Peak Files\\";
                    }
                }
                else
                {
                    // Set default peak file directory
                    PeakFileFolderPath = m_applicationDataFolderPath + "\\Peak Files\\";
                }

                // Check if the peak folder exists
                if (!Directory.Exists(PeakFileFolderPath))
                {
                    // Create directory                    
                    frmSplash.SetStatus("Creating peak file folder...");
                    Directory.CreateDirectory(PeakFileFolderPath);
                }

                try
                {
                    // Check if the database file exists
                    if (!File.Exists(m_databaseFilePath))
                    {                    
                        // Create database file
                        frmSplash.SetStatus("Creating database file...");
                        MPfm.Library.Library.CreateDatabaseFile(m_databaseFilePath);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error initializing MPfm: Could not create database file!", ex);
                }

                try
                {
                    // Check current database version
                    string databaseVersion = MPfm.Library.Library.GetDatabaseVersion(m_databaseFilePath);

                    // Extract major/minor
                    string[] currentVersionSplit = databaseVersion.Split('.');

                    // Check integrity of the setting value (should be split in 2)
                    if (currentVersionSplit.Length != 2)
                    {
                        throw new Exception("Error fetching database version; the setting value is invalid!");
                    }

                    int currentMajor = 0;
                    int currentMinor = 0;
                    int.TryParse(currentVersionSplit[0], out currentMajor);
                    int.TryParse(currentVersionSplit[1], out currentMinor);

                    // Is this earlier than 1.04?
                    if (currentMajor == 1 && currentMinor < 4)
                    {
                        // Set buffer size
                        Config.Audio.Mixer.BufferSize = 1000;
                    }

                    // Check if the database needs to be updated
                    MPfm.Library.Library.CheckIfDatabaseVersionNeedsToBeUpdated(m_databaseFilePath);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error initializing MPfm: The MPfm database could not be updated!", ex);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("Configuration error:" + ex.Message);
                frmSplash.SetError("Configuration error: " + ex.Message);
            }

            try
            {
                // Create player
                Tracing.Log("Main form init -- Loading player...");
                frmSplash.SetStatus("Loading player...");

                m_player = new MPfm.Player.Player(new Device(), Config.Audio.Mixer.Frequency, Config.Audio.Mixer.BufferSize, Config.Audio.Mixer.UpdatePeriod, false);
                m_player.OnPlaylistIndexChanged += new Player.Player.PlaylistIndexChanged(m_player_OnPlaylistIndexChanged);                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Check if it's the first time the user runs the application
            if (Config.GetKeyValueGeneric<bool>("FirstRun") == null ||
                Config.GetKeyValueGeneric<bool>("FirstRun") == true)
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
                    Config.SetKeyValue<bool>("FirstRun", false);
                }

                // Save initial configuration
                Config.Save();
            }
            
            // Create player
            try
            {
                Device device = null;
                Tracing.Log("Main form init -- Initializing device...");
                frmSplash.SetStatus("Initializing device...");
                
                // Get configuration values
                DriverType driverType = Config.Audio.DriverType;
                string deviceName = Config.Audio.Device.Name;

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

                // Initialize device
                m_player.InitializeDevice(device, Config.Audio.Mixer.Frequency);

                // Create timer
                m_timerSongPosition = new System.Windows.Forms.Timer();
                m_timerSongPosition.Interval = 10;
                m_timerSongPosition.Tick += new EventHandler(m_timerSongPosition_Tick);
                m_timerSongPosition.Enabled = true;
            }
            catch (Exception ex)
            {
                // Set error in splash and hide splash
                frmSplash.SetStatus("Error initializing device!");
                frmSplash.HideSplash();

                // Display message box with error
                this.TopMost = true;
                MessageBox.Show("There was an error while initializing the player device.\nYou can delete the MPfm.Configuration.xml file in the MPfm application data folder (" + m_applicationDataFolderPath + ") to reset the configuration and display the First Run screen.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error initializing player!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracing.Log("Main form init -- Player init error: " + ex.Message + "\nStack trace: " + ex.StackTrace);
                
                // Exit application
                Application.Exit();
                return;
            }

            try
            {
                // Load library
                Tracing.Log("Main form init -- Loading library...");
                frmSplash.SetStatus("Loading library...");                
                m_library = new Library.Library(m_databaseFilePath);
            }
            catch (Exception ex)
            {
                // Set error in splash and hide splash
                frmSplash.SetStatus("Error initializing library!");
                frmSplash.HideSplash();

                // Display message box with error
                this.TopMost = true;
                MessageBox.Show("There was an error while initializing the library.\nYou can delete the MPfm.Database.db file in the MPfm application data folder (" + m_applicationDataFolderPath + ") to reset the library.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error initializing library!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracing.Log("Main form init -- Library init error: " + ex.Message + "\nStack trace: " + ex.StackTrace);
                
                // Exit application
                Application.Exit();
                return;
            }

            // Load UI
            try
            {
                Tracing.Log("Main form init -- Loading UI...");
                frmSplash.SetStatus("Loading UI...");

                Tracing.Log("Main form init -- Loading UI - Effects...");
                formEffects = new frmEffects(this);

                Tracing.Log("Main form init -- Loading UI - Settings...");
                formSettings = new frmSettings(this);

                Tracing.Log("Main form init -- Loading UI - Playlist...");
                formPlaylist = new frmPlaylist(this);

                Tracing.Log("Main form init -- Loading UI - Visualizer...");
                formVisualizer = new frmVisualizer(this);

                // Hide update library progress by default
                ShowUpdateLibraryProgress(false);
            }
            catch (Exception ex)
            {
                // Set error in splash and hide splash
                frmSplash.SetStatus("Error initializing UI!");
                frmSplash.HideSplash();

                // Display message box with error
                this.TopMost = true;
                MessageBox.Show("There was an error while initializing the UI.\nYou can delete the MPfm.Configuration.xml file in the MPfm application data folder (" + m_applicationDataFolderPath + ") to reset the configuration and display the First Run screen.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error initializing player!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracing.Log("UI error: " + ex.Message + "\nStack trace: " + ex.StackTrace);

                // Exit application
                Application.Exit();
                return;
            }

            try
            {
                Tracing.Log("Main form init -- Applying configuration...");
                frmSplash.SetStatus("Applying configuration...");


                // Resetting display
                lblCurrentAlbumTitle.Text = "";
                lblCurrentArtistName.Text = "";
                lblCurrentSongTitle.Text = "";
                lblCurrentFilePath.Text = "";
                lblBitrate.Text = "";
                lblSoundFormat.Text = "";
                lblFrequency.Text = "";

                // Load window configuration (position, size, column sizes, etc.)
                LoadWindowConfiguration();

                // Reset song query
                m_querySongBrowser = new SongQuery();

                // Get query if available
                string queryArtistName = Config.Controls.SongGridView.Query.ArtistName;
                string queryAlbumTitle = Config.Controls.SongGridView.Query.AlbumTitle;
                string queryPlaylistId = Config.Controls.SongGridView.Query.PlaylistId.ToString();
                string querySongId = Config.Controls.SongGridView.Query.AudioFileId.ToString();
                string currentNodeType = Config.Controls.SongGridView.Query.NodeType.ToString();

                // Get media type filter configuration and set media type before refreshing the tree library
                string filterSoundFormat = Config.GetKeyValue("FilterSoundFormat");

                // Populate the supported formats
                comboSoundFormat.Items.Clear();
                comboSoundFormat.Items.Add("All");
                comboSoundFormat.Items.Add("MP3");
                comboSoundFormat.Items.Add("FLAC");
                comboSoundFormat.Items.Add("OGG");
                comboSoundFormat.Items.Add("MPC");
                comboSoundFormat.Items.Add("APE");
                comboSoundFormat.Items.Add("WV");                
                //Array audioFileFormats = Enum.GetValues(typeof(AudioFileFormat));

                //foreach (AudioFileFormat audioFileFormat in audioFileFormats)
                //{
                //    // Check if the item is not unknown
                //    if (audioFileFormat != AudioFileFormat.Unknown && audioFileFormat != AudioFileFormat.All)
                //    {
                //        // Add item to combo box
                //        comboSoundFormat.Items.Add(audioFileFormat.ToString());
                //    }
                //}

                // Check if the configuration is null or empty
                if (String.IsNullOrEmpty(filterSoundFormat))
                {
                    // Set MP3 filter by default
                    filterSoundFormat = AudioFileFormat.MP3.ToString();
                }

                // Set combo box selected item
                comboSoundFormat.SelectedItem = filterSoundFormat;

                // Refresh tree library
                RefreshTreeLibrary();

                // Set tray settings
                notifyIcon.Visible = Config.GetKeyValueGeneric<bool>("ShowTray").HasValue ? Config.GetKeyValueGeneric<bool>("ShowTray").Value : false;                

                // Set volume
                faderVolume.Value = Config.Audio.Mixer.Volume;
                lblVolume.Text = Config.Audio.Mixer.Volume + " %";
                Player.Volume = ((float)Config.Audio.Mixer.Volume / 100);

                // Update peak file warning if necessary
                RefreshPeakFileDirectorySizeWarning();

                //// Set Init current song Id
                //if (!string.IsNullOrEmpty(querySongId))
                //{
                //    // Make sure the application doesn't crash if it tries to convert a string into Guid
                //    try
                //    {
                //        InitCurrentSongId = new Guid(querySongId);
                //    }
                //    catch
                //    {
                //        // Do nothing
                //    }
                //}

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
                    m_initOpenNodeArtist = queryArtistName;
                    nodeAllArtists.Expand();

                    // Can't declare init done yet since background thread is running
                }
                else if (currentNodeType == "Album")
                {
                    // Expand the AllAlbums node                
                    m_initOpenNodeAlbum = queryAlbumTitle;
                    nodeAllAlbums.Expand();
                }
                else if (currentNodeType == "ArtistAlbum")
                {
                    // Expand the AllArtists node
                    m_initOpenNodeArtist = queryArtistName;
                    m_initOpenNodeArtistAlbum = queryAlbumTitle;
                    nodeAllArtists.Expand();

                    // Can't declare init done yet since background thread is running
                }
                else if (currentNodeType == "Playlist")
                {
                    //// Expand the playlist node                
                    //if (!String.IsNullOrEmpty(queryPlaylistId))
                    //{                    
                    //    try
                    //    {
                    //        InitOpenNodePlaylistId = new Guid(queryPlaylistId);
                    //    }
                    //    catch
                    //    {
                    //        InitOpenNodePlaylistId = Guid.Empty;
                    //    }
                    //}                
                    //nodeAllPlaylists.Expand();
                }

                // Start output meter timer
                timerUpdateOutputMeter.Start();
            }
            catch (Exception ex)
            {
                // Set error in splash and hide splash
                frmSplash.SetStatus("Error applying configuration!");
                frmSplash.HideSplash();

                // Display message box with error
                this.TopMost = true;
                MessageBox.Show("There was an error while applying the configuration.\nYou can delete the MPfm.Configuration.xml file in the MPfm application data folder (" + m_applicationDataFolderPath + ") to reset the library.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error initializing library!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracing.Log("Main form init -- Configuration apply error: " + ex.Message + "\nStack trace: " + ex.StackTrace);

                // Exit application
                Application.Exit();
                return;
            }
        }

        /// <summary>
        /// Loads the window configuration (window position, size, columns, etc.) from the configuration file.
        /// </summary>
        public void LoadWindowConfiguration()
        {
            // Main window
            WindowConfiguration windowMain = Config.Windows.Windows.FirstOrDefault(x => x.Name.ToUpper() == "MAIN");
            if(windowMain != null)
            {
                // Check if the window needs to use the default position (center screen)
                if(windowMain.UseDefaultPosition)
                {
                    // No configuration for window position; center window in screen
                    StartPosition = FormStartPosition.CenterScreen;
                }
                else
                {
                    int x = 0;
                    int y = 0;

                    // Make sure the window X isn't negative
                    if (windowMain.X < 0)
                    {
                        x = 0;
                    }
                    else
                    {
                        x = windowMain.X;
                    }
                    // Make sure the window Y isn't negative
                    if (windowMain.Y < 0)
                    {
                        y = 0;
                    }
                    else
                    {
                        y = windowMain.Y;
                    }

                    Location = new Point(x, y);
                }

                // Set size
                Width = windowMain.Width;
                Height = windowMain.Height;

                // Set maximized state
                if (windowMain.Maximized)
                {
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    WindowState = FormWindowState.Normal;
                }

                // Set splitter distance
                splitFirst.SplitterDistance = Config.GetKeyValueGeneric<int>("WindowSplitterDistance") ?? 175;

                // Load song browser column widths
                foreach (SongGridViewColumn column in Config.Controls.SongGridView.Columns)
                {
                    SongGridViewColumn columnGrid = viewSongs2.Columns.FirstOrDefault(x => x.Title == column.Title);
                    if (columnGrid != null)
                    {
                        columnGrid.Visible = column.Visible;
                        columnGrid.Width = column.Width;
                        columnGrid.Order = column.Order;
                    }
                }
            }           

            // Playlist window
            WindowConfiguration windowPlaylist = Config.Windows.Windows.FirstOrDefault(x => x.Name.ToUpper() == "PLAYLIST");            
            if (formPlaylist != null && windowPlaylist != null)
            {
                // Check if the window needs to use the default position (center screen)
                if (windowPlaylist.UseDefaultPosition)
                {
                    // No configuration for window position; center window in screen
                    formPlaylist.StartPosition = FormStartPosition.CenterScreen;                    
                }
                else
                {
                    formPlaylist.Location = new Point(windowPlaylist.X, windowPlaylist.Y);
                }

                formPlaylist.Width = windowPlaylist.Width;
                formPlaylist.Height = windowPlaylist.Height;

                if (windowPlaylist.Maximized)
                {
                    formPlaylist.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    formPlaylist.WindowState = FormWindowState.Normal;
                }

                formPlaylist.Visible = windowPlaylist.Visible;

                // Load playlist column widths                
                foreach (SongGridViewColumn column in Config.Controls.PlaylistGridView.Columns)
                {
                    SongGridViewColumn columnGrid = formPlaylist.viewSongs2.Columns.FirstOrDefault(x => x.Title == column.Title);
                    if (columnGrid != null)
                    {
                        columnGrid.Visible = column.Visible;
                        columnGrid.Width = column.Width;
                        columnGrid.Order = column.Order;
                    }
                }
            }
        }

        /// <summary>
        /// Saves the window configuration (window position, size, columns, etc.) into the configuration file.
        /// </summary>
        public void SaveWindowConfiguration()
        {
            // Save window position and size
            WindowConfiguration windowMain = Config.Windows.Windows.FirstOrDefault(x => x.Name.ToUpper() == "MAIN");
            if(windowMain != null)
            {
                // Set position and size
                windowMain.X = Location.X;
                windowMain.Y = Location.Y;
                windowMain.Width = Width;
                windowMain.Height = Height;
                windowMain.UseDefaultPosition = false;

                // Set maximized and visible
                bool isMaximized = false;
                if (WindowState == FormWindowState.Maximized)
                {
                    isMaximized = true;
                }
                windowMain.Maximized = isMaximized;
                windowMain.Visible = true;

                // Set splitter position
                Config.SetKeyValue<int>("WindowSplitterDistance", splitFirst.SplitterDistance);

                // Add columns
                Config.Controls.SongGridView.Columns = viewSongs2.Columns;
            }

            // Save playlist window position and size
            if (formPlaylist != null)
            {
                WindowConfiguration windowPlaylist = Config.Windows.Windows.FirstOrDefault(x => x.Name.ToUpper() == "PLAYLIST");
                if (windowPlaylist != null)
                {
                    windowPlaylist.X = formPlaylist.Location.X;
                    windowPlaylist.Y = formPlaylist.Location.Y;
                    windowPlaylist.Width = formPlaylist.Width;
                    windowPlaylist.Height = formPlaylist.Height;
                    windowPlaylist.Visible = formPlaylist.Visible;
                    windowPlaylist.UseDefaultPosition = false;

                    bool isMaximized = false;
                    if (formPlaylist.WindowState == FormWindowState.Maximized)
                    {
                        isMaximized = true;
                    }
                    windowPlaylist.Maximized = isMaximized;

                    // Add columns
                    Config.Controls.PlaylistGridView.Columns = formPlaylist.viewSongs2.Columns;
                }
            }

            // Save configuration
            Config.Save();
        }

        /// <summary>
        /// Sets the initialization phase to done. Closes the splash screen.
        /// </summary>
        public void SetInitDone()
        {
            // Set initialization boolean
            m_isInitDone = true;

            Tracing.Log("Main form init -- Initialization successful!");
            frmSplash.SetStatus("Initialization successful!");

            this.BringToFront();
            this.Activate();
            frmSplash.CloseFormWithFadeOut();            
        }

        #endregion

        #region Player Events

        /// <summary>
        /// Occurs when the timer for the output meter is expired. This forces the
        /// output meter to refresh itself every 10ms.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void timerUpdateOutputMeter_Tick(object sender, EventArgs e)
        {            
            // Check for valid objects
            if (m_player == null || !m_player.IsPlaying ||
                m_player.Playlist == null || m_player.Playlist.CurrentItem == null || m_player.Playlist.CurrentItem.Channel == null)
            {
                return;
            }

            float maxL = 0f;
            float maxR = 0f;

            // length of a 20ms window in bytes
            int length20ms = (int)m_player.MainChannel.Seconds2Bytes2(0.02);   //(int)Bass.BASS_ChannelSeconds2Bytes(channel, 0.02);
            // the number of 32-bit floats required (since length is in bytes!)
            int l4 = length20ms / 4; // 32-bit = 4 bytes

            // create a data buffer as needed
            float[] sampleData = new float[l4];

            //int length = Bass.BASS_ChannelGetData(channel, sampleData, length20ms);
            int length = m_player.MainChannel.GetData(sampleData, length20ms);

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

            //// limit the maximum peak levels to +6bB = 65535 = 0xFFFF
            //// the peak levels will be int values, where 32767 = 0dB
            //// and a float value of 1.0 also represents 0db.
            //peakL = (int)Math.Round(32767f * maxL) & 0xFFFF;
            //peakR = (int)Math.Round(32767f * maxR) & 0xFFFF;

            //// convert the level to dB
            //double dBlevelL = Base.LevelToDB_16Bit(peakL);
            //double dBlevelR = Base.LevelToDB_16Bit(peakR);

            //lblBitsPerSampleTitle.Text = dBlevelL.ToString("0.000");

            outputMeter.AddWaveDataBlock(left, right);
            outputMeter.Refresh();

            // Get min max info from wave block
            if (AudioTools.CheckForDistortion(left, right, true, -3.0f))
            {
                // Show distortion warning "LED"
                //picDistortionWarning.Visible = true;
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
            if (m_player == null || !m_player.IsPlaying ||
                m_player.Playlist == null || m_player.Playlist.CurrentItem == null || m_player.Playlist.CurrentItem.Channel == null)
            {
                return;
            }

            try
            {
                // Get position
                long positionBytes = m_player.GetPosition();
                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)m_player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
                long positionMS = (int)ConvertAudio.ToMS(positionSamples, (uint)m_player.Playlist.CurrentItem.AudioFile.SampleRate);
                string position = Conversion.MillisecondsToTimeString((ulong)positionMS);                

                // Set UI            
                lblCurrentPosition.Text = position;                
                miTraySongPosition.Text = "[ " + position + " / " + m_player.Playlist.CurrentItem.LengthString + " ]";

                // Set position in the wave form display
                if (!waveFormMarkersLoops.IsLoading)
                {
                    waveFormMarkersLoops.SetPosition(positionBytes, position);
                }

                // Update the song position
                if (!m_songPositionChanging)
                {
                    // Get ratio
                    float ratio = (float)positionSamples / (float)m_player.Playlist.CurrentItem.LengthSamples;
                    if (ratio <= 0.99f)
                    {
                        trackPosition.Value = Convert.ToInt32(ratio * 1000);
                    }

                    // Set time on seek control
                    lblSongPosition.Text = position;
                    lblSongPercentage.Text = (ratio * 100).ToString("0.00") + " %";
                }
            }
            catch
            {
                // Just don't do anything, this might be because the playlist items are now gone.
            }
        }

        /// <summary>
        /// Occurs when the playlist has changed index (when the an audio file starts/ends).
        /// Updates the UI.
        /// </summary>
        /// <param name="data">Event data</param>
        public void m_player_OnPlaylistIndexChanged(Player.PlayerPlaylistIndexChangedData data)
        {
            // Declare variables
            AudioFile audioFileDatabase = null;

            // If the initialization isn't finished, exit this event
            if (!IsInitDone)
            {
                return;
            }

            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Check if the event data is null
                if (data.AudioFileEnded != null)
                {
                    // Get audio file from database
                    audioFileDatabase = Library.Gateway.SelectAudioFile(data.AudioFileEnded.Id);
                }

                // Check if this was the last song
                if (data.IsPlaybackStopped)
                {
                    // Refresh controls
                    btnAddMarker.Enabled = false;
                    m_timerSongPosition.Enabled = false;
                    waveFormMarkersLoops.Clear();
                    RefreshSongControls();
                    RefreshMarkers();
                    RefreshLoops();
                    formPlaylist.RefreshPlaylistPlayIcon(Guid.Empty);
                }
                else
                {
                    // Refresh song information                    
                    RefreshSongInformation();

                    // Set the play icon in the song browser                
                    RefreshSongBrowserPlayIcon(m_player.Playlist.CurrentItem.AudioFile.Id);

                    // Refresh play icon in playlist                    
                    formPlaylist.RefreshPlaylistPlayIcon(m_player.Playlist.CurrentItem.Id);

                    // Set next song in configuration                                    
                    Config.Controls.SongGridView.Query.AudioFileId = m_player.Playlist.CurrentItem.AudioFile.Id;

                    // Refresh loops & markers
                    RefreshMarkers();
                    RefreshLoops();
                    
                    // Make sure the file exists in the database                    
                    if (audioFileDatabase != null)
                    {
                        // Update the new song in the database (in case the metadata has changed)
                        Library.Gateway.UpdateAudioFile(m_player.Playlist.CurrentItem.AudioFile);

                        // Refresh play count
                        SongGridViewItem item = viewSongs2.Items.FirstOrDefault(x => x.AudioFile.Id == m_player.Playlist.CurrentItem.AudioFile.Id);
                        if (item != null)
                        {
                            // Set updated data
                            AudioFile updatedAudioFile = Library.SelectAudioFile(m_player.Playlist.CurrentItem.AudioFile.Id);
                            item.AudioFile = updatedAudioFile;
                        }
                    }
                }

                // Check if the event data contains an audio file that has just ended
                if (data.AudioFileEnded != null && audioFileDatabase != null)
                {
                    // Update play count
                    Library.UpdateAudioFilePlayCount(data.AudioFileEnded.Id);                    

                    // Update the song grid view
                    viewSongs2.UpdateAudioFileLine(data.AudioFileEnded.Id);
                }

                // Update peak file warning if necessary
                RefreshPeakFileDirectorySizeWarning();
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
            if (e.CloseReason == CloseReason.FormOwnerClosing ||
                e.CloseReason == CloseReason.UserClosing)
            {
                // Check configuration values
                if (Config.GetKeyValueGeneric<bool>("ShowTray") == true &&
                    Config.GetKeyValueGeneric<bool>("HideTray") == true)
                {
                    e.Cancel = true;
                    this.Hide();
                    return;
                }
            }

            Tracing.Log("Main form -- Closing MPfm...");

            // Save configuration
            SaveWindowConfiguration();
            e.Cancel = false;

            // Close player if not null
            if (m_player != null)
            {
                // Stop playback if necessary
                if (m_player.IsPlaying)
                {
                    // Stop playback
                    m_player.Stop();
                }

                // Check if a wave form is generating
                if (waveFormMarkersLoops.IsLoading)
                {
                    // Cancel loading
                    waveFormMarkersLoops.CancelWaveFormLoading();
                }

                // Release the sound system from memory
                m_player.Dispose();
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

            //// Show panel
            //ShowUpdateLibraryProgress(true);

            //// Start update timer to display progress
            //timerUpdateLibrary.Start();

            //// Start new thread
            //new Thread(delegate()
            //{                
            //    List<string> filePaths = AudioTools.SearchAudioFilesRecursive(dialogAddFolder.SelectedPath, "MP3;FLAC;OGG;MPC;APE;WV");                
            //    updateLibrary = new Library.UpdateLibrary(1, m_library.Gateway.DatabaseFilePath);                
            //    Task<List<AudioFile>> audioFiles = updateLibrary.LoadFiles(filePaths);
            //}).Start();
        }

        /// <summary>
        /// Occurs when the user clicks on the "File -> Open an audio file" menu item.
        /// Pops an open file dialog to let the user select the file to play.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void miFileOpenAudioFile_Click(object sender, EventArgs e)
        {
            // Display dialog
            if (dialogOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                // The user has cancelled the operation
                return;
            }

            // Declare variables
            Dictionary<string, string> failedAudioFiles = new Dictionary<string, string>();

            // Check if the playback needs to be stopped
            if (Player.IsPlaying)
            {
                // Stop playback
                Player.Stop();
            }

            // Remove play icon on song browser and playlist
            RefreshSongBrowserPlayIcon(Guid.Empty);
            formPlaylist.RefreshPlaylistPlayIcon(Guid.Empty);

            // Determine if this is a playlist file or an audio file
            if (dialogOpenFile.FileName.ToUpper().Contains(".M3U") ||
                dialogOpenFile.FileName.ToUpper().Contains(".M3U8") ||
                dialogOpenFile.FileName.ToUpper().Contains(".PLS") ||
                dialogOpenFile.FileName.ToUpper().Contains(".XSPF"))
            {
                // Load playlist
                formPlaylist.LoadPlaylist(dialogOpenFile.FileName);
            }
            else
            {
                // Load audio files
                List<AudioFile> audioFiles = new List<AudioFile>();                
                foreach (string fileName in dialogOpenFile.FileNames)
                {
                    try
                    {
                        // Create audio file and add to list
                        AudioFile audioFile = new AudioFile(fileName);
                        audioFiles.Add(audioFile);
                    }
                    catch(Exception ex)
                    {
                        // Add to list of failed playback files
                        failedAudioFiles.Add(fileName, ex.Message);
                    }
                }

                // Check if there is at least one file to load
                if (audioFiles.Count == 0)
                {
                    // Build list
                    StringBuilder sbMessage = new StringBuilder();
                    sbMessage.AppendLine("Error: The playlist is empty. No files could not be loaded:\n");
                    foreach (KeyValuePair<string, string> fileName in failedAudioFiles)
                    {
                        sbMessage.AppendLine(fileName.Key + "\nReason: " + fileName.Value);
                    }

                    // Display message box
                    MessageBox.Show(sbMessage.ToString(), "Error loading files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Add items to playlist
                Player.Playlist.Clear();
                //Player.Playlist.AddItems(dialogOpenFile.FileNames.ToList());
                Player.Playlist.AddItems(audioFiles);
                Player.Playlist.First();
            }

            // Start playback
            Player.Play();

            // Get the song info
            RefreshSongInformation();

            // Refresh controls after song playback
            RefreshSongControls();

            // Refresh playlist
            formPlaylist.RefreshPlaylist();

            // Refresh loop and marker controls
            RefreshMarkers();
            RefreshLoops();

            // Start timer
            m_timerSongPosition.Enabled = true;

            // Make sure the user cannot add markers and loops
            btnAddLoop.Enabled = false;
            btnAddMarker.Enabled = false;

            // Display the list of failed songs to the user
            if (failedAudioFiles.Count > 0)
            {
                // Build list
                StringBuilder sbMessage = new StringBuilder();
                sbMessage.AppendLine("Some files could not be loaded:\n");
                foreach (KeyValuePair<string, string> fileName in failedAudioFiles)
                {
                    sbMessage.AppendLine(fileName.Key +"\nReason: " + fileName.Value);
                }

                // Display warning
                MessageBox.Show(sbMessage.ToString(), "Some files could not be loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
            // Start playback of currently selected item
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
            if (m_player == null || m_player.Playlist == null || !m_player.IsPlaying)
            {
                return;
            }

            // Check pause status
            if (m_player.IsPaused)
            {
                btnPause.Checked = false;
                miTrayPause.Checked = false;
                m_timerSongPosition.Enabled = true;
                timerUpdateOutputMeter.Enabled = true;
            }
            else
            {
                btnPause.Checked = true;
                miTrayPause.Checked = true;
                m_timerSongPosition.Enabled = false;
                timerUpdateOutputMeter.Enabled = false;
            }

            // Set pause
            m_player.Pause();
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
            if (m_player == null || m_player.Playlist == null || !m_player.IsPlaying)
            {
                return;
            }

            // Skip to next song in player
            m_player.Next();

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
            if (m_player == null || m_player.Playlist == null || !m_player.IsPlaying)
            {
                return;
            }

            // Go to previous song in player
            m_player.Previous();

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
            if (m_player.RepeatType == RepeatType.Off)
            {
                m_player.RepeatType = RepeatType.Playlist;
            }
            else if (m_player.RepeatType == RepeatType.Playlist)
            {
                m_player.RepeatType = RepeatType.Song;
            }
            else
            {
                m_player.RepeatType = RepeatType.Off;
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
                //formPlaylist.Show(this);
                formPlaylist.Show();
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
        /// Refreshes the peak file directory size warning.
        /// </summary>
        public void RefreshPeakFileDirectorySizeWarning()
        {
            // Get configuration
            int? peakFileDisplayWarningThreshold = Config.GetKeyValueGeneric<int>("PeakFile_DisplayWarningThreshold");
            bool? peakFileDisplayWarning = Config.GetKeyValueGeneric<bool>("PeakFile_DisplayWarning");

            // Check if the warning needs to be displayed
            if (peakFileDisplayWarning.HasValue && peakFileDisplayWarning.Value && peakFileDisplayWarningThreshold.HasValue)
            {
                // Check peak file directory size
                long size = PeakFile.CheckDirectorySize(m_peakFileFolderPath);

                // Check free space
                long freeSpace = 0;
                try
                {
                    // Get drive info
                    DriveInfo driveInfo = new DriveInfo(m_peakFileFolderPath[0] + ":");
                    freeSpace = driveInfo.AvailableFreeSpace;
                }
                catch
                {
                    // Set value to indicate this has failed
                    freeSpace = -1;
                }

                // Compare size with threshold
                if (size > peakFileDisplayWarningThreshold.Value * 1000000)
                {
                    // Display warning
                    lblPeakFileWarning.Text = "Warning: The peak file directory size exceeds " + peakFileDisplayWarningThreshold.Value.ToString() + " MB. Total peak file directory size: " + ((float)size / 1000000).ToString("0") + " MB. Free space on disk: " + ((float)freeSpace / 1000000).ToString("0") + " MB.";
                    lblPeakFileWarning.Visible = true;
                }
                else
                {
                    // Hide warning
                    lblPeakFileWarning.Visible = false;
                }
            }
            else
            {
                // Hide warning
                lblPeakFileWarning.Visible = false;
            }
        }

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
            if (Player.IsPlaying)
            {
                // Enable/disable buttons
                btnStop.Enabled = true;
                btnPause.Enabled = true;
                btnPlay.Enabled = false;
                btnNextSong.Enabled = (Player.Playlist.CurrentItemIndex < Player.Playlist.Items.Count - 1) ? true : false;
                btnPreviousSong.Enabled = (Player.Playlist.CurrentItemIndex == 0) ? false : true;

                // Mantis 0000042
                // Reset the pause button icon
                btnPause.Checked = false;

                // Set the play icon in the song browser                
                RefreshSongBrowserPlayIcon(Player.Playlist.CurrentItem.AudioFile.Id);

                // Refresh playlist window
                if (refreshPlaylistWindow)
                {
                    formPlaylist.RefreshPlaylist();
                }

                // Refresh playlist icon
                formPlaylist.RefreshPlaylistPlayIcon(Player.Playlist.CurrentItem.Id);
            }
            else
            {
                // Enable/disable buttons
                btnStop.Enabled = false;
                btnPause.Enabled = false;
                btnNextSong.Enabled = false;
                btnPreviousSong.Enabled = false;
                btnPlay.Enabled = true;

                // Nothing is playing, then display "stop" song information                
                lblCurrentArtistName.Text = string.Empty;
                lblCurrentAlbumTitle.Text = string.Empty;
                lblCurrentSongTitle.Text = string.Empty;
                lblCurrentFilePath.Text = string.Empty;
                lblCurrentPosition.Text = "0:00.000";
                lblLength.Text = "0:00.000";
                lblSoundFormat.Text = string.Empty;
                lblBitrate.Text = string.Empty;
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
        /// specified audio file in the newAudioFileId parameter. If the Guid is empty,
        /// the icon will be removed.
        /// </summary>
        /// <param name="newAudioFileId">AudioFile identifier</param>
        public void RefreshSongBrowserPlayIcon(Guid newAudioFileId)
        {
            // Set currently playing song
            viewSongs2.NowPlayingAudioFileId = newAudioFileId;
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
            m_querySongBrowser = metadata.Query;

            // Set config
            Config.Controls.SongGridView.Query.ArtistName = m_querySongBrowser.ArtistName;
            Config.Controls.SongGridView.Query.AlbumTitle = m_querySongBrowser.AlbumTitle;
            Config.Controls.SongGridView.Query.PlaylistId = m_querySongBrowser.PlaylistId;

            try
            {
                Config.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Refresh song browser
            RefreshSongBrowser(m_querySongBrowser);
        }

        /// <summary>
        /// Refreshes the Song Browser using the query specified in parameter.
        /// </summary>
        /// <param name="query">Query for Song Browser</param>
        public void RefreshSongBrowser(SongQuery query)
        {
            // Create the list of audio files for the browser
            List<AudioFile> audioFiles = null;
            string orderBy = viewSongs2.OrderByFieldName;
            bool orderByAscending = viewSongs2.OrderByAscending;

            // Get query type
            if (query.Type == SongQueryType.Album)
            {
                audioFiles = Library.SelectAudioFiles(m_filterAudioFileFormat, orderBy, orderByAscending, query.ArtistName, query.AlbumTitle, txtSearch.Text);
            }
            else if (query.Type == SongQueryType.Artist)
            {
                audioFiles = Library.SelectAudioFiles(m_filterAudioFileFormat, orderBy, orderByAscending, query.ArtistName, string.Empty, txtSearch.Text);
            }
            else if (query.Type == SongQueryType.All)
            {
                audioFiles = Library.SelectAudioFiles(m_filterAudioFileFormat, orderBy, orderByAscending, string.Empty, string.Empty, txtSearch.Text);
            }
            else if (query.Type == SongQueryType.None)
            {
                audioFiles = new List<AudioFile>();
            }

            // Make sure the audio file list is valid            
            if (audioFiles == null)
            {
                return;
            }

            // Import list of audio files into grid view
            viewSongs2.ImportAudioFiles(audioFiles);            
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
                    //workerAlbumArt.RunWorkerAsync(m_player.Playlist.CurrentItem.FilePath);
                    workerAlbumArt.RunWorkerAsync(m_player.Playlist.CurrentItem.AudioFile.FilePath);
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
                lblCurrentArtistName.Text = m_player.Playlist.CurrentItem.AudioFile.ArtistName;
                lblCurrentAlbumTitle.Text = m_player.Playlist.CurrentItem.AudioFile.AlbumTitle;
                lblCurrentSongTitle.Text = m_player.Playlist.CurrentItem.AudioFile.Title;
                lblCurrentFilePath.Text = m_player.Playlist.CurrentItem.AudioFile.FilePath;
                lblLength.Text = m_player.Playlist.CurrentItem.LengthString;

                // Set tray menu metadata
                miTrayArtistName.Text = m_player.Playlist.CurrentItem.AudioFile.ArtistName;
                miTrayAlbumTitle.Text = m_player.Playlist.CurrentItem.AudioFile.AlbumTitle;
                miTraySongTitle.Text = m_player.Playlist.CurrentItem.AudioFile.Title;

                // Set format labels
                lblSoundFormat.Text = Path.GetExtension(m_player.Playlist.CurrentItem.AudioFile.FilePath).Replace(".", "").ToUpper();
                lblBitrate.Text = m_player.Playlist.CurrentItem.AudioFile.Bitrate.ToString();
                lblFrequency.Text = m_player.Playlist.CurrentItem.AudioFile.SampleRate.ToString();

                // Get length
                long length = m_player.Playlist.CurrentItem.Channel.GetLength();

                // Check if this is a FLAC file over 44100Hz
                if (m_player.Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && m_player.Playlist.CurrentItem.AudioFile.SampleRate > 44100)
                {
                    // Multiply by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                    length = (long)((float)length * 1.5f);
                }

                // Set the song length for the Loops & Markers wave form display control                
                waveFormMarkersLoops.Length = length;

                // Load the wave form                
                waveFormMarkersLoops.LoadWaveForm(m_player.Playlist.CurrentItem.AudioFile.FilePath);
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
            // Clear items
            viewMarkers.Items.Clear();

            // Set marker buttons
            btnEditMarker.Enabled = false;
            btnRemoveMarker.Enabled = false;
            btnGoToMarker.Enabled = false;

            // Check if a song is currently playing
            if (!Player.IsPlaying)
            {
                // Reset buttons
                btnAddMarker.Enabled = false;
                return;
            }

            // Set button
            btnAddMarker.Enabled = true;

            // Fetch markers from database
            List<Marker> markers = Library.Gateway.SelectMarkers(Player.Playlist.CurrentItem.AudioFile.Id);

            // Update grid view
            foreach (Marker marker in markers)
            {
                // Create grid view item
                ListViewItem item = viewMarkers.Items.Add(marker.Name);
                item.Tag = marker.MarkerId;
                item.SubItems.Add(marker.Position);
                item.SubItems.Add(marker.Comments);
                item.SubItems.Add(marker.PositionBytes.ToString());
            }
        }

        /// <summary>
        /// Refreshes the Loops grid view.
        /// </summary>
        public void RefreshLoops()
        {
            // Clear items
            viewLoops.Items.Clear();

            // Set buttons
            btnEditLoop.Enabled = false;
            btnRemoveLoop.Enabled = false;
            btnPlayLoop.Enabled = false;
            btnStopLoop.Enabled = false;

            // Check if a song is currently playing
            if (!Player.IsPlaying)
            {
                // Reset buttons
                btnAddLoop.Enabled = false;
                return;
            }

            // Set button
            btnAddLoop.Enabled = true;

            // Fetch loops from database
            List<Loop> loops = Library.Gateway.SelectLoops(Player.Playlist.CurrentItem.AudioFile.Id);
            List<Marker> markers = Library.Gateway.SelectMarkers(Player.Playlist.CurrentItem.AudioFile.Id);

            // Update grid view
            foreach (Loop loop in loops)
            {
                // Create grid view item
                ListViewItem item = viewLoops.Items.Add("");
                item.Tag = loop.LoopId;
                item.SubItems.Add(loop.Name);
                item.SubItems.Add(loop.Length);

                // Update marker subitems
                item.SubItems.Add(loop.StartPosition);
                item.SubItems.Add(loop.EndPosition);

                // Check if this is the currently playing loop
                if (Player.CurrentLoop != null && Player.CurrentLoop.Name == loop.Name)
                {
                    item.ImageIndex = 7;
                }
            }
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

            if (m_querySongBrowser.Type == SongQueryType.None)
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

            //nodeAllPlaylists = new TreeNode("Playlists");
            //nodeAllPlaylists.ImageIndex = 4;
            //nodeAllPlaylists.SelectedImageIndex = 4;
            //nodeAllPlaylists.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.AllPlaylists, new SongQuery(SongQueryType.None));
            //nodeAllPlaylists.Nodes.Add("dummy", "dummy");

            //nodeRecentlyPlayed = new TreeNode("Recently Played");
            //nodeRecentlyPlayed.ImageIndex = 18;
            //nodeRecentlyPlayed.SelectedImageIndex = 18;
            //nodeRecentlyPlayed.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.RecentlyPlayed, new SongQuery());

            //if (this.currentSongBrowserQueryType == "RecentlyPlayed")
            //{
            //    selectedNode = nodeRecentlyPlayed;
            //}

            // Add main nodes to the treeview
            treeLibrary.Nodes.Add(nodeAllSongs);
            treeLibrary.Nodes.Add(nodeAllArtists);
            treeLibrary.Nodes.Add(nodeAllAlbums);
            //treeLibrary.Nodes.Add(nodeAllPlaylists);
            //treeLibrary.Nodes.Add(nodeRecentlyPlayed);

            // Set selected node
            treeLibrary.SelectedNode = selectedNode;

            // Set update done
            treeLibrary.EndUpdate();
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
            if (m_player.RepeatType == RepeatType.Playlist)
            {
                btnRepeat.Text = repeatPlaylist;
                btnRepeat.Checked = true;

                miTrayRepeat.Text = repeatPlaylist;
                miTrayRepeat.Checked = true;
            }
            else if (m_player.RepeatType == RepeatType.Song)
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
        /// Starts playback from the current playlist item.
        /// </summary>
        public void Play()
        {
            // Start playback
            m_player.Play();

            // Refresh song information
            RefreshSongInformation();

            // Refresh controls after song playback
            RefreshSongControls();

            // Refresh loop and marker controls
            RefreshMarkers();
            RefreshLoops();

            // Refresh playlist window
            formPlaylist.RefreshPlaylist();

            // Set marker/loops buttons
            btnAddMarker.Enabled = true;
            btnAddLoop.Enabled = true;

            // Start song position timer
            m_timerSongPosition.Enabled = true;

            // Check if the audio file exists in the database
            AudioFile audioFileDatabase = Library.Gateway.SelectAudioFile(m_player.Playlist.CurrentItem.AudioFile.Id);
            if(audioFileDatabase != null)
            {
                // Update the first audio file in the database (in case the metadata has changed)
                Library.Gateway.UpdateAudioFile(m_player.Playlist.CurrentItem.AudioFile);
            }

            // Refresh warning
            RefreshPeakFileDirectorySizeWarning();
        }

        /// <summary>
        /// Plays the selected song query in the Song Browser. The playback can be paused to seeked to a specific 
        /// position before playing. Refreshes UI controls.
        /// </summary>
        public void Play(SongQuery query, Guid audioFileId)
        {
            try
            {
                // Check if a song is playing
                if (Player.IsPlaying)
                {
                    // Stop playback
                    Player.Stop();
                }

                // Set playback depending on the query in the song browser
                List<AudioFile> audioFiles = null;
                if (query.Type == SongQueryType.Album)
                {
                    // Generate an artist/album playlist and start playback
                    audioFiles = Library.SelectAudioFiles(m_filterAudioFileFormat, string.Empty, true, query.ArtistName, query.AlbumTitle);                        
                }
                else if (query.Type == SongQueryType.Artist)
                {
                    // Generate an artist playlist and start playback                                                                        
                    audioFiles = Library.SelectAudioFiles(m_filterAudioFileFormat, string.Empty, true, query.ArtistName);                        
                }
                else if (query.Type == SongQueryType.All)
                {
                    // Generate a playlist with all the library and start playback
                    audioFiles = Library.SelectAudioFiles(m_filterAudioFileFormat);                        
                }

                // Clear playlist and add songs
                m_player.Playlist.Clear();
                m_player.Playlist.AddItems(audioFiles);

                // Set initial item
                if (audioFileId != Guid.Empty)
                {
                    // Set current item
                    m_player.Playlist.GoTo(audioFileId);
                }

                // Start playback
                Play();                             
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured while loading audio files:\n" + ex.Message, "An error has occured while loading audio files.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Plays the selected song query in the Song Browser. Refreshes UI controls.
        /// </summary>
        public void PlaySelectedSongQuery()
        {
            // Make sure there is a selected song
            if (viewSongs2.SelectedItems.Count == 0)
            {
                return;
            }

            // Play selected song
            Play(m_querySongBrowser, viewSongs2.SelectedItems[0].AudioFile.Id);
        } 

        /// <summary>
        /// Stops playback and refreshes UI controls.
        /// </summary>
        public void Stop()
        {
            // Validate player
            if(m_player == null || m_player.Playlist == null || !m_player.IsPlaying)
            {
                return;
            }

            // Check if a wave form is generating
            if (waveFormMarkersLoops.IsLoading)
            {
                // Cancel loading
                waveFormMarkersLoops.CancelWaveFormLoading();
            }

            // Stop song
            m_player.Stop();

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
            m_songPositionChanging = true;
        }

        /// <summary>
        /// Occurs when the user releases a mouse button on the Song Position trackbar.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void trackPosition_MouseUp(object sender, MouseEventArgs e)
        {
            // Validate player
            if (m_player == null || !m_player.IsPlaying ||
                m_player.Playlist == null || m_player.Playlist.CurrentItem == null)
            {
                return;
            }

            try
            {
                // Get ratio and set position
                double ratio = (double)trackPosition.Value / 1000;

                // Get length
                int positionBytes = (int)(ratio * (double)m_player.Playlist.CurrentItem.LengthBytes);
                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)m_player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
                long positionMS = ConvertAudio.ToMS(positionSamples, (uint)m_player.Playlist.CurrentItem.AudioFile.SampleRate);

                // Set player position
                m_player.SetPosition(positionBytes);

                // Set UI
                lblSongPosition.Text = Conversion.MillisecondsToTimeString((ulong)positionMS);                
                lblSongPercentage.Text = (ratio * 100).ToString("00.00");

                // Set flags
                m_songPositionChanging = false;
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
            if (m_player == null || !m_player.IsPlaying ||
                m_player.Playlist == null || m_player.Playlist.CurrentItem == null)
            {
                return;
            }

            // Get ratio
            double ratio = (double)trackPosition.Value / 1000;

            // Check if the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Get time                
                lblSongPosition.Text = Conversion.MillisecondsToTimeString(Convert.ToUInt32((ratio * (double)m_player.Playlist.CurrentItem.LengthMilliseconds)));
                lblSongPercentage.Text = (ratio * 100).ToString("0.00") + " %";
            }
        }

        /// <summary>
        /// Fires when the user scrolls the time shifting slider.
        /// </summary>
        private void trackTimeShiftingNew_OnTrackBarValueChanged()
        {
            double multiplier = 1 / ((double)trackTimeShifting.Value / 100);

            lblTimeShifting.Text = trackTimeShifting.Value.ToString() + " %";

            Player.TimeShifting = trackTimeShifting.Value;
        }


        /// <summary>
        /// Fires when the user releases the mouse button on the Volume slider. Saves the final value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackVolume_MouseUp(object sender, MouseEventArgs e)
        {
            //Config["Volume"] = trackVolume.Value.ToString();
            Config.Audio.Mixer.Volume = faderVolume.Value;
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
            if (!Player.IsPlaying)
            {
                return;
            }

            // Open window
            EditSongMetadata(Player.Playlist.CurrentItem.AudioFile.FilePath);
        }

        /// <summary>
        /// Fires when the volume slider value has changed. Updates the player volume.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void faderVolume_OnFaderValueChanged(object sender, EventArgs e)
        {
            // Check if the form has finished loading
            if (!IsInitDone)
            {
                return;
            }

            // Set volume and update label            
            m_player.Volume = (float)faderVolume.Value / 100;
            lblVolume.Text = faderVolume.Value.ToString() + " %";
            Config.Audio.Mixer.Volume = faderVolume.Value;
            Config.Save();
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
            if (Player != null && Player.IsPlaying)
            {
                Process.Start("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.Title) + "+guitar+tab");
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
            if (Player != null && Player.IsPlaying)
            {
                Process.Start("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.Title) + "+bass+tab");
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
            if (Player != null && Player.IsPlaying)
            {
                Process.Start("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.Title) + "+lyrics");
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
            if (Player != null && Player.IsPlaying)
            {
                Process.Start("http://www.google.ca/imghp?q=" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.ArtistName) + "+" + HttpUtility.UrlEncode(Player.Playlist.CurrentItem.AudioFile.AlbumTitle));
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

        /// <summary>
        /// Occurs when the user changes the selection on the Song Browser.
        /// </summary>
        /// <param name="data">Event data</param>
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
                Config.Controls.SongGridView.Query.AudioFileId = viewSongs2.SelectedItems[0].AudioFile.Id;
            }
        }

        /// <summary>
        /// Occurs when the user double clicks on an item of the Song Browser.
        /// Plays the selected song query.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewSongs2_DoubleClick(object sender, EventArgs e)
        {
            // Start playback of currently selected item
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

            // Get audio file from item metadata (check for null)
            AudioFile audioFile = viewSongs2.SelectedItems[0].AudioFile;
            if (audioFile == null)
            {
                return;
            }

            // Open window
            EditSongMetadata(audioFile.FilePath);
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
                result.Albums = Library.SelectArtistAlbumTitles(args.ArtistName, m_filterAudioFileFormat);
            }
            else if (args.OperationType == WorkerTreeLibraryOperationType.GetArtists)
            {
                // Select all artists
                result.Artists = Library.SelectArtistNames(m_filterAudioFileFormat);
            }
            else if (args.OperationType == WorkerTreeLibraryOperationType.GetAlbums)
            {
                // Select all albums
                result.AllAlbums = Library.SelectAlbumTitles(m_filterAudioFileFormat);
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

                // Check which operation type to do
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
                            if (!IsInitDone && result.ArtistName == m_initOpenNodeArtist && albumTitle == m_initOpenNodeArtistAlbum)
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
                            if (!IsInitDone && artistName == m_initOpenNodeArtist)
                            {
                                // Set node as selected
                                treeLibrary.SelectedNode = nodeArtist;
                            }
                        }
                    }

                    // Check if an ArtistAlbum child node needs to be opened
                    if (!IsInitDone && !String.IsNullOrEmpty(m_initOpenNodeArtistAlbum))
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

                        // If the form is initializing and setting the initial opened node from history...
                        if (!IsInitDone && albumTitle == m_initOpenNodeAlbum)
                        {
                            // Set node as selected
                            treeLibrary.SelectedNode = nodeAlbum;
                        }
                    }

                    // Check if an Album child node needs to be opened
                    if (!IsInitDone && !String.IsNullOrEmpty(m_initOpenNodeAlbum))
                    {
                        // The artist node must be expanded
                        treeLibrary.SelectedNode.Expand();
                    }
                }
                else if (result.OperationType == WorkerTreeLibraryOperationType.GetPlaylists)
                {
                    //// Check result
                    //if (result.Playlists != null)
                    //{                    
                    //    // For each playlist
                    //    foreach (PlaylistDTO playlist in result.Playlists)
                    //    {
                    //        // Create tree node
                    //        TreeNode nodePlaylist = new TreeNode();
                    //        nodePlaylist.Text = playlist.PlaylistName;
                    //        nodePlaylist.Tag = new TreeLibraryNodeMetadata(TreeLibraryNodeType.Playlist, new SongQuery(playlist.PlaylistId));
                    //        nodePlaylist.ImageIndex = 11;
                    //        nodePlaylist.SelectedImageIndex = 11;

                    //        // Add node to tree
                    //        result.TreeNodeToUpdate.Nodes.Add(nodePlaylist);

                    //        // If the form is initializing and setting the initial opened node from history...
                    //        if (!IsInitDone && playlist.PlaylistId == InitOpenNodePlaylistId)
                    //        {
                    //            // Set node as selected
                    //            treeLibrary.SelectedNode = nodePlaylist;
                    //        }
                    //    }
                    //}
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
                // Set current tree node type in config
                TreeLibraryNodeMetadata metadata = (TreeLibraryNodeMetadata)e.Node.Tag;
                if (metadata != null)
                {
                    // Set node type
                    Config.Controls.SongGridView.Query.NodeType = metadata.NodeType;
                }

                // Refresh song browser
                RefreshSongBrowser();
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

        //private UpdateLibrary updateLibrary = null;

        /// <summary>
        /// Displays the Update Library Status window and updates the library
        /// using the mode passed in parameter.
        /// </summary>
        public void UpdateLibrary()
        {
            //timerUpdateLibrary.Start();

            //new Thread(delegate()
            //    {
            //        List<string> filePaths = AudioTools.SearchAudioFilesRecursive(@"E:\MP3\", "MP3;FLAC;OGG");
            //        updateLibrary = new Library.UpdateLibrary(1, m_library.Gateway.DatabaseFilePath);
            //        updateLibrary.OnProcessData += new MPfm.Library.UpdateLibrary.ProcessData(updateLibrary_OnProcessData);
            //        Task<List<AudioFile>> audioFiles = updateLibrary.LoadFiles(filePaths);
            //    }).Start();

            //timerUpdateLibrary.Stop();            

            // Create window and display as dialog
            formUpdateLibraryStatus = new frmUpdateLibraryStatus(this);
            formUpdateLibraryStatus.ShowDialog(this);
        }

        private void timerUpdateLibrary_Tick(object sender, EventArgs e)
        {
            //if (updateLibrary == null)
            //{
            //    return;
            //}

            //// Update progress
            //progressUpdateLibrary.Value = (int)updateLibrary.PercentageDone;
            //lblUpdateLibraryCurrentFileValue.Text = updateLibrary.CurrentFile;
            ////lblStatusPercentage.Text = updateLibrary.PercentageDone.ToString("0.00") + "%";

            //// Check if process is done
            //if (updateLibrary.PercentageDone == 100)
            //{
            //    // Stop timer
            //    timerUpdateLibrary.Stop();

            //    // Show panel
            //    ShowUpdateLibraryProgress(false);
            //}
        }

        /// <summary>
        /// Occurs when the user changes the sound format filter using the Sound Format combobox.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void comboSoundFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get audio file format
            AudioFileFormat audioFileFormat = AudioFileFormat.Unknown;
            Enum.TryParse<AudioFileFormat>(comboSoundFormat.Text, out audioFileFormat);

            // Set filter
            m_filterAudioFileFormat = audioFileFormat;

            // Check if init is done
            if (IsInitDone)
            {
                // Set configuration                
                Config.SetKeyValue("FilterSoundFormat", audioFileFormat.ToString());
                Config.Save();

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
            // Refresh song browser
            RefreshSongBrowser();
        }

        /// <summary>
        /// Occurs when the user clicks on the Add songs to playlist button or menu item in the Song Browser.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnAddSongToPlaylist_Click(object sender, EventArgs e)
        {
            // Loop through selected items
            for (int a = 0; a < viewSongs2.SelectedItems.Count; a++)
            {
                // Get the song from the tag of the item
                AudioFile audioFile = viewSongs2.SelectedItems[a].AudioFile;

                // Check for null
                if (audioFile != null)
                {
                    // Add to playlist                    
                    m_player.Playlist.AddItem(audioFile.FilePath);
                }
            }

            // Refresh playlists (if there was at least one selected item)
            if (viewSongs2.SelectedItems.Count > 0)
            {
                formPlaylist.RefreshPlaylist();
            }
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
        /// Occurs when the user has clicked on the Markers and Loops Wave Form Display control and
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
            m_player.SetPosition(data.Percentage);

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
                formAddEditMarker = new frmAddEditMarker(this, AddEditMarkerWindowMode.Add, Player.Playlist.CurrentItem.AudioFile, Guid.Empty);
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
            // Check if an item is selected
            if (viewMarkers.SelectedItems.Count == 0)
            {
                return;
            }

            // Get selected markerId
            Guid markerId = new Guid(viewMarkers.SelectedItems[0].Tag.ToString());

            // Create window and show as dialog
            formAddEditMarker = new frmAddEditMarker(this, AddEditMarkerWindowMode.Edit, Player.Playlist.CurrentItem.AudioFile, markerId);
            formAddEditMarker.ShowDialog(this);
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
                Library.Gateway.DeleteMarker(markerId);
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
            // Check if an item is selected
            if (viewMarkers.SelectedItems.Count == 0)
            {
                return;
            }

            // Get selected markerId
            Guid markerId = new Guid(viewMarkers.SelectedItems[0].Tag.ToString());

            // Get PCM position
            uint position = 0;
            uint.TryParse(viewMarkers.SelectedItems[0].SubItems[3].Text, out position);

            // Set player position            
            m_player.SetPosition(position);
        }

        /// <summary>
        /// Occurs when the user double-clicks on the Marker grid view.
        /// Sets the player position as the currently selecter marker position.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewMarkers_DoubleClick(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (viewMarkers.SelectedItems.Count == 0)
            {
                return;
            }

            // Get selected markerId
            Guid markerId = new Guid(viewMarkers.SelectedItems[0].Tag.ToString());

            // Get PCM position
            uint position = 0;
            uint.TryParse(viewMarkers.SelectedItems[0].SubItems[3].Text, out position);

            // Set player position            
            m_player.SetPosition(position);
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
            // Check if the wave data is loaded
            if (waveFormMarkersLoops.WaveDataHistory.Count == 0)
            {
                return;
            }

            //// Check if there are at least two markers
            //if (viewMarkers.Items.Count < 2)
            //{
            //    // Display message
            //    MessageBox.Show("You must add at least two markers before adding a loop.", "Error adding loop", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            // Create window and show as dialog
            formAddEditLoop = new frmAddEditLoop(this, AddEditLoopWindowMode.Add, Player.Playlist.CurrentItem.AudioFile, Guid.Empty);
            formAddEditLoop.ShowDialog(this);            
        }

        /// <summary>
        /// Occurs when the user has clicked on the Edit Loop button.
        /// Opens the Add/Edit Loop window.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnEditLoop_Click(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (viewLoops.SelectedItems.Count == 0)
            {
                return;
            }

            // Get selected loopId
            Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

            // Create window and show as dialog
            formAddEditLoop = new frmAddEditLoop(this, AddEditLoopWindowMode.Edit, Player.Playlist.CurrentItem.AudioFile, loopId);
            formAddEditLoop.ShowDialog(this);
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
                // Get selected loopId
                Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

                // Delete loop and refresh list                
                Library.Gateway.DeleteLoop(loopId);
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
            // Check if an item is selected
            if (viewLoops.SelectedItems.Count == 0)
            {
                return;
            }            

            // Get selected loopId
            Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

            // Fetch loop from database
            Loop loop = Library.Gateway.SelectLoop(loopId);

            // Check if the loop is valid
            if (loop == null)
            {
                return;
            }

            // Set current loop in player            
            Player.StartLoop(loop);

            // Set currently playing loop icon
            for (int a = 0; a < viewLoops.Items.Count; a++)
            {
                // Check if the loop is currently playing
                if (new Guid(viewLoops.Items[a].Tag.ToString()) == loop.LoopId)
                {
                    // Set flag
                    viewLoops.Items[a].ImageIndex = 7;
                }
                else
                {
                    // Reset flag
                    viewLoops.Items[a].ImageIndex = -1;
                }
            }

            // Reset buttons
            btnPlayLoop.Enabled = false;
            btnStopLoop.Enabled = true;
        }

        /// <summary>
        /// Occurs when the user has clicked on the Stop Loop button.
        /// Stops the playback of a loop.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnStopLoop_Click(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (viewLoops.SelectedItems.Count == 0)
            {
                return;
            }

            // Reset loop
            Player.StopLoop();

            // Refresh loops
            RefreshLoops();

            // Reset buttons
            btnPlayLoop.Enabled = true;
            btnStopLoop.Enabled = false;
        }

        /// <summary>
        /// Occurs when the user changes the selection on the Loops grid view.
        /// Enables/disables loop buttons.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewLoops_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // Enable/disable loop buttons
            if (viewLoops.SelectedItems.Count == 0)
            {
                btnPlayLoop.Enabled = false;
                btnEditLoop.Enabled = false;
                btnRemoveLoop.Enabled = false;
            }
            else
            {
                // At least one item is selected.                
                btnPlayLoop.Enabled = true;
                btnStopLoop.Enabled = false;
                btnEditLoop.Enabled = true;
                btnRemoveLoop.Enabled = true;

                // Check if the loop is currently playing
                if (Player.CurrentLoop != null)
                {
                    // Check if the loop matches
                    if (new Guid(viewLoops.SelectedItems[0].Tag.ToString()) == Player.CurrentLoop.LoopId)
                    {
                        // Set buttons
                        btnPlayLoop.Enabled = false;
                        btnStopLoop.Enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the user double-clicks on the Loops grid view.
        /// Sets the current loop as the currently selected item.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewLoops_DoubleClick(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (viewLoops.SelectedItems.Count == 0)
            {
                return;
            }

            // Check if the player is already playing a loop
            if (Player.CurrentLoop != null)
            {
                return;
            }

            // Get selected loopId
            Guid loopId = new Guid(viewLoops.SelectedItems[0].Tag.ToString());

            // Fetch loop from database
            Loop loop = Library.Gateway.SelectLoop(loopId);

            // Check if the loop is valid
            if (loop == null)
            {
                return;
            }

            // Set current loop in player            
            Player.StartLoop(loop);

            // Set currently playing loop icon
            for (int a = 0; a < viewLoops.Items.Count; a++)
            {
                // Check if the loop is currently playing
                if (new Guid(viewLoops.Items[a].Tag.ToString()) == loop.LoopId)
                {
                    // Set flag
                    viewLoops.Items[a].ImageIndex = 7;
                }
                else
                {
                    // Reset flag
                    viewLoops.Items[a].ImageIndex = -1;
                }
            }

            // Reset buttons
            btnPlayLoop.Enabled = false;
            btnStopLoop.Enabled = true;
        }

        #endregion

        /// <summary>
        /// Occurs when the user clicks on one of the columns of the song gridview.
        /// </summary>
        /// <param name="data">Event data</param>
        private void viewSongs2_OnColumnClick(SongGridViewColumnClickData data)
        {
            // Refresh browser
            RefreshSongBrowser();
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
                treeLibrary.Height -= 102;
                panelUpdateLibraryProgress.Visible = true;
            }
            else
            {
                treeLibrary.Height += 102;
                panelUpdateLibraryProgress.Visible = false;
            }
        }

        private void btnCancelUpdateLibrary_Click(object sender, EventArgs e)
        {
            
        }
    }

    #region Classes and enums

    /// <summary>
    /// Defines the data structure for reporting progress when generating a wave form
    /// for the Loops and Markers UI.
    /// </summary>
    public class WorkerWaveFormLoopsMarkersReportProgress
    {
        /// <summary>
        /// Indicates how many bytes are read.
        /// </summary>
        public uint BytesRead { get; set; }
        /// <summary>
        /// Indicates the total number of bytes to read.
        /// </summary>
        public uint TotalBytes { get; set; }
        /// <summary>
        /// Indicates the percentage done.
        /// </summary>
        public float PercentageDone { get; set; }
        /// <summary>
        /// WaveDataMinMax data structure.
        /// </summary>
        public WaveDataMinMax WaveDataMinMax { get; set; }
    }

    /// <summary>
    /// Defines the arguments passed to the background worker of the tree library. This
    /// allows the background worker to get the type of operation it needs to do.
    /// </summary>
    public class WorkerTreeLibraryArgs
    {
        /// <summary>
        /// Operation type.
        /// </summary>
        public WorkerTreeLibraryOperationType OperationType { get; set; }
        /// <summary>
        /// Indicates which tree node to update.
        /// </summary>
        public TreeNode TreeNodeToUpdate { get; set; }
        /// <summary>
        /// Artist name.
        /// </summary>
        public string ArtistName { get; set; }
    }

    /// <summary>
    /// Defines the results coming out of the background worker of the tree library.
    /// </summary>
    public class WorkerTreeLibraryResult
    {
        /// <summary>
        /// Operation type.
        /// </summary>
        public WorkerTreeLibraryOperationType OperationType { get; set; }
        /// <summary>
        /// Indicates which tree node to update.
        /// </summary>
        public TreeNode TreeNodeToUpdate { get; set; }
        /// <summary>
        /// Artist name.
        /// </summary>
        public string ArtistName { get; set; }
        /// <summary>
        /// List of album titles.
        /// </summary>
        public List<string> Albums { get; set; }
        /// <summary>
        /// List of artist names.
        /// </summary>
        public List<string> Artists { get; set; }                
        /// <summary>
        /// List of albums (key = artist name, value = album title).
        /// </summary>
        public Dictionary<string, List<string>> AllAlbums { get; set; }
    }

    /// <summary>
    /// Defines what kind of operation the background worker process needs to do.
    /// </summary>
    public enum WorkerTreeLibraryOperationType
    {
        /// <summary>
        /// Gets all artists.
        /// </summary>
        GetArtists = 0, 
        /// <summary>
        /// Gets all albums from a specific artist.
        /// </summary>
        GetArtistAlbums = 1, 
        /// <summary>
        /// Gets all albums.
        /// </summary>
        GetAlbums = 2, 
        /// <summary>
        /// Gets all playlists.
        /// </summary>
        GetPlaylists = 3
    }

    /// <summary>
    /// Defines what the tree library node represents (artist, album, playlist, etc.)
    /// </summary>
    public enum TreeLibraryNodeType
    {
        /// <summary>
        /// "All" node type.
        /// </summary>
        All = 0, 
        /// <summary>
        /// "All songs" node type.
        /// </summary>
        AllSongs = 1, 
        /// <summary>
        /// "All artists" node type.
        /// </summary>
        AllArtists = 2,
        /// <summary>
        /// "All albums" node type.
        /// </summary>
        AllAlbums = 3,
        /// <summary>
        /// "All playlists" node type.
        /// </summary>
        AllPlaylists = 4,
        /// <summary>
        /// "Artist" node type.
        /// </summary>
        Artist = 5,
        /// <summary>
        /// "Album" node type.
        /// </summary>
        Album = 6,
        /// <summary>
        /// "Artist/Album" node type.
        /// </summary>
        ArtistAlbum = 7,
        /// <summary>
        /// "Playlist" node type.
        /// </summary>
        Playlist = 8,
        /// <summary>
        /// "Recently played" node type.
        /// </summary>
        RecentlyPlayed = 9
    }

    /// <summary>
    /// Data structure used with the Tag property of the tree library TreeNode object. 
    /// Contains the type of node and its query.
    /// </summary>
    public class TreeLibraryNodeMetadata
    {
        /// <summary>
        /// Defines the node type.
        /// </summary>
        public TreeLibraryNodeType NodeType { get; set; }

        /// <summary>
        /// Defines the query associated with this node type.
        /// </summary>
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