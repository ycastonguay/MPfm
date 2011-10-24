//
// frmSettings.cs: This class is part of the PlaybackEngineV4 demo application.
//                 This is the settings form of the application.
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Sound.BassNetWrapper;

namespace PlaybackEngineV4
{
    /// <summary>
    /// Settings form.
    /// </summary>
    public partial class frmSettings : Form
    {
        // Private variables
        private frmMain m_main = null;
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
            m_devices = DeviceHelper.DetectDevices();
            m_devicesDirectSound = m_devices.Where(x => x.DriverType == DriverType.DirectSound).ToList();
            m_devicesASIO = m_devices.Where(x => x.DriverType == DriverType.ASIO).ToList();
            m_devicesWASAPI = m_devices.Where(x => x.DriverType == DriverType.WASAPI).ToList();

            // Update combo box
            List<DriverComboBoxItem> drivers = new List<DriverComboBoxItem>();
            drivers.Add(new DriverComboBoxItem() { DriverType = DriverType.DirectSound, Title = "DirectSound (default, recommended)" });
            drivers.Add(new DriverComboBoxItem() { DriverType = DriverType.ASIO, Title = "ASIO (driver required, supports VST plugins)" });
            drivers.Add(new DriverComboBoxItem() { DriverType = DriverType.WASAPI, Title = "WASAPI (Windows Vista/Windows 7 only)" });
            comboDriver.DataSource = drivers;            
        }

        #region Button Events
        
        /// <summary>
        /// Occurs when the user clicks on the Close button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
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
            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)comboDriver.SelectedItem;

            // Get selected device
            Device device = (Device)comboOutputDevice.SelectedItem;

            // Check if the player exists
            if (m_main.player != null)
            {
                // Dispose player (cannot have two BASS.NET engines running on the same device)
                m_main.player.Stop();
                m_main.player.Dispose();
                m_main.player = null;
            }

            // Show open file dialog
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    // Create the test device
                    TestDevice testDevice = new TestDevice(driver.DriverType, device.Id);
                    testDevice.Play(openFile.FileName);

                    // Display message during playback
                    MessageBox.Show("If you hear an audio file playing, it means that the audio test is successful.\nClick on OK to stop the playback.", "Audio test successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Dispose it
                    testDevice.Stop();
                    testDevice.Dispose();
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
            // Set value and update player
            txtUpdateThreads.Text = trackUpdateThreads.Value.ToString("0");
            m_main.player.UpdateThreads = trackUpdateThreads.Value;
        }

        #endregion

        private void comboDriver_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private void comboOutputDevice_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }

    public class DriverComboBoxItem
    {
        public DriverType DriverType { get; set; }
        public string Title { get; set; }
    }
}
