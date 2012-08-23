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

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
        }

        public void InitDone()
        {
            InvokeOnMainThread(delegate {

//                // Fade in splash screen
//                NSMutableDictionary dict = new NSMutableDictionary();
//                dict.Add(NSViewAnimation.TargetKey, Window);
//                dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeOutEffect);
//                NSViewAnimation anim = new NSViewAnimation(new List<NSMutableDictionary>(){ dict }.ToArray());
//                anim.Duration = 0.4f;
//                anim.StartAnimation();

                // Load screens
                AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
                appDelegate.LoadScreens();
            });
        }

        #endregion

    }
}
