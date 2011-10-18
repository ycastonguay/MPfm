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

        // Private variables
        private string filePath = string.Empty;

        /// <summary>
        /// Constructor for the First Run form. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmFirstRun(frmMain main)
        {
            m_main = main;
            InitializeComponent();

            List<string> listOutputDevices = new List<string>();
            try
            {
                // Get list of output devices without starting the sound system
                listOutputDevices = MPfm.Sound.FMODWrapper.System.GetOutputDevicesWithoutStartingSystem();

                // Update combo list of output devices
                cboOutputDevices.Items.Clear();
                foreach (string outputDevice in listOutputDevices)
                {
                    cboOutputDevices.Items.Add(outputDevice);
                }

                // By default, select the first one
                cboOutputDevices.SelectedIndex = 0;

                // Get list of drivers (output types)
                cboDrivers.DataSource = MPfm.Sound.FMODWrapper.System.GetOutputTypes();
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
            // Get driver
            FMOD.OUTPUTTYPE outputType = FMOD.OUTPUTTYPE.UNKNOWN;
            if (cboDrivers.SelectedItem != null && cboDrivers.SelectedValue != null)
            {
                outputType = (FMOD.OUTPUTTYPE)cboDrivers.SelectedValue;
            }

            // Save configuration            
            Main.Config.OutputDevice = cboOutputDevices.SelectedItem.ToString();
            Main.Config.Driver = outputType;

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
            // Get driver
            FMOD.OUTPUTTYPE outputType = FMOD.OUTPUTTYPE.UNKNOWN;
            if (cboDrivers.SelectedItem != null && cboDrivers.SelectedValue != null)
            {
                outputType = (FMOD.OUTPUTTYPE)cboDrivers.SelectedValue;
            }

            // Disable output device combo box if the user has chosen "No Sound"
            if (outputType == FMOD.OUTPUTTYPE.NOSOUND)
            {
                cboOutputDevices.Enabled = false;
            }
            else
            {
                cboOutputDevices.Enabled = true;
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

                // This can't be done in a background thread. Display an hourglass mouse cursor
                Cursor.Current = Cursors.WaitCursor;

                // Log 
                Tracing.Log("Starting audio settings test with the following settings: ");
                Tracing.Log("Output Type: " + outputType.ToString());
                Tracing.Log("Output Device: " + (string)cboOutputDevices.SelectedItem);               

                // Reset player
                Tracing.Log("Creating player...");
                Main.ResetPlayer(outputType, (string)cboOutputDevices.SelectedItem, true, false);

                // Reset cursor
                Cursor.Current = Cursors.Default;

                // Display the open file dialog (set filepath first)
                Tracing.Log("User selects a file.");
                openFile.FileName = filePath;
                if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }

                // Play sound file
                Tracing.Log("The audio file is playing...");
                Main.Player.PlayFile(openFile.FileName);

                // Display info
                MessageBox.Show(this, "The sound system was initialized successfully.\nYou should now hear the file you have selected in the previous dialog.\nIf you do not hear a sound, your configuration might not working (unless you selected the \"No audio\" driver).\nIn that case, check the volume of your sound card mixer, or try changing the driver and/or output device.", "Sound system is working", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Stop playback
                Tracing.Log("User stops playback.");
                Main.Player.Stop();

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
}
