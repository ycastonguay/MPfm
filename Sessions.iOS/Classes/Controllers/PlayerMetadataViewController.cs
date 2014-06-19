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
using System.Drawing;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Objects;
using Sessions.Player.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;

namespace Sessions.iOS
{
    public partial class PlayerMetadataViewController : BaseViewController, IPlayerMetadataView
    {
        //NSTimer timer;

        public PlayerMetadataViewController()
            : base (UserInterfaceIdiomIsPhone ? "PlayerMetadataViewController_iPhone" : "PlayerMetadataViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {                
            // Reset temporary text
            lblArtistName.Text = string.Empty;
            lblAlbumTitle.Text = string.Empty;
            lblTitle.Text = string.Empty;
            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;

            base.ViewDidLoad();          

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindPlayerMetadataView(this);
        }

        public void ShowPanel(bool show, bool swipeUp)
        {
            if (show && !swipeUp)
            {
                viewBackground.Frame = new RectangleF(viewBackground.Frame.X, View.Bounds.Height - viewBackground.Frame.Height, viewBackground.Frame.Width, viewBackground.Frame.Height);
                UIView.Animate(0.2f, () => {
                    viewBackground.Alpha = 1;
                });
            }
            else if (show && swipeUp)
            {
                UIView.Animate(0.4f, () => {
                    viewBackground.Frame = new RectangleF(viewBackground.Frame.X, View.Bounds.Height - viewBackground.Frame.Height, viewBackground.Frame.Width, viewBackground.Frame.Height);
                    viewBackground.Alpha = 1;
                });
            }
            else
            {
                UIView.Animate(0.4f, () => {
                    viewBackground.Frame = new RectangleF(viewBackground.Frame.X, View.Bounds.Height, viewBackground.Frame.Width, viewBackground.Frame.Height);
                    viewBackground.Alpha = 0;
                });
            }
        }

        #region IPlayerMetadataView implementation

        public Action OnOpenPlaylist { get; set; }
        public Action OnToggleShuffle { get; set; }
        public Action OnToggleRepeat { get; set; }

        public void RefreshShuffle(bool shuffle)
        {
        }

        public void RefreshRepeat(RepeatType repeatType)
        {
        }

        public void RefreshMetadata(AudioFile audioFile, int playlistIndex, int playlistCount)
        {
            InvokeOnMainThread(() => {
                if(audioFile == null)
                {
                    lblArtistName.Text = string.Empty;
                    lblAlbumTitle.Text = string.Empty;
                    lblTitle.Text = string.Empty;
                    lblSongCount.Text = string.Empty;
                    
                    // Update AirPlay metadata with generic info
                    if(MPNowPlayingInfoCenter.DefaultCenter != null)
                    {
                        // Reset info
                        MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = null;
                    }
                }
                else
                {
                    lblArtistName.Text = audioFile.ArtistName;
                    lblAlbumTitle.Text = audioFile.AlbumTitle;
                    lblTitle.Text = audioFile.Title;
                    lblSongCount.Text = string.Format("{0}/{1}", playlistIndex+1, playlistCount);

                    ShowPanel(true, false);

                    // Update AirPlay metadata with generic info
                    if(MPNowPlayingInfoCenter.DefaultCenter != null)
                    {
                        // TODO: Add a memory cache and stop reloading the image from disk every time
                        byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                        NSData imageData = NSData.FromArray(bytesImage);
                        UIImage image = UIImage.LoadFromData(imageData);

                        MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = new MPNowPlayingInfo() {
                            Artist = audioFile.ArtistName,
                            AlbumTitle = audioFile.AlbumTitle,
                            Title = audioFile.Title,
                            Artwork = (image != null) ? new MPMediaItemArtwork(image) : null
                        };
                    }
                }
            });
        }

        #endregion
    }
}
