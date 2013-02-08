using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MPfm.Mac
{
	public partial class LibraryBrowser : MonoMac.AppKit.NSView
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public LibraryBrowser(IntPtr handle) : base (handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LibraryBrowser(NSCoder coder) : base (coder)
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
		}
		
		#endregion
	}
}

