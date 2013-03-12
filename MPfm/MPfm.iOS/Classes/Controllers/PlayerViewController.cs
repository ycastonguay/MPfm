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
using MPfm.Sound.Bass.Net;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Helpers;

namespace MPfm.iOS.Classes.Controllers
{
	public partial class PlayerViewController : BaseViewController, IPlayerView
	{
        private bool _isPositionChanging = false;
        private string _currentAlbumArtKey = string.Empty;

		public PlayerViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "PlayerViewController_iPhone" : "PlayerViewController_iPad", null)
		{
		}
		
		public override void ViewDidLoad()
        {
            // Set fonts
            lblPosition.Font = UIFont.FromName("OstrichSans-Black", 18);
            lblLength.Font = UIFont.FromName("OstrichSans-Black", 18);

            // Load button bitmaps
            btnPrevious.SetImage(UIImage.FromBundle("Images/Buttons/previous"), UIControlState.Normal);
            btnPrevious.SetImage(UIImage.FromBundle("Images/Buttons/previous_on"), UIControlState.Highlighted);
            btnPlayPause.SetImage(UIImage.FromBundle("Images/Buttons/pause"), UIControlState.Normal);
            btnPlayPause.SetImage(UIImage.FromBundle("Images/Buttons/pause_on"), UIControlState.Highlighted);
            btnNext.SetImage(UIImage.FromBundle("Images/Buttons/next"), UIControlState.Normal);
            btnNext.SetImage(UIImage.FromBundle("Images/Buttons/next_on"), UIControlState.Highlighted);

            //sliderPosition.SetThumbImage(UIImage.FromBundle("Images/Sliders/slider_ball"), UIControlState.Normal);
            //sliderPosition.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slide"), UIControlState.Normal);

            // Reduce the song position slider size
            sliderPosition.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
            sliderPosition.Frame = new RectangleF(70, sliderPosition.Frame.Y, 180, sliderPosition.Frame.Height);

            // Setup scroll view and page control
            scrollView.WeakDelegate = this;
            scrollView.PagingEnabled = true;
            scrollView.ShowsHorizontalScrollIndicator = false;
            scrollView.ShowsVerticalScrollIndicator = false;
            pageControl.CurrentPage = 0;

            // TODO: Block slider when the player is paused.
            sliderPosition.OnTouchesMoved = (position) => {
                _isPositionChanging = true;
                Console.WriteLine("Position: Setting value to " + position.ToString());
                lblPosition.Text = position.ToString();
            };
            sliderPosition.OnTouchesEnded = (position) => {
                Console.WriteLine("Position: Setting value to " + position.ToString());
                OnPlayerSetPosition(position / 100);
                _isPositionChanging = false;
            };

            // Create MPVolumeView (only visible on physical iOS device)
            MPVolumeView volumeView = new MPVolumeView(new RectangleF(8, UIScreen.MainScreen.Bounds.Height - 44 - 46, UIScreen.MainScreen.Bounds.Width - 16, 46));
            //volumeView.SetVolumeThumbImage(UIImage.FromBundle("Images/Sliders/slider_ball"), UIControlState.Normal);
            //volumeView.SetMinimumVolumeSliderImage(UIImage.FromBundle("Images/Sliders/slide"), UIControlState.Normal);
            this.View.AddSubview(volumeView);

            // Only display wave form on iPhone 5+
            if (DarwinHardwareHelper.Version != DarwinHardwareHelper.HardwareVersion.iPhone5)
            {
                waveFormView.Hidden = true;
            }

            base.ViewDidLoad();            
		}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetSubtitle("Now Playing");
        }

        public void AddScrollView(UIViewController viewController)
        {
            viewController.View.Frame = new RectangleF(scrollView.Subviews.Length * scrollView.Frame.Width, 0, scrollView.Frame.Width, scrollView.Frame.Height);
            scrollView.AddSubview(viewController.View);
            pageControl.Pages = scrollView.Subviews.Length;
            scrollView.ContentSize = new SizeF(scrollView.Subviews.Length * scrollView.Frame.Width, scrollView.Frame.Height);
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

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            InvokeOnMainThread(() => {
                if(!_isPositionChanging)
                {
                    lblPosition.Text = entity.Position;
                    sliderPosition.SetPosition(entity.PositionPercentage * 100);
                }
            });
        }

        public void RefreshSongInformation(AudioFile audioFile)
        {
            InvokeOnMainThread(() => {

                if(audioFile != null)
                {
                    Console.WriteLine("PlayerViewCtrl - RefreshSongInformation - " + audioFile.FilePath);
                    try
                    {
                        // Check if the album art needs to be refreshed
                        string key = audioFile.ArtistName.ToUpper() + "_" + audioFile.AlbumTitle.ToUpper();
                        if(_currentAlbumArtKey != key)
                        {
                            // TODO: Add a memory cache and stop reloading the image from disk every time
                            _currentAlbumArtKey = key;
                            byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                            using(NSData imageData = NSData.FromArray(bytesImage))
                            using(UIImage image = UIImage.LoadFromData(imageData))
                            {
                                imageViewAlbumArt.Alpha = 0;
                                imageViewAlbumArt.Image = image;
                            }

                            UIView.Animate(0.3, () => {
                                imageViewAlbumArt.Alpha = 1;
                            });
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Could not load album art: " + ex.Message);
                    }

                    lblLength.Text = audioFile.Length;

                    // Load peak file in background
                    waveFormView.LoadPeakFile(audioFile);
                }
                else
                {
                    // TODO: If the playlist is finished, return to the Mobile Library Browser. At least that's what the iOS Music app does.
                    lblLength.Text = string.Empty;
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

