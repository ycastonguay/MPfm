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
            OnViewReady.Invoke(this);
        }

        private void LoadFontsAndImages()
        {
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

            popupPreset.Font = NSFont.FromFontName("Junction", 11f);
            lblName.Font = NSFont.FromFontName("Junction", 11f);
            lblTitleInformation.Font = NSFont.FromFontName("TitilliumText25L-800wt", 13f);
            lblTitlePreset.Font = NSFont.FromFontName("TitilliumText25L-800wt", 13f);
            lblEQOn.Font = NSFont.FromFontName("Junction", 11f);
            lblScalePlus6.Font = NSFont.FromFontName("Junction", 11f);
            lblScale0.Font = NSFont.FromFontName("Junction", 11f);
            lblScaleMinus6.Font = NSFont.FromFontName("Junction", 11f);
            txtName.Font = NSFont.FromFontName("Junction", 11f);

            btnNewPreset.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_fam_exclamation");
            btnAutoLevel.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_fam_shape_align_middle");
            btnDelete.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_fam_delete");
            btnSave.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_fam_tick");
            btnReset.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_fam_exclamation");

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
//            
//            // Set label fonts
            lblEQ0.Font = NSFont.FromFontName("Junction", 11);
            lblEQ1.Font = NSFont.FromFontName("Junction", 11);
            lblEQ2.Font = NSFont.FromFontName("Junction", 11);
            lblEQ3.Font = NSFont.FromFontName("Junction", 11);
            lblEQ4.Font = NSFont.FromFontName("Junction", 11);
            lblEQ5.Font = NSFont.FromFontName("Junction", 11);
            lblEQ6.Font = NSFont.FromFontName("Junction", 11);
            lblEQ7.Font = NSFont.FromFontName("Junction", 11);
            lblEQ8.Font = NSFont.FromFontName("Junction", 11);
            lblEQ9.Font = NSFont.FromFontName("Junction", 11);
            lblEQ10.Font = NSFont.FromFontName("Junction", 11);
            lblEQ11.Font = NSFont.FromFontName("Junction", 11);
            lblEQ12.Font = NSFont.FromFontName("Junction", 11);
            lblEQ13.Font = NSFont.FromFontName("Junction", 11);
            lblEQ14.Font = NSFont.FromFontName("Junction", 11);
            lblEQ15.Font = NSFont.FromFontName("Junction", 11);
            lblEQ16.Font = NSFont.FromFontName("Junction", 11);
            lblEQ17.Font = NSFont.FromFontName("Junction", 11);

            lblEQValue0.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue1.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue2.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue3.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue4.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue5.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue6.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue7.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue8.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue9.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue10.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue11.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue12.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue13.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue14.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue15.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue16.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue17.Font = NSFont.FromFontName("Junction", 11);
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
        }

        partial void actionAutoLevel(NSObject sender)
        {
            // Display confirmation dialog
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
            // Display confirmation dialog
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

        partial void actionSlider0ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            Console.WriteLine("EffectsWindowController - actionSlider0ChangeValue - value: {0}", slider.FloatValue);
            lblEQValue0.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ0.StringValue, slider.FloatValue);
        }

        partial void actionSlider1ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue1.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ1.StringValue, slider.FloatValue);
        }

        partial void actionSlider2ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue2.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ2.StringValue, slider.FloatValue);
        }

        partial void actionSlider3ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue3.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ3.StringValue, slider.FloatValue);
        }

        partial void actionSlider4ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue4.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ4.StringValue, slider.FloatValue);
        }

        partial void actionSlider5ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue5.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ5.StringValue, slider.FloatValue);
        }

        partial void actionSlider6ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue6.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ6.StringValue, slider.FloatValue);
        }

        partial void actionSlider7ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue7.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ7.StringValue, slider.FloatValue);
        }

        partial void actionSlider8ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue8.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ8.StringValue, slider.FloatValue);
        }

        partial void actionSlider9ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue9.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ9.StringValue, slider.FloatValue);
        }

        partial void actionSlider10ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue10.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ10.StringValue, slider.FloatValue);
        }

        partial void actionSlider11ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue11.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ11.StringValue, slider.FloatValue);
        }

        partial void actionSlider12ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue12.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ12.StringValue, slider.FloatValue);
        }

        partial void actionSlider13ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue13.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ13.StringValue, slider.FloatValue);
        }

        partial void actionSlider14ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue14.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ14.StringValue, slider.FloatValue);
        }

        partial void actionSlider15ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue15.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ15.StringValue, slider.FloatValue);
        }

        partial void actionSlider16ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue16.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ16.StringValue, slider.FloatValue);
        }

        partial void actionSlider17ChangeValue(NSObject sender)
        {
            NSSlider slider = (NSSlider)sender;
            lblEQValue17.StringValue = FormatEQValue(slider.FloatValue);
            OnSetFaderGain(lblEQ17.StringValue, slider.FloatValue);
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
                    popupPreset.LastItem.Title = preset.Name;
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

                txtName.StringValue = _preset.Name;
                sliderEQ0.FloatValue = _preset.Gain0;
                lblEQValue0.StringValue = FormatEQValue(_preset.Gain0);
                sliderEQ1.FloatValue = _preset.Gain1;
                lblEQValue1.StringValue = FormatEQValue(_preset.Gain1);
                sliderEQ2.FloatValue = _preset.Gain2;
                lblEQValue2.StringValue = FormatEQValue(_preset.Gain2);
                sliderEQ3.FloatValue = _preset.Gain3;
                lblEQValue3.StringValue = FormatEQValue(_preset.Gain3);
                sliderEQ4.FloatValue = _preset.Gain4;
                lblEQValue4.StringValue = FormatEQValue(_preset.Gain4);
                sliderEQ5.FloatValue = _preset.Gain5;
                lblEQValue5.StringValue = FormatEQValue(_preset.Gain5);
                sliderEQ6.FloatValue = _preset.Gain6;
                lblEQValue6.StringValue = FormatEQValue(_preset.Gain6);
                sliderEQ7.FloatValue = _preset.Gain7;
                lblEQValue7.StringValue = FormatEQValue(_preset.Gain7);
                sliderEQ8.FloatValue = _preset.Gain8;
                lblEQValue8.StringValue = FormatEQValue(_preset.Gain8);
                sliderEQ9.FloatValue = _preset.Gain9;
                lblEQValue9.StringValue = FormatEQValue(_preset.Gain9);
                sliderEQ10.FloatValue = _preset.Gain10;
                lblEQValue10.StringValue = FormatEQValue(_preset.Gain10);
                sliderEQ11.FloatValue = _preset.Gain11;
                lblEQValue11.StringValue = FormatEQValue(_preset.Gain11);
                sliderEQ12.FloatValue = _preset.Gain12;
                lblEQValue12.StringValue = FormatEQValue(_preset.Gain12);
                sliderEQ13.FloatValue = _preset.Gain13;
                lblEQValue13.StringValue = FormatEQValue(_preset.Gain13);
                sliderEQ14.FloatValue = _preset.Gain14;
                lblEQValue14.StringValue = FormatEQValue(_preset.Gain14);
                sliderEQ15.FloatValue = _preset.Gain15;
                lblEQValue15.StringValue = FormatEQValue(_preset.Gain15);
                sliderEQ16.FloatValue = _preset.Gain16;
                lblEQValue16.StringValue = FormatEQValue(_preset.Gain16);
                sliderEQ17.FloatValue = _preset.Gain17;
                lblEQValue17.StringValue = FormatEQValue(_preset.Gain17);
            });
        }

        #endregion

    }
}
