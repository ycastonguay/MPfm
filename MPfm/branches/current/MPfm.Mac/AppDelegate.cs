//
// AppDelegate.cs: App delegate. Uses Ninject to create the MainWindow.
//
// Copyright © 2011-2012 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreFoundation;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MPfm.MVP;
using Ninject;

namespace MPfm.Mac
{
    /// <summary>
    /// App delegate. Uses Ninject to create the MainWindow.
    /// </summary>
	public partial class AppDelegate : NSApplicationDelegate
	{
        SplashWindowController splashWindowController;
		MainWindowController mainWindowController;
        NavigationManager navigationManager;
		
		public AppDelegate()
		{
		}

		public override void FinishedLaunching(NSObject notification)
		{
            // Add view implementations to IoC
            Bootstrapper.GetKernel().Bind<NavigationManager>().To<MacNavigationManager>();
            Bootstrapper.GetKernel().Bind<ISplashView>().To<SplashWindowController>();
            Bootstrapper.GetKernel().Bind<IMainView>().To<MainWindowController>();
            Bootstrapper.GetKernel().Bind<IUpdateLibraryView>().To<UpdateLibraryWindowController>();
            Bootstrapper.GetKernel().Bind<IPlaylistView>().To<PlaylistWindowController>();
            Bootstrapper.GetKernel().Bind<IEffectsView>().To<EffectsWindowController>();
            Bootstrapper.GetKernel().Bind<IPreferencesView>().To<PreferencesWindowController>();

            // Create and start navigation manager
            navigationManager = Bootstrapper.GetKernel().Get<NavigationManager>();
            navigationManager.Start();




            // Create splash screen
            //splashWindowController = Bootstrapper.GetKernel().Get<SplashWindowController>();
            //splashWindowController.Window.AlphaValue = 0;
            //splashWindowController.Window.MakeKeyAndOrderFront(this);
            //splashWindowController.Window.OrderFront(this);

//            // Fade in splash screen
//            NSMutableDictionary dict = new NSMutableDictionary();
//            dict.Add(NSViewAnimation.TargetKey, splashWindowController.Window);
//            dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeInEffect);
//            NSViewAnimation anim = new NSViewAnimation(new List<NSMutableDictionary>(){ dict }.ToArray());
//            anim.Duration = 0.4f;
//            anim.StartAnimation();
        }

        public void LoadScreens()
        {
            // Load main window
            //mainWindowController = Bootstrapper.GetKernel().Get<MainWindowController>();
            //mainWindowController.Window.AlphaValue = 0;
            //mainWindowController.Window.MakeKeyAndOrderFront(this);
            //mainWindowController.Window.OrderBack(this);

//            // Fade in main window
//            NSMutableDictionary dict = new NSMutableDictionary();
//            dict.Add(NSViewAnimation.TargetKey, mainWindowController.Window);
//            dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeInEffect);
//            NSViewAnimation anim = new NSViewAnimation(new List<NSMutableDictionary>(){ dict }.ToArray());
//            anim.Duration = 0.4f;
//            anim.StartAnimation();
//            anim.AnimationDidEnd += HandleAnimationDidEnd;
        }

        void HandleAnimationDidEnd(object sender, EventArgs e)
        {
            splashWindowController.Close();
        }
	}
}
