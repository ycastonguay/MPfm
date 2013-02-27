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
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Player;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Bass.Net;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MonoTouch.MediaPlayer;

namespace MPfm.iOS.Classes.Controllers
{
	public partial class PlayerViewController : BaseViewController, IPlayerView
	{
		public PlayerViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "PlayerViewController_iPhone" : "PlayerViewController_iPad", null)
		{
		}
		
		public override void ViewDidLoad()
        {
            // Set fonts
            lblPosition.Font = UIFont.FromName("OstrichSans-Black", 18);
            lblLength.Font = UIFont.FromName("OstrichSans-Black", 18);
            btnPrevious.Font = UIFont.FromName("OstrichSans-Black", 18);
            btnPlayPause.Font = UIFont.FromName("OstrichSans-Black", 18);
            btnNext.Font = UIFont.FromName("OstrichSans-Black", 18);
			
            // Reduce the song position slider size
            sliderPosition.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
            sliderPosition.Frame = new RectangleF(70, sliderPosition.Frame.Y, 180, sliderPosition.Frame.Height);

            // Setup scroll view and page control
            scrollView.WeakDelegate = this;
            scrollView.PagingEnabled = true;
            scrollView.ShowsHorizontalScrollIndicator = false;
            scrollView.ShowsVerticalScrollIndicator = false;
            pageControl.CurrentPage = 0;

            sliderPosition.OnTouchesMoved = (position) => {
                // 0 to 10000
                Console.WriteLine("Position: Setting value to " + position.ToString());
                OnPlayerSetPosition(position / 100);
            };

            // Create MPVolumeView (only visible on physical iOS device)
            MPVolumeView volumeView = new MPVolumeView(new RectangleF(8, UIScreen.MainScreen.Bounds.Height - 44 - 46 - 4, UIScreen.MainScreen.Bounds.Width - 16, 46));
            this.View.AddSubview(volumeView);

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
                lblPosition.Text = entity.Position;
                sliderPosition.SetPosition(entity.PositionPercentage * 100);
            });
        }

        public void RefreshSongInformation(AudioFile audioFile)
        {
            InvokeOnMainThread(() => {

                if(audioFile != null)
                {
                    try
                    {
                        // TODO: Add a memory cache and stop reloading the image from disk every time
                        byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                        NSData imageData = NSData.FromArray(bytesImage);
                        UIImage image = UIImage.LoadFromData(imageData);
                        imageViewAlbumArt.Image = image;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Could not load album art: " + ex.Message);
                    }

                    lblLength.Text = audioFile.Length;
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
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {
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

