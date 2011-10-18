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
using System.Text;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Sound;
using MPfm.WindowsControls;
using MPfm.Library;
using MPfm.Library.Data;

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
        private FMOD.OUTPUTTYPE outputType = FMOD.OUTPUTTYPE.UNKNOWN;
        private string outputDevice = string.Empty;
        
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
            // Refresh controls           
            cboOutputDevices.DataSource = Main.Player.SoundSystem.Drivers;
            cboDrivers.DataSource = Main.Player.SoundSystem.OutputTypes;
            RefreshFolders();

            // Load configuration
            LoadConfig();
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

            // Flow:

            // 1) detect if settings have changed
            // 2) ask user if he wants to save settings. display if settings have been tested or not.
            // 3) save or skip save

            // Get new settings from controls
            FMOD.OUTPUTTYPE newOutputType = (FMOD.OUTPUTTYPE)cboDrivers.SelectedValue;
            string newOutputDevice = (string)cboOutputDevices.SelectedItem;

            // Get settings from player
            FMOD.OUTPUTTYPE playerOutputType = Main.Player.SoundSystem.GetOutputType();
            string playerOutputDevice = Main.Player.SoundSystem.Drivers[Main.Player.SoundSystem.GetDriver()];

            // Did the user change the settings?
            if (newOutputType != playerOutputType || newOutputDevice != playerOutputDevice)
            {
                // Yeah, the user changed the settings.
                // Did the user test the new settings?
                if (!settingsTested)
                {
                    // Warn user
                    DialogResult dialogResult = MessageBox.Show(this, "Warning: The new audio settings haven't been tested. Saving an incompatible configuration WILL crash the application.\nTo reset the application configuration, you must edit the configuration file (MPfm.exe.config) and remove the appSettings node. This will display the First Run screen again.\n\nClick Yes to continue and save the untested configuration.\nClick No to exit the Settings window without saving the new configuration.\nClick Cancel to go back to the Settings window and test your new configuration.", "Warning: Audio settings untested", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

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
                // Did the user test the new settings and is trying to apply wrong configuration?
                else if (settingsTested && !testSuccessful)
                {
                    // Warn user
                    MessageBox.Show(this, "You cannot save audio settings that are compatible with your sound card!", "Error: Cannot save audio settings", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Cancel 
                    return;
                }
                // The user tested the new settings
                else if (settingsTested && testSuccessful)
                {
                    // The user tested the configuration and we're happy to simply ask him or her to save the new audio settings
                    if (MessageBox.Show(this, "Do you want to save the new audio settings?", "Save audio settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    {
                        // Set save settings flag
                        saveSettings = true;
                    }
                }
            }
            else
            {
                // No, the control values match the values they were before entering the Settings window.

                // However, did the user test a configuration that is different than the original?
                if (playerOutputType != outputType || playerOutputDevice != outputDevice)
                {
                    // Restore original configuration to player
                    Main.ResetPlayer(outputType, outputDevice, true, true);
                }
            }

            // Save settings if the user confirmed the save
            if (saveSettings)
            {
                // This can't be done in a background thread. Display an hourglass mouse cursor
                Cursor.Current = Cursors.WaitCursor;

                // Stop playback
                Main.Stop();

                // Save config
                SaveConfig();

                // Recreate player in main form
                Main.ResetPlayer();

                // Reset flags
                testSuccessful = false;
                settingsTested = false;
                settingsChanged = false;

                // Remove the hourglass
                Cursor.Current = Cursors.Default;
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
        /// Loads the form values based on configuration.
        /// </summary>
        private void LoadConfig()
        {
            // Load values into controls
            chkHideTray.Checked = Main.Config.HideTray;
            chkShowTray.Checked = Main.Config.ShowTray;
            cboOutputDevices.SelectedText = Main.Config.OutputDevice;
            cboDrivers.SelectedValue = Main.Config.Driver;            

            // Load values for history (if the user applies something and then wants to cancel)
            outputType = Main.Config.Driver;
            outputDevice = Main.Config.OutputDevice;
        }

        /// <summary>
        /// Saves the form values in the configuration file.
        /// </summary>
        private void SaveConfig()
        {
            Main.Config.OutputDevice = (string)cboOutputDevices.SelectedItem;
            Main.Config.Driver = (FMOD.OUTPUTTYPE)cboDrivers.SelectedValue;
            Main.Config.HideTray = chkHideTray.Checked;
            Main.Config.ShowTray = chkShowTray.Checked;
        }

        #endregion

        #region Refresh Methods

        /// <summary>
        /// Refreshes the list of folders.
        /// </summary>
        public void RefreshFolders()
        {
            // Get the list of folders from the database
            List<Folder> folders = DataAccess.SelectFolders();

            // Check if the list is null
            if (folders != null)
            {
                viewFolders.Items.Clear();

                foreach (Folder folder in folders)
                {
                    ListViewItem item = new ListViewItem(folder.FolderPath);
                    item.Tag = folder.FolderId;

                    /*if (folder.LastUpdate == DateTime.MinValue)
                    {
                        item.SubItems.Add("Never");
                    }
                    else
                    {
                        item.SubItems.Add(folder.LastUpdate.ToShortDateString());
                    }*/

                    item.SubItems.Add(folder.Recursive.ToString());

                    viewFolders.Items.Add(item);
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
                Folder folder = DataAccess.SelectFolderByPath(dialogAddFolder.SelectedPath);

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
                DataAccess.InsertFolder(dialogAddFolder.SelectedPath, recursive);
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
            if (viewFolders.SelectedItems.Count == 0)
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
            foreach (ListViewItem item in viewFolders.SelectedItems)
            {
                // Check if the tag is null
                if (item.Tag != null)
                {
                    // Get the folder id
                    Guid folderId = new Guid(item.Tag.ToString());

                    // Remove songs that match the path
                    if (removeSongsFromLibrary)
                    {
                        Main.Player.Library.RemoveSongsFromLibrary(item.SubItems[0].Text);
                    }

                    // Delete the folder from the list of configured folders
                    DataAccess.DeleteFolder(folderId);

                    // Remove item from list view
                    item.Remove();
                }
            }

            // Refresh cache
            Main.Player.Library.RefreshCache();
            Main.RefreshAll();
        }

        /// <summary>
        /// Occurs when the user tries to change the width of a column. This blocks the user from
        /// doing so.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewFolders_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
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
            if (MessageBox.Show(this, "Are you sure you wish to reset your library?\n\nThis will remove all songs from your library (they will not be deleted!)", "Reset Library", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                // Stop the song if one is playing
                Main.Player.Stop();

                // Reset library
                Main.Player.Library.ResetLibrary();

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
            settingsChanged = true;
            settingsTested = false;
            testSuccessful = false;            
        }

        /// <summary>
        /// Occurs when the user clicks on the Test audio configuration button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnTestSound_Click(object sender, EventArgs e)
        {
            try
            {               
                // Get driver
                FMOD.OUTPUTTYPE outputType = FMOD.OUTPUTTYPE.UNKNOWN;
                if (cboDrivers.SelectedItem != null && cboDrivers.SelectedValue != null)
                {
                    outputType = (FMOD.OUTPUTTYPE)cboDrivers.SelectedValue;
                }

                // Check if parameters are valid
                if (outputType == FMOD.OUTPUTTYPE.UNKNOWN || cboOutputDevices.SelectedIndex == -1)
                {
                    return;
                }

                // Warn user if system is already playing a song
                if (Main.Player.IsPlaying)
                {
                    if (MessageBox.Show(this, "Testing an audio file will stop the current playback. Click OK to continue or click Cancel to cancel the test.", "Interrupt playback", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        // The user cancelled
                        return;
                    }
                }

                // This can't be done in a background thread. Display an hourglass mouse cursor
                Cursor.Current = Cursors.WaitCursor;

                // Log 
                Tracing.Log("Starting audio settings test with the following settings: ");
                Tracing.Log("Output Type: " + outputType.ToString());
                Tracing.Log("Output Device: " + (string)cboOutputDevices.SelectedItem);      

                // Set flag
                settingsTested = true;

                // Stop playback
                Main.Stop();

                // Reset player
                Tracing.Log("Creating player...");
                Main.ResetPlayer(outputType, (string)cboOutputDevices.SelectedItem, true, true);

                // Reset cursor
                Cursor.Current = Cursors.Default;

                // Display the open file dialog (set filepath first)
                Tracing.Log("User selects a file.");
                dialogOpenFile.FileName = filePath;
                if (dialogOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }

                // Play sound file
                Tracing.Log("The audio file is playing...");
                Main.Player.PlayFile(dialogOpenFile.FileName);

                // Display info
                MessageBox.Show(this, "The sound system was initialized successfully.\nYou should now hear the file you have selected in the previous dialog.\nIf you do not hear a sound, your configuration might not working (unless you selected the \"No audio\" driver).\nIn that case, check the volume of your sound card mixer, or try changing the driver and/or output device.", "Sound system is working", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Stop playback
                Tracing.Log("User stops playback.");
                Main.Player.Stop();

                // Set flag
                testSuccessful = true;
                Tracing.Log("The audio settings test is successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error testing sound configuration!\nThis configuration will not work on your system.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error testing sound configuration!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracing.Log("The audio settings test has failed!");
                Tracing.Log("Exception message: " + ex.Message);
                Tracing.Log("Stack trace: " + ex.StackTrace);
                testSuccessful = false;
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

            if (chkShowTray.Checked)
            {
                chkHideTray.Enabled = true;
            }
            else
            {
                chkHideTray.Enabled = false;
                chkHideTray.Checked = false;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Hide tray" check box.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void chkHideTray_CheckedChanged(object sender, EventArgs e)
        {
            settingsChanged = true;
        }

        #endregion

        // VST Plugin test button
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    List<string> plugins = Main.Player.SoundSystem.GetVSTPluginList();

        //    MessageBox.Show(Main.Player.SoundSystem.GetOutput().ToString());
        //    Driver[] drivers = Main.Player.SoundSystem.GetDrivers();

        //    for (int a = 0; a < drivers.Length; a++)
        //    {
        //        MessageBox.Show(drivers[a].Name);
        //    }
        //}

    }

}