using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MPfm.Mac
{
	public partial class LibraryBrowserController : MonoMac.AppKit.NSViewController
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public LibraryBrowserController(IntPtr handle) : base (handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LibraryBrowserController(NSCoder coder) : base (coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public LibraryBrowserController() : base ("LibraryBrowser", NSBundle.MainBundle)
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
		}
		
		#endregion
		
		//strongly typed view accessor
		public new LibraryBrowser View {
			get {
				return (LibraryBrowser)base.View;
			}
		}
	}
}

