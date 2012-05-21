#define MACOSX
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
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
		
		partial void toolbarOpenAudioFiles_Click(NSObject sender)
		{	
			// Open panel to choose audio files to play
			IEnumerable<string> filePaths = null;
			using(NSOpenPanel openPanel = new NSOpenPanel())
			{
				openPanel.ReleasedWhenClosed = true;
				openPanel.AllowsMultipleSelection = true;
				openPanel.AllowedFileTypes = new string[]{ "FLAC", "MP3", "OGG" };
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
		
		partial void toolbarPlay(NSObject sender)
		{
			presenter.Play();	
		}
		
		partial void toolbarPause(NSObject sender)
		{
			presenter.Pause();
		}
		
		partial void toolbarStop(NSObject sender)
		{
			presenter.Stop();
		}
		
		partial void toolbarNext(NSObject sender)
		{
			presenter.Next();
		}
		
		partial void toolbarPrevious(NSObject sender)
		{
			presenter.Previous();
		}
		
		partial void toolbarRepeatType(NSObject sender)
		{
			presenter.RepeatType();
		}
		
		partial void toolbarUpdateLibrary(NSObject sender)
		{
			updateLibraryWindowController = new UpdateLibraryWindowController(); //MPfm.Library.UpdateLibraryMode.WholeLibrary, null);// null, null);
			updateLibraryWindowController.Window.MakeKeyAndOrderFront(this);
			updateLibraryWindowController.StartProcess(MPfm.Library.UpdateLibraryMode.WholeLibrary, null, null);
		}
		
		partial void toolbarPlaylist(NSObject sender)
		{
			
		}
		
		partial void toolbarEffects(NSObject sender)
		{
			
		}
			
		partial void toolbarPreferences(NSObject sender)
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
	}
}

