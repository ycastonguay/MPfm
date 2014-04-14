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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using MPfm.Library.Objects;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.WPF.Classes.Controls;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    public partial class EffectsWindow : BaseWindow, IDesktopEffectsView
    {
        private EQPreset _preset;
        private List<EQPreset> _presets;

        public EffectsWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
            DataContext = this;

            gridCurrentPreset.IsEnabled = false;
            gridFaders.IsEnabled = false;
        }

        private void BtnNewPreset_OnClick(object sender, RoutedEventArgs e)
        {
            OnAddPreset();
        }

        private void BtnSavePreset_OnClick(object sender, RoutedEventArgs e)
        {
            OnSavePreset(txtPresetName.Text);
        }

        private void BtnDeletePreset_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to delete this equalizer preset?", "Delete equalizer preset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            OnDeletePreset(_preset.EQPresetId);
        }

        private void BtnNormalize_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to normalize this equalizer preset?", "Normalize equalizer preset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            OnNormalizePreset();
        }

        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to reset this equalizer preset?", "Reset equalizer preset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            OnResetPreset();
        }

        private void CboPresets_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboPresets.SelectedIndex == -1)
                return;

            OnEditPreset(_presets[cboPresets.SelectedIndex].EQPresetId);
        }

        private void Fader_OnFaderValueChanged(object sender, EventArgs e)
        {
            var fader = sender as Fader;
            int faderIndex = 0;
            int.TryParse(fader.Name.Replace("fader", ""), out faderIndex);

            if (_preset == null)
                return;

            float value = fader.Value/10f;
            var band = _preset.Bands[faderIndex];
            band.Gain = value;

            var fieldInfo = this.GetType().GetField(string.Format("lblValue{0}", faderIndex), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var label = fieldInfo.GetValue(this) as Label;
            if (label != null)
            {
                label.Content = FormatEQValue(value);
                OnSetFaderGain(band.CenterString, value);
            }
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

        private int FormatFaderValue(float value)
        {
            return (int)(value * 10);
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
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in EqualizerPresets: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void RefreshPresets(IEnumerable<EQPreset> presets, Guid selectedPresetId, bool isEQBypassed)
        {
            _presets = presets.ToList();
            _preset = _presets.FirstOrDefault(x => x.EQPresetId == selectedPresetId);
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                cboPresets.DisplayMemberPath = "Name";
                cboPresets.SelectedValuePath = "EQPresetId";
                cboPresets.ItemsSource = _presets;
                RefreshPreset(_preset);
            }));
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
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in EqualizerPresetDetails: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void ShowMessage(string title, string message)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void RefreshPreset(EQPreset preset)
        {
            if (preset == null)
                return;

            _preset = preset;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                gridCurrentPreset.IsEnabled = _preset != null;
                gridFaders.IsEnabled = _preset != null;
                txtPresetName.Text = preset.Name;
                //lblGain0.Text = preset.Gain0;
                fader0.ValueWithoutEvent = (Int32)(preset.Bands[0].Gain * 10);                
                fader1.ValueWithoutEvent = (Int32)(preset.Bands[1].Gain * 10);
                fader2.ValueWithoutEvent = (Int32)(preset.Bands[2].Gain * 10);
                fader3.ValueWithoutEvent = (Int32)(preset.Bands[3].Gain * 10);
                fader4.ValueWithoutEvent = (Int32)(preset.Bands[4].Gain * 10);
                fader5.ValueWithoutEvent = (Int32)(preset.Bands[5].Gain * 10);
                fader6.ValueWithoutEvent = (Int32)(preset.Bands[6].Gain * 10);
                fader7.ValueWithoutEvent = (Int32)(preset.Bands[7].Gain * 10);
                fader8.ValueWithoutEvent = (Int32)(preset.Bands[8].Gain * 10);
                fader9.ValueWithoutEvent = (Int32)(preset.Bands[9].Gain * 10);
                fader10.ValueWithoutEvent = (Int32)(preset.Bands[10].Gain * 10);
                fader11.ValueWithoutEvent = (Int32)(preset.Bands[11].Gain * 10);
                fader12.ValueWithoutEvent = (Int32)(preset.Bands[12].Gain * 10);
                fader13.ValueWithoutEvent = (Int32)(preset.Bands[13].Gain * 10);
                fader14.ValueWithoutEvent = (Int32)(preset.Bands[14].Gain * 10);
                fader15.ValueWithoutEvent = (Int32)(preset.Bands[15].Gain * 10);
                fader16.ValueWithoutEvent = (Int32)(preset.Bands[16].Gain * 10);
                fader17.ValueWithoutEvent = (Int32)(preset.Bands[17].Gain * 10);

                lblValue0.Content = FormatEQValue(preset.Bands[0].Gain);
                lblValue1.Content = FormatEQValue(preset.Bands[1].Gain);
                lblValue2.Content = FormatEQValue(preset.Bands[2].Gain);
                lblValue3.Content = FormatEQValue(preset.Bands[3].Gain);
                lblValue4.Content = FormatEQValue(preset.Bands[4].Gain);
                lblValue5.Content = FormatEQValue(preset.Bands[5].Gain);
                lblValue6.Content = FormatEQValue(preset.Bands[6].Gain);
                lblValue7.Content = FormatEQValue(preset.Bands[7].Gain);
                lblValue8.Content = FormatEQValue(preset.Bands[8].Gain);
                lblValue9.Content = FormatEQValue(preset.Bands[9].Gain);
                lblValue10.Content = FormatEQValue(preset.Bands[10].Gain);
                lblValue11.Content = FormatEQValue(preset.Bands[11].Gain);
                lblValue12.Content = FormatEQValue(preset.Bands[12].Gain);
                lblValue13.Content = FormatEQValue(preset.Bands[13].Gain);
                lblValue14.Content = FormatEQValue(preset.Bands[14].Gain);
                lblValue15.Content = FormatEQValue(preset.Bands[15].Gain);
                lblValue16.Content = FormatEQValue(preset.Bands[16].Gain);
                lblValue17.Content = FormatEQValue(preset.Bands[17].Gain);
            }));
        }

        #endregion

    }
}
