//
// frmSettings.cs: Settings window. This is where the user selects the driver and output device.
//                 The user can also configure the library folders in the Library tab.   
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Library;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Settings window. This is where the user selects the driver and output device.
    /// The user can also configure the library folders in the Library tab.    
    /// </summary>
    public partial class frmSettings : MPfm.WindowsControls.Form
    {
        // Private variables
        private bool settingsChanged = false;
        private bool settingsTested = false;
        private bool testSuccessful = false;
        private string filePath = string.Empty;        
        private List<Device> m_devices = null;
        private List<Device> m_devicesDirectSound = null;
        private List<Device> m_devicesASIO = null;
        private List<Device> m_devicesWASAPI = null;        
        
        private PeakFile m_peakFile = null;
        private UpdateLibrary m_updateLibrary = null;
        
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

        /// <summary>
        /// Constructor for Settings window. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmSettings(frmMain main)
        {
            InitializeComponent();
            m_main = main;


            //m_peakFile = new PeakFile(5);
            //m_peakFile.OnProcessStarted += new PeakFile.ProcessStarted(m_peakFile_OnProcessStarted);
            //m_peakFile.OnProcessData += new PeakFile.ProcessData(m_peakFile_OnProcessData);
            //m_peakFile.OnProcessDone += new PeakFile.ProcessDone(m_peakFile_OnProcessDone);

            //m_updateLibrary = new UpdateLibrary(5, Main.Library.Gateway.DatabaseFilePath);
            //m_updateLibrary.OnProcessData += new UpdateLibrary.ProcessData(m_importAudioFiles_OnProcessData);
            //m_updateLibrary.OnProcessDone += new UpdateLibrary.ProcessDone(m_importAudioFiles_OnProcessDone);
        }

        /// <summary>
        /// Occurs when the form has loaded.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmSettings_Load(object sender, EventArgs e)
        {
            List<string> controls = new List<string>();
            controls.Add("Main.SongBrowser");
            controls.Add("Playlist.SongBrowser");
            comboThemeControl.Items.Clear();
            foreach (string control in controls)
            {
                comboThemeControl.Items.Add(control);
            }
            comboThemeControl.SelectedIndex = 0;

            // Load sample data into grid
            List<AudioFile> audioFiles = new List<AudioFile>();
            for (int a = 0; a < 20; a++)
            {
                // Create audio file
                AudioFile audioFile = new AudioFile(@"file://", Guid.NewGuid(), false);
                audioFile.TrackNumber = (uint)a + 1;
                audioFile.Length = "10:23.450";
                audioFile.ArtistName = "Artist Name";
                audioFile.AlbumTitle = "Album Title";
                audioFile.Title = "Song Title #" + (a + 1).ToString();

                // Add to list
                audioFiles.Add(audioFile);
            }          

            // Set now playing song
            songBrowser.NowPlayingAudioFileId = audioFiles[0].Id;

            // Load into control
            songBrowser.Theme = Main.viewSongs2.Theme;
            songBrowser.ImportAudioFiles(audioFiles);
            propertyGridTheme.SelectedObject = songBrowser.Theme;

            // Set column widths
            songBrowser.Columns[0].Width = 100;
        }

        protected void m_importAudioFiles_OnProcessData(UpdateLibraryProgressData data)
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                if (data.Exception != null)
                {
                    MessageBox.Show(data.FilePath + "\n" + data.Exception.Message + "\n" + data.Exception.InnerException.StackTrace);
                }
                else
                {
                    lblOutputDriver.Text = data.PercentageDone.ToString();
                    lblDriver.Text = data.AudioFile.FilePath;
                    lblTest.Text = data.ThreadNumber.ToString();
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

        protected void m_importAudioFiles_OnProcessDone(UpdateLibraryDoneData data)
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Refresh everything
                lblOutputDriver.Text = "DONE";
                Main.Library.RefreshCache();
                Main.RefreshAll();
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

        protected void m_peakFile_OnProcessStarted(PeakFileStartedData data)
        {
            
        }

        protected void m_peakFile_OnProcessData(PeakFileProgressData data)
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                lblOutputDriver.Text = data.PercentageDone.ToString();
                lblDriver.Text = data.AudioFilePath;
                lblTest.Text = data.ThreadNumber.ToString();
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

        protected void m_peakFile_OnProcessDone(PeakFileDoneData data)
        {
            MessageBox.Show("DONE!!!!!");
        }

        #region Form Events

        /// <summary>
        /// Occurs when the form becomes visible (each time the user presses on the
        /// Settings button on the main form toolbar).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmSettings_Shown(object sender, EventArgs e)
        {
            // Detect devices
            m_devices = DeviceHelper.DetectOutputDevices();
            m_devicesDirectSound = m_devices.Where(x => x.DriverType == DriverType.DirectSound).ToList();
            m_devicesASIO = m_devices.Where(x => x.DriverType == DriverType.ASIO).ToList();
            m_devicesWASAPI = m_devices.Where(x => x.DriverType == DriverType.WASAPI).ToList();

            // Update combo box
            List<DriverComboBoxItem> drivers = new List<DriverComboBoxItem>();
            DriverComboBoxItem driverDirectSound = new DriverComboBoxItem() { DriverType = DriverType.DirectSound, Title = "DirectSound (default, recommended)" };
            DriverComboBoxItem driverASIO = new DriverComboBoxItem() { DriverType = DriverType.ASIO, Title = "ASIO (driver required) *EXPERIMENTAL*" };
            DriverComboBoxItem driverWASAPI = new DriverComboBoxItem() { DriverType = DriverType.WASAPI, Title = "WASAPI (Vista/Windows 7 only) *EXPERIMENTAL*" };
            drivers.Add(driverDirectSound);
            drivers.Add(driverASIO);
            drivers.Add(driverWASAPI);
            cboDrivers.DataSource = drivers;

            // Set default value
            cboDrivers.SelectedIndex = 0;

            // Refresh controls           
            RefreshFolders();

            // Load configuration
            LoadAudioConfig();
            LoadGeneralConfig();
            settingsChanged = false;            
        }

        #endregion

        #region Close Events

        /// <summary>
        /// Occurs when the form is about to close.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the Close button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Variables
            bool saveSettings = false;

            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Get selected device
            Device device = (Device)cboOutputDevices.SelectedItem;            

            // Check if the settings have changed
            if (settingsChanged)
            {
                //// Compare the original configured values to make sure the settings have really changed
                //if(driver.DriverType != Main.Config.Audio.DriverType ||
                //   device.Name.ToUpper() != Main.Config.Audio.Device.Name.ToUpper())
                //{
                    // Yes they have really changed!
                    // Have the new settings been tested?
                    if (!settingsTested)
                    {
                        // Warn user
                        DialogResult dialogResult = MessageBox.Show(this, "Warning: The new audio configuration hasn't been tested. Saving an incompatible configuration WILL crash the application.\nTo reset the application configuration, you must delete the MPfm configuration file (MPfm.Configuration.xml) in the MPfm application data folder (" + Main.ApplicationDataFolderPath + "). This will display the First Run screen again.\n\nClick YES to save and apply the untested configuration.\nClick NO to exit the Settings window without saving the new configuration.\nClick CANCEL to go back to the Settings window and test your new configuration.", "Warning: New audio configuration untested", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                        // Yes: Continue and save untested settings
                        // No: Do not save new settings
                        // Cancel: Go back and change settings
                        if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                        {
                            // Cancel 
                            return;
                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            // Set save settings flag
                            saveSettings = true;
                        }
                    }
                    else if(settingsTested && !testSuccessful)
                    {
                        // The configuration has been tested but the audio test has failed
                        MessageBox.Show("Error: You cannot apply an incompatible audio configuration because this will crash the application.\nPlease select a new audio configuration.", "Cannot apply an incompatible configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else if (settingsTested && testSuccessful)
                    {                        
                        // Warn user
                        DialogResult dialogResult = MessageBox.Show(this, "Are you sure you wish to save and apply this new audio configuration?\n\nClick YES to save and apply the new configuration.\nClick NO to exit the Settings window without saving the new configuration.\nClick CANCEL to go back to the Settings window and change the configuration.", "New audio configuration validation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        // Yes: Continue and save tested settings
                        // No: Do not save new settings
                        // Cancel: Go back and change settings
                        if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                        {
                            // Cancel 
                            return;
                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            // Set save settings flag
                            saveSettings = true;
                        }
                    }
                //}

                // Ask user to save.

                // Nothing has changed; just exit without warning
                // HOWEVER: The user might have tested something, so the player might have been disposed. Add a flag for this.
            }

            // Save new settings?
            if (saveSettings)
            {
                // Save new configuration
                SaveAudioConfig();

                // Check if the device has been initialized
                if (!Main.Player.IsDeviceInitialized)
                {
                    // Initialize new device
                    Main.Player.InitializeDevice(device);
                }
            }
            else
            {
                // Do not save settings; restore original configuration                
                Device originalDevice = null;
                if (Main.Config.Audio.DriverType == DriverType.DirectSound)
                {
                    // Loop through devices
                    for (int a = 0; a < m_devicesDirectSound.Count; a++)
                    {
                        // Check if the device matches
                        if (m_devicesDirectSound[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
                        {
                            // Set device
                            originalDevice = m_devicesDirectSound[a];
                            break;
                        }
                    }
                }
                else if (Main.Config.Audio.DriverType == DriverType.ASIO)
                {
                    // Loop through devices
                    for (int a = 0; a < m_devicesASIO.Count; a++)
                    {
                        // Check if the device matches
                        if (m_devicesASIO[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
                        {
                            // Set device
                            originalDevice = m_devicesASIO[a];
                            break;
                        }
                    }
                }
                else if (Main.Config.Audio.DriverType == DriverType.WASAPI)
                {
                    // Loop through devices
                    for (int a = 0; a < m_devicesWASAPI.Count; a++)
                    {
                        // Check if the device matches
                        if (m_devicesWASAPI[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
                        {
                            // Set device
                            originalDevice = m_devicesWASAPI[a];
                            break;
                        }
                    }
                }

                // Check if the device has been initialized
                if (!Main.Player.IsDeviceInitialized)
                {
                    // Initialize new device
                    Main.Player.InitializeDevice(originalDevice);
                }
            }

            // Reset flags
            settingsChanged = false;
            testSuccessful = false;
            settingsTested = false;

            // Hide form
            this.Close();
            Main.BringToFront();
            Main.Focus();
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Loads the audio settings from the configuration file.
        /// </summary>
        private void LoadAudioConfig()
        {
            // Load values into controls
            cboOutputDevices.SelectedText = Main.Config.Audio.Device.Name;
            cboDrivers.SelectedValue = Main.Config.Audio.DriverType;
            txtMixerSampleRate.Value = Main.Config.Audio.Mixer.Frequency;
            txtBufferSize.Value = Main.Config.Audio.Mixer.BufferSize;
            txtUpdatePeriod.Value = Main.Config.Audio.Mixer.UpdatePeriod;

            // Check driver
            if (Main.Config.Audio.DriverType == DriverType.DirectSound)
            {
                // Loop through devices
                for (int a = 0; a < m_devicesDirectSound.Count; a++)
                {
                    // Check if the device matches
                    if (m_devicesDirectSound[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
                    {
                        // Set selected index
                        cboOutputDevices.SelectedIndex = a;
                        break;
                    }
                }
            }
            else if (Main.Config.Audio.DriverType == DriverType.ASIO)
            {
                // Loop through devices
                for (int a = 0; a < m_devicesASIO.Count; a++)
                {
                    // Check if the device matches
                    if (m_devicesASIO[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
                    {
                        // Set selected index
                        cboOutputDevices.SelectedIndex = a;
                        break;
                    }
                }
            }
            else if (Main.Config.Audio.DriverType == DriverType.WASAPI)
            {
                // Loop through devices
                for (int a = 0; a < m_devicesWASAPI.Count; a++)
                {
                    // Check if the device matches
                    if (m_devicesWASAPI[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
                    {
                        // Set selected index
                        cboOutputDevices.SelectedIndex = a;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Saves the audio settings to the configuration file.
        /// </summary>
        private void SaveAudioConfig()
        {
            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Get selected device
            Device device = (Device)cboOutputDevices.SelectedItem;

            // Save configuration values
            Main.Config.Audio.Device.Name = device.Name;
            Main.Config.Audio.DriverType = driver.DriverType;
            Main.Config.Audio.Mixer.Frequency = (int)txtMixerSampleRate.Value;
            Main.Config.Audio.Mixer.BufferSize = (int)txtBufferSize.Value;
            Main.Config.Audio.Mixer.UpdatePeriod = (int)txtUpdatePeriod.Value;
            
        }

        /// <summary>
        /// Loads the general settings from the configuration file.
        /// </summary>
        private void LoadGeneralConfig()
        {
            // Load tray options
            bool? hideTray = Main.Config.GetKeyValueGeneric<bool>("HideTray");
            bool? showTray = Main.Config.GetKeyValueGeneric<bool>("ShowTray");
            chkShowTray.Checked = (showTray.HasValue) ? showTray.Value : false;
            chkHideTray.Checked = (hideTray.HasValue) ? hideTray.Value : false;
            chkHideTray.Enabled = chkShowTray.Enabled;
        }

        /// <summary>
        /// Saves the general settings to the configuration file.
        /// </summary>
        private void SaveGeneralConfig()
        {
            // Save configuration values
            Main.Config.SetKeyValue<bool>("HideTray", chkHideTray.Checked);
            Main.Config.SetKeyValue<bool>("ShowTray", chkShowTray.Checked);
        }

        #endregion

        #region Refresh Methods

        /// <summary>
        /// Refreshes the list of folders.
        /// </summary>
        public void RefreshFolders()
        {
            // Get the list of folders from the database            
            List<Folder> folders = m_main.Library.Gateway.SelectFolders();

            // Check if the list is null
            if (folders != null)
            {
                // Clear items                 
                listViewFolders.Items.Clear();

                // Loop through folders
                foreach (Folder folder in folders)
                {
                    // Add item
                    ListViewItem item = new ListViewItem(folder.FolderPath);
                    item.Tag = folder.FolderId;
                    item.SubItems.Add(folder.IsRecursive.ToString());                                        
                    listViewFolders.Items.Add(item);
                }
            }
        }

        #endregion

        #region Library Tab Events

        /// <summary>
        /// Occurs when the user clicks on the Add Folder button. The user must select
        /// a path to add to the library.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            if (dialogAddFolder.ShowDialog() == DialogResult.OK)
            {
                // Check if folder already exists
                //Folder folder = DataAccess.SelectFolderByPath(dialogAddFolder.SelectedPath);
                Folder folder = m_main.Library.Gateway.SelectFolderByPath(dialogAddFolder.SelectedPath);

                // Cancel if folder already exists
                if (folder != null)
                {
                    MessageBox.Show("The folder has already been added to your list!", "Error adding folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ask user for recursive value
                bool recursive = false;
                DialogResult result = MessageBox.Show("Do you want MPfm to search the folder recursively (e.g. it will search sub-folders)?", "Folder recursivity", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    recursive = true;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }

                // Create new folder
                //DataAccess.InsertFolder(dialogAddFolder.SelectedPath, recursive);
                m_main.Library.Gateway.InsertFolder(dialogAddFolder.SelectedPath, recursive);
                RefreshFolders();

                //ArrayList list = db.GetFolderNewSongs(dialogAddFolder.SelectedPath);
                //StopSong();
                //formAddFolderStatus.ShowDialog(this);                
            }

        }

        /// <summary>
        /// Occurs when the user clicks on the Remove Folder button. The user is prompted with a 
        /// dialog box to make sure if he/she wants to remove the folders. The user is also prompted with a dialog box
        /// if he/she wants to remove the songs from the folders from the library.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnRemoveFolder_Click(object sender, EventArgs e)
        {
            // If no items are selected, return immediately
            if (listViewFolders.SelectedItems.Count == 0)
            {
                return;
            }

            // Get user confirmation
            if (MessageBox.Show("Are you sure you wish to remove the selected folders from your library?", "Removing folders from your library", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            // Ask the user if he/she wants to remove the songs from his/her library
            bool removeSongsFromLibrary = false;
            DialogResult result = MessageBox.Show("Do you want to remove the songs of this folder from your library?", "Remove songs from library", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                removeSongsFromLibrary = true;
            }
            else if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            // Delete selected folders
            foreach (ListViewItem item in listViewFolders.SelectedItems)
            {
                // Check if the tag is null
                if (item.Tag != null)
                {
                    // Get the folder id
                    Guid folderId = new Guid(item.Tag.ToString());

                    // Remove songs that match the path
                    if (removeSongsFromLibrary)
                    {
                        Main.Library.RemoveSongsFromLibrary(item.SubItems[0].Text);
                    }

                    // Delete the folder from the list of configured folders                    
                    Main.Library.Gateway.DeleteFolder(folderId);

                    // Remove item from list view
                    item.Remove();
                }
            }

            // Refresh cache
            Main.Library.RefreshCache();
            Main.RefreshAll();
        }

        /// <summary>
        /// Occurs when the user tries to change the width of a column. This blocks the user from
        /// doing so.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void listViewFolders_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Update Library" button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnUpdateLibrary_Click(object sender, EventArgs e)
        {
            // Display the update library window and update the whole library
            Main.UpdateLibrary();
        }


        /// <summary>
        /// Fires when the user clicks on the Reset Library button. Confirms the reset library with the user.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void btnResetLibrary_Click(object sender, EventArgs e)
        {
            // Confirm operation
            if (MessageBox.Show(this, "Are you sure you wish to reset your library?\n\nThis will remove all the songs, loops, markers and playlists from your library.\nThis will *NOT* delete the actual files from your hard disk.", "Reset Library Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                // Stop the song if one is playing
                if (Main.Player.IsPlaying)
                {
                    // Stop playback
                    Main.Player.Stop();
                }

                // Reset library
                Main.Library.ResetLibrary();

                // Refresh everything
                Main.RefreshAll();
            }
        }

        #endregion

        #region Audio Settings Tab Events

        /// <summary>
        /// Occurs when the user selects a driver or an output type using the combo boxes.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void cboDriverOrOutputType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set flags
            settingsChanged = true;
            settingsTested = false;
            testSuccessful = false;

            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Check driver type
            if (driver.DriverType == DriverType.DirectSound)
            {
                // Set combo box data source
                cboOutputDevices.DataSource = m_devicesDirectSound;

                // Find default device
                Device defaultDevice = m_devicesDirectSound.FirstOrDefault(x => x.IsDefault);
            }
            else if (driver.DriverType == DriverType.ASIO)
            {
                // Set combo box data source
                cboOutputDevices.DataSource = m_devicesASIO;

                // Find default device
                Device defaultDevice = m_devicesASIO.FirstOrDefault(x => x.IsDefault);
            }
            else if (driver.DriverType == DriverType.WASAPI)
            {
                // Set combo box data source
                cboOutputDevices.DataSource = m_devicesWASAPI;

                // Find default device
                Device defaultDevice = m_devicesWASAPI.FirstOrDefault(x => x.IsDefault);
            }
        }        

        /// <summary>
        /// Occurs when the user clicks on the Test audio configuration button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnTestSound_Click(object sender, EventArgs e)
        {
            // Set flags
            settingsTested = false;
            testSuccessful = false;

            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Get selected device
            Device device = (Device)cboOutputDevices.SelectedItem;

            try
            {
                // Warn user if system is already playing a song
                if (Main.Player.IsPlaying)
                {
                    // Display message box                    
                    if (MessageBox.Show(this, "Testing an audio file will stop the current playback. Click OK to continue or click Cancel to cancel the test.", "Interrupt playback", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        // The user cancelled
                        return;
                    }

                    // Stop player
                    Main.Player.Stop();
                }

                // Log 
                Tracing.Log("Starting audio settings test with the following settings: ");
                Tracing.Log("Driver Type: " + driver.DriverType.ToString());
                Tracing.Log("Output Device Id: " + device.Id);
                Tracing.Log("Output Device Name: " + device.Name);
                Tracing.Log("Output Device Driver: " + device.Driver);
                Tracing.Log("Output Device IsDefault: " + device.IsDefault.ToString());

                // Free device                
                Main.Player.FreeDevice();

                // Create test device
                Tracing.Log("Creating test device...");

                // Display the open file dialog (set filepath first)
                Tracing.Log("User selects a file.");
                dialogOpenFile.FileName = filePath;
                if (dialogOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }

                // Set flags
                settingsTested = true;

                // Create test device
                Tracing.Log("Creating test device...");
                TestDevice testDevice = new TestDevice(driver.DriverType, device.Id, (int)txtMixerSampleRate.Value);

                // Play sound file                
                Tracing.Log("Starting playback...");
                testDevice.Play(dialogOpenFile.FileName);
                Tracing.Log("The audio file is playing...");

                // Display info
                MessageBox.Show(this, "The sound system was initialized successfully.\nYou should now hear the file you have selected in the previous dialog.\nIf you do not hear a sound, your configuration might not working (unless you selected the \"No audio\" driver).\nIn that case, check the volume of your sound card mixer, or try changing the driver and/or output device.", "Sound system is working", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Stop and dispose the device
                Tracing.Log("User stops playback.");
                testDevice.Stop();

                // Dispose test device
                Tracing.Log("Disposing test device...");
                testDevice.Dispose();

                // The test is successful           
                Tracing.Log("The audio settings test is successful!");

                // Set flags
                testSuccessful = true;
            }
            catch (Exception ex)
            {
                // Show error
                MessageBox.Show(this, "Error testing sound configuration!\nThis audio configuration might not work on your system.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error testing sound configuration!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracing.Log("The audio settings test has failed!");
                Tracing.Log("Exception message: " + ex.Message);
                Tracing.Log("Stack trace: " + ex.StackTrace);
            }

            Tracing.Log("End of audio settings test.");
        }

        #endregion

        #region Tray Options

        /// <summary>
        /// Occurs when the user clicks on the "Show tray" check box.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void chkShowTray_CheckedChanged(object sender, EventArgs e)
        {
            settingsChanged = true;

            // Enable checkboxes depending on value
            if (chkShowTray.Checked)
            {
                chkHideTray.Enabled = true;
            }
            else
            {
                chkHideTray.Enabled = false;
                chkHideTray.Checked = false;
            }

            // Set tray icon visibility
            Main.notifyIcon.Visible = chkShowTray.Checked;

            // Save settings
            SaveGeneralConfig();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Hide tray" check box.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void chkHideTray_CheckedChanged(object sender, EventArgs e)
        {
            settingsChanged = true;

            // Save settings
            SaveGeneralConfig();
        }

        #endregion

        /// <summary>
        /// Occurs when the user clicks on the "Display MPfm in system tray" label.
        /// Triggers the related checkbox.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lblShowTray_Click(object sender, EventArgs e)
        {
            // Set opposite value
            chkShowTray.Checked = !chkShowTray.Checked;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Hide MPfm in system tray" label.
        /// Triggers the related checkbox.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lblHideTray_Click(object sender, EventArgs e)
        {
            // Set opposite value
            if (chkHideTray.Enabled)
            {
                chkHideTray.Checked = !chkHideTray.Checked;
            }
        }

        /// <summary>
        /// Occurs when the mixer sample rate value changes.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void txtMixerSampleRate_ValueChanged(object sender, EventArgs e)
        {
            // Set flags
            settingsChanged = true;
            settingsTested = false;
            testSuccessful = false;
        }

        /// <summary>
        /// Occurs when the buffer size value changes.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void txtBufferSize_ValueChanged(object sender, EventArgs e)
        {
            // Set flags
            settingsChanged = true;
            settingsTested = false;
            testSuccessful = false;
        }

        /// <summary>
        /// Occurs when the update period value changes.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void txtUpdatePeriod_ValueChanged(object sender, EventArgs e)
        {
            // Set flags
            settingsChanged = true;
            settingsTested = false;
            testSuccessful = false;
        }

        private void btnTestPeak_Click(object sender, EventArgs e)
        {
            try
            {
                MPfmConfiguration cfg = new MPfmConfiguration(@"D:\Code\MPfm\Branches\Current\MPfm\ExampleConfig.xml");
                cfg.Clear();
                //cfg.Load();
                cfg.Audio.Mixer.Frequency = 44100;
                cfg.General.KeyValues.Add(new GeneralConfigurationKeyValue() { Name = "Test", Value = "hahaha", ValueType = typeof(string) });
                cfg.General.KeyValues.Add(new GeneralConfigurationKeyValue() { Name = "Test2", Value = 22, ValueType = typeof(Int32) });
                cfg.General.KeyValues.Add(new GeneralConfigurationKeyValue() { Name = "Test3", Value = 100.33003f, ValueType = typeof(float) });
                cfg.Controls.SongGridView.Columns.Clear();
                cfg.Controls.SongGridView.Columns.AddRange(Main.viewSongs2.Columns);
                cfg.Save(@"C:\_\config.xml");
                return;

                //string peakFileDirectory = @"D:\_peak\";

                //List<string> filePaths = AudioTools.SearchAudioFilesRecursive(txtPath.Text, "MP3;FLAC;OGG");

                //Dictionary<string, string> dictionary = new Dictionary<string, string>();
                //foreach (string filePath in filePaths)
                //{
                //    // Extract file name from path
                //    //string fileName = Path.GetFileName(filePath);
                //    string peakFilePath = peakFileDirectory + filePath.Replace(@"\", "_").Replace(":", "_").Replace(".", "_") + ".mpfmPeak";

                //    // Add dictionary value
                //    dictionary.Add(filePath, peakFilePath);
                //}

                //m_peakFile.GeneratePeakFiles(dictionary);

                //List<string> filePaths = AudioTools.SearchAudioFilesRecursive(txtPath.Text, "MP3;FLAC;OGG");

                m_updateLibrary = new UpdateLibrary(5, Main.Library.Gateway.DatabaseFilePath);
                m_updateLibrary.OnProcessData += new UpdateLibrary.ProcessData(m_importAudioFiles_OnProcessData);
                m_updateLibrary.OnProcessDone += new UpdateLibrary.ProcessDone(m_importAudioFiles_OnProcessDone);

                m_updateLibrary.ImportFolder(txtPath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void btnStopPeak_Click(object sender, EventArgs e)
        {
            //m_peakFile.ReadPeakFile(@"D:\_peak\01 Natural Mystic_mp3.mpfmPeak");
            //m_peakFile.ReadPeakFile(@"D:\_peak\03 Guiltiness_mp3.mpfmPeak");            
            //m_peakFile.ReadPeakFile(@"D:\_peak\E__Mp3_Bob Marley_Exodus_05 Exodus_mp3.mpfmPeak");

            try
            {
                //m_peakFile.Cancel();
                m_updateLibrary.Cancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void propertyGridTheme_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // Get theme
            SongGridViewTheme theme = (SongGridViewTheme)propertyGridTheme.SelectedObject;

            // Refresh theme
            songBrowser.Theme = theme;
            songBrowser.InvalidateSongCache();
            songBrowser.Refresh();
        }
    }

}