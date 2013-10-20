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
using System.Windows;
using System.Windows.Threading;
using MPfm.Library.Objects;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    public partial class EffectsWindow : BaseWindow, IDesktopEffectsView
    {
        public EffectsWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
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
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    comboEQPreset.Items.Clear();
            //    foreach (EQPreset preset in presets)
            //    {
            //        comboEQPreset.Items.Add(preset);
            //    }
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
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
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    txtEQPresetName.Text = preset.Name;
            //    //lblGain0.Text = preset.Gain0;
            //    fader0.Value = (Int32)(preset.Bands[0].Gain * 10);
            //    fader1.Value = (Int32)(preset.Bands[1].Gain * 10);
            //    fader2.Value = (Int32)(preset.Bands[2].Gain * 10);
            //    fader3.Value = (Int32)(preset.Bands[3].Gain * 10);
            //    fader4.Value = (Int32)(preset.Bands[4].Gain * 10);
            //    fader5.Value = (Int32)(preset.Bands[5].Gain * 10);
            //    fader6.Value = (Int32)(preset.Bands[6].Gain * 10);
            //    fader7.Value = (Int32)(preset.Bands[7].Gain * 10);
            //    fader8.Value = (Int32)(preset.Bands[8].Gain * 10);
            //    fader9.Value = (Int32)(preset.Bands[9].Gain * 10);
            //    fader10.Value = (Int32)(preset.Bands[10].Gain * 10);
            //    fader11.Value = (Int32)(preset.Bands[11].Gain * 10);
            //    fader12.Value = (Int32)(preset.Bands[12].Gain * 10);
            //    fader13.Value = (Int32)(preset.Bands[13].Gain * 10);
            //    fader14.Value = (Int32)(preset.Bands[14].Gain * 10);
            //    fader15.Value = (Int32)(preset.Bands[15].Gain * 10);
            //    fader16.Value = (Int32)(preset.Bands[16].Gain * 10);
            //    fader17.Value = (Int32)(preset.Bands[17].Gain * 10);
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        #endregion

    }
}
