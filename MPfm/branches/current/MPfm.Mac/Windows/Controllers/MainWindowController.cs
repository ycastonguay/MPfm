//
// MainWindowController.cs: Main window controller.
//
// Copyright © 2011-2012 Yanick Castonguay
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

using MonoMac.CoreGraphics;

//#define MACOSX
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.Library;
using MPfm.MVP;
using MPfm.Sound;
using Ninject;
using System.Drawing;
using System.Text;
using MPfm.Core;
using System.Reflection;
using MonoMac.CoreText;

namespace MPfm.Mac
{
    /// <summary>
    /// Main window controller.
    /// </summary>
    //[Register("NSWindow")]
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController, IPlayerView, ISongBrowserView, ILibraryBrowserView
	{
		readonly IPlayerPresenter playerPresenter = null;
		readonly ISongBrowserPresenter songBrowserPresenter = null;
		readonly ILibraryBrowserPresenter libraryBrowserPresenter = null;

		UpdateLibraryWindowController updateLibraryWindowController = null;
        PlaylistWindowController playlistWindowController = null;
        EffectsWindowController effectsWindowController = null;
        PreferencesWindowController preferencesWindowController = null;

        LibraryBrowserOutlineViewDelegate libraryBrowserOutlineViewDelegate = null;
		LibraryBrowserDataSource libraryBrowserDataSource = null;
        SongBrowserTableViewDelegate songBrowserOutlineViewDelegate = null;
        SongBrowserSource songBrowserSource = null;
        AlbumCoverSource albumCoverSource = null;
        AlbumCoverCacheService albumCoverCacheService = null;

		//strongly typed window accessor00
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
		
		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindowController(IntPtr handle) : base (handle)
		{
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController(NSCoder coder) : base (coder)
		{
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController(IPlayerPresenter playerPresenter,
		                            ISongBrowserPresenter songBrowserPresenter,
		                            ILibraryBrowserPresenter libraryBrowserPresenter,
                                    AlbumCoverCacheService albumCoverCacheService) : base ("MainWindow")
        {
            // Set properties
            this.playerPresenter = playerPresenter;
            this.songBrowserPresenter = songBrowserPresenter;
            this.libraryBrowserPresenter = libraryBrowserPresenter;
            this.albumCoverCacheService = new AlbumCoverCacheService();
		}		

		public override void WindowDidLoad()
		{
            base.WindowDidLoad();
		}

		public override void AwakeFromNib()
        {
            // Set main window title
            this.Window.Title = "MPfm: Music Player for Musicians - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA";
            Tracing.Log("MainWindowController.AwakeFromNib -- Initializing user interface...");

            // Create split delegate
            splitMain.Delegate = new MainSplitViewDelegate();

            // Add items to Sound Format combo box
            cboSoundFormat.RemoveAllItems();
            cboSoundFormat.AddItem("All");
            cboSoundFormat.AddItem("FLAC");
            cboSoundFormat.AddItem("OGG");
            cboSoundFormat.AddItem("MP3");
            cboSoundFormat.AddItem("MPC");
            cboSoundFormat.AddItem("WAV");
            cboSoundFormat.AddItem("WV");

            // Initialize and configure Library Browser
            libraryBrowserOutlineViewDelegate = new LibraryBrowserOutlineViewDelegate(this.libraryBrowserPresenter);
            outlineLibraryBrowser.Delegate = libraryBrowserOutlineViewDelegate;
            outlineLibraryBrowser.AllowsMultipleSelection = false;
            outlineLibraryBrowser.DoubleClick += HandleLibraryBrowserDoubleClick;

            // Initialize and configure Song Browser
            songBrowserOutlineViewDelegate = new SongBrowserTableViewDelegate(this.songBrowserPresenter);
            tableSongBrowser.Delegate = songBrowserOutlineViewDelegate;
            tableSongBrowser.AllowsMultipleSelection = true;
            tableSongBrowser.DoubleClick += HandleSongBrowserDoubleClick;

            // Load images and set theme
            LoadImages();
            SetTheme();

            // Create controllers
            playlistWindowController = new PlaylistWindowController();
            effectsWindowController = new EffectsWindowController();
            preferencesWindowController = new PreferencesWindowController();

			// Bind views
            this.playerPresenter.BindView(this);
			this.songBrowserPresenter.BindView(this);
			this.libraryBrowserPresenter.BindView(this);

            tableAlbumCovers.FocusRingType = NSFocusRingType.None;
            tableSongBrowser.FocusRingType = NSFocusRingType.None;
            scrollViewSongBrowser.BorderType = NSBorderType.NoBorder;
            scrollViewAlbumCovers.BorderType = NSBorderType.NoBorder;
            scrollViewAlbumCovers.HasHorizontalScroller = false;
            scrollViewAlbumCovers.HasVerticalScroller = false;

            scrollViewAlbumCovers.SetSynchronizedScrollView(scrollViewSongBrowser);
            scrollViewSongBrowser.SetSynchronizedScrollView(scrollViewAlbumCovers);
		}

        private void SetTheme()
        {
            // Set colors
//            viewLeftHeader.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewLeftHeader.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            viewRightHeader.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewRightHeader.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            viewLibraryBrowser.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewLibraryBrowser.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            viewNowPlaying.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewNowPlaying.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
            viewInformation.IsHeaderVisible = true;
            viewSongPosition.IsHeaderVisible = true;
            viewVolume.IsHeaderVisible = true;
            viewTimeShifting.IsHeaderVisible = true;

            // Set label fonts
            lblArtistName.Font = NSFont.FromFontName("TitilliumText25L-999wt", 22);
            lblAlbumTitle.Font = NSFont.FromFontName("TitilliumText25L-600wt", 20);
            lblSongTitle.Font = NSFont.FromFontName("TitilliumText25L-600wt", 16);
            lblSongPath.Font = NSFont.FromFontName("TitilliumText25L-400wt", 12);

            lblSampleRate.Font = NSFont.FromFontName("Junction", 11.4f);
            lblBitrate.Font = NSFont.FromFontName("Junction", 11.4f);
            lblFileType.Font = NSFont.FromFontName("Junction", 11.4f);
            lblBitsPerSample.Font = NSFont.FromFontName("Junction", 11.4f);
            lblFilterBySoundFormat.Font = NSFont.FromFontName("Junction", 11.4f);

            lblTitleLibraryBrowser.Font = NSFont.FromFontName("TitilliumText25L-999wt", 14);
            lblTitleCurrentSong.Font = NSFont.FromFontName("TitilliumText25L-999wt", 14);
            lblTitleLoops.Font = NSFont.FromFontName("TitilliumText25L-999wt", 14);
            lblTitleMarkers.Font = NSFont.FromFontName("TitilliumText25L-999wt", 14);
            lblTitleSongBrowser.Font = NSFont.FromFontName("TitilliumText25L-999wt", 14);

            lblSubtitleSongPosition.Font = NSFont.FromFontName("TitilliumText25L-999wt", 13);
            lblSubtitleTimeShifting.Font = NSFont.FromFontName("TitilliumText25L-999wt", 13);
            lblSubtitleVolume.Font = NSFont.FromFontName("TitilliumText25L-999wt", 13);
            lblSubtitleInformation.Font = NSFont.FromFontName("TitilliumText25L-999wt", 13);

            lblPosition.Font = NSFont.FromFontName("DroidSansMono", 15f);
            lblLength.Font = NSFont.FromFontName("DroidSansMono", 15f);
            lblTimeShifting.Font = NSFont.FromFontName("DroidSansMono", 11f);
            lblVolume.Font = NSFont.FromFontName("DroidSansMono", 11f);

            cboSoundFormat.Font = NSFont.FromFontName("Junction", 11);
            searchSongBrowser.Font = NSFont.FromFontName("Junction", 12);

            // Set cell fonts for Library Browser
            NSTableColumn columnText = outlineLibraryBrowser.FindTableColumn(new NSString("columnText"));
            columnText.DataCell.Font = NSFont.FromFontName("Junction", 11f);

            // Set cell fonts for Song Browser
            NSTableColumn columnTrackNumber = tableSongBrowser.FindTableColumn(new NSString("columnTrackNumber"));
            NSTableColumn columnTitle = tableSongBrowser.FindTableColumn(new NSString("columnTitle"));
            NSTableColumn columnLength = tableSongBrowser.FindTableColumn(new NSString("columnLength"));
            NSTableColumn columnArtistName = tableSongBrowser.FindTableColumn(new NSString("columnArtistName"));
            NSTableColumn columnAlbumTitle = tableSongBrowser.FindTableColumn(new NSString("columnAlbumTitle"));
            columnTrackNumber.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnTrackNumber.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnTitle.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnTitle.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLength.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLength.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnArtistName.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnArtistName.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnAlbumTitle.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnAlbumTitle.DataCell.Font = NSFont.FromFontName("Junction", 11f);

            // Set cell fonts for Loops
            NSTableColumn columnLoopName = tableLoops.FindTableColumn(new NSString("columnLoopName"));
            NSTableColumn columnLoopLength = tableLoops.FindTableColumn(new NSString("columnLoopLength"));
            NSTableColumn columnLoopStartPosition = tableLoops.FindTableColumn(new NSString("columnLoopStartPosition"));
            NSTableColumn columnLoopEndPosition = tableLoops.FindTableColumn(new NSString("columnLoopEndPosition"));
            columnLoopName.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopName.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopLength.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopLength.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopStartPosition.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopStartPosition.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopEndPosition.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopEndPosition.DataCell.Font = NSFont.FromFontName("Junction", 11f);

            // Set cell fonts for Markers
            NSTableColumn columnMarkerName = tableMarkers.FindTableColumn(new NSString("columnMarkerName"));
            NSTableColumn columnMarkerPosition = tableMarkers.FindTableColumn(new NSString("columnMarkerPosition"));
            NSTableColumn columnMarkerComments = tableMarkers.FindTableColumn(new NSString("columnMarkerComments"));
            columnMarkerName.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerName.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerPosition.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerPosition.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerComments.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerComments.DataCell.Font = NSFont.FromFontName("Junction", 11f);

            btnPlayLoop.Font = NSFont.FromFontName("Junction", 11f);
            btnStopLoop.Font = NSFont.FromFontName("Junction", 11f);
            btnAddLoop.Font = NSFont.FromFontName("Junction", 11f);
            btnEditLoop.Font = NSFont.FromFontName("Junction", 11f);
            btnRemoveLoop.Font = NSFont.FromFontName("Junction", 11f);

            btnGoToMarker.Font = NSFont.FromFontName("Junction", 11f);
            btnAddMarker.Font = NSFont.FromFontName("Junction", 11f);
            btnEditMarker.Font = NSFont.FromFontName("Junction", 11f);
            btnRemoveMarker.Font = NSFont.FromFontName("Junction", 11f);

        }

        /// <summary>
        /// Loads the image resources in all controls.
        /// </summary>
        private void LoadImages()
        {
            // Load images in toolbar
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarOpen").Image = ImageResources.images32x32[0];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarUpdateLibrary").Image = ImageResources.images32x32[1];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlay").Image = ImageResources.images32x32[2];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPause").Image = ImageResources.images32x32[3];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarStop").Image = ImageResources.images32x32[4];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPrevious").Image = ImageResources.images32x32[5];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarNext").Image = ImageResources.images32x32[6];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarRepeat").Image = ImageResources.images32x32[7];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarEffects").Image = ImageResources.images32x32[8];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlaylist").Image = ImageResources.images32x32[9];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPreferences").Image = ImageResources.images32x32[10];

            // Load button images
            btnAddLoop.Image = ImageResources.images16x16[0];
            btnAddMarker.Image = ImageResources.images16x16[0];
            btnAddSongToPlaylist.Image = ImageResources.images16x16[0];
            btnEditLoop.Image = ImageResources.images16x16[1];
            btnEditMarker.Image = ImageResources.images16x16[1];
            btnEditSongMetadata.Image = ImageResources.images16x16[1];
            btnRemoveLoop.Image = ImageResources.images16x16[2];
            btnRemoveMarker.Image = ImageResources.images16x16[2];
            btnPlayLoop.Image = ImageResources.images16x16[3];
            btnPlaySelectedSong.Image = ImageResources.images16x16[3];
            btnStopLoop.Image = ImageResources.images16x16[4];
            btnGoToMarker.Image = ImageResources.images16x16[5];
        }

		#endregion

		partial void actionAddFilesToLibrary(NSObject sender)
		{
			// Open panel to choose audio files
			IEnumerable<string> filePaths = null;
			using(NSOpenPanel openPanel = new NSOpenPanel())
			{
				openPanel.CanChooseDirectories = false;
				openPanel.CanChooseFiles = true;
				openPanel.ReleasedWhenClosed = true;
				openPanel.AllowsMultipleSelection = true;
                openPanel.AllowedFileTypes = new string[]{ "FLAC", "MP3", "OGG", "WAV", "MPC", "WV" };
                openPanel.Title = "Please select audio files to add to the library";
				openPanel.Prompt = "Add to library";
				openPanel.RunModal();

				filePaths = openPanel.Urls.Select(x => x.Path);
			}

			// Check if files were found
			if(filePaths != null && filePaths.Count() > 0)
			{
				StartUpdateLibrary(UpdateLibraryMode.SpecificFiles, filePaths.ToList(), null);
			}
		}

		partial void actionAddFolderLibrary(NSObject sender)
		{
			// Open panel to choose folder
			string folderPath = string.Empty;
			using(NSOpenPanel openPanel = new NSOpenPanel())
			{
				openPanel.CanChooseDirectories = true;
				openPanel.CanChooseFiles = false;
				openPanel.ReleasedWhenClosed = true;
				openPanel.AllowsMultipleSelection = false;
				openPanel.Title = "Please select a folder to add to the library";
				openPanel.Prompt = "Add to library";	
				openPanel.RunModal();

				folderPath = openPanel.Url.Path;
			}

			// Check if the folder is valid
			if(!String.IsNullOrEmpty(folderPath))
			{
				StartUpdateLibrary(UpdateLibraryMode.SpecificFolder, null, folderPath);
			}
		}

		partial void actionOpenAudioFiles(NSObject sender)
		{
			// Open panel to choose audio files to play
			IEnumerable<string> filePaths = null;
			using(NSOpenPanel openPanel = new NSOpenPanel())
			{
				openPanel.CanChooseDirectories = false;
				openPanel.CanChooseFiles = true;
				openPanel.ReleasedWhenClosed = true;
				openPanel.AllowsMultipleSelection = true;
				openPanel.AllowedFileTypes = new string[]{ "FLAC", "MP3", "OGG", "WAV", "MPC", "WV" };
				openPanel.Title = "Please select audio files to play";
				openPanel.Prompt = "Add to playlist";	
				openPanel.RunModal();

				filePaths = openPanel.Urls.Select(x => x.Path);
			}

			// Check if files were found
			if(filePaths != null && filePaths.Count() > 0)
			{
				//playerPresenter.Player.Playlist.Clear();
				//playerPresenter.Player.Playlist.AddItems(filePaths.ToList());
				playerPresenter.Play(filePaths);
			}
		}

		partial void actionUpdateLibrary(NSObject sender)
		{
			StartUpdateLibrary(UpdateLibraryMode.WholeLibrary, null, null);
		}

        partial void actionSoundFormatChanged(NSObject sender)
        {
            // Set audio file format filter in presenter
            AudioFileFormat format;
            Enum.TryParse<AudioFileFormat>(cboSoundFormat.TitleOfSelectedItem, out format);
            libraryBrowserPresenter.AudioFileFormatFilterChanged(format);
        }

		partial void actionPlay(NSObject sender)
		{
			playerPresenter.Play();
		}

		partial void actionPause(NSObject sender)
		{
			playerPresenter.Pause();
		}

		partial void actionStop(NSObject sender)
		{
			playerPresenter.Stop();
		}

		partial void actionPrevious(NSObject sender)
		{
			playerPresenter.Previous();
		}

		partial void actionNext(NSObject sender)
		{
			playerPresenter.Next();
		}

		partial void actionRepeatType(NSObject sender)
		{
			playerPresenter.RepeatType();
		}

        partial void actionOpenMainWindow(NSObject sender)
        {
            // Show window
            this.Window.MakeKeyAndOrderFront(this);
        }

		partial void actionOpenPlaylistWindow(NSObject sender)
		{
            // Show window
            playlistWindowController.Window.MakeKeyAndOrderFront(this);          
		}

		partial void actionOpenEffectsWindow(NSObject sender)
		{
            // Show window
            effectsWindowController.Window.MakeKeyAndOrderFront(this);          
		}

		partial void actionOpenPreferencesWindow(NSObject sender)
		{
            // Show window
            preferencesWindowController.Window.MakeKeyAndOrderFront(this);
		}

        partial void actionAddSongToPlaylist(NSObject sender)
        {
        }

        partial void actionEditSongMetadata(NSObject sender)
        {

        }

        partial void actionPlaySelectedSong(NSObject sender)
        {

        }

        partial void actionChangeTimeShifting(NSObject sender)
        {
            playerPresenter.SetTimeShifting(sliderTimeShifting.FloatValue);
        }

        partial void actionChangeSongPosition(NSObject sender)
        {

        }

        partial void actionChangeVolume(NSObject sender)
        {
            playerPresenter.SetVolume(sliderVolume.FloatValue);
        }

        partial void actionPlayLoop(NSObject sender)
        {
        }

        partial void actionStopLoop(NSObject sender)
        {
        }

        partial void actionAddLoop(NSObject sender)
        {
        }

        partial void actionEditLoop(NSObject sender)
        {           
        }

        partial void actionRemoveLoop(NSObject sender)
        {
        }

        partial void actionGoToMarker(NSObject sender)
        {
        }

        partial void actionAddMarker(NSObject sender)
        {
        }

        partial void actionEditMarker(NSObject sender)
        {
        }

        partial void actionRemoveMarker(NSObject sender)
        {
        }
        
        protected void HandleLibraryBrowserDoubleClick(object sender, EventArgs e)
        {
            // Check for selection
            if(outlineLibraryBrowser.SelectedRow == -1)
            {
                return;
            }

            try
            {
                // Get selected item and start playback
                Tracing.Log("MainWindowController.HandleLibraryBrowserDoubleClick -- Getting library browser item...");
                LibraryBrowserItem item = (LibraryBrowserItem)outlineLibraryBrowser.ItemAtRow(outlineLibraryBrowser.SelectedRow);
                Tracing.Log("MainWindowController.HandleLibraryBrowserDoubleClick -- Calling LibraryBrowserPresenter.TreeNodeDoubleClicked...");
                libraryBrowserPresenter.TreeNodeDoubleClicked(item.Entity);
            } 
            catch (Exception ex)
            {
                // Build text
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the Main Window Controller component (when double-clicking on an item in the Library Browser):");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);

                // Show alert
                Tracing.Log(sb.ToString());
                CocoaHelper.ShowCriticalAlert(sb.ToString());
            }
        }

        protected void HandleSongBrowserDoubleClick(object sender, EventArgs e)
        {
            // Check for selection
            if (tableSongBrowser.SelectedRow == -1)
            {
                return;
            }

            try
            {
                // Get selected item and start playback
                AudioFile audioFile = songBrowserSource.Items[tableSongBrowser.SelectedRow].AudioFile;
                songBrowserPresenter.TableRowDoubleClicked(audioFile);
            } 
            catch (Exception ex)
            {
                // Build text
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the Main Window Controller component (when double-clicking on an item in the Song Browser):");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);

                // Show alert
                Tracing.Log(sb.ToString());
                CocoaHelper.ShowCriticalAlert(sb.ToString());
            }
        }

        void StartUpdateLibrary(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
        {
            // Create window and start process
            if(updateLibraryWindowController != null) {
                updateLibraryWindowController.Dispose();
            }

            updateLibraryWindowController = new UpdateLibraryWindowController(this);
            updateLibraryWindowController.Window.MakeKeyAndOrderFront(this);
            updateLibraryWindowController.StartProcess(mode, filePaths, folderPath);
        }

        public void RefreshAll()
        {
            libraryBrowserPresenter.AudioFileFormatFilterChanged(AudioFileFormat.All);
            songBrowserPresenter.ChangeQuery(songBrowserPresenter.Query);
        }

        #region IPlayerView implementation

		public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            lblPosition.StringValue = entity.Position;           
            sliderPosition.SetPosition(entity.PositionPercentage * 100);
		}
		
		public void RefreshSongInformation(SongInformationEntity entity)
        {
            // Set labels
            lblArtistName.StringValue = entity.ArtistName;
            lblAlbumTitle.StringValue = entity.AlbumTitle;
            lblSongTitle.StringValue = entity.Title;
            lblSongPath.StringValue = entity.FilePath;
            lblPosition.StringValue = entity.Position;
            lblLength.StringValue = entity.Length;

            lblFileType.StringValue = entity.FileTypeString;
            lblBitrate.StringValue = entity.BitrateString;
            lblBitsPerSample.StringValue = entity.BitsPerSampleString;
            lblSampleRate.StringValue = entity.SampleRateString;

            // Set album cover
            if (!String.IsNullOrEmpty(entity.FilePath))
            {
                NSImage image = AlbumCoverHelper.GetAlbumCover(entity.FilePath);
                if (image != null)
                    imageAlbumCover.Image = image;
                else
                    imageAlbumCover.Image = NSImage.ImageNamed("NSUser");
            } 
            else
            {
                imageAlbumCover.Image = NSImage.ImageNamed("NSUser");
            }

            // Refresh which song is playing in the Song Browser
            if(songBrowserSource != null)
                songBrowserSource.RefreshIsPlaying(tableSongBrowser, entity.FilePath);
		}

        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
        {
            lblVolume.StringValue = entity.VolumeString;
            if(sliderVolume.FloatValue != entity.Volume)
                sliderVolume.FloatValue = entity.Volume;
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {
            lblTimeShifting.StringValue = entity.TimeShiftingString;
            if(sliderTimeShifting.FloatValue != entity.TimeShifting)
                sliderTimeShifting.FloatValue = entity.TimeShifting;
        }

        public void PlayerError(Exception ex)
        {
            // Build text
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("An error occured in the Player component:");
            sb.AppendLine(ex.Message);
            sb.AppendLine();
            sb.AppendLine(ex.StackTrace);

            // Show alert
            Tracing.Log(sb.ToString());
            CocoaHelper.ShowCriticalAlert(sb.ToString());
        }

        #endregion

		#region ISongBrowserView implementation

		public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
        {
            // Set data source
            songBrowserSource = new SongBrowserSource(audioFiles);
            tableSongBrowser.Source = songBrowserSource;
            albumCoverSource = new AlbumCoverSource(albumCoverCacheService, audioFiles);
            tableAlbumCovers.Source = albumCoverSource;
		}

//        public void RefreshCurrentlyPlayingSong(AudioFile audioFile)
//        {
//            songBrowserSource.RefreshIsPlaying(tableSongBrowser, audioFile);
//        }

		#endregion

		#region ILibraryBrowserView implementation

		public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities)
		{
			// Set Library Browser data source
			libraryBrowserDataSource = new LibraryBrowserDataSource(entities, this.libraryBrowserPresenter);
			outlineLibraryBrowser.DataSource = libraryBrowserDataSource;
		}

		public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
		{
		    // Not used in Cocoa.
		}

		#endregion

	}
}

