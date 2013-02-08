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
	public partial class PlaylistWindow
	{
		private global::Gtk.UIManager UIManager;
		private global::Gtk.Action newAction;
		private global::Gtk.Action openAction;
		private global::Gtk.Action saveAction;
		private global::Gtk.Action saveAsAction;
		private global::Gtk.Action actionNewPlaylist;
		private global::Gtk.Action actionOpenPlaylist;
		private global::Gtk.Action actionSavePlaylist;
		private global::Gtk.Action actionSavePlaylistAs;
		private global::Gtk.VBox vbox1;
		private global::Gtk.Toolbar toolbar;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treePlaylistBrowser;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MPfm.GTK.PlaylistWindow
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.newAction = new global::Gtk.Action ("newAction", global::Mono.Unix.Catalog.GetString ("New"), null, "gtk-new");
			this.newAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("New");
			w1.Add (this.newAction, null);
			this.openAction = new global::Gtk.Action ("openAction", global::Mono.Unix.Catalog.GetString ("Open"), null, "gtk-open");
			this.openAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Open");
			w1.Add (this.openAction, null);
			this.saveAction = new global::Gtk.Action ("saveAction", null, null, "gtk-save");
			w1.Add (this.saveAction, null);
			this.saveAsAction = new global::Gtk.Action ("saveAsAction", null, null, "gtk-save-as");
			w1.Add (this.saveAsAction, null);
			this.actionNewPlaylist = new global::Gtk.Action ("actionNewPlaylist", null, null, "gtk-new");
			w1.Add (this.actionNewPlaylist, null);
			this.actionOpenPlaylist = new global::Gtk.Action ("actionOpenPlaylist", null, null, "gtk-open");
			w1.Add (this.actionOpenPlaylist, null);
			this.actionSavePlaylist = new global::Gtk.Action ("actionSavePlaylist", null, null, "gtk-save");
			w1.Add (this.actionSavePlaylist, null);
			this.actionSavePlaylistAs = new global::Gtk.Action ("actionSavePlaylistAs", null, null, "gtk-save-as");
			w1.Add (this.actionSavePlaylistAs, null);
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.Name = "MPfm.GTK.PlaylistWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Playlist");
			this.Icon = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./icon48.png"));
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			// Container child MPfm.GTK.PlaylistWindow.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			// Container child vbox1.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='toolbar'><toolitem name='actionNewPlaylist' action='actionNewPlaylist'/><toolitem name='actionOpenPlaylist' action='actionOpenPlaylist'/><toolitem name='actionSavePlaylist' action='actionSavePlaylist'/><toolitem name='actionSavePlaylistAs' action='actionSavePlaylistAs'/></toolbar></ui>");
			this.toolbar = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbar")));
			this.toolbar.Name = "toolbar";
			this.toolbar.ShowArrow = false;
			this.vbox1.Add (this.toolbar);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.toolbar]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.treePlaylistBrowser = new global::Gtk.TreeView ();
			this.treePlaylistBrowser.CanFocus = true;
			this.treePlaylistBrowser.Name = "treePlaylistBrowser";
			this.GtkScrolledWindow.Add (this.treePlaylistBrowser);
			this.vbox1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.GtkScrolledWindow]));
			w4.Position = 1;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 816;
			this.DefaultHeight = 303;
			this.Hide ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.actionNewPlaylist.Activated += new global::System.EventHandler (this.OnActionNewPlaylistActivated);
			this.actionOpenPlaylist.Activated += new global::System.EventHandler (this.OnActionOpenPlaylistActivated);
			this.actionSavePlaylist.Activated += new global::System.EventHandler (this.OnActionSavePlaylistActivated);
			this.actionSavePlaylistAs.Activated += new global::System.EventHandler (this.OnActionSavePlaylistAsActivated);
		}
	}
}