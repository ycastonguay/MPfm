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
using MPfm.Player.PlayerV4;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
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
            //LoadConfig();

            // Set UI values
            chkEQOn.Checked = m_main.PlayerV4.IsEQEnabled;

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
            //List<Equalizer> eqs = DataAccess.SelectEqualizers();
            List<EqualizerDTO> eqs = m_main.Library.Gateway.SelectEqualizers();

            // For each EQ
            foreach (EqualizerDTO eq in eqs)
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
            fader0.Value = 0;
            fader1.Value = 0;
            fader2.Value = 0;
            fader3.Value = 0;
            fader4.Value = 0;
            fader5.Value = 0;
            fader6.Value = 0;
            fader7.Value = 0;
            fader8.Value = 0;
            fader9.Value = 0;
            fader10.Value = 0;
            fader11.Value = 0;
            fader12.Value = 0;
            fader13.Value = 0;
            fader14.Value = 0;
            fader15.Value = 0;
            fader16.Value = 0;
            fader17.Value = 0;            
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
            value = fader0.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader1.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader2.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader3.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader4.Value;           

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader5.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader6.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader7.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader8.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader9.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader10.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader11.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader12.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader13.Value;            

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader14.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader15.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader16.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            value = fader17.Value;

            // Check for high
            if (value > highestValue)
                highestValue = value;

            #endregion

            // Substract value for every fader
            fader0.Value = fader0.Value - highestValue;
            fader1.Value = fader1.Value - highestValue;
            fader2.Value = fader2.Value - highestValue;
            fader3.Value = fader3.Value - highestValue;
            fader4.Value = fader4.Value - highestValue;
            fader5.Value = fader5.Value - highestValue;
            fader6.Value = fader6.Value - highestValue;
            fader7.Value = fader7.Value - highestValue;
            fader8.Value = fader8.Value - highestValue;
            fader9.Value = fader9.Value - highestValue;
            fader10.Value = fader10.Value - highestValue;
            fader11.Value = fader11.Value - highestValue;
            fader12.Value = fader12.Value - highestValue;
            fader13.Value = fader13.Value - highestValue;
            fader14.Value = fader14.Value - highestValue;
            fader15.Value = fader15.Value - highestValue;
            fader16.Value = fader16.Value - highestValue;
            fader17.Value = fader17.Value - highestValue;

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
            //Equalizer equalizerExists = DataAccess.SelectEqualizer(txtEQPresetName.Text);
            EqualizerDTO equalizerExists = m_main.Library.Gateway.SelectEqualizer(txtEQPresetName.Text);

            if (equalizerExists != null)
            {
                // Are you sure?
                if (MessageBox.Show("Are you sure you wish to delete this equalizer?", "Delete equalizer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {                    
                    //DataAccess.DeleteEqualizer(new Guid(equalizerExists.EqualizerId));
                    m_main.Library.Gateway.DeleteEqualizer(equalizerExists.EqualizerId);

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
            EqualizerDTO eq = new EqualizerDTO();
            eq.Gain55Hz = (float)fader0.Value / 10;
            eq.Gain77Hz = (float)fader1.Value / 10;
            eq.Gain110Hz = (float)fader2.Value / 10;
            eq.Gain156Hz = (float)fader3.Value / 10;
            eq.Gain220Hz = (float)fader4.Value / 10;
            eq.Gain311Hz = (float)fader5.Value / 10;
            eq.Gain440Hz = (float)fader6.Value / 10;
            eq.Gain622Hz = (float)fader7.Value / 10;
            eq.Gain880Hz = (float)fader8.Value / 10;
            eq.Gain1_2kHz = (float)fader9.Value / 10;
            eq.Gain1_8kHz = (float)fader10.Value / 10;
            eq.Gain2_5kHz = (float)fader11.Value / 10;
            eq.Gain3_5kHz = (float)fader12.Value / 10;
            eq.Gain5kHz = (float)fader13.Value / 10;
            eq.Gain7kHz = (float)fader14.Value / 10;
            eq.Gain10kHz = (float)fader15.Value / 10;
            eq.Gain14kHz = (float)fader16.Value / 10;
            eq.Gain20kHz = (float)fader17.Value / 10;

            eq.Name = txtEQPresetName.Text;

            // Check if equalizer exists            
            //EqualizerDTO equalizerExists = DataAccess.SelectEqualizer(txtEQPresetName.Text);
            EqualizerDTO equalizerExists = m_main.Library.Gateway.SelectEqualizer(txtEQPresetName.Text);

            if (equalizerExists == null)
            {
                //DataAccess.InsertEqualizer(eq);
                m_main.Library.Gateway.InsertEqualizer(eq);
            }
            else
            {
                if (MessageBox.Show("Are you sure you wish to overwrite the " + equalizerExists.Name + " equalizer preset?", "Overwrite equalizer preset", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    // Update ID and EQ
                    eq.EqualizerId = equalizerExists.EqualizerId;
                    //DataAccess.UpdateEqualizer(eq);
                    m_main.Library.Gateway.UpdateEqualizer(eq);
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
            // Bypass EQ
            m_main.PlayerV4.BypassEQ();

            //// Set equalizer
            //if (m_main.PlayerV4.IsEQEnabled)
            //{
            //    // Remove EQ
            //    m_main.PlayerV4.RemoveEQ();
            //}
            //else
            //{
            //    // Add EQ
            //    EQPreset preset = GetEQPresetFromCurrentValues();
            //    m_main.PlayerV4.AddEQ(preset);
            //}
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
                    //Equalizer equalizer = DataAccess.SelectEqualizer(comboEQPreset.SelectedItem.ToString());
                    EqualizerDTO equalizer = m_main.Library.Gateway.SelectEqualizer(comboEQPreset.SelectedItem.ToString());

                    // Set values
                    txtEQPresetName.Text = equalizer.Name;
                    fader0.Value = (Int32)(equalizer.Gain55Hz * 10);
                    fader1.Value = (Int32)(equalizer.Gain77Hz * 10);
                    fader2.Value = (Int32)(equalizer.Gain110Hz * 10);
                    fader3.Value = (Int32)(equalizer.Gain156Hz * 10);
                    fader4.Value = (Int32)(equalizer.Gain220Hz * 10);
                    fader5.Value = (Int32)(equalizer.Gain311Hz * 10);
                    fader6.Value = (Int32)(equalizer.Gain440Hz * 10);
                    fader7.Value = (Int32)(equalizer.Gain622Hz * 10);
                    fader8.Value = (Int32)(equalizer.Gain880Hz * 10);
                    fader9.Value = (Int32)(equalizer.Gain1_2kHz * 10);
                    fader10.Value = (Int32)(equalizer.Gain1_8kHz * 10);
                    fader11.Value = (Int32)(equalizer.Gain2_5kHz * 10);
                    fader12.Value = (Int32)(equalizer.Gain3_5kHz * 10);
                    fader13.Value = (Int32)(equalizer.Gain5kHz * 10);
                    fader14.Value = (Int32)(equalizer.Gain7kHz * 10);
                    fader15.Value = (Int32)(equalizer.Gain10kHz * 10);
                    fader16.Value = (Int32)(equalizer.Gain14kHz * 10);
                    fader17.Value = (Int32)(equalizer.Gain20kHz * 10);

                    // Set config                    
                    Main.Config.EQPreset = equalizer.Name;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EQPreset GetEQPresetFromCurrentValues()
        {
            EQPreset preset = new EQPreset();

            for (int a = 0; a < 18; a++)
            {
                FieldInfo infoFader = this.GetType().GetField("fader" + a.ToString(), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                MPfm.WindowsControls.VolumeFader fader = infoFader.GetValue(this) as MPfm.WindowsControls.VolumeFader;
                preset.Bands[a].Gain = (float)fader.Value / 10;
            }

            return preset;
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
            int index = 0;
            int.TryParse(fader.Tag.ToString(), out index);

            // Get label
            FieldInfo infoLabel = this.GetType().GetField("lblGain" + index.ToString(), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            MPfm.WindowsControls.Label label = infoLabel.GetValue(this) as MPfm.WindowsControls.Label;

            float gain = (float)fader.Value / 10;            
            m_main.PlayerV4.UpdateEQBand(index, gain);

            // Update dB display
            string strDB = "";
            if (gain > 0)
            {
                strDB = "+" + gain.ToString("0.0") + " dB";
            }
            else
            {
                strDB = gain.ToString("0.0") + " dB";
            }
            label.Text = strDB;

            //// Default gain: 0 dB
            //double gainParameter = 1;

            //// Since the track resolution top is 30 and the min is -30, divide by 10 to get actual dB.
            //float trackValue = ((float)fader.Value) / 10;

            //// 10 power dB            
            ////gainParameter = Math.Pow(10, trackValue / 10);
            //gainParameter = Math.Pow(10, trackValue / 20);
            ////gainParameter = Math.Pow(10, trackValue);

            //// FMOD_DSP_PARAMEQ_GAIN
            //// Frequency Gain. 0.05 to 3.0. Default = 1.0 (no gain)
            ////
            //// Fader value range: -30 to 30. 0 = no gain.
            ////
            //// My tests: Gain parameter of 2 is a LOT louder.

            ////gainParameter = ratio;


            //label.Text = gainParameter.ToString("0.00");
            

            //try
            //{
            //    // Set EQ gain
            //    PropertyInfo propEQ = Main.Player.GetType().GetProperty("ParamEQ" + hertz);

            //    if (propEQ != null)
            //    {
            //        object obj = propEQ.GetValue(Main.Player, null);

            //        if (obj != null)
            //        {
            //            ParamEQDSP dsp = (ParamEQDSP)obj;
            //            dsp.SetGain((float)gainParameter);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Ignore for the moment
            //}
        }

        #endregion
    }
}