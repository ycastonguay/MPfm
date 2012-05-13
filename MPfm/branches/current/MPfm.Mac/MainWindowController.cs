#define MACOSX
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.Sound;

namespace MPfm.Mac
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController, IMainView
	{
		private MPfm.MVP.MainPresenter presenter = null;		
		private NSTimer timer = null;
		
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
			
#if (MACOSX)						
			Console.WriteLine("stuff");			
#endif			
		}
		
		public override void WindowDidLoad()
		{
			base.WindowDidLoad ();			
		}
		
		public override void AwakeFromNib()
		{
			// Create presenter
			presenter = new MPfm.MVP.MainPresenter(this);
		}
		
		#endregion
		
		partial void toolbarOpenAudioFiles_Click(NSObject sender)
		{			
			// Create open file panel
			var openPanel = new NSOpenPanel();
			openPanel.ReleasedWhenClosed = true;
			openPanel.AllowsMultipleSelection = true;
			openPanel.AllowedFileTypes = new string[]{ "FLAC", "MP3" };
			openPanel.Title = "Please select audio files to play";
			openPanel.Prompt = "Add to playlist";
//			
			var result = openPanel.RunModal();
			if(result == 1)
			{
				List<string> files = openPanel.Urls.Select(x => x.Path).ToList();
				presenter.Player.Playlist.Clear();
				presenter.Player.Playlist.AddItems(files);				
			}
			
			//using(NSAutoreleasePool pool = new NSAutoreleasePool())
			//{	
				
			//}
				
			//timer = NSTimer.CreateRepeatingScheduledTimer(1, delegate{ lblPosition.StringValue = DateTime.Now.ToLongTimeString(); });
			//timer = NSTimer.CreateRepeatingScheduledTimer(1, delegate{ lblPosition.StringValue = controller.Player.GetPosition().ToString(); });
			//timer = NSTimer.CreateRepeatingScheduledTimer(1,  BeginInvokeOnMainThread(delegate{ lblPosition.StringValue = DateTime.Now.ToLongTimeString(); });							
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
