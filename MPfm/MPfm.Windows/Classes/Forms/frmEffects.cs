// Copyright © 2011-2013 Yanick Castonguay
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
using System.Reflection;
using System.Windows.Forms;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.WindowsControls;

namespace MPfm.Windows.Classes.Forms
{
    /// <summary>
    /// Effects window. This is where the user can configure a 18-band equalizer and
    /// VST plugins.
    /// </summary>
    public partial class frmEffects : BaseForm, IDesktopEffectsView
    {
        public frmEffects(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Auto Level" button. Auto levels the
        /// equalizer values.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnAutoLevel_Click(object sender, EventArgs e)
        {
            OnNormalizePreset();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Delete preset" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnDeletePreset_Click(object sender, EventArgs e)
        {
            //OnDeletePreset()
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save Preset" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSavePreset_Click(object sender, EventArgs e)
        {
            OnSavePreset(txtEQPresetName.Text);
        }

        /// <summary>
        /// Occurs when the user clicks on the "EQ on" check box. Enables or
        /// disables the equalizer.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void chkEQOn_CheckedChanged(object sender, EventArgs e)
        {
            OnBypassEqualizer();
        }        

        /// <summary>
        /// Occurs when the user clicks on the "Reset EQ" button. Resets the values
        /// of the equalizer to 0.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnResetEQ_Click(object sender, EventArgs e)
        {
            OnResetPreset();
        }

        /// <summary>
        /// Occurs when the user changes the selected preset. Updates the EQ values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboEQPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboEQPreset.SelectedItem == null)
                return;

            OnChangePreset(((EQPreset)comboEQPreset.SelectedItem).EQPresetId);
        }

        /// <summary>
        /// Occurs when the user changes the value of a fader. This event is dynamic based on the Event sender (fader).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void fader_ValueChanged(object sender, EventArgs e)
        {
            Fader fader = sender as Fader;
            int index = 0;
            int.TryParse(fader.Tag.ToString(), out index);

            FieldInfo infoLabelGain = this.GetType().GetField("lblGain" + index.ToString(), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            FieldInfo infoLabel = this.GetType().GetField("lbl" + index.ToString(), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            MPfm.WindowsControls.Label labelGain = infoLabelGain.GetValue(this) as MPfm.WindowsControls.Label;
            MPfm.WindowsControls.Label label = infoLabel.GetValue(this) as MPfm.WindowsControls.Label;

            float gain = (float)fader.Value / 10;            
            labelGain.Text = FormatEQValue(gain);
            OnSetFaderGain(label.Text, gain);
        }

        /// <summary>
        /// Occurs when the user clicks on the "EQ On" label.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void lblEQOn_Click(object sender, EventArgs e)
        {
            chkEQOn.Checked = !chkEQOn.Checked;
        }

        private string FormatEQValue(float value)
        {
            string strValue = string.Empty;
            if (value > 0)
                strValue = "+" + value.ToString("0.0").Replace(",", ".") + " dB";
            else
                strValue = value.ToString("0.0").Replace(",", ".") + " dB";
            return strValue;
        }

        #region IEqualizerPresetsView implementation

        public Action OnBypassEqualizer { get; set; }
        public Action<float> OnSetVolume { get; set; }
        public Action OnAddPreset { get; set; }
        public Action<Guid> OnLoadPreset { get; set; }
        public Action<Guid> OnEditPreset { get; set; }
        public Action<Guid> OnDeletePreset { get; set; }

        public void EqualizerPresetsError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                MessageBox.Show(string.Format("An error occured in EqualizerPresets: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshPresets(IEnumerable<EQPreset> presets, Guid selectedPresetId, bool isEQBypassed)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                comboEQPreset.Items.Clear();
                foreach (EQPreset preset in presets)
                {
                    comboEQPreset.Items.Add(preset);                    
                }
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
            // Not used on desktop devices
        }

        public void RefreshVolume(float volume)
        {
            // Not used on desktop devices
        }

        #endregion

        #region IEqualizerPresetDetailsView implementation

        public Action<Guid> OnChangePreset { get; set; }
        public Action OnResetPreset { get; set; }
        public Action OnNormalizePreset { get; set; }
        public Action OnRevertPreset { get; set; }
        public Action<string> OnSavePreset { get; set; }
        public Action<string, float> OnSetFaderGain { get; set; }

        public void EqualizerPresetDetailsError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                MessageBox.Show(string.Format("An error occured in EqualizerPresetDetails: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void ShowMessage(string title, string message)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshPreset(EQPreset preset)
        {
            MethodInvoker methodUIUpdate = delegate 
            {
                txtEQPresetName.Text = preset.Name;
                //lblGain0.Text = preset.Gain0;
                fader0.Value = (Int32)(preset.Bands[0].Gain * 10);
                fader1.Value = (Int32)(preset.Bands[1].Gain * 10);
                fader2.Value = (Int32)(preset.Bands[2].Gain * 10);
                fader3.Value = (Int32)(preset.Bands[3].Gain * 10);
                fader4.Value = (Int32)(preset.Bands[4].Gain * 10);
                fader5.Value = (Int32)(preset.Bands[5].Gain * 10);
                fader6.Value = (Int32)(preset.Bands[6].Gain * 10);
                fader7.Value = (Int32)(preset.Bands[7].Gain * 10);
                fader8.Value = (Int32)(preset.Bands[8].Gain * 10);
                fader9.Value = (Int32)(preset.Bands[9].Gain * 10);
                fader10.Value = (Int32)(preset.Bands[10].Gain * 10);
                fader11.Value = (Int32)(preset.Bands[11].Gain * 10);
                fader12.Value = (Int32)(preset.Bands[12].Gain * 10);
                fader13.Value = (Int32)(preset.Bands[13].Gain * 10);
                fader14.Value = (Int32)(preset.Bands[14].Gain * 10);
                fader15.Value = (Int32)(preset.Bands[15].Gain * 10);
                fader16.Value = (Int32)(preset.Bands[16].Gain * 10);
                fader17.Value = (Int32)(preset.Bands[17].Gain * 10);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        #endregion

    }
}
