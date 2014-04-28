// Copyright © 2011-2013 Yanick Castonguay
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
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.MVP.Views;
using MPfm.Mac.Classes.Objects;

namespace MPfm.Mac
{
    public partial class SplashWindowController : BaseWindowController, ISplashView
    {
        #region Constructors
        
        // Called when created from unmanaged code
        public SplashWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public SplashWindowController(Action<IBaseView> onViewReady) 
            : base ("SplashWindow", onViewReady)
        {
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
            this.Window.AlphaValue = 0;
            ShowWindowCentered();

            // Fade in splash screen
            NSMutableDictionary dict = new NSMutableDictionary();
            dict.Add(NSViewAnimation.TargetKey, Window);
            dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeInEffect);
            NSViewAnimation anim = new NSViewAnimation(new List<NSMutableDictionary>(){ dict }.ToArray());
            anim.Duration = 0.25f;
            anim.StartAnimation();
        }
        
        #endregion
        
        //strongly typed window accessor
        public new SplashWindow Window
        {
            get
            {
                return (SplashWindow)base.Window;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            // Load image
            //imageView.Image = ImageResources.imageSplash;
            //var imageSplash = new NSImage(NSBundle.MainBundle.PathForResource("Splash", "png", "Resources", string.Empty));
            var imageSplashLogoClear = new NSImage(NSBundle.MainBundle.PathForResource("splash_logo_clear", "png", "Resources/Splash", string.Empty));
            var imageSplashLogoFull = new NSImage(NSBundle.MainBundle.PathForResource("splash_logo_full", "png", "Resources/Splash", string.Empty));
            //imageViewLogo.WantsLayer = true;
            //imageViewLogo.AlphaValue = 1;
            imageViewLogo.Image = imageSplashLogoClear;
            imageViewLogoFull.WantsLayer = true;
            imageViewLogoFull.AlphaValue = 0;
            imageViewLogoFull.Image = imageSplashLogoFull;

            NSAnimationContext.BeginGrouping();
            NSAnimationContext.CurrentContext.Duration = 0.6;
            //(imageViewLogo.Animator as NSImageView).AlphaValue = 0;
            (imageViewLogoFull.Animator as NSImageView).AlphaValue = 1;
            NSAnimationContext.EndGrouping();

            Window.BackgroundColor = GlobalTheme.MainWindowColor;

            // Center position
            float x = (NSScreen.MainScreen.Frame.Width - this.Window.Frame.Width) / 2;
            float y = (NSScreen.MainScreen.Frame.Height - this.Window.Frame.Height) / 2;
            //this.Window.SetFrameTopLeftPoint(new PointF(x, y));
            this.Window.SetFrameOrigin(new PointF(x, y));
            this.Window.Display();

            // Set fonts
            lblMessage.Font = NSFont.FromFontName("Roboto Light", 13.0f);
            lblMessage.StringValue = "Loading player...";

            // Set view as ready
            OnViewReady.Invoke(this);
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
            InvokeOnMainThread(delegate
            {
                lblMessage.StringValue = message;
            });
        }

        public void DestroyView()
        {
        }

        public void InitDone(bool isAppFirstStart)
        {
            InvokeOnMainThread(delegate {
                lblMessage.StringValue = "Initialization successful!";                

                // Fade out splash screen
                NSMutableDictionary dict = new NSMutableDictionary();
                dict.Add(NSViewAnimation.TargetKey, Window);
                dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeOutEffect);
                NSViewAnimation anim = new NSViewAnimation(new List<NSMutableDictionary>(){ dict }.ToArray());
                anim.Duration = 0.25f;
                anim.StartAnimation();

            });
        }

        #endregion

    }
}
