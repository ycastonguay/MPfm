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
	public partial class SplashWindow
	{
		private global::Gtk.VBox vbox1;
		private global::Gtk.Image imageBackground;
		private global::Gtk.Label lblStatus;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MPfm.GTK.SplashWindow
			this.Name = "MPfm.GTK.SplashWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("SplashWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(3));
			this.Resizable = false;
			this.AllowGrow = false;
			this.Decorated = false;
			// Container child MPfm.GTK.SplashWindow.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			// Container child vbox1.Gtk.Box+BoxChild
			this.imageBackground = new global::Gtk.Image ();
			this.imageBackground.Name = "imageBackground";
			this.vbox1.Add (this.imageBackground);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.imageBackground]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.lblStatus = new global::Gtk.Label ();
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Xpad = 4;
			this.lblStatus.Xalign = 0F;
			this.lblStatus.LabelProp = global::Mono.Unix.Catalog.GetString ("Loading...");
			this.vbox1.Add (this.lblStatus);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.lblStatus]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			w2.Padding = ((uint)(2));
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 646;
			this.DefaultHeight = 554;
			this.Show ();
		}
	}
}
