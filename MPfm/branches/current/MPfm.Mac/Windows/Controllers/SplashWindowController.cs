using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;

namespace MPfm.Mac
{
    public partial class SplashWindowController : MonoMac.AppKit.NSWindowController, ISplashView
    {
        ISplashPresenter splashPresenter;

        #region Constructors
        
        // Called when created from unmanaged code
        public SplashWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public SplashWindowController(NSCoder coder) : base (coder)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public SplashWindowController(ISplashPresenter splashPresenter) 
            : base ("SplashWindow")
        {
            this.splashPresenter = splashPresenter;
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
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
            imageView.Image = ImageResources.imageSplash;

            // Center position
            float x = (NSScreen.MainScreen.Frame.Width - this.Window.Frame.Width) / 2;
            float y = (NSScreen.MainScreen.Frame.Height - this.Window.Frame.Height) / 2;
            //this.Window.SetFrameTopLeftPoint(new PointF(x, y));
            this.Window.SetFrameOrigin(new PointF(x, y));
            this.Window.Display();
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

//            // Load screens
//            AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
//            appDelegate.LoadScreens();

            // Bind view and initialize
            splashPresenter.BindView(this);
            splashPresenter.Initialize();
        }

        public void Test()
        {
//            NSMutableDictionary dict = new NSMutableDictionary();
//            dict.Add(NSViewAnimation.TargetKey, Window);
//            dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeOutEffect);
//
//            NSViewAnimation anim = new NSViewAnimation(new NSMutableDictionary[] { dict });
//            //anim.SetValuesForKeysWithDictionary(dict);
//            //anim.Duration = 2;
//            anim.StartAnimation();

            NSAnimationContext.BeginGrouping();
            NSAnimationContext.CurrentContext.Duration = 0.5;
            //NSObject obj = splashWindowController.Window.Animator;
            //NSWindow window = (NSWindow)splashWindowController.Window.Animator;
            //((NSWindow)Window.Animator).AlphaValue = 0;
            ((NSImageView)imageView.Animator).AlphaValue = 0;
            //window.AlphaValue = 0;
            NSAnimationContext.EndGrouping();
            int a = 0;
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
        }

        public void InitDone()
        {
            // Load screens
            AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
            appDelegate.LoadScreens();
        }

        #endregion

    }
}
