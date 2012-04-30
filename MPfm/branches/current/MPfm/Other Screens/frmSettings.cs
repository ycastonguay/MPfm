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
        private AudioSettingsState audioSettingsState = AudioSettingsState.NotChanged;
        private bool initializing = false;
        private string filePath = string.Empty;        
        private List<Device> devices = null;
        private List<Device> devicesDirectSound = null;
        private List<Device> devicesASIO = null;
        private List<Device> devicesWASAPI = null;        
                
        private frmMain main = null;
        /// <summary>
        /// Hook to the main form.
        /// </summary>
        public frmMain Main
        {
            get
            {
                return main;
            }
        }

        /// <summary>
        /// Constructor for Settings window. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmSettings(frmMain main)
        {
            InitializeComponent();
            this.main = main;
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
            // Set flags
            initializing = true;
            audioSettingsState = AudioSettingsState.NotChanged;

            // Detect devices
            devices = DeviceHelper.DetectOutputDevices();
            devicesDirectSound = devices.Where(x => x.DriverType == DriverType.DirectSound).ToList();
            devicesASIO = devices.Where(x => x.DriverType == DriverType.ASIO).ToList();
            devicesWASAPI = devices.Where(x => x.DriverType == DriverType.WASAPI).ToList();

            // Update combo box
            List<DriverComboBoxItem> drivers = new List<DriverComboBoxItem>();
            DriverComboBoxItem driverDirectSound = new DriverComboBoxItem() { DriverType = DriverType.DirectSound, Title = "DirectSound (default)" };
            DriverComboBoxItem driverASIO = new DriverComboBoxItem() { DriverType = DriverType.ASIO, Title = "ASIO" };
            DriverComboBoxItem driverWASAPI = new DriverComboBoxItem() { DriverType = DriverType.WASAPI, Title = "WASAPI (Vista/Win7 only) *EXPERIMENTAL*" };
            drivers.Add(driverDirectSound);
            drivers.Add(driverASIO);
            //drivers.Add(driverWASAPI);
            cboDrivers.DataSource = drivers;

            // Set default value
            cboDrivers.SelectedIndex = 0;

            // Set general settings lavels
            lblPeakFileDefaultDirectory.Text = "Use default directory (" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MPfm\\Peak Files\\)";

            btnGeneralSettings.Enabled = false;
            btnAudioSettings.Enabled = true;
            btnLibrarySettings.Enabled = true;

            panelGeneralSettings.Visible = true;
            panelAudioSettings.Visible = false;
            panelLibrarySettings.Visible = false;

            // Load configuration
            LoadAudioConfig();
            LoadGeneralConfig();

            // Refresh controls           
            RefreshFolders();
            RefreshAudioSettingsState();

            // Set flag
            initializing = false;            
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
            // Fix for FormClosing event called twice because of message box below
            if (!this.Visible)
            {
                return;
            }

            // Check if the application is exiting
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                // Exit immediately without saving
                e.Cancel = false;
                return;
            }

            // Save general settings
            if (!SaveGeneralConfig())
            {
                // Cancel
                e.Cancel = true;
                return;
            }

            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Get selected device
            Device device = (Device)cboOutputDevices.SelectedItem;

            // Check if the settings have changed
            if (audioSettingsState == AudioSettingsState.NotTested || audioSettingsState == AudioSettingsState.Tested)
            {
                string messageBoxText = string.Empty;
                string messageBoxTitle = string.Empty;

                // Check state
                if (audioSettingsState == AudioSettingsState.NotTested)
                {
                    messageBoxText = "The audio settings have changed but haven't been tested.\nDo you still wish to exit the Settings window?\n\nClick OK to continue without saving.\nClick Cancel to go back.";
                    messageBoxTitle = "Audio settings have changed";
                }
                else if (audioSettingsState == AudioSettingsState.Tested)
                {
                    messageBoxText = "The audio settings have been tested but haven't been saved.\nDo you still wish to exit the Settings window?\n\nClick OK to continue without saving.\nClick Cancel to go back.";
                    messageBoxTitle = "Audio settings have been tested";
                }

                // Display message box
                DialogResult dialogResult = MessageBox.Show(Main, messageBoxText, messageBoxTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    // Cancel 
                    e.Cancel = true;
                    return;
                }
            }

            // Check if the original audio settings need to be restored            
            if(!Main.Player.IsDeviceInitialized)
            {
                // Do not save settings; restore original configuration                
                Device originalDevice = null;
                if (Main.Config.Audio.DriverType == DriverType.DirectSound)
                {
                    // Loop through devices
                    for (int a = 0; a < devicesDirectSound.Count; a++)
                    {
                        // Check if the device matches
                        if (devicesDirectSound[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
                        {
                            // Set device
                            originalDevice = devicesDirectSound[a];
                            break;
                        }
                    }
                }
                else if (Main.Config.Audio.DriverType == DriverType.ASIO)
                {
                    // Loop through devices
                    for (int a = 0; a < devicesASIO.Count; a++)
                    {
                        // Check if the device matches
                        if (devicesASIO[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
                        {
                            // Set device
                            originalDevice = devicesASIO[a];
                            break;
                        }
                    }
                }
                else if (Main.Config.Audio.DriverType == DriverType.WASAPI)
                {
                    // Loop through devices
                    for (int a = 0; a < devicesWASAPI.Count; a++)
                    {
                        // Check if the device matches
                        if (devicesWASAPI[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
                        {
                            // Set device
                            originalDevice = devicesWASAPI[a];
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

            // Hide form            
            //Main.BringToFront();
            Main.Focus();
            //this.Close();

            e.Cancel = true;
            this.Hide();         
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
            numericBufferSize.Value = Main.Config.Audio.Mixer.BufferSize;
            numericUpdatePeriod.Value = Main.Config.Audio.Mixer.UpdatePeriod;
            trackBufferSize.Value = Main.Config.Audio.Mixer.BufferSize;
            trackUpdatePeriod.Value = Main.Config.Audio.Mixer.UpdatePeriod;

            // Set sample rate
            cboSampleRate.Items.Clear();
            cboSampleRate.Items.Add("44100");
            cboSampleRate.Items.Add("48000");
            cboSampleRate.Items.Add("96000");
            if (Main.Config.Audio.Mixer.Frequency == 44100)
            {                
                cboSampleRate.SelectedIndex = 0;
            }
            else if (Main.Config.Audio.Mixer.Frequency == 48000)
            {
                cboSampleRate.SelectedIndex = 1;
            }
            else if (Main.Config.Audio.Mixer.Frequency == 96000)
            {
                cboSampleRate.SelectedIndex = 2;
            }

            // Check driver
            if (Main.Config.Audio.DriverType == DriverType.DirectSound)
            {
                // Loop through devices
                for (int a = 0; a < devicesDirectSound.Count; a++)
                {
                    // Check if the device matches
                    if (devicesDirectSound[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
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
                for (int a = 0; a < devicesASIO.Count; a++)
                {
                    // Check if the device matches
                    if (devicesASIO[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
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
                for (int a = 0; a < devicesWASAPI.Count; a++)
                {
                    // Check if the device matches
                    if (devicesWASAPI[a].Name.ToUpper() == Main.Config.Audio.Device.Name.ToUpper())
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
            Main.Config.Audio.Mixer.BufferSize = (int)numericBufferSize.Value;
            Main.Config.Audio.Mixer.UpdatePeriod = (int)numericUpdatePeriod.Value;

            int frequency = 44100;
            int.TryParse(cboSampleRate.Text, out frequency);
            Main.Config.Audio.Mixer.Frequency = frequency;

            Main.Config.Save();
        }

        /// <summary>
        /// Loads the general settings from the configuration file.
        /// </summary>
        private void LoadGeneralConfig()
        {
            // Load user interface options
            bool? showTooltips = Main.Config.GetKeyValueGeneric<bool>("ShowTooltips");
            bool? hideTray = Main.Config.GetKeyValueGeneric<bool>("HideTray");
            bool? showTray = Main.Config.GetKeyValueGeneric<bool>("ShowTray");

            chkShowTooltips.Checked = (showTooltips.HasValue) ? showTooltips.Value : true;
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

            // Load update frequency
            int? positionUpdateFrequency = Main.Config.GetKeyValueGeneric<int>("PositionUpdateFrequency");
            int? outputMeterUpdateFrequency = Main.Config.GetKeyValueGeneric<int>("OutputMeterUpdateFrequency");

            numericPositionUpdateFrequency.Value = (positionUpdateFrequency.HasValue) ? positionUpdateFrequency.Value : 10;
            numericOutputMeterUpdateFrequency.Value = (outputMeterUpdateFrequency.HasValue) ? outputMeterUpdateFrequency.Value : 20;
            trackPositionUpdateFrequency.Value = (positionUpdateFrequency.HasValue) ? positionUpdateFrequency.Value : 10;
            trackOutputMeterUpdateFrequency.Value = (outputMeterUpdateFrequency.HasValue) ? outputMeterUpdateFrequency.Value : 20;

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

            // Save user interface options
            Main.Config.SetKeyValue<bool>("ShowTooltips", chkShowTooltips.Checked);
            Main.Config.SetKeyValue<bool>("HideTray", chkHideTray.Checked);
            Main.Config.SetKeyValue<bool>("ShowTray", chkShowTray.Checked);

            // Save update frequency
            Main.Config.SetKeyValue<int>("PositionUpdateFrequency", (int)numericPositionUpdateFrequency.Value);
            Main.Config.SetKeyValue<int>("OutputMeterUpdateFrequency", (int)numericOutputMeterUpdateFrequency.Value);

            // Save peak file options
            Main.Config.SetKeyValue<bool>("PeakFile_UseCustomDirectory", radioPeakFileCustomDirectory.Checked);
            Main.Config.SetKeyValue("PeakFile_CustomDirectory", txtPeakFileCustomDirectory.Text);
            Main.Config.SetKeyValue<bool>("PeakFile_DisplayWarning", chkPeakFileDisplayWarning.Checked);
            Main.Config.SetKeyValue<int>("PeakFile_DisplayWarningThreshold", (int)txtPeakFileDisplayWarningThreshold.Value);

            // Set current peak file folder
            Main.PeakFileFolderPath = peakFileFolderPath;

            // Refresh peak file warning
            Main.RefreshPeakFileDirectorySizeWarning();

            // Save configuration file
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
            List<Folder> folders = main.Library.Gateway.SelectFolders();

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

        /// <summary>
        /// Refreshes the Audio Settings tab state information (near the bottom of the tab).
        /// </summary>
        public void RefreshAudioSettingsState()
        {
            // Check state and update UI
            if (audioSettingsState == AudioSettingsState.NotChanged)
            {
                picAudioSettingsWarning.Image = global::MPfm.Properties.Resources.tick;
                lblAudioSettingsWarning.Text = "The audio settings haven't been changed.";
                btnTestSaveAudioSettings.Image = global::MPfm.Properties.Resources.sound;
                btnTestSaveAudioSettings.Text = "Test audio settings";
                btnTestSaveAudioSettings.Enabled = true;
            }
            else if (audioSettingsState == AudioSettingsState.NotTested)
            {
                picAudioSettingsWarning.Image = global::MPfm.Properties.Resources.error;
                lblAudioSettingsWarning.Text = "The audio settings have changed but haven't been tested.";
                btnTestSaveAudioSettings.Image = global::MPfm.Properties.Resources.sound;
                btnTestSaveAudioSettings.Text = "Test audio settings";
                btnTestSaveAudioSettings.Enabled = true;
            }
            else if (audioSettingsState == AudioSettingsState.Tested)
            {
                picAudioSettingsWarning.Image = global::MPfm.Properties.Resources.accept;
                lblAudioSettingsWarning.Text = "The new audio settings have been tested successfully. Click on 'Save audio settings' to continue.";
                btnTestSaveAudioSettings.Image = global::MPfm.Properties.Resources.disk;
                btnTestSaveAudioSettings.Text = "Save audio settings";
                btnTestSaveAudioSettings.Enabled = true;
            }
            else if (audioSettingsState == AudioSettingsState.Saved)
            {
                lblAudioSettingsWarning.Text = "The new audio settings has been applied and saved successfully.";
                picAudioSettingsWarning.Image = global::MPfm.Properties.Resources.accept;                
                btnTestSaveAudioSettings.Image = global::MPfm.Properties.Resources.disk;
                btnTestSaveAudioSettings.Text = "Save audio settings";
                btnTestSaveAudioSettings.Enabled = false;
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
                Folder folder = main.Library.Gateway.SelectFolderByPath(dialogAddFolder.SelectedPath);

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
                main.Library.Gateway.InsertFolder(dialogAddFolder.SelectedPath, recursive);
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
        /// Occurs when the user selects a new item in the Driver combo box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void cboDrivers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Check driver type
            if (driver.DriverType == DriverType.DirectSound)
            {
                // Show the appropriate panel
                panelAudioSettingsMixerDirectSound.Height = 76;
                panelAudioSettingsMixerASIO.Height = 20;
                //panelAudioSettingsMixerASIO.Visible = false;     

                // Set combo box data source
                cboOutputDevices.DataSource = devicesDirectSound;

                // Find default device
                Device defaultDevice = devicesDirectSound.FirstOrDefault(x => x.IsDefault);
            }
            else if (driver.DriverType == DriverType.ASIO)
            {
                // Show the appropriate panel
                panelAudioSettingsMixerASIO.Height = 76;
                panelAudioSettingsMixerDirectSound.Height = 20;
                //panelAudioSettingsMixerDirectSound.Visible = false;
                //panelAudioSettingsMixerASIO.Visible = true;                

                // Check the number of devices
                if (devicesASIO.Count == 0)
                {
                    // Show warning and reset selection to DirectSound
                    MessageBox.Show("No ASIO output devices were found on this system.", "No ASIO output devices detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboDrivers.SelectedIndex = 0;
                    return;
                }

                // Set combo box data source
                cboOutputDevices.DataSource = devicesASIO;

                // Find default device
                Device defaultDevice = devicesASIO.FirstOrDefault(x => x.IsDefault);

                // Refresh ASIO panel
                //RefreshASIOPanel();
            }
            else if (driver.DriverType == DriverType.WASAPI)
            {
                // Check the number of devices
                if (devicesWASAPI.Count == 0)
                {
                    // Show warning and reset selection to DirectSound
                    MessageBox.Show("No WASAPI output devices were found on this system.", "No WASAPI output devices detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboDrivers.SelectedIndex = 0;
                    return;
                }

                // Set combo box data source
                cboOutputDevices.DataSource = devicesWASAPI;

                // Find default device
                Device defaultDevice = devicesWASAPI.FirstOrDefault(x => x.IsDefault);
            }

            // Set state 
            if (!initializing)
            {
                audioSettingsState = AudioSettingsState.NotTested;
                RefreshAudioSettingsState();
            }
        }

        /// <summary>
        /// Occurs when the user selects a new item in the Output Device combo box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void cboOutputDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Check if driver is ASIO
            if (driver.DriverType == DriverType.ASIO)
            {
                // Refresh ASIO panel
                RefreshASIOPanel();
            }

            // Set state 
            if (!initializing)
            {
                audioSettingsState = AudioSettingsState.NotTested;
                RefreshAudioSettingsState();
            }
        }

        /// <summary>
        /// Occurs when the user changes the "Sample rate" combo box value.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void cboSampleRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set state 
            if (!initializing)
            {
                audioSettingsState = AudioSettingsState.NotTested;
                RefreshAudioSettingsState();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Test audio settings"/"Save audio settings" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnTestSaveAudioSettings_Click(object sender, EventArgs e)
        {
            // Check state
            if (audioSettingsState == AudioSettingsState.NotTested || audioSettingsState == AudioSettingsState.NotChanged)
            {
                // Get selected driver
                DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

                // Get selected device
                Device device = (Device)cboOutputDevices.SelectedItem;

                // Get sample rate
                int frequency = 44100;
                int.TryParse(cboSampleRate.Text, out frequency);                

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
                    //settingsTested = true;

                    // Check if device needs to be freed
                    if (Main.Player.IsDeviceInitialized)
                    {
                        // Free device                
                        Main.Player.FreeDevice();
                    }

                    // Disable output meter timer
                    Main.timerUpdateOutputMeter.Enabled = false;
                    Main.timerSongPosition.Enabled = false;

                    // Create test device
                    Tracing.Log("Creating test device...");
                    Main.Player.InitializeDevice(device, frequency);

                    // Set player properties
                    Main.Player.UpdatePeriod = (int)numericUpdatePeriod.Value;
                    Main.Player.BufferSize = (int)numericBufferSize.Value;

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
                    Main.timerSongPosition.Enabled = true;

                    // The test is successful           
                    Tracing.Log("The audio settings test is successful!");

                    // Set flags
                    //testSuccessful = true;
                    audioSettingsState = AudioSettingsState.Tested;
                    RefreshAudioSettingsState();
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
            else if (audioSettingsState == AudioSettingsState.Tested)
            {                
                // Save audio configuration
                SaveAudioConfig();

                // Set state and show message
                audioSettingsState = AudioSettingsState.Saved;
                RefreshAudioSettingsState();
                MessageBox.Show("The new audio settings has been applied and saved successfully.", "New audio settings saved successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
            for (int a = 0; a < devicesDirectSound.Count; a++)
            {
                // Is this the default device?
                if (devicesDirectSound[a].IsDefault)
                {
                    // Set default device and exit loop
                    cboOutputDevices.SelectedIndex = a;
                    break;
                }
            }

            // Set default values
            cboSampleRate.SelectedIndex = 0;
            numericBufferSize.Value = 1000;
            trackBufferSize.Value = 1000;
            numericUpdatePeriod.Value = 10;
            trackUpdatePeriod.Value = 10;

            // Set state
            audioSettingsState = AudioSettingsState.NotTested;
            RefreshAudioSettingsState();
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

        /// <summary>
        /// Occurs when the user clicks on the "Display MPfm in system tray" label.
        /// Triggers the related checkbox.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lblShowTray_Click(object sender, EventArgs e)
        {            
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
            if (chkHideTray.Enabled)
            {
                chkHideTray.Checked = !chkHideTray.Checked;
            }
        }

        #endregion

        /// <summary>
        /// Occurs when the user clicks on the "Show tooltips" label.
        /// Shows/hides tooltips from the application.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lblShowTooltips_Click(object sender, EventArgs e)
        {
            if (chkShowTooltips.Enabled)
            {
                chkShowTooltips.Checked = !chkShowTooltips.Checked;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Show tooltips" label.
        /// Shows/hides tooltips from the application.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void chkShowTooltips_CheckedChanged(object sender, EventArgs e)
        {
            Main.EnableTooltips(chkShowTooltips.Checked);
        }

        /// <summary>
        /// Occurs when the buffer size value changes.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void numericBufferSize_ValueChanged(object sender, EventArgs e)
        {
            // Set state 
            if (!initializing)
            {
                audioSettingsState = AudioSettingsState.NotTested;
                RefreshAudioSettingsState();
            }
        }

        /// <summary>
        /// Occurs when the update period value changes.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void numericUpdatePeriod_ValueChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the user changes the update period value using the track bar.
        /// </summary>
        private void trackUpdatePeriod_OnTrackBarValueChanged()
        {
            // Set value
            numericUpdatePeriod.Value = trackUpdatePeriod.Value;

            // Set state
            if (!initializing)
            {                
                audioSettingsState = AudioSettingsState.NotTested;
                RefreshAudioSettingsState();
            }
        }

        /// <summary>
        /// Occurs when the user changes the buffer size value using the track bar.
        /// </summary>
        private void trackBufferSize_OnTrackBarValueChanged()
        {
            // Set value
            numericBufferSize.Value = trackBufferSize.Value;

            // Set state
            if (!initializing)
            {
                audioSettingsState = AudioSettingsState.NotTested;
                RefreshAudioSettingsState();
            }
        }

        /// <summary>
        /// Occurs when the user changes the position update frequency value using the track bar.
        /// </summary>
        private void trackPositionUpdateFrequency_OnTrackBarValueChanged()
        {                        
            // Set value (only if different, to prevent triggering events)
            if (numericPositionUpdateFrequency.Value != trackPositionUpdateFrequency.Value)
            {
                numericPositionUpdateFrequency.Value = trackPositionUpdateFrequency.Value;
                Main.timerSongPosition.Interval = (int)numericPositionUpdateFrequency.Value;
            }
        }

        /// <summary>
        /// Occurs when the user changes the output meter update frequency value using the track bar.
        /// </summary>
        private void trackOutputMeterUpdateFrequency_OnTrackBarValueChanged()
        {
            // Set value (only if different, to prevent triggering events)
            if (numericOutputMeterUpdateFrequency.Value != trackOutputMeterUpdateFrequency.Value)
            {
                numericOutputMeterUpdateFrequency.Value = trackOutputMeterUpdateFrequency.Value;
                Main.timerUpdateOutputMeter.Interval = (int)trackOutputMeterUpdateFrequency.Value;
            }
        }

        /// <summary>
        /// Occurs when the user clicks or leaves the numeric control for the position update frequency.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void numericPositionUpdateFrequency_Leave(object sender, EventArgs e)
        {
            trackPositionUpdateFrequency.Value = (int)numericPositionUpdateFrequency.Value;
            Main.timerSongPosition.Interval = (int)numericPositionUpdateFrequency.Value;
        }

        /// <summary>
        /// Occurs when the user clicks or leaves the numeric control for the output meter update frequency.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void numericOutputMeterUpdateFrequency_Leave(object sender, EventArgs e)
        {
            trackOutputMeterUpdateFrequency.Value = (int)numericOutputMeterUpdateFrequency.Value;
            Main.timerUpdateOutputMeter.Interval = (int)numericOutputMeterUpdateFrequency.Value;
        }

        /// <summary>
        /// Occurs when the user clicks or leaves the numeric control for the audio buffer size.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void numericBufferSize_Leave(object sender, EventArgs e)
        {
            // Update only if value is different (to prevent triggering events)
            if (trackBufferSize.Value != (int)numericBufferSize.Value)
            {
                trackBufferSize.Value = (int)numericBufferSize.Value;
            }
        }

        /// <summary>
        /// Occurs when the user clicks or leaves the numeric control for the audio update period.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void numericUpdatePeriod_Leave(object sender, EventArgs e)
        {
            // Update only if value is different (to prevent triggering events)
            if (trackUpdatePeriod.Value != (int)numericUpdatePeriod.Value)
            {
                trackUpdatePeriod.Value = (int)numericUpdatePeriod.Value;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on one of peak file directory radio buttons.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void radioPeakFile_CheckedChanged(object sender, EventArgs e)
        {
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
                lblHideTray.Theme.TextGradient.Font.Color = Color.Black;
            }
            else
            {
                // Set label color
                lblHideTray.Theme.TextGradient.Font.Color = Color.FromArgb(80, 80, 80);
            }

            // Enable checkboxes depending on value
            txtPeakFileDisplayWarningThreshold.Enabled = chkPeakFileDisplayWarning.Checked;

            // Check check box value
            if (chkPeakFileDisplayWarning.Checked)
            {
                // Set label color                
                lblPeakFileDisplayWarningUnit.Theme.TextGradient.Font.Color = Color.Black;
            }
            else
            {
                // Set label color                
                lblPeakFileDisplayWarningUnit.Theme.TextGradient.Font.Color = Color.FromArgb(80, 80, 80);
            }

            lblHideTray.Refresh();
            lblPeakFileDisplayWarningUnit.Refresh();
        }

        /// <summary>
        /// Occurs when the user clicks on the "General" button.
        /// Switches to the "General Settings" tab.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnGeneralSettings_Click(object sender, EventArgs e)
        {
            btnGeneralSettings.Enabled = false;
            btnAudioSettings.Enabled = true;
            btnLibrarySettings.Enabled = true;

            panelGeneralSettings.Visible = true;
            panelAudioSettings.Visible = false;
            panelLibrarySettings.Visible = false;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Audio" button.
        /// Switches to the "Audio Settings" tab.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnAudioSettings_Click(object sender, EventArgs e)
        {
            btnGeneralSettings.Enabled = true;
            btnAudioSettings.Enabled = false;
            btnLibrarySettings.Enabled = true;

            panelAudioSettings.Visible = true;
            panelGeneralSettings.Visible = false;            
            panelLibrarySettings.Visible = false;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Library" button.
        /// Switches to the "Library Settings" tab.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnLibrarySettings_Click(object sender, EventArgs e)
        {
            btnGeneralSettings.Enabled = true;
            btnAudioSettings.Enabled = true;
            btnLibrarySettings.Enabled = false;

            panelLibrarySettings.Visible = true;
            panelGeneralSettings.Visible = false;
            panelAudioSettings.Visible = false;            
        }

        /// <summary>
        /// Occurs when the user clicks on the "Open ASIO Control Panel" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnOpenASIOControlPanel_Click(object sender, EventArgs e)
        {
            //// Get selected driver
            //DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            //// Get selected device
            //Device device = (Device)cboOutputDevices.SelectedItem;            

            //try
            //{
            //    // Get device info (force init if not done yet)
            //    ASIOInfo info = Base.GetASIOInfo(!Main.Player.IsPlaying, device.Id, 44100);

            //    // Open control panel
            //    Base.ASIO_ControlPanel();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error loading ASIO control panel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        /// <summary>
        /// Refreshes the information inside the ASIO panel.
        /// </summary>
        public void RefreshASIOPanel()
        {
            //// Get selected driver
            //DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            //// Get selected device
            //Device device = (Device)cboOutputDevices.SelectedItem;

            //try
            //{
            //    // Get device info (force init if not done yet)
            //    ASIOInfo info = Base.GetASIOInfo(!Main.Player.IsPlaying, device.Id, 44100);

            //    // Update UI
            //    lblASIOLatencyValue.Text = info.Latency.ToString();        
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error loading ASIO control panel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
    }

    /// <summary>
    /// Defines the different states of the Audio settings tab.
    /// </summary>
    public enum AudioSettingsState
    {
        /// <summary>
        /// The settings haven't been changed.
        /// </summary>
        NotChanged = 0,
        /// <summary>
        /// The settings have changed but haven't been tested.
        /// </summary>
        NotTested = 1,
        /// <summary>
        /// The settings have changed and have been tested.
        /// </summary>
        Tested = 2,
        /// <summary>
        /// The settings have been saved.
        /// </summary>
        Saved = 3
    }
}