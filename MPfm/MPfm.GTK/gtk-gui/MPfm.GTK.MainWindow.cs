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
	public partial class MainWindow
	{
		private global::Gtk.UIManager UIManager;
		private global::Gtk.Action FileAction;
		private global::Gtk.Action actionExit;
		private global::Gtk.Action actionAbout;
		private global::Gtk.Action actionHelp;
		private global::Gtk.Action aboutAction;
		private global::Gtk.Action PlaybackAction;
		private global::Gtk.Action actionPlay;
		private global::Gtk.Action actionPause;
		private global::Gtk.Action actionStop;
		private global::Gtk.Action actionPrevious;
		private global::Gtk.Action actionNext;
		private global::Gtk.Action actionRepeatType;
		private global::Gtk.Action openAction;
		private global::Gtk.Action actionUpdateLibrary;
		private global::Gtk.Action actionSettings;
		private global::Gtk.Action actionEffects;
		private global::Gtk.Action WindowsAction;
		private global::Gtk.Action actionPlaylist;
		private global::Gtk.Action actionRemoveLoop;
		private global::Gtk.Action actionAddMarker;
		private global::Gtk.Action actionRemoveMarker;
		private global::Gtk.Action addAction;
		private global::Gtk.Action editAction;
		private global::Gtk.Action actionEditMarker;
		private global::Gtk.Action actionGoToMarker;
		private global::Gtk.Action actionAddFiles;
		private global::Gtk.Action actionAddFolder;
		private global::Gtk.Action HelpAction;
		private global::Gtk.Action actionPlayLoop;
		private global::Gtk.VBox vboxMain;
		private global::Gtk.MenuBar menubarMain;
		private global::Gtk.Toolbar toolbarMain;
		private global::Gtk.HPaned hpanedMain;
		private global::Gtk.VBox vboxLeft;
		private global::Gtk.Label lblLibraryBrowser;
		private global::Gtk.HBox hbox13;
		private global::Gtk.Label lblLibraryFilter;
		private global::Gtk.ComboBox cboSoundFormat;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TreeView treeLibraryBrowser;
		private global::Gtk.VBox vboxRight;
		private global::Gtk.VBox vboxCurrentSong;
		private global::Gtk.Label lblCurrentSong;
		private global::Gtk.HBox hbox4;
		private global::Gtk.Image imageAlbumCover;
		private global::Gtk.VBox vbox3;
		private global::Gtk.HBox hbox5;
		private global::Gtk.VBox vbox4;
		private global::Gtk.Label lblArtistName;
		private global::Gtk.Label lblAlbumTitle;
		private global::Gtk.Label lblSongTitle;
		private global::Gtk.Label lblSongFilePath;
		private global::Gtk.VBox vbox1;
		private global::Gtk.HBox hbox14;
		private global::Gtk.Label lblPitchShifting;
		private global::Gtk.Button btnPitchShiftingMore;
		private global::Gtk.HScale hscaleTimeShifting1;
		private global::Gtk.HBox hbox15;
		private global::Gtk.Label lblPitchShiftingReset;
		private global::Gtk.Label lblPitchShiftingValue;
		private global::Gtk.Button btnDetectTempo;
		private global::Gtk.HBox hbox16;
		private global::Gtk.Label lblOriginalTempo;
		private global::Gtk.SpinButton spinOriginalTempoValue;
		private global::Gtk.Label lblOriginalTempoBPM;
		private global::Gtk.VBox vbox10;
		private global::Gtk.Label lblInformation;
		private global::Gtk.Label lblCurrentFileType;
		private global::Gtk.Label lblCurrentSampleRate;
		private global::Gtk.Label lblCurrentBitsPerSample;
		private global::Gtk.Label lblCurrentBitrate;
		private global::Gtk.HBox hbox6;
		private global::Gtk.VBox vbox5;
		private global::Gtk.Label lblSongPosition;
		private global::Gtk.HBox hbox7;
		private global::Gtk.Label lblCurrentPosition;
		private global::Gtk.HScale hscaleSongPosition;
		private global::Gtk.Label lblCurrentLength;
		private global::Gtk.VBox vbox8;
		private global::Gtk.HBox hbox11;
		private global::Gtk.Label lblTimeShifting;
		private global::Gtk.Button btnTimeShiftingMore;
		private global::Gtk.HScale hscaleTimeShifting;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Label lblTimeShiftingReset;
		private global::Gtk.Label lblTimeShiftingValue;
		private global::Gtk.VBox vbox9;
		private global::Gtk.Label lblVolume;
		private global::Gtk.VScale vscaleVolume;
		private global::Gtk.Label lblCurrentVolume;
		private global::Gtk.VPaned vpanedLoopsMarkersSongBrowser;
		private global::Gtk.HPaned hpanedLoopsMarkers;
		private global::Gtk.VBox vboxLoops;
		private global::Gtk.HBox hbox8;
		private global::Gtk.Label lblLoops;
		private global::Gtk.Toolbar toolbarLoops;
		private global::Gtk.ScrolledWindow GtkScrolledWindow2;
		private global::Gtk.TreeView treeLoops;
		private global::Gtk.VBox vboxLoops1;
		private global::Gtk.HBox hbox9;
		private global::Gtk.Label lblMarkers;
		private global::Gtk.Toolbar toolbarMarkers;
		private global::Gtk.ScrolledWindow GtkScrolledWindow3;
		private global::Gtk.TreeView treeMarkers;
		private global::Gtk.VBox vboxSongBrowser;
		private global::Gtk.HBox hbox10;
		private global::Gtk.Label lblSongBrowser;
		private global::Gtk.Label lblSearchFor;
		private global::Gtk.Entry txtSearch;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeSongBrowser;
		private global::Gtk.Statusbar statusbar1;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MPfm.GTK.MainWindow
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
			this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
			w1.Add (this.FileAction, null);
			this.actionExit = new global::Gtk.Action ("actionExit", global::Mono.Unix.Catalog.GetString ("Exit"), null, "gtk-quit");
			this.actionExit.ShortLabel = global::Mono.Unix.Catalog.GetString ("Exit");
			w1.Add (this.actionExit, null);
			this.actionAbout = new global::Gtk.Action ("actionAbout", global::Mono.Unix.Catalog.GetString ("About MPfm"), null, null);
			this.actionAbout.ShortLabel = global::Mono.Unix.Catalog.GetString ("About MPfm");
			w1.Add (this.actionAbout, null);
			this.actionHelp = new global::Gtk.Action ("actionHelp", global::Mono.Unix.Catalog.GetString ("Help"), null, "gtk-help");
			this.actionHelp.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
			w1.Add (this.actionHelp, null);
			this.aboutAction = new global::Gtk.Action ("aboutAction", global::Mono.Unix.Catalog.GetString ("About MPfm"), null, "gtk-about");
			this.aboutAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("About MPfm");
			w1.Add (this.aboutAction, null);
			this.PlaybackAction = new global::Gtk.Action ("PlaybackAction", global::Mono.Unix.Catalog.GetString ("Playback"), null, null);
			this.PlaybackAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Playback");
			w1.Add (this.PlaybackAction, null);
			this.actionPlay = new global::Gtk.Action ("actionPlay", global::Mono.Unix.Catalog.GetString ("Play"), null, "gtk-media-play");
			this.actionPlay.ShortLabel = global::Mono.Unix.Catalog.GetString ("Play");
			w1.Add (this.actionPlay, null);
			this.actionPause = new global::Gtk.Action ("actionPause", global::Mono.Unix.Catalog.GetString ("Pause"), null, "gtk-media-pause");
			this.actionPause.ShortLabel = global::Mono.Unix.Catalog.GetString ("Pause");
			w1.Add (this.actionPause, null);
			this.actionStop = new global::Gtk.Action ("actionStop", global::Mono.Unix.Catalog.GetString ("Stop"), null, "gtk-media-stop");
			this.actionStop.ShortLabel = global::Mono.Unix.Catalog.GetString ("Stop");
			w1.Add (this.actionStop, null);
			this.actionPrevious = new global::Gtk.Action ("actionPrevious", global::Mono.Unix.Catalog.GetString ("Previous Song"), null, "gtk-media-previous");
			this.actionPrevious.ShortLabel = global::Mono.Unix.Catalog.GetString ("Previous Song");
			w1.Add (this.actionPrevious, null);
			this.actionNext = new global::Gtk.Action ("actionNext", global::Mono.Unix.Catalog.GetString ("Next Song"), null, "gtk-media-next");
			this.actionNext.ShortLabel = global::Mono.Unix.Catalog.GetString ("Next Song");
			w1.Add (this.actionNext, null);
			this.actionRepeatType = new global::Gtk.Action ("actionRepeatType", global::Mono.Unix.Catalog.GetString ("Repeat Type (Off)"), null, "stock_repeat");
			this.actionRepeatType.ShortLabel = global::Mono.Unix.Catalog.GetString ("Repeat Type (Off)");
			w1.Add (this.actionRepeatType, null);
			this.openAction = new global::Gtk.Action ("openAction", global::Mono.Unix.Catalog.GetString ("Open audio files..."), null, "gtk-open");
			this.openAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Open Audio Files");
			w1.Add (this.openAction, null);
			this.actionUpdateLibrary = new global::Gtk.Action ("actionUpdateLibrary", global::Mono.Unix.Catalog.GetString ("Update Library"), null, "gtk-refresh");
			this.actionUpdateLibrary.ShortLabel = global::Mono.Unix.Catalog.GetString ("Update Library");
			w1.Add (this.actionUpdateLibrary, null);
			this.actionSettings = new global::Gtk.Action ("actionSettings", global::Mono.Unix.Catalog.GetString ("Settings"), null, "gtk-preferences");
			this.actionSettings.ShortLabel = global::Mono.Unix.Catalog.GetString ("Settings");
			w1.Add (this.actionSettings, null);
			this.actionEffects = new global::Gtk.Action ("actionEffects", global::Mono.Unix.Catalog.GetString ("Effects"), null, "stock_volume");
			this.actionEffects.ShortLabel = global::Mono.Unix.Catalog.GetString ("Effects");
			w1.Add (this.actionEffects, null);
			this.WindowsAction = new global::Gtk.Action ("WindowsAction", global::Mono.Unix.Catalog.GetString ("Windows"), null, null);
			this.WindowsAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Windows");
			w1.Add (this.WindowsAction, null);
			this.actionPlaylist = new global::Gtk.Action ("actionPlaylist", global::Mono.Unix.Catalog.GetString ("Playlist"), null, "gtk-dnd-multiple");
			this.actionPlaylist.ShortLabel = global::Mono.Unix.Catalog.GetString ("Playlist");
			w1.Add (this.actionPlaylist, null);
			this.actionRemoveLoop = new global::Gtk.Action ("actionRemoveLoop", null, null, "gtk-remove");
			w1.Add (this.actionRemoveLoop, null);
			this.actionAddMarker = new global::Gtk.Action ("actionAddMarker", null, null, "gtk-add");
			w1.Add (this.actionAddMarker, null);
			this.actionRemoveMarker = new global::Gtk.Action ("actionRemoveMarker", null, null, "gtk-remove");
			w1.Add (this.actionRemoveMarker, null);
			this.addAction = new global::Gtk.Action ("addAction", null, null, "gtk-add");
			w1.Add (this.addAction, null);
			this.editAction = new global::Gtk.Action ("editAction", null, null, "gtk-edit");
			w1.Add (this.editAction, null);
			this.actionEditMarker = new global::Gtk.Action ("actionEditMarker", null, null, "gtk-edit");
			w1.Add (this.actionEditMarker, null);
			this.actionGoToMarker = new global::Gtk.Action ("actionGoToMarker", null, null, "gtk-go-forward");
			w1.Add (this.actionGoToMarker, null);
			this.actionAddFiles = new global::Gtk.Action ("actionAddFiles", global::Mono.Unix.Catalog.GetString ("Add files to library..."), null, "gtk-add");
			this.actionAddFiles.ShortLabel = global::Mono.Unix.Catalog.GetString ("Add files to library...");
			w1.Add (this.actionAddFiles, null);
			this.actionAddFolder = new global::Gtk.Action ("actionAddFolder", global::Mono.Unix.Catalog.GetString ("Add folder to library..."), null, "gtk-add");
			this.actionAddFolder.ShortLabel = global::Mono.Unix.Catalog.GetString ("Add folder to library...");
			w1.Add (this.actionAddFolder, null);
			this.HelpAction = new global::Gtk.Action ("HelpAction", global::Mono.Unix.Catalog.GetString ("Help"), null, null);
			this.HelpAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
			w1.Add (this.HelpAction, null);
			this.actionPlayLoop = new global::Gtk.Action ("actionPlayLoop", null, null, "gtk-media-play");
			w1.Add (this.actionPlayLoop, null);
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.Name = "MPfm.GTK.MainWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("MPfm: Music Player for Musicians");
			this.Icon = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./icon48.png"));
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.DefaultWidth = 100;
			// Container child MPfm.GTK.MainWindow.Gtk.Container+ContainerChild
			this.vboxMain = new global::Gtk.VBox ();
			this.vboxMain.Name = "vboxMain";
			// Container child vboxMain.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><menubar name='menubarMain'><menu name='FileAction' action='FileAction'><menuitem name='actionAddFiles' action='actionAddFiles'/><menuitem name='actionAddFolder' action='actionAddFolder'/><separator/><menuitem name='openAction' action='openAction'/><separator/><menuitem name='actionUpdateLibrary' action='actionUpdateLibrary'/><separator/><menuitem name='actionExit' action='actionExit'/></menu><menu name='PlaybackAction' action='PlaybackAction'><menuitem name='actionPlay' action='actionPlay'/><menuitem name='actionPause' action='actionPause'/><menuitem name='actionStop' action='actionStop'/><menuitem name='actionPrevious' action='actionPrevious'/><menuitem name='actionNext' action='actionNext'/><menuitem name='actionRepeatType' action='actionRepeatType'/></menu><menu name='WindowsAction' action='WindowsAction'/><menu name='HelpAction' action='HelpAction'><menuitem name='actionHelp' action='actionHelp'/><menuitem name='actionAbout' action='actionAbout'/></menu></menubar></ui>");
			this.menubarMain = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubarMain")));
			this.menubarMain.Name = "menubarMain";
			this.vboxMain.Add (this.menubarMain);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.menubarMain]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='toolbarMain'><toolitem name='openAction' action='openAction'/><separator/><toolitem name='actionUpdateLibrary' action='actionUpdateLibrary'/><separator/><toolitem name='actionPlay' action='actionPlay'/><toolitem name='actionPause' action='actionPause'/><toolitem name='actionStop' action='actionStop'/><toolitem name='actionPrevious' action='actionPrevious'/><toolitem name='actionNext' action='actionNext'/><toolitem name='actionRepeatType' action='actionRepeatType'/><separator/><toolitem name='actionPlaylist' action='actionPlaylist'/><toolitem name='actionEffects' action='actionEffects'/><toolitem name='actionSettings' action='actionSettings'/></toolbar></ui>");
			this.toolbarMain = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbarMain")));
			this.toolbarMain.Name = "toolbarMain";
			this.toolbarMain.ShowArrow = false;
			this.toolbarMain.ToolbarStyle = ((global::Gtk.ToolbarStyle)(0));
			this.toolbarMain.IconSize = ((global::Gtk.IconSize)(2));
			this.vboxMain.Add (this.toolbarMain);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.toolbarMain]));
			w3.Position = 1;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.hpanedMain = new global::Gtk.HPaned ();
			this.hpanedMain.CanFocus = true;
			this.hpanedMain.Name = "hpanedMain";
			this.hpanedMain.Position = 236;
			// Container child hpanedMain.Gtk.Paned+PanedChild
			this.vboxLeft = new global::Gtk.VBox ();
			this.vboxLeft.Name = "vboxLeft";
			this.vboxLeft.Spacing = 6;
			this.vboxLeft.BorderWidth = ((uint)(4));
			// Container child vboxLeft.Gtk.Box+BoxChild
			this.lblLibraryBrowser = new global::Gtk.Label ();
			this.lblLibraryBrowser.Name = "lblLibraryBrowser";
			this.lblLibraryBrowser.Xalign = 0F;
			this.lblLibraryBrowser.LabelProp = global::Mono.Unix.Catalog.GetString ("Library Browser");
			this.vboxLeft.Add (this.lblLibraryBrowser);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vboxLeft [this.lblLibraryBrowser]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vboxLeft.Gtk.Box+BoxChild
			this.hbox13 = new global::Gtk.HBox ();
			this.hbox13.Name = "hbox13";
			this.hbox13.Spacing = 6;
			// Container child hbox13.Gtk.Box+BoxChild
			this.lblLibraryFilter = new global::Gtk.Label ();
			this.lblLibraryFilter.Name = "lblLibraryFilter";
			this.lblLibraryFilter.Xalign = 0F;
			this.lblLibraryFilter.LabelProp = global::Mono.Unix.Catalog.GetString ("Filter by Sound Format:");
			this.hbox13.Add (this.lblLibraryFilter);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox13 [this.lblLibraryFilter]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox13.Gtk.Box+BoxChild
			this.cboSoundFormat = global::Gtk.ComboBox.NewText ();
			this.cboSoundFormat.Name = "cboSoundFormat";
			this.hbox13.Add (this.cboSoundFormat);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox13 [this.cboSoundFormat]));
			w6.Position = 1;
			this.vboxLeft.Add (this.hbox13);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vboxLeft [this.hbox13]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vboxLeft.Gtk.Box+BoxChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.treeLibraryBrowser = new global::Gtk.TreeView ();
			this.treeLibraryBrowser.CanFocus = true;
			this.treeLibraryBrowser.Name = "treeLibraryBrowser";
			this.GtkScrolledWindow1.Add (this.treeLibraryBrowser);
			this.vboxLeft.Add (this.GtkScrolledWindow1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vboxLeft [this.GtkScrolledWindow1]));
			w9.Position = 2;
			this.hpanedMain.Add (this.vboxLeft);
			global::Gtk.Paned.PanedChild w10 = ((global::Gtk.Paned.PanedChild)(this.hpanedMain [this.vboxLeft]));
			w10.Resize = false;
			// Container child hpanedMain.Gtk.Paned+PanedChild
			this.vboxRight = new global::Gtk.VBox ();
			this.vboxRight.Name = "vboxRight";
			this.vboxRight.Spacing = 6;
			this.vboxRight.BorderWidth = ((uint)(4));
			// Container child vboxRight.Gtk.Box+BoxChild
			this.vboxCurrentSong = new global::Gtk.VBox ();
			this.vboxCurrentSong.Name = "vboxCurrentSong";
			this.vboxCurrentSong.Spacing = 2;
			// Container child vboxCurrentSong.Gtk.Box+BoxChild
			this.lblCurrentSong = new global::Gtk.Label ();
			this.lblCurrentSong.Name = "lblCurrentSong";
			this.lblCurrentSong.Xalign = 0F;
			this.lblCurrentSong.LabelProp = global::Mono.Unix.Catalog.GetString ("Current Song");
			this.vboxCurrentSong.Add (this.lblCurrentSong);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vboxCurrentSong [this.lblCurrentSong]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vboxCurrentSong.Gtk.Box+BoxChild
			this.hbox4 = new global::Gtk.HBox ();
			this.hbox4.Name = "hbox4";
			this.hbox4.Spacing = 6;
			// Container child hbox4.Gtk.Box+BoxChild
			this.imageAlbumCover = new global::Gtk.Image ();
			this.imageAlbumCover.Name = "imageAlbumCover";
			this.imageAlbumCover.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-about", global::Gtk.IconSize.Menu);
			this.hbox4.Add (this.imageAlbumCover);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.imageAlbumCover]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			// Container child hbox4.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox5 = new global::Gtk.HBox ();
			this.hbox5.Name = "hbox5";
			this.hbox5.Spacing = 6;
			// Container child hbox5.Gtk.Box+BoxChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.lblArtistName = new global::Gtk.Label ();
			this.lblArtistName.Name = "lblArtistName";
			this.lblArtistName.Xalign = 0F;
			this.lblArtistName.LabelProp = global::Mono.Unix.Catalog.GetString ("Artist Name");
			this.lblArtistName.Wrap = true;
			this.lblArtistName.Ellipsize = ((global::Pango.EllipsizeMode)(3));
			this.vbox4.Add (this.lblArtistName);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.lblArtistName]));
			w13.Position = 0;
			w13.Expand = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.lblAlbumTitle = new global::Gtk.Label ();
			this.lblAlbumTitle.Name = "lblAlbumTitle";
			this.lblAlbumTitle.Xalign = 0F;
			this.lblAlbumTitle.LabelProp = global::Mono.Unix.Catalog.GetString ("Album Title");
			this.lblAlbumTitle.Wrap = true;
			this.lblAlbumTitle.Ellipsize = ((global::Pango.EllipsizeMode)(3));
			this.vbox4.Add (this.lblAlbumTitle);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.lblAlbumTitle]));
			w14.Position = 1;
			w14.Expand = false;
			w14.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.lblSongTitle = new global::Gtk.Label ();
			this.lblSongTitle.Name = "lblSongTitle";
			this.lblSongTitle.Xalign = 0F;
			this.lblSongTitle.LabelProp = global::Mono.Unix.Catalog.GetString ("Song Title");
			this.lblSongTitle.Wrap = true;
			this.lblSongTitle.Ellipsize = ((global::Pango.EllipsizeMode)(3));
			this.vbox4.Add (this.lblSongTitle);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.lblSongTitle]));
			w15.Position = 2;
			w15.Expand = false;
			w15.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.lblSongFilePath = new global::Gtk.Label ();
			this.lblSongFilePath.Name = "lblSongFilePath";
			this.lblSongFilePath.Xalign = 0F;
			this.lblSongFilePath.LabelProp = global::Mono.Unix.Catalog.GetString ("Song File Path");
			this.lblSongFilePath.Wrap = true;
			this.lblSongFilePath.Ellipsize = ((global::Pango.EllipsizeMode)(3));
			this.vbox4.Add (this.lblSongFilePath);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.lblSongFilePath]));
			w16.Position = 3;
			w16.Expand = false;
			w16.Fill = false;
			this.hbox5.Add (this.vbox4);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.vbox4]));
			w17.Position = 0;
			// Container child hbox5.Gtk.Box+BoxChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.WidthRequest = 180;
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 2;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox14 = new global::Gtk.HBox ();
			this.hbox14.Name = "hbox14";
			this.hbox14.Spacing = 8;
			// Container child hbox14.Gtk.Box+BoxChild
			this.lblPitchShifting = new global::Gtk.Label ();
			this.lblPitchShifting.HeightRequest = 20;
			this.lblPitchShifting.Name = "lblPitchShifting";
			this.lblPitchShifting.Xalign = 0F;
			this.lblPitchShifting.LabelProp = global::Mono.Unix.Catalog.GetString ("Pitch Shifting");
			this.hbox14.Add (this.lblPitchShifting);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox14 [this.lblPitchShifting]));
			w18.Position = 0;
			// Container child hbox14.Gtk.Box+BoxChild
			this.btnPitchShiftingMore = new global::Gtk.Button ();
			this.btnPitchShiftingMore.HeightRequest = 20;
			this.btnPitchShiftingMore.CanFocus = true;
			this.btnPitchShiftingMore.Name = "btnPitchShiftingMore";
			this.btnPitchShiftingMore.UseUnderline = true;
			this.btnPitchShiftingMore.Label = global::Mono.Unix.Catalog.GetString ("...");
			this.hbox14.Add (this.btnPitchShiftingMore);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.hbox14 [this.btnPitchShiftingMore]));
			w19.Position = 1;
			w19.Expand = false;
			w19.Fill = false;
			this.vbox1.Add (this.hbox14);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox14]));
			w20.Position = 0;
			w20.Expand = false;
			w20.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hscaleTimeShifting1 = new global::Gtk.HScale (null);
			this.hscaleTimeShifting1.CanFocus = true;
			this.hscaleTimeShifting1.Name = "hscaleTimeShifting1";
			this.hscaleTimeShifting1.Adjustment.Lower = 50;
			this.hscaleTimeShifting1.Adjustment.Upper = 150;
			this.hscaleTimeShifting1.Adjustment.PageIncrement = 10;
			this.hscaleTimeShifting1.Adjustment.StepIncrement = 1;
			this.hscaleTimeShifting1.Adjustment.Value = 100;
			this.hscaleTimeShifting1.DrawValue = false;
			this.hscaleTimeShifting1.Digits = 0;
			this.hscaleTimeShifting1.ValuePos = ((global::Gtk.PositionType)(0));
			this.vbox1.Add (this.hscaleTimeShifting1);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hscaleTimeShifting1]));
			w21.Position = 1;
			w21.Expand = false;
			w21.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox15 = new global::Gtk.HBox ();
			this.hbox15.Name = "hbox15";
			this.hbox15.Spacing = 4;
			// Container child hbox15.Gtk.Box+BoxChild
			this.lblPitchShiftingReset = new global::Gtk.Label ();
			this.lblPitchShiftingReset.Name = "lblPitchShiftingReset";
			this.lblPitchShiftingReset.Xalign = 0F;
			this.lblPitchShiftingReset.LabelProp = global::Mono.Unix.Catalog.GetString ("Reset");
			this.hbox15.Add (this.lblPitchShiftingReset);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.hbox15 [this.lblPitchShiftingReset]));
			w22.Position = 0;
			// Container child hbox15.Gtk.Box+BoxChild
			this.lblPitchShiftingValue = new global::Gtk.Label ();
			this.lblPitchShiftingValue.Name = "lblPitchShiftingValue";
			this.lblPitchShiftingValue.Xalign = 0F;
			this.lblPitchShiftingValue.LabelProp = global::Mono.Unix.Catalog.GetString ("100 %");
			this.hbox15.Add (this.lblPitchShiftingValue);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.hbox15 [this.lblPitchShiftingValue]));
			w23.Position = 2;
			w23.Expand = false;
			w23.Fill = false;
			this.vbox1.Add (this.hbox15);
			global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox15]));
			w24.Position = 2;
			w24.Expand = false;
			w24.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.btnDetectTempo = new global::Gtk.Button ();
			this.btnDetectTempo.HeightRequest = 22;
			this.btnDetectTempo.CanFocus = true;
			this.btnDetectTempo.Name = "btnDetectTempo";
			this.btnDetectTempo.UseUnderline = true;
			this.btnDetectTempo.Label = global::Mono.Unix.Catalog.GetString ("Detect tempo");
			this.vbox1.Add (this.btnDetectTempo);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.btnDetectTempo]));
			w25.Position = 3;
			w25.Expand = false;
			w25.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox16 = new global::Gtk.HBox ();
			this.hbox16.Name = "hbox16";
			this.hbox16.Spacing = 4;
			// Container child hbox16.Gtk.Box+BoxChild
			this.lblOriginalTempo = new global::Gtk.Label ();
			this.lblOriginalTempo.Name = "lblOriginalTempo";
			this.lblOriginalTempo.Xalign = 0F;
			this.lblOriginalTempo.LabelProp = global::Mono.Unix.Catalog.GetString ("Original tempo:");
			this.hbox16.Add (this.lblOriginalTempo);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.hbox16 [this.lblOriginalTempo]));
			w26.Position = 0;
			// Container child hbox16.Gtk.Box+BoxChild
			this.spinOriginalTempoValue = new global::Gtk.SpinButton (0, 100, 1);
			this.spinOriginalTempoValue.CanFocus = true;
			this.spinOriginalTempoValue.Name = "spinOriginalTempoValue";
			this.spinOriginalTempoValue.Adjustment.PageIncrement = 10;
			this.spinOriginalTempoValue.ClimbRate = 1;
			this.spinOriginalTempoValue.Numeric = true;
			this.hbox16.Add (this.spinOriginalTempoValue);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.hbox16 [this.spinOriginalTempoValue]));
			w27.Position = 1;
			w27.Expand = false;
			w27.Fill = false;
			// Container child hbox16.Gtk.Box+BoxChild
			this.lblOriginalTempoBPM = new global::Gtk.Label ();
			this.lblOriginalTempoBPM.Name = "lblOriginalTempoBPM";
			this.lblOriginalTempoBPM.Xalign = 0F;
			this.lblOriginalTempoBPM.LabelProp = global::Mono.Unix.Catalog.GetString ("bpm");
			this.hbox16.Add (this.lblOriginalTempoBPM);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(this.hbox16 [this.lblOriginalTempoBPM]));
			w28.Position = 2;
			w28.Expand = false;
			w28.Fill = false;
			this.vbox1.Add (this.hbox16);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox16]));
			w29.Position = 4;
			w29.Expand = false;
			w29.Fill = false;
			this.hbox5.Add (this.vbox1);
			global::Gtk.Box.BoxChild w30 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.vbox1]));
			w30.PackType = ((global::Gtk.PackType)(1));
			w30.Position = 1;
			w30.Expand = false;
			w30.Fill = false;
			// Container child hbox5.Gtk.Box+BoxChild
			this.vbox10 = new global::Gtk.VBox ();
			this.vbox10.WidthRequest = 84;
			this.vbox10.Name = "vbox10";
			this.vbox10.Spacing = 6;
			// Container child vbox10.Gtk.Box+BoxChild
			this.lblInformation = new global::Gtk.Label ();
			this.lblInformation.HeightRequest = 20;
			this.lblInformation.Name = "lblInformation";
			this.lblInformation.Xalign = 0F;
			this.lblInformation.LabelProp = global::Mono.Unix.Catalog.GetString ("Information");
			this.vbox10.Add (this.lblInformation);
			global::Gtk.Box.BoxChild w31 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.lblInformation]));
			w31.Position = 0;
			w31.Expand = false;
			w31.Fill = false;
			// Container child vbox10.Gtk.Box+BoxChild
			this.lblCurrentFileType = new global::Gtk.Label ();
			this.lblCurrentFileType.Name = "lblCurrentFileType";
			this.lblCurrentFileType.Xalign = 0F;
			this.lblCurrentFileType.LabelProp = global::Mono.Unix.Catalog.GetString ("FLAC");
			this.vbox10.Add (this.lblCurrentFileType);
			global::Gtk.Box.BoxChild w32 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.lblCurrentFileType]));
			w32.Position = 1;
			w32.Expand = false;
			w32.Fill = false;
			// Container child vbox10.Gtk.Box+BoxChild
			this.lblCurrentSampleRate = new global::Gtk.Label ();
			this.lblCurrentSampleRate.Name = "lblCurrentSampleRate";
			this.lblCurrentSampleRate.Xalign = 0F;
			this.lblCurrentSampleRate.LabelProp = global::Mono.Unix.Catalog.GetString ("44100 Hz");
			this.vbox10.Add (this.lblCurrentSampleRate);
			global::Gtk.Box.BoxChild w33 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.lblCurrentSampleRate]));
			w33.Position = 2;
			w33.Expand = false;
			w33.Fill = false;
			// Container child vbox10.Gtk.Box+BoxChild
			this.lblCurrentBitsPerSample = new global::Gtk.Label ();
			this.lblCurrentBitsPerSample.Name = "lblCurrentBitsPerSample";
			this.lblCurrentBitsPerSample.Xalign = 0F;
			this.lblCurrentBitsPerSample.LabelProp = global::Mono.Unix.Catalog.GetString ("16 bits");
			this.vbox10.Add (this.lblCurrentBitsPerSample);
			global::Gtk.Box.BoxChild w34 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.lblCurrentBitsPerSample]));
			w34.Position = 3;
			w34.Expand = false;
			w34.Fill = false;
			// Container child vbox10.Gtk.Box+BoxChild
			this.lblCurrentBitrate = new global::Gtk.Label ();
			this.lblCurrentBitrate.Name = "lblCurrentBitrate";
			this.lblCurrentBitrate.Xalign = 0F;
			this.lblCurrentBitrate.LabelProp = global::Mono.Unix.Catalog.GetString ("320 kbps");
			this.vbox10.Add (this.lblCurrentBitrate);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.lblCurrentBitrate]));
			w35.Position = 4;
			w35.Expand = false;
			w35.Fill = false;
			this.hbox5.Add (this.vbox10);
			global::Gtk.Box.BoxChild w36 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.vbox10]));
			w36.PackType = ((global::Gtk.PackType)(1));
			w36.Position = 2;
			w36.Expand = false;
			w36.Fill = false;
			this.vbox3.Add (this.hbox5);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox5]));
			w37.Position = 0;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox6 = new global::Gtk.HBox ();
			this.hbox6.Name = "hbox6";
			this.hbox6.Spacing = 6;
			// Container child hbox6.Gtk.Box+BoxChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			// Container child vbox5.Gtk.Box+BoxChild
			this.lblSongPosition = new global::Gtk.Label ();
			this.lblSongPosition.HeightRequest = 20;
			this.lblSongPosition.Name = "lblSongPosition";
			this.lblSongPosition.Xalign = 0F;
			this.lblSongPosition.LabelProp = global::Mono.Unix.Catalog.GetString ("Song Position");
			this.vbox5.Add (this.lblSongPosition);
			global::Gtk.Box.BoxChild w38 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.lblSongPosition]));
			w38.Position = 0;
			w38.Expand = false;
			w38.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.hbox7 = new global::Gtk.HBox ();
			this.hbox7.Name = "hbox7";
			this.hbox7.Spacing = 6;
			// Container child hbox7.Gtk.Box+BoxChild
			this.lblCurrentPosition = new global::Gtk.Label ();
			this.lblCurrentPosition.Name = "lblCurrentPosition";
			this.lblCurrentPosition.Xalign = 0F;
			this.lblCurrentPosition.LabelProp = global::Mono.Unix.Catalog.GetString ("[Position]");
			this.hbox7.Add (this.lblCurrentPosition);
			global::Gtk.Box.BoxChild w39 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.lblCurrentPosition]));
			w39.Position = 0;
			w39.Expand = false;
			w39.Fill = false;
			// Container child hbox7.Gtk.Box+BoxChild
			this.hscaleSongPosition = new global::Gtk.HScale (null);
			this.hscaleSongPosition.CanFocus = true;
			this.hscaleSongPosition.Name = "hscaleSongPosition";
			this.hscaleSongPosition.Adjustment.Upper = 9999;
			this.hscaleSongPosition.Adjustment.PageIncrement = 10;
			this.hscaleSongPosition.Adjustment.StepIncrement = 1;
			this.hscaleSongPosition.Adjustment.Value = 10;
			this.hscaleSongPosition.DrawValue = false;
			this.hscaleSongPosition.Digits = 0;
			this.hscaleSongPosition.ValuePos = ((global::Gtk.PositionType)(2));
			this.hbox7.Add (this.hscaleSongPosition);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.hscaleSongPosition]));
			w40.Position = 1;
			// Container child hbox7.Gtk.Box+BoxChild
			this.lblCurrentLength = new global::Gtk.Label ();
			this.lblCurrentLength.Name = "lblCurrentLength";
			this.lblCurrentLength.Xalign = 0F;
			this.lblCurrentLength.LabelProp = global::Mono.Unix.Catalog.GetString ("[Length]");
			this.hbox7.Add (this.lblCurrentLength);
			global::Gtk.Box.BoxChild w41 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.lblCurrentLength]));
			w41.Position = 2;
			w41.Expand = false;
			w41.Fill = false;
			this.vbox5.Add (this.hbox7);
			global::Gtk.Box.BoxChild w42 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.hbox7]));
			w42.Position = 1;
			w42.Expand = false;
			w42.Fill = false;
			this.hbox6.Add (this.vbox5);
			global::Gtk.Box.BoxChild w43 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.vbox5]));
			w43.Position = 0;
			// Container child hbox6.Gtk.Box+BoxChild
			this.vbox8 = new global::Gtk.VBox ();
			this.vbox8.WidthRequest = 180;
			this.vbox8.Name = "vbox8";
			this.vbox8.Spacing = 2;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox11 = new global::Gtk.HBox ();
			this.hbox11.Name = "hbox11";
			this.hbox11.Spacing = 8;
			// Container child hbox11.Gtk.Box+BoxChild
			this.lblTimeShifting = new global::Gtk.Label ();
			this.lblTimeShifting.HeightRequest = 20;
			this.lblTimeShifting.Name = "lblTimeShifting";
			this.lblTimeShifting.Xalign = 0F;
			this.lblTimeShifting.LabelProp = global::Mono.Unix.Catalog.GetString ("Time Shifting");
			this.hbox11.Add (this.lblTimeShifting);
			global::Gtk.Box.BoxChild w44 = ((global::Gtk.Box.BoxChild)(this.hbox11 [this.lblTimeShifting]));
			w44.Position = 0;
			// Container child hbox11.Gtk.Box+BoxChild
			this.btnTimeShiftingMore = new global::Gtk.Button ();
			this.btnTimeShiftingMore.HeightRequest = 20;
			this.btnTimeShiftingMore.CanFocus = true;
			this.btnTimeShiftingMore.Name = "btnTimeShiftingMore";
			this.btnTimeShiftingMore.UseUnderline = true;
			this.btnTimeShiftingMore.Label = global::Mono.Unix.Catalog.GetString ("...");
			this.hbox11.Add (this.btnTimeShiftingMore);
			global::Gtk.Box.BoxChild w45 = ((global::Gtk.Box.BoxChild)(this.hbox11 [this.btnTimeShiftingMore]));
			w45.Position = 1;
			w45.Expand = false;
			w45.Fill = false;
			this.vbox8.Add (this.hbox11);
			global::Gtk.Box.BoxChild w46 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox11]));
			w46.Position = 0;
			w46.Expand = false;
			w46.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hscaleTimeShifting = new global::Gtk.HScale (null);
			this.hscaleTimeShifting.CanFocus = true;
			this.hscaleTimeShifting.Name = "hscaleTimeShifting";
			this.hscaleTimeShifting.Adjustment.Lower = 50;
			this.hscaleTimeShifting.Adjustment.Upper = 150;
			this.hscaleTimeShifting.Adjustment.PageIncrement = 10;
			this.hscaleTimeShifting.Adjustment.StepIncrement = 1;
			this.hscaleTimeShifting.Adjustment.Value = 100;
			this.hscaleTimeShifting.DrawValue = false;
			this.hscaleTimeShifting.Digits = 0;
			this.hscaleTimeShifting.ValuePos = ((global::Gtk.PositionType)(0));
			this.vbox8.Add (this.hscaleTimeShifting);
			global::Gtk.Box.BoxChild w47 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hscaleTimeShifting]));
			w47.Position = 1;
			w47.Expand = false;
			w47.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 4;
			// Container child hbox3.Gtk.Box+BoxChild
			this.lblTimeShiftingReset = new global::Gtk.Label ();
			this.lblTimeShiftingReset.Name = "lblTimeShiftingReset";
			this.lblTimeShiftingReset.Xalign = 0F;
			this.lblTimeShiftingReset.LabelProp = global::Mono.Unix.Catalog.GetString ("Reset");
			this.hbox3.Add (this.lblTimeShiftingReset);
			global::Gtk.Box.BoxChild w48 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.lblTimeShiftingReset]));
			w48.Position = 0;
			// Container child hbox3.Gtk.Box+BoxChild
			this.lblTimeShiftingValue = new global::Gtk.Label ();
			this.lblTimeShiftingValue.Name = "lblTimeShiftingValue";
			this.lblTimeShiftingValue.Xalign = 0F;
			this.lblTimeShiftingValue.LabelProp = global::Mono.Unix.Catalog.GetString ("100 %");
			this.hbox3.Add (this.lblTimeShiftingValue);
			global::Gtk.Box.BoxChild w49 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.lblTimeShiftingValue]));
			w49.Position = 2;
			w49.Expand = false;
			w49.Fill = false;
			this.vbox8.Add (this.hbox3);
			global::Gtk.Box.BoxChild w50 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox3]));
			w50.Position = 2;
			w50.Expand = false;
			w50.Fill = false;
			this.hbox6.Add (this.vbox8);
			global::Gtk.Box.BoxChild w51 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.vbox8]));
			w51.Position = 1;
			w51.Expand = false;
			w51.Fill = false;
			this.vbox3.Add (this.hbox6);
			global::Gtk.Box.BoxChild w52 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox6]));
			w52.Position = 1;
			w52.Expand = false;
			w52.Fill = false;
			this.hbox4.Add (this.vbox3);
			global::Gtk.Box.BoxChild w53 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.vbox3]));
			w53.Position = 1;
			// Container child hbox4.Gtk.Box+BoxChild
			this.vbox9 = new global::Gtk.VBox ();
			this.vbox9.Name = "vbox9";
			this.vbox9.Spacing = 6;
			// Container child vbox9.Gtk.Box+BoxChild
			this.lblVolume = new global::Gtk.Label ();
			this.lblVolume.HeightRequest = 20;
			this.lblVolume.Name = "lblVolume";
			this.lblVolume.Xalign = 0F;
			this.lblVolume.LabelProp = global::Mono.Unix.Catalog.GetString ("Volume");
			this.vbox9.Add (this.lblVolume);
			global::Gtk.Box.BoxChild w54 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.lblVolume]));
			w54.Position = 0;
			w54.Expand = false;
			w54.Fill = false;
			// Container child vbox9.Gtk.Box+BoxChild
			this.vscaleVolume = new global::Gtk.VScale (null);
			this.vscaleVolume.TooltipMarkup = "Changes the playback volume (in percentage).";
			this.vscaleVolume.CanFocus = true;
			this.vscaleVolume.Name = "vscaleVolume";
			this.vscaleVolume.Inverted = true;
			this.vscaleVolume.Adjustment.Upper = 100;
			this.vscaleVolume.Adjustment.PageIncrement = 10;
			this.vscaleVolume.Adjustment.StepIncrement = 1;
			this.vscaleVolume.Adjustment.Value = 100;
			this.vscaleVolume.DrawValue = false;
			this.vscaleVolume.Digits = 0;
			this.vscaleVolume.ValuePos = ((global::Gtk.PositionType)(3));
			this.vbox9.Add (this.vscaleVolume);
			global::Gtk.Box.BoxChild w55 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.vscaleVolume]));
			w55.Position = 1;
			// Container child vbox9.Gtk.Box+BoxChild
			this.lblCurrentVolume = new global::Gtk.Label ();
			this.lblCurrentVolume.Name = "lblCurrentVolume";
			this.lblCurrentVolume.LabelProp = global::Mono.Unix.Catalog.GetString ("100 %");
			this.vbox9.Add (this.lblCurrentVolume);
			global::Gtk.Box.BoxChild w56 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.lblCurrentVolume]));
			w56.Position = 2;
			w56.Expand = false;
			w56.Fill = false;
			this.hbox4.Add (this.vbox9);
			global::Gtk.Box.BoxChild w57 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.vbox9]));
			w57.PackType = ((global::Gtk.PackType)(1));
			w57.Position = 2;
			w57.Expand = false;
			w57.Fill = false;
			this.vboxCurrentSong.Add (this.hbox4);
			global::Gtk.Box.BoxChild w58 = ((global::Gtk.Box.BoxChild)(this.vboxCurrentSong [this.hbox4]));
			w58.Position = 1;
			w58.Expand = false;
			w58.Fill = false;
			this.vboxRight.Add (this.vboxCurrentSong);
			global::Gtk.Box.BoxChild w59 = ((global::Gtk.Box.BoxChild)(this.vboxRight [this.vboxCurrentSong]));
			w59.Position = 0;
			w59.Expand = false;
			w59.Fill = false;
			// Container child vboxRight.Gtk.Box+BoxChild
			this.vpanedLoopsMarkersSongBrowser = new global::Gtk.VPaned ();
			this.vpanedLoopsMarkersSongBrowser.CanFocus = true;
			this.vpanedLoopsMarkersSongBrowser.Name = "vpanedLoopsMarkersSongBrowser";
			this.vpanedLoopsMarkersSongBrowser.Position = 150;
			// Container child vpanedLoopsMarkersSongBrowser.Gtk.Paned+PanedChild
			this.hpanedLoopsMarkers = new global::Gtk.HPaned ();
			this.hpanedLoopsMarkers.CanFocus = true;
			this.hpanedLoopsMarkers.Name = "hpanedLoopsMarkers";
			this.hpanedLoopsMarkers.Position = 350;
			// Container child hpanedLoopsMarkers.Gtk.Paned+PanedChild
			this.vboxLoops = new global::Gtk.VBox ();
			this.vboxLoops.Name = "vboxLoops";
			// Container child vboxLoops.Gtk.Box+BoxChild
			this.hbox8 = new global::Gtk.HBox ();
			this.hbox8.Name = "hbox8";
			this.hbox8.Spacing = 6;
			// Container child hbox8.Gtk.Box+BoxChild
			this.lblLoops = new global::Gtk.Label ();
			this.lblLoops.Name = "lblLoops";
			this.lblLoops.Xalign = 0F;
			this.lblLoops.LabelProp = global::Mono.Unix.Catalog.GetString ("Loops");
			this.hbox8.Add (this.lblLoops);
			global::Gtk.Box.BoxChild w60 = ((global::Gtk.Box.BoxChild)(this.hbox8 [this.lblLoops]));
			w60.Position = 0;
			w60.Expand = false;
			w60.Fill = false;
			// Container child hbox8.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='toolbarLoops'><toolitem name='actionPlayLoop' action='actionPlayLoop'/><toolitem name='editAction' action='editAction'/><toolitem name='addAction' action='addAction'/><toolitem name='actionRemoveLoop' action='actionRemoveLoop'/></toolbar></ui>");
			this.toolbarLoops = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbarLoops")));
			this.toolbarLoops.Name = "toolbarLoops";
			this.toolbarLoops.ShowArrow = false;
			this.toolbarLoops.ToolbarStyle = ((global::Gtk.ToolbarStyle)(0));
			this.toolbarLoops.IconSize = ((global::Gtk.IconSize)(1));
			this.hbox8.Add (this.toolbarLoops);
			global::Gtk.Box.BoxChild w61 = ((global::Gtk.Box.BoxChild)(this.hbox8 [this.toolbarLoops]));
			w61.Position = 1;
			this.vboxLoops.Add (this.hbox8);
			global::Gtk.Box.BoxChild w62 = ((global::Gtk.Box.BoxChild)(this.vboxLoops [this.hbox8]));
			w62.Position = 0;
			w62.Expand = false;
			w62.Fill = false;
			// Container child vboxLoops.Gtk.Box+BoxChild
			this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
			this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
			this.treeLoops = new global::Gtk.TreeView ();
			this.treeLoops.CanFocus = true;
			this.treeLoops.Name = "treeLoops";
			this.GtkScrolledWindow2.Add (this.treeLoops);
			this.vboxLoops.Add (this.GtkScrolledWindow2);
			global::Gtk.Box.BoxChild w64 = ((global::Gtk.Box.BoxChild)(this.vboxLoops [this.GtkScrolledWindow2]));
			w64.Position = 1;
			this.hpanedLoopsMarkers.Add (this.vboxLoops);
			global::Gtk.Paned.PanedChild w65 = ((global::Gtk.Paned.PanedChild)(this.hpanedLoopsMarkers [this.vboxLoops]));
			w65.Resize = false;
			// Container child hpanedLoopsMarkers.Gtk.Paned+PanedChild
			this.vboxLoops1 = new global::Gtk.VBox ();
			this.vboxLoops1.Name = "vboxLoops1";
			// Container child vboxLoops1.Gtk.Box+BoxChild
			this.hbox9 = new global::Gtk.HBox ();
			this.hbox9.Name = "hbox9";
			this.hbox9.Spacing = 6;
			// Container child hbox9.Gtk.Box+BoxChild
			this.lblMarkers = new global::Gtk.Label ();
			this.lblMarkers.Name = "lblMarkers";
			this.lblMarkers.Xalign = 0F;
			this.lblMarkers.LabelProp = global::Mono.Unix.Catalog.GetString ("Markers");
			this.hbox9.Add (this.lblMarkers);
			global::Gtk.Box.BoxChild w66 = ((global::Gtk.Box.BoxChild)(this.hbox9 [this.lblMarkers]));
			w66.Position = 0;
			w66.Expand = false;
			w66.Fill = false;
			// Container child hbox9.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='toolbarMarkers'><toolitem name='actionGoToMarker' action='actionGoToMarker'/><toolitem name='actionAddMarker' action='actionAddMarker'/><toolitem name='actionRemoveMarker' action='actionRemoveMarker'/><toolitem name='actionEditMarker' action='actionEditMarker'/></toolbar></ui>");
			this.toolbarMarkers = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbarMarkers")));
			this.toolbarMarkers.Name = "toolbarMarkers";
			this.toolbarMarkers.ShowArrow = false;
			this.toolbarMarkers.ToolbarStyle = ((global::Gtk.ToolbarStyle)(0));
			this.toolbarMarkers.IconSize = ((global::Gtk.IconSize)(1));
			this.hbox9.Add (this.toolbarMarkers);
			global::Gtk.Box.BoxChild w67 = ((global::Gtk.Box.BoxChild)(this.hbox9 [this.toolbarMarkers]));
			w67.Position = 1;
			this.vboxLoops1.Add (this.hbox9);
			global::Gtk.Box.BoxChild w68 = ((global::Gtk.Box.BoxChild)(this.vboxLoops1 [this.hbox9]));
			w68.Position = 0;
			w68.Expand = false;
			w68.Fill = false;
			// Container child vboxLoops1.Gtk.Box+BoxChild
			this.GtkScrolledWindow3 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow3.Name = "GtkScrolledWindow3";
			this.GtkScrolledWindow3.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow3.Gtk.Container+ContainerChild
			this.treeMarkers = new global::Gtk.TreeView ();
			this.treeMarkers.CanFocus = true;
			this.treeMarkers.Name = "treeMarkers";
			this.GtkScrolledWindow3.Add (this.treeMarkers);
			this.vboxLoops1.Add (this.GtkScrolledWindow3);
			global::Gtk.Box.BoxChild w70 = ((global::Gtk.Box.BoxChild)(this.vboxLoops1 [this.GtkScrolledWindow3]));
			w70.Position = 1;
			this.hpanedLoopsMarkers.Add (this.vboxLoops1);
			this.vpanedLoopsMarkersSongBrowser.Add (this.hpanedLoopsMarkers);
			global::Gtk.Paned.PanedChild w72 = ((global::Gtk.Paned.PanedChild)(this.vpanedLoopsMarkersSongBrowser [this.hpanedLoopsMarkers]));
			w72.Resize = false;
			// Container child vpanedLoopsMarkersSongBrowser.Gtk.Paned+PanedChild
			this.vboxSongBrowser = new global::Gtk.VBox ();
			this.vboxSongBrowser.Name = "vboxSongBrowser";
			// Container child vboxSongBrowser.Gtk.Box+BoxChild
			this.hbox10 = new global::Gtk.HBox ();
			this.hbox10.Name = "hbox10";
			this.hbox10.Spacing = 6;
			// Container child hbox10.Gtk.Box+BoxChild
			this.lblSongBrowser = new global::Gtk.Label ();
			this.lblSongBrowser.Name = "lblSongBrowser";
			this.lblSongBrowser.Xalign = 0F;
			this.lblSongBrowser.LabelProp = global::Mono.Unix.Catalog.GetString ("Song Browser");
			this.hbox10.Add (this.lblSongBrowser);
			global::Gtk.Box.BoxChild w73 = ((global::Gtk.Box.BoxChild)(this.hbox10 [this.lblSongBrowser]));
			w73.Position = 0;
			w73.Expand = false;
			w73.Fill = false;
			// Container child hbox10.Gtk.Box+BoxChild
			this.lblSearchFor = new global::Gtk.Label ();
			this.lblSearchFor.Name = "lblSearchFor";
			this.lblSearchFor.Xalign = 1F;
			this.lblSearchFor.LabelProp = global::Mono.Unix.Catalog.GetString ("Search for:");
			this.hbox10.Add (this.lblSearchFor);
			global::Gtk.Box.BoxChild w74 = ((global::Gtk.Box.BoxChild)(this.hbox10 [this.lblSearchFor]));
			w74.Position = 1;
			// Container child hbox10.Gtk.Box+BoxChild
			this.txtSearch = new global::Gtk.Entry ();
			this.txtSearch.CanFocus = true;
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.IsEditable = true;
			this.txtSearch.InvisibleChar = '•';
			this.hbox10.Add (this.txtSearch);
			global::Gtk.Box.BoxChild w75 = ((global::Gtk.Box.BoxChild)(this.hbox10 [this.txtSearch]));
			w75.Position = 2;
			this.vboxSongBrowser.Add (this.hbox10);
			global::Gtk.Box.BoxChild w76 = ((global::Gtk.Box.BoxChild)(this.vboxSongBrowser [this.hbox10]));
			w76.Position = 0;
			w76.Expand = false;
			w76.Fill = false;
			// Container child vboxSongBrowser.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.treeSongBrowser = new global::Gtk.TreeView ();
			this.treeSongBrowser.CanFocus = true;
			this.treeSongBrowser.Name = "treeSongBrowser";
			this.GtkScrolledWindow.Add (this.treeSongBrowser);
			this.vboxSongBrowser.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w78 = ((global::Gtk.Box.BoxChild)(this.vboxSongBrowser [this.GtkScrolledWindow]));
			w78.Position = 1;
			this.vpanedLoopsMarkersSongBrowser.Add (this.vboxSongBrowser);
			this.vboxRight.Add (this.vpanedLoopsMarkersSongBrowser);
			global::Gtk.Box.BoxChild w80 = ((global::Gtk.Box.BoxChild)(this.vboxRight [this.vpanedLoopsMarkersSongBrowser]));
			w80.Position = 1;
			this.hpanedMain.Add (this.vboxRight);
			this.vboxMain.Add (this.hpanedMain);
			global::Gtk.Box.BoxChild w82 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.hpanedMain]));
			w82.Position = 2;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.statusbar1 = new global::Gtk.Statusbar ();
			this.statusbar1.Name = "statusbar1";
			this.statusbar1.Spacing = 6;
			this.vboxMain.Add (this.statusbar1);
			global::Gtk.Box.BoxChild w83 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.statusbar1]));
			w83.Position = 3;
			w83.Expand = false;
			w83.Fill = false;
			this.Add (this.vboxMain);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultHeight = 646;
			this.Show ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.actionExit.Activated += new global::System.EventHandler (this.OnExitActionActivated);
			this.actionAbout.Activated += new global::System.EventHandler (this.OnAboutActionActivated);
			this.actionHelp.Activated += new global::System.EventHandler (this.OnHelpActionContentsActivated);
			this.aboutAction.Activated += new global::System.EventHandler (this.OnAboutActionActivated);
			this.actionPlay.Activated += new global::System.EventHandler (this.OnActionPlayActivated);
			this.actionPause.Activated += new global::System.EventHandler (this.OnActionPauseActivated);
			this.actionStop.Activated += new global::System.EventHandler (this.OnActionStopActivated);
			this.actionPrevious.Activated += new global::System.EventHandler (this.OnActionPreviousActivated);
			this.actionNext.Activated += new global::System.EventHandler (this.OnActionNextActivated);
			this.actionRepeatType.Activated += new global::System.EventHandler (this.OnActionRepeatTypeActivated);
			this.openAction.Activated += new global::System.EventHandler (this.OnOpenActionActivated);
			this.actionUpdateLibrary.Activated += new global::System.EventHandler (this.OnActionUpdateLibraryActivated);
			this.actionSettings.Activated += new global::System.EventHandler (this.OnActionSettingsActivated);
			this.actionEffects.Activated += new global::System.EventHandler (this.OnActionEffectsActivated);
			this.actionPlaylist.Activated += new global::System.EventHandler (this.OnActionPlaylistActivated);
			this.actionAddFiles.Activated += new global::System.EventHandler (this.OnActionAddFilesActivated);
			this.actionAddFolder.Activated += new global::System.EventHandler (this.OnActionAddFolderActivated);
			this.cboSoundFormat.Changed += new global::System.EventHandler (this.OnSoundFormatChanged);
			this.treeLibraryBrowser.RowActivated += new global::Gtk.RowActivatedHandler (this.OnTreeLibraryBrowserRowActivated);
			this.treeLibraryBrowser.CursorChanged += new global::System.EventHandler (this.OnTreeLibraryBrowserCursorChanged);
			this.treeLibraryBrowser.RowExpanded += new global::Gtk.RowExpandedHandler (this.OnTreeLibraryBrowserRowExpanded);
			this.hscaleTimeShifting1.ValueChanged += new global::System.EventHandler (this.OnTimeShiftingValueChanged);
			this.hscaleSongPosition.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnHscaleSongPositionButtonPressEvent);
			this.hscaleSongPosition.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnHscaleSongPositionButtonReleaseEvent);
			this.hscaleTimeShifting.ValueChanged += new global::System.EventHandler (this.OnTimeShiftingValueChanged);
			this.vscaleVolume.ValueChanged += new global::System.EventHandler (this.OnVolumeValueChanged);
			this.treeSongBrowser.RowActivated += new global::Gtk.RowActivatedHandler (this.OnTreeSongBrowserRowActivated);
		}
	}
}
