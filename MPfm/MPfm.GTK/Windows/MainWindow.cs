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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Gtk;
using Gdk;
using Pango;
using Mono.Unix;
using System.Drawing.Imaging;
using System.Text;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Models;
using MPfm.Library.UpdateLibrary;
using MPfm.GTK.Helpers;
using MPfm.MVP.Messages;

namespace MPfm.GTK.Windows
{
	/// <summary>
	/// Main window.
	/// </summary>
	public partial class MainWindow: BaseWindow, IMainView
	{
		private UpdateLibraryWindow windowUpdateLibrary = null;
	
		private bool isSongPositionChanging = false;
		
		private Gtk.TreeStore storeLibraryBrowser = null;
		private Gtk.ListStore storeSongBrowser = null;
		private Gtk.ListStore storeAudioFileFormat = null;
		
        #region View Actions
        
        public System.Action OnOpenPreferencesWindow { get; set; }
        public System.Action OnOpenEffectsWindow { get; set; }
        public System.Action OnOpenPlaylistWindow { get; set; }
		public System.Action OnOpenSyncWindow { get; set; }
        public System.Action OnPlayerPlay { get; set; }
        public System.Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        public System.Action OnPlayerPause { get; set; }
        public System.Action OnPlayerStop { get; set; }
        public System.Action OnPlayerPrevious { get; set; }
        public System.Action OnPlayerNext { get; set; } 
        public System.Action<float> OnPlayerSetPitchShifting { get; set; }
        public System.Action<float> OnPlayerSetTimeShifting { get; set; }
        public System.Action<float> OnPlayerSetVolume { get; set; }
        public System.Action<float> OnPlayerSetPosition { get; set; }
		public Func<float, PlayerPositionEntity> OnPlayerRequestPosition { get; set; }
        
        public System.Action<AudioFileFormat> OnAudioFileFormatFilterChanged { get; set; }
        public System.Action<LibraryBrowserEntity> OnTreeNodeSelected { get; set; }
        public System.Action<LibraryBrowserEntity, object> OnTreeNodeExpanded { get; set; }     
        public System.Action<LibraryBrowserEntity> OnTreeNodeDoubleClicked { get; set; }
        public Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable { get; set; }
        
        public System.Action<AudioFile> OnTableRowDoubleClicked { get; set; }
        
        #endregion
		
		#region Application Init/Destroy
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow(Action<IBaseView> onViewReady): base (Gtk.WindowType.Toplevel, onViewReady)
		{			
			Build();			
			Icon = new Pixbuf(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/icon48.png");
					
	        // Set form title
	        this.Title = "MPfm: Music Player for Musicians - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA";
	
			// Set application icon (for some reason it is not visible on Unity)
			this.SetIconFromFile("icon48.png");
			
			// Set font properties
			SetFontProperties();
			
			// Initialize browsers
			InitializeSongBrowser();
			InitializeLibraryBrowser();

			// Force refresh song browser to create columns
			RefreshSongBrowser(new List<AudioFile>());		
	
			// Refresh other stuff
			RefreshRepeatButton();
			RefreshSongInformation(null, 0, 0, 0);			
			
			// Fill sound format combo box
			storeAudioFileFormat = new ListStore(typeof(string));			
			storeAudioFileFormat.AppendValues("All");
			storeAudioFileFormat.AppendValues("FLAC");
			storeAudioFileFormat.AppendValues("MP3");					
			storeAudioFileFormat.AppendValues("MPC");
			storeAudioFileFormat.AppendValues("OGG");
			storeAudioFileFormat.AppendValues("WAV");
			storeAudioFileFormat.AppendValues("WV");
			cboSoundFormat.Model = storeAudioFileFormat;								
			
			// Select first item
			Gtk.TreeIter iter;
			cboSoundFormat.Model.IterNthChild(out iter, 0);
			cboSoundFormat.SetActiveIter(iter);
			
			// Set focus to something else than the toolbar (for some reason, the first button is selected)
			cboSoundFormat.GrabFocus();
						
			this.hscaleSongPosition.AddEvents((int)EventMask.ButtonPressMask | (int)EventMask.ButtonReleaseMask);
			
			// Set view as ready
			onViewReady.Invoke(this);
		}
	
		/// <summary>
		/// Exits the application.
		/// </summary>
		protected void ExitApplication()
		{
			// Exit application
			Console.WriteLine("MainWindow - ExitApplication");
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
		
		#endregion
		
		#region Song Browser Methods
	
		/// <summary>
		/// Creates the song browser columns.
		/// </summary>
		protected void InitializeSongBrowser()
		{
			// Create store
			storeSongBrowser = new Gtk.ListStore(typeof(AudioFile));

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
			try
			{
				//Console.WriteLine("MainWindow - RenderSongBrowserCell");
				// Get model data
				AudioFile audioFile = (AudioFile)model.GetValue(iter, 0);
				if(audioFile == null)
					return;
		
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
				//(cell as Gtk.CellRendererText).Text = property;
			}
			catch(Exception ex)
			{
				Console.WriteLine("RenderSongBrowserCell - Exception: " + ex.Message + "\n" + ex.StackTrace);
				throw ex;
			}
		}
		
		#endregion
		
		#region Library Browser Methods
				
		protected void InitializeLibraryBrowser()
		{
			// Create store
			storeLibraryBrowser = new Gtk.TreeStore(typeof(LibraryBrowserEntity));						

			// Hide header
			treeLibraryBrowser.HeadersVisible = false;

			// Create title column
			Gtk.TreeViewColumn colTitle = new Gtk.TreeViewColumn();
			Gtk.CellRendererText cellTitle = new Gtk.CellRendererText();	
			colTitle.Data.Add("Property", "Title");
			colTitle.PackStart(cellTitle, true);
			colTitle.SetCellDataFunc(cellTitle, new Gtk.TreeCellDataFunc(RenderLibraryBrowserCell));
			treeLibraryBrowser.AppendColumn(colTitle);
		}
		
		protected void CreateLibraryBrowserStore(Gtk.TreeStore store, Nullable<Gtk.TreeIter> iter, IEnumerable<LibraryBrowserEntity> items)
		{			
			// Loop through entities
			Console.WriteLine("MainWindow - CreateLibraryBrowserStore");
			foreach(LibraryBrowserEntity entity in items)
			{
				Nullable<Gtk.TreeIter> currentIter = null;
				if(iter == null)
				{
					currentIter = store.AppendValues(entity);
				}
				else
				{
					store.AppendValues(iter, entity);
					currentIter = iter;
				}
				
				// Create new iter and subiters
				if(entity.SubItems != null)
				{					
					CreateLibraryBrowserStore(store, currentIter, entity.SubItems);
				}
			}
		}
			
		/// <summary>
		/// Renders a cell from the Song Browser.
		/// </summary>
		/// <param name='column'>Column</param>
		/// <param name='cell'>Cell</param>
		/// <param name='model'>Model</param>
		/// <param name='iter'>Iter</param>
		private void RenderLibraryBrowserCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			// Get model data
			//Console.WriteLine("MainWindow - RenderLibraryBrowserCell");
			LibraryBrowserEntity entity = (LibraryBrowserEntity)model.GetValue(iter, 0);
	
			// Get property name
			string property = (string)column.Data["Property"];
			if(String.IsNullOrEmpty(property))			
				return;			
	
			// Get value
			PropertyInfo propertyInfo = typeof(LibraryBrowserEntity).GetProperty(property);
			object propertyValue = propertyInfo.GetValue(entity, null);
	
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
//	        if (playerPresenter.Player.RepeatType == RepeatType.Playlist)
//	        {				
//				actionRepeatType.Label = actionRepeatType.ShortLabel = "Repeat Type (" + repeatPlaylist + ")";								
//	        }
//	        else if (playerPresenter.Player.RepeatType == RepeatType.Song)
//	        {				
//				actionRepeatType.Label = actionRepeatType.ShortLabel = "Repeat Type (" + repeatSong + ")";
//	        }
//	        else
//	        {				
//				actionRepeatType.Label = actionRepeatType.ShortLabel = "Repeat Type (" + repeatOff + ")";
//	        }
	    }

		#endregion
	
		#region Other Methods
		
		private void SetFontProperties()
		{				
			// Get default font name
			string defaultFontName = this.lblArtistName.Style.FontDescription.Family;			
			string titleFontName = "Sans Bold 10";
			string subtitleFontName = "Sans Bold 8";
			string buttonFontName = "Sans Bold 8";
			string textFontName = "Sans 8";
			string largePositionFontName = "Mono Bold 11";
			
			this.btnPitchShiftingMore.ModifyFont(FontDescription.FromString(buttonFontName));
			this.btnTimeShiftingMore.ModifyFont(FontDescription.FromString(buttonFontName));
			
			this.lblArtistName.ModifyFont(FontDescription.FromString(defaultFontName +" 16"));
			this.lblAlbumTitle.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
			this.lblSongTitle.ModifyFont(FontDescription.FromString(defaultFontName +" 11"));
			this.lblSongFilePath.ModifyFont(FontDescription.FromString(defaultFontName +" 8"));
	
			this.toolbarMain.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));	
			
			this.lblLibraryBrowser.ModifyFont(FontDescription.FromString(titleFontName));								
			this.lblSongBrowser.ModifyFont(FontDescription.FromString(titleFontName));
			this.lblCurrentSong.ModifyFont(FontDescription.FromString(titleFontName));			
			this.lblLoops.ModifyFont(FontDescription.FromString(titleFontName));
			this.lblMarkers.ModifyFont(FontDescription.FromString(titleFontName));
			
			//this.lblSearchFor.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			this.lblLibraryFilter.ModifyFont(FontDescription.FromString(textFontName));			
				
			this.lblCurrentPosition.ModifyFont(FontDescription.FromString(largePositionFontName));
			this.lblCurrentLength.ModifyFont(FontDescription.FromString(largePositionFontName));
			
			this.lblSongPosition.ModifyFont(FontDescription.FromString(subtitleFontName));
			
			this.lblTimeShifting.ModifyFont(FontDescription.FromString(subtitleFontName));
			this.lblTimeShiftingReset.ModifyFont(FontDescription.FromString(textFontName));
			this.lblTimeShiftingValue.ModifyFont(FontDescription.FromString(textFontName));
			this.lblOriginalTempo.ModifyFont(FontDescription.FromString(textFontName));
			this.lblOriginalTempoBPM.ModifyFont(FontDescription.FromString(textFontName));
			this.btnDetectTempo.ModifyFont(FontDescription.FromString(textFontName));			
			
			this.lblPitchShifting.ModifyFont(FontDescription.FromString(subtitleFontName));
			this.lblPitchShiftingReset.ModifyFont(FontDescription.FromString(textFontName));
			this.lblPitchShiftingValue.ModifyFont(FontDescription.FromString(textFontName));
			
			this.lblInformation.ModifyFont(FontDescription.FromString(subtitleFontName));
			this.lblVolume.ModifyFont(FontDescription.FromString(subtitleFontName));
			
			this.lblCurrentFileType.ModifyFont(FontDescription.FromString(textFontName));
			this.lblCurrentBitrate.ModifyFont(FontDescription.FromString(textFontName));
			this.lblCurrentSampleRate.ModifyFont(FontDescription.FromString(textFontName));
			this.lblCurrentBitsPerSample.ModifyFont(FontDescription.FromString(textFontName));
						
			this.lblTimeShiftingValue.ModifyFont(FontDescription.FromString(textFontName));
			this.lblCurrentVolume.ModifyFont(FontDescription.FromString(textFontName));
		}
		
		private AudioFileFormat GetCurrentAudioFileFormatFilter()
		{
			// Get current audio file format
			Gtk.TreeIter iter;
			AudioFileFormat format;
			cboSoundFormat.GetActiveIter(out iter);
			string filter = storeAudioFileFormat.GetValue(iter, 0).ToString();
			Enum.TryParse<AudioFileFormat>(filter, out format);
			
			return format;
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
				//playerPresenter.Player.Playlist.Clear();
				//playerPresenter.Player.Playlist.AddItems(dialog.Filenames.ToList());
				
				// Create list of audio files
				//audioFiles = new List<AudioFile>();
			}
				
			// Destroy dialog
			dialog.Destroy();
		}

		protected void OnActionAddFilesActivated(object sender, System.EventArgs e)
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
									
		protected void OnActionAddFolderActivated(object sender, System.EventArgs e)
		{
			// Create dialog box			
			Gtk.FileChooserDialog dialog = 
				new Gtk.FileChooserDialog("Select the audio files to add to the library.", 
			                                  this, FileChooserAction.SelectFolder, 
			                                  "Cancel", ResponseType.Cancel, 
			                                  "Add folder to library", ResponseType.Accept);
						
			// Let the user choose multiple files
			//dialog.SelectMultiple = true;						
	
			// Show dialog box
			if(dialog.Run() == (int)ResponseType.Accept)
			{			
				// Destroy window if it still exists
				if(windowUpdateLibrary != null)
				{								
					windowUpdateLibrary.Destroy();				
				}
				
				// Create and display window
				windowUpdateLibrary = new UpdateLibraryWindow(UpdateLibraryMode.SpecificFolder, null, dialog.Filename, null);		
				windowUpdateLibrary.ShowAll();	
			}
							
			// Destroy dialog
			dialog.Destroy();
		}
		
		protected void OnActionUpdateLibraryActivated(object sender, System.EventArgs e)
		{
			// Destroy window if it still exists
			if(windowUpdateLibrary != null)
			{								
				windowUpdateLibrary.Destroy();				
			}
			
			// Create and display window
			windowUpdateLibrary = new UpdateLibraryWindow(UpdateLibraryMode.WholeLibrary, null, null, null);		
			windowUpdateLibrary.ShowAll();
		}		
					
		protected void OnActionPlayActivated(object sender, System.EventArgs e)
		{
			//playerPresenter.Play();
			if(OnPlayerPlay != null)
				OnPlayerPlay.Invoke();
		}
		
		protected void OnActionPauseActivated(object sender, System.EventArgs e)
		{
			//playerPresenter.Pause();			
			if(OnPlayerPause != null)
				OnPlayerPause.Invoke();
		}
		
		protected void OnActionStopActivated(object sender, System.EventArgs e)
		{
			//playerPresenter.Stop();
			if(OnPlayerStop != null)
				OnPlayerStop.Invoke();
		}
		
		protected void OnActionPreviousActivated(object sender, System.EventArgs e)
		{
			//playerPresenter.Previous();
			if(OnPlayerPrevious != null)
				OnPlayerPrevious.Invoke();			
		}

		protected void OnActionNextActivated(object sender, System.EventArgs e)
		{
			//playerPresenter.Next();
			if(OnPlayerNext != null)
				OnPlayerNext.Invoke();
		}

		protected void OnActionRepeatTypeActivated(object sender, System.EventArgs e)
		{
//	        // Cycle through the repeat types
//	        if (playerPresenter.Player.RepeatType == RepeatType.Off)
//	        {
//	            playerPresenter.Player.RepeatType = RepeatType.Playlist;
//	        }
//	        else if (playerPresenter.Player.RepeatType == RepeatType.Playlist)
//	        {
//	            playerPresenter.Player.RepeatType = RepeatType.Song;
//	        }
//	        else
//	        {
//	            playerPresenter.Player.RepeatType = RepeatType.Off;
//	        }
	
	        // Update repeat button
	        RefreshRepeatButton();
		}

		protected void OnActionPlaylistActivated(object sender, System.EventArgs e)
		{
		    OnOpenPlaylistWindow();
		}

		protected void OnActionEffectsActivated(object sender, System.EventArgs e)
		{							
		    OnOpenEffectsWindow();
		}
		
		protected void OnActionSettingsActivated(object sender, System.EventArgs e)
		{
			OnOpenPreferencesWindow();
		}

		protected void OnActionSyncLibrary(object sender, EventArgs e)
		{
            OnOpenSyncWindow();
		}

		protected void OnAboutActionActivated(object sender, System.EventArgs e)
		{
			string text = this.Title + "\nMPfm: Music Player for Musicians is © 2011-2012 Yanick Castonguay and is released under the GPLv3 license.";
			text += "\nThe BASS audio library is © 1999-2012 Un4seen Developments.";
		    text += "\nThe BASS.NET audio library is © 2005-2012 radio42.";
	

			MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, text);
			md.Run();
			md.Destroy();
		}
	
		protected void OnHelpActionContentsActivated(object sender, System.EventArgs e)
		{
	
		}

		protected void OnVolumeValueChanged(object sender, System.EventArgs e)
		{
			// Set player volume			
			//playerPresenter.SetVolume((float)vscaleVolume.Value);
			if(OnPlayerSetVolume != null)
				OnPlayerSetVolume.Invoke((float)vscaleVolume.Value);
		}
		
		[GLib.ConnectBefore]
		protected void OnHscaleSongPositionButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			isSongPositionChanging = true;
		}
		
		[GLib.ConnectBefore]
		protected void OnHscaleSongPositionButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
		{
			// Set new position
			float value = (float)hscaleSongPosition.Value / 100;
			//playerPresenter.SetPosition(value);
			if(OnPlayerSetPosition != null)
				OnPlayerSetPosition.Invoke(value);
			isSongPositionChanging = false;
		}
		
		protected void OnTimeShiftingValueChanged(object sender, System.EventArgs e)
		{
			//playerPresenter.SetTimeShifting((float)hscaleTimeShifting.Value);
			if(OnPlayerSetTimeShifting != null)
				OnPlayerSetTimeShifting.Invoke((float)hscaleTimeShifting.Value);
		}

		protected void OnSoundFormatChanged(object sender, System.EventArgs e)
		{			
			//libraryBrowserPresenter.AudioFileFormatFilterChanged(GetCurrentAudioFileFormatFilter());
			if(OnAudioFileFormatFilterChanged != null)
				OnAudioFileFormatFilterChanged.Invoke(GetCurrentAudioFileFormatFilter());
		}
		
		/// <summary>
		/// Raises the Library Browser row activated event (when the user double-clicks on an item).
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnTreeLibraryBrowserRowActivated(object sender, Gtk.RowActivatedArgs args)
		{
			// Get selected item
			TreeModel model;
			TreeIter iter;	
			if((sender as TreeView).Selection.GetSelected(out model, out iter))
			{
				// Get entity and change query 
				LibraryBrowserEntity entity = (LibraryBrowserEntity)storeLibraryBrowser.GetValue(iter, 0);								
				//libraryBrowserPresenter.TreeNodeDoubleClicked(entity);				
				if(OnTreeNodeDoubleClicked != null)
					OnTreeNodeDoubleClicked.Invoke(entity);	
			}
		}
		
		/// <summary>
		/// Raises the Library Browser cursor changed event (when the user selects an item).
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnTreeLibraryBrowserCursorChanged(object sender, System.EventArgs e)
		{
			// Get selected item
			TreeModel model;
			TreeIter iter;	
			if((sender as TreeView).Selection.GetSelected(out model, out iter))
			{
				// Get entity and change query 
				LibraryBrowserEntity entity = (LibraryBrowserEntity)storeLibraryBrowser.GetValue(iter, 0);								
				//libraryBrowserPresenter.TreeNodeSelected(entity);
				if(OnTreeNodeSelected != null)
					OnTreeNodeSelected.Invoke(entity);				
				//songBrowserPresenter.ChangeQuery(entity.Query);
			}
		}
		
		/// <summary>
		/// Raises the Library Browser row expanded event (when the user expands an item).
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnTreeLibraryBrowserRowExpanded(object sender, Gtk.RowExpandedArgs args)
		{
			// Get data
			Gtk.TreeIter iter;
			LibraryBrowserEntity entity = (LibraryBrowserEntity)storeLibraryBrowser.GetValue(args.Iter, 0);						

			// Check for dummy node		
			storeLibraryBrowser.IterChildren(out iter, args.Iter);
			LibraryBrowserEntity entityChildren = (LibraryBrowserEntity)storeLibraryBrowser.GetValue(iter, 0);			
			if(entityChildren.Type == LibraryBrowserEntityType.Dummy)
			{	
				// Send update to presenter
				//libraryBrowserPresenter.TreeNodeExpanded(entity, args.Iter);
				if(OnTreeNodeExpanded != null)
					OnTreeNodeExpanded.Invoke(entity, args.Iter);

				// Remove dummy node
				storeLibraryBrowser.Remove(ref iter);						
			}	
		}

		/// <summary>
		/// Raises the Song Bowser row activated event (when the user double-clicks on the row).
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnTreeSongBrowserRowActivated(object sender, Gtk.RowActivatedArgs args)
		{
			// Get selected item
			TreeModel model;
			TreeIter iter;	
			if((sender as TreeView).Selection.GetSelected(out model, out iter))
			{
				// Get entity and change query 
				AudioFile audioFile = (AudioFile)storeSongBrowser.GetValue(iter, 0);				
				//songBrowserPresenter.TableRowDoubleClicked(audioFile);
				if(OnTableRowDoubleClicked != null)
					OnTableRowDoubleClicked.Invoke(audioFile);
			}
		}
		
		#endregion
	
		private static Gdk.Pixbuf ImageToPixbuf(System.Drawing.Image image)
		{
			using (MemoryStream stream = new MemoryStream()) {
				image.Save(stream, ImageFormat.Bmp);
				stream.Position = 0;
				Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(stream);	
				return pixbuf;
			}
		}
				
		#region IPlayerView implementation
			
		public void RefreshPlayerStatus(PlayerStatusType status)
		{
		}

		public void RefreshPlayerPosition(PlayerPositionEntity entity)
		{
			Gtk.Application.Invoke(delegate{
				lblCurrentPosition.Text = entity.Position.ToString();
	
				// Check if the user is currently changing the position
				if(!isSongPositionChanging)
				{
					hscaleSongPosition.Value = (double)(entity.PositionPercentage * 100);
				}
			});
		}
		
		public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
		{
			Gtk.Application.Invoke(delegate{
				if(audioFile != null)
				{
			        // Refresh labels
					Console.WriteLine("MainWindow - RefreshSongInformation");
			        lblArtistName.Text = audioFile.ArtistName;
			        lblAlbumTitle.Text = audioFile.AlbumTitle;
			        lblSongTitle.Text = audioFile.Title;
			        lblSongFilePath.Text = audioFile.FilePath;	        
					//lblCurrentPosition.Text = audioFile.Position;
					lblCurrentLength.Text = audioFile.Length;
								
					lblCurrentFileType.Text = audioFile.FileType.ToString();
					lblCurrentBitrate.Text = audioFile.Bitrate.ToString();
					lblCurrentSampleRate.Text = audioFile.SampleRate.ToString();
					lblCurrentBitsPerSample.Text = audioFile.BitsPerSample.ToString();
								
					//Pixbuf stuff = new Pixbuf("icon48.png");
					//stuff = stuff.ScaleSimple(150, 150, InterpType.Bilinear);
					//this.imageAlbumCover.Pixbuf = stuff;
					
					System.Drawing.Image drawingImage = AudioFile.ExtractImageForAudioFile(audioFile.FilePath);			
					
					if(drawingImage != null)
					{
						// Resize image
						drawingImage = ImageManipulation.ResizeImage(drawingImage, 150, 150);
						
						// Set album cover
						imageAlbumCover.Pixbuf = ImageToPixbuf(drawingImage);
					}
					else
					{
						// Get Unix-style directory information (i.e. case sensitive file names)
						if(!String.IsNullOrEmpty(audioFile.FilePath))
						{
							try
							{
								bool imageFound = false;
								string folderPath = System.IO.Path.GetDirectoryName(audioFile.FilePath);
								UnixDirectoryInfo rootDirectoryInfo = new UnixDirectoryInfo(folderPath);
								
								// For each directory, search for new directories
								UnixFileSystemInfo[] infos = rootDirectoryInfo.GetFileSystemEntries();
				            	foreach (UnixFileSystemInfo fileInfo in rootDirectoryInfo.GetFileSystemEntries())
				            	{
									// Check if the file matches
									string fileName = fileInfo.Name.ToUpper();
									if((fileName.EndsWith(".JPG") ||
									    fileName.EndsWith(".JPEG") ||
									    fileName.EndsWith(".PNG") ||
									    fileName.EndsWith(".GIF")) &&
									   (fileName.StartsWith("FOLDER") ||
									 	fileName.StartsWith("COVER")))
									{
										// Get image from file
										imageFound = true;
										Pixbuf imageCover = new Pixbuf(fileInfo.FullName);
										imageCover = imageCover.ScaleSimple(150, 150, InterpType.Bilinear);
										this.imageAlbumCover.Pixbuf = imageCover;
									}
								}
								
								// Set empty image if not cover not found
								if(!imageFound)
								{
									this.imageAlbumCover.Pixbuf = null;
								}
							}
							catch
							{
								this.imageAlbumCover.Pixbuf = null;
							}
						}
						else
						{
							// Set empty album cover
							imageAlbumCover.Pixbuf = null;
						}
					}
					
					// Check if image cover is still empty
					if(imageAlbumCover.Pixbuf == null)
					{				
						Pixbuf imageCover = new Pixbuf("black.png");
						imageCover = imageCover.ScaleSimple(150, 150, InterpType.Bilinear);
						this.imageAlbumCover.Pixbuf = imageCover;
					}
				}
			});
		}		
		
        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
		{			
			Gtk.Application.Invoke(delegate{			
				lblCurrentVolume.Text = entity.VolumeString;
				if(entity.Volume != vscaleVolume.Value)
					vscaleVolume.Value = (float)entity.Volume;			
			});
		}
		
        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
		{
			Gtk.Application.Invoke(delegate{			
//				lblTimeShiftingValue.Text = entity.TimeShiftingString;
//				if(entity.TimeShifting != hscaleTimeShifting.Value)
//					hscaleTimeShifting.Value = (float)entity.TimeShifting;
			});
		}
			
		public void PlayerError(Exception ex)
		{
			Gtk.Application.Invoke(delegate{			
				// Build text
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("An error occured in the Player component:");
				sb.AppendLine(ex.Message);
				sb.AppendLine();
				sb.AppendLine(ex.StackTrace);																
				
				// Display alert
				MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, sb.ToString());
				md.Run();
				md.Destroy();
			});
		}
		
		#endregion
		
		#region ILibraryBrowserView implementation
		
		public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities)
		{
			Gtk.Application.Invoke(delegate{
				// Clear list
				Console.WriteLine("MainWindow - RefreshLibraryBrowser");
				storeLibraryBrowser.Clear();			
				
				// Get first level nodes and add to tree store			
				foreach(LibraryBrowserEntity entity in entities)
				{
					// Add tree iter
					Gtk.TreeIter iter = storeLibraryBrowser.AppendValues(entity);
					
					// The first subitems are always dummy or static.
					foreach(LibraryBrowserEntity entitySub in entity.SubItems)				
						storeLibraryBrowser.AppendValues(iter, entitySub);
				}
							
				// Set model
				treeLibraryBrowser.Model = storeLibraryBrowser;
			});			
		}
		
		public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
		{
			Gtk.Application.Invoke(delegate{
				// Get data
				Console.WriteLine("MainWindow - RefreshLibraryBrowserNode");
				Gtk.TreeIter iter = (Gtk.TreeIter)userData;
				
				// Determine type
				if(entity.Type == LibraryBrowserEntityType.Artists)
				{							
					foreach(LibraryBrowserEntity artist in entities)
					{
						// Add artist node
						Gtk.TreeIter iterArtist = storeLibraryBrowser.AppendValues(iter, artist);
						
						// The first subitems are always dummy or static.
						foreach(LibraryBrowserEntity entitySub in artist.SubItems)													
							storeLibraryBrowser.AppendValues(iterArtist, entitySub);					
					}
				}								
				else if(entity.Type == LibraryBrowserEntityType.Albums)
				{
						// Fetch album titles
						foreach(LibraryBrowserEntity album in entities)
							storeLibraryBrowser.AppendValues(iter, album);
				}
				else if(entity.Type == LibraryBrowserEntityType.Artist)
				{
						// Fetch artist names					
						foreach(LibraryBrowserEntity artist in entities)
							storeLibraryBrowser.AppendValues(iter, artist);
				}
			});
		}
		
		#endregion
		
		#region ISongBrowserView implementation

		/// <summary>
		/// Refreshes the song browser.
		/// </summary>
		/// <param name='audioFiles'>List of audio files to display in the Song Browser.</param>
		public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
		{	
			Gtk.Application.Invoke(delegate{
				try 
				{
					Console.WriteLine("MainWindow - RefreshSongBrowser");
					
					// Add audio files
					storeSongBrowser.Clear();
					foreach(AudioFile audioFile in audioFiles)			
							storeSongBrowser.AppendValues(audioFile);			
	
					// Set model
					treeSongBrowser.Model = storeSongBrowser;
				} 
				catch(Exception ex) {
					Console.WriteLine("RefreshSongBrowser - Exception: " + ex.Message + "\n" + ex.StackTrace);
					throw ex;
					//MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "An error occured while refreshing the Song Browser: " + ex.Message + "\n" + ex.StackTrace);
					//md.Run();
					//md.Destroy();
				}
			});
		}

		#endregion

	}
}
