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
        private frmMain m_main = null;
        private ConfigData m_configData = null;
        private bool m_settingsChanged = false;
        private bool m_isNewAudioDeviceTested = false;
        private bool m_isAudioDeviceDifferent = false;
        private List<Device> m_devices = null;
        private List<Device> m_devicesDirectSound = null;
        private List<Device> m_devicesASIO = null;
        private List<Device> m_devicesWASAPI = null;

        /// <summary>
        /// Constructor. Requires hook to the main form.
        /// </summary>
        /// <param name="main">Pointer to the main form</param>
        public frmSettings(frmMain main)
        {
            // Set default stuff
            m_main = main;
            InitializeComponent();

            // Set UI values
            txtBufferSize.Text = main.player.BufferSize.ToString("0000");
            txtUpdatePeriod.Text = main.player.UpdatePeriod.ToString("0000");
            txtUpdateThreads.Text = main.player.UpdateThreads.ToString("0");
            trackBufferSize.Value = main.player.BufferSize;
            trackUpdatePeriod.Value = main.player.UpdatePeriod;
            trackUpdateThreads.Value = main.player.UpdateThreads;

            // Detect devices
            m_devices = DeviceHelper.DetectOutputDevices();
            m_devicesDirectSound = m_devices.Where(x => x.DriverType == DriverType.DirectSound).ToList();
            m_devicesASIO = m_devices.Where(x => x.DriverType == DriverType.ASIO).ToList();
            m_devicesWASAPI = m_devices.Where(x => x.DriverType == DriverType.WASAPI).ToList();

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
            m_configData = new ConfigData();

            // Set values
            txtBufferSize.Text = m_configData.bufferSize.ToString("0000");
            txtUpdatePeriod.Text = m_configData.updatePeriod.ToString("0000");
            txtUpdateThreads.Text = m_configData.updateThreads.ToString("0");
            trackBufferSize.Value = m_configData.bufferSize;
            trackUpdatePeriod.Value = m_configData.updatePeriod;
            trackUpdateThreads.Value = m_configData.updateThreads;

            // Check the driver type
            int deviceId = -1;
            int index = 0;
            if (m_configData.driverType.ToUpper() == "DIRECTSOUND")
            {
                // Select the driver
                comboDriver.SelectedIndex = 0;

                // Loop through devices to get the deviceId
                for (int a = 0; a < m_devicesDirectSound.Count; a++)
                {
                    // Match output device by name instead of deviceId (because deviceId can change!)
                    if (m_devicesDirectSound[a].Name.ToUpper() == m_configData.deviceName.ToUpper())
                    {
                        // Set deviceId
                        deviceId = m_devicesDirectSound[a].Id;
                        index = a;
                    }
                }
            }
            else if (m_configData.driverType.ToUpper() == "ASIO")
            {
                // Select the driver
                comboDriver.SelectedIndex = 1;

                // Loop through devices to get the deviceId
                for (int a = 0; a < m_devicesASIO.Count; a++)
                {
                    // Match output device by name instead of deviceId (because deviceId can change!)
                    if (m_devicesASIO[a].Name.ToUpper() == m_configData.deviceName.ToUpper())
                    {
                        // Set deviceId
                        deviceId = m_devicesASIO[a].Id;
                        index = a;
                    }
                }
            }
            else if (m_configData.driverType.ToUpper() == "WASAPI")
            {
                // Select the driver
                comboDriver.SelectedIndex = 2;

                // Loop through devices to get the deviceId
                for (int a = 0; a < m_devicesWASAPI.Count; a++)
                {
                    // Match output device by name instead of deviceId (because deviceId can change!)
                    if (m_devicesWASAPI[a].Name.ToUpper() == m_configData.deviceName.ToUpper())
                    {
                        // Set deviceId
                        deviceId = m_devicesWASAPI[a].Id;
                        index = a;
                    }
                }
            }
            
            // Set output device combo box value
            comboOutputDevice.SelectedIndex = index;           

            // Set flags
            m_isNewAudioDeviceTested = false;
            m_isAudioDeviceDifferent = false;
            m_settingsChanged = false;
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
            if (m_isAudioDeviceDifferent && !m_isNewAudioDeviceTested)
            {
                // Warn user
                if (MessageBox.Show("Warning: The new sound card configuration has not been tested.\nIt is strongly recommended to test the audio device before applying the new configuration.\n\nDo you wish to apply this configuration without testing?", "New audio device hasn't been tested", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    // The user has cancelled
                    return;
                }
            }

            // Check if the settings changed
            if (m_settingsChanged)
            {
                // Ask user for saving settings
                if (MessageBox.Show("Do you wish to save your settings?", "Save settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    // Get selected device and selected driver
                    DriverComboBoxItem driver = (DriverComboBoxItem)comboDriver.SelectedItem;
                    Device device = (Device)comboOutputDevice.SelectedItem;

                    // Get values before saving to configuration file
                    int.TryParse(txtBufferSize.Text, out m_configData.bufferSize);
                    int.TryParse(txtUpdatePeriod.Text, out m_configData.updatePeriod);
                    int.TryParse(txtUpdateThreads.Text, out m_configData.updateThreads);
                    m_configData.driverType = driver.DriverType.ToString();
                    m_configData.deviceName = device.Name;
                    m_configData.deviceId = device.Id;

                    // Save configuration values
                    m_configData.Save();
                    //Config.Save("BufferSize", bufferSize.ToString());
                    //Config.Save("UpdatePeriod", updatePeriod.ToString());                
                    //Config.Save("UpdateThreads", updateThreads.ToString());
                    //Config.Save("DriverType", device.DriverType.ToString());                
                    //Config.Save("DeviceId", device.Id.ToString());
                    //Config.Save("DeviceName", device.Name);

                    // Check if the device needs to be initialized
                    if (!m_main.player.IsDeviceInitialized)
                    {
                        // Initialize device
                        m_main.player.InitializeDevice(device, 44100);
                    }
                }
            }

            // Check if the device needs to be initialized
            if (!m_main.player.IsDeviceInitialized)
            {
                // Set player default device
                m_main.player.InitializeDevice();
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

            m_main.player.BufferSize = trackBufferSize.Value;
            m_main.player.UpdatePeriod = trackUpdatePeriod.Value;
            m_main.player.UpdateThreads = trackUpdateThreads.Value;

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
            if (m_main.player.IsPlaying)
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
            if (m_main.player.IsPlaying)
            {               
                // Stop playback
                m_main.player.Stop();
            }

            // Free device
            m_main.player.FreeDevice();

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
                    m_isNewAudioDeviceTested = true;
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
            m_settingsChanged = true;

            // Set value and update player
            txtBufferSize.Text = trackBufferSize.Value.ToString("0000");
            m_main.player.BufferSize = trackBufferSize.Value;
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
            m_settingsChanged = true;

            // Set value and update player
            txtUpdatePeriod.Text = trackUpdatePeriod.Value.ToString("0000");
            m_main.player.UpdatePeriod = trackUpdatePeriod.Value;
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
            m_settingsChanged = true;

            // Set value and update player
            txtUpdateThreads.Text = trackUpdateThreads.Value.ToString("0");
            m_main.player.UpdateThreads = trackUpdateThreads.Value;
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
            m_isNewAudioDeviceTested = false;
            m_isAudioDeviceDifferent = true;            
            m_settingsChanged = true;

            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)comboDriver.SelectedItem;

            // Check driver type
            if (driver.DriverType == DriverType.DirectSound)
            {
                // Set combo box data source
                comboOutputDevice.DataSource = m_devicesDirectSound;

                // Find default device
                Device defaultDevice = m_devicesDirectSound.FirstOrDefault(x => x.IsDefault);
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
                comboOutputDevice.DataSource = m_devicesASIO;

                // Find default device
                Device defaultDevice = m_devicesASIO.FirstOrDefault(x => x.IsDefault);
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
                comboOutputDevice.DataSource = m_devicesWASAPI;

                // Find default device
                Device defaultDevice = m_devicesWASAPI.FirstOrDefault(x => x.IsDefault);
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
            m_settingsChanged = true;
            m_isAudioDeviceDifferent = true;
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
