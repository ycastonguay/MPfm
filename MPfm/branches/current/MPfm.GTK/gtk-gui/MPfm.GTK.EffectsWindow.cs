
// This file has been generated by the GUI designer. Do not modify.
namespace MPfm.GTK
{
	public partial class EffectsWindow
	{
		private global::Gtk.VBox vbox2;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MPfm.GTK.EffectsWindow
			this.Name = "MPfm.GTK.EffectsWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Effects");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child MPfm.GTK.EffectsWindow.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			this.Hide ();
		}
	}
}
