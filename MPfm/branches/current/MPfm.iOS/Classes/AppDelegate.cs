using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MPfm.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		MPfm_iOSViewController viewController;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
					
			viewController = new MPfm_iOSViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

            // Show status bar again
            UIApplication.SharedApplication.SetStatusBarHidden(false, true);
			
			return true;
		}

        public override void WillTerminate(UIApplication application)
        {
            // Clean up player
            if (MPfm.Player.Player.CurrentPlayer.IsPlaying)
            {
                MPfm.Player.Player.CurrentPlayer.Stop();
            }
            MPfm.Player.Player.CurrentPlayer.Dispose();
        }
	}
}

