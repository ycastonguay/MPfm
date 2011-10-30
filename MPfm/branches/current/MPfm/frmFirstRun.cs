//
// frmFirstRun.cs: First Run window. This window is displayed to the user when he/she first starts the application.
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// First Run window. This window is displayed to the user when he/she first starts the application.
    /// The window helps the user to select a driver and an output device. The user is required to test the configuration
    /// using an audio file. The user can then access the application.
    /// </summary>
    public partial class frmFirstRun : MPfm.WindowsControls.Form
    {
        // Private variables
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
        /// Constructor for the First Run form. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmFirstRun(frmMain main)
        {
            m_main = main;
            InitializeComponent();
            
            try
            {
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
                cboDrivers.DataSource = drivers;

                // Set default value
                cboDrivers.SelectedIndex = 0;

                // Get list of drivers (output types)
                //cboDrivers.DataSource = MPfm.Sound.FMODWrapper.System.GetOutputTypes();
            }
            catch (Exception ex)
            {
                lblError.Text = "An error has occured while detecting output devices:";
                StringBuilder sbError = new StringBuilder();
                sbError.AppendLine("Message: " + ex.Message);
                sbError.AppendLine("Stack trace: \n" + ex.StackTrace);
                txtError.Text = sbError.ToString();
                panelError.Visible = true;
            }
        }

        #region Control Events

        #region Error Panel Events

        /// <summary>
        /// Occurs when the user clicks on the "Exit PMP" button on the Error panel.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnErrorExitPMP_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Send Email" button on the Error panel.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnErrorSendEmail_Click(object sender, EventArgs e)
        {
            Process.Start(@"mailto:yanick.castonguay@gmail.com");
        }

        /// <summary>
        /// Occurs when the user clicks on the "Copy to Clipboard" button on the Error panel.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnErrorCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, txtError.Text);
        }

        #endregion

        /// <summary>
        /// Occurs when the user clicks on the "Cancel" button. This exits the application.        
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnCancelWizard_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Next" button. 
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnNext_Click(object sender, EventArgs e)
        {     
            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Get selected device
            Device device = (Device)cboOutputDevices.SelectedItem;

            // Save configuration            
            Main.Config.OutputDevice = device.Name;
            Main.Config.Driver = driver.DriverType;

            // Close wizard
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Occurs when the user changes the driver using the combobox.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void cboDrivers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Check driver type
            if (driver.DriverType == DriverType.DirectSound)
            {
                // Set combo box data source
                cboOutputDevices.DataSource = m_devicesDirectSound;

                // Find default device
                Device defaultDevice = m_devicesDirectSound.FirstOrDefault(x => x.IsDefault);
                //if (defaultDevice != null)
                //{
                //    lblDefaultValue.Text = "[" + defaultDevice.Id.ToString() + "] " + defaultDevice.Name;
                //}
                //else
                //{
                //    lblDefaultValue.Text = "Unknown";
                //}
            }
            else if (driver.DriverType == DriverType.ASIO)
            {
                // Set combo box data source
                cboOutputDevices.DataSource = m_devicesASIO;

                // Find default device
                Device defaultDevice = m_devicesASIO.FirstOrDefault(x => x.IsDefault);
                //if (defaultDevice != null)
                //{
                //    lblDefaultValue.Text = "[" + defaultDevice.Id.ToString() + "] " + defaultDevice.Name;
                //}
                //else
                //{
                //    lblDefaultValue.Text = "Unknown";
                //}
            }
            else if (driver.DriverType == DriverType.WASAPI)
            {
                // Set combo box data source
                cboOutputDevices.DataSource = m_devicesWASAPI;

                // Find default device
                Device defaultDevice = m_devicesWASAPI.FirstOrDefault(x => x.IsDefault);
                //if (defaultDevice != null)
                //{
                //    lblDefaultValue.Text = "[" + defaultDevice.Id.ToString() + "] " + defaultDevice.Name;
                //}
                //else
                //{
                //    lblDefaultValue.Text = "Unknown";
                //}
            }

            // The test is successful, enable Next button
            btnNext.Enabled = false;
        }

        /// <summary>
        /// Occurs when the user changes the output device using the combobox.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void cboOutputDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            // The test is successful, enable Next button
            btnNext.Enabled = false;
        }

        /// <summary>
        /// Occrus when the user clicks on the Test Audio Settings button. Sets the
        /// player with the specified configuration. If successful, the user is presented
        /// with an open file dialog. The user is then shown the result of the audio test.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnTestAudioSettings_Click(object sender, EventArgs e)
        {
            // Get selected driver
            DriverComboBoxItem driver = (DriverComboBoxItem)cboDrivers.SelectedItem;

            // Get selected device
            Device device = (Device)cboOutputDevices.SelectedItem;

            try
            {
                // Display the open file dialog (set filepath first)
                Tracing.Log("User selects a file.");
                openFile.FileName = filePath;
                if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
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

                // Create the test device
                int flacPluginHandle = Base.LoadFlacPlugin();
                TestDevice testDevice = new TestDevice(driver.DriverType, device.Id, 44100);
                testDevice.Play(openFile.FileName);

                // Play sound file
                Tracing.Log("The audio file is playing...");

                // Display info
                MessageBox.Show(this, "The sound system was initialized successfully.\nYou should now hear the file you have selected in the previous dialog.\nIf you do not hear a sound, your configuration might not working (unless you selected the \"No audio\" driver).\nIn that case, check the volume of your sound card mixer, or try changing the driver and/or output device.", "Sound system is working", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Stop and dispose the device
                Tracing.Log("User stops playback.");                
                testDevice.Stop();
                testDevice.Dispose();
                Base.FreeFlacPlugin(flacPluginHandle);

                // The test is successful, enable Next button
                btnNext.Enabled = true;
                Tracing.Log("The audio settings test is successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error testing sound configuration!\nThis configuration will not work on your system.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error testing sound configuration!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracing.Log("The audio settings test has failed!");
                Tracing.Log("Exception message: " + ex.Message);
                Tracing.Log("Stack trace: " + ex.StackTrace);

                // The test is successful, enable Next button
                btnNext.Enabled = false;
            }

            Tracing.Log("End of audio settings test.");
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
