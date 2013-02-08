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

namespace MPfm.GTK
{
	public partial class EffectsWindow
	{
		private global::Gtk.HBox hboxMain;
		private global::Gtk.HBox hboxFaders;
		private global::Gtk.VBox vbox3;
		private global::Gtk.VScale vscale1;
		private global::Gtk.Label label1;
		private global::Gtk.Label label1Value;
		private global::Gtk.VBox vbox4;
		private global::Gtk.VScale vscale2;
		private global::Gtk.Label label2;
		private global::Gtk.Label label2Value;
		private global::Gtk.VBox vbox5;
		private global::Gtk.VScale vscale3;
		private global::Gtk.Label label3;
		private global::Gtk.Label label3Value;
		private global::Gtk.VBox vbox6;
		private global::Gtk.VScale vscale4;
		private global::Gtk.Label label4;
		private global::Gtk.Label label4Value;
		private global::Gtk.VBox vbox7;
		private global::Gtk.VScale vscale5;
		private global::Gtk.Label label5;
		private global::Gtk.Label label5Value;
		private global::Gtk.VBox vbox8;
		private global::Gtk.VScale vscale6;
		private global::Gtk.Label label6;
		private global::Gtk.Label label6Value;
		private global::Gtk.VBox vbox9;
		private global::Gtk.VScale vscale7;
		private global::Gtk.Label label7;
		private global::Gtk.Label label7Value;
		private global::Gtk.VBox vbox10;
		private global::Gtk.VScale vscale8;
		private global::Gtk.Label label8;
		private global::Gtk.Label label8Value;
		private global::Gtk.VBox vbox11;
		private global::Gtk.VScale vscale9;
		private global::Gtk.Label label9;
		private global::Gtk.Label label9Value;
		private global::Gtk.VBox vbox12;
		private global::Gtk.VScale vscale10;
		private global::Gtk.Label label10;
		private global::Gtk.Label label10Value;
		private global::Gtk.VBox vbox13;
		private global::Gtk.VScale vscale11;
		private global::Gtk.Label label11;
		private global::Gtk.Label label11Value;
		private global::Gtk.VBox vbox14;
		private global::Gtk.VScale vscale12;
		private global::Gtk.Label label12;
		private global::Gtk.Label label12Value;
		private global::Gtk.VBox vbox15;
		private global::Gtk.VScale vscale13;
		private global::Gtk.Label label13;
		private global::Gtk.Label label13Value;
		private global::Gtk.VBox vbox16;
		private global::Gtk.VScale vscale14;
		private global::Gtk.Label label14;
		private global::Gtk.Label label14Value;
		private global::Gtk.VBox vbox17;
		private global::Gtk.VScale vscale15;
		private global::Gtk.Label label15;
		private global::Gtk.Label label15Value;
		private global::Gtk.VBox vbox18;
		private global::Gtk.VScale vscale16;
		private global::Gtk.Label label16;
		private global::Gtk.Label label16Value;
		private global::Gtk.VBox vbox19;
		private global::Gtk.VScale vscale17;
		private global::Gtk.Label label17;
		private global::Gtk.Label label17Value;
		private global::Gtk.VBox vbox20;
		private global::Gtk.VScale vscale18;
		private global::Gtk.Label label18;
		private global::Gtk.Label label18Value;
		private global::Gtk.VBox vboxPanel;
		private global::Gtk.Label lblPreset;
		private global::Gtk.ComboBox comboPreset;
		private global::Gtk.CheckButton chkEQOn;
		private global::Gtk.Label lblInformation;
		private global::Gtk.Label lblName;
		private global::Gtk.Entry txtPresetName;
		private global::Gtk.Button btnSavePreset;
		private global::Gtk.Button btnDeletePreset;
		private global::Gtk.Button btnAutoLevel;
		private global::Gtk.Button btnResetEQ;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MPfm.GTK.EffectsWindow
			this.Name = "MPfm.GTK.EffectsWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Effects");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			// Container child MPfm.GTK.EffectsWindow.Gtk.Container+ContainerChild
			this.hboxMain = new global::Gtk.HBox ();
			this.hboxMain.Name = "hboxMain";
			this.hboxMain.Spacing = 6;
			// Container child hboxMain.Gtk.Box+BoxChild
			this.hboxFaders = new global::Gtk.HBox ();
			this.hboxFaders.Name = "hboxFaders";
			this.hboxFaders.Spacing = 6;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.vscale1 = new global::Gtk.VScale (null);
			this.vscale1.CanFocus = true;
			this.vscale1.Name = "vscale1";
			this.vscale1.Adjustment.Upper = 100;
			this.vscale1.Adjustment.PageIncrement = 10;
			this.vscale1.Adjustment.StepIncrement = 1;
			this.vscale1.DrawValue = false;
			this.vscale1.Digits = 0;
			this.vscale1.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox3.Add (this.vscale1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.vscale1]));
			w1.Position = 0;
			w1.Padding = ((uint)(6));
			// Container child vbox3.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.WidthRequest = 40;
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("55 Hz");
			this.vbox3.Add (this.label1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.label1]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.label1Value = new global::Gtk.Label ();
			this.label1Value.Name = "label1Value";
			this.label1Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox3.Add (this.label1Value);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.label1Value]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			w3.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox3);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox3]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.vscale2 = new global::Gtk.VScale (null);
			this.vscale2.CanFocus = true;
			this.vscale2.Name = "vscale2";
			this.vscale2.Adjustment.Upper = 100;
			this.vscale2.Adjustment.PageIncrement = 10;
			this.vscale2.Adjustment.StepIncrement = 1;
			this.vscale2.DrawValue = false;
			this.vscale2.Digits = 0;
			this.vscale2.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox4.Add (this.vscale2);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.vscale2]));
			w5.Position = 0;
			w5.Padding = ((uint)(6));
			// Container child vbox4.Gtk.Box+BoxChild
			this.label2 = new global::Gtk.Label ();
			this.label2.WidthRequest = 40;
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("77 Hz");
			this.vbox4.Add (this.label2);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.label2]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.label2Value = new global::Gtk.Label ();
			this.label2Value.Name = "label2Value";
			this.label2Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox4.Add (this.label2Value);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.label2Value]));
			w7.Position = 2;
			w7.Expand = false;
			w7.Fill = false;
			w7.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox4);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox4]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			// Container child vbox5.Gtk.Box+BoxChild
			this.vscale3 = new global::Gtk.VScale (null);
			this.vscale3.CanFocus = true;
			this.vscale3.Name = "vscale3";
			this.vscale3.Adjustment.Upper = 100;
			this.vscale3.Adjustment.PageIncrement = 10;
			this.vscale3.Adjustment.StepIncrement = 1;
			this.vscale3.DrawValue = false;
			this.vscale3.Digits = 0;
			this.vscale3.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox5.Add (this.vscale3);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.vscale3]));
			w9.Position = 0;
			w9.Padding = ((uint)(6));
			// Container child vbox5.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.WidthRequest = 40;
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("110 Hz");
			this.vbox5.Add (this.label3);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.label3]));
			w10.Position = 1;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.label3Value = new global::Gtk.Label ();
			this.label3Value.Name = "label3Value";
			this.label3Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox5.Add (this.label3Value);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.label3Value]));
			w11.Position = 2;
			w11.Expand = false;
			w11.Fill = false;
			w11.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox5);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox5]));
			w12.Position = 2;
			w12.Expand = false;
			w12.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox6 = new global::Gtk.VBox ();
			this.vbox6.Name = "vbox6";
			this.vbox6.Spacing = 6;
			// Container child vbox6.Gtk.Box+BoxChild
			this.vscale4 = new global::Gtk.VScale (null);
			this.vscale4.CanFocus = true;
			this.vscale4.Name = "vscale4";
			this.vscale4.Adjustment.Upper = 100;
			this.vscale4.Adjustment.PageIncrement = 10;
			this.vscale4.Adjustment.StepIncrement = 1;
			this.vscale4.DrawValue = false;
			this.vscale4.Digits = 0;
			this.vscale4.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox6.Add (this.vscale4);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.vscale4]));
			w13.Position = 0;
			w13.Padding = ((uint)(6));
			// Container child vbox6.Gtk.Box+BoxChild
			this.label4 = new global::Gtk.Label ();
			this.label4.WidthRequest = 40;
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("156 Hz");
			this.vbox6.Add (this.label4);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.label4]));
			w14.Position = 1;
			w14.Expand = false;
			w14.Fill = false;
			// Container child vbox6.Gtk.Box+BoxChild
			this.label4Value = new global::Gtk.Label ();
			this.label4Value.Name = "label4Value";
			this.label4Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox6.Add (this.label4Value);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.label4Value]));
			w15.Position = 2;
			w15.Expand = false;
			w15.Fill = false;
			w15.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox6);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox6]));
			w16.Position = 3;
			w16.Expand = false;
			w16.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox7 = new global::Gtk.VBox ();
			this.vbox7.Name = "vbox7";
			this.vbox7.Spacing = 6;
			// Container child vbox7.Gtk.Box+BoxChild
			this.vscale5 = new global::Gtk.VScale (null);
			this.vscale5.CanFocus = true;
			this.vscale5.Name = "vscale5";
			this.vscale5.Adjustment.Upper = 100;
			this.vscale5.Adjustment.PageIncrement = 10;
			this.vscale5.Adjustment.StepIncrement = 1;
			this.vscale5.DrawValue = false;
			this.vscale5.Digits = 0;
			this.vscale5.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox7.Add (this.vscale5);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.vscale5]));
			w17.Position = 0;
			w17.Padding = ((uint)(6));
			// Container child vbox7.Gtk.Box+BoxChild
			this.label5 = new global::Gtk.Label ();
			this.label5.WidthRequest = 40;
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("220 Hz");
			this.vbox7.Add (this.label5);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.label5]));
			w18.Position = 1;
			w18.Expand = false;
			w18.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.label5Value = new global::Gtk.Label ();
			this.label5Value.Name = "label5Value";
			this.label5Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox7.Add (this.label5Value);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.label5Value]));
			w19.Position = 2;
			w19.Expand = false;
			w19.Fill = false;
			w19.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox7);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox7]));
			w20.Position = 4;
			w20.Expand = false;
			w20.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox8 = new global::Gtk.VBox ();
			this.vbox8.Name = "vbox8";
			this.vbox8.Spacing = 6;
			// Container child vbox8.Gtk.Box+BoxChild
			this.vscale6 = new global::Gtk.VScale (null);
			this.vscale6.CanFocus = true;
			this.vscale6.Name = "vscale6";
			this.vscale6.Adjustment.Upper = 100;
			this.vscale6.Adjustment.PageIncrement = 10;
			this.vscale6.Adjustment.StepIncrement = 1;
			this.vscale6.DrawValue = false;
			this.vscale6.Digits = 0;
			this.vscale6.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox8.Add (this.vscale6);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.vscale6]));
			w21.Position = 0;
			w21.Padding = ((uint)(6));
			// Container child vbox8.Gtk.Box+BoxChild
			this.label6 = new global::Gtk.Label ();
			this.label6.WidthRequest = 40;
			this.label6.Name = "label6";
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("311 Hz");
			this.vbox8.Add (this.label6);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.label6]));
			w22.Position = 1;
			w22.Expand = false;
			w22.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.label6Value = new global::Gtk.Label ();
			this.label6Value.Name = "label6Value";
			this.label6Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox8.Add (this.label6Value);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.label6Value]));
			w23.Position = 2;
			w23.Expand = false;
			w23.Fill = false;
			w23.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox8);
			global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox8]));
			w24.Position = 5;
			w24.Expand = false;
			w24.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox9 = new global::Gtk.VBox ();
			this.vbox9.Name = "vbox9";
			this.vbox9.Spacing = 6;
			// Container child vbox9.Gtk.Box+BoxChild
			this.vscale7 = new global::Gtk.VScale (null);
			this.vscale7.CanFocus = true;
			this.vscale7.Name = "vscale7";
			this.vscale7.Adjustment.Upper = 100;
			this.vscale7.Adjustment.PageIncrement = 10;
			this.vscale7.Adjustment.StepIncrement = 1;
			this.vscale7.DrawValue = false;
			this.vscale7.Digits = 0;
			this.vscale7.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox9.Add (this.vscale7);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.vscale7]));
			w25.Position = 0;
			w25.Padding = ((uint)(6));
			// Container child vbox9.Gtk.Box+BoxChild
			this.label7 = new global::Gtk.Label ();
			this.label7.WidthRequest = 40;
			this.label7.Name = "label7";
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("440 Hz");
			this.vbox9.Add (this.label7);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.label7]));
			w26.Position = 1;
			w26.Expand = false;
			w26.Fill = false;
			// Container child vbox9.Gtk.Box+BoxChild
			this.label7Value = new global::Gtk.Label ();
			this.label7Value.Name = "label7Value";
			this.label7Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox9.Add (this.label7Value);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.label7Value]));
			w27.Position = 2;
			w27.Expand = false;
			w27.Fill = false;
			w27.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox9);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox9]));
			w28.Position = 6;
			w28.Expand = false;
			w28.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox10 = new global::Gtk.VBox ();
			this.vbox10.Name = "vbox10";
			this.vbox10.Spacing = 6;
			// Container child vbox10.Gtk.Box+BoxChild
			this.vscale8 = new global::Gtk.VScale (null);
			this.vscale8.CanFocus = true;
			this.vscale8.Name = "vscale8";
			this.vscale8.Adjustment.Upper = 100;
			this.vscale8.Adjustment.PageIncrement = 10;
			this.vscale8.Adjustment.StepIncrement = 1;
			this.vscale8.DrawValue = false;
			this.vscale8.Digits = 0;
			this.vscale8.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox10.Add (this.vscale8);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.vscale8]));
			w29.Position = 0;
			w29.Padding = ((uint)(6));
			// Container child vbox10.Gtk.Box+BoxChild
			this.label8 = new global::Gtk.Label ();
			this.label8.WidthRequest = 40;
			this.label8.Name = "label8";
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("622 Hz");
			this.vbox10.Add (this.label8);
			global::Gtk.Box.BoxChild w30 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.label8]));
			w30.Position = 1;
			w30.Expand = false;
			w30.Fill = false;
			// Container child vbox10.Gtk.Box+BoxChild
			this.label8Value = new global::Gtk.Label ();
			this.label8Value.Name = "label8Value";
			this.label8Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox10.Add (this.label8Value);
			global::Gtk.Box.BoxChild w31 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.label8Value]));
			w31.Position = 2;
			w31.Expand = false;
			w31.Fill = false;
			w31.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox10);
			global::Gtk.Box.BoxChild w32 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox10]));
			w32.Position = 7;
			w32.Expand = false;
			w32.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox11 = new global::Gtk.VBox ();
			this.vbox11.Name = "vbox11";
			this.vbox11.Spacing = 6;
			// Container child vbox11.Gtk.Box+BoxChild
			this.vscale9 = new global::Gtk.VScale (null);
			this.vscale9.CanFocus = true;
			this.vscale9.Name = "vscale9";
			this.vscale9.Adjustment.Upper = 100;
			this.vscale9.Adjustment.PageIncrement = 10;
			this.vscale9.Adjustment.StepIncrement = 1;
			this.vscale9.DrawValue = false;
			this.vscale9.Digits = 0;
			this.vscale9.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox11.Add (this.vscale9);
			global::Gtk.Box.BoxChild w33 = ((global::Gtk.Box.BoxChild)(this.vbox11 [this.vscale9]));
			w33.Position = 0;
			w33.Padding = ((uint)(6));
			// Container child vbox11.Gtk.Box+BoxChild
			this.label9 = new global::Gtk.Label ();
			this.label9.WidthRequest = 40;
			this.label9.Name = "label9";
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("880 Hz");
			this.vbox11.Add (this.label9);
			global::Gtk.Box.BoxChild w34 = ((global::Gtk.Box.BoxChild)(this.vbox11 [this.label9]));
			w34.Position = 1;
			w34.Expand = false;
			w34.Fill = false;
			// Container child vbox11.Gtk.Box+BoxChild
			this.label9Value = new global::Gtk.Label ();
			this.label9Value.Name = "label9Value";
			this.label9Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox11.Add (this.label9Value);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.vbox11 [this.label9Value]));
			w35.Position = 2;
			w35.Expand = false;
			w35.Fill = false;
			w35.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox11);
			global::Gtk.Box.BoxChild w36 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox11]));
			w36.Position = 8;
			w36.Expand = false;
			w36.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox12 = new global::Gtk.VBox ();
			this.vbox12.Name = "vbox12";
			this.vbox12.Spacing = 6;
			// Container child vbox12.Gtk.Box+BoxChild
			this.vscale10 = new global::Gtk.VScale (null);
			this.vscale10.CanFocus = true;
			this.vscale10.Name = "vscale10";
			this.vscale10.Adjustment.Upper = 100;
			this.vscale10.Adjustment.PageIncrement = 10;
			this.vscale10.Adjustment.StepIncrement = 1;
			this.vscale10.DrawValue = false;
			this.vscale10.Digits = 0;
			this.vscale10.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox12.Add (this.vscale10);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vbox12 [this.vscale10]));
			w37.Position = 0;
			w37.Padding = ((uint)(6));
			// Container child vbox12.Gtk.Box+BoxChild
			this.label10 = new global::Gtk.Label ();
			this.label10.WidthRequest = 40;
			this.label10.Name = "label10";
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("1.2 kHz");
			this.vbox12.Add (this.label10);
			global::Gtk.Box.BoxChild w38 = ((global::Gtk.Box.BoxChild)(this.vbox12 [this.label10]));
			w38.Position = 1;
			w38.Expand = false;
			w38.Fill = false;
			// Container child vbox12.Gtk.Box+BoxChild
			this.label10Value = new global::Gtk.Label ();
			this.label10Value.Name = "label10Value";
			this.label10Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox12.Add (this.label10Value);
			global::Gtk.Box.BoxChild w39 = ((global::Gtk.Box.BoxChild)(this.vbox12 [this.label10Value]));
			w39.Position = 2;
			w39.Expand = false;
			w39.Fill = false;
			w39.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox12);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox12]));
			w40.Position = 9;
			w40.Expand = false;
			w40.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox13 = new global::Gtk.VBox ();
			this.vbox13.Name = "vbox13";
			this.vbox13.Spacing = 6;
			// Container child vbox13.Gtk.Box+BoxChild
			this.vscale11 = new global::Gtk.VScale (null);
			this.vscale11.CanFocus = true;
			this.vscale11.Name = "vscale11";
			this.vscale11.Adjustment.Upper = 100;
			this.vscale11.Adjustment.PageIncrement = 10;
			this.vscale11.Adjustment.StepIncrement = 1;
			this.vscale11.DrawValue = false;
			this.vscale11.Digits = 0;
			this.vscale11.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox13.Add (this.vscale11);
			global::Gtk.Box.BoxChild w41 = ((global::Gtk.Box.BoxChild)(this.vbox13 [this.vscale11]));
			w41.Position = 0;
			w41.Padding = ((uint)(6));
			// Container child vbox13.Gtk.Box+BoxChild
			this.label11 = new global::Gtk.Label ();
			this.label11.WidthRequest = 40;
			this.label11.Name = "label11";
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("1.8 kHz");
			this.vbox13.Add (this.label11);
			global::Gtk.Box.BoxChild w42 = ((global::Gtk.Box.BoxChild)(this.vbox13 [this.label11]));
			w42.Position = 1;
			w42.Expand = false;
			w42.Fill = false;
			// Container child vbox13.Gtk.Box+BoxChild
			this.label11Value = new global::Gtk.Label ();
			this.label11Value.Name = "label11Value";
			this.label11Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox13.Add (this.label11Value);
			global::Gtk.Box.BoxChild w43 = ((global::Gtk.Box.BoxChild)(this.vbox13 [this.label11Value]));
			w43.Position = 2;
			w43.Expand = false;
			w43.Fill = false;
			w43.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox13);
			global::Gtk.Box.BoxChild w44 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox13]));
			w44.Position = 10;
			w44.Expand = false;
			w44.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox14 = new global::Gtk.VBox ();
			this.vbox14.Name = "vbox14";
			this.vbox14.Spacing = 6;
			// Container child vbox14.Gtk.Box+BoxChild
			this.vscale12 = new global::Gtk.VScale (null);
			this.vscale12.CanFocus = true;
			this.vscale12.Name = "vscale12";
			this.vscale12.Adjustment.Upper = 100;
			this.vscale12.Adjustment.PageIncrement = 10;
			this.vscale12.Adjustment.StepIncrement = 1;
			this.vscale12.DrawValue = false;
			this.vscale12.Digits = 0;
			this.vscale12.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox14.Add (this.vscale12);
			global::Gtk.Box.BoxChild w45 = ((global::Gtk.Box.BoxChild)(this.vbox14 [this.vscale12]));
			w45.Position = 0;
			w45.Padding = ((uint)(6));
			// Container child vbox14.Gtk.Box+BoxChild
			this.label12 = new global::Gtk.Label ();
			this.label12.WidthRequest = 40;
			this.label12.Name = "label12";
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString ("2.5 kHz");
			this.vbox14.Add (this.label12);
			global::Gtk.Box.BoxChild w46 = ((global::Gtk.Box.BoxChild)(this.vbox14 [this.label12]));
			w46.Position = 1;
			w46.Expand = false;
			w46.Fill = false;
			// Container child vbox14.Gtk.Box+BoxChild
			this.label12Value = new global::Gtk.Label ();
			this.label12Value.Name = "label12Value";
			this.label12Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox14.Add (this.label12Value);
			global::Gtk.Box.BoxChild w47 = ((global::Gtk.Box.BoxChild)(this.vbox14 [this.label12Value]));
			w47.Position = 2;
			w47.Expand = false;
			w47.Fill = false;
			w47.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox14);
			global::Gtk.Box.BoxChild w48 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox14]));
			w48.Position = 11;
			w48.Expand = false;
			w48.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox15 = new global::Gtk.VBox ();
			this.vbox15.Name = "vbox15";
			this.vbox15.Spacing = 6;
			// Container child vbox15.Gtk.Box+BoxChild
			this.vscale13 = new global::Gtk.VScale (null);
			this.vscale13.CanFocus = true;
			this.vscale13.Name = "vscale13";
			this.vscale13.Adjustment.Upper = 100;
			this.vscale13.Adjustment.PageIncrement = 10;
			this.vscale13.Adjustment.StepIncrement = 1;
			this.vscale13.DrawValue = false;
			this.vscale13.Digits = 0;
			this.vscale13.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox15.Add (this.vscale13);
			global::Gtk.Box.BoxChild w49 = ((global::Gtk.Box.BoxChild)(this.vbox15 [this.vscale13]));
			w49.Position = 0;
			w49.Padding = ((uint)(6));
			// Container child vbox15.Gtk.Box+BoxChild
			this.label13 = new global::Gtk.Label ();
			this.label13.WidthRequest = 40;
			this.label13.Name = "label13";
			this.label13.LabelProp = global::Mono.Unix.Catalog.GetString ("3.5 kHz");
			this.vbox15.Add (this.label13);
			global::Gtk.Box.BoxChild w50 = ((global::Gtk.Box.BoxChild)(this.vbox15 [this.label13]));
			w50.Position = 1;
			w50.Expand = false;
			w50.Fill = false;
			// Container child vbox15.Gtk.Box+BoxChild
			this.label13Value = new global::Gtk.Label ();
			this.label13Value.Name = "label13Value";
			this.label13Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox15.Add (this.label13Value);
			global::Gtk.Box.BoxChild w51 = ((global::Gtk.Box.BoxChild)(this.vbox15 [this.label13Value]));
			w51.Position = 2;
			w51.Expand = false;
			w51.Fill = false;
			w51.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox15);
			global::Gtk.Box.BoxChild w52 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox15]));
			w52.Position = 12;
			w52.Expand = false;
			w52.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox16 = new global::Gtk.VBox ();
			this.vbox16.Name = "vbox16";
			this.vbox16.Spacing = 6;
			// Container child vbox16.Gtk.Box+BoxChild
			this.vscale14 = new global::Gtk.VScale (null);
			this.vscale14.CanFocus = true;
			this.vscale14.Name = "vscale14";
			this.vscale14.Adjustment.Upper = 100;
			this.vscale14.Adjustment.PageIncrement = 10;
			this.vscale14.Adjustment.StepIncrement = 1;
			this.vscale14.DrawValue = false;
			this.vscale14.Digits = 0;
			this.vscale14.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox16.Add (this.vscale14);
			global::Gtk.Box.BoxChild w53 = ((global::Gtk.Box.BoxChild)(this.vbox16 [this.vscale14]));
			w53.Position = 0;
			w53.Padding = ((uint)(6));
			// Container child vbox16.Gtk.Box+BoxChild
			this.label14 = new global::Gtk.Label ();
			this.label14.WidthRequest = 40;
			this.label14.Name = "label14";
			this.label14.LabelProp = global::Mono.Unix.Catalog.GetString ("5 kHz");
			this.vbox16.Add (this.label14);
			global::Gtk.Box.BoxChild w54 = ((global::Gtk.Box.BoxChild)(this.vbox16 [this.label14]));
			w54.Position = 1;
			w54.Expand = false;
			w54.Fill = false;
			// Container child vbox16.Gtk.Box+BoxChild
			this.label14Value = new global::Gtk.Label ();
			this.label14Value.Name = "label14Value";
			this.label14Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox16.Add (this.label14Value);
			global::Gtk.Box.BoxChild w55 = ((global::Gtk.Box.BoxChild)(this.vbox16 [this.label14Value]));
			w55.Position = 2;
			w55.Expand = false;
			w55.Fill = false;
			w55.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox16);
			global::Gtk.Box.BoxChild w56 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox16]));
			w56.Position = 13;
			w56.Expand = false;
			w56.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox17 = new global::Gtk.VBox ();
			this.vbox17.Name = "vbox17";
			this.vbox17.Spacing = 6;
			// Container child vbox17.Gtk.Box+BoxChild
			this.vscale15 = new global::Gtk.VScale (null);
			this.vscale15.WidthRequest = 0;
			this.vscale15.CanFocus = true;
			this.vscale15.Name = "vscale15";
			this.vscale15.Adjustment.Upper = 100;
			this.vscale15.Adjustment.PageIncrement = 10;
			this.vscale15.Adjustment.StepIncrement = 1;
			this.vscale15.DrawValue = false;
			this.vscale15.Digits = 0;
			this.vscale15.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox17.Add (this.vscale15);
			global::Gtk.Box.BoxChild w57 = ((global::Gtk.Box.BoxChild)(this.vbox17 [this.vscale15]));
			w57.Position = 0;
			w57.Padding = ((uint)(6));
			// Container child vbox17.Gtk.Box+BoxChild
			this.label15 = new global::Gtk.Label ();
			this.label15.WidthRequest = 40;
			this.label15.Name = "label15";
			this.label15.LabelProp = global::Mono.Unix.Catalog.GetString ("7 kHz");
			this.vbox17.Add (this.label15);
			global::Gtk.Box.BoxChild w58 = ((global::Gtk.Box.BoxChild)(this.vbox17 [this.label15]));
			w58.Position = 1;
			w58.Expand = false;
			w58.Fill = false;
			// Container child vbox17.Gtk.Box+BoxChild
			this.label15Value = new global::Gtk.Label ();
			this.label15Value.Name = "label15Value";
			this.label15Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox17.Add (this.label15Value);
			global::Gtk.Box.BoxChild w59 = ((global::Gtk.Box.BoxChild)(this.vbox17 [this.label15Value]));
			w59.Position = 2;
			w59.Expand = false;
			w59.Fill = false;
			w59.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox17);
			global::Gtk.Box.BoxChild w60 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox17]));
			w60.Position = 14;
			w60.Expand = false;
			w60.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox18 = new global::Gtk.VBox ();
			this.vbox18.Name = "vbox18";
			this.vbox18.Spacing = 6;
			// Container child vbox18.Gtk.Box+BoxChild
			this.vscale16 = new global::Gtk.VScale (null);
			this.vscale16.WidthRequest = 0;
			this.vscale16.CanFocus = true;
			this.vscale16.Name = "vscale16";
			this.vscale16.Adjustment.Upper = 100;
			this.vscale16.Adjustment.PageIncrement = 10;
			this.vscale16.Adjustment.StepIncrement = 1;
			this.vscale16.DrawValue = false;
			this.vscale16.Digits = 0;
			this.vscale16.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox18.Add (this.vscale16);
			global::Gtk.Box.BoxChild w61 = ((global::Gtk.Box.BoxChild)(this.vbox18 [this.vscale16]));
			w61.Position = 0;
			w61.Padding = ((uint)(6));
			// Container child vbox18.Gtk.Box+BoxChild
			this.label16 = new global::Gtk.Label ();
			this.label16.WidthRequest = 40;
			this.label16.Name = "label16";
			this.label16.LabelProp = global::Mono.Unix.Catalog.GetString ("10 kHz");
			this.vbox18.Add (this.label16);
			global::Gtk.Box.BoxChild w62 = ((global::Gtk.Box.BoxChild)(this.vbox18 [this.label16]));
			w62.Position = 1;
			w62.Expand = false;
			w62.Fill = false;
			// Container child vbox18.Gtk.Box+BoxChild
			this.label16Value = new global::Gtk.Label ();
			this.label16Value.Name = "label16Value";
			this.label16Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox18.Add (this.label16Value);
			global::Gtk.Box.BoxChild w63 = ((global::Gtk.Box.BoxChild)(this.vbox18 [this.label16Value]));
			w63.Position = 2;
			w63.Expand = false;
			w63.Fill = false;
			w63.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox18);
			global::Gtk.Box.BoxChild w64 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox18]));
			w64.Position = 15;
			w64.Expand = false;
			w64.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox19 = new global::Gtk.VBox ();
			this.vbox19.Name = "vbox19";
			this.vbox19.Spacing = 6;
			// Container child vbox19.Gtk.Box+BoxChild
			this.vscale17 = new global::Gtk.VScale (null);
			this.vscale17.WidthRequest = 0;
			this.vscale17.CanFocus = true;
			this.vscale17.Name = "vscale17";
			this.vscale17.Adjustment.Upper = 100;
			this.vscale17.Adjustment.PageIncrement = 10;
			this.vscale17.Adjustment.StepIncrement = 1;
			this.vscale17.DrawValue = false;
			this.vscale17.Digits = 0;
			this.vscale17.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox19.Add (this.vscale17);
			global::Gtk.Box.BoxChild w65 = ((global::Gtk.Box.BoxChild)(this.vbox19 [this.vscale17]));
			w65.Position = 0;
			w65.Padding = ((uint)(6));
			// Container child vbox19.Gtk.Box+BoxChild
			this.label17 = new global::Gtk.Label ();
			this.label17.WidthRequest = 40;
			this.label17.Name = "label17";
			this.label17.LabelProp = global::Mono.Unix.Catalog.GetString ("14 kHz");
			this.vbox19.Add (this.label17);
			global::Gtk.Box.BoxChild w66 = ((global::Gtk.Box.BoxChild)(this.vbox19 [this.label17]));
			w66.Position = 1;
			w66.Expand = false;
			w66.Fill = false;
			// Container child vbox19.Gtk.Box+BoxChild
			this.label17Value = new global::Gtk.Label ();
			this.label17Value.Name = "label17Value";
			this.label17Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox19.Add (this.label17Value);
			global::Gtk.Box.BoxChild w67 = ((global::Gtk.Box.BoxChild)(this.vbox19 [this.label17Value]));
			w67.Position = 2;
			w67.Expand = false;
			w67.Fill = false;
			w67.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox19);
			global::Gtk.Box.BoxChild w68 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox19]));
			w68.Position = 16;
			w68.Expand = false;
			w68.Fill = false;
			// Container child hboxFaders.Gtk.Box+BoxChild
			this.vbox20 = new global::Gtk.VBox ();
			this.vbox20.Name = "vbox20";
			this.vbox20.Spacing = 6;
			// Container child vbox20.Gtk.Box+BoxChild
			this.vscale18 = new global::Gtk.VScale (null);
			this.vscale18.WidthRequest = 0;
			this.vscale18.CanFocus = true;
			this.vscale18.Name = "vscale18";
			this.vscale18.Adjustment.Upper = 100;
			this.vscale18.Adjustment.PageIncrement = 10;
			this.vscale18.Adjustment.StepIncrement = 1;
			this.vscale18.DrawValue = false;
			this.vscale18.Digits = 0;
			this.vscale18.ValuePos = ((global::Gtk.PositionType)(2));
			this.vbox20.Add (this.vscale18);
			global::Gtk.Box.BoxChild w69 = ((global::Gtk.Box.BoxChild)(this.vbox20 [this.vscale18]));
			w69.Position = 0;
			w69.Padding = ((uint)(6));
			// Container child vbox20.Gtk.Box+BoxChild
			this.label18 = new global::Gtk.Label ();
			this.label18.WidthRequest = 40;
			this.label18.Name = "label18";
			this.label18.LabelProp = global::Mono.Unix.Catalog.GetString ("20 kHz");
			this.vbox20.Add (this.label18);
			global::Gtk.Box.BoxChild w70 = ((global::Gtk.Box.BoxChild)(this.vbox20 [this.label18]));
			w70.Position = 1;
			w70.Expand = false;
			w70.Fill = false;
			// Container child vbox20.Gtk.Box+BoxChild
			this.label18Value = new global::Gtk.Label ();
			this.label18Value.Name = "label18Value";
			this.label18Value.LabelProp = global::Mono.Unix.Catalog.GetString ("0 dB");
			this.vbox20.Add (this.label18Value);
			global::Gtk.Box.BoxChild w71 = ((global::Gtk.Box.BoxChild)(this.vbox20 [this.label18Value]));
			w71.Position = 2;
			w71.Expand = false;
			w71.Fill = false;
			w71.Padding = ((uint)(3));
			this.hboxFaders.Add (this.vbox20);
			global::Gtk.Box.BoxChild w72 = ((global::Gtk.Box.BoxChild)(this.hboxFaders [this.vbox20]));
			w72.Position = 17;
			w72.Expand = false;
			w72.Fill = false;
			this.hboxMain.Add (this.hboxFaders);
			global::Gtk.Box.BoxChild w73 = ((global::Gtk.Box.BoxChild)(this.hboxMain [this.hboxFaders]));
			w73.Position = 0;
			w73.Expand = false;
			w73.Fill = false;
			w73.Padding = ((uint)(6));
			// Container child hboxMain.Gtk.Box+BoxChild
			this.vboxPanel = new global::Gtk.VBox ();
			this.vboxPanel.Name = "vboxPanel";
			this.vboxPanel.Spacing = 6;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.lblPreset = new global::Gtk.Label ();
			this.lblPreset.Name = "lblPreset";
			this.lblPreset.LabelProp = global::Mono.Unix.Catalog.GetString ("Preset");
			this.vboxPanel.Add (this.lblPreset);
			global::Gtk.Box.BoxChild w74 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.lblPreset]));
			w74.Position = 0;
			w74.Expand = false;
			w74.Fill = false;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.comboPreset = global::Gtk.ComboBox.NewText ();
			this.comboPreset.Name = "comboPreset";
			this.vboxPanel.Add (this.comboPreset);
			global::Gtk.Box.BoxChild w75 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.comboPreset]));
			w75.Position = 1;
			w75.Expand = false;
			w75.Fill = false;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.chkEQOn = new global::Gtk.CheckButton ();
			this.chkEQOn.CanFocus = true;
			this.chkEQOn.Name = "chkEQOn";
			this.chkEQOn.Label = global::Mono.Unix.Catalog.GetString ("EQ On");
			this.chkEQOn.DrawIndicator = true;
			this.chkEQOn.UseUnderline = true;
			this.vboxPanel.Add (this.chkEQOn);
			global::Gtk.Box.BoxChild w76 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.chkEQOn]));
			w76.Position = 2;
			w76.Expand = false;
			w76.Fill = false;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.lblInformation = new global::Gtk.Label ();
			this.lblInformation.Name = "lblInformation";
			this.lblInformation.LabelProp = global::Mono.Unix.Catalog.GetString ("Information");
			this.vboxPanel.Add (this.lblInformation);
			global::Gtk.Box.BoxChild w77 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.lblInformation]));
			w77.Position = 3;
			w77.Expand = false;
			w77.Fill = false;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.lblName = new global::Gtk.Label ();
			this.lblName.Name = "lblName";
			this.lblName.Xalign = 0F;
			this.lblName.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			this.vboxPanel.Add (this.lblName);
			global::Gtk.Box.BoxChild w78 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.lblName]));
			w78.Position = 4;
			w78.Expand = false;
			w78.Fill = false;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.txtPresetName = new global::Gtk.Entry ();
			this.txtPresetName.CanFocus = true;
			this.txtPresetName.Name = "txtPresetName";
			this.txtPresetName.IsEditable = true;
			this.txtPresetName.InvisibleChar = '●';
			this.vboxPanel.Add (this.txtPresetName);
			global::Gtk.Box.BoxChild w79 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.txtPresetName]));
			w79.Position = 5;
			w79.Expand = false;
			w79.Fill = false;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.btnSavePreset = new global::Gtk.Button ();
			this.btnSavePreset.CanFocus = true;
			this.btnSavePreset.Name = "btnSavePreset";
			this.btnSavePreset.UseUnderline = true;
			this.btnSavePreset.Label = global::Mono.Unix.Catalog.GetString ("Save Preset");
			this.vboxPanel.Add (this.btnSavePreset);
			global::Gtk.Box.BoxChild w80 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.btnSavePreset]));
			w80.Position = 6;
			w80.Expand = false;
			w80.Fill = false;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.btnDeletePreset = new global::Gtk.Button ();
			this.btnDeletePreset.CanFocus = true;
			this.btnDeletePreset.Name = "btnDeletePreset";
			this.btnDeletePreset.UseUnderline = true;
			this.btnDeletePreset.Label = global::Mono.Unix.Catalog.GetString ("Delete Preset");
			this.vboxPanel.Add (this.btnDeletePreset);
			global::Gtk.Box.BoxChild w81 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.btnDeletePreset]));
			w81.Position = 7;
			w81.Expand = false;
			w81.Fill = false;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.btnAutoLevel = new global::Gtk.Button ();
			this.btnAutoLevel.CanFocus = true;
			this.btnAutoLevel.Name = "btnAutoLevel";
			this.btnAutoLevel.UseUnderline = true;
			this.btnAutoLevel.Label = global::Mono.Unix.Catalog.GetString ("Auto Level");
			this.vboxPanel.Add (this.btnAutoLevel);
			global::Gtk.Box.BoxChild w82 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.btnAutoLevel]));
			w82.Position = 8;
			w82.Expand = false;
			w82.Fill = false;
			// Container child vboxPanel.Gtk.Box+BoxChild
			this.btnResetEQ = new global::Gtk.Button ();
			this.btnResetEQ.CanFocus = true;
			this.btnResetEQ.Name = "btnResetEQ";
			this.btnResetEQ.UseUnderline = true;
			this.btnResetEQ.Label = global::Mono.Unix.Catalog.GetString ("Reset EQ");
			this.vboxPanel.Add (this.btnResetEQ);
			global::Gtk.Box.BoxChild w83 = ((global::Gtk.Box.BoxChild)(this.vboxPanel [this.btnResetEQ]));
			w83.Position = 9;
			w83.Expand = false;
			w83.Fill = false;
			this.hboxMain.Add (this.vboxPanel);
			global::Gtk.Box.BoxChild w84 = ((global::Gtk.Box.BoxChild)(this.hboxMain [this.vboxPanel]));
			w84.Position = 1;
			this.Add (this.hboxMain);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 1032;
			this.DefaultHeight = 697;
			this.Hide ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		}
	}
}
