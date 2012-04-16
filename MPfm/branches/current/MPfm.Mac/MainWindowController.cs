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
			controller = new MPfm.UI.MainWindowController();
			controller.CreatePlayer();
		}
		
		#endregion

		partial void _ButtonClick(NSObject sender)
		{		
			//List<string> files = new List<string>();
			//files.Add("/Users/animal/Documents/test.ogg");			
			//controller.Player.PlayFiles(files);
			
//			_Label1.StringValue = "Hello";
//			
			var openPanel = new NSOpenPanel();
			openPanel.ReleasedWhenClosed = true;
			openPanel.AllowsMultipleSelection = true;
			openPanel.Prompt = "Please select a file";
//			
			var result = openPanel.RunModal();
			if(result == 1)
			{
				_Label1.StringValue = openPanel.Filename;
			}
			
			List<string> files = openPanel.Filenames.ToList();
			controller.Player.PlayFiles(files);
		}

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
	}
}

