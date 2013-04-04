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
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TinyMessenger;
using MPfm.iOS.Classes.Controllers;
using MonoTouch.CoreAnimation;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmNavigationController")]
    public class MPfmNavigationController : UINavigationController
    {
        bool _isPlayerPlaying;
        bool _isViewPlayer;
        UILabel _lblTitle;
        UILabel _lblSubtitle;
        UIButton _btnBack;
        UIButton _btnEffects;
        UIButton _btnNowPlaying;
        UIView _viewNowPlaying;
        ITinyMessengerHub _messengerHub;
        MobileNavigationTabType _tabType;
        
        public MPfmNavigationController(MobileNavigationTabType tabType) : base()
        {
            // TODO: Cannot bind this to IPlayerStatusView since there's multiple NavCtrl in the application...

            this._tabType = tabType;
            this.WeakDelegate = this;

            _btnBack = new UIButton(UIButtonType.Custom);
            _btnBack.Frame = new RectangleF(4, 4, 36, 36);
            _btnBack.SetBackgroundImage(UIImage.FromBundle("Images/back.png"), UIControlState.Normal);
            _btnBack.TouchUpInside += (sender, e) => { 
                if(ViewControllers.Length > 1)
                {
                    PopViewControllerAnimated(true);
                }
            };

            _btnEffects = new UIButton(UIButtonType.Custom);
            _btnEffects.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 4 - 36, 4, 36, 36);
            _btnEffects.SetBackgroundImage(UIImage.FromBundle("Images/effects.png"), UIControlState.Normal);
            _btnEffects.Alpha = 0;

            _lblTitle = new UILabel(new RectangleF(50, 6, UIScreen.MainScreen.Bounds.Width - 120, 20));
            _lblTitle.TextColor = UIColor.White;
            _lblTitle.BackgroundColor = UIColor.Clear;
            _lblTitle.Text = "MPfm";
            _lblTitle.TextAlignment = UITextAlignment.Left;
            _lblTitle.Font = UIFont.FromName("HelveticaNeue", 16);

            _lblSubtitle = new UILabel(new RectangleF(50, 20, UIScreen.MainScreen.Bounds.Width - 120, 20));
            _lblSubtitle.LineBreakMode = UILineBreakMode.TailTruncation;
            _lblSubtitle.TextColor = UIColor.LightGray;
            _lblSubtitle.BackgroundColor = UIColor.Clear;
            _lblSubtitle.Text = "Library Browser";
            _lblSubtitle.TextAlignment = UITextAlignment.Left;
            _lblSubtitle.Font = UIFont.FromName("HelveticaNeue-Light", 12);

            _viewNowPlaying = new UIView(new RectangleF(UIScreen.MainScreen.Bounds.Width - 4 - 36, 4, 36, 36));
            _viewNowPlaying.BackgroundColor = UIColor.Clear;
            _viewNowPlaying.Alpha = 0;

            _btnNowPlaying = new UIButton(UIButtonType.Custom);
            _btnNowPlaying.Frame = new RectangleF(4, 4, 28, 28);
            _btnNowPlaying.SetBackgroundImage(UIImage.FromBundle("Images/media.png"), UIControlState.Normal);
            _btnNowPlaying.TouchUpInside += HandleButtonNowPlayingTouchUpInside;
            _viewNowPlaying.AddSubview(_btnNowPlaying);

            // Add gradient background to Now Playing view
            CAGradientLayer gradient = new CAGradientLayer();
            gradient.Frame = _viewNowPlaying.Bounds;
            gradient.Colors = new MonoTouch.CoreGraphics.CGColor[2] { new CGColor(0.2f, 0.2f, 0.2f, 1), new CGColor(0.3f, 0.3f, 0.3f, 1) }; //[NSArray arrayWithObjects:(id)[[UIColor blackColor] CGColor], (id)[[UIColor whiteColor] CGColor], nil];
            _viewNowPlaying.Layer.InsertSublayer(gradient, 0);

            this.NavigationBar.AddSubview(_viewNowPlaying);
            this.NavigationBar.AddSubview(_btnBack);
            this.NavigationBar.AddSubview(_btnEffects);
            this.NavigationBar.AddSubview(_lblTitle);
            this.NavigationBar.AddSubview(_lblSubtitle);

            // Instead of using a view/presenter, listen to messages from TinyMessenger
            // (the NavCtrl is unique to iOS and shouldn't be created by the NavMgr in my opinion)
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                // Display the "Now Playing" icon, but only if the current view isn't PlayerViewCtrl.
                Console.WriteLine("NavCtrl (" + _tabType.ToString() + ") - PlayerPlaylistIndexChangedMessage");
                UpdateNowPlayingView();
            });
            _messengerHub.Subscribe<PlayerStatusMessage>((message) => {
                Console.WriteLine("NavCtrl (" + _tabType.ToString() + ") - PlayerStatusMessage - Status=" + message.Status.ToString());
                if(message.Status == PlayerStatusType.Playing)
                    _isPlayerPlaying = true;
                else
                    _isPlayerPlaying = false;

                // TODO: Stop vinyl animation when player is paused                
                UpdateNowPlayingView();
            });
        }

        private void HandleButtonNowPlayingTouchUpInside(object sender, EventArgs e)
        {

        }

        private void UpdateNowPlayingView()
        {
            Console.WriteLine("NavCtrl (" + _tabType.ToString() + ") - UpdateNowPlayingView: isPlayerPlaying=" + _isPlayerPlaying.ToString() + " isViewPlayer=" + _isViewPlayer.ToString());
            if(_isPlayerPlaying && !_isViewPlayer)
            {
                Console.WriteLine("NavCtrl - Showing Now Playing view...");
                UIView.Animate(0.3f, () => {
                    _btnEffects.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width, 4, 36, 36);
                    _btnEffects.Alpha = 0;

                    _viewNowPlaying.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 4 - 36, 4, 36, 36);
                    _viewNowPlaying.Alpha = 1;
                });
            }
            else if(_isViewPlayer)
            {
                UIView.Animate(0.3f, () => {
                    _btnEffects.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 4 - 36, 4, 36, 36);
                    _btnEffects.Alpha = 1;

                    _viewNowPlaying.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width, 4, 36, 36);
                    _viewNowPlaying.Alpha = 0;
                });
            }
        }

        [Export("navigationBar:shouldPushItem:")]
        public bool ShouldPushItem(UINavigationItem item)
        {
            if(this.VisibleViewController is PlayerViewController)
            {
                // Hide Now Playing view
                _isViewPlayer = true;
            }
            else
            {
                // Show Now Playing view
                _isViewPlayer = false;
            }
            UpdateNowPlayingView();

            if (ViewControllers.Length > 1)
            {
                UIView.Animate(0.25, () => { 
                    _btnBack.SetBackgroundImage(UIImage.FromBundle("Images/back_wide.png"), UIControlState.Normal);
                    _btnBack.Frame = new RectangleF(4, 4, 43, 36);
                    _lblTitle.Frame = new RectangleF(57, 6, UIScreen.MainScreen.Bounds.Width - 120, 20);//string.IsNullOrEmpty(_lblSubtitle.Text) ? 20 : 40);
                    _lblSubtitle.Frame = new RectangleF(57, 20, UIScreen.MainScreen.Bounds.Width - 120, 20);
                });
            }

            return true;
        }

        [Export("navigationBar:shouldPopItem:")]
        public bool ShouldPopItem(UINavigationItem item)
        {
            if(this.VisibleViewController is PlayerViewController)
            {
                // Hide Now Playing view
                _isViewPlayer = true;
            }
            else
            {
                // Show Now Playing view
                _isViewPlayer = false;
            }
            UpdateNowPlayingView();

            if (ViewControllers.Length == 1)
            {
                UIView.Animate(0.25, () => { 
                    _btnBack.SetBackgroundImage(UIImage.FromBundle("Images/back.png"), UIControlState.Normal);
                    _btnBack.Frame = new RectangleF(4, 4, 36, 36);
                    _lblTitle.Frame = new RectangleF(50, 6, UIScreen.MainScreen.Bounds.Width - 120, 20); //string.IsNullOrEmpty(_lblSubtitle.Text) ? 20 : 40);
                    _lblSubtitle.Frame = new RectangleF(50, 20, UIScreen.MainScreen.Bounds.Width - 120, 20);
                });
            }

            return true;
        }

        public void SetTitle(string title, string subtitle)
        {
            UIView.Animate(0.25f, delegate
            {
                if(_lblTitle.Text != title)
                    _lblTitle.Alpha = 0;

                if(!string.IsNullOrEmpty(subtitle))
                    _lblSubtitle.Alpha = 0;

            }, delegate
            {
                _lblTitle.Text = title;
                _lblSubtitle.Text = subtitle;
            });
            UIView.Animate(0.25f, delegate
            {
                _lblTitle.Alpha = 1;
                //_lblTitle.Frame = new RectangleF(50, 6, UIScreen.MainScreen.Bounds.Width - 120, string.IsNullOrEmpty(subtitle) ? 20 : 40);

//                if(!string.IsNullOrEmpty(subtitle))
//                    _lblTitle.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
//                    //_lblTitle.Font = UIFont.FromName("HelveticaNeue", 16);
//                else
//                    _lblTitle.Transform = CGAffineTransform.MakeScale(1.4f, 1.4f);
//                    //_lblTitle.Font = UIFont.FromName("HelveticaNeue", 24);


                if(!string.IsNullOrEmpty(subtitle))
                    _lblSubtitle.Alpha = 1;
            });
        }
    }
}
