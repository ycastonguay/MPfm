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
using MPfm.OSX.Classes.Objects;
using MPfm.OSX.Classes.Helpers;
using MPfm.OSX.Classes.Controls;
using System.Reflection;

namespace MPfm.OSX
{
    public partial class EffectsWindowController : BaseWindowController, IDesktopEffectsView
    {
        private List<EQPreset> _presets = new List<EQPreset>();
        private EQPreset _preset;

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
            ShowWindowCentered();
        }
        
        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            LoadButtons();
            LoadFontsAndImages();
            LoadFaders();
            LoadTableView();
            EnablePresetDetails(false);
            EnableFaders(false);

            OnViewReady(this);
        }

        private void LoadTableView()
        {
            tablePresets.RowHeight = 20;
            tablePresets.WeakDelegate = this;
            tablePresets.WeakDataSource = this;
        }  

        private void LoadButtons()
        {
            btnAddPreset.RoundedRadius = 0;
            btnAddPreset.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnAddPreset.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnAddPreset.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnAddPreset.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
            btnRemovePreset.RoundedRadius = 0;
            btnRemovePreset.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnRemovePreset.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnRemovePreset.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnRemovePreset.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
        }

        private void LoadFontsAndImages()
        {
            //viewBackgroundInformation.IsEnabled = false;
            //viewBackgroundFaders.IsEnabled = false;

            viewBackgroundPreset.HeaderHeight = 25;
            viewBackgroundInformation.HeaderHeight = 25;
            viewTitle.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewTitle.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewEqualizer.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewEqualizer.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewPresetsHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewPresetsHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;

            viewBackground.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewBackground.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            viewBackgroundPreset.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewBackgroundPreset.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            viewBackgroundPreset.HeaderColor1 = GlobalTheme.PanelHeader2Color1;
            viewBackgroundPreset.HeaderColor2 = GlobalTheme.PanelHeader2Color2;
            viewBackgroundPreset.IsHeaderVisible = true;
            viewBackgroundInformation.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewBackgroundInformation.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            viewBackgroundInformation.HeaderColor1 = GlobalTheme.PanelHeader2Color1;
            viewBackgroundInformation.HeaderColor2 = GlobalTheme.PanelHeader2Color2;
            viewBackgroundInformation.IsHeaderVisible = true;

            //popupPreset.Font = NSFont.FromFontName("Roboto", 11f);
            lblTitle.Font = NSFont.FromFontName("Roboto Light", 16f);
            lblEqualizer.Font = NSFont.FromFontName("Roboto Light", 13f);
            lblName.Font = NSFont.FromFontName("Roboto", 11f);
            lblTitleInformation.Font = NSFont.FromFontName("Roboto Light", 13f);
            lblTitlePreset.Font = NSFont.FromFontName("Roboto Light", 13f);
            lblEQOn.Font = NSFont.FromFontName("Roboto", 11f);
            lblScalePlus6.Font = NSFont.FromFontName("Roboto", 11f);
            lblScale0.Font = NSFont.FromFontName("Roboto", 11f);
            lblScaleMinus6.Font = NSFont.FromFontName("Roboto", 11f);
            txtName.Font = NSFont.FromFontName("Roboto", 12f);

            btnAutoLevel.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_reset");
            btnSavePreset.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_save");
            btnReset.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_reset");
            btnAddPreset.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_add");
            btnRemovePreset.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_delete");
        }

        private void LoadFaders()
        {
            for(int a = 0; a < 18; a++)
                ConfigureFader(a);

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

        private void EnablePresetDetails(bool enable)
        {
            txtName.Enabled = enable;
            btnSavePreset.Enabled = enable;
            btnAutoLevel.Enabled = enable;
            btnReset.Enabled = enable;
        }

        private void EnableFaders(bool enable)
        {
            fader0.Enabled = enable;
            fader1.Enabled = enable;
            fader2.Enabled = enable;
            fader3.Enabled = enable;
            fader4.Enabled = enable;
            fader5.Enabled = enable;
            fader6.Enabled = enable;
            fader7.Enabled = enable;
            fader8.Enabled = enable;
            fader9.Enabled = enable;
            fader10.Enabled = enable;
            fader11.Enabled = enable;
            fader12.Enabled = enable;
            fader13.Enabled = enable;
            fader14.Enabled = enable;
            fader15.Enabled = enable;
            fader16.Enabled = enable;
            fader17.Enabled = enable;

            float alpha = enable ? 1 : 0.5f;
            fader0.AlphaValue = alpha;
            fader1.AlphaValue = alpha;
            fader2.AlphaValue = alpha;
            fader3.AlphaValue = alpha;
            fader4.AlphaValue = alpha;
            fader5.AlphaValue = alpha;
            fader6.AlphaValue = alpha;
            fader7.AlphaValue = alpha;
            fader8.AlphaValue = alpha;
            fader9.AlphaValue = alpha;
            fader10.AlphaValue = alpha;
            fader11.AlphaValue = alpha;
            fader12.AlphaValue = alpha;
            fader13.AlphaValue = alpha;
            fader14.AlphaValue = alpha;
            fader15.AlphaValue = alpha;
            fader16.AlphaValue = alpha;
            fader17.AlphaValue = alpha;
        }

        private void ResetFaderValuesAndPresetDetails()
        {
            txtName.StringValue = string.Empty;
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

        partial void actionEQOnChange(NSObject sender)
        {
            OnBypassEqualizer();
        }

        partial void actionNameChanged(NSObject sender)
        {
        }

        partial void actionAddPreset(NSObject sender)
        {
            OnAddPreset();
        }

        partial void actionRemovePreset(NSObject sender)
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
                    EnableFaders(false);
                    EnablePresetDetails(false);
                    ResetFaderValuesAndPresetDetails();
                    _preset = null;
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => {
                    NSApplication.SharedApplication.StopModal();
                };
                alert.RunModal();
            }
        }

        partial void actionSavePreset(NSObject sender)
        {
            OnSavePreset(txtName.StringValue);
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
                    OnNormalizePreset();
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => {
                    NSApplication.SharedApplication.StopModal();
                };
                alert.RunModal();
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

        [Export ("numberOfRowsInTableView:")]
        public int GetRowCount(NSTableView tableView)
        {
            return _presets.Count;
        }

        [Export ("tableView:dataCellForTableColumn:row:")]
        public NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            if (tableColumn.Identifier.ToString() == "columnName")
                return new NSString(_presets [row].Name);
            else
                return new NSString();
        }

        [Export ("tableViewSelectionDidChange:")]
        public void SelectionDidChange(NSNotification notification)
        {
            EnablePresetDetails(tablePresets.SelectedRow >= 0);
            EnableFaders(tablePresets.SelectedRow >= 0);
            if (tablePresets.SelectedRow < 0)
            {
                ResetFaderValuesAndPresetDetails();
                return;
            }

            var id = _presets[tablePresets.SelectedRow].EQPresetId;
            OnLoadPreset(id); // EqualizerPresets
            OnChangePreset(id); // EqualizerPresetDetails
        }

        [Export ("tableView:viewForTableColumn:row:")]
        public NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            NSTableCellView view;
            if(tableColumn.Identifier.ToString() == "columnName")
            {
                view = (NSTableCellView)tableView.MakeView("cellName", this);
                view.TextField.StringValue = _presets[row].Name;
            }
            else
            {
                view = (NSTableCellView)tableView.MakeView("cellEqualizer", this);
            }

            view.TextField.Font = NSFont.FromFontName("Roboto", 12);

            return view;
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
            InvokeOnMainThread(delegate 
            {
                _presets = presets.ToList();
                                        //int row = tablePresets.SelectedRow;
                                        //var presetId = row >= 0 && _presets.Count > row ? _presets[row].EQPresetId : Guid.Empty;
                tablePresets.ReloadData();
                if (_preset != null)
                {
                    int newRow = _presets.FindIndex(x => x.EQPresetId == _preset.EQPresetId);
                    if (newRow >= 0)
                        tablePresets.SelectRow(newRow, false);
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
            _preset = preset;
            InvokeOnMainThread(delegate {
                int row = _presets.FindIndex(x => x.EQPresetId == _preset.EQPresetId);
                if(row >= 0)
                    tablePresets.SelectRows(NSIndexSet.FromIndex(row), false);

                EnableFaders(true);
                EnablePresetDetails(true);

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
