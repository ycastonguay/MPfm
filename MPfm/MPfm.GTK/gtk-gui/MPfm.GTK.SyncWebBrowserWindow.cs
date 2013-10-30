
// This file has been generated by the GUI designer. Do not modify.
namespace MPfm.GTK
{
	public partial class SyncWebBrowserWindow
	{
		private global::Gtk.VBox vbox1;
		private global::Gtk.Label label1;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.Label lblUrl;
		private global::Gtk.Label label5;
		private global::Gtk.Label label6;
		private global::Gtk.Label lblAuthenticationCode;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MPfm.GTK.SyncWebBrowserWindow
			this.Name = "MPfm.GTK.SyncWebBrowserWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("SyncWebBrowserWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child MPfm.GTK.SyncWebBrowserWindow.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			this.vbox1.BorderWidth = ((uint)(6));
			// Container child vbox1.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Sync (Web Browser)");
			this.vbox1.Add (this.label1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.label1]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Step 1:");
			this.vbox1.Add (this.label3);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.label3]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Open a web browser and enter the following url in the address bar:");
			this.vbox1.Add (this.label4);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.label4]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.lblUrl = new global::Gtk.Label ();
			this.lblUrl.Name = "lblUrl";
			this.lblUrl.LabelProp = global::Mono.Unix.Catalog.GetString ("http://192.168.1.1:53551");
			this.vbox1.Add (this.lblUrl);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.lblUrl]));
			w4.Position = 3;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xalign = 0F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Step 2:");
			this.vbox1.Add (this.label5);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.label5]));
			w5.Position = 4;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Enter the following authentication code and click on the Login button:");
			this.vbox1.Add (this.label6);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.label6]));
			w6.Position = 5;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.lblAuthenticationCode = new global::Gtk.Label ();
			this.lblAuthenticationCode.Name = "lblAuthenticationCode";
			this.lblAuthenticationCode.LabelProp = global::Mono.Unix.Catalog.GetString ("00000");
			this.vbox1.Add (this.lblAuthenticationCode);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.lblAuthenticationCode]));
			w7.Position = 6;
			w7.Expand = false;
			w7.Fill = false;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 411;
			this.DefaultHeight = 300;
			this.Show ();
		}
	}
}