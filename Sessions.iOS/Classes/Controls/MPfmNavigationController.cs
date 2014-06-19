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
using System.Drawing;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Messages;
using Sessions.MVP.Navigation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TinyMessenger;
using MPfm.iOS.Classes.Controllers;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Helpers;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmNavigationController")]
    public class MPfmNavigationController : UINavigationController
    {
        bool _isPlayerPlaying;
        bool _viewShouldShowPlayerButton;
        bool _viewShouldShowEffectsButton;
        bool _viewShouldShowPlaylistButton;
        bool _confirmedViewPop;
        UILabel _lblTitle;
        UIImageView _imageViewIcon;
        MPfmFlatButton _btnBack;
        MPfmFlatButton _btnPlaylist;
		//MPfmFlatButton _btnPlaylist;
        MPfmFlatButton _btnNowPlaying;
        ITinyMessengerHub _messengerHub;

        public MPfmFlatButton BtnEffects { get { return _btnPlaylist; } }
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
                if(message.Status == PlayerStatusType.Playing ||
                   message.Status == PlayerStatusType.Paused)
                    _isPlayerPlaying = true;
                else
                    _isPlayerPlaying = false;
                
                UpdateNowPlayingView();
            });

            // Create controls
            _lblTitle = new UILabel(new RectangleF(60, 4, UIScreen.MainScreen.Bounds.Width - 120, 20));
            _lblTitle.TextColor = UIColor.White;
            _lblTitle.BackgroundColor = UIColor.Clear;
            _lblTitle.Text = "";
            _lblTitle.TextAlignment = UITextAlignment.Center;
            _lblTitle.AdjustsFontSizeToFitWidth = true;
            _lblTitle.MinimumScaleFactor = 14f/16f; // min:14pt max:16pt
            _lblTitle.Font = UIFont.FromName("HelveticaNeue", 16);

            _imageViewIcon = new UIImageView();
            _imageViewIcon.Image = UIImage.FromBundle("Images/Nav/album");
            _imageViewIcon.BackgroundColor = UIColor.Clear;

            _btnBack = new MPfmFlatButton();
            _btnBack.Alpha = 0;
            _btnBack.Frame = new RectangleF(0, 0, 70, 44);
            _btnBack.OnButtonClick += () =>  {

                var viewController = (BaseViewController)VisibleViewController;
                if (viewController.ConfirmBackButton && !_confirmedViewPop)
                {
                    var alertView = new UIAlertView(viewController.ConfirmBackButtonTitle, viewController.ConfirmBackButtonMessage, null, "OK", new string[1] { "Cancel" });
                    alertView.Clicked += (object sender, UIButtonEventArgs e) => {
                        Console.WriteLine("AlertView button index: {0}", e.ButtonIndex);
                        switch(e.ButtonIndex)
                        {
                            case 0:
                                viewController.ConfirmedBackButton();
                                _confirmedViewPop = true;
								Console.WriteLine("NavCtrl - PopViewController A");
                                PopViewControllerAnimated(true);
                                break;
                            default:
                                break;
                        }
                    };
                    alertView.Show();
                    return;
                }

                _confirmedViewPop = false;
                if(ViewControllers.Length > 1)
				{
					Console.WriteLine("NavCtrl - PopViewController B");
                    PopViewControllerAnimated(true);
				}
            };

            _btnPlaylist = new MPfmFlatButton();
            _btnPlaylist.LabelAlignment = UIControlContentHorizontalAlignment.Right;
			_btnPlaylist.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 80, 0, 80, 44);
            _btnPlaylist.Alpha = 0;
            _btnPlaylist.Label.TextAlignment = UITextAlignment.Right;
			_btnPlaylist.Label.Text = "Playlist";
			_btnPlaylist.Label.Frame = new RectangleF(0, 0, 54, 44);
            _btnPlaylist.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_blue");
			_btnPlaylist.ImageChevron.Frame = new RectangleF(80 - 22, 0, 22, 44);
            _btnPlaylist.OnButtonClick += () => {
				_messengerHub.PublishAsync<MobileNavigationManagerCommandMessage>(new MobileNavigationManagerCommandMessage(this, MobileNavigationManagerCommandMessageType.ShowPlaylistView));
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
            this.NavigationBar.AddSubview(_btnPlaylist);
            this.NavigationBar.AddSubview(_btnNowPlaying);
            this.NavigationBar.AddSubview(_lblTitle);
            this.NavigationBar.AddSubview(_imageViewIcon);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            var screenSize = UIKitHelper.GetDeviceSize();
            _btnBack.Frame = new RectangleF(0, 0, 70, 44);
			_btnPlaylist.Frame = new RectangleF(screenSize.Width - 80, 0, 80, 44);
            _btnNowPlaying.Frame = new RectangleF(screenSize.Width - 70, 0, 70, 44);
            SetTitleFrame();
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
                //Console.WriteLine("NavCtrl (" + TabType.ToString() + ") - UpdateNowPlayingView: isPlayerPlaying=" + _isPlayerPlaying.ToString() + " viewShouldShowPlayerButton=" + _viewShouldShowPlayerButton.ToString());
                var screenSize = UIKitHelper.GetDeviceSize();
                if(_isPlayerPlaying && _viewShouldShowPlayerButton)
                {
                    UIView.Animate(0.2f, () => {
                        _btnNowPlaying.Frame = new RectangleF(screenSize.Width - 70, 0, 70, 44);
                        _btnNowPlaying.Alpha = 1;
                    });
                }
                else if(!_viewShouldShowPlayerButton)
                {
                    UIView.Animate(0.2f, () => {
                        _btnNowPlaying.Frame = new RectangleF(screenSize.Width, 0, 70, 44);
                        _btnNowPlaying.Alpha = 0;
                    });
                }

                if(_viewShouldShowEffectsButton)
                {
                    UIView.Animate(0.2f, () => {
						_btnPlaylist.Frame = new RectangleF(screenSize.Width - 80, 0, 80, 44);
                        _btnPlaylist.Alpha = 1;
                    });
                }
                else
                {
                    UIView.Animate(0.2f, () => {
						_btnPlaylist.Frame = new RectangleF(screenSize.Width, 0, 80, 44);
                        _btnPlaylist.Alpha = 0;
                    });
                }

                if(_viewShouldShowPlaylistButton)
                {
                    UIView.Animate(0.2f, () => {
						//_lblTitle.Alpha = 0;
                        _imageViewIcon.Alpha = 0;
						//_btnPlaylist.Alpha = 1;
                    });
                }
                else
                {
                    UIView.Animate(0.2f, () => {
                        _imageViewIcon.Alpha = 1;
						//_lblTitle.Alpha = 1;
						//_btnPlaylist.Alpha = 0;
					});
                }
            });
        }

        [Export("navigationBar:shouldPushItem:")]
        public bool ShouldPushItem(UINavigationItem item)
        {
			//Console.WriteLine("NavCtrl - ShouldPushItem - VisibleViewCtrl: {0}", VisibleViewController.GetType().FullName);
            SetButtonVisibility();
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
			//Console.WriteLine("NavCtrl - ShouldPopItem - VisibleViewCtrl: {0}", VisibleViewController.GetType().FullName);
            SetButtonVisibility();
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

        private void SetButtonVisibility()
        {
            if(VisibleViewController is PlayerViewController ||
               VisibleViewController is PlaylistViewController ||
               VisibleViewController is SyncViewController ||
               VisibleViewController is SyncMenuViewController)
                _viewShouldShowPlayerButton = false;
            else
                _viewShouldShowPlayerButton = true;

            if(VisibleViewController is PlayerViewController)
                _viewShouldShowEffectsButton = true;
            else
                _viewShouldShowEffectsButton = false;

            if(VisibleViewController is PlayerViewController)
                _viewShouldShowPlaylistButton = true;
            else
                _viewShouldShowPlaylistButton = false;
        }

        public void SetTitle(string title)
        {
            SetTitle(title, string.Empty);
        }       

        public void SetTitle(string title, string iconName)
        {
            var screenSize = UIKitHelper.GetDeviceSize();

            if (_lblTitle.Text != title)
            {
                _lblTitle.Alpha = 0;
                _imageViewIcon.Alpha = 0;
            }

            _lblTitle.Text = title;

            if (string.IsNullOrEmpty(iconName))
            {
                _lblTitle.Frame = new RectangleF(78, 0, screenSize.Width - 156, 44);
                _imageViewIcon.Image = null;
            }
            else
            {
                _imageViewIcon.Image = UIImage.FromBundle(string.Format("Images/Nav/{0}", iconName));
                SetTitleFrame();
            }

            UIView.Animate(0.2f, delegate() {
                _lblTitle.Alpha = 1;
                _imageViewIcon.Alpha = 1f;
            });
        }

        public void SetBackButtonVisible(bool visible)
        {
            _btnBack.Hidden = !visible;
        }

        private void SetTitleFrame()
        {
            var screenSize = UIKitHelper.GetDeviceSize();

            if (_imageViewIcon.Image == null)
            {
                _lblTitle.Frame = new RectangleF(78, 0, screenSize.Width - 156, 44);
            }
            else
            {
                UIGraphics.BeginImageContextWithOptions(View.Frame.Size, true, 0);
                var context = UIGraphics.GetCurrentContext();
                float width = CoreGraphicsHelper.MeasureStringWidth(context, _lblTitle.Text, _lblTitle.Font.Name, _lblTitle.Font.PointSize);
                UIGraphics.EndImageContext();

                float titleWidth = width > screenSize.Width - 156 - 24 ? screenSize.Width - 156 - 24 : width;
                _lblTitle.Frame = new RectangleF((screenSize.Width - titleWidth + 24) / 2, 0, titleWidth, 44);
                _imageViewIcon.Frame = new RectangleF(((screenSize.Width - titleWidth + 24) / 2) - 24, 14, 16, 16);
            }
        }
    }
}
