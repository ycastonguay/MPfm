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
        ITinyMessengerHub _messengerHub;

        public MobileNavigationTabType TabType { get; set; }
        
        public MPfmNavigationController(MobileNavigationTabType tabType) : base()
        {
            TabType = tabType;
            WeakDelegate = this;

            // Create messenger hub to listen to player changes
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                //Console.WriteLine("NavCtrl (" + TabType.ToString() + ") - PlayerPlaylistIndexChangedMessage");
                UpdateNowPlayingView();
            });
            _messengerHub.Subscribe<PlayerStatusMessage>((message) => {
                //Console.WriteLine("NavCtrl (" + TabType.ToString() + ") - PlayerStatusMessage - Status=" + message.Status.ToString());
                if(message.Status == PlayerStatusType.Playing)
                    _isPlayerPlaying = true;
                else
                    _isPlayerPlaying = false;
                
                // TODO: Stop vinyl animation when player is paused                
                UpdateNowPlayingView();
            });

            // Create controls
            _lblTitle = new UILabel(new RectangleF(0, 6, UIScreen.MainScreen.Bounds.Width - 120, 20));
            _lblTitle.TextColor = UIColor.White;
            _lblTitle.BackgroundColor = UIColor.Clear;
            _lblTitle.Text = "MPfm";
            _lblTitle.TextAlignment = UITextAlignment.Left;
            _lblTitle.Font = UIFont.FromName("HelveticaNeue", 16);
            
            _lblSubtitle = new UILabel(new RectangleF(0, 20, UIScreen.MainScreen.Bounds.Width - 120, 20));
            _lblSubtitle.LineBreakMode = UILineBreakMode.TailTruncation;
            _lblSubtitle.TextColor = UIColor.LightGray;
            _lblSubtitle.BackgroundColor = UIColor.Clear;
            _lblSubtitle.Text = "Library Browser";
            _lblSubtitle.TextAlignment = UITextAlignment.Left;
            _lblSubtitle.Font = UIFont.FromName("HelveticaNeue-Light", 12);

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
            _btnEffects.BackgroundColor = UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 1);
            _btnEffects.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 44, 0, 44, 44);
            _btnEffects.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
            _btnEffects.SetImage(UIImage.FromBundle("Images/effects.png"), UIControlState.Normal);
            _btnEffects.Alpha = 0;
            _btnEffects.TouchUpInside += (sender, e) => {
                _messengerHub.PublishAsync<MobileNavigationManagerCommandMessage>(new MobileNavigationManagerCommandMessage(this, MobileNavigationManagerCommandMessageType.ShowEffectsView));
            };           

            _btnNowPlaying = new UIButton(UIButtonType.Custom);
            _btnNowPlaying.BackgroundColor = UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 1);
            _btnNowPlaying.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 44, 0, 44, 44);
            _btnNowPlaying.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
            _btnNowPlaying.SetImage(UIImage.FromBundle("Images/media.png"), UIControlState.Normal);
            _btnNowPlaying.Alpha = 0;
            _btnNowPlaying.TouchUpInside += (sender, e) => {
                _messengerHub.PublishAsync<MobileNavigationManagerCommandMessage>(new MobileNavigationManagerCommandMessage(this, MobileNavigationManagerCommandMessageType.ShowPlayerView));
            };

            this.NavigationBar.AddSubview(_btnBack);
            this.NavigationBar.AddSubview(_btnEffects);
            this.NavigationBar.AddSubview(_btnNowPlaying);
            this.NavigationBar.AddSubview(_lblTitle);
            this.NavigationBar.AddSubview(_lblSubtitle);
        }

        public override void ViewWillLayoutSubviews()
        {
            //Console.WriteLine("MPfmNavCtrl - ViewWillLayoutSubviews");

            float x = 12;
            if(this.VisibleViewController.NavigationItem.LeftBarButtonItem != null)
            {
                UIView view = (UIView)this.VisibleViewController.NavigationItem.LeftBarButtonItem.ValueForKey(new NSString("view"));
                float width = (view != null) ? view.Frame.Size.Width : 0; 
                x += width;
            }
            else if(this.ViewControllers.Length > 1)
            {
                x += _btnBack.Frame.Size.Width;
            }
            else if(this.ViewControllers.Length == 1)
            {
                x += _btnBack.Frame.Size.Width;
            }

            // Animate new x position only if the position has changed
            if(x != _lblTitle.Frame.X)
            {
                // Do not animate the first time we are setting the position
                if(_lblTitle.Frame.X == 0)
                {
                    _lblTitle.Frame = new RectangleF(x, 6, UIScreen.MainScreen.Bounds.Width - 120, 20);
                    _lblSubtitle.Frame = new RectangleF(x, 20, UIScreen.MainScreen.Bounds.Width - 120, 20);
                }
                else
                {
                    UIView.Animate(0.25f, () => { 
                        _lblTitle.Frame = new RectangleF(x, 6, UIScreen.MainScreen.Bounds.Width - 120, 20);
                        _lblSubtitle.Frame = new RectangleF(x, 20, UIScreen.MainScreen.Bounds.Width - 120, 20);
                    });
                }
            }

            base.ViewWillLayoutSubviews();
        }

        private void UpdateNowPlayingView()
        {
            //Console.WriteLine("NavCtrl (" + TabType.ToString() + ") - UpdateNowPlayingView: isPlayerPlaying=" + _isPlayerPlaying.ToString() + " isViewPlayer=" + _isViewPlayer.ToString());
            if(_isPlayerPlaying && !_isViewPlayer)
            {
                //Console.WriteLine("NavCtrl - Showing Now Playing view...");
                UIView.Animate(0.3f, () => {
                    _btnEffects.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width, 0, 44, 44);
                    _btnEffects.Alpha = 0;
                    _btnNowPlaying.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 44, 0, 44, 44);
                    _btnNowPlaying.Alpha = 1;
                });
            }
            else if(_isViewPlayer)
            {
                UIView.Animate(0.3f, () => {
                    _btnEffects.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 44, 0, 44, 44);
                    _btnEffects.Alpha = 1;
                    _btnNowPlaying.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width, 0, 44, 44);
                    _btnNowPlaying.Alpha = 0;
                });
            }
        }

        [Export("navigationBar:shouldPushItem:")]
        public bool ShouldPushItem(UINavigationItem item)
        {
            if(this.VisibleViewController is PlayerViewController)
                _isViewPlayer = true;
            else
                _isViewPlayer = false;

            UpdateNowPlayingView();

            if (ViewControllers.Length > 1)
            {
                UIView.Animate(0.25, () => { 
                    _btnBack.SetBackgroundImage(UIImage.FromBundle("Images/back_wide.png"), UIControlState.Normal);
                    _btnBack.Frame = new RectangleF(4, 4, 43, 36);
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

        public void SetBackButtonVisible(bool visible)
        {
            _btnBack.Hidden = !visible;
        }
    }
}
