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
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.MVP;

namespace MPfm.GTK
{
	/// <summary>
	/// Main window.
	/// </summary>
	public partial class MainWindow: Gtk.Window, IMainView
	{
		// Private variables
		private string currentDirectory = string.Empty;
		
		private MainPresenter presenter = null;
		public MainPresenter Presenter
		{
			get
			{
				return presenter;
			}
		}
		
		private SettingsWindow windowSettings = null;
		private PlaylistWindow windowPlaylist = null;
		private EffectsWindow windowEffects = null;
		private UpdateLibraryWindow windowUpdateLibrary = null;
	
		private Pango.Layout layout = null;
	
		private List<AudioFile> audioFiles = null;
		private TestDevice testDevice = null;
	
		private bool isSongPositionChanging = false;
	
		private Gdk.Pixbuf appIcon = null;
		private StatusIcon statusIcon = null;
		
        private MPfm.Library.Library library = null;
	
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow(): base (Gtk.WindowType.Toplevel)
		{
			Build ();
	
	        // Set form title
	        this.Title = "MPfm: Music Player for Musicians - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA";
	
			// Set application icon (for some reason it is not visible on Unity)
			this.SetIconFromFile("icon48.png");
			
			// Set font properties
			SetFontProperties();
					
			// Set temporary album cover
			Pixbuf stuff = new Pixbuf("icon48.png");
			stuff = stuff.ScaleSimple(150, 150, InterpType.Bilinear);
			this.imageAlbumCover.Pixbuf = stuff;
	
			// Create controller
			presenter = new MainPresenter(this);
			
			// Create song browser columns
			CreateSongBrowserColumns();
			RefreshSongBrowser(new List<AudioFile>());
	
			// Refresh other stuff
			RefreshRepeatButton();
			RefreshSongInformation(new SongInformationEntity());
	
			//#if VER_LINUX
			//		device.Name = "LINUX";
			//#endif
			
			this.cboSoundFormat.GrabFocus();			
		}
	
		/// <summary>
		/// Exits the application.
		/// </summary>
		protected void ExitApplication()
		{
			// Dispose controller
			presenter.Dispose();
			presenter = null;
	
			// Exit application
			Application.Quit();
		}
	
		/// <summary>
		/// Raises the delete event (when the form is closing).
		/// Exits the application.
		/// </summary>
		/// <param name='o'>Object</param>
		/// <param name='args'>Event arguments</param>
		protected void OnDeleteEvent(object sender, DeleteEventArgs a)
		{
			ExitApplication();
			a.RetVal = true;
		}
		
		private void SetFontProperties()
		{				
			// Get default font name
			string defaultFontName = this.lblArtistName.Style.FontDescription.Family;
			this.hscaleSongPosition.AddEvents((int)EventMask.ButtonPressMask | (int)EventMask.ButtonReleaseMask);
			this.lblArtistName.ModifyFont(FontDescription.FromString(defaultFontName +" 16"));
			this.lblAlbumTitle.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
			this.lblSongTitle.ModifyFont(FontDescription.FromString(defaultFontName +" 11"));
			this.lblSongFilePath.ModifyFont(FontDescription.FromString(defaultFontName +" 8"));
	
			this.toolbarMain.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));	
			
			this.lblLibraryBrowser.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));								
			this.lblCurrentSong.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
			this.lblMarkers.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
			this.lblLoops.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
			this.lblSongBrowser.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
						
			//this.lblSearchFor.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			this.lblLibraryFilter.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			
			this.lblCurrentPosition.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
			this.lblCurrentLength.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
						
			this.lblPosition.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			this.lblLength.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			this.lblSongPosition.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			this.lblTimeShifting.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			this.lblInformation.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			this.lblVolume.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			
			this.lblCurrentFileType.ModifyFont(FontDescription.FromString(defaultFontName +" 8"));
			this.lblCurrentBitrate.ModifyFont(FontDescription.FromString(defaultFontName +" 8"));
			this.lblCurrentSampleRate.ModifyFont(FontDescription.FromString(defaultFontName +" 8"));
		}
				
		public void RefreshPlayerPosition(PlayerPositionEntity entity)
		{
			// Invoke UI changes
			Gtk.Application.Invoke(delegate{
				lblCurrentPosition.Text = entity.Position.ToString();
	
				// Check if the user is currently changing the position
				if(!isSongPositionChanging)
				{
					hscaleSongPosition.Value = entity.PositionBytes;
				}
			});
		}
		
		public void RefreshSongInformation(SongInformationEntity entity)
		{
	        // Refresh labels
	        lblArtistName.Text = entity.ArtistName;
	        lblAlbumTitle.Text = entity.AlbumTitle;
	        lblSongTitle.Text = entity.Title;
	        lblSongFilePath.Text = entity.FilePath;
	        lblCurrentLength.Text = entity.Length;
			lblCurrentPosition.Text = entity.Position;
			//hscaleSongPosition.Adjustment.Upper = audioFile			
		}
		
		#region Refresh Methods
	
		/// <summary>
		/// Creates the song browser columns.
		/// </summary>
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
	
			treeSongBrowser.AppendColumn(colPlaybackIcon);
			treeSongBrowser.AppendColumn(colTrackNumber);
			treeSongBrowser.AppendColumn(colSongTitle);
			treeSongBrowser.AppendColumn(colLength);
			treeSongBrowser.AppendColumn(colArtistName);
			treeSongBrowser.AppendColumn(colAlbumTitle);
	
			//colArtistName.AddAttribute(cellArtistName, "text", 0);
			//colAlbumTitle.AddAttribute(cellAlbumTitle, "text", 0);
			//colSongTitle.AddAttribute(cellSongTitle, "text", 0);
		}
	
		/// <summary>
		/// Refreshes the song browser.
		/// </summary>
		/// <param name='audioFiles'>List of audio files to display in the Song Browser.</param>
		protected void RefreshSongBrowser(List<AudioFile> audioFiles)
		{
			// Create store
			Gtk.ListStore musicListStore = new Gtk.ListStore(typeof(AudioFile));
			foreach(AudioFile audioFile in audioFiles)
			{
				musicListStore.AppendValues(audioFile);
			}
	
			// Set model
			treeSongBrowser.Model = musicListStore;
		}
	
		/// <summary>
		/// Renders a cell from the Song Browser.
		/// </summary>
		/// <param name='column'>Column</param>
		/// <param name='cell'>Cell</param>
		/// <param name='model'>Model</param>
		/// <param name='iter'>Iter</param>
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
			
	    /// <summary>
	    /// Refreshes the "Repeat" button in the main window toolbar.
	    /// </summary>
	    public void RefreshRepeatButton()
	    {
	        string repeatOff = "Off";
	        string repeatPlaylist = "Playlist";
	        string repeatSong = "Song";
	
	        // Display the repeat type
	        if (presenter.Player.RepeatType == RepeatType.Playlist)
	        {				
				actionRepeatType.Label = actionRepeatType.ShortLabel = "Repeat Type (" + repeatPlaylist + ")";								
	        }
	        else if (presenter.Player.RepeatType == RepeatType.Song)
	        {				
				actionRepeatType.Label = actionRepeatType.ShortLabel = "Repeat Type (" + repeatSong + ")";
	        }
	        else
	        {				
				actionRepeatType.Label = actionRepeatType.ShortLabel = "Repeat Type (" + repeatOff + ")";
	        }
	    }
		
		#endregion
	
		#region Action Events
	
		/// <summary>
		/// Raises the Exit action activated event.
		/// Exits the application.
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnExitActionActivated(object sender, System.EventArgs e)
		{
			ExitApplication();
		}

		/// <summary>
		/// Raises the Open action activated event.
		/// Opens a file browser to select audio file(s) to play.
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnOpenActionActivated(object sender, System.EventArgs e)
		{
			// Create dialog box
			Gtk.FileChooserDialog dialog = 
				new Gtk.FileChooserDialog("Select the audio files to play.", 
			                                  this, FileChooserAction.Open, 
			                                  "Cancel", ResponseType.Cancel, 
			                                  "Add to playlist", ResponseType.Accept);
			
			// Let the user choose multiple files
			dialog.SelectMultiple = true;
	
			// Show dialog box
			if(dialog.Run() == (int)ResponseType.Accept)
			{
				// Replace playlist
				presenter.Player.Playlist.Clear();
				presenter.Player.Playlist.AddItems(dialog.Filenames.ToList());
				
				// Create list of audio files
				audioFiles = new List<AudioFile>();

				// Refresh song browser
				RefreshSongBrowser(audioFiles);
			}
				
			// Destroy dialog
			dialog.Destroy();
		}

		protected void OnActionAddFilesActivated (object sender, System.EventArgs e)
		{
			// Create dialog box
			Gtk.FileChooserDialog dialog = 
				new Gtk.FileChooserDialog("Select the audio files to add to the library.", 
			                                  this, FileChooserAction.Open, 
			                                  "Cancel", ResponseType.Cancel, 
			                                  "Add files to library", ResponseType.Accept);
			
			// Let the user choose multiple files
			dialog.SelectMultiple = true;			
	
			// Show dialog box
			if(dialog.Run() == (int)ResponseType.Accept)
			{
				
			}
							
			// Destroy dialog
			dialog.Destroy();
		}		
									
		protected void OnActionAddFolderActivated (object sender, System.EventArgs e)
		{
			// Create dialog box			
			Gtk.FileChooserDialog dialog = 
				new Gtk.FileChooserDialog("Select the audio files to add to the library.", 
			                                  this, FileChooserAction.SelectFolder, 
			                                  "Cancel", ResponseType.Cancel, 
			                                  "Add folder to library", ResponseType.Accept);
						
			// Let the user choose multiple files
			dialog.SelectMultiple = true;						
	
			// Show dialog box
			if(dialog.Run() == (int)ResponseType.Accept)
			{
				// Create window if it doesn't exists
				if(windowUpdateLibrary == null)
				{				
					windowUpdateLibrary = new UpdateLibraryWindow(this, UpdateLibraryMode.SpecificFolder, null, dialog.Filename);
				}
				
				// Display window
				windowUpdateLibrary.ShowAll();	
			}
							
			// Destroy dialog
			dialog.Destroy();
		}
		
		protected void OnActionUpdateLibraryActivated(object sender, System.EventArgs e)
		{
			// Create window if it doesn't exists
			if(windowUpdateLibrary == null)
			{				
				windowUpdateLibrary = new UpdateLibraryWindow(this, UpdateLibraryMode.WholeLibrary, null, null);			
			}
			
			// Display window			
			windowUpdateLibrary.ShowAll();			
		}		
					
		protected void OnActionPlayActivated(object sender, System.EventArgs e)
		{
			presenter.Play();
		}
		
		protected void OnActionPauseActivated(object sender, System.EventArgs e)
		{
			presenter.Pause();
		}
		
		protected void OnActionStopActivated(object sender, System.EventArgs e)
		{
			presenter.Stop();
		}
		
		protected void OnActionPreviousActivated(object sender, System.EventArgs e)
		{
			presenter.Previous();
		}

		protected void OnActionNextActivated(object sender, System.EventArgs e)
		{
			presenter.Next();
		}

		protected void OnActionRepeatTypeActivated(object sender, System.EventArgs e)
		{
	        // Cycle through the repeat types
	        if (presenter.Player.RepeatType == RepeatType.Off)
	        {
	            presenter.Player.RepeatType = RepeatType.Playlist;
	        }
	        else if (presenter.Player.RepeatType == RepeatType.Playlist)
	        {
	            presenter.Player.RepeatType = RepeatType.Song;
	        }
	        else
	        {
	            presenter.Player.RepeatType = RepeatType.Off;
	        }
	
	        // Update repeat button
	        RefreshRepeatButton();
		}

		protected void OnActionPlaylistActivated(object sender, System.EventArgs e)
		{
			// Create window if it doesn't exists
			if(windowPlaylist == null)
			{				
				windowPlaylist = new PlaylistWindow(this);			
			}
			
			// Display window			
			windowPlaylist.ShowAll();	
		}

		protected void OnActionEffectsActivated(object sender, System.EventArgs e)
		{							
			// Create window if it doesn't exists
			if(windowEffects == null)
			{				
				windowEffects = new EffectsWindow(this);			
			}
			
			// Display window			
			windowEffects.ShowAll();	
		}
		
		protected void OnActionSettingsActivated(object sender, System.EventArgs e)
		{
			// Create window if it doesn't exists
			if(windowSettings == null)
			{				
				windowSettings = new SettingsWindow(this);			
			}
			
			// Display window			
			windowSettings.ShowAll();		
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
			float value = ((float)vscaleVolume.Value / 100);
			presenter.Player.Volume = value;
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
	
		protected void OnSongPositionMoveSlider(object o, Gtk.MoveSliderArgs args)
		{

		}
	
		protected void OnSongPositionChangeValue(object o, Gtk.ChangeValueArgs args)
		{
			//if(args.Scroll == ScrollType.Jump)
	
			//int test = (int)args.Args[1];
			//controller.Player.SetPosition(args.RetVal);

		}

		#endregion
		

	}
}
