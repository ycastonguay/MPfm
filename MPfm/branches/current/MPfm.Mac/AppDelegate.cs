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
        CTFont ctFont;
		
		public AppDelegate()
		{
		}

		public override void FinishedLaunching(NSObject notification)
		{
            //Bootstrapper.GetKernel()
            splashWindowController = Bootstrapper.GetKernel().Get<SplashWindowController>();
            splashWindowController.Window.MakeKeyAndOrderFront(this);
		}

        public void LoadScreens()
        {           

//            string urlString = NSBundle.MainBundle.PathForResource("TitilliumTitle20", "otf", "Resources/Fonts", string.Empty);
////            NSUrl url = new NSUrl(urlString);
////            bool supported = CTFontManager.IsFontSupported(url);
////            if (supported)
////            {            
////                NSError error = CTFontManager.RegisterFontsForUrl(url, CTFontManagerScope.Session);
////            }
//            CGDataProvider dataProvider = new CGDataProvider(urlString);
//            //NSData fontData = NSData.FromFile(urlString);
//            CGFont cgFont = CGFont.CreateFromProvider(dataProvider);
//            CTFontDescriptor desc = new CTFontDescriptor("TitilliumTitle20", 12.0f);
//            ctFont = new CTFont(cgFont, 12.0f, desc);
//            NSFont.from
//            //fontData.ref 
//           // CGDataProviderRef 




//            //error.
//            //OSStatus status;
//            string stuff;
//            if (error != null)
//            {
//                stuff = error.LocalizedDescription;
//            }

            mainWindowController = Bootstrapper.GetKernel().Get<MainWindowController>();
            mainWindowController.Window.MakeMainWindow();


            //CFUrl url = CFUrl.FromFile(

//            NSAnimationContext.BeginGrouping();
//            NSAnimationContext.CurrentContext.Duration = 0.5;
//            //NSObject obj = splashWindowController.Window.Animator;
//            //NSWindow window = (NSWindow)splashWindowController.Window.Animator;
//            ((SplashWindow)splashWindowController.Window.Animator).AlphaValue = 0;
//            //window.AlphaValue = 0;
//            NSAnimationContext.EndGrouping();

               

            splashWindowController.Window.AlphaValue = 0;

//          NSMutableDictionary dict = new NSMutableDictionary();
//            dict.Add(NSViewAnimation.TargetKey, splashWindowController.Window);
//            dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeOutEffect);
//
//            NSViewAnimation anim = new NSViewAnimation(new NSMutableDictionary[] { dict });
//            //anim.SetValuesForKeysWithDictionary(dict);
//            //anim.Duration = 2;
//            anim.StartAnimation();
//            int a = 0;

            //splashWindowController.Test();

//            CABasicAnimation anim = new CABasicAnimation();
//            NSMutableDictionary dict = new NSMutableDictionary();
//            dict.Add(new NSString("alphaValue"), anim);
//            splashWindowController.Window.Animations = dict;
//            ((NSWindow)splashWindowController.Window.Animator).AlphaValue = 0.0f;

            //mainWindowController.Window.MakeKeyAndOrderFront(this);


        }
	}
}
