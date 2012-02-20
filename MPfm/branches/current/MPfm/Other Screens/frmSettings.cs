//
// frmSettings.cs: Settings window. This is where the user selects the driver and output device.
//                 The user can also configure the library folders in the Library tab.   
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

        #region Form Events

        /// <summary>
        /// Occurs when the form has loaded.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmSettings_Load(object sender, EventArgs e)
        {
        }

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

            // Set general settings lavels
            lblPeakFileDefaultDirectory.Text = "Use default directory (" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MPfm\\Peak Files\\)";

            // Refresh controls           
            RefreshFolders();

            btnGeneralSettings.Enabled = false;
            btnAudioSettings.Enabled = true;
            btnLibrarySettings.Enabled = true;

            panelGeneralSettings.Visible = true;
            panelAudioSettings.Visible = false;
            panelLibrarySettings.Visible = false;

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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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

            // Save general settings
            if (!SaveGeneralConfig())
            {
                // Cancel
                return;
            }            

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
            }

            // Save new settings?
            if (saveSettings)
            {
                // Save new configuration
                SaveAudioConfig();

                // Check if the player is playing
                if (Main.Player.IsPlaying)
                {
                    // Stop playback
                    Main.Stop();
                }

                // Free device
                Main.Player.FreeDevice();

                // Initialize new device
                Main.Player.UpdatePeriod = (int)txtUpdatePeriod.Value;
                Main.Player.BufferSize = (int)txtBufferSize.Value;
                Main.Player.InitializeDevice(device, (int)txtMixerSampleRate.Value);

                //// Check if the device has been initialized
                //if (!Main.Player.IsDeviceInitialized)
                //{
                //    // Initialize new device
                //    Main.Player.UpdatePeriod = (int)txtUpdatePeriod.Value;
                //    Main.Player.BufferSize = (int)txtBufferSize.Value;                    
                //    Main.Player.InitializeDevice(device);
                //}
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
                    Main.Player.InitializeDevice(originalDevice, Main.Config.Audio.Mixer.Frequency);

                    // Set original properties
                    Main.Player.BufferSize = Main.Config.Audio.Mixer.BufferSize;
                    Main.Player.UpdatePeriod = Main.Config.Audio.Mixer.UpdatePeriod;
                }
            }

            // Reset flags
            settingsChanged = false;
            testSuccessful = false;
            settingsTested = false;

            // Hide form            
            Main.BringToFront();
            Main.Focus();
            this.Close();
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
            trackBufferSize.Value = Main.Config.Audio.Mixer.BufferSize;
            trackUpdatePeriod.Value = Main.Config.Audio.Mixer.UpdatePeriod;

            // Check sample rate
            if (Main.Config.Audio.Mixer.Frequency == 44100)
            {
                radio44100Hz.Checked = true;
            }
            else if (Main.Config.Audio.Mixer.Frequency == 48000)
            {
                radio48000Hz.Checked = true;
            }
            else if (Main.Config.Audio.Mixer.Frequency == 96000)
            {
                radio96000Hz.Checked = true;
            }

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

            Main.Config.Save();
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

            // Load peak file options
            bool? peakFileUseCustomDirectory = Main.Config.GetKeyValueGeneric<bool>("PeakFile_UseCustomDirectory");            
            bool? peakFileDisplayWarning = Main.Config.GetKeyValueGeneric<bool>("PeakFile_DisplayWarning");            
            int? peakFileDisplayWarningThreshold = Main.Config.GetKeyValueGeneric<int>("PeakFile_DisplayWarningThreshold");
            string peakFileCustomDirectory = Main.Config.GetKeyValue("PeakFile_CustomDirectory");

            radioPeakFileCustomDirectory.Checked = (peakFileUseCustomDirectory.HasValue) ? peakFileUseCustomDirectory.Value : false;            
            chkPeakFileDisplayWarning.Checked = (peakFileDisplayWarning.HasValue) ? peakFileDisplayWarning.Value : false;            
            txtPeakFileDisplayWarningThreshold.Value = (peakFileDisplayWarningThreshold.HasValue) ? peakFileDisplayWarningThreshold.Value : 1000;
            txtPeakFileCustomDirectory.Text = peakFileCustomDirectory;

            // Set control enable
            EnableGeneralSettingsControls();
        }

        /// <summary>
        /// Saves the general settings to the configuration file.
        /// </summary>
        /// <returns>Indicates if the save was successful</returns>
        private bool SaveGeneralConfig()
        {
            // Validate peak file directory if custom
            if (radioPeakFileCustomDirectory.Checked)
            {
                // Validate directory existence
                if (!Directory.Exists(txtPeakFileCustomDirectory.Text))
                {
                    MessageBox.Show("Error: The custom peak file directory does not exist.\nPlease select the default peak file directory or use the Browse button to create a custom directory.", "Peak file directory does not exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // Check if the peak file directory has changed    
            string peakFileFolderPath = (radioPeakFileCustomDirectory.Checked) ? txtPeakFileCustomDirectory.Text : Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MPfm\\Peak Files";
            //if (Main.PeakFileFolderPath != peakFileFolderPath)
            //{
            //    // Ask user if he wants to delete the peak files in the original folder (or move them?)
            //    DialogResult dialogResult = MessageBox.Show("The peak file directory has changed.\nDo you wish to delete the existing peak files in the older directory?\n\nClick YES to delete peak files in the older directory.\nClick NO to keep the peak files in the older directory.\nClick CANCEL to go back to the Settings window.", "Delete peak files", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            //    if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
            //    {
            //        return false;
            //    }
            //    else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            //    {
            //        // Delete peak files
            //        PeakFile.DeletePeakFiles(Main.PeakFileFolderPath);
            //    }
            //}

            // Save tray options
            Main.Config.SetKeyValue<bool>("HideTray", chkHideTray.Checked);
            Main.Config.SetKeyValue<bool>("ShowTray", chkShowTray.Checked);

            // Save peak file options
            Main.Config.SetKeyValue<bool>("PeakFile_UseCustomDirectory", radioPeakFileCustomDirectory.Checked);
            Main.Config.SetKeyValue("PeakFile_CustomDirectory", txtPeakFileCustomDirectory.Text);
            Main.Config.SetKeyValue<bool>("PeakFile_DisplayWarning", chkPeakFileDisplayWarning.Checked);
            Main.Config.SetKeyValue<int>("PeakFile_DisplayWarningThreshold", (int)txtPeakFileDisplayWarningThreshold.Value);

            // Set current peak file folder
            Main.PeakFileFolderPath = peakFileFolderPath;

            // Refresh peak file warning
            Main.RefreshPeakFileDirectorySizeWarning();

            Main.Config.Save();

            return true;
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void listViewFolders_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Update Library" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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
                    Main.Stop();
                }

                // Log 
                Tracing.Log("Starting audio settings test with the following settings: ");
                Tracing.Log("Driver Type: " + driver.DriverType.ToString());
                Tracing.Log("Output Device Id: " + device.Id);
                Tracing.Log("Output Device Name: " + device.Name);
                Tracing.Log("Output Device Driver: " + device.Driver);
                Tracing.Log("Output Device IsDefault: " + device.IsDefault.ToString());

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

                // Free device                
                Main.Player.FreeDevice();

                //// Create test device
                //Tracing.Log("Creating test device...");
                //TestDevice testDevice = new TestDevice(driver.DriverType, device.Id, (int)txtMixerSampleRate.Value);

                //// Play sound file                
                //Tracing.Log("Starting playback...");
                //testDevice.Play(dialogOpenFile.FileName);
                //Tracing.Log("The audio file is playing...");

                //// Display info
                //MessageBox.Show(this, "The sound system was initialized successfully.\nYou should now hear the file you have selected in the previous dialog.\nIf you do not hear a sound, your configuration might not working (unless you selected the \"No audio\" driver).\nIn that case, check the volume of your sound card mixer, or try changing the driver and/or output device.", "Sound system is working", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //// Stop and dispose the device
                //Tracing.Log("User stops playback.");
                //testDevice.Stop();

                //// Dispose test device
                //Tracing.Log("Disposing test device...");
                //testDevice.Dispose();

                // Disable output meter timer
                Main.timerUpdateOutputMeter.Enabled = false;

                // Create test device
                Tracing.Log("Creating test device...");
                Main.Player.InitializeDevice(device, (int)txtMixerSampleRate.Value);

                // Set player properties
                Main.Player.UpdatePeriod = (int)txtUpdatePeriod.Value;
                Main.Player.BufferSize = (int)txtBufferSize.Value;

                // Play sound file                
                Tracing.Log("Starting playback...");
                Main.Player.PlayFiles(dialogOpenFile.FileNames.ToList());                
                Tracing.Log("The audio file is playing...");

                // Display info
                MessageBox.Show(this, "The sound system was initialized successfully.\nYou should now hear the file you have selected in the previous dialog.\nIf you do not hear a sound, your configuration might not working.\nIn that case, check the volume of your sound card mixer, or try changing the driver and/or output device.", "Sound system is working", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Stop and dispose the device
                Tracing.Log("User stops playback.");
                Main.Player.Stop();                

                // Dispose test device
                Tracing.Log("Disposing test device...");
                Main.Player.FreeDevice();                

                // Re-enaable output meter timer
                Main.timerUpdateOutputMeter.Enabled = true;

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

        /// <summary>
        /// Occurs when the user clicks on the Reset to Default button (in the Audio Settings tab).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnResetToDefault_Click(object sender, EventArgs e)
        {
            // Select DirectSound driver
            cboDrivers.SelectedIndex = 0;

            // Loop through DirectSound devices to get the default device
            for (int a = 0; a < m_devicesDirectSound.Count; a++)
            {
                // Is this the default device?
                if (m_devicesDirectSound[a].IsDefault)
                {
                    // Set default device and exit loop
                    cboOutputDevices.SelectedIndex = a;
                    break;
                }
            }

            // Set default values
            radio44100Hz.Checked = true;            
            txtBufferSize.Value = 1000;
            trackBufferSize.Value = 1000;
            txtUpdatePeriod.Value = 10;
            trackUpdatePeriod.Value = 10;
        }

        #endregion

        #region Tray Options

        /// <summary>
        /// Occurs when the user clicks on the "Show tray" check box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void chkShowTray_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the show tray is checked
            if (!chkShowTray.Checked)
            {
                // Reset check
                chkHideTray.Checked = false;
            }

            // Set check box enable
            EnableGeneralSettingsControls();

            // Set tray icon visibility
            Main.notifyIcon.Visible = chkShowTray.Checked;
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

        /// <summary>
        /// Occurs when the user changes the update period value using the track bar.
        /// </summary>
        private void trackUpdatePeriod_OnTrackBarValueChanged()
        {
            // Set value
            txtUpdatePeriod.Value = trackUpdatePeriod.Value;
        }

        /// <summary>
        /// Occurs when the user changes the buffer size value using the track bar.
        /// </summary>
        private void trackBufferSize_OnTrackBarValueChanged()
        {
            // Set value
            txtBufferSize.Value = trackBufferSize.Value;
        }

        /// <summary>
        /// Occurs when the user clicks on the 44100Hz label.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lbl44100Hz_Click(object sender, EventArgs e)
        {
            radio44100Hz.Checked = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the 48000Hz label.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lbl48000Hz_Click(object sender, EventArgs e)
        {
            radio48000Hz.Checked = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the 96000Hz label.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lbl96000Hz_Click(object sender, EventArgs e)
        {
            radio96000Hz.Checked = true;
        }

        /// <summary>
        /// Occurs when the user clicks on one of the sample rate radio buttons.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void radio44100Hz_CheckedChanged(object sender, EventArgs e)
        {
            // Check which radio button is checked
            if (radio44100Hz.Checked)
            {
                // Set value
                txtMixerSampleRate.Value = 44100;
            }
            else if (radio48000Hz.Checked)
            {
                // Set value
                txtMixerSampleRate.Value = 48000;
            }
            else if (radio96000Hz.Checked)
            {
                // Set value
                txtMixerSampleRate.Value = 96000;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on one of peak file directory radio buttons.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void radioPeakFile_CheckedChanged(object sender, EventArgs e)
        {
            // Check which radio button is checked
            if (radioPeakFileDefaultDirectory.Checked)
            {
                
                
            }
            else if (radioPeakFileCustomDirectory.Checked)
            {

            }

            // Enable/disable controls
            txtPeakFileCustomDirectory.Enabled = radioPeakFileCustomDirectory.Checked;
            btnPeakFileCustomDirectoryBrowse.Enabled = radioPeakFileCustomDirectory.Checked;
        }

        /// <summary>
        /// Occurs when the user clicks on the Peak File Default Directory label.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lblPeakFileDefaultDirectory_Click(object sender, EventArgs e)
        {
            radioPeakFileDefaultDirectory.Checked = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the Peak File Custom Directory label.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lblPeakFileCustomDirectory_Click(object sender, EventArgs e)
        {
            radioPeakFileCustomDirectory.Checked = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the Browse (Peak File Custom Directory) button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnPeakFileCustomDirectoryBrowse_Click(object sender, EventArgs e)
        {
            // Display dialog
            if (dialogBrowsePeakFileDirectory.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Set text box value
                txtPeakFileCustomDirectory.Text = dialogBrowsePeakFileDirectory.SelectedPath;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the Peak File Use Maximum label.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lblPeakFileUseMaximum_Click(object sender, EventArgs e)
        {
            chkPeakFileDisplayWarning.Checked = !chkPeakFileDisplayWarning.Checked;
        }

        /// <summary>
        /// Occurs when the user clicks on the Peak File Use Maximum check box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void chkPeakFileUseMaximum_CheckedChanged(object sender, EventArgs e)
        {
            // Enable/disable controls
            EnableGeneralSettingsControls();
        }

        /// <summary>
        /// Enables/disables controls in General Settings tab depending on the configuration values.
        /// </summary>
        private void EnableGeneralSettingsControls()
        {
            // Enable checkboxes depending on value
            chkHideTray.Enabled = chkShowTray.Checked;

            // Check if the show tray is checked
            if (chkShowTray.Checked)
            {
                // Set label color
                lblHideTray.ForeColor = Color.Black;
            }
            else
            {
                // Set label color
                lblHideTray.ForeColor = Color.FromArgb(80, 80, 80);
            }

            // Enable checkboxes depending on value
            txtPeakFileDisplayWarningThreshold.Enabled = chkPeakFileDisplayWarning.Checked;

            // Check check box value
            if (chkPeakFileDisplayWarning.Checked)
            {
                // Set label color                
                lblPeakFileDisplayWarningUnit.ForeColor = Color.Black;
            }
            else
            {
                // Set label color                
                lblPeakFileDisplayWarningUnit.ForeColor = Color.FromArgb(80, 80, 80);
            }
        }

        private void btnGeneralSettings_Click(object sender, EventArgs e)
        {
            btnGeneralSettings.Enabled = false;
            btnAudioSettings.Enabled = true;
            btnLibrarySettings.Enabled = true;

            panelGeneralSettings.Visible = true;
            panelAudioSettings.Visible = false;
            panelLibrarySettings.Visible = false;
        }

        private void btnAudioSettings_Click(object sender, EventArgs e)
        {
            btnGeneralSettings.Enabled = true;
            btnAudioSettings.Enabled = false;
            btnLibrarySettings.Enabled = true;

            panelGeneralSettings.Visible = false;
            panelAudioSettings.Visible = true;
            panelLibrarySettings.Visible = false;

        }

        private void btnLibrarySettings_Click(object sender, EventArgs e)
        {
            btnGeneralSettings.Enabled = true;
            btnAudioSettings.Enabled = true;
            btnLibrarySettings.Enabled = false;

            panelGeneralSettings.Visible = false;
            panelAudioSettings.Visible = false;
            panelLibrarySettings.Visible = true;

        }
        
        public void RefreshShits()
        {

        }
    }

}