
// This file has been generated by the GUI designer. Do not modify.
namespace MPfm.GTK
{
	public partial class SettingsWindow
	{
		private global::Gtk.VBox vbox3;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MPfm.GTK.SettingsWindow
			this.Name = "MPfm.GTK.SettingsWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Settings");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			// Container child MPfm.GTK.SettingsWindow.Gtk.Container+ContainerChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			this.Add (this.vbox3);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			this.Hide ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		}
	}
}