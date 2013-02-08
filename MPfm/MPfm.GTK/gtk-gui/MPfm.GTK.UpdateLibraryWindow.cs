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
	public partial class UpdateLibraryWindow
	{
		private global::Gtk.UIManager UIManager;
		private global::Gtk.Action actionOK;
		private global::Gtk.Action actionCancel;
		private global::Gtk.Action actionSaveLog;
		private global::Gtk.VBox vboxMain;
		private global::Gtk.Toolbar toolbar;
		private global::Gtk.VBox vboxMain1;
		private global::Gtk.VBox vbox2;
		private global::Gtk.Label lblTitle;
		private global::Gtk.Label lblSubtitle;
		private global::Gtk.ProgressBar progressbar;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Label lblTimeElapsed;
		private global::Gtk.Label lblPercentage;
		private global::Gtk.Label lblEstimatedTimeLeft;
		private global::Gtk.Label lblErrorLog;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TextView textviewErrorLog;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MPfm.GTK.UpdateLibraryWindow
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.actionOK = new global::Gtk.Action ("actionOK", global::Mono.Unix.Catalog.GetString ("OK"), null, "gtk-apply");
			this.actionOK.Sensitive = false;
			this.actionOK.ShortLabel = global::Mono.Unix.Catalog.GetString ("OK");
			w1.Add (this.actionOK, null);
			this.actionCancel = new global::Gtk.Action ("actionCancel", null, null, "gtk-cancel");
			w1.Add (this.actionCancel, null);
			this.actionSaveLog = new global::Gtk.Action ("actionSaveLog", global::Mono.Unix.Catalog.GetString ("Save Log"), null, "gtk-floppy");
			this.actionSaveLog.Sensitive = false;
			this.actionSaveLog.ShortLabel = global::Mono.Unix.Catalog.GetString ("Save Log");
			w1.Add (this.actionSaveLog, null);
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.Name = "MPfm.GTK.UpdateLibraryWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Update Library");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.Modal = true;
			// Container child MPfm.GTK.UpdateLibraryWindow.Gtk.Container+ContainerChild
			this.vboxMain = new global::Gtk.VBox ();
			this.vboxMain.Name = "vboxMain";
			this.vboxMain.Spacing = 6;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='toolbar'><toolitem name='actionOK' action='actionOK'/><toolitem name='actionCancel' action='actionCancel'/><toolitem name='actionSaveLog' action='actionSaveLog'/><toolitem/><toolitem/><toolitem/></toolbar></ui>");
			this.toolbar = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbar")));
			this.toolbar.Name = "toolbar";
			this.toolbar.ShowArrow = false;
			this.toolbar.ToolbarStyle = ((global::Gtk.ToolbarStyle)(0));
			this.vboxMain.Add (this.toolbar);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.toolbar]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.vboxMain1 = new global::Gtk.VBox ();
			this.vboxMain1.Name = "vboxMain1";
			this.vboxMain1.Spacing = 6;
			this.vboxMain1.BorderWidth = ((uint)(6));
			// Container child vboxMain1.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.vbox2.BorderWidth = ((uint)(3));
			// Container child vbox2.Gtk.Box+BoxChild
			this.lblTitle = new global::Gtk.Label ();
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Xalign = 0F;
			this.lblTitle.LabelProp = global::Mono.Unix.Catalog.GetString ("Updating database...");
			this.vbox2.Add (this.lblTitle);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.lblTitle]));
			w3.Position = 0;
			w3.Expand = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.lblSubtitle = new global::Gtk.Label ();
			this.lblSubtitle.Name = "lblSubtitle";
			this.lblSubtitle.Xalign = 0F;
			this.lblSubtitle.LabelProp = global::Mono.Unix.Catalog.GetString ("Adding file /home/animal/Music/Flac/Neu");
			this.lblSubtitle.Wrap = true;
			this.lblSubtitle.Ellipsize = ((global::Pango.EllipsizeMode)(3));
			this.lblSubtitle.SingleLineMode = true;
			this.vbox2.Add (this.lblSubtitle);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.lblSubtitle]));
			w4.Position = 1;
			w4.Expand = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.progressbar = new global::Gtk.ProgressBar ();
			this.progressbar.Name = "progressbar";
			this.progressbar.Fraction = 1;
			this.vbox2.Add (this.progressbar);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.progressbar]));
			w5.Position = 2;
			w5.Expand = false;
			w5.Fill = false;
			w5.Padding = ((uint)(3));
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.lblTimeElapsed = new global::Gtk.Label ();
			this.lblTimeElapsed.Name = "lblTimeElapsed";
			this.lblTimeElapsed.LabelProp = global::Mono.Unix.Catalog.GetString ("Time elapsed: 0:00 minute");
			this.hbox1.Add (this.lblTimeElapsed);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.lblTimeElapsed]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.lblPercentage = new global::Gtk.Label ();
			this.lblPercentage.Name = "lblPercentage";
			this.lblPercentage.LabelProp = global::Mono.Unix.Catalog.GetString ("60.58 %");
			this.hbox1.Add (this.lblPercentage);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.lblPercentage]));
			w7.Position = 1;
			// Container child hbox1.Gtk.Box+BoxChild
			this.lblEstimatedTimeLeft = new global::Gtk.Label ();
			this.lblEstimatedTimeLeft.Name = "lblEstimatedTimeLeft";
			this.lblEstimatedTimeLeft.LabelProp = global::Mono.Unix.Catalog.GetString ("Estimated Time Left: 20 minutes");
			this.hbox1.Add (this.lblEstimatedTimeLeft);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.lblEstimatedTimeLeft]));
			w8.Position = 2;
			w8.Expand = false;
			w8.Fill = false;
			this.vbox2.Add (this.hbox1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox1]));
			w9.Position = 3;
			w9.Expand = false;
			w9.Fill = false;
			this.vboxMain1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vboxMain1 [this.vbox2]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vboxMain1.Gtk.Box+BoxChild
			this.lblErrorLog = new global::Gtk.Label ();
			this.lblErrorLog.Name = "lblErrorLog";
			this.lblErrorLog.LabelProp = global::Mono.Unix.Catalog.GetString ("Error Log");
			this.vboxMain1.Add (this.lblErrorLog);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vboxMain1 [this.lblErrorLog]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vboxMain1.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textviewErrorLog = new global::Gtk.TextView ();
			this.textviewErrorLog.CanFocus = true;
			this.textviewErrorLog.Name = "textviewErrorLog";
			this.textviewErrorLog.Editable = false;
			this.textviewErrorLog.LeftMargin = 3;
			this.textviewErrorLog.RightMargin = 3;
			this.GtkScrolledWindow.Add (this.textviewErrorLog);
			this.vboxMain1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vboxMain1 [this.GtkScrolledWindow]));
			w13.Position = 2;
			this.vboxMain.Add (this.vboxMain1);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.vboxMain1]));
			w14.Position = 1;
			this.Add (this.vboxMain);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 691;
			this.DefaultHeight = 426;
			this.Show ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.actionOK.Activated += new global::System.EventHandler (this.OnActionOKActivated);
			this.actionCancel.Activated += new global::System.EventHandler (this.OnActionCancelActivated);
			this.actionSaveLog.Activated += new global::System.EventHandler (this.OnActionSaveLogActivated);
		}
	}
}
