// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using org.sessionsapp.player;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Delegates;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Classes.Services;
using Sessions.iOS.Helpers;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Messages;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Services;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using Sessions.Sound.Objects;
using Sessions.Sound.Player;

namespace Sessions.iOS.Classes.Controllers
{
	public partial class PlayerViewController : BaseViewController, IPlayerView
	{
        private NowPlayingInfoService _nowPlayingInfoService;
        private ITinyMessengerHub _messageHub;
        private IDownloadImageService _downloadImageService;
        private SongInformationEntity _currentSongInfo;
        private NSTimer _timerHidePlayerMetadata;
        private bool _isPositionChanging = false;
        private string _currentAlbumArtKey = string.Empty;
        private PlayerMetadataViewController _playerMetadataViewController;
        private float _lastSliderPositionValue = 0;
        private UIImage _downloadedAlbumArtImage;
        private byte[] _downloadedAlbumArtData;
        private bool _isAppInactive;
        private long _currentPositionMS;
        private int _albumArtHeight;
        private MPVolumeView _volumeView;

		public PlayerViewController()
			: base (UserInterfaceIdiomIsPhone ? "PlayerViewController_iPhone" : "PlayerViewController_iPad", null)
		{
		}

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            if(imageViewAlbumArt.Image != null)
            {
                UIImage image = imageViewAlbumArt.Image;
                imageViewAlbumArt.Image = null;                
                image.Dispose();
                image = null;
            }
        }

		public override void ViewDidLoad()
        {
            _nowPlayingInfoService = Bootstrapper.GetContainer().Resolve<NowPlayingInfoService>();
            _messageHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _messageHub.Subscribe<AppInactiveMessage>(AppInactiveMessageReceived);
            _messageHub.Subscribe<AppActivatedMessage>(AppActivatedMessageReceived);

            _downloadImageService = new DownloadImageService();

			if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
				UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);

            viewAlbumArt.UserInteractionEnabled = true;
            viewAlbumArt.OnButtonClick += HandleOnAlbumArtButtonClick;
                
            View.BackgroundColor = GlobalTheme.BackgroundColor;
            btnPrevious.GlyphImageView.Image = UIImage.FromBundle("Images/Player/previous");
            btnPlayPause.GlyphImageView.Image = UIImage.FromBundle("Images/Player/pause");
            btnNext.GlyphImageView.Image = UIImage.FromBundle("Images/Player/next");
            btnShuffle.GlyphImageView.Image = UIImage.FromBundle("Images/Player/shuffle");
            btnRepeat.GlyphImageView.Image = UIImage.FromBundle("Images/Player/repeat");

            viewPosition.BackgroundColor = GlobalTheme.BackgroundColor;
            viewMain.BackgroundColor = GlobalTheme.BackgroundColor;
            viewPageControls.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;

            ConfigureScrollViews();
            ConfigurePositionSlider();

//            // Create text attributes for navigation bar button
//            var attr = new UITextAttributes();
//            attr.Font = UIFont.FromName("HelveticaNeue-Medium", 12);
//            attr.TextColor = UIColor.White;
//            attr.TextShadowColor = UIColor.DarkGray;
//            attr.TextShadowOffset = new UIOffset(0, 0);
            
//            // Set back button for navigation bar
//            _btnBack = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null, null);
//            _btnBack.SetTitleTextAttributes(attr, UIControlState.Normal);
//            this.NavigationItem.BackBarButtonItem = _btnBack;

            // Reset temporary text
            lblLength.Text = string.Empty;
            lblPosition.Text = string.Empty;

			// Create MPVolumeView (only visible on iPad or maybe in the future on iPhone with a scroll view)
			var rectVolume = new RectangleF(12, 10, 100, 46);
			_volumeView = new MPVolumeView(rectVolume);
			_volumeView.SetMinimumVolumeSliderImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
			_volumeView.SetMaximumVolumeSliderImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
			_volumeView.SetVolumeThumbImage(UIImage.FromBundle("Images/Sliders/thumbbig"), UIControlState.Normal);
			viewVolume.AddSubview(_volumeView);

			graphView.BackgroundColor = GlobalTheme.BackgroundColor;
			outputMeter.BackgroundColor = GlobalTheme.BackgroundColor;
			imageViewVolumeLow.Alpha = 0.125f;
			imageViewVolumeHigh.Alpha = 0.125f;
			viewEffects.OnScaleViewClicked += () => OnOpenEffects();

			if (UserInterfaceIdiomIsPhone)
			{
				viewVolume.RemoveFromSuperview();
				viewPlayerButtons.RemoveFromSuperview();
				viewEffects.RemoveFromSuperview();
				scrollViewPlayer.AddSubview(viewEffects);
				scrollViewPlayer.AddSubview(viewPlayerButtons);
				scrollViewPlayer.AddSubview(viewVolume);
				scrollViewPlayer.ContentSize = new SizeF(3 * UIScreen.MainScreen.Bounds.Width, 72);
                scrollViewPlayer.ContentOffset = new PointF(UIScreen.MainScreen.Bounds.Width, 0);
				viewEffects.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, 72);
				viewPlayerButtons.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width, 0, UIScreen.MainScreen.Bounds.Width, 72);
				viewVolume.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width * 2, 0, UIScreen.MainScreen.Bounds.Width, 72);

				imageViewVolumeLow.Image = UIImage.FromBundle("Images/Buttons/volume_low");
				imageViewVolumeHigh.Image = UIImage.FromBundle("Images/Buttons/volume_high");
			}
			else
			{
				imageViewVolumeLow.Image = UIImage.FromBundle("Images/SmallWhiteIcons/volume_low");
				imageViewVolumeHigh.Image = UIImage.FromBundle("Images/SmallWhiteIcons/volume_high");
			}

            imageViewAlbumArt.Alpha = 0;
            _albumArtHeight = (int)(imageViewAlbumArt.Bounds.Height * UIScreen.MainScreen.Scale);

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindPlayerView(MobileNavigationTabType.Playlists, this);

            base.ViewDidLoad();           
		}

        private void ConfigureScrollViews()
        {
            // Setup scroll view and page control
            scrollView.WeakDelegate = this;
            scrollView.PagingEnabled = true;
            scrollView.ShowsHorizontalScrollIndicator = false;
            scrollView.ShowsVerticalScrollIndicator = false;
            scrollView.DelaysContentTouches = false;
            
            var swipeDown = new UISwipeGestureRecognizer(HandleScrollViewSwipeDown);
            swipeDown.Direction = UISwipeGestureRecognizerDirection.Down;
            scrollView.AddGestureRecognizer(swipeDown);
            
            var swipeUp = new UISwipeGestureRecognizer(HandleScrollViewSwipeUp);
            swipeUp.Direction = UISwipeGestureRecognizerDirection.Up;
            scrollView.AddGestureRecognizer(swipeUp);

            if (scrollViewPlayer != null)
            {
                scrollViewPlayer.IndicatorStyle = UIScrollViewIndicatorStyle.White;
                scrollViewPlayer.PagingEnabled = true;
                scrollViewPlayer.DelaysContentTouches = false;
            }

            // Only display wave form on iPhone 5+ and iPad
            bool showWaveForm = DarwinHardwareHelper.Version != DarwinHardwareHelper.HardwareVersion.iPhone3GS &&
                                DarwinHardwareHelper.Version != DarwinHardwareHelper.HardwareVersion.iPhone4 &&
                                DarwinHardwareHelper.Version != DarwinHardwareHelper.HardwareVersion.iPhone4S;
            scrollViewWaveForm.Hidden = !showWaveForm;
            scrollViewWaveForm.UserInteractionEnabled = showWaveForm;
            scrollViewWaveForm.MultipleTouchEnabled = showWaveForm;
            scrollViewWaveForm.ZoomChanged += (sender, e) => {
                //sliderPosition.ScrubbingSpeedAdjustmentFactor = 1 / scrollViewWaveForm.Zoom;
            };
        }

        private void ConfigurePositionSlider()
        {
            sliderPosition.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            sliderPosition.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            sliderPosition.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);

            // Reduce the song position slider size for iPhone
            sliderPosition.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);

            // TODO: Block slider when the player is paused.
            sliderPosition.OnScrubbingSpeedChanged += (scrubbingSpeed) => {
                lblScrubbingType.Text = sliderPosition.CurrentScrubbingSpeed.Label;
            };
            sliderPosition.TouchesBeganEvent += (sender, e) => {
                UIView.Animate(0.2f, () => {
                    float offset = 42;
                    viewPosition.Frame = new RectangleF(viewPosition.Frame.X, viewPosition.Frame.Y, viewPosition.Frame.Width, viewPosition.Frame.Height + offset);
                    scrollViewWaveForm.Frame = new RectangleF(scrollViewWaveForm.Frame.X, scrollViewWaveForm.Frame.Y + offset, scrollViewWaveForm.Frame.Width, scrollViewWaveForm.Frame.Height * 2);
                    viewMain.Frame = new RectangleF(viewMain.Frame.X, viewPosition.Frame.Height + scrollViewWaveForm.Frame.Height, viewMain.Frame.Width, viewMain.Frame.Height);
                    lblSlideMessage.Alpha = 1;
                    lblScrubbingType.Alpha = 1;
                    scrollViewWaveForm.ShowSecondaryPosition(true);
                });
            };
            sliderPosition.TouchesMovedEvent += (sender, e) => {
                _isPositionChanging = true;
                _lastSliderPositionValue = sliderPosition.Value / 100;
                var entity = OnPlayerRequestPosition(sliderPosition.Value / 10000);
                lblPosition.Text = entity.Str;
                scrollViewWaveForm.SetSecondaryPosition(entity.Bytes);
            };
            sliderPosition.TouchesEndedEvent += (sender, e) => {
                UIView.Animate(0.2f, () => {
                    float offset = 42;
                    viewPosition.Frame = new RectangleF(viewPosition.Frame.X, viewPosition.Frame.Y, viewPosition.Frame.Width, viewPosition.Frame.Height - offset);
                    scrollViewWaveForm.Frame = new RectangleF(scrollViewWaveForm.Frame.X, scrollViewWaveForm.Frame.Y - offset, scrollViewWaveForm.Frame.Width, scrollViewWaveForm.Frame.Height / 2);
                    viewMain.Frame = new RectangleF(viewMain.Frame.X, viewPosition.Frame.Height + scrollViewWaveForm.Frame.Height, viewMain.Frame.Width, viewMain.Frame.Height);
                    lblSlideMessage.Alpha = 0;
                    lblScrubbingType.Alpha = 0;
                });

                // Sometimes the position from TouchesEnded is different than the last position returned by TouchesMoved.
                // This gives the user the impression that the selected position is different.
                OnPlayerSetPosition(_lastSliderPositionValue);
                scrollViewWaveForm.ShowSecondaryPosition(false);
                _isPositionChanging = false;
            };
        }
            
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            var navCtrl = (SessionsNavigationController)this.NavigationController;
			navCtrl.SetTitle("Now Playing", "Now Playing");
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
            OnPlayerViewAppeared();
//			if (scrollViewPlayer != null)
//				scrollViewPlayer.FlashScrollIndicators();
		}

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
			//Tracing.Log("PlayerVC - ViewDidLayoutSubviews - View.Width: {0} - scrollView.Width: {1}", View.Frame.Width, scrollView.Frame.Width);

			if (graphView != null)
				graphView.SetNeedsDisplay();

			if(UserInterfaceIdiomIsPhone)
                _volumeView.Frame = new RectangleF(48, 25, UIScreen.MainScreen.Bounds.Width - 96, 46);
			else
				_volumeView.Frame = new RectangleF(16 + 16 + 12, 32, viewVolume.Frame.Width - 88, 46);

            if (!UserInterfaceIdiomIsPhone)
            {
				// Resize scrollview subviews
				float width = View.Frame.Width;
				float height = viewMain.Frame.Height - 24; // 24 = Page Control
				if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait ||
				    UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown)
				{
					for (int a = 0; a < scrollView.Subviews.Count(); a++)
					{
						var view = scrollView.Subviews[a];
						if (a == 0)
							view.Frame = new RectangleF(0, 0, width, height);
						else if (a == 1)
							view.Frame = new RectangleF(width, 0, width, height / 2);
						else if (a == 2)
							view.Frame = new RectangleF(width, height / 2, width, height / 2);
						else if (a == 3)
							view.Frame = new RectangleF(2 * width, 0, width, height / 2);
						else if (a == 4)
							view.Frame = new RectangleF(2 * width, height / 2, width, height / 2);
					}
				}
				else
				{
					for (int a = 0; a < scrollView.Subviews.Count(); a++)
					{
						var view = scrollView.Subviews[a];
						if (a == 0)
							view.Frame = new RectangleF(0, 0, width, height);
						else if (a == 1)
							view.Frame = new RectangleF(width, 0, width / 2, height);
						else if (a == 2)
							view.Frame = new RectangleF(width + (width / 2), 0, width / 2, height);
						else if (a == 3)
							view.Frame = new RectangleF(2 * width, 0, width/ 2, height);
						else if (a == 4)
							view.Frame = new RectangleF((2 * width) + (width / 2), 0, width / 2, height);
					}
				}
				float oldWidth = scrollView.ContentSize.Width / 3f;
				float offset = scrollView.ContentOffset.X / oldWidth;
				scrollView.ContentSize = new SizeF(3 * width, height);
				scrollView.ContentOffset = new PointF(offset * width, 0);
            }

			//scrollViewWaveForm.RefreshWaveFormBitmap(View.Frame.Width);

            // IMPORTANT: Keep this property here to override the new Frame position by AutoLayout
            sliderPosition.TranslatesAutoresizingMaskIntoConstraints = true;

            // We need to keep a negative Y because of scaling issues (i.e. 70% of normal size)
            sliderPosition.Frame = new RectangleF(70, -8, View.Frame.Width - 140, 40); 
            
        }

        private void AppInactiveMessageReceived(AppInactiveMessage message)
        {
            // Cancel peak file loading/generation when the app is put to sleep
            _isAppInactive = true;
            scrollViewWaveForm.CancelPeakFile();
        }

        private void AppActivatedMessageReceived(AppActivatedMessage message)
        {
            InvokeOnMainThread(() => {
                _isAppInactive = false;

                // If the peak file loading/generation was interrupted, restart the process when the app is once again visible
                if (scrollViewWaveForm.IsEmpty && _currentSongInfo != null)
                {
                    Console.WriteLine("PlayerViewController - AppActivatedMessageReceived - Loading peak file...");
                    scrollViewWaveForm.LoadPeakFile(_currentSongInfo.AudioFile);
                }
            });
        }

        private void HandleScrollViewSwipeDown(UISwipeGestureRecognizer gestureRecognizer)
        {
            if(pageControl.CurrentPage == 0)
                ShowPlayerMetadata(false, false);
        }

        private void HandleScrollViewSwipeUp(UISwipeGestureRecognizer gestureRecognizer)
        {
            if(pageControl.CurrentPage == 0)
                ShowPlayerMetadata(true, true);
        }

        public void AddScrollView(UIViewController viewController)
        {
            // No other choice than to add vc to a list of vc to notify them when child views become visible.
            // i.e. make sure the player metadata panel is visible when it is scrolled into the scrollview.
            // or use tiny messenger... but that would suck sending hundreds of messages if you want smooth 
            // actually add the timer here... so no need to calculate when the player metadata view is visible! also easier to hide the page control.

            if (viewController is PlayerMetadataViewController)
                _playerMetadataViewController = (PlayerMetadataViewController)viewController;

            // Hot fix for iOS 7 Beta 3
            int scrollSubviewsLength = 0;
            if (scrollView.Subviews != null)
                scrollSubviewsLength = scrollView.Subviews.Length;

            if (UserInterfaceIdiomIsPhone)
            {
//                viewController.View.Frame = new RectangleF(scrollSubviewsLength * scrollView.Frame.Width, 0, scrollView.Frame.Width, scrollView.Frame.Height);
//                scrollView.AddSubview(viewController.View);
//                pageControl.Pages = scrollSubviewsLength + 1;
//                scrollView.ContentSize = new SizeF((scrollSubviewsLength + 1) * scrollView.Frame.Width, scrollView.Frame.Height);

                //float scrollViewWidth = UIScreen.MainScreen.Bounds.Width;
                float scrollViewWidth = View.Frame.Width;
                float scrollViewWidth2 = UIScreen.MainScreen.Bounds.Width;
                //viewController.View.Frame = new RectangleF(scrollSubviewsLength * scrollViewWidth, 0, scrollViewWidth, scrollView.Frame.Height);
                viewController.View.Frame = new RectangleF(scrollSubviewsLength * scrollViewWidth2, 0, scrollViewWidth, scrollView.Frame.Height);
                scrollView.AddSubview(viewController.View);
                pageControl.Pages = scrollSubviewsLength + 1;
                scrollView.ContentSize = new SizeF((scrollSubviewsLength + 1) * scrollViewWidth, scrollView.Frame.Height);

                Console.WriteLine("---------->> Scrollview.Frame.Width: {0} -- scrollViewWidth: {1} -- scrollView.ContentSize: {2} -- View.Frame.Width: {3} -- View.Bounds.Width: {4} -- UIScreen.Bounds.Width: {5}", scrollView.Frame.Width, scrollViewWidth, scrollView.ContentSize, View.Frame.Width, View.Bounds.Width, UIScreen.MainScreen.Bounds.Width);
            }
            else
            {
                if (viewController is PlayerMetadataViewController)
                    viewController.View.Frame = new RectangleF(0, 0, scrollView.Frame.Width, scrollView.Frame.Height);
                else if(viewController is MarkersViewController)
                    viewController.View.Frame = new RectangleF(scrollView.Frame.Width, 0, scrollView.Frame.Width, scrollView.Frame.Height / 2);
                else if(viewController is LoopsViewController)
                    viewController.View.Frame = new RectangleF(scrollView.Frame.Width, scrollView.Frame.Height / 2, scrollView.Frame.Width, scrollView.Frame.Height / 2);
                else if(viewController is TimeShiftingViewController)
                    viewController.View.Frame = new RectangleF(2 * scrollView.Frame.Width, 0, scrollView.Frame.Width, scrollView.Frame.Height / 2);
                else if(viewController is PitchShiftingViewController)
                    viewController.View.Frame = new RectangleF(2 * scrollView.Frame.Width, scrollView.Frame.Height / 2, scrollView.Frame.Width, scrollView.Frame.Height / 2);

                scrollView.AddSubview(viewController.View);
                scrollView.ContentSize = new SizeF(3 * scrollView.Frame.Width, scrollView.Frame.Height);
            }
        }

        [Export("scrollViewDidScroll:")]
        public void ScrollViewDidScroll(UIScrollView scrollview)
        {
            float pageWidth = scrollView.Frame.Width;
            int page = (int)Math.Floor((scrollView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;
            pageControl.CurrentPage = page;

            float alpha = 1 - Math.Min(scrollView.ContentOffset.X, scrollView.Frame.Width) / scrollview.Frame.Width;
            viewAlbumArt.Alpha = alpha;

            if(_timerHidePlayerMetadata != null)
            {
                Console.WriteLine("PlayerMetadataVC - ScrollViewDidScroll - Invalidating timer...");
                _timerHidePlayerMetadata.Invalidate();
                _timerHidePlayerMetadata = null;
            }

            if(pageControl.Alpha == 0)
                ShowPlayerMetadata(true, false);
        }

        [Export("scrollViewDidEndDecelerating:")]
        public void ScrollViewDidEndDecelerating(UIScrollView scrollView)
        {
            CreateHidePlayerMetadataTimer();
        }

        private void HandleOnAlbumArtButtonClick()
        {
            // Show a list of templates for the marker name
            var actionSheet = new UIActionSheet("Do you wish to apply this album art to:", null, "Cancel", null, new string[3] { "All songs from this album", "This song only", "Choose another image..." });
            actionSheet.Style = UIActionSheetStyle.BlackTranslucent;
            actionSheet.Clicked += (eventSender, e) => {
                switch(e.ButtonIndex)
                {
                    case 0:
                        OnApplyAlbumArtToAlbum(_downloadedAlbumArtData);
                        break;
                    case 1:
                        OnApplyAlbumArtToSong(_downloadedAlbumArtData);
                        break;
                    case 2:
                        OnOpenSelectAlbumArt();
                        break;
                }
            };

            // Must use the tab bar controller to spawn the action sheet correctly. Remember, we're in a UIScrollView...
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            actionSheet.ShowFromTabBar(appDelegate.MainViewController.TabBarController.TabBar);
        }

        private void ShowPlayerMetadata(bool show, bool swipeUp)
        {
			//Console.WriteLine("PlayerVC - ShowPlayerMetadata: {0}", show);

            if (_playerMetadataViewController != null)
                _playerMetadataViewController.ShowPanel(show, swipeUp);

            if(show && !swipeUp)
            {
                //viewPageControls.Frame = new RectangleF(viewPageControls.Frame.X, scrollView.Frame.Y + scrollView.Frame.Height - viewPageControls.Frame.Height, viewPageControls.Frame.Width, viewPageControls.Frame.Height);
                //viewPageControls.Frame = new RectangleF(viewPageControls.Frame.X, scrollView.Frame.Y + scrollView.Frame.Height, viewPageControls.Frame.Width, viewPageControls.Frame.Height);
                UIView.Animate(0.2f, () => {
                    viewPageControls.Alpha = 1;
                    pageControl.Alpha = 1;
                });
            }
            else if(show && swipeUp)
            {
                UIView.Animate(0.4f, () => {
                    viewPageControls.Alpha = 1;
                    pageControl.Alpha = 1;
                });
            }
            else
            {
                UIView.Animate(0.4f, () => {
                    //viewPageControls.Frame = new RectangleF(viewPageControls.Frame.X, scrollView.Frame.Y + scrollView.Frame.Height, viewPageControls.Frame.Width, viewPageControls.Frame.Height);
                    //viewPageControls.Frame = new RectangleF(viewPageControls.Frame.X, scrollView.Frame.Y + scrollView.Frame.Height + viewPageControls.Frame.Height, viewPageControls.Frame.Width, viewPageControls.Frame.Height);
                    viewPageControls.Alpha = 0;
                    pageControl.Alpha = 0;
                });
            }
        }

        private void CreateHidePlayerMetadataTimer()
        {
//            if(_timerHidePlayerMetadata != null)
//                return;
//
//            Console.WriteLine("PlayerVC - CreateHidePlayerMetadataTimer - Setting up timer...");
//            _timerHidePlayerMetadata = NSTimer.CreateRepeatingScheduledTimer(30, () => {
//                Console.WriteLine("PlayerVC - CreateHidePlayerMetadataTimer - Timer elasped! Hiding view...");
//
//                if(pageControl.CurrentPage == 0)
//                    ShowPlayerMetadata(false);
//            });
        }

        partial void actionPause(NSObject sender)
        {
            OnPlayerPause();
        }

        partial void actionPrevious(NSObject sender)
        {
            OnPlayerPrevious();
        }

        partial void actionNext(NSObject sender)
        {
            OnPlayerNext();
        }

        partial void actionRepeat(NSObject sender)
        {
            OnPlayerRepeat();
        }

        partial void actionShuffle(NSObject sender)
        {
            OnPlayerShuffle();
        }

        #region IPlayerView implementation

		// TODO: Remove output meter for iPhone 4/4S?
		public bool IsOutputMeterEnabled
		{ 
			get 
			{ 
				if (DarwinHardwareHelper.Version == DarwinHardwareHelper.HardwareVersion.iPhone3GS ||
				   DarwinHardwareHelper.Version == DarwinHardwareHelper.HardwareVersion.iPhone4 ||
				   DarwinHardwareHelper.Version == DarwinHardwareHelper.HardwareVersion.iPhone4S)
				{
					return false;
				}
				return true; 
			} 
		}

        public Action OnPlayerPlay { get; set; }
        public Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        public Action OnPlayerPause { get; set; }
        public Action OnPlayerStop { get; set; }
        public Action OnPlayerPrevious { get; set; }
        public Action OnPlayerNext { get; set; }
        public Action<float> OnPlayerSetVolume { get; set; }
        public Action<float> OnPlayerSetPitchShifting { get; set; }
        public Action<float> OnPlayerSetTimeShifting { get; set; }
        public Action<float> OnPlayerSetPosition { get; set; }
        public Action OnPlayerShuffle { get; set; }
        public Action OnPlayerRepeat { get; set; }
        public Action OnEditSongMetadata { get; set; }
        public Action OnOpenPlaylist { get; set; }
		public Action OnOpenEffects { get; set; }
        public Action OnOpenSelectAlbumArt { get; set; }
        public Action OnPlayerViewAppeared { get; set; }
        public Func<float, SSPPosition> OnPlayerRequestPosition { get; set; }
        public Action<byte[]> OnApplyAlbumArtToSong { get; set; }
        public Action<byte[]> OnApplyAlbumArtToAlbum { get; set; }

        public void PlayerError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alert = new UIAlertView("An error occured in Player", ex.Message, null, "OK", null);
                alert.Show();
            });
        }

        public void PushSubView(IBaseView view)
        {
            AddScrollView((UIViewController)view);
        }

        public void RefreshPlayerPosition(SSPPosition position)
        {
            InvokeOnMainThread(() => {
                _currentPositionMS = position.MS;
                if(!_isPositionChanging)
                {
                    lblPosition.Text = position.Str;
                    sliderPosition.SetPosition(((float)position.Bytes / (float)_currentSongInfo.AudioFile.LengthBytes) * 10000f);
                }

                scrollViewWaveForm.SetPosition(position.Bytes);
            });
        }

        public void RefreshMarkers(IEnumerable<Marker> markers)
        {
            InvokeOnMainThread(() => {
                scrollViewWaveForm.SetMarkers(markers);
            });
        }

		public void RefreshMarkerPosition(Marker marker)
		{
			//Tracing.Log("PlayerViewController - RefreshMarkerPosition - position: {0}", marker.Position);
			InvokeOnMainThread(() => scrollViewWaveForm.SetMarkerPosition(marker));
		}

		public void RefreshActiveMarker(Guid markerId)
		{
			InvokeOnMainThread(() => scrollViewWaveForm.SetActiveMarker(markerId));
		}

        public void RefreshLoops(IEnumerable<SSPLoop> loops)
        {
        }

        public void RefreshPlaylist(Playlist playlist)
        {
        }

        public void RefreshSongInformation(SongInformationEntity entity)
        {
            if (entity == null || entity.AudioFile == null)
                    return;

            InvokeOnMainThread(() =>
            {
                // Prevent refreshing song twice
                if (_currentSongInfo != null && _currentSongInfo.AudioFile.Id == entity.AudioFile.Id)
                    return;

                _currentSongInfo = entity;

                try
                {
                    //_currentNavigationSubtitle = (playlistIndex+1).ToString() + " of " + playlistCount.ToString();
                    //SessionsNavigationController navCtrl = (SessionsNavigationController)this.NavigationController;
                    //navCtrl.SetTitle("Now Playing", _currentNavigationSubtitle);

                    ShowPlayerMetadata(true, false);
                    lblLength.Text = entity.AudioFile.Length;

                    if (IsOutputMeterEnabled)
                    {
                        // The wave form scroll view isn't aware of floating point
                        long lengthWaveForm = entity.AudioFile.LengthBytes;
                        if(entity.UseFloatingPoint)
                            lengthWaveForm /= 2;

                        scrollViewWaveForm.SetWaveFormLength(lengthWaveForm);

                        // Do not load/generate peak files when the app is sleeping
                        if(!_isAppInactive)
                            scrollViewWaveForm.LoadPeakFile(entity.AudioFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to set wave form information: {0}", ex);
                }
            });

            LoadAlbumArt(entity.AudioFile);
        }

        public async void LoadAlbumArt(AudioFile audioFile)
        {
            // Check if the album art needs to be refreshed
            string key = audioFile.ArtistName.ToUpper() + "_" + audioFile.AlbumTitle.ToUpper();
            if (_currentAlbumArtKey == key)
            {
                Console.WriteLine("PlayerViewController - RefreshSongInformation - The current album key matches ({0}); keeping the same album art.", key);
                return;
            }

            InvokeOnMainThread(() =>
            {
                try
                {
                    _nowPlayingInfoService.AlbumArtImage = null;
                    _nowPlayingInfoService.UpdateInfo();

                    UIView.Animate(0.3, () =>
                    {
                        imageViewAlbumArt.Alpha = 0;
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to set image view album art alpha: {0}", ex);
                }
            });

            // Load album art + resize in another thread
            var task = Task<UIImage>.Factory.StartNew(() =>
            {
                try
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Extracting album art from audio file...");
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);                        
                    using (NSData imageData = NSData.FromArray(bytesImage))
                    {
                        using (UIImage imageFullSize = UIImage.LoadFromData(imageData))
                        {
                            if (imageFullSize != null)
                            {
                                try
                                {
                                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Scaling album art to {0} (key={1})...", _albumArtHeight, key);
                                    _currentAlbumArtKey = key;
                                    UIImage imageResized = CoreGraphicsHelper.ScaleImage(imageFullSize, _albumArtHeight);
                                    return imageResized;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error resizing image " + audioFile.ArtistName + " - " + audioFile.AlbumTitle + ": " + ex.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to process image: {0}", ex);
                }
                
                return null;
            });

            UIImage image = await task;                    
            InvokeOnMainThread(() => {
                try
                {
                    if (image == null)
                    {
                        Console.WriteLine("PlayerViewController - RefreshSongInformation - Downloading image from the internet...");
                        DownloadImage(audioFile);
                    }
                    else
                    {
                        Console.WriteLine("PlayerViewController - RefreshSongInformation - Assigning album art from file...");
                        imageViewAlbumArt.Image = image;
                        UIView.Animate(0.2, () => {
                            imageViewAlbumArt.Alpha = 1;
                        });                            

                        _nowPlayingInfoService.AlbumArtImage = image;
                        _nowPlayingInfoService.UpdateInfo();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to set image after processing: {0}", ex);
                }
            });
        }

        public async void DownloadImage(AudioFile audioFile)
        {
            string key = audioFile.ArtistName.ToUpper() + "_" + audioFile.AlbumTitle.ToUpper();
            if (_currentAlbumArtKey == key)
                return;

            viewAlbumArt.ShowDownloadingView();
            InvokeOnMainThread(() =>  {
                UIView.Animate(0.2, () =>
                {
                    imageViewAlbumArt.Alpha = 0;
                });
            });

            var task = _downloadImageService.DownloadAlbumArt(audioFile);
            task.Start();
            var result = await task;
            if (result == null)
            {
                Console.WriteLine("AlbumArtView - Error downloading image!");
                viewAlbumArt.ShowDownloadErrorView();
            }
            else
            {
                _downloadedAlbumArtData = result.ImageData;
                _downloadedAlbumArtImage = await ResizeImage(result, key);
                if (_downloadedAlbumArtImage == null)
                {
                    Console.WriteLine("AlbumArtView - Error resizing image!");
                    viewAlbumArt.ShowDownloadErrorView();
                }
                else
                {                    
                    Console.WriteLine("AlbumArtView - Setting album art...");
                    InvokeOnMainThread(() =>  {
                        imageViewAlbumArt.Alpha = 0;
                        imageViewAlbumArt.Image = _downloadedAlbumArtImage;

                        _nowPlayingInfoService.AlbumArtImage = _downloadedAlbumArtImage;
                        _nowPlayingInfoService.UpdateInfo();

                        viewAlbumArt.ShowDownloadedView(() => {
                            UIView.Animate(0.2, () => {
                                imageViewAlbumArt.Alpha = 1;
                            });                            
                        });                            
                    });                        
                }
            }
        }

        private Task<UIImage> ResizeImage(DownloadImageService.DownloadImageResult result, string key)
        {
            // Load album art + resize in another thread
            Console.WriteLine("AlbumArtView - Downloaded image successfully!");
            var task = Task<UIImage>.Factory.StartNew(() =>
            {
                try
                {
                    using (NSData imageData = NSData.FromArray(result.ImageData))
                    {
                        using (UIImage imageFullSize = UIImage.LoadFromData(imageData))
                        {
                            if (imageFullSize != null)
                            {
                                try
                                {
                                    UIImage imageResized = null;
                                    Console.WriteLine("AlbumArtView - Resizing image...");
                                    InvokeOnMainThread(() =>
                                    {
                                        _currentAlbumArtKey = key;                                    
                                        imageResized = CoreGraphicsHelper.ScaleImage(imageFullSize, (int)imageViewAlbumArt.Frame.Height);
                                    });
                                    return imageResized;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error resizing image: {0}", ex);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AlbumArtView - DownloadImage - Failed to process image: {0}", ex);
                }

                return null;
            });
            return task;
        }

        public void RefreshPlayerVolume(PlayerVolume entity)
        {
            // Not necessary on iOS. The volume is controlled by the MPVolumeView.
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShifting entity)
        {
        }

		public void RefreshPlayerState(SSPPlayerState state, SSPRepeatType repeatType, bool isShuffleEnabled)
        {
            InvokeOnMainThread(() => {
                switch (state)
                {
                    case SSPPlayerState.Paused:
                        btnPlayPause.GlyphImageView.Image = UIImage.FromBundle("Images/Player/play");
                        break;
                    case SSPPlayerState.Playing:
                        btnPlayPause.GlyphImageView.Image = UIImage.FromBundle("Images/Player/pause");

                        // Force update position or iOS will reset position to 0
                        _nowPlayingInfoService.PositionMS = _currentPositionMS;
                        _nowPlayingInfoService.UpdateInfo();
                        break;
                }
            });
        }

		public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
		{
			InvokeOnMainThread(() => {
				if(outputMeter != null)
				{
					outputMeter.AddWaveDataBlock(dataLeft, dataRight);
					outputMeter.SetNeedsDisplay();
				}
			});
		}

        #endregion
	}
}

