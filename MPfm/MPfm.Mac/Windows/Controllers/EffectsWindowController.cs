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
using System.Linq;
using MPfm.MVP;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.Mac.Classes.Objects;
using MPfm.Mac.Classes.Helpers;
using MPfm.Mac.Classes.Controls;
using System.Reflection;

namespace MPfm.Mac
{
    public partial class EffectsWindowController : BaseWindowController, IDesktopEffectsView
    {
        EQPreset _preset;

        public EffectsWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        public EffectsWindowController(Action<IBaseView> onViewReady) 
            : base ("EffectsWindow", onViewReady)
        {
            Initialize();
        }
        
        private void Initialize()
        {
            this.Window.Center();
            this.Window.MakeKeyAndOrderFront(this);
        }
        
        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            LoadFontsAndImages();
            OnViewReady(this);
        }

        private void LoadFontsAndImages()
        {
            //viewBackgroundInformation.IsEnabled = false;
            //viewBackgroundFaders.IsEnabled = false;

            viewBackgroundPreset.HeaderHeight = 22;
            viewBackgroundInformation.HeaderHeight = 22;

            viewBackground.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewBackground.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            viewBackgroundPreset.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewBackgroundPreset.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            viewBackgroundPreset.HeaderColor1 = GlobalTheme.PanelHeaderColor1;
            viewBackgroundPreset.HeaderColor2 = GlobalTheme.PanelHeaderColor2;
            viewBackgroundPreset.IsHeaderVisible = true;
            viewBackgroundInformation.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewBackgroundInformation.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            viewBackgroundInformation.HeaderColor1 = GlobalTheme.PanelHeaderColor1;
            viewBackgroundInformation.HeaderColor2 = GlobalTheme.PanelHeaderColor2;
            viewBackgroundInformation.IsHeaderVisible = true;

            popupPreset.Font = NSFont.FromFontName("Roboto", 11f);
            lblName.Font = NSFont.FromFontName("Roboto", 11f);
            lblTitleInformation.Font = NSFont.FromFontName("Roboto", 13f);
            lblTitlePreset.Font = NSFont.FromFontName("Roboto", 13f);
            lblEQOn.Font = NSFont.FromFontName("Roboto", 11f);
            lblScalePlus6.Font = NSFont.FromFontName("Roboto", 11f);
            lblScale0.Font = NSFont.FromFontName("Roboto", 11f);
            lblScaleMinus6.Font = NSFont.FromFontName("Roboto", 11f);
            txtName.Font = NSFont.FromFontName("Roboto", 11f);

            btnNewPreset.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_add");
            btnAutoLevel.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_reset");
            btnDelete.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_delete");
            btnSave.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_save");
            btnReset.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_reset");

            for(int a = 0; a < 18; a++)
                ConfigureFader(a);

            SetTheme();
        }

        private void SetTheme()
        {
//            // Set colors
//            viewLeftHeader.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewLeftHeader.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            viewRightHeader.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewRightHeader.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            viewLibraryBrowser.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewLibraryBrowser.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            viewNowPlaying.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewNowPlaying.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);

            // Set tags for fader (to find out which fader is being modified)
            fader0.Index = 0;
            fader1.Index = 1;
            fader2.Index = 2;
            fader3.Index = 3;
            fader4.Index = 4;
            fader5.Index = 5;
            fader6.Index = 6;
            fader7.Index = 7;
            fader8.Index = 8;
            fader9.Index = 9;
            fader10.Index = 10;
            fader11.Index = 11;
            fader12.Index = 12;
            fader13.Index = 13;
            fader14.Index = 14;
            fader15.Index = 15;
            fader16.Index = 16;
            fader17.Index = 17;

            // Set label fonts
            lblEQ0.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ1.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ2.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ3.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ4.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ5.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ6.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ7.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ8.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ9.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ10.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ11.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ12.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ13.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ14.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ15.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ16.Font = NSFont.FromFontName("Roboto", 11);
            lblEQ17.Font = NSFont.FromFontName("Roboto", 11);

            lblEQValue0.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue1.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue2.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue3.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue4.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue5.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue6.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue7.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue8.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue9.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue10.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue11.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue12.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue13.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue14.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue15.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue16.Font = NSFont.FromFontName("Roboto", 11);
            lblEQValue17.Font = NSFont.FromFontName("Roboto", 11);
        }

        partial void actionPresetChange(NSObject sender)
        {
            string tag = popupPreset.SelectedItem.ToolTip;
            OnLoadPreset(new Guid(tag)); // EqualizerPresets
            OnChangePreset(new Guid(tag)); // EqualizerPresetDetails
        }

        partial void actionEQOnChange(NSObject sender)
        {
            OnBypassEqualizer();
        }

        partial void actionNameChanged(NSObject sender)
        {
        }

        partial void actionNewPreset(NSObject sender)
        {
            OnAddPreset();
        }

        partial void actionSave(NSObject sender)
        {
            OnSavePreset(txtName.StringValue);
        }

        partial void actionDelete(NSObject sender)
        {
            using(NSAlert alert = new NSAlert())
            {
                alert.MessageText = "Equalizer preset will be deleted";
                alert.InformativeText = "Are you sure you wish to delete this equalizer preset?";
                alert.AlertStyle = NSAlertStyle.Warning;
                var btnOK = alert.AddButton("OK");
                btnOK.Activated += (sender2, e2) => {
                    NSApplication.SharedApplication.StopModal();
                    OnDeletePreset(_preset.EQPresetId);
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => {
                    NSApplication.SharedApplication.StopModal();
                };
                alert.RunModal();
            }
        }

        partial void actionAutoLevel(NSObject sender)
        {
            using(NSAlert alert = new NSAlert())
            {
                alert.MessageText = "Equalizer preset will be normalized";
                alert.InformativeText = "Are you sure you wish to normalize this equalizer preset?";
                alert.AlertStyle = NSAlertStyle.Warning;
                var btnOK = alert.AddButton("OK");
                btnOK.Activated += (sender2, e2) => {
                    NSApplication.SharedApplication.StopModal();
                    //NSApplication.SharedApplication.EndSheet((NSWindow)alert.Window); // crashes
                    OnNormalizePreset();
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => {
                    NSApplication.SharedApplication.StopModal();
                    //NSApplication.SharedApplication.EndSheet((NSWindow)alert.Window); // crashes
                };
                alert.RunModal();
//                alert.BeginSheet(this.Window, () => {
//                    //this.Window.Close();
//                });
            }
        }

        partial void actionReset(NSObject sender)
        {
            using(NSAlert alert = new NSAlert())
            {
                alert.MessageText = "Equalizer preset will be reset";
                alert.InformativeText = "Are you sure you wish to reset this equalizer preset?";
                alert.AlertStyle = NSAlertStyle.Warning;
                var btnOK = alert.AddButton("OK");
                btnOK.Activated += (sender2, e2) => {
                    NSApplication.SharedApplication.StopModal();
                    OnResetPreset();
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => {
                    NSApplication.SharedApplication.StopModal();
                };
                alert.RunModal();
            }
        }

        private void ConfigureFader(int index)
        {
            var fieldInfo = this.GetType().GetProperty(string.Format("fader{0}", index), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var fader = fieldInfo.GetValue(this) as MPfmFaderView;
            fader.OnFaderValueChanged += HandleOnFaderValueChanged;
            fader.Minimum = -60;
            fader.Maximum = 60;
            //fader.Value = 0;
        }

        private void HandleOnFaderValueChanged(object sender, EventArgs e)
        {
            if (_preset == null)
                return;

            var fader = (MPfmFaderView)sender;
            int pos = fader.Index;
            var fieldInfo = this.GetType().GetProperty(string.Format("lblEQValue{0}", pos), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var label = fieldInfo.GetValue(this) as NSTextField;
            if (label != null)
            {
                float value = fader.Value / 10f;
                label.StringValue = FormatEQValue(value);
                OnSetFaderGain(_preset.Bands[pos].CenterString, value);
            }
        }

        private string FormatEQValue(float value)
        {
            string strValue = string.Empty;
            if(value > 0)
                strValue = "+" + value.ToString("0.0").Replace(",",".") + " dB";
            else
                strValue = value.ToString("0.0").Replace(",",".") + " dB";
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
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in EqualizerPresets: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshPresets(IEnumerable<EQPreset> presets, Guid selectedPresetId, bool isEQBypassed)
        {
            InvokeOnMainThread(() => {
                popupPreset.RemoveAllItems();
                foreach(var preset in presets)
                {
                    popupPreset.AddItem(preset.EQPresetId.ToString());
                    popupPreset.LastItem.Title = preset.EQPresetId.ToString(); //preset.Name;
                    popupPreset.LastItem.ToolTip = preset.EQPresetId.ToString(); // Tag only supports an integer!
                }
            });
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
            // Only shown on mobile devices.
        }

        public void RefreshVolume(float volume)
        {
            // Only shown on mobile devices.
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
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in EqualizerPresets: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void ShowMessage(string title, string message)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert(title, message, NSAlertStyle.Warning);
            });
        }

        public void RefreshPreset(EQPreset preset)
        {
            InvokeOnMainThread(delegate {
                _preset = preset;
                //viewBackgroundInformation.IsEnabled = _preset != null;
                //viewBackgroundFaders.IsEnabled = _preset != null;

                txtName.StringValue = _preset.Name;
                fader0.ValueWithoutEvent = FormatFaderValue(_preset.Gain0);
                fader1.ValueWithoutEvent = FormatFaderValue(_preset.Gain1);
                fader2.ValueWithoutEvent = FormatFaderValue(_preset.Gain2);
                fader3.ValueWithoutEvent = FormatFaderValue(_preset.Gain3);
                fader4.ValueWithoutEvent = FormatFaderValue(_preset.Gain4);
                fader5.ValueWithoutEvent = FormatFaderValue(_preset.Gain5);
                fader6.ValueWithoutEvent = FormatFaderValue(_preset.Gain6);
                fader7.ValueWithoutEvent = FormatFaderValue(_preset.Gain7);
                fader8.ValueWithoutEvent = FormatFaderValue(_preset.Gain8);
                fader9.ValueWithoutEvent = FormatFaderValue(_preset.Gain9);
                fader10.ValueWithoutEvent = FormatFaderValue(_preset.Gain10);
                fader11.ValueWithoutEvent = FormatFaderValue(_preset.Gain11);
                fader12.ValueWithoutEvent = FormatFaderValue(_preset.Gain12);
                fader13.ValueWithoutEvent = FormatFaderValue(_preset.Gain13);
                fader14.ValueWithoutEvent = FormatFaderValue(_preset.Gain14);
                fader15.ValueWithoutEvent = FormatFaderValue(_preset.Gain15);
                fader16.ValueWithoutEvent = FormatFaderValue(_preset.Gain16);
                fader17.ValueWithoutEvent = FormatFaderValue(_preset.Gain17);
                lblEQValue0.StringValue = FormatEQValue(_preset.Gain0);
                lblEQValue1.StringValue = FormatEQValue(_preset.Gain1);
                lblEQValue2.StringValue = FormatEQValue(_preset.Gain2);
                lblEQValue3.StringValue = FormatEQValue(_preset.Gain3);
                lblEQValue4.StringValue = FormatEQValue(_preset.Gain4);
                lblEQValue5.StringValue = FormatEQValue(_preset.Gain5);
                lblEQValue6.StringValue = FormatEQValue(_preset.Gain6);
                lblEQValue7.StringValue = FormatEQValue(_preset.Gain7);
                lblEQValue8.StringValue = FormatEQValue(_preset.Gain8);
                lblEQValue9.StringValue = FormatEQValue(_preset.Gain9);
                lblEQValue10.StringValue = FormatEQValue(_preset.Gain10);
                lblEQValue11.StringValue = FormatEQValue(_preset.Gain11);
                lblEQValue12.StringValue = FormatEQValue(_preset.Gain12);
                lblEQValue13.StringValue = FormatEQValue(_preset.Gain13);
                lblEQValue14.StringValue = FormatEQValue(_preset.Gain14);
                lblEQValue15.StringValue = FormatEQValue(_preset.Gain15);
                lblEQValue16.StringValue = FormatEQValue(_preset.Gain16);
                lblEQValue17.StringValue = FormatEQValue(_preset.Gain17);
            });
        }

        #endregion

    }
}
