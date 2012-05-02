using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.UI;

namespace MPfm.Mac
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		private MPfm.UI.MainWindowController controller = null;
		
		private NSTimer timer = null;
		
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
						
			
			
			//controller.Player.OnPlaylistIndexChanged += HandlePlayerOnPlaylistIndexChanged;
			//controller.Player.OnPlaylistIndexChanged += HandleControllerPlayerOnPlaylistIndexChanged;
		}
		
		public override void WindowDidLoad()
		{
			base.WindowDidLoad ();			
		}
		
		public override void AwakeFromNib()
		{
			//using(NSAutoreleasePool pool = new NSAutoreleasePool())
			//{			
				controller = new MPfm.UI.MainWindowController();
				controller.CreatePlayer();
			//}
			
			lblArtistName.StringValue = "Test223";		
			
			
			timer = NSTimer.CreateRepeatingScheduledTimer(0.1, delegate {  
			
				//BeginInvokeOnMainThread(delegate() {
				if(controller.Player.IsPlaying)
				{
					
					PlayerPositionEntity position = controller.GetPlayerPosition();
					
					lblPosition.StringValue = position.Position;
							//using(NSAutoreleasePool pool = new NSAutoreleasePool())
			//{	
					//lblPosition.StringValue = DateTime.Now.ToLongTimeString();
					//lblPosition.StringValue = controller.Player.GetPosition().ToString() + " " + DateTime.Now.ToLongTimeString();
				}
				//}
				//});
				
			});
		}

		void HandleControllerPlayerOnPlaylistIndexChanged(MPfm.Player.PlayerPlaylistIndexChangedData data)
		{
			
		}
		
		#endregion
		
		partial void toolbarOpenAudioFiles_Click(NSObject sender)
		{
			
			
			var openPanel = new NSOpenPanel();
			openPanel.ReleasedWhenClosed = true;
			openPanel.AllowsMultipleSelection = true;
			openPanel.Prompt = "Please select a file";
//			
			var result = openPanel.RunModal();
			if(result == 1)
			{
				lblSongPath.StringValue = openPanel.Filename;
				
				List<string> files = openPanel.Filenames.ToList();
				
							//using(NSAutoreleasePool pool = new NSAutoreleasePool())
			//{	
				controller.Player.PlayFiles(files);
				//}
				
				//timer = NSTimer.CreateRepeatingScheduledTimer(1, delegate{ lblPosition.StringValue = DateTime.Now.ToLongTimeString(); });
				//timer = NSTimer.CreateRepeatingScheduledTimer(1, delegate{ lblPosition.StringValue = controller.Player.GetPosition().ToString(); });
				//timer = NSTimer.CreateRepeatingScheduledTimer(1,  BeginInvokeOnMainThread(delegate{ lblPosition.StringValue = DateTime.Now.ToLongTimeString(); });
				
				
			}
		}
		
		partial void _ButtonClick(NSObject sender)
		{
			//List<string> files = new List<string>();
			//files.Add("/Users/animal/Documents/test.ogg");			
			//controller.Player.PlayFiles(files);
			
//			_Label1.StringValue = "Hello";
//			
			lblArtistName.StringValue = "Stuff";
		}

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
	}
}

