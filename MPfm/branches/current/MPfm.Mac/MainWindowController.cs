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
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController, IMainView
	{
		private MainPresenter presenter = null;		
		private NSTimer timer = null;
		private NSOpenPanel openPanel = null;
		private UpdateLibraryWindowController updateLibraryWindowController = null;

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
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController(NSCoder coder) : base (coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController() : base ("MainWindow")
		{
			Initialize();
		}		
		
		// Shared initialization code
		void Initialize()
		{
			// Initialize bootstrapper
			IKernel kernel = Bootstrapper.GetServiceLocator();

			// Create presenter
			presenter = new MPfm.MVP.MainPresenter(this);
		}
		
		public override void WindowDidLoad()
		{
			base.WindowDidLoad ();			
		}
		
		public override void AwakeFromNib()
		{

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
				presenter.Player.Playlist.Clear();
				presenter.Player.Playlist.AddItems(filePaths.ToList());
				presenter.Play();
			}
		}

		partial void actionUpdateLibrary(NSObject sender)
		{
			StartUpdateLibrary(UpdateLibraryMode.WholeLibrary, null, null);
		}

		partial void actionPlay(NSObject sender)
		{
			presenter.Play();
		}

		partial void actionPause(NSObject sender)
		{
			presenter.Pause();
		}

		partial void actionStop(NSObject sender)
		{
			presenter.Stop();
		}

		partial void actionPrevious(NSObject sender)
		{
			presenter.Previous();
		}

		partial void actionNext(NSObject sender)
		{
			presenter.Next();
		}

		partial void actionRepeatType(NSObject sender)
		{
			presenter.RepeatType();
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

	}
}

