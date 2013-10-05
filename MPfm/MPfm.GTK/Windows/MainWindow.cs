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
using MPfm.Player.Objects;
using MPfm.MVP.Presenters;

namespace MPfm.GTK.Windows
{
	/// <summary>
	/// Main window.
	/// </summary>
	public partial class MainWindow: BaseWindow, IMainView
	{
		private bool _isSongPositionChanging = false;
		private Gtk.TreeStore _storeLibraryBrowser = null;
		private Gtk.ListStore _storeSongBrowser = null;
        private Gtk.ListStore _storeMarkers = null;
		private Gtk.ListStore _storeAudioFileFormat = null;
		
		#region Application Init/Destroy
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow(Action<IBaseView> onViewReady): base (Gtk.WindowType.Toplevel, onViewReady)
		{			
			Build();			
			Icon = new Pixbuf(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/icon48.png");
	        Title = "MPfm: Music Player for Musicians - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA";
	
			// Set application icon (for some reason it is not visible on Unity)
			//SetIconFromFile(ResourceHelper.GetEmbeddedImageResource("icon48.png"));
            Icon = ResourceHelper.GetEmbeddedImageResource("icon48.png");
			
			SetFontProperties();
			InitializeSongBrowser();
			InitializeLibraryBrowser();
            InitializeMarkers();

            // Force refresh song browser to create columns
			RefreshSongBrowser(new List<AudioFile>());		
            RefreshSongInformation(null, 0, 0, 0);          	
			
			// Fill sound format combo box
			_storeAudioFileFormat = new ListStore(typeof(string));			
			_storeAudioFileFormat.AppendValues("All");
			_storeAudioFileFormat.AppendValues("FLAC");
			_storeAudioFileFormat.AppendValues("MP3");					
			_storeAudioFileFormat.AppendValues("MPC");
			_storeAudioFileFormat.AppendValues("OGG");
			_storeAudioFileFormat.AppendValues("WAV");
			_storeAudioFileFormat.AppendValues("WV");
			cboSoundFormat.Model = _storeAudioFileFormat;								
			
			// Select first item
			Gtk.TreeIter iter;
			cboSoundFormat.Model.IterNthChild(out iter, 0);
			cboSoundFormat.SetActiveIter(iter);
			
			// Set focus to something else than the toolbar (for some reason, the first button is selected)
			cboSoundFormat.GrabFocus();
						
			hscaleSongPosition.AddEvents((int)EventMask.ButtonPressMask | (int)EventMask.ButtonReleaseMask);
			
			onViewReady(this);
		}
	
		/// <summary>
		/// Exits the application.
		/// </summary>
		protected void ExitApplication()
		{
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
			_storeSongBrowser = new Gtk.ListStore(typeof(AudioFile));

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
					return;
		
				// Get value and set cell text
				PropertyInfo propertyInfo = typeof(AudioFile).GetProperty(property);
				object propertyValue = propertyInfo.GetValue(audioFile, null);
				(cell as Gtk.CellRendererText).Text = propertyValue.ToString();
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
			_storeLibraryBrowser = new Gtk.TreeStore(typeof(LibraryBrowserEntity));						
			treeLibraryBrowser.HeadersVisible = false;

			// Create title column
			Gtk.TreeViewColumn colTitle = new Gtk.TreeViewColumn();
			Gtk.CellRendererText cellTitle = new Gtk.CellRendererText();	
			colTitle.Data.Add("Property", "Title");
			colTitle.PackStart(cellTitle, true);
			colTitle.SetCellDataFunc(cellTitle, new Gtk.TreeCellDataFunc(RenderLibraryBrowserCell));
			treeLibraryBrowser.AppendColumn(colTitle);
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
	
			// Get value and set cell text
			PropertyInfo propertyInfo = typeof(LibraryBrowserEntity).GetProperty(property);
			object propertyValue = propertyInfo.GetValue(entity, null);
			(cell as Gtk.CellRendererText).Text = propertyValue.ToString();
		}

		#endregion

        #region Markers Methods
                
        protected void InitializeMarkers()
        {
            _storeMarkers = new Gtk.ListStore(typeof(Marker));

            Gtk.TreeViewColumn colTitle = new Gtk.TreeViewColumn();
            Gtk.CellRendererText cellTitle = new Gtk.CellRendererText();
            colTitle.Title = "Name";
            colTitle.Resizable = true;
            colTitle.Data.Add("Property", "Name");
            colTitle.PackStart(cellTitle, true);
            colTitle.SetCellDataFunc(cellTitle, new Gtk.TreeCellDataFunc(RenderMarkerCell));
            treeMarkers.AppendColumn(colTitle);

            Gtk.TreeViewColumn colPosition = new Gtk.TreeViewColumn();
            Gtk.CellRendererText cellPosition = new Gtk.CellRendererText();    
            colPosition.Title = "Position";
            colPosition.Resizable = true;
            colPosition.Data.Add("Property", "Position");
            colPosition.PackStart(cellPosition, true);
            colPosition.SetCellDataFunc(cellPosition, new Gtk.TreeCellDataFunc(RenderMarkerCell));
            treeMarkers.AppendColumn(colPosition);
        }
            
        /// <summary>
        /// Renders a cell from the Song Browser.
        /// </summary>
        /// <param name='column'>Column</param>
        /// <param name='cell'>Cell</param>
        /// <param name='model'>Model</param>
        /// <param name='iter'>Iter</param>
        private void RenderMarkerCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            // Get model data
            //Console.WriteLine("MainWindow - RenderMarkerCell");
            Marker marker = (Marker)model.GetValue(iter, 0);
    
            // Get property name
            string property = (string)column.Data["Property"];
            if(String.IsNullOrEmpty(property))          
                return;         
    
            // Get value and set cell text
            PropertyInfo propertyInfo = typeof(Marker).GetProperty(property);
            object propertyValue = propertyInfo.GetValue(marker, null);
            (cell as Gtk.CellRendererText).Text = propertyValue.ToString();

        }

        protected void OnTreeMarkersRowActivated(object o, RowActivatedArgs args)
        {
            TreeModel model;
            TreeIter iter;  
            if((o as TreeView).Selection.GetSelected(out model, out iter))
            {
                Marker marker = (Marker)_storeMarkers.GetValue(iter, 0);               
                OnSelectMarker(marker);
            }
        }

        #endregion
	
		#region Other Methods
		
		private void SetFontProperties()
		{				
			// Get default font name
			string defaultFontName = lblArtistName.Style.FontDescription.Family;			
			string titleFontName = "Sans Bold 10";
			string subtitleFontName = "Sans Bold 8";
			string buttonFontName = "Sans Bold 8";
			string textFontName = "Sans 8";
			string largePositionFontName = "Mono Bold 11";
			
			//btnPitchShiftingMore.ModifyFont(FontDescription.FromString(buttonFontName));
			btnTimeShiftingMore.ModifyFont(FontDescription.FromString(buttonFontName));
			
			lblArtistName.ModifyFont(FontDescription.FromString(defaultFontName +" 16"));
			lblAlbumTitle.ModifyFont(FontDescription.FromString(defaultFontName +" 12"));
			lblSongTitle.ModifyFont(FontDescription.FromString(defaultFontName +" 11"));
			lblSongFilePath.ModifyFont(FontDescription.FromString(defaultFontName +" 8"));
	
			toolbarMain.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));	
			
			lblLibraryBrowser.ModifyFont(FontDescription.FromString(titleFontName));								
			lblSongBrowser.ModifyFont(FontDescription.FromString(titleFontName));
			lblCurrentSong.ModifyFont(FontDescription.FromString(titleFontName));			
			lblLoops.ModifyFont(FontDescription.FromString(titleFontName));
			lblMarkers.ModifyFont(FontDescription.FromString(titleFontName));
			
			//lblSearchFor.ModifyFont(FontDescription.FromString(defaultFontName +" 9"));
			lblLibraryFilter.ModifyFont(FontDescription.FromString(textFontName));			
				
			lblCurrentPosition.ModifyFont(FontDescription.FromString(largePositionFontName));
			lblCurrentLength.ModifyFont(FontDescription.FromString(largePositionFontName));
			
			lblSongPosition.ModifyFont(FontDescription.FromString(subtitleFontName));
			
			lblTimeShifting.ModifyFont(FontDescription.FromString(subtitleFontName));
			lblTimeShiftingReset.ModifyFont(FontDescription.FromString(textFontName));
			lblTimeShiftingValue.ModifyFont(FontDescription.FromString(textFontName));
			//lblOriginalTempo.ModifyFont(FontDescription.FromString(textFontName));
			//lblOriginalTempoBPM.ModifyFont(FontDescription.FromString(textFontName));
			btnDetectTempo.ModifyFont(FontDescription.FromString(textFontName));			
			
			lblPitchShifting.ModifyFont(FontDescription.FromString(subtitleFontName));
			//lblPitchShiftingReset.ModifyFont(FontDescription.FromString(textFontName));
			lblPitchShiftingValue.ModifyFont(FontDescription.FromString(textFontName));
			
			lblInformation.ModifyFont(FontDescription.FromString(subtitleFontName));
			lblVolume.ModifyFont(FontDescription.FromString(subtitleFontName));
			
			lblCurrentFileType.ModifyFont(FontDescription.FromString(textFontName));
			lblCurrentBitrate.ModifyFont(FontDescription.FromString(textFontName));
			lblCurrentSampleRate.ModifyFont(FontDescription.FromString(textFontName));
			lblCurrentBitsPerSample.ModifyFont(FontDescription.FromString(textFontName));
						
			lblTimeShiftingValue.ModifyFont(FontDescription.FromString(textFontName));
			lblCurrentVolume.ModifyFont(FontDescription.FromString(textFontName));
		}
		
		private AudioFileFormat GetCurrentAudioFileFormatFilter()
		{
			// Get current audio file format
			Gtk.TreeIter iter;
			AudioFileFormat format;
			cboSoundFormat.GetActiveIter(out iter);
			string filter = _storeAudioFileFormat.GetValue(iter, 0).ToString();
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
				
			dialog.Destroy();
		}

		protected void OnActionAddFilesActivated(object sender, System.EventArgs e)
		{
			Gtk.FileChooserDialog dialog = 
				new Gtk.FileChooserDialog("Select the audio files to add to the library.", 
			                                  this, FileChooserAction.Open, 
			                                  "Cancel", ResponseType.Cancel, 
			                                  "Add files to library", ResponseType.Accept);
			
			dialog.SelectMultiple = true;
			if(dialog.Run() == (int)ResponseType.Accept)
                OnAddFilesToLibrary(dialog.Filenames.ToList());
							
			dialog.Destroy();
		}		
									
		protected void OnActionAddFolderActivated(object sender, System.EventArgs e)
		{
			Gtk.FileChooserDialog dialog = 
				new Gtk.FileChooserDialog("Select a folder to add to the library.", 
			                                  this, FileChooserAction.SelectFolder, 
			                                  "Cancel", ResponseType.Cancel, 
			                                  "Add folder to library", ResponseType.Accept);
						
			if(dialog.Run() == (int)ResponseType.Accept)
                OnAddFolderToLibrary(dialog.Filename);
							
			dialog.Destroy();
		}
		
		protected void OnActionUpdateLibraryActivated(object sender, System.EventArgs e)
		{
            OnUpdateLibrary();
		}		
					
		protected void OnActionPlayActivated(object sender, System.EventArgs e)
		{
            OnPlayerPause();
		}
		
		protected void OnActionPauseActivated(object sender, System.EventArgs e)
		{
            OnPlayerPause();		
        }
		
		protected void OnActionStopActivated(object sender, System.EventArgs e)
		{
            OnPlayerStop();
		}
		
		protected void OnActionPreviousActivated(object sender, System.EventArgs e)
		{
            OnPlayerPrevious();
		}

		protected void OnActionNextActivated(object sender, System.EventArgs e)
		{
            OnPlayerNext();
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
			string text = Title + "\nMPfm: Music Player for Musicians is © 2011-2012 Yanick Castonguay and is released under the GPLv3 license.";
			text += "\nThe BASS audio library is © 1999-2012 Un4seen Developments.";
		    text += "\nThe BASS.NET audio library is © 2005-2012 radio42.";
	

			MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, text);
			md.Run();
			md.Destroy();
		}
	
		protected void OnHelpActionContentsActivated(object sender, System.EventArgs e)
		{
	
		}

		protected void OnVolumeValueChanged(object sender, System.EventArgs e)
		{
			OnPlayerSetVolume((float)vscaleVolume.Value);
		}
		
		[GLib.ConnectBefore]
		protected void OnHscaleSongPositionButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			_isSongPositionChanging = true;
		}
		
		[GLib.ConnectBefore]
		protected void OnHscaleSongPositionButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
		{
			float value = (float)hscaleSongPosition.Value / 100;
		    OnPlayerSetPosition(value);
			_isSongPositionChanging = false;
		}
		
		protected void OnTimeShiftingValueChanged(object sender, System.EventArgs e)
		{
	        OnPlayerSetTimeShifting((float)hscaleTimeShifting.Value);
		}

		protected void OnSoundFormatChanged(object sender, System.EventArgs e)
		{			
		    //OnAudioFileFormatFilterChanged(GetCurrentAudioFileFormatFilter());
		}
		
		/// <summary>
		/// Raises the Library Browser row activated event (when the user double-clicks on an item).
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnTreeLibraryBrowserRowActivated(object sender, Gtk.RowActivatedArgs args)
		{
			TreeModel model;
			TreeIter iter;	
			if((sender as TreeView).Selection.GetSelected(out model, out iter))
			{
				LibraryBrowserEntity entity = (LibraryBrowserEntity)_storeLibraryBrowser.GetValue(iter, 0);								
			    OnTreeNodeDoubleClicked(entity);	
			}
		}
		
		/// <summary>
		/// Raises the Library Browser cursor changed event (when the user selects an item).
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnTreeLibraryBrowserCursorChanged(object sender, System.EventArgs e)
		{
			TreeModel model;
			TreeIter iter;	
			if((sender as TreeView).Selection.GetSelected(out model, out iter))
			{
				LibraryBrowserEntity entity = (LibraryBrowserEntity)_storeLibraryBrowser.GetValue(iter, 0);								
                OnTreeNodeSelected(entity);
			}
		}
		
		/// <summary>
		/// Raises the Library Browser row expanded event (when the user expands an item).
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnTreeLibraryBrowserRowExpanded(object sender, Gtk.RowExpandedArgs args)
		{
			Gtk.TreeIter iter;
			LibraryBrowserEntity entity = (LibraryBrowserEntity)_storeLibraryBrowser.GetValue(args.Iter, 0);						

			// Check for dummy node		
			_storeLibraryBrowser.IterChildren(out iter, args.Iter);
			LibraryBrowserEntity entityChildren = (LibraryBrowserEntity)_storeLibraryBrowser.GetValue(iter, 0);			
			if(entityChildren.Type == LibraryBrowserEntityType.Dummy)
			{	
				OnTreeNodeExpanded(entity, args.Iter);
			}	
		}

		/// <summary>
		/// Raises the Song Bowser row activated event (when the user double-clicks on the row).
		/// </summary>
		/// <param name='sender'>Event sender</param>
		/// <param name='e'>Event arguments</param>
		protected void OnTreeSongBrowserRowActivated(object sender, Gtk.RowActivatedArgs args)
		{
			TreeModel model;
			TreeIter iter;	
			if((sender as TreeView).Selection.GetSelected(out model, out iter))
			{
				AudioFile audioFile = (AudioFile)_storeSongBrowser.GetValue(iter, 0);				
				OnTableRowDoubleClicked(audioFile);
			}
		}

        protected void OnActionPlayLoopActivated(object sender, EventArgs e)
        {
        }

        protected void OnActionGoToMarkerActivated(object sender, EventArgs e)
        {
            TreeModel model;
            TreeIter iter;  
            if(treeMarkers.Selection.GetSelected(out model, out iter))
            {
                Marker marker = (Marker)_storeMarkers.GetValue(iter, 0);
                OnSelectMarker(marker);
            }
        }

        protected void OnActionAddMarkerActivated(object sender, EventArgs e)
        {
            OnAddMarkerWithTemplate(MarkerTemplateNameType.Verse);
        }

        protected void OnActionEditMarkerActivated(object sender, EventArgs e)
        {
        }

        protected void OnActionDeleteMarkerActivated(object sender, EventArgs e)
        {
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

        protected void btnTest_Click(object sender, EventArgs e)
        {
            //vboxPitchShifting.Visible = false;
        }

        #region IMainView implementation

        public System.Action OnOpenPreferencesWindow { get; set; }
        public System.Action OnOpenEffectsWindow { get; set; }
        public System.Action OnOpenPlaylistWindow { get; set; }
        public System.Action OnOpenSyncWindow { get; set; }

        public System.Action<List<string>> OnAddFilesToLibrary { get; set; }
        public System.Action<string> OnAddFolderToLibrary { get; set; }
        public System.Action OnUpdateLibrary { get; set; }

        #endregion
			
		#region IPlayerView implementation
			
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
        public System.Action OnEditSongMetadata { get; set; }
        public System.Action OnPlayerShuffle { get; set; }
        public System.Action OnPlayerRepeat { get; set; }
        public System.Action OnOpenPlaylist { get; set; }

		public void RefreshPlayerStatus(PlayerStatusType status)
        {
            Console.WriteLine("MainWindow - RefreshPlayerStatus - status: {0}", status.ToString());
            Gtk.Application.Invoke(delegate{
                switch (status)
                {
                    case PlayerStatusType.Initialized:
                    case PlayerStatusType.Stopped:
                    case PlayerStatusType.Paused:
                        actionPlay.StockId = "gtk-media-play";
                        //actionPlay.IconName = "gtk-media-play";
                        break;
                    case PlayerStatusType.Playing:
                        actionPlay.StockId = "gtk-media-pause";
                        //actionPlay.IconName = "gtk-media-pause";
                        break;
                }
            });
		}

		public void RefreshPlayerPosition(PlayerPositionEntity entity)
		{
			Gtk.Application.Invoke(delegate{
				lblCurrentPosition.Text = entity.Position.ToString();
	
				// Check if the user is currently changing the position
				if(!_isSongPositionChanging)
					hscaleSongPosition.Value = (double)(entity.PositionPercentage * 100);
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
					//imageAlbumCover.Pixbuf = stuff;
					
					System.Drawing.Image drawingImage = AudioFile.ExtractImageForAudioFile(audioFile.FilePath);			
					
					if(drawingImage != null)
					{
						// Resize image and set cover
						drawingImage = SystemDrawingHelper.ResizeImage(drawingImage, 150, 150);
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
										imageAlbumCover.Pixbuf = imageCover;
									}
								}
								
								// Set empty image if not cover not found
								if(!imageFound)
									imageAlbumCover.Pixbuf = null;
							}
							catch
							{
								imageAlbumCover.Pixbuf = null;
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
                        Pixbuf imageCover = ResourceHelper.GetEmbeddedImageResource("black.png");
						imageCover = imageCover.ScaleSimple(150, 150, InterpType.Bilinear);
						imageAlbumCover.Pixbuf = imageCover;
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

        public void RefreshMarkers(IEnumerable<Marker> markers)
        {
        }

        public void RefreshLoops(IEnumerable<Loop> loops)
        {
        }
			
		public void PlayerError(Exception ex)
		{
			Gtk.Application.Invoke(delegate{			
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, string.Format("An error occured in the Player component: {0}", ex));
				md.Run();
				md.Destroy();
			});
		}
		
		#endregion
		
		#region ILibraryBrowserView implementation
		
        public System.Action<AudioFileFormat> OnAudioFileFormatFilterChanged { get; set; }
        public System.Action<LibraryBrowserEntity> OnTreeNodeSelected { get; set; }
        public System.Action<LibraryBrowserEntity, object> OnTreeNodeExpanded { get; set; }     
        public System.Action<LibraryBrowserEntity> OnTreeNodeDoubleClicked { get; set; }
        public Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable { get; set; }

		public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities)
		{
			Gtk.Application.Invoke(delegate{
				Console.WriteLine("MainWindow - RefreshLibraryBrowser");
				_storeLibraryBrowser.Clear();			
				
				// Get first level nodes and add to tree store			
				foreach(LibraryBrowserEntity entity in entities)
				{
					// Add tree iter
					Gtk.TreeIter iter = _storeLibraryBrowser.AppendValues(entity);
					
					// The first subitems are always dummy or static.
					foreach(LibraryBrowserEntity entitySub in entity.SubItems)				
						_storeLibraryBrowser.AppendValues(iter, entitySub);
				}
							
				treeLibraryBrowser.Model = _storeLibraryBrowser;
			});			
		}
		
		public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
		{
			Gtk.Application.Invoke(delegate{
				Console.WriteLine("MainWindow - RefreshLibraryBrowserNode");
				Gtk.TreeIter iter = (Gtk.TreeIter)userData;
                TreeIter treeIterDummy;
                _storeLibraryBrowser.IterNthChild(out treeIterDummy, iter, 0);
				
				switch(entity.Type)
				{				
                    case LibraryBrowserEntityType.Artists:
    					foreach(LibraryBrowserEntity artist in entities)
    					{
    						// Add artist node
    						Gtk.TreeIter iterArtist = _storeLibraryBrowser.AppendValues(iter, artist);
    						
    						// The first subitems are always dummy or static.
    						foreach(LibraryBrowserEntity entitySub in artist.SubItems)													
    							_storeLibraryBrowser.AppendValues(iterArtist, entitySub);					
    					}
                        break;
                    case LibraryBrowserEntityType.Albums:
						foreach(LibraryBrowserEntity album in entities)
							_storeLibraryBrowser.AppendValues(iter, album);
                        break;
                    case LibraryBrowserEntityType.Artist:
						foreach(LibraryBrowserEntity artist in entities)
							_storeLibraryBrowser.AppendValues(iter, artist);
                        break;
                }

                // Remove dummy node
                _storeLibraryBrowser.Remove(ref treeIterDummy);
			});
		}
		
		#endregion
		
		#region ISongBrowserView implementation

        public System.Action<AudioFile> OnTableRowDoubleClicked { get; set; }
        public Action<AudioFile> OnSongBrowserEditSongMetadata { get; set; }
        public Action<string> OnSearchTerms { get; set; }

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
					_storeSongBrowser.Clear();
					foreach(AudioFile audioFile in audioFiles)			
							_storeSongBrowser.AppendValues(audioFile);			
	
					// Set model
					treeSongBrowser.Model = _storeSongBrowser;
				} 
				catch(Exception ex) {
					Console.WriteLine("RefreshSongBrowser - Exception: " + ex.Message + "\n" + ex.StackTrace);
					throw ex;
				}
			});
		}

		#endregion

        #region IMarkersView implementation

        public System.Action OnAddMarker { get; set; }
        public Action<MarkerTemplateNameType> OnAddMarkerWithTemplate { get; set; }
        public Action<Marker> OnEditMarker { get; set; }
        public Action<Marker> OnSelectMarker { get; set; }
        public Action<Marker> OnDeleteMarker { get; set; }

        public void MarkerError(Exception ex)
        {
            Gtk.Application.Invoke(delegate{            
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, string.Format("An error occured in the Markers component: {0}", ex));
                md.Run();
                md.Destroy();
            });
        }

        public void RefreshMarkers(List<Marker> markers)
        {
            Gtk.Application.Invoke(delegate{
                try 
                {
                    Console.WriteLine("MainWindow - RefreshMarkers - markers.Count: {0}", markers.Count);
                    _storeMarkers.Clear();
                    foreach(var marker in markers)
                        _storeMarkers.AppendValues(marker);
                    treeMarkers.Model = _storeMarkers;
                } 
                catch(Exception ex) {
                    Console.WriteLine("RefreshMarkers - Exception: " + ex.Message + "\n" + ex.StackTrace);
                    throw ex;
                }
            });
        }

        #endregion

        #region ILoopsView implementation

        public System.Action OnAddLoop { get; set; }
        public Action<Loop> OnEditLoop { get; set; }

        public void LoopError(Exception ex)
        {
            Gtk.Application.Invoke(delegate{            
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, string.Format("An error occured in the Loops component: {0}", ex));
                md.Run();
                md.Destroy();
            });
        }

        public void RefreshLoops(List<Loop> loops)
        {
        }

        #endregion

        #region ITimeShiftingView implementation

        public Action<float> OnSetTimeShifting { get; set; }
        public System.Action OnResetTimeShifting { get; set; }
        public System.Action OnUseDetectedTempo { get; set; }
        public System.Action OnIncrementTempo { get; set; }
        public System.Action OnDecrementTempo { get; set; }

        public void TimeShiftingError(Exception ex)
        {
            Gtk.Application.Invoke(delegate{            
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, string.Format("An error occured in the TimeShifting component: {0}", ex));
                md.Run();
                md.Destroy();
            });
        }

        public void RefreshTimeShifting(PlayerTimeShiftingEntity entity)
        {
        }

        #endregion

        #region IPitchShiftingView implementation

        public Action<int> OnChangeKey { get; set; }
        public Action<int> OnSetInterval { get; set; }
        public System.Action OnResetInterval { get; set; }
        public System.Action OnIncrementInterval { get; set; }
        public System.Action OnDecrementInterval { get; set; }

        public void PitchShiftingError(Exception ex)
        {
            Gtk.Application.Invoke(delegate{            
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, string.Format("An error occured in the PitchShifting component: {0}", ex));
                md.Run();
                md.Destroy();
            });
        }

        public void RefreshKeys(List<Tuple<int, string>> keys)
        {
        }

        public void RefreshPitchShifting(PlayerPitchShiftingEntity entity)
        {
        }

        #endregion        

	}
}
