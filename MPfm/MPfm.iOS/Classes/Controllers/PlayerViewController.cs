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
using System.Timers;
using MPfm.Core;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Player;
using MPfm.Sound.AudioFiles;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Helpers;
using MPfm.iOS.Classes.Objects;
using System.Threading.Tasks;
using MPfm.Player.Objects;

namespace MPfm.iOS.Classes.Controllers
{
	public partial class PlayerViewController : BaseViewController, IPlayerView
	{
        private bool _isPositionChanging = false;
        private string _currentAlbumArtKey = string.Empty;
        private string _currentNavigationSubtitle = string.Empty;
        private MPVolumeView _volumeView;
        private UIBarButtonItem _btnBack;

		public PlayerViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "PlayerViewController_iPhone" : "PlayerViewController_iPad", null)
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
            btnPrevious.BackgroundColor = GlobalTheme.BackgroundColor;
            btnPlayPause.BackgroundColor = GlobalTheme.BackgroundColor;
            btnNext.BackgroundColor = GlobalTheme.BackgroundColor;
            btnPrevious.SetImage(UIImage.FromBundle("Images/Buttons/previous"), UIControlState.Normal);
            btnPrevious.SetImage(UIImage.FromBundle("Images/Buttons/previous_on"), UIControlState.Highlighted);
            btnPlayPause.SetImage(UIImage.FromBundle("Images/Buttons/pause"), UIControlState.Normal);
            btnPlayPause.SetImage(UIImage.FromBundle("Images/Buttons/pause_on"), UIControlState.Highlighted);
            btnNext.SetImage(UIImage.FromBundle("Images/Buttons/next"), UIControlState.Normal);
            btnNext.SetImage(UIImage.FromBundle("Images/Buttons/next_on"), UIControlState.Highlighted);

            viewPosition.BackgroundColor = GlobalTheme.BackgroundColor;
            viewMain.BackgroundColor = GlobalTheme.BackgroundColor;
            viewPageControls.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;

            sliderPosition.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            sliderPosition.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            sliderPosition.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);

            // Reduce the song position slider size
            sliderPosition.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
            sliderPosition.Frame = new RectangleF(70, sliderPosition.Frame.Y - 3, UIScreen.MainScreen.Bounds.Width - 140, sliderPosition.Frame.Height);

            // Setup scroll view and page control
            scrollView.WeakDelegate = this;
            scrollView.PagingEnabled = true;
            scrollView.ShowsHorizontalScrollIndicator = false;
            scrollView.ShowsVerticalScrollIndicator = false;
            scrollView.DelaysContentTouches = false;
            pageControl.CurrentPage = 0;

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
                    scrollViewWaveForm.WaveFormView.Frame = new RectangleF(scrollViewWaveForm.WaveFormView.Frame.X, scrollViewWaveForm.WaveFormView.Frame.Y, scrollViewWaveForm.WaveFormView.Frame.Width, scrollViewWaveForm.WaveFormView.Frame.Height * 2);
                    viewMain.Frame = new RectangleF(viewMain.Frame.X, viewPosition.Frame.Height + scrollViewWaveForm.Frame.Height, viewMain.Frame.Width, viewMain.Frame.Height);
                    _volumeView.Frame = new RectangleF(_volumeView.Frame.X, viewPosition.Frame.Height + scrollViewWaveForm.Frame.Height + viewMain.Frame.Height - 32, _volumeView.Frame.Width, _volumeView.Frame.Height);
                    lblSlideMessage.Alpha = 1;
                    lblScrubbingType.Alpha = 1;
                });
            };
            sliderPosition.TouchesMovedEvent += (sender, e) => {
                _isPositionChanging = true;
                //Console.WriteLine("Position: Setting value to " + position.ToString());

                PlayerPositionEntity entity = OnPlayerRequestPosition(sliderPosition.Value / 10000);
                lblPosition.Text = entity.Position;
                scrollViewWaveForm.WaveFormView.SecondaryPosition = entity.PositionBytes;
            };
            sliderPosition.TouchesEndedEvent += (sender, e) => {
                //Console.WriteLine("Position: Setting value to " + position.ToString());
                UIView.Animate(0.2f, () => {
                    float offset = 42;
                    viewPosition.Frame = new RectangleF(viewPosition.Frame.X, viewPosition.Frame.Y, viewPosition.Frame.Width, viewPosition.Frame.Height - offset);
                    scrollViewWaveForm.Frame = new RectangleF(scrollViewWaveForm.Frame.X, scrollViewWaveForm.Frame.Y - offset, scrollViewWaveForm.Frame.Width, scrollViewWaveForm.Frame.Height / 2);
                    scrollViewWaveForm.WaveFormView.Frame = new RectangleF(scrollViewWaveForm.WaveFormView.Frame.X, scrollViewWaveForm.WaveFormView.Frame.Y, scrollViewWaveForm.WaveFormView.Frame.Width, scrollViewWaveForm.WaveFormView.Frame.Height / 2);
                    viewMain.Frame = new RectangleF(viewMain.Frame.X, viewPosition.Frame.Height + scrollViewWaveForm.Frame.Height, viewMain.Frame.Width, viewMain.Frame.Height);
                    _volumeView.Frame = new RectangleF(_volumeView.Frame.X, viewPosition.Frame.Height + scrollViewWaveForm.Frame.Height + viewMain.Frame.Height - 32, _volumeView.Frame.Width, _volumeView.Frame.Height);
                    lblSlideMessage.Alpha = 0;
                    lblScrubbingType.Alpha = 0;
                });
                OnPlayerSetPosition(sliderPosition.Value / 100);
                scrollViewWaveForm.WaveFormView.SecondaryPosition = 0;
                _isPositionChanging = false;
            };

            // Create MPVolumeView (only visible on physical iOS device)

            RectangleF rectVolume;
            if (UserInterfaceIdiomIsPhone)
                rectVolume = new RectangleF(8, UIScreen.MainScreen.Bounds.Height - 44 - 52, UIScreen.MainScreen.Bounds.Width - 16, 46);
            else
                rectVolume = new RectangleF(8 + 320, UIScreen.MainScreen.Bounds.Height - 44 - 50, UIScreen.MainScreen.Bounds.Width - 16 - 320, 46);
            _volumeView = new MPVolumeView(rectVolume);
            _volumeView.SetVolumeThumbImage(UIImage.FromBundle("Images/Sliders/thumbbig"), UIControlState.Normal);
            _volumeView.SetMinimumVolumeSliderImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            _volumeView.SetMaximumVolumeSliderImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            this.View.AddSubview(_volumeView);

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
            
            // Set back button for navigation bar
            _btnBack = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null, null);
            _btnBack.SetTitleTextAttributes(attr, UIControlState.Normal);
            this.NavigationItem.BackBarButtonItem = _btnBack;

            // Reset temporary text
            lblLength.Text = string.Empty;
            lblPosition.Text = string.Empty;

            base.ViewDidLoad();           
		}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Now Playing", _currentNavigationSubtitle);
        }

        public void AddScrollView(UIViewController viewController)
        {
            if (UserInterfaceIdiomIsPhone)
            {
                viewController.View.Frame = new RectangleF(scrollView.Subviews.Length * scrollView.Frame.Width, 0, scrollView.Frame.Width, scrollView.Frame.Height);
                scrollView.AddSubview(viewController.View);
                pageControl.Pages = scrollView.Subviews.Length;
                scrollView.ContentSize = new SizeF(scrollView.Subviews.Length * scrollView.Frame.Width, scrollView.Frame.Height);
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
                pageControl.Pages = 3;
                scrollView.ContentSize = new SizeF(3 * scrollView.Frame.Width, scrollView.Frame.Height);
            }
        }

        [Export("scrollViewDidScroll:")]
        public void ScrollViewDidScroll(UIScrollView scrollview)
        {
            float pageWidth = scrollView.Frame.Size.Width;
            int page = (int)Math.Floor((scrollView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;
            pageControl.CurrentPage = page;
        }

        [Export("scrollViewDidEndDecelerating:")]
        public void ScrollViewDidEndDecelerating(UIScrollView scrollView)
        {
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
        public Func<float, PlayerPositionEntity> OnPlayerRequestPosition { get; set; }

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            InvokeOnMainThread(() => {
                if(!_isPositionChanging)
                {
                    lblPosition.Text = entity.Position;
                    sliderPosition.SetPosition(entity.PositionPercentage * 100);
                }

                scrollViewWaveForm.WaveFormView.Position = entity.PositionBytes;                
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

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
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
                Task<UIImage>.Factory.StartNew(() => {
                    try
                    {
                        byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);                        
                        using (NSData imageData = NSData.FromArray(bytesImage))
                        {
                            using (UIImage image = UIImage.LoadFromData(imageData))
                            {
                                if (image != null)
                                {
                                    try
                                    {
                                        _currentAlbumArtKey = key;                                    
                                        UIImage imageResized = CoreGraphicsHelper.ScaleImage(image, height);
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
                    catch(Exception ex)
                    {
                        Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to process image: {0}", ex);
                    }
                    
                    return null;
                }).ContinueWith(t => {
                    UIImage image = t.Result;
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
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            // Refresh other fields
            InvokeOnMainThread(() => {
                try
                {
                    lblLength.Text = audioFile.Length;
                    scrollViewWaveForm.SetWaveFormLength(lengthBytes);

                    _currentNavigationSubtitle = (playlistIndex+1).ToString() + " of " + playlistCount.ToString();
                    MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
                    navCtrl.SetTitle("Now Playing", _currentNavigationSubtitle);

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
                        btnPlayPause.SetImage(UIImage.FromBundle("Images/Buttons/play"), UIControlState.Normal);
                        btnPlayPause.SetImage(UIImage.FromBundle("Images/Buttons/play_on"), UIControlState.Highlighted);
                        break;
                    case PlayerStatusType.Playing:
                        btnPlayPause.SetImage(UIImage.FromBundle("Images/Buttons/pause"), UIControlState.Normal);
                        btnPlayPause.SetImage(UIImage.FromBundle("Images/Buttons/pause_on"), UIControlState.Highlighted);
                        break;
                }
            });
        }

        public void PlayerError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alert = new UIAlertView("An error occured", ex.Message, null, "OK", null);
                alert.Show();
            });
        }

        #endregion
	}
}

