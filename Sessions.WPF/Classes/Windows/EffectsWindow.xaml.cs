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
using System.Windows.Input;
using System.Windows.Threading;
using MPfm.MVP.Views;
using MPfm.WPF.Classes.Controls;
using MPfm.WPF.Classes.Helpers;
using MPfm.WPF.Classes.Windows.Base;
using Sessions.Player.Objects;

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

            EnablePresetDetails(false);
        }

        private void EnableContextMenu(bool enable)
        {
            //menuRemovePreset.Enabled = enable;
            //menuDuplicatePreset.Enabled = enable;
            //menuExportPreset.Enabled = enable;
        }

        private void EnablePresetDetails(bool enable)
        {
            txtPresetName.IsEnabled = enable;
            btnSavePreset.Enabled = enable;
            btnNormalize.Enabled = enable;
            btnReset.Enabled = enable;
        }

        private void EnableFaders(bool enable)
        {
            fader0.IsEnabled = enable;
            fader1.IsEnabled = enable;
            fader2.IsEnabled = enable;
            fader3.IsEnabled = enable;
            fader4.IsEnabled = enable;
            fader5.IsEnabled = enable;
            fader6.IsEnabled = enable;
            fader7.IsEnabled = enable;
            fader8.IsEnabled = enable;
            fader9.IsEnabled = enable;
            fader10.IsEnabled = enable;
            fader11.IsEnabled = enable;
            fader12.IsEnabled = enable;
            fader13.IsEnabled = enable;
            fader14.IsEnabled = enable;
            fader15.IsEnabled = enable;
            fader16.IsEnabled = enable;
            fader17.IsEnabled = enable;

            double alpha = enable ? 1 : 0.5;
            fader0.Opacity = alpha;
            fader1.Opacity = alpha;
            fader2.Opacity = alpha;
            fader3.Opacity = alpha;
            fader4.Opacity = alpha;
            fader5.Opacity = alpha;
            fader6.Opacity = alpha;
            fader7.Opacity = alpha;
            fader8.Opacity = alpha;
            fader9.Opacity = alpha;
            fader10.Opacity = alpha;
            fader11.Opacity = alpha;
            fader12.Opacity = alpha;
            fader13.Opacity = alpha;
            fader14.Opacity = alpha;
            fader15.Opacity = alpha;
            fader16.Opacity = alpha;
            fader17.Opacity = alpha;
        }

        private void ResetFaderValuesAndPresetDetails()
        {
            txtPresetName.Text = string.Empty;
            fader0.ValueWithoutEvent = 0;
            fader1.ValueWithoutEvent = 0;
            fader2.ValueWithoutEvent = 0;
            fader3.ValueWithoutEvent = 0;
            fader4.ValueWithoutEvent = 0;
            fader5.ValueWithoutEvent = 0;
            fader6.ValueWithoutEvent = 0;
            fader7.ValueWithoutEvent = 0;
            fader8.ValueWithoutEvent = 0;
            fader9.ValueWithoutEvent = 0;
            fader10.ValueWithoutEvent = 0;
            fader11.ValueWithoutEvent = 0;
            fader12.ValueWithoutEvent = 0;
            fader13.ValueWithoutEvent = 0;
            fader14.ValueWithoutEvent = 0;
            fader15.ValueWithoutEvent = 0;
            fader16.ValueWithoutEvent = 0;
            fader17.ValueWithoutEvent = 0;
        }

        private void BtnSavePreset_OnClick(object sender, RoutedEventArgs e)
        {
            OnSavePreset(txtPresetName.Text);
        }

        private void BtnNormalize_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to normalize this equalizer preset?", "Normalize equalizer preset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            OnNormalizePreset();
        }

        private void BtnAddPreset_OnClick(object sender, RoutedEventArgs e)
        {
            OnAddPreset();
        }

        private void BtnRemovePreset_OnClick(object sender, RoutedEventArgs e)
        {
            RemovePreset();
        }

        private void RemovePreset()
        {
            if (MessageBox.Show("Are you sure you wish to delete this equalizer preset?", "Delete equalizer preset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            OnDeletePreset(_preset.EQPresetId);
            EnableFaders(false);
            EnablePresetDetails(false);
            ResetFaderValuesAndPresetDetails();
            _preset = null;
        }

        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to reset this equalizer preset?", "Reset equalizer preset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            OnResetPreset();
        }

        private void ListViewPresets_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //EnableMarkerButtons(listViewMarkers.SelectedIndex >= 0);

            //if (listViewMarkers.SelectedIndex >= 0)
            //    _selectedMarkerIndex = listViewMarkers.SelectedIndex;

            EnablePresetDetails(listViewPresets.SelectedIndex >= 0);
            EnableFaders(listViewPresets.SelectedIndex >= 0);
            EnableContextMenu(listViewPresets.SelectedIndex >= 0);
            if (listViewPresets.SelectedIndex < 0)
            {
                ResetFaderValuesAndPresetDetails();
                return;
            }

            var id = _presets[listViewPresets.SelectedIndex].EQPresetId;
            OnLoadPreset(id); // EqualizerPresets
            OnChangePreset(id); // EqualizerPresetDetails
        }

        private void ListViewPresets_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIHelper.ListView_PreviewMouseDown_RemoveSelectionIfNotClickingOnAListViewItem(listViewPresets, e);
        }

        private void MenuItemRemovePreset_OnClick(object sender, RoutedEventArgs e)
        {
            RemovePreset();
        }

        private void MenuItemDuplicatePreset_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewPresets.SelectedIndex < 0)
                return;

            var preset = _presets[listViewPresets.SelectedIndex];
            OnDuplicatePreset(preset.EQPresetId);
        }

        private void MenuItemExportPreset_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewPresets.SelectedIndex < 0)
                return;

            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "JSON file (*.json)|*.json";
            dialog.Title = "Export preset as JSON";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var preset = _presets[listViewPresets.SelectedIndex];
                OnExportPreset(preset.EQPresetId, dialog.FileName);
            }
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
        public Action<Guid> OnDuplicatePreset { get; set; }
        public Action<Guid, string> OnExportPreset { get; set; }

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
                listViewPresets.Items.Clear();
                foreach (var preset in _presets)
                    listViewPresets.Items.Add(preset);
                //cboPresets.DisplayMemberPath = "Name";
                //cboPresets.SelectedValuePath = "EQPresetId";
                //cboPresets.ItemsSource = _presets;
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
                int row = _presets.FindIndex(x => x.EQPresetId == _preset.EQPresetId);
                if (row >= 0)
                    listViewPresets.SelectedIndex = row;

                EnableFaders(true);
                EnablePresetDetails(true);

                //gridCurrentPreset.IsEnabled = _preset != null;
                //gridFaders.IsEnabled = _preset != null;
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
