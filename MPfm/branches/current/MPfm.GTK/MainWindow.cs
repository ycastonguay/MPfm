using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using Gtk;
using Gdk;
using Pango;
using MPfm.Core;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.UI;

public partial class MainWindow: Gtk.Window
{
	private MainWindowController controller = null;

	private Pango.Layout layout = null;

	private List<AudioFile> audioFiles = null;
	private TestDevice testDevice = null;
	private Timer timerSongPosition = null;

	private bool isSongPositionChanging = false;

	private Gdk.Pixbuf appIcon = null;
	private StatusIcon statusIcon = null;

	/// <summary>
	/// Initializes a new instance of the <see cref="MainWindow"/> class.
	/// </summary>
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();

        // Set form title
        this.Title = "MPfm: Music Player for Musicians - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA";

		// Set application icon (visible in GNOME but not UNITY)
		this.SetIconFromFile("icon48.png");

//		// Get default font name
//		string defaultFontName = this.lblArtistName.Style.FontDescription.Family;
//		this.hscaleSongPosition.AddEvents((int)EventMask.ButtonPressMask | (int)EventMask.ButtonReleaseMask);
//		this.lblArtistName.ModifyFont(FontDescription.FromString(defaultFontName +" 16"));
//		this.lblAlbumTitle.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
//		this.lblSongTitle.ModifyFont(FontDescription.FromString(defaultFontName +" 10"));
//		this.lblSongFilePath.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));

		//this.drawingArea1.SetSizeRequest(200, 200);

		//drawingArea1.GdkWindow.//

		layout = new Pango.Layout(this.PangoContext);
		layout.Width = Pango.Units.FromPixels(200);
		layout.Wrap = Pango.WrapMode.Word;
		layout.Alignment = Pango.Alignment.Left;
		layout.FontDescription = Pango.FontDescription.FromString("Ubuntu 14");

		// Create controller
		// TODO: BUG CANNOT QUIT THE APPLICATION SINCE MOVING PLAYER TO CONTROLLER.
		controller = new MainWindowController();

		// Create player
		controller.CreatePlayer();
		controller.Player.OnPlaylistIndexChanged += HandlePlayerOnPlaylistIndexChanged;

		// Create song position timer
		timerSongPosition = new Timer(100);
		timerSongPosition.Elapsed += HandleTimerSongPositionElapsed;

		// Create song browser columns
		CreateSongBrowserColumns();
		RefreshSongBrowser(new List<AudioFile>());

		// Refresh other stuff
		RefreshRepeatButton();
		RefreshSongInformation();

		//#if VER_LINUX
		//		device.Name = "LINUX";
		//#endif
	}

	protected void ExitApplication()
	{
		// Dispose controller
		controller.Dispose();
		controller = null;

		// Exit application
		Application.Quit ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		ExitApplication();
		a.RetVal = true;
	}

	protected void HandlePlayerOnPlaylistIndexChanged (PlayerPlaylistIndexChangedData data)
	{
		// Check if the playback has stopped
		if(data.IsPlaybackStopped)
		{
			return;
		}

		// Refresh song information
		RefreshSongInformation();
	}

	protected void HandleTimerSongPositionElapsed (object sender, ElapsedEventArgs e)
	{
		// Get position
		PlayerPositionEntity position = controller.GetPlayerPosition();

		// Invoke UI changes
		Gtk.Application.Invoke(delegate{
			//lblCurrentPosition.Text = position.Position.ToString();

			// Check if the user is currently changing the position
			if(!isSongPositionChanging)
			{
				//hscaleSongPosition.Value = position.PositionBytes;
			}
		});
	}

	protected void CreateSongBrowserColumns()
	{
		Gtk.TreeViewColumn colPlaybackIcon = new Gtk.TreeViewColumn();
		Gtk.TreeViewColumn colTrackNumber = new Gtk.TreeViewColumn();
		Gtk.TreeViewColumn colArtistName = new Gtk.TreeViewColumn();
		Gtk.TreeViewColumn colAlbumTitle = new Gtk.TreeViewColumn();
		Gtk.TreeViewColumn colSongTitle = new Gtk.TreeViewColumn();
		Gtk.TreeViewColumn colLength = new Gtk.TreeViewColumn();

		colPlaybackIcon.Title = string.Empty;
		colTrackNumber.Title = "Tr#";
		colArtistName.Title = "Artist Name";
		colAlbumTitle.Title = "Album Title";
		colSongTitle.Title = "Title";
		colLength.Title = "Length";

		colArtistName.Data.Add("Property", "ArtistName");
		colAlbumTitle.Data.Add("Property", "AlbumTitle");
		colSongTitle.Data.Add("Property", "Title");
		colLength.Data.Add("Property", "Length");
		colTrackNumber.Data.Add("Property", "TrackNumber");

		colPlaybackIcon.Resizable = false;
		colTrackNumber.Resizable = true;
		colArtistName.Resizable = true;
		colAlbumTitle.Resizable = true;
		colSongTitle.Resizable = true;
		colLength.Resizable = true;

		Gtk.CellRendererText cellArtistName = new Gtk.CellRendererText();
		Gtk.CellRendererText cellAlbumTitle = new Gtk.CellRendererText();
		Gtk.CellRendererText cellSongTitle = new Gtk.CellRendererText();
		Gtk.CellRendererText cellPlaybackIcon = new Gtk.CellRendererText();
		Gtk.CellRendererText cellTrackNumber = new Gtk.CellRendererText();
		Gtk.CellRendererText cellLength = new Gtk.CellRendererText();

		colArtistName.PackStart(cellArtistName, true);
		colAlbumTitle.PackStart(cellAlbumTitle, true);
		colSongTitle.PackStart(cellSongTitle, true);
		colPlaybackIcon.PackStart(cellPlaybackIcon, true);
		colTrackNumber.PackStart(cellTrackNumber, true);
		colLength.PackStart(cellLength, true);

		colArtistName.SetCellDataFunc(cellArtistName, new Gtk.TreeCellDataFunc(RenderSongBrowserCell));
		colAlbumTitle.SetCellDataFunc(cellAlbumTitle, new Gtk.TreeCellDataFunc(RenderSongBrowserCell));
		colSongTitle.SetCellDataFunc(cellSongTitle, new Gtk.TreeCellDataFunc(RenderSongBrowserCell));
		colPlaybackIcon.SetCellDataFunc(cellPlaybackIcon, new Gtk.TreeCellDataFunc(RenderSongBrowserCell));
		colTrackNumber.SetCellDataFunc(cellTrackNumber, new Gtk.TreeCellDataFunc(RenderSongBrowserCell));
		colLength.SetCellDataFunc(cellLength, new Gtk.TreeCellDataFunc(RenderSongBrowserCell));

//		treeSongBrowser.AppendColumn(colPlaybackIcon);
//		treeSongBrowser.AppendColumn(colTrackNumber);
//		treeSongBrowser.AppendColumn(colSongTitle);
//		treeSongBrowser.AppendColumn(colLength);
//		treeSongBrowser.AppendColumn(colArtistName);
//		treeSongBrowser.AppendColumn(colAlbumTitle);
	}

	protected void RefreshSongBrowser(List<AudioFile> audioFiles)
	{
		// Create store
		Gtk.ListStore musicListStore = new Gtk.ListStore(typeof(AudioFile));
		foreach(AudioFile audioFile in audioFiles)
		{
			musicListStore.AppendValues(audioFile);
		}

		// Set model
		//treeSongBrowser.Model = musicListStore;
	}

	private void RenderSongBrowserCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		// Get model data
		AudioFile audioFile = (AudioFile)model.GetValue(iter, 0);

		// Get property name
		string property = (string)column.Data["Property"];
		if(String.IsNullOrEmpty(property))
		{
			return;
		}

		// Get value
		PropertyInfo propertyInfo = typeof(AudioFile).GetProperty(property);
		object propertyValue = propertyInfo.GetValue(audioFile, null);

		// Set cell text
		(cell as Gtk.CellRendererText).Text = propertyValue.ToString();
	}

	protected void RefreshSongInformation()
	{
//		// Check if the current item is valid
//		if(controller.Player.Playlist.CurrentItem == null)
//		{
//	        // Set empty values
//	        lblArtistName.Text = string.Empty;
//	        lblAlbumTitle.Text = string.Empty;
//	        lblSongTitle.Text = string.Empty;
//	        lblSongFilePath.Text = "No song currently playing.";
//			lblCurrentPosition.Text = "0:00.000";
//	        lblLength.Text = "0:00.000";
//			hscaleSongPosition.Value = 0;
//		}
//		else
//		{
//	        // Set metadata and file path labels
//	        lblArtistName.Text = controller.Player.Playlist.CurrentItem.AudioFile.ArtistName;
//	        lblAlbumTitle.Text = controller.Player.Playlist.CurrentItem.AudioFile.AlbumTitle;
//	        lblSongTitle.Text = controller.Player.Playlist.CurrentItem.AudioFile.Title;
//	        lblSongFilePath.Text = controller.Player.Playlist.CurrentItem.AudioFile.FilePath;
//	        lblLength.Text = controller.Player.Playlist.CurrentItem.LengthString;
//			hscaleSongPosition.Adjustment.Upper = controller.Player.Playlist.CurrentItem.LengthBytes;
//		}
	}

    /// <summary>
    /// Refreshes the "Repeat" button in the main window toolbar.
    /// </summary>
    public void RefreshRepeatButton()
    {
        string repeatOff = "Off";
        string repeatPlaylist = "Playlist";
        string repeatSong = "Song";

//        // Display the repeat type
//        if (controller.Player.RepeatType == RepeatType.Playlist)
//        {
//			lblRepeatType.Text = repeatPlaylist;
//			actionRepeatType.Label = "Repeat Type (" + repeatPlaylist + ")";
//        }
//        else if (controller.Player.RepeatType == RepeatType.Song)
//        {
//			lblRepeatType.Text = repeatSong;
//			actionRepeatType.Label = "Repeat Type (" + repeatSong + ")";
//        }
//        else
//        {
//			lblRepeatType.Text = repeatOff;
//			actionRepeatType.Label = "Repeat Type (" + repeatOff + ")";
//        }
    }

	#region Action Events

	protected void OnExitActionActivated(object sender, System.EventArgs e)
	{
		ExitApplication();
	}

	protected void OnOpenActionActivated(object sender, System.EventArgs e)
	{
		// Create dialog box
		Gtk.FileChooserDialog dialog = 
			new Gtk.FileChooserDialog("Select the audio file(s) to play.", 
		                                  this, FileChooserAction.Open, 
		                                  "Cancel", ResponseType.Cancel, 
		                                  "Open", ResponseType.Accept);
		
		// Let the user choose multiple files
		dialog.SelectMultiple = true;

		// Show dialog box
		if(dialog.Run() == (int)ResponseType.Accept)
		{
			//player.PlayFiles(dialog.Filenames.ToList());

			// Create list of audio files
			audioFiles = new List<AudioFile>();

			// Read audio files
			for(int a = 0; a < dialog.Filenames.Length; a++)
			{
				// Create audio file and add to list
				AudioFile audioFile = new AudioFile(dialog.Filenames[a]);
				audioFiles.Add(audioFile);
			}

			// Refresh song browser
			RefreshSongBrowser(audioFiles);
		}

		// Destroy dialog
		dialog.Destroy();
	}

	protected void OnPlayClicked(object sender, System.EventArgs e)
	{
		if(audioFiles == null)
		{
			return;
		}

		// Play playlist
		controller.Player.PlayFiles(audioFiles);

		// Refresh song information
		RefreshSongInformation();

		// Start timer
		timerSongPosition.Start();
	}

	protected void OnPauseClicked(object sender, System.EventArgs e)
	{
		// Pause player
		controller.Player.Pause();
	}

	protected void OnStopClicked(object sender, System.EventArgs e)
	{
		// Check if the player is playing
		if(controller.Player.IsPlaying)
		{
			// Stop player
			controller.Player.Stop();
	
			// Stop timer
			timerSongPosition.Stop();
		}
	}

	protected void OnPreviousClicked(object sender, System.EventArgs e)
	{
		// Go to previous song
		controller.Player.Previous();

		// Refresh controls
		RefreshSongInformation();
	}

	protected void OnNextClicked(object sender, System.EventArgs e)
	{
		// Go to next song
		controller.Player.Next();

		// Refresh controls
		RefreshSongInformation();
	}

	protected void OnRepeatClicked(object sender, System.EventArgs e)
	{
        // Cycle through the repeat types
        if (controller.Player.RepeatType == RepeatType.Off)
        {
            controller.Player.RepeatType = RepeatType.Playlist;
        }
        else if (controller.Player.RepeatType == RepeatType.Playlist)
        {
            controller.Player.RepeatType = RepeatType.Song;
        }
        else
        {
            controller.Player.RepeatType = RepeatType.Off;
        }

        // Update repeat button
        RefreshRepeatButton();
	}

	protected void OnPlaylistClicked(object sender, System.EventArgs e)
	{

	}

	protected void OnEffectsClicked(object sender, System.EventArgs e)
	{

	}

	protected void OnSettingsClicked(object sender, System.EventArgs e)
	{

	}

	protected void OnUpdateLibraryClicked(object sender, System.EventArgs e)
	{

	}

	protected void OnAboutActionActivated(object sender, System.EventArgs e)
	{
		string text = this.Title + "\nMPfm: Music Player for Musicians is © 2011-2012 Yanick Castonguay and is released under the GPLv3 license.";
		text += "\nThe BASS audio library is © 1999-2012 Un4seen Developments.";
	    text += "\nThe BASS.NET audio library is © 2005-2012 radio42.";

		MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, text);
		md.Run ();
		md.Destroy ();
	}

	protected void OnHelpActionContentsActivated(object sender, System.EventArgs e)
	{

	}

	protected void OnVolumeValueChanged(object sender, System.EventArgs e)
	{
		// Set player volume
		//float value = ((float)vscaleVolume.Value / 100);
		//controller.Player.Volume = value;
	}

	protected void OnSongPositionValueChanged(object sender, System.EventArgs e)
	{

	}

	protected void OnSongPositionButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
	{
		//isSongPositionChanging = true;
	}

	protected void OnSongPositionButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
	{
		//isSongPositionChanging = false;
	}


	protected void OnSongPositionMoveSlider (object o, Gtk.MoveSliderArgs args)
	{
		int a = 0;
	}

	protected void OnSongPositionChangeValue (object o, Gtk.ChangeValueArgs args)
	{
		//if(args.Scroll == ScrollType.Jump)

		//int test = (int)args.Args[1];
		//controller.Player.SetPosition(args.RetVal);

		int a = 0;
	}
	#endregion
}
