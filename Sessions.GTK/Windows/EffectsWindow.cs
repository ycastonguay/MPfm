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
using System.Text;
using Pango;
using Sessions.MVP;
using Sessions.MVP.Views;
using Sessions.Player.Objects;
using Gtk;

namespace MPfm.GTK.Windows
{
	/// <summary>
	/// Effects window.
	/// </summary>
	public partial class EffectsWindow : BaseWindow, IDesktopEffectsView
	{
        EQPreset _preset;
        Gtk.ListStore _storePresets = null;

		public EffectsWindow(Action<IBaseView> onViewReady) : 
				base(Gtk.WindowType.Toplevel, onViewReady)
		{
			this.Build();
			SetFontProperties();
            onViewReady(this);
            //btnRefreshDeviceList.GrabFocus(); // the list view changes color when focused by default. it annoys me!
            this.Center();
            this.Show();
		}
		
		private void SetFontProperties()
		{				
			// Get default font name
			string defaultFontName = this.label1.Style.FontDescription.Family;
			string label1FontName = defaultFontName + " 8";
			this.label1.ModifyFont(FontDescription.FromString(label1FontName));
			this.label2.ModifyFont(FontDescription.FromString(label1FontName));
			this.label3.ModifyFont(FontDescription.FromString(label1FontName));
			this.label4.ModifyFont(FontDescription.FromString(label1FontName));
			this.label5.ModifyFont(FontDescription.FromString(label1FontName));
			this.label6.ModifyFont(FontDescription.FromString(label1FontName));
			this.label7.ModifyFont(FontDescription.FromString(label1FontName));
			this.label8.ModifyFont(FontDescription.FromString(label1FontName));
			this.label9.ModifyFont(FontDescription.FromString(label1FontName));
			this.label10.ModifyFont(FontDescription.FromString(label1FontName));
			this.label11.ModifyFont(FontDescription.FromString(label1FontName));
			this.label12.ModifyFont(FontDescription.FromString(label1FontName));
			this.label13.ModifyFont(FontDescription.FromString(label1FontName));
			this.label14.ModifyFont(FontDescription.FromString(label1FontName));
			this.label15.ModifyFont(FontDescription.FromString(label1FontName));
			this.label16.ModifyFont(FontDescription.FromString(label1FontName));
			this.label17.ModifyFont(FontDescription.FromString(label1FontName));
			this.label18.ModifyFont(FontDescription.FromString(label1FontName));			
			this.label1Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label2Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label3Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label4Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label5Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label6Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label7Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label8Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label9Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label10Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label11Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label12Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label13Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label14Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label15Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label16Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label17Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label18Value.ModifyFont(FontDescription.FromString(label1FontName));			
		}

        protected void OnClickSavePreset(object sender, EventArgs e)
        {
            OnSavePreset(txtPresetName.Text);
        }

        protected void OnClickDeletePreset(object sender, EventArgs e)
        {
            //OnDeletePreset(
        }

        protected void OnClickNormalizePreset(object sender, EventArgs e)
        {
            MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.OkCancel, "Are you sure you wish to normalize this preset?");
            md.Response += (o, args) => {
                if(args.ResponseId == ResponseType.Ok)
                    OnNormalizePreset();
            };
            md.Run();
            md.Destroy();
        }

        protected void OnClickResetPreset(object sender, EventArgs e)
        {
            MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.OkCancel, "Are you sure you wish to reset this preset?");
            md.Response += (o, args) => {
                if(args.ResponseId == ResponseType.Ok)
                    OnResetPreset();                    
            };
            md.Run();
            md.Destroy();
        }

        protected void OnClickNewPreset(object sender, EventArgs e)
        {

        }

        protected void OnEQOnToggled(object sender, EventArgs e)
        {
            OnBypassEqualizer();
        }

        protected void OnScaleChangeValue1(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            //string index = vscale.Name.Replace("vscale", "");
            //Console.WriteLine("EffectsWindow - OnScaleChangeValue - value: {0} name: {1} index: {2}", vscale.Value, vscale.Name, index);
            label1Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label1.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue2(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label2Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label2.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue3(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label3Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label3.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue4(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label4Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label4.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue5(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label5Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label5.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue6(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label6Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label6.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue7(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label7Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label7.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue8(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label8Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label8.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue9(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label9Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label9.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue10(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label10Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label10.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue11(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label11Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label11.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue12(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label12Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label12.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue13(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label13Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label13.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue14(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label14Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label14.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue15(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label15Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label15.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue16(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label16Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label16.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue17(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label17Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label17.Text, (float)vscale.Value);
        }

        protected void OnScaleChangeValue18(object o, Gtk.ChangeValueArgs args)
        {
            var vscale = (Gtk.VScale)o;
            label18Value.Text = FormatEQValue(vscale.Value);
            OnSetFaderGain(label18.Text, (float)vscale.Value);
        }

        private string FormatEQValue(double value)
        {
            string strValue = string.Empty;
            if(value > 0)
                strValue = "+" + value.ToString("0.0").Replace(",",".") + " dB";
            else
                strValue = value.ToString("0.0").Replace(",",".") + " dB";
            return strValue;
        }

        #region IEqualizerPresetsView implementation

        public System.Action OnBypassEqualizer { get; set; }
        public Action<float> OnSetVolume { get; set; }
        public System.Action OnAddPreset { get; set; }
        public Action<Guid> OnLoadPreset { get; set; }
        public Action<Guid> OnEditPreset { get; set; }
        public Action<Guid> OnDeletePreset { get; set; }

        public void EqualizerPresetsError(Exception ex)
        {
            Gtk.Application.Invoke(delegate{            
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the EqualizerPresets component:");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);                                                               
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, sb.ToString());
                md.Run();
                md.Destroy();
            });
        }

        public void RefreshPresets(IEnumerable<EQPreset> presets, Guid selectedPresetId, bool isEQBypassed)
        {
            Console.WriteLine("EffectsWindow - RefreshPresets - presets.Count: {0} selectedPresetId: {1} isEQBypassed: {2}", presets.Count(), selectedPresetId, isEQBypassed);
            Gtk.Application.Invoke(delegate{  
                chkEQOn.Active = isEQBypassed;

                _storePresets = new ListStore(typeof(string));   
                foreach(var preset in presets)
                    _storePresets.AppendValues(preset.Name);
                comboPreset.Model = _storePresets;   
            });
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
            // Not used on desktop
        }

        public void RefreshVolume(float volume)
        {
            // Not used on desktop
        }

        #endregion

        #region IEqualizerPresetDetailsView implementation

        public Action<Guid> OnChangePreset { get; set; }
        public System.Action OnResetPreset { get; set; }
        public System.Action OnNormalizePreset { get; set; }
        public System.Action OnRevertPreset { get; set; }
        public Action<string> OnSavePreset { get; set; }
        public Action<string, float> OnSetFaderGain { get; set; }
		public Action<Guid> OnDuplicatePreset { get; set; }
		public Action<Guid, string> OnExportPreset { get; set; }

        public void EqualizerPresetDetailsError(Exception ex)
        {
            Gtk.Application.Invoke(delegate{            
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the EqualizerPresetDetails component:");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);                                                               
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, sb.ToString());
                md.Run();
                md.Destroy();
            });
        }

        public void ShowMessage(string title, string message)
        {
        }

        public void RefreshPreset(EQPreset preset)
        {
            Console.WriteLine("EffectsWindow - RefreshPreset");
            Gtk.Application.Invoke(delegate{            
                _preset = preset;

                txtPresetName.Text = preset.Name;
                label1Value.Text = FormatEQValue(preset.Gain0);
                vscale1.Value = preset.Gain0;
                label2Value.Text = FormatEQValue(preset.Gain1);
                vscale2.Value = preset.Gain1;
                label3Value.Text = FormatEQValue(preset.Gain2);
                vscale3.Value = preset.Gain2;
                label4Value.Text = FormatEQValue(preset.Gain3);
                vscale4.Value = preset.Gain3;
                label5Value.Text = FormatEQValue(preset.Gain4);
                vscale5.Value = preset.Gain4;
                label6Value.Text = FormatEQValue(preset.Gain5);
                vscale6.Value = preset.Gain5;
                label7Value.Text = FormatEQValue(preset.Gain6);
                vscale7.Value = preset.Gain6;
                label8Value.Text = FormatEQValue(preset.Gain7);
                vscale8.Value = preset.Gain7;
                label9Value.Text = FormatEQValue(preset.Gain8);
                vscale9.Value = preset.Gain8;
                label10Value.Text = FormatEQValue(preset.Gain9);
                vscale10.Value = preset.Gain9;
                label11Value.Text = FormatEQValue(preset.Gain10);
                vscale11.Value = preset.Gain10;
                label12Value.Text = FormatEQValue(preset.Gain11);
                vscale12.Value = preset.Gain11;
                label13Value.Text = FormatEQValue(preset.Gain12);
                vscale13.Value = preset.Gain12;
                label14Value.Text = FormatEQValue(preset.Gain13);
                vscale14.Value = preset.Gain13;
                label15Value.Text = FormatEQValue(preset.Gain14);
                vscale15.Value = preset.Gain14;
                label16Value.Text = FormatEQValue(preset.Gain15);
                vscale16.Value = preset.Gain15;
                label17Value.Text = FormatEQValue(preset.Gain16);
                vscale17.Value = preset.Gain16;
                label18Value.Text = FormatEQValue(preset.Gain17);
                vscale18.Value = preset.Gain17;
            });
        }

        #endregion
	}
}
