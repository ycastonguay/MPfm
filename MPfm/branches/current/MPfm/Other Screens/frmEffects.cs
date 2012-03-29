//
// frmEffects.cs: Effects window. This is where the user can configure a 18-band equalizer and VST plugins.
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
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using MPfm.Library;
using MPfm.Player;
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
        // Private variables
        private bool isFormLoaded = false;

        /// <summary>
        /// Private value for the Main property.
        /// </summary>
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
        /// Constructor for Effects window. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmEffects(frmMain main)
        {
            InitializeComponent();
            this.main = main;

            RefreshEQPresets();
            LoadConfig();

            // Set flag
            isFormLoaded = true;
        }

        #region Close Events

        /// <summary>
        /// Occurs when the form is about to close.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmEffects_FormClosing(object sender, FormClosingEventArgs e)
        {            
            SaveConfig();

            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {                
                e.Cancel = true;
                this.Hide();
                Main.btnEffects.Checked = false;
            }
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Loads the form values based on configuration.
        /// </summary>
        private void LoadConfig()
        {   
            // Set UI values
            chkEQOn.Checked = Main.Config.Audio.EQ.Enabled;
            comboEQPreset.SelectedItem = Main.Config.Audio.EQ.Preset;

            // Set player EQ
            if (Main.Config.Audio.EQ.Enabled && !String.IsNullOrEmpty(Main.Config.Audio.EQ.Preset))
            {
                // Set preset
                SetPreset(Main.Config.Audio.EQ.Preset);
            }
            
        }

        /// <summary>
        /// Saves the form values in the configuration file.
        /// </summary>
        private void SaveConfig()
        {
            Main.Config.Audio.EQ.Enabled = chkEQOn.Checked;

            if (comboEQPreset.SelectedItem != null)
            {
                Main.Config.Audio.EQ.Preset = comboEQPreset.SelectedItem.ToString();
            }
            else
            {
                Main.Config.Audio.EQ.Preset = string.Empty;
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
            List<EQPreset> eqs = main.Library.Gateway.SelectEQPresets();

            // For each EQ
            foreach (EQPreset eq in eqs)
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnDeletePreset_Click(object sender, EventArgs e)
        {
            // Check if equalizer exists            
            //Equalizer equalizerExists = DataAccess.SelectEqualizer(txtEQPresetName.Text);
            EQPreset equalizerExists = main.Library.Gateway.SelectEQPreset(txtEQPresetName.Text);

            if (equalizerExists != null)
            {
                // Are you sure?
                if (MessageBox.Show("Are you sure you wish to delete this equalizer?", "Delete equalizer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {                    
                    //DataAccess.DeleteEqualizer(new Guid(equalizerExists.EqualizerId));
                    main.Library.Gateway.DeleteEqualizer(equalizerExists.EQPresetId);

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

            // Build preset
            EQPreset eq = new EQPreset();
            eq.Name = txtEQPresetName.Text;
            eq.Bands[0].Gain = (float)fader0.Value / 10;
            eq.Bands[1].Gain = (float)fader1.Value / 10;
            eq.Bands[2].Gain = (float)fader2.Value / 10;
            eq.Bands[3].Gain = (float)fader3.Value / 10;
            eq.Bands[4].Gain = (float)fader4.Value / 10;
            eq.Bands[5].Gain = (float)fader5.Value / 10;
            eq.Bands[6].Gain = (float)fader6.Value / 10;
            eq.Bands[7].Gain = (float)fader7.Value / 10;
            eq.Bands[8].Gain = (float)fader8.Value / 10;
            eq.Bands[9].Gain = (float)fader9.Value / 10;
            eq.Bands[10].Gain = (float)fader10.Value / 10;
            eq.Bands[11].Gain = (float)fader11.Value / 10;
            eq.Bands[12].Gain = (float)fader12.Value / 10;
            eq.Bands[13].Gain = (float)fader13.Value / 10;
            eq.Bands[14].Gain = (float)fader14.Value / 10;
            eq.Bands[15].Gain = (float)fader15.Value / 10;
            eq.Bands[16].Gain = (float)fader16.Value / 10;
            eq.Bands[17].Gain = (float)fader17.Value / 10;            

            // Check if equalizer exists                        
            EQPreset equalizerExists = main.Library.Gateway.SelectEQPreset(txtEQPresetName.Text);

            if (equalizerExists == null)
            {
                //DataAccess.InsertEqualizer(eq);
                main.Library.Gateway.InsertEqualizer(eq);
            }
            else
            {
                if (MessageBox.Show("Are you sure you wish to overwrite the " + equalizerExists.Name + " equalizer preset?", "Overwrite equalizer preset", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    // Update ID and EQ
                    eq.EQPresetId = equalizerExists.EQPresetId;                    
                    main.Library.Gateway.UpdateEqualizer(eq);
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void chkEQOn_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the form has loaded
            if (!isFormLoaded)
            {
                return;
            }

            // Bypass EQ
            main.Player.BypassEQ();

            // Set configuration and save
            main.Config.Audio.EQ.Enabled = chkEQOn.Checked;
            main.Config.Save();

            //// Set equalizer
            //if (m_main.Player.IsEQEnabled)
            //{
            //    // Remove EQ
            //    main.Player.RemoveEQ();
            //}
            //else
            //{
            //    // Add EQ
            //    EQPreset preset = GetEQPresetFromCurrentValues();
            //    main.Player.AddEQ(preset);
            //}
        }        

        /// <summary>
        /// Occurs when the user clicks on the "Reset EQ" button. Resets the values
        /// of the equalizer to 0.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnResetEQ_Click(object sender, EventArgs e)
        {
            // Reset equalizer
            ResetEQ();
        }

        /// <summary>
        /// Sets an equalizer preset.
        /// </summary>
        /// <param name="name">Preset name</param>
        private void SetPreset(string name)
        {
            // Get equalizer                    
            EQPreset equalizer = main.Library.Gateway.SelectEQPreset(name);

            // Set values
            txtEQPresetName.Text = equalizer.Name;
            fader0.Value = (Int32)(equalizer.Bands[0].Gain * 10);
            fader1.Value = (Int32)(equalizer.Bands[1].Gain * 10);
            fader2.Value = (Int32)(equalizer.Bands[2].Gain * 10);
            fader3.Value = (Int32)(equalizer.Bands[3].Gain * 10);
            fader4.Value = (Int32)(equalizer.Bands[4].Gain * 10);
            fader5.Value = (Int32)(equalizer.Bands[5].Gain * 10);
            fader6.Value = (Int32)(equalizer.Bands[6].Gain * 10);
            fader7.Value = (Int32)(equalizer.Bands[7].Gain * 10);
            fader8.Value = (Int32)(equalizer.Bands[8].Gain * 10);
            fader9.Value = (Int32)(equalizer.Bands[9].Gain * 10);
            fader10.Value = (Int32)(equalizer.Bands[10].Gain * 10);
            fader11.Value = (Int32)(equalizer.Bands[11].Gain * 10);
            fader12.Value = (Int32)(equalizer.Bands[12].Gain * 10);
            fader13.Value = (Int32)(equalizer.Bands[13].Gain * 10);
            fader14.Value = (Int32)(equalizer.Bands[14].Gain * 10);
            fader15.Value = (Int32)(equalizer.Bands[15].Gain * 10);
            fader16.Value = (Int32)(equalizer.Bands[16].Gain * 10);
            fader17.Value = (Int32)(equalizer.Bands[17].Gain * 10);
        }

        /// <summary>
        /// Occurs when the user changes the selected preset. Updates the EQ values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboEQPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the form has loaded
            if (!isFormLoaded)
            {
                return;
            }

            try
            {
                // Check if selection is valid
                if (comboEQPreset.SelectedItem != null && !String.IsNullOrEmpty(comboEQPreset.SelectedItem.ToString()))
                {
                    // Set preset
                    SetPreset(comboEQPreset.SelectedItem.ToString());

                    // Set config                    
                    Main.Config.Audio.EQ.Preset = comboEQPreset.SelectedItem.ToString();
                    Main.Config.Save();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns the EQ preset based on current values.
        /// </summary>
        /// <returns>EQPreset object</returns>
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
        /// Occurs when the user changes the value of a fader. This event is dynamic based on the Event sender (fader).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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
            main.Player.UpdateEQBand(index, gain, true);

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
        }

        #endregion
    }
}