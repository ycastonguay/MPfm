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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using MPfm.Core;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Player;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;

namespace MPfm.iOS.Classes.Controllers
{
	public partial class PlayerViewController : BaseViewController, IPlayerView
	{
        NSTimer _timerHidePlayerMetadata;
        bool _isPositionChanging = false;
        string _currentAlbumArtKey = string.Empty;
        //string _currentNavigationSubtitle = string.Empty;
        PlayerMetadataViewController _playerMetadataViewController;
        float _lastSliderPositionValue = 0;

		public PlayerViewController()
			: base (UserInterfaceIdiomIsPhone ? "PlayerViewController_iPhone" : "PlayerViewController_iPad", null)
		{
		}

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            scrollViewWaveForm.WaveFormView.FlushCache();
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
            View.BackgroundColor = GlobalTheme.BackgroundColor;
            btnPrevious.GlyphImageView.Image = UIImage.FromBundle("Images/Player/previous");
            btnPlayPause.GlyphImageView.Image = UIImage.FromBundle("Images/Player/pause");
            btnNext.GlyphImageView.Image = UIImage.FromBundle("Images/Player/next");
            btnShuffle.GlyphImageView.Image = UIImage.FromBundle("Images/Player/shuffle");
            btnRepeat.GlyphImageView.Image = UIImage.FromBundle("Images/Player/repeat");

            viewPosition.BackgroundColor = GlobalTheme.BackgroundColor;
            viewMain.BackgroundColor = GlobalTheme.BackgroundColor;
            viewPageControls.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;

            sliderPosition.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            sliderPosition.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            sliderPosition.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);

            // Reduce the song position slider size for iPhone
            sliderPosition.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
//            if (UserInterfaceIdiomIsPhone)
//            {
//                sliderPosition.Frame = new RectangleF(70, sliderPosition.Frame.Y - 10, UIScreen.MainScreen.Bounds.Width - 140, sliderPosition.Frame.Height);
//            }

            // Setup scroll view and page control
            scrollView.WeakDelegate = this;
            scrollView.PagingEnabled = true;
            scrollView.ShowsHorizontalScrollIndicator = false;
            scrollView.ShowsVerticalScrollIndicator = false;
            scrollView.DelaysContentTouches = false;
            UISwipeGestureRecognizer swipeDown = new UISwipeGestureRecognizer(HandleScrollViewSwipeDown);
            swipeDown.Direction = UISwipeGestureRecognizerDirection.Down;
            scrollView.AddGestureRecognizer(swipeDown);
            UISwipeGestureRecognizer swipeUp = new UISwipeGestureRecognizer(HandleScrollViewSwipeUp);
            swipeUp.Direction = UISwipeGestureRecognizerDirection.Up;
            scrollView.AddGestureRecognizer(swipeUp);

            // TODO: Block slider when the player is paused.
            sliderPosition.ScrubbingTypeChanged += (sender, e) => {
                string scrubbingType = "High-speed scrubbing";
                switch(sliderPosition.ScrubbingType)
                {
                    case SliderScrubbingType.Fine:
                        scrubbingType = "Fine scrubbing";
                        break;
                    case SliderScrubbingType.QuarterSpeed:
                        scrubbingType = "Quarter-speed scrubbing";
                        break;
                    case SliderScrubbingType.HalfSpeed:
                        scrubbingType = "Half-speed scrubbing";
                        break;
                    case SliderScrubbingType.HighSpeed:
                        scrubbingType = "High-speed scrubbing";
                        break;
                }
                lblScrubbingType.Text = scrubbingType;
            };
            sliderPosition.TouchesBeganEvent += (sender, e) => {
                UIView.Animate(0.2f, () => {
                    float offset = 42;
                    viewPosition.Frame = new RectangleF(viewPosition.Frame.X, viewPosition.Frame.Y, viewPosition.Frame.Width, viewPosition.Frame.Height + offset);
                    scrollViewWaveForm.Frame = new RectangleF(scrollViewWaveForm.Frame.X, scrollViewWaveForm.Frame.Y + offset, scrollViewWaveForm.Frame.Width, scrollViewWaveForm.Frame.Height * 2);
                    scrollViewWaveForm.WaveFormView.Frame = new RectangleF(scrollViewWaveForm.WaveFormView.Frame.X, scrollViewWaveForm.WaveFormView.Frame.Y, scrollViewWaveForm.WaveFormView.Frame.Width, (scrollViewWaveForm.WaveFormView.Frame.Height * 2) + 22);
                    viewMain.Frame = new RectangleF(viewMain.Frame.X, viewPosition.Frame.Height + scrollViewWaveForm.Frame.Height, viewMain.Frame.Width, viewMain.Frame.Height);
                    lblSlideMessage.Alpha = 1;
                    lblScrubbingType.Alpha = 1;
                    scrollViewWaveForm.ShowSecondaryPosition(true);
                });
            };
            sliderPosition.TouchesMovedEvent += (sender, e) => {
                _isPositionChanging = true;
                _lastSliderPositionValue = sliderPosition.Value / 100;
                PlayerPositionEntity entity = OnPlayerRequestPosition(sliderPosition.Value / 10000);
                lblPosition.Text = entity.Position;
                scrollViewWaveForm.SetSecondaryPosition(entity.PositionBytes);
            };
            sliderPosition.TouchesEndedEvent += (sender, e) => {
                UIView.Animate(0.2f, () => {
                    float offset = 42;
                    viewPosition.Frame = new RectangleF(viewPosition.Frame.X, viewPosition.Frame.Y, viewPosition.Frame.Width, viewPosition.Frame.Height - offset);
                    scrollViewWaveForm.Frame = new RectangleF(scrollViewWaveForm.Frame.X, scrollViewWaveForm.Frame.Y - offset, scrollViewWaveForm.Frame.Width, scrollViewWaveForm.Frame.Height / 2);
                    scrollViewWaveForm.WaveFormView.Frame = new RectangleF(scrollViewWaveForm.WaveFormView.Frame.X, scrollViewWaveForm.WaveFormView.Frame.Y, scrollViewWaveForm.WaveFormView.Frame.Width, (scrollViewWaveForm.WaveFormView.Frame.Height - 22) / 2);
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

            // Only display wave form on iPhone 5+ and iPad
            if (DarwinHardwareHelper.Version == DarwinHardwareHelper.HardwareVersion.iPhone3GS ||
                DarwinHardwareHelper.Version == DarwinHardwareHelper.HardwareVersion.iPhone4 ||
                DarwinHardwareHelper.Version == DarwinHardwareHelper.HardwareVersion.iPhone4S)
            {
                scrollViewWaveForm.Hidden = true;
            }

            // Create text attributes for navigation bar button
            UITextAttributes attr = new UITextAttributes();
            attr.Font = UIFont.FromName("HelveticaNeue-Medium", 12);
            attr.TextColor = UIColor.White;
            attr.TextShadowColor = UIColor.DarkGray;
            attr.TextShadowOffset = new UIOffset(0, 0);
            
//            // Set back button for navigation bar
//            _btnBack = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null, null);
//            _btnBack.SetTitleTextAttributes(attr, UIControlState.Normal);
//            this.NavigationItem.BackBarButtonItem = _btnBack;

            // Reset temporary text
            lblLength.Text = string.Empty;
            lblPosition.Text = string.Empty;

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindPlayerView(MobileNavigationTabType.Playlists, this);

            base.ViewDidLoad();           
		}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            //MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            //navCtrl.SetTitle("Now Playing", _currentNavigationSubtitle);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (!UserInterfaceIdiomIsPhone)
            {
                // Resize scrollview subviews.
                for (int a = 0; a < scrollView.Subviews.Count(); a++)
                {
                    var view = scrollView.Subviews[a];

                    if (a == 0)
                        view.Frame = new RectangleF(0, 0, scrollView.Frame.Width, scrollView.Frame.Height);
                    else if (a == 1)
                        view.Frame = new RectangleF(scrollView.Frame.Width, 0, scrollView.Frame.Width, scrollView.Frame.Height / 2);
                    else if (a == 2)
                        view.Frame = new RectangleF(scrollView.Frame.Width, scrollView.Frame.Height / 2, scrollView.Frame.Width, scrollView.Frame.Height / 2);
                    else if (a == 3)
                        view.Frame = new RectangleF(2 * scrollView.Frame.Width, 0, scrollView.Frame.Width, scrollView.Frame.Height / 2);
                    else if (a == 4)
                        view.Frame = new RectangleF(2 * scrollView.Frame.Width, scrollView.Frame.Height / 2, scrollView.Frame.Width, scrollView.Frame.Height / 2);
                }

                scrollView.ContentSize = new SizeF(3 * scrollView.Frame.Width, scrollView.Frame.Height);
            }

            sliderPosition.Frame = new RectangleF(70, sliderPosition.Frame.Y, View.Frame.Width - 140, sliderPosition.Frame.Height);
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
                viewController.View.Frame = new RectangleF(scrollSubviewsLength * scrollView.Frame.Width, 0, scrollView.Frame.Width, scrollView.Frame.Height);
                scrollView.AddSubview(viewController.View);
                pageControl.Pages = scrollSubviewsLength + 1;
                //pageControl.Pages = 5;
                scrollView.ContentSize = new SizeF((scrollSubviewsLength + 1) * scrollView.Frame.Width, scrollView.Frame.Height);
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
                //pageControl.Pages = 3;
                scrollView.ContentSize = new SizeF(3 * scrollView.Frame.Width, scrollView.Frame.Height);
            }
        }

        [Export("scrollViewDidScroll:")]
        public void ScrollViewDidScroll(UIScrollView scrollview)
        {
            float pageWidth = scrollView.Frame.Size.Width;
            int page = (int)Math.Floor((scrollView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;
            pageControl.CurrentPage = page;

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

        private void ShowPlayerMetadata(bool show, bool swipeUp)
        {
            Console.WriteLine("PlayerVC - ShowPlayerMetadata: {0}", show);

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

        }

        partial void actionShuffle(NSObject sender)
        {

        }

        #region IPlayerView implementation

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
        public Func<float, PlayerPositionEntity> OnPlayerRequestPosition { get; set; }

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

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            InvokeOnMainThread(() => {
                if(!_isPositionChanging)
                {
                    lblPosition.Text = entity.Position;
                    sliderPosition.SetPosition(entity.PositionPercentage * 100);
                }

                scrollViewWaveForm.SetPosition(entity.PositionBytes);
            });
        }

        public void RefreshMarkers(IEnumerable<Marker> markers)
        {
            InvokeOnMainThread(() => {
                scrollViewWaveForm.SetMarkers(markers);
            });
        }

        public void RefreshLoops(IEnumerable<Loop> loops)
        {
        }

        public async void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            if(audioFile == null)
                return;

            // Check if the album art needs to be refreshed
            string key = audioFile.ArtistName.ToUpper() + "_" + audioFile.AlbumTitle.ToUpper();
            if(_currentAlbumArtKey != key)
            {
                int height = 44;
                InvokeOnMainThread(() => {
                    try
                    {
                        height = (int)(imageViewAlbumArt.Bounds.Height * UIScreen.MainScreen.Scale);
                        UIView.Animate(0.3, () => {
                            imageViewAlbumArt.Alpha = 0;
                        });
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to set image view album art alpha: {0}", ex);
                    }
                });

                // Load album art + resize in another thread
                var task = Task<UIImage>.Factory.StartNew(() => {
                    try
                    {
                        byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);                        
                        using (NSData imageData = NSData.FromArray(bytesImage))
                        {
                            using (UIImage imageFullSize = UIImage.LoadFromData(imageData))
                            {
                                if (imageFullSize != null)
                                {
                                    try
                                    {
                                        _currentAlbumArtKey = key;                                    
                                        UIImage imageResized = CoreGraphicsHelper.ScaleImage(imageFullSize, height);
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
                //}).ContinueWith(t => {
                UIImage image = await task;
                    if(image == null)
                        return;
                    
                    InvokeOnMainThread(() => {
                        try
                        {
                            imageViewAlbumArt.Alpha = 0;
                            imageViewAlbumArt.Image = image;              

                            UIView.Animate(0.3, () => {
                                imageViewAlbumArt.Alpha = 1;
                            });
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to set image after processing: {0}", ex);
                        }
                    });
                //}, TaskScheduler.FromCurrentSynchronizationContext());
            }

            // Refresh other fields
            InvokeOnMainThread(() => {
                try
                {
                    //_currentNavigationSubtitle = (playlistIndex+1).ToString() + " of " + playlistCount.ToString();
                    //MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
                    //navCtrl.SetTitle("Now Playing", _currentNavigationSubtitle);

                    ShowPlayerMetadata(true, false);
                    lblLength.Text = audioFile.Length;
                    scrollViewWaveForm.SetWaveFormLength(lengthBytes);
                    scrollViewWaveForm.LoadPeakFile(audioFile);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to set wave form information: {0}", ex);
                }
            });
        }

        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
        {
            // Not necessary on iOS. The volume is controlled by the MPVolumeView.
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {
        }

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
            InvokeOnMainThread(() => {
                switch (status)
                {
                    case PlayerStatusType.Paused:
                        btnPlayPause.GlyphImageView.Image = UIImage.FromBundle("Images/Player/play");
                        break;
                    case PlayerStatusType.Playing:
                        btnPlayPause.GlyphImageView.Image = UIImage.FromBundle("Images/Player/pause");
                        break;
                }
            });
        }

        #endregion
	}
}

