#define MACOSX
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.Library;
using MPfm.MVP;
using MPfm.Sound;
using Ninject;

namespace MPfm.Mac
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController, IPlayerView, ISongBrowserView, ILibraryBrowserView
	{
		private readonly IInitializationService initializationService = null;
		private readonly IPlayerPresenter playerPresenter = null;
		private readonly ISongBrowserPresenter songBrowserPresenter = null;
		private readonly ILibraryBrowserPresenter libraryBrowserPresenter = null;

		private UpdateLibraryWindowController updateLibraryWindowController = null;

		private LibraryBrowserDataSource libraryBrowserDataSource = null;

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
		}		

		public override void WindowDidLoad()
		{
			base.WindowDidLoad();			
		}
		
		public override void AwakeFromNib()
		{
			// Add items to Sound Format combo box
			cboSoundFormat.RemoveAllItems();
			cboSoundFormat.AddItem("All");
			cboSoundFormat.AddItem("FLAC");
			cboSoundFormat.AddItem("OGG");
			cboSoundFormat.AddItem("MP3");
			cboSoundFormat.AddItem("MPC");
			cboSoundFormat.AddItem("WAV");
			cboSoundFormat.AddItem("WV");

			// Bind views
			this.playerPresenter.BindView(this);
			this.songBrowserPresenter.BindView(this);
			this.libraryBrowserPresenter.BindView(this);
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
		
		public void RefreshPlayerPosition(PlayerPositionEntity entity)
		{
			lblPosition.StringValue = entity.Position;
		}
		
		public void RefreshSongInformation(SongInformationEntity entity)
		{									
			lblArtistName.StringValue = entity.ArtistName;
			lblAlbumTitle.StringValue = entity.AlbumTitle;
			lblSongTitle.StringValue = entity.Title;
			lblSongPath.StringValue = entity.FilePath;
			lblPosition.StringValue = entity.Position;
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

		}

		#endregion

		#region ILibraryBrowserView implementation

		public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities)
		{
			// Set Library Browser data source
			libraryBrowserDataSource = new LibraryBrowserDataSource(entities);
			viewLibraryBrowser.DataSource = libraryBrowserDataSource;
		}

		public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
		{
		
		}

		#endregion
	}

}

