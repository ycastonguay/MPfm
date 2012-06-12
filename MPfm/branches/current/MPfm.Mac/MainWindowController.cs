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

namespace MPfm.Mac
{
    /// <summary>
    /// Main window controller.
    /// </summary>
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController, IPlayerView, ISongBrowserView, ILibraryBrowserView
	{
		private readonly IInitializationService initializationService = null;
		private readonly IPlayerPresenter playerPresenter = null;
		private readonly ISongBrowserPresenter songBrowserPresenter = null;
		private readonly ILibraryBrowserPresenter libraryBrowserPresenter = null;

		private UpdateLibraryWindowController updateLibraryWindowController = null;
        private LibraryBrowserOutlineViewDelegate libraryBrowserOutlineViewDelegate = null;
		private LibraryBrowserDataSource libraryBrowserDataSource = null;
        private SongBrowserDataSource songBrowserDataSource = null;
        private List<NSImage> listImages = null;

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
		public MainWindowController(IInitializationService initializationService,
									IPlayerPresenter playerPresenter,
		                            ISongBrowserPresenter songBrowserPresenter,
		                            ILibraryBrowserPresenter libraryBrowserPresenter) : base ("MainWindow")
        {
            // Set properties
            this.playerPresenter = playerPresenter;
            this.songBrowserPresenter = songBrowserPresenter;
            this.libraryBrowserPresenter = libraryBrowserPresenter;
            this.initializationService = initializationService;

            // Initialize configuration and library
            initializationService.CreateConfiguration();
            initializationService.CreateLibrary();
		}		

		public override void WindowDidLoad()
		{
			base.WindowDidLoad();			
		}

		public override void AwakeFromNib()
        {
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
            libraryBrowserOutlineViewDelegate = new LibraryBrowserOutlineViewDelegate();
            viewLibraryBrowser.Delegate = libraryBrowserOutlineViewDelegate;
            viewLibraryBrowser.AllowsMultipleSelection = false;
            viewLibraryBrowser.DoubleClick += HandleDoubleClick;

            // Load images
            listImages = new List<NSImage>() {
                new NSImage(NSBundle.MainBundle.PathForResource("document-open", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("drive-harddisk", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-start", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-pause", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-stop", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("media-skip-backward", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("media-skip-forward", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("view-refresh", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("preferences-desktop", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("audio-x-generic", "png", "Resources", string.Empty)),
                new NSImage(NSBundle.MainBundle.PathForResource("preferences-system", "png", "Resources", string.Empty))
            };

            // Load images in toolbar
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarOpen").Image = listImages[0];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarUpdateLibrary").Image = listImages[1];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlay").Image = listImages[2];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPause").Image = listImages[3];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarStop").Image = listImages[4];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPrevious").Image = listImages[5];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarNext").Image = listImages[6];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarRepeat").Image = listImages[7];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarEffects").Image = listImages[8];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlaylist").Image = listImages[9];
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPreferences").Image = listImages[10];

			// Bind views
            this.playerPresenter.BindView(this);
			this.songBrowserPresenter.BindView(this);
			this.libraryBrowserPresenter.BindView(this);
		}

        void HandleDoubleClick(object sender, EventArgs e)
        {
            int z = 0;
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

		partial void actionOpenPlaylistWindow(NSObject sender)
		{

		}

		partial void actionOpenEffectsWindow(NSObject sender)
		{

		}

		partial void actionOpenPreferencesWindow(NSObject sender)
		{

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

        }

        partial void actionChangeSongPosition(NSObject sender)
        {

        }

        partial void actionChangeVolume(NSObject sender)
        {

        }

		public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            lblPosition.StringValue = entity.Position;
            sliderPosition.FloatValue = entity.PositionPercentage * 100;
		}
		
		public void RefreshSongInformation(SongInformationEntity entity)
        {									
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

            NSImage image = AlbumCoverHelper.GetAlbumCover(entity.FilePath);
            if(image != null)
                imageAlbumCover.Image = image;
		}

		private void StartUpdateLibrary(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
		{
			// Create window and start process
			if(updateLibraryWindowController != null) {
				updateLibraryWindowController.Dispose();
			}

			updateLibraryWindowController = new UpdateLibraryWindowController();
			updateLibraryWindowController.Window.MakeKeyAndOrderFront(this);
			updateLibraryWindowController.StartProcess(mode, filePaths, folderPath);
		}

		#region ISongBrowserView implementation

		public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
        {
            // Set data source
            songBrowserDataSource = new SongBrowserDataSource(audioFiles);
            tableSongBrowser.DataSource = songBrowserDataSource;
		}

		#endregion

		#region ILibraryBrowserView implementation

		public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities)
		{
			// Set Library Browser data source
			libraryBrowserDataSource = new LibraryBrowserDataSource(entities, this.libraryBrowserPresenter);
			viewLibraryBrowser.DataSource = libraryBrowserDataSource;
		}

		public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
		{
		    // Not used in Cocoa.
		}

		#endregion

	}
}

