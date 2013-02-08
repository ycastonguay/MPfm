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
	public partial class PreferencesWindow
	{
		private global::Gtk.UIManager UIManager;
		private global::Gtk.Action actionAddFolder;
		private global::Gtk.Action actionRemoveFolder;
		private global::Gtk.Action actionResetLibrary;
		private global::Gtk.Notebook notebookSettings;
		private global::Gtk.VBox vboxGeneral;
		private global::Gtk.Label label4;
		private global::Gtk.VBox vboxAudio;
		private global::Gtk.VBox vboxLibraryFolders1;
		private global::Gtk.Label lblAudioOutput;
		private global::Gtk.Table table1;
		private global::Gtk.ComboBox comboAudioOutputDevice;
		private global::Gtk.ComboBox comboAudioSampleRate;
		private global::Gtk.Label lblAudioOutputDevice;
		private global::Gtk.Label lblAudioSampleRate;
		private global::Gtk.VBox vboxLibraryFolders2;
		private global::Gtk.Label lblAudioMixer;
		private global::Gtk.Table table2;
		private global::Gtk.HScale hscaleAudioBufferSize;
		private global::Gtk.HScale hscaleAudioUpdatePeriod;
		private global::Gtk.Label lblAudioBufferSize;
		private global::Gtk.Label lblAudioEvery1;
		private global::Gtk.Label lblAudioEvery2;
		private global::Gtk.Label lblAudioUpdatePeriod;
		private global::Gtk.Label lblMS1;
		private global::Gtk.Label lblMS2;
		private global::Gtk.SpinButton spinAudioBufferSize;
		private global::Gtk.SpinButton spinAudioUpdatePeriod;
		private global::Gtk.VBox vboxLibraryFolders3;
		private global::Gtk.Label lblAudioStatus;
		private global::Gtk.Table table3;
		private global::Gtk.Button btnResetToDefault;
		private global::Gtk.Button btnTestAudioSettings;
		private global::Gtk.Image imageAudioStatus;
		private global::Gtk.Label lblAudioStatusValue;
		private global::Gtk.Label label5;
		private global::Gtk.VBox vboxLibrary;
		private global::Gtk.VBox vboxLibraryFolders;
		private global::Gtk.Label lblFolders;
		private global::Gtk.Toolbar toolbarLibraryFolders;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeFolders;
		private global::Gtk.Label label6;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MPfm.GTK.PreferencesWindow
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.actionAddFolder = new global::Gtk.Action ("actionAddFolder", null, null, "gtk-add");
			w1.Add (this.actionAddFolder, null);
			this.actionRemoveFolder = new global::Gtk.Action ("actionRemoveFolder", null, null, "gtk-remove");
			w1.Add (this.actionRemoveFolder, null);
			this.actionResetLibrary = new global::Gtk.Action ("actionResetLibrary", null, null, "gtk-cancel");
			w1.Add (this.actionResetLibrary, null);
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.Name = "MPfm.GTK.PreferencesWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Preferences");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			// Container child MPfm.GTK.PreferencesWindow.Gtk.Container+ContainerChild
			this.notebookSettings = new global::Gtk.Notebook ();
			this.notebookSettings.CanFocus = true;
			this.notebookSettings.Name = "notebookSettings";
			this.notebookSettings.CurrentPage = 1;
			// Container child notebookSettings.Gtk.Notebook+NotebookChild
			this.vboxGeneral = new global::Gtk.VBox ();
			this.vboxGeneral.Name = "vboxGeneral";
			this.vboxGeneral.Spacing = 6;
			this.notebookSettings.Add (this.vboxGeneral);
			// Notebook tab
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("General");
			this.notebookSettings.SetTabLabel (this.vboxGeneral, this.label4);
			this.label4.ShowAll ();
			// Container child notebookSettings.Gtk.Notebook+NotebookChild
			this.vboxAudio = new global::Gtk.VBox ();
			this.vboxAudio.Name = "vboxAudio";
			this.vboxAudio.Spacing = 6;
			// Container child vboxAudio.Gtk.Box+BoxChild
			this.vboxLibraryFolders1 = new global::Gtk.VBox ();
			this.vboxLibraryFolders1.Name = "vboxLibraryFolders1";
			this.vboxLibraryFolders1.BorderWidth = ((uint)(6));
			// Container child vboxLibraryFolders1.Gtk.Box+BoxChild
			this.lblAudioOutput = new global::Gtk.Label ();
			this.lblAudioOutput.Name = "lblAudioOutput";
			this.lblAudioOutput.Xalign = 0F;
			this.lblAudioOutput.LabelProp = global::Mono.Unix.Catalog.GetString ("Output");
			this.vboxLibraryFolders1.Add (this.lblAudioOutput);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vboxLibraryFolders1 [this.lblAudioOutput]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			w3.Padding = ((uint)(3));
			// Container child vboxLibraryFolders1.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.comboAudioOutputDevice = global::Gtk.ComboBox.NewText ();
			this.comboAudioOutputDevice.Name = "comboAudioOutputDevice";
			this.table1.Add (this.comboAudioOutputDevice);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.comboAudioOutputDevice]));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.comboAudioSampleRate = global::Gtk.ComboBox.NewText ();
			this.comboAudioSampleRate.Name = "comboAudioSampleRate";
			this.table1.Add (this.comboAudioSampleRate);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.comboAudioSampleRate]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.lblAudioOutputDevice = new global::Gtk.Label ();
			this.lblAudioOutputDevice.Name = "lblAudioOutputDevice";
			this.lblAudioOutputDevice.Xalign = 0F;
			this.lblAudioOutputDevice.LabelProp = global::Mono.Unix.Catalog.GetString ("Output device:");
			this.table1.Add (this.lblAudioOutputDevice);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.lblAudioOutputDevice]));
			w6.XPadding = ((uint)(6));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.lblAudioSampleRate = new global::Gtk.Label ();
			this.lblAudioSampleRate.Name = "lblAudioSampleRate";
			this.lblAudioSampleRate.Xalign = 0F;
			this.lblAudioSampleRate.LabelProp = global::Mono.Unix.Catalog.GetString ("Sample rate:");
			this.table1.Add (this.lblAudioSampleRate);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.lblAudioSampleRate]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.XPadding = ((uint)(6));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vboxLibraryFolders1.Add (this.table1);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vboxLibraryFolders1 [this.table1]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			w8.Padding = ((uint)(6));
			this.vboxAudio.Add (this.vboxLibraryFolders1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vboxAudio [this.vboxLibraryFolders1]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child vboxAudio.Gtk.Box+BoxChild
			this.vboxLibraryFolders2 = new global::Gtk.VBox ();
			this.vboxLibraryFolders2.Name = "vboxLibraryFolders2";
			this.vboxLibraryFolders2.BorderWidth = ((uint)(6));
			// Container child vboxLibraryFolders2.Gtk.Box+BoxChild
			this.lblAudioMixer = new global::Gtk.Label ();
			this.lblAudioMixer.Name = "lblAudioMixer";
			this.lblAudioMixer.Xalign = 0F;
			this.lblAudioMixer.LabelProp = global::Mono.Unix.Catalog.GetString ("Mixer");
			this.vboxLibraryFolders2.Add (this.lblAudioMixer);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vboxLibraryFolders2 [this.lblAudioMixer]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			w10.Padding = ((uint)(3));
			// Container child vboxLibraryFolders2.Gtk.Box+BoxChild
			this.table2 = new global::Gtk.Table (((uint)(2)), ((uint)(5)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.hscaleAudioBufferSize = new global::Gtk.HScale (null);
			this.hscaleAudioBufferSize.CanFocus = true;
			this.hscaleAudioBufferSize.Name = "hscaleAudioBufferSize";
			this.hscaleAudioBufferSize.Adjustment.Upper = 100;
			this.hscaleAudioBufferSize.Adjustment.PageIncrement = 10;
			this.hscaleAudioBufferSize.Adjustment.StepIncrement = 1;
			this.hscaleAudioBufferSize.DrawValue = false;
			this.hscaleAudioBufferSize.Digits = 0;
			this.hscaleAudioBufferSize.ValuePos = ((global::Gtk.PositionType)(2));
			this.table2.Add (this.hscaleAudioBufferSize);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table2 [this.hscaleAudioBufferSize]));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hscaleAudioUpdatePeriod = new global::Gtk.HScale (null);
			this.hscaleAudioUpdatePeriod.CanFocus = true;
			this.hscaleAudioUpdatePeriod.Name = "hscaleAudioUpdatePeriod";
			this.hscaleAudioUpdatePeriod.Adjustment.Upper = 100;
			this.hscaleAudioUpdatePeriod.Adjustment.PageIncrement = 10;
			this.hscaleAudioUpdatePeriod.Adjustment.StepIncrement = 1;
			this.hscaleAudioUpdatePeriod.DrawValue = false;
			this.hscaleAudioUpdatePeriod.Digits = 0;
			this.hscaleAudioUpdatePeriod.ValuePos = ((global::Gtk.PositionType)(2));
			this.table2.Add (this.hscaleAudioUpdatePeriod);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table2 [this.hscaleAudioUpdatePeriod]));
			w12.TopAttach = ((uint)(1));
			w12.BottomAttach = ((uint)(2));
			w12.LeftAttach = ((uint)(1));
			w12.RightAttach = ((uint)(2));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.lblAudioBufferSize = new global::Gtk.Label ();
			this.lblAudioBufferSize.Name = "lblAudioBufferSize";
			this.lblAudioBufferSize.Xalign = 0F;
			this.lblAudioBufferSize.LabelProp = global::Mono.Unix.Catalog.GetString ("Buffer size:");
			this.table2.Add (this.lblAudioBufferSize);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table2 [this.lblAudioBufferSize]));
			w13.XPadding = ((uint)(6));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.lblAudioEvery1 = new global::Gtk.Label ();
			this.lblAudioEvery1.Name = "lblAudioEvery1";
			this.lblAudioEvery1.Xalign = 0F;
			this.lblAudioEvery1.LabelProp = global::Mono.Unix.Catalog.GetString ("every");
			this.table2.Add (this.lblAudioEvery1);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table2 [this.lblAudioEvery1]));
			w14.LeftAttach = ((uint)(2));
			w14.RightAttach = ((uint)(3));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.lblAudioEvery2 = new global::Gtk.Label ();
			this.lblAudioEvery2.Name = "lblAudioEvery2";
			this.lblAudioEvery2.Xalign = 0F;
			this.lblAudioEvery2.LabelProp = global::Mono.Unix.Catalog.GetString ("every");
			this.table2.Add (this.lblAudioEvery2);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table2 [this.lblAudioEvery2]));
			w15.TopAttach = ((uint)(1));
			w15.BottomAttach = ((uint)(2));
			w15.LeftAttach = ((uint)(2));
			w15.RightAttach = ((uint)(3));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.lblAudioUpdatePeriod = new global::Gtk.Label ();
			this.lblAudioUpdatePeriod.Name = "lblAudioUpdatePeriod";
			this.lblAudioUpdatePeriod.Xalign = 0F;
			this.lblAudioUpdatePeriod.LabelProp = global::Mono.Unix.Catalog.GetString ("Update period:");
			this.table2.Add (this.lblAudioUpdatePeriod);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table2 [this.lblAudioUpdatePeriod]));
			w16.TopAttach = ((uint)(1));
			w16.BottomAttach = ((uint)(2));
			w16.XPadding = ((uint)(6));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.lblMS1 = new global::Gtk.Label ();
			this.lblMS1.Name = "lblMS1";
			this.lblMS1.Xalign = 0F;
			this.lblMS1.LabelProp = global::Mono.Unix.Catalog.GetString ("ms");
			this.table2.Add (this.lblMS1);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table2 [this.lblMS1]));
			w17.LeftAttach = ((uint)(4));
			w17.RightAttach = ((uint)(5));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.lblMS2 = new global::Gtk.Label ();
			this.lblMS2.Name = "lblMS2";
			this.lblMS2.Xalign = 0F;
			this.lblMS2.LabelProp = global::Mono.Unix.Catalog.GetString ("ms");
			this.table2.Add (this.lblMS2);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table2 [this.lblMS2]));
			w18.TopAttach = ((uint)(1));
			w18.BottomAttach = ((uint)(2));
			w18.LeftAttach = ((uint)(4));
			w18.RightAttach = ((uint)(5));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.spinAudioBufferSize = new global::Gtk.SpinButton (0, 100, 1);
			this.spinAudioBufferSize.CanFocus = true;
			this.spinAudioBufferSize.Name = "spinAudioBufferSize";
			this.spinAudioBufferSize.Adjustment.PageIncrement = 10;
			this.spinAudioBufferSize.ClimbRate = 1;
			this.spinAudioBufferSize.Numeric = true;
			this.table2.Add (this.spinAudioBufferSize);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table2 [this.spinAudioBufferSize]));
			w19.LeftAttach = ((uint)(3));
			w19.RightAttach = ((uint)(4));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.spinAudioUpdatePeriod = new global::Gtk.SpinButton (0, 100, 1);
			this.spinAudioUpdatePeriod.CanFocus = true;
			this.spinAudioUpdatePeriod.Name = "spinAudioUpdatePeriod";
			this.spinAudioUpdatePeriod.Adjustment.PageIncrement = 10;
			this.spinAudioUpdatePeriod.ClimbRate = 1;
			this.spinAudioUpdatePeriod.Numeric = true;
			this.table2.Add (this.spinAudioUpdatePeriod);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table2 [this.spinAudioUpdatePeriod]));
			w20.TopAttach = ((uint)(1));
			w20.BottomAttach = ((uint)(2));
			w20.LeftAttach = ((uint)(3));
			w20.RightAttach = ((uint)(4));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vboxLibraryFolders2.Add (this.table2);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vboxLibraryFolders2 [this.table2]));
			w21.Position = 1;
			w21.Expand = false;
			w21.Fill = false;
			this.vboxAudio.Add (this.vboxLibraryFolders2);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vboxAudio [this.vboxLibraryFolders2]));
			w22.Position = 1;
			w22.Expand = false;
			w22.Fill = false;
			// Container child vboxAudio.Gtk.Box+BoxChild
			this.vboxLibraryFolders3 = new global::Gtk.VBox ();
			this.vboxLibraryFolders3.Name = "vboxLibraryFolders3";
			this.vboxLibraryFolders3.BorderWidth = ((uint)(6));
			// Container child vboxLibraryFolders3.Gtk.Box+BoxChild
			this.lblAudioStatus = new global::Gtk.Label ();
			this.lblAudioStatus.Name = "lblAudioStatus";
			this.lblAudioStatus.Xalign = 0F;
			this.lblAudioStatus.LabelProp = global::Mono.Unix.Catalog.GetString ("Status");
			this.vboxLibraryFolders3.Add (this.lblAudioStatus);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vboxLibraryFolders3 [this.lblAudioStatus]));
			w23.Position = 0;
			w23.Expand = false;
			w23.Fill = false;
			w23.Padding = ((uint)(3));
			// Container child vboxLibraryFolders3.Gtk.Box+BoxChild
			this.table3 = new global::Gtk.Table (((uint)(2)), ((uint)(3)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(6));
			this.table3.ColumnSpacing = ((uint)(6));
			// Container child table3.Gtk.Table+TableChild
			this.btnResetToDefault = new global::Gtk.Button ();
			this.btnResetToDefault.CanFocus = true;
			this.btnResetToDefault.Name = "btnResetToDefault";
			this.btnResetToDefault.UseUnderline = true;
			this.btnResetToDefault.Label = global::Mono.Unix.Catalog.GetString ("Reset to default");
			this.table3.Add (this.btnResetToDefault);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table3 [this.btnResetToDefault]));
			w24.TopAttach = ((uint)(1));
			w24.BottomAttach = ((uint)(2));
			w24.LeftAttach = ((uint)(2));
			w24.RightAttach = ((uint)(3));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.btnTestAudioSettings = new global::Gtk.Button ();
			this.btnTestAudioSettings.CanFocus = true;
			this.btnTestAudioSettings.Name = "btnTestAudioSettings";
			this.btnTestAudioSettings.UseUnderline = true;
			this.btnTestAudioSettings.Label = global::Mono.Unix.Catalog.GetString ("Test audio settings");
			this.table3.Add (this.btnTestAudioSettings);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table3 [this.btnTestAudioSettings]));
			w25.TopAttach = ((uint)(1));
			w25.BottomAttach = ((uint)(2));
			w25.LeftAttach = ((uint)(1));
			w25.RightAttach = ((uint)(2));
			w25.XOptions = ((global::Gtk.AttachOptions)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.imageAudioStatus = new global::Gtk.Image ();
			this.imageAudioStatus.Name = "imageAudioStatus";
			this.imageAudioStatus.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-ok", global::Gtk.IconSize.Menu);
			this.table3.Add (this.imageAudioStatus);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table3 [this.imageAudioStatus]));
			w26.XPadding = ((uint)(6));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.lblAudioStatusValue = new global::Gtk.Label ();
			this.lblAudioStatusValue.Name = "lblAudioStatusValue";
			this.lblAudioStatusValue.Xalign = 0F;
			this.lblAudioStatusValue.LabelProp = global::Mono.Unix.Catalog.GetString ("The audio settings haven't been changed.");
			this.table3.Add (this.lblAudioStatusValue);
			global::Gtk.Table.TableChild w27 = ((global::Gtk.Table.TableChild)(this.table3 [this.lblAudioStatusValue]));
			w27.LeftAttach = ((uint)(1));
			w27.RightAttach = ((uint)(2));
			w27.XOptions = ((global::Gtk.AttachOptions)(4));
			w27.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vboxLibraryFolders3.Add (this.table3);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(this.vboxLibraryFolders3 [this.table3]));
			w28.Position = 1;
			w28.Expand = false;
			w28.Fill = false;
			this.vboxAudio.Add (this.vboxLibraryFolders3);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vboxAudio [this.vboxLibraryFolders3]));
			w29.Position = 2;
			w29.Expand = false;
			w29.Fill = false;
			this.notebookSettings.Add (this.vboxAudio);
			global::Gtk.Notebook.NotebookChild w30 = ((global::Gtk.Notebook.NotebookChild)(this.notebookSettings [this.vboxAudio]));
			w30.Position = 1;
			// Notebook tab
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Audio");
			this.notebookSettings.SetTabLabel (this.vboxAudio, this.label5);
			this.label5.ShowAll ();
			// Container child notebookSettings.Gtk.Notebook+NotebookChild
			this.vboxLibrary = new global::Gtk.VBox ();
			this.vboxLibrary.Name = "vboxLibrary";
			this.vboxLibrary.Spacing = 6;
			// Container child vboxLibrary.Gtk.Box+BoxChild
			this.vboxLibraryFolders = new global::Gtk.VBox ();
			this.vboxLibraryFolders.Name = "vboxLibraryFolders";
			this.vboxLibraryFolders.BorderWidth = ((uint)(6));
			// Container child vboxLibraryFolders.Gtk.Box+BoxChild
			this.lblFolders = new global::Gtk.Label ();
			this.lblFolders.Name = "lblFolders";
			this.lblFolders.Xalign = 0F;
			this.lblFolders.LabelProp = global::Mono.Unix.Catalog.GetString ("Folders");
			this.vboxLibraryFolders.Add (this.lblFolders);
			global::Gtk.Box.BoxChild w31 = ((global::Gtk.Box.BoxChild)(this.vboxLibraryFolders [this.lblFolders]));
			w31.Position = 0;
			w31.Expand = false;
			w31.Fill = false;
			w31.Padding = ((uint)(3));
			// Container child vboxLibraryFolders.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='toolbarLibraryFolders'><toolitem name='actionAddFolder' action='actionAddFolder'/><toolitem name='actionRemoveFolder' action='actionRemoveFolder'/><toolitem name='actionResetLibrary' action='actionResetLibrary'/></toolbar></ui>");
			this.toolbarLibraryFolders = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbarLibraryFolders")));
			this.toolbarLibraryFolders.Name = "toolbarLibraryFolders";
			this.toolbarLibraryFolders.ShowArrow = false;
			this.vboxLibraryFolders.Add (this.toolbarLibraryFolders);
			global::Gtk.Box.BoxChild w32 = ((global::Gtk.Box.BoxChild)(this.vboxLibraryFolders [this.toolbarLibraryFolders]));
			w32.Position = 1;
			w32.Expand = false;
			w32.Fill = false;
			// Container child vboxLibraryFolders.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.treeFolders = new global::Gtk.TreeView ();
			this.treeFolders.CanFocus = true;
			this.treeFolders.Name = "treeFolders";
			this.GtkScrolledWindow.Add (this.treeFolders);
			this.vboxLibraryFolders.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w34 = ((global::Gtk.Box.BoxChild)(this.vboxLibraryFolders [this.GtkScrolledWindow]));
			w34.Position = 2;
			this.vboxLibrary.Add (this.vboxLibraryFolders);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.vboxLibrary [this.vboxLibraryFolders]));
			w35.Position = 0;
			this.notebookSettings.Add (this.vboxLibrary);
			global::Gtk.Notebook.NotebookChild w36 = ((global::Gtk.Notebook.NotebookChild)(this.notebookSettings [this.vboxLibrary]));
			w36.Position = 2;
			// Notebook tab
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Library");
			this.notebookSettings.SetTabLabel (this.vboxLibrary, this.label6);
			this.label6.ShowAll ();
			this.Add (this.notebookSettings);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 625;
			this.DefaultHeight = 462;
			this.Hide ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		}
	}
}
