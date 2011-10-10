//
// frmEffects.cs: Effects window. This is where the user can configure a 18-band equalizer and VST plugins.
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
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using MPfm.Library;
using MPfm.Library.Data;
using MPfm.Sound;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Effects window. This is where the user can configure a 18-band equalizer and
    /// VST plugins.
    /// </summary>
    public partial class frmEffects : MPfm.WindowsControls.Form
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

        /// <summary>
        /// Constructor for Effects window. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmEffects(frmMain main)
        {
            InitializeComponent();
            m_main = main;

            RefreshEQPresets();
            LoadConfig();
        }

        #region Close Events

        /// <summary>
        /// Occurs when the form is about to close.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void frmEffects_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {                
                e.Cancel = true;
                this.Hide();
                Main.btnEffects.Checked = false;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the Close button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            SaveConfig();
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
            chkEQOn.Checked = Main.Config.EQOn;
            comboEQPreset.SelectedItem = Main.Config.EQPreset;
        }

        /// <summary>
        /// Saves the form values in the configuration file.
        /// </summary>
        private void SaveConfig()
        {
            Main.Config.EQOn = chkEQOn.Checked;

            if (comboEQPreset.SelectedItem != null)
            {
                Main.Config.EQPreset = comboEQPreset.SelectedItem.ToString();
            }
            else
            {
                Main.Config.EQPreset = string.Empty;
            }
        }

        #endregion

        #region Refresh Methods

        /// <summary>
        /// Refreshes the EQ presets.
        /// </summary>
        public void RefreshEQPresets()
        {
            // Clear combo
            comboEQPreset.Items.Clear();

            // Select presets
            List<Equalizer> eqs = DataAccess.SelectEqualizers();

            // For each EQ
            foreach (Equalizer eq in eqs)
            {
                // Add the preset
                comboEQPreset.Items.Add(eq.Name);
            }
        }

        /// <summary>
        /// Resets the equalizer and all the bands to 0dB.
        /// </summary>
        public void ResetEQ()
        {
            comboEQPreset.SelectedItem = null;
            txtEQPresetName.Text = "";
            fader1_2kHz.Value = 0;
            fader1_8kHz.Value = 0;
            fader10kHz.Value = 0;
            fader110Hz.Value = 0;
            fader14kHz.Value = 0;
            fader156Hz.Value = 0;
            fader2_5kHz.Value = 0;
            fader20kHz.Value = 0;
            fader220Hz.Value = 0;
            fader3_5kHz.Value = 0;
            fader311Hz.Value = 0;
            fader440Hz.Value = 0;
            fader55Hz.Value = 0;
            fader5kHz.Value = 0;
            fader622Hz.Value = 0;
            fader77Hz.Value = 0;
            fader7kHz.Value = 0;
            fader880Hz.Value = 0;
        }

        #endregion

        #region Other Events

        /// <summary>
        /// Occurs when the user clicks on the "Auto Level" button. Auto levels the
        /// equalizer values.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnAutoLevel_Click(object sender, EventArgs e)
        {
            // Declare variables
            int highestValue = -30;                     
            int value = 0;

            #region Find highest value

            // Find the highest and lowest value in the equalizer            
            value = fader55Hz.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader77Hz.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader110Hz.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader156Hz.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader220Hz.Value;           

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader311Hz.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader440Hz.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader622Hz.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader880Hz.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader1_2kHz.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader1_8kHz.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader2_5kHz.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader3_5kHz.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader5kHz.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader7kHz.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader10kHz.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader14kHz.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader20kHz.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            #endregion

            // Substract value for every fader
            fader55Hz.Value = fader55Hz.Value - highestValue;
            fader77Hz.Value = fader77Hz.Value - highestValue;
            fader110Hz.Value = fader110Hz.Value - highestValue;
            fader156Hz.Value = fader156Hz.Value - highestValue;
            fader220Hz.Value = fader220Hz.Value - highestValue;
            fader311Hz.Value = fader311Hz.Value - highestValue;
            fader440Hz.Value = fader440Hz.Value - highestValue;
            fader622Hz.Value = fader622Hz.Value - highestValue;
            fader880Hz.Value = fader880Hz.Value - highestValue;
            fader1_2kHz.Value = fader1_2kHz.Value - highestValue;
            fader1_8kHz.Value = fader1_8kHz.Value - highestValue;
            fader2_5kHz.Value = fader2_5kHz.Value - highestValue;
            fader3_5kHz.Value = fader3_5kHz.Value - highestValue;
            fader5kHz.Value = fader5kHz.Value - highestValue;
            fader7kHz.Value = fader7kHz.Value - highestValue;
            fader10kHz.Value = fader10kHz.Value - highestValue;
            fader14kHz.Value = fader14kHz.Value - highestValue;
            fader20kHz.Value = fader20kHz.Value - highestValue;

            //MessageBox.Show("highest: " + highestValue.ToString());
        }

        /// <summary>
        /// Occurs when the user clicks on the "Delete preset" button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnDeletePreset_Click(object sender, EventArgs e)
        {
            // Check if equalizer exists            
            Equalizer equalizerExists = DataAccess.SelectEqualizer(txtEQPresetName.Text);

            if (equalizerExists != null)
            {
                // Are you sure?
                if (MessageBox.Show("Are you sure you wish to delete this equalizer?", "Delete equalizer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {                    
                    DataAccess.DeleteEqualizer(new Guid(equalizerExists.EqualizerId));

                    RefreshEQPresets();
                    ResetEQ();
                }
            }
            
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save Preset" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSavePreset_Click(object sender, EventArgs e)
        {
            // Check if EQ has a name
            if (txtEQPresetName.Text.Length == 0)
            {
                MessageBox.Show("You must give a name to your equalizer preset.", "Cannot save equalizer preset", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Build DTO
            Equalizer eq = new Equalizer();
            eq.Gain55Hz = (double)fader55Hz.Value / 10;
            eq.Gain77Hz = (double)fader77Hz.Value / 10;
            eq.Gain110Hz = (double)fader110Hz.Value / 10;
            eq.Gain156Hz = (double)fader156Hz.Value / 10;
            eq.Gain220Hz = (double)fader220Hz.Value / 10;
            eq.Gain311Hz = (double)fader311Hz.Value / 10;
            eq.Gain440Hz = (double)fader440Hz.Value / 10;
            eq.Gain622Hz = (double)fader622Hz.Value / 10;
            eq.Gain880Hz = (double)fader880Hz.Value / 10;
            eq.Gain1_2kHz = (double)fader1_2kHz.Value / 10;
            eq.Gain1_8kHz = (double)fader1_8kHz.Value / 10;
            eq.Gain2_5kHz = (double)fader2_5kHz.Value / 10;
            eq.Gain3_5kHz = (double)fader3_5kHz.Value / 10;
            eq.Gain5kHz = (double)fader5kHz.Value / 10;
            eq.Gain7kHz = (double)fader7kHz.Value / 10;
            eq.Gain10kHz = (double)fader10kHz.Value / 10;
            eq.Gain14kHz = (double)fader14kHz.Value / 10;
            eq.Gain20kHz = (double)fader20kHz.Value / 10;

            eq.Name = txtEQPresetName.Text;

            // Check if equalizer exists            
            Equalizer equalizerExists = DataAccess.SelectEqualizer(txtEQPresetName.Text);

            if (equalizerExists == null)
            {
                DataAccess.InsertEqualizer(eq);
            }
            else
            {
                if (MessageBox.Show("Are you sure you wish to overwrite the " + equalizerExists.Name + " equalizer preset?", "Overwrite equalizer preset", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    // Update ID and EQ
                    eq.EqualizerId = equalizerExists.EqualizerId;
                    DataAccess.UpdateEqualizer(eq);
                }
            }

            // Refresh
            RefreshEQPresets();
            comboEQPreset.SelectedItem = eq.Name;
        }

        /// <summary>
        /// Occurs when the user clicks on the "EQ on" check box. Enables or
        /// disables the equalizer.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void chkEQOn_CheckedChanged(object sender, EventArgs e)
        {
            // Set equalizer
            Main.Player.IsEQOn = chkEQOn.Checked;    
        }        

        /// <summary>
        /// Occurs when the user clicks on the "Reset EQ" button. Resets the values
        /// of the equalizer to 0.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnResetEQ_Click(object sender, EventArgs e)
        {
            // Reset equalizer
            ResetEQ();
        }

        /// <summary>
        /// Occurs when the user changes the selected preset. Updates the EQ values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboEQPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Check if selection is valid
                if (comboEQPreset.SelectedItem != null && !String.IsNullOrEmpty(comboEQPreset.SelectedItem.ToString()))
                {
                    // Get equalizer                    
                    Equalizer equalizer = DataAccess.SelectEqualizer(comboEQPreset.SelectedItem.ToString());

                    // Set values
                    txtEQPresetName.Text = equalizer.Name;
                    fader55Hz.Value = (Int32)(equalizer.Gain55Hz * 10);
                    fader77Hz.Value = (Int32)(equalizer.Gain77Hz * 10);
                    fader110Hz.Value = (Int32)(equalizer.Gain110Hz * 10);
                    fader156Hz.Value = (Int32)(equalizer.Gain156Hz * 10);
                    fader220Hz.Value = (Int32)(equalizer.Gain220Hz * 10);
                    fader311Hz.Value = (Int32)(equalizer.Gain311Hz * 10);
                    fader440Hz.Value = (Int32)(equalizer.Gain440Hz * 10);
                    fader622Hz.Value = (Int32)(equalizer.Gain622Hz * 10);
                    fader880Hz.Value = (Int32)(equalizer.Gain880Hz * 10);
                    fader1_2kHz.Value = (Int32)(equalizer.Gain1_2kHz * 10);
                    fader1_8kHz.Value = (Int32)(equalizer.Gain1_8kHz * 10);
                    fader2_5kHz.Value = (Int32)(equalizer.Gain2_5kHz * 10);
                    fader3_5kHz.Value = (Int32)(equalizer.Gain3_5kHz * 10);
                    fader5kHz.Value = (Int32)(equalizer.Gain5kHz * 10);
                    fader7kHz.Value = (Int32)(equalizer.Gain7kHz * 10);
                    fader10kHz.Value = (Int32)(equalizer.Gain10kHz * 10);
                    fader14kHz.Value = (Int32)(equalizer.Gain14kHz * 10);
                    fader20kHz.Value = (Int32)(equalizer.Gain20kHz * 10);

                    // Set config                    
                    Main.Config.EQPreset = equalizer.Name;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Occurs when the user changes the value of a fader. This event is dynamic based on the event sender (fader).
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void fader_ValueChanged(object sender, EventArgs e)
        {
            // Get track bar
            VolumeFader fader = sender as VolumeFader;
            string hertz = fader.Tag.ToString();

            // Get label
            FieldInfo infoLabel = this.GetType().GetField("lblGain" + hertz, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            MPfm.WindowsControls.Label label = infoLabel.GetValue(this) as MPfm.WindowsControls.Label;

            // Default gain: 0 dB
            double gainParameter = 1;

            // Since the track resolution top is 30 and the min is -30, divide by 10 to get actual dB.
            float trackValue = ((float)fader.Value) / 10;

            // 10 power dB            
            //gainParameter = Math.Pow(10, trackValue / 10);
            gainParameter = Math.Pow(10, trackValue / 20);
            //gainParameter = Math.Pow(10, trackValue);

            // FMOD_DSP_PARAMEQ_GAIN
            // Frequency Gain. 0.05 to 3.0. Default = 1.0 (no gain)
            //
            // Fader value range: -30 to 30. 0 = no gain.
            //
            // My tests: Gain parameter of 2 is a LOT louder.
            
            //gainParameter = ratio;

            // Update dB display
            string strDB = "";
            if (trackValue > 0)
            {
                strDB = "+" + trackValue.ToString("0.0") + " dB";
            }
            else
            {
                strDB = trackValue.ToString("0.0") + " dB";
            }
            label.Text = gainParameter.ToString("0.00");
            //label.Text = strDB;

            try
            {
                // Set EQ gain
                PropertyInfo propEQ = Main.Player.GetType().GetProperty("ParamEQ" + hertz);

                if (propEQ != null)
                {
                    object obj = propEQ.GetValue(Main.Player, null);

                    if (obj != null)
                    {
                        ParamEQDSP dsp = (ParamEQDSP)obj;
                        dsp.SetGain((float)gainParameter);
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore for the moment
            }
        }

        #endregion
    }
}