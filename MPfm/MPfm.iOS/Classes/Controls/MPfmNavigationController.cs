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
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmNavigationController")]
    public class MPfmNavigationController : UINavigationController
    {
        bool _isPlayerPlaying;
        bool _isViewPlayer;
        UILabel _lblTitle;
        UILabel _lblSubtitle;
        MPfmFlatButton _btnBack;
        MPfmFlatButton _btnEffects;
        MPfmFlatButton _btnNowPlaying;
        ITinyMessengerHub _messengerHub;

        public MPfmFlatButton BtnEffects { get { return _btnEffects; } }
        public MPfmFlatButton BtnNowPlaying { get { return _btnNowPlaying; } }
        public MobileNavigationTabType TabType { get; set; }

        public event EventHandler ViewDismissedEvent;
        
        public MPfmNavigationController(MobileNavigationTabType tabType) : base(typeof(MPfmNavigationBar), typeof(UIToolbar))
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
            _lblTitle = new UILabel(new RectangleF(60, 4, UIScreen.MainScreen.Bounds.Width - 120, 20));
            _lblTitle.TextColor = UIColor.White;
            _lblTitle.BackgroundColor = UIColor.Clear;
            _lblTitle.Text = "MPfm";
            _lblTitle.TextAlignment = UITextAlignment.Center;
            _lblTitle.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            
            _lblSubtitle = new UILabel(new RectangleF(60, 20, UIScreen.MainScreen.Bounds.Width - 120, 20));
            _lblSubtitle.LineBreakMode = UILineBreakMode.TailTruncation;
            _lblSubtitle.TextColor = UIColor.LightGray;
            _lblSubtitle.BackgroundColor = UIColor.Clear;
            _lblSubtitle.Text = "Library Browser";
            _lblSubtitle.TextAlignment = UITextAlignment.Center;
            _lblSubtitle.Font = UIFont.FromName("HelveticaNeue", 12);

            _btnBack = new MPfmFlatButton();
            _btnBack.Alpha = 0;
            _btnBack.Frame = new RectangleF(0, 0, 70, 44);
            _btnBack.OnButtonClick += () =>  {
                if(ViewControllers.Length > 1)
                    PopViewControllerAnimated(true);
            };

            _btnEffects = new MPfmFlatButton();
            _btnEffects.LabelAlignment = UIControlContentHorizontalAlignment.Right;
            _btnEffects.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 70, 0, 70, 44);
            _btnEffects.Alpha = 0;
            _btnEffects.Label.TextAlignment = UITextAlignment.Right;
            _btnEffects.Label.Text = "Effects";
            _btnEffects.Label.Frame = new RectangleF(0, 0, 44, 44);
            _btnEffects.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_blue");
            _btnEffects.ImageChevron.Frame = new RectangleF(70 - 22, 0, 22, 44);
            _btnEffects.OnButtonClick += () => {
                _messengerHub.PublishAsync<MobileNavigationManagerCommandMessage>(new MobileNavigationManagerCommandMessage(this, MobileNavigationManagerCommandMessageType.ShowEqualizerPresetsView));
            };

            _btnNowPlaying = new MPfmFlatButton();
            _btnNowPlaying.LabelAlignment = UIControlContentHorizontalAlignment.Right;
            _btnNowPlaying.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 70, 0, 70, 44);
            _btnNowPlaying.Alpha = 0;
            _btnNowPlaying.Label.TextAlignment = UITextAlignment.Right;
            _btnNowPlaying.Label.Frame = new RectangleF(0, 0, 44, 44);
            _btnNowPlaying.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_blue");
            _btnNowPlaying.ImageChevron.Frame = new RectangleF(70 - 22, 0, 22, 44);
            _btnNowPlaying.Label.Text = "Player";
            _btnNowPlaying.OnButtonClick += () => {
                _messengerHub.PublishAsync<MobileNavigationManagerCommandMessage>(new MobileNavigationManagerCommandMessage(this, MobileNavigationManagerCommandMessageType.ShowPlayerView));
            };

            this.NavigationBar.AddSubview(_btnBack);
            this.NavigationBar.AddSubview(_btnEffects);
            this.NavigationBar.AddSubview(_btnNowPlaying);
            this.NavigationBar.AddSubview(_lblTitle);
            this.NavigationBar.AddSubview(_lblSubtitle);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            float width = UIScreen.MainScreen.Bounds.Width;
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft ||
                UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
                width = UIScreen.MainScreen.Bounds.Height;

            _lblTitle.Frame = new RectangleF(78, 4, width - 156, 20);
            _lblSubtitle.Frame = new RectangleF(78, 20, width - 156, 20);
            _btnBack.Frame = new RectangleF(0, 0, 70, 44);
            _btnEffects.Frame = new RectangleF(width - 70, 0, 70, 44);
            _btnNowPlaying.Frame = new RectangleF(width - 70, 0, 70, 44);
        }
        
        protected virtual void OnViewDismissed(EventArgs e)
        {
            if(ViewDismissedEvent != null)
                ViewDismissedEvent(this, e);
        }

        public override void DismissViewController(bool animated, NSAction completionHandler)
        {
            base.DismissViewController(animated, completionHandler);
            OnViewDismissed(new EventArgs());
        }

        private void UpdateNowPlayingView()
        {
            InvokeOnMainThread(() => {
                //Console.WriteLine("NavCtrl (" + TabType.ToString() + ") - UpdateNowPlayingView: isPlayerPlaying=" + _isPlayerPlaying.ToString() + " isViewPlayer=" + _isViewPlayer.ToString());
                if(_isPlayerPlaying && !_isViewPlayer)
                {
                    //Console.WriteLine("NavCtrl - Showing Now Playing view...");
                    UIView.Animate(0.2f, () => {
                        _btnEffects.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width, 0, 70, 44);
                        _btnEffects.Alpha = 0;
                        _btnNowPlaying.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 70, 0, 70, 44);
                        _btnNowPlaying.Alpha = 1;
                    });
                }
                else if(_isViewPlayer)
                {
                    UIView.Animate(0.2f, () => {
                        _btnEffects.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 70, 0, 70, 44);
                        _btnEffects.Alpha = 1;
                        _btnNowPlaying.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width, 0, 70, 44);
                        _btnNowPlaying.Alpha = 0;
                    });
                }
            });
        }

        [Export("navigationBar:shouldPushItem:")]
        public bool ShouldPushItem(UINavigationItem item)
        {
            if(this.VisibleViewController is PlayerViewController)
                _isViewPlayer = true;
            else
                _isViewPlayer = false;
            UpdateNowPlayingView();

            if (ViewControllers.Length == 2)
            {
                _btnBack.Frame = new RectangleF(50, _btnBack.Frame.Y, _btnBack.Frame.Width, _btnBack.Frame.Height);
                UIView.Animate(0.2, () => { 
                    _btnBack.Frame = new RectangleF(0, _btnBack.Frame.Y, _btnBack.Frame.Width, _btnBack.Frame.Height);
                    _btnBack.Alpha = 1;
                });
            }

            return true;
        }

        [Export("navigationBar:shouldPopItem:")]
        public bool ShouldPopItem(UINavigationItem item)
        {
            if(this.VisibleViewController is PlayerViewController)
                _isViewPlayer = true;
            else
                _isViewPlayer = false;
            UpdateNowPlayingView();

            if (ViewControllers.Length == 1)
            {
                _btnBack.Frame = new RectangleF(0, _btnBack.Frame.Y, _btnBack.Frame.Width, _btnBack.Frame.Height);
                UIView.Animate(0.2, () => { 
                    _btnBack.Frame = new RectangleF(50, _btnBack.Frame.Y, _btnBack.Frame.Width, _btnBack.Frame.Height);
                    _btnBack.Alpha = 0;
                });
            }

            return true;
        }

        public void SetTitle(string title, string subtitle)
        {
            if(_lblTitle.Text != title)
                _lblTitle.Alpha = 0;

            if(_lblSubtitle.Text != title)
                _lblSubtitle.Alpha = 0;

            _lblTitle.Text = title;
            _lblSubtitle.Text = subtitle;

            UIView.Animate(0.2f, delegate() {
                _lblTitle.Alpha = 1;
                _lblSubtitle.Alpha = 1;
            });
        }

        public void SetBackButtonVisible(bool visible)
        {
            _btnBack.Hidden = !visible;
        }
    }
}
