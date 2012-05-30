using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using Ninject;
using MPfm.MVP;

namespace MPfm.Mac
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		
		public AppDelegate()
		{
		}

		public override void FinishedLaunching(NSObject notification)
		{
			mainWindowController = Bootstrapper.GetKernel().Get<MainWindowController>();
			mainWindowController.Window.MakeKeyAndOrderFront(this);
		}
	}
}

