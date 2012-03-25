//
// frmSettings.cs: This class is part of the PlaybackEngineV4 demo application.
//                 This is the settings form of the application.
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.Player.Demo
{
    /// <summary>
    /// Settings form.
    /// </summary>
    public partial class frmSettings : Form
    {
        // Private variables
        private frmMain main = null;
        private ConfigData configData = null;
        private bool settingsChanged = false;
        private bool isNewAudioDeviceTested = false;
        private bool isAudioDeviceDifferent = false;
        private List<Device> devices = null;
        private List<Device> devicesDirectSound = null;
        private List<Device> devicesASIO = null;
        private List<Device> devicesWASAPI = null;

        /// <summary>
        /// Constructor. Requires hook to the main form.
        /// </summary>
        /// <param name="main">Pointer to the main form</param>
        public frmSettings(frmMain main)
        {
            // Set default stuff
            this.main = main;
            InitializeComponent();

            // Set UI values
            txtBufferSize.Text = main.player.BufferSize.ToString("0000");
            txtUpdatePeriod.Text = main.player.UpdatePeriod.ToString("0000");
            txtUpdateThreads.Text = main.player.UpdateThreads.ToString("0");
            trackBufferSize.Value = main.player.BufferSize;
            trackUpdatePeriod.Value = main.player.UpdatePeriod;
            trackUpdateThreads.Value = main.player.UpdateThreads;

            // Detect devices
            devices = DeviceHelper.DetectOutputDevices();
            devicesDirectSound = devices.Where(x => x.DriverType == DriverType.DirectSound).ToList();
            devicesASIO = devices.Where(x => x.DriverType == DriverType.ASIO).ToList();
            devicesWASAPI = devices.Where(x => x.DriverType == DriverType.WASAPI).ToList();

            // Update combo box
            List<DriverComboBoxItem> drivers = new List<DriverComboBoxItem>();
            DriverComboBoxItem driverDirectSound = new DriverComboBoxItem() { DriverType = DriverType.DirectSound, Title = "DirectSound (default, recommended)" };
            DriverComboBoxItem driverASIO = new DriverComboBoxItem() { DriverType = DriverType.ASIO, Title = "ASIO (driver required, supports VST plugins)" };
            DriverComboBoxItem driverWASAPI = new DriverComboBoxItem() { DriverType = DriverType.WASAPI, Title = "WASAPI (Windows Vista/Windows 7 only)" };
            drivers.Add(driverDirectSound);
            drivers.Add(driverASIO);
            drivers.Add(driverWASAPI);
            comboDriver.DataSource = drivers;

            // Get configuration values
            configData = new ConfigData();

            // Set values
            txtBufferSize.Text = configData.bufferSize.ToString("0000");
            txtUpdatePeriod.Text = configData.updatePeriod.ToString("0000");
            txtUpdateThreads.Text = configData.updateThreads.ToString("0");
            trackBufferSize.Value = configData.bufferSize;
            trackUpdatePeriod.Value = configData.updatePeriod;
            trackUpdateThreads.Value = configData.updateThreads;

            // Check the driver type
            int deviceId = -1;
            int index = 0;
            if (configData.driverType.ToUpper() == "DIRECTSOUND")
            {
                // Select the driver
                comboDriver.SelectedIndex = 0;

                // Loop through devices to get the deviceId
                for (int a = 0; a < devicesDirectSound.Count; a++)
                {
                    // Match output device by name instead of deviceId (because deviceId can change!)
                    if (devicesDirectSound[a].Name.ToUpper() == configData.deviceName.ToUpper())
                    {
                        // Set deviceId
                        deviceId = devicesDirectSound[a].Id;
                        index = a;
                    }
                }
            }
            else if (configData.driverType.ToUpper() == "ASIO")
            {
                // Select the driver
                comboDriver.SelectedIndex = 1;

                // Loop through devices to get the deviceId
                for (int a = 0; a < devicesASIO.Count; a++)
                {
                    // Match output device by name instead of deviceId (because deviceId can change!)
                    if (devicesASIO[a].Name.ToUpper() == configData.deviceName.ToUpper())
                    {
                        // Set deviceId
                        deviceId = devicesASIO[a].Id;
                        index = a;
                    }
                }
            }
            else if (configData.driverType.ToUpper() == "WASAPI")
            {
                // Select the driver
                comboDriver.SelectedIndex = 2;

                // Loop through devices to get the deviceId
                for (int a = 0; a < devicesWASAPI.Count; a++)
                {
                    // Match output device by name instead of deviceId (because deviceId can change!)
                    if (devicesWASAPI[a].Name.ToUpper() == configData.deviceName.ToUpper())
                    {
                        // Set deviceId
                        deviceId = devicesWASAPI[a].Id;
                        index = a;
                    }
                }
            }
            
            // Set output device combo box value
            comboOutputDevice.SelectedIndex = index;           

            // Set flags
            isNewAudioDeviceTested = false;
            isAudioDeviceDifferent = false;
            settingsChanged = false;
        }

        #region Button Events
        
        /// <summary>
        /// Occurs when the user clicks on the Close button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Check if the audio device has changed
            if (isAudioDeviceDifferent && !isNewAudioDeviceTested)
            {
                // Warn user
                if (MessageBox.Show("Warning: The new sound card configuration has not been tested.\nIt is strongly recommended to test the audio device before applying the new configuration.\n\nDo you wish to apply this configuration without testing?", "New audio device hasn't been tested", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    // The user has cancelled
                    return;
                }
            }

            // Check if the settings changed
            if (settingsChanged)
            {
                // Ask user for saving settings
                if (MessageBox.Show("Do you wish to save your settings?", "Save settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    // Get selected device and selected driver
                    DriverComboBoxItem driver = (DriverComboBoxItem)comboDriver.SelectedItem;
                    Device device = (Device)comboOutputDevice.SelectedItem;

                    // Get values before saving to configuration file
                    int.TryParse(txtBufferSize.Text, out configData.bufferSize);
                    int.TryParse(txtUpdatePeriod.Text, out configData.updatePeriod);
                    int.TryParse(txtUpdateThreads.Text, out configData.updateThreads);
                    configData.driverType = driver.DriverType.ToString();
                    configData.deviceName = device.Name;
                    configData.deviceId = device.Id;

                    // Save configuration values
                    configData.Save();
                    //Config.Save("BufferSize", bufferSize.ToString());
                    //Config.Save("UpdatePeriod", updatePeriod.ToString());                
                    //Config.Save("UpdateThreads", updateThreads.ToString());
                    //Config.Save("DriverType", device.DriverType.ToString());                
                    //Config.Save("DeviceId", device.Id.ToString());
                    //Config.Save("DeviceName", device.Name);

                    // Check if the device needs to be initialized
                    if (!main.player.IsDeviceInitialized)
                    {
                        // Initialize device
                        main.player.InitializeDevice(device, 44100);
                    }
                }
            }

            // Check if the device needs to be initialized
            if (!main.player.IsDeviceInitialized)
            {
                // Set player default device
                main.player.InitializeDevice();
            }

            // Close settings window
            this.Close();
        }

        /// <summary>
        /// Occurs when the user clicks on the Reset button.
        /// This resets the settings to the default values.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            trackBufferSize.Value = 100;
            trackUpdatePeriod.Value = 10;
            trackUpdateThreads.Value = 1;

            main.player.BufferSize = trackBufferSize.Value;
            main.player.UpdatePeriod = trackUpdatePeriod.Value;
            main.player.UpdateThreads = trackUpdateThreads.Value;

            txtBufferSize.Text = trackBufferSize.Value.ToString("0000");
            txtUpdatePeriod.Text = trackUpdatePeriod.Value.ToString("0000");
            txtUpdateThreads.Text = trackUpdateThreads.Value.ToString("0");
        }

        /// <summary>
        /// Occurs when the user clicks on the Save button.
        /// This saves the settings in the configuration file.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Occurs when the user clicks on the Test audio button.
        /// The user must select an audio file to play to test the device.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnTestAudio_Click(object sender, EventArgs e)
        {
            // Check if the player is playing
            if (main.player.IsPlaying)
            {
                // Warn the user this will stop the playback!
                if (MessageBox.Show("Warning: The player is currently playing a song. This will interrumpt the playback. Are you sure?", "Interrumpt playback", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)comboDriver.SelectedItem;

            // Get selected device
            Device device = (Device)comboOutputDevice.SelectedItem;

            // Check if the player exists
            if (main.player.IsPlaying)
            {               
                // Stop playback
                main.player.Stop();
            }

            // Free device
            main.player.FreeDevice();

            // Show open file dialog
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    // Create the test device
                    TestDevice testDevice = new TestDevice(driver.DriverType, device.Id, 44100);
                    testDevice.Play(openFile.FileName);

                    // Display message during playback
                    MessageBox.Show("If you hear an audio file playing, it means that the audio test is successful.\nClick on OK to stop the playback.", "Audio test successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Stop and dispose the device
                    testDevice.Stop();
                    testDevice.Dispose();

                    // Set flags
                    isNewAudioDeviceTested = true;
                }
                catch (Exception ex)
                {
                    // Display error
                    MessageBox.Show("An error occured while testing the audio device.\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error playing audio file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Trackbar Events
        
        /// <summary>
        /// Occurs when the user changes the buffer size value using the trackbar.
        /// Updates the player buffer size value, even when playing a song.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackBufferSize_Scroll(object sender, EventArgs e)
        {
            // Set flags
            settingsChanged = true;

            // Set value and update player
            txtBufferSize.Text = trackBufferSize.Value.ToString("0000");
            main.player.BufferSize = trackBufferSize.Value;
        }

        /// <summary>
        /// Occurs when the user changes the update period value using the trackbar.
        /// Updates the player buffer size value, even when playing a song.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackUpdatePeriod_Scroll(object sender, EventArgs e)
        {
            // Set flags
            settingsChanged = true;

            // Set value and update player
            txtUpdatePeriod.Text = trackUpdatePeriod.Value.ToString("0000");
            main.player.UpdatePeriod = trackUpdatePeriod.Value;
        }

        /// <summary>
        /// Occurs when the user changes the update threads value using the trackbar.
        /// Updates the player buffer size value, even when playing a song.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackUpdateThreads_Scroll(object sender, EventArgs e)
        {
            // Set flags
            settingsChanged = true;

            // Set value and update player
            txtUpdateThreads.Text = trackUpdateThreads.Value.ToString("0");
            main.player.UpdateThreads = trackUpdateThreads.Value;
        }

        #endregion

        #region Combo Box Events

        /// <summary>
        /// Occurs when the user changes the driver combo box value.        
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void comboDriver_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset flags
            isNewAudioDeviceTested = false;
            isAudioDeviceDifferent = true;            
            settingsChanged = true;

            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)comboDriver.SelectedItem;

            // Check driver type
            if (driver.DriverType == DriverType.DirectSound)
            {
                // Set combo box data source
                comboOutputDevice.DataSource = devicesDirectSound;

                // Find default device
                Device defaultDevice = devicesDirectSound.FirstOrDefault(x => x.IsDefault);
                if (defaultDevice != null)
                {
                    lblDefaultValue.Text = "[" + defaultDevice.Id.ToString() + "] " + defaultDevice.Name;
                }
                else
                {
                    lblDefaultValue.Text = "Unknown";
                }
            }
            else if (driver.DriverType == DriverType.ASIO)
            {
                // Set combo box data source
                comboOutputDevice.DataSource = devicesASIO;

                // Find default device
                Device defaultDevice = devicesASIO.FirstOrDefault(x => x.IsDefault);
                if (defaultDevice != null)
                {
                    lblDefaultValue.Text = "[" + defaultDevice.Id.ToString() + "] " + defaultDevice.Name;
                }
                else
                {
                    lblDefaultValue.Text = "Unknown";
                }
            }
            else if (driver.DriverType == DriverType.WASAPI)
            {
                // Set combo box data source
                comboOutputDevice.DataSource = devicesWASAPI;

                // Find default device
                Device defaultDevice = devicesWASAPI.FirstOrDefault(x => x.IsDefault);
                if (defaultDevice != null)
                {
                    lblDefaultValue.Text = "[" + defaultDevice.Id.ToString() + "] " + defaultDevice.Name;
                }
                else
                {
                    lblDefaultValue.Text = "Unknown";
                }
            }
        }

        /// <summary>
        /// Occurs when the user changes the output device combo box value.        
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void comboOutputDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set flags
            settingsChanged = true;
            isAudioDeviceDifferent = true;
        }

        #endregion
    }

    /// <summary>
    /// This class represents a driver combo box item.
    /// </summary>
    public class DriverComboBoxItem
    {
        public DriverType DriverType { get; set; }
        public string Title { get; set; }
    }
}
