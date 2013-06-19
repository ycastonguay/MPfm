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
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Objects;
using MPfm.Player.Objects;

namespace MPfm.iOS
{
    public partial class PlayerMetadataViewController : BaseViewController, IPlayerMetadataView
    {
        //NSTimer timer;

        public PlayerMetadataViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "PlayerMetadataViewController_iPhone" : "PlayerMetadataViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {                
            // Reset temporary text
            lblArtistName.Text = string.Empty;
            lblAlbumTitle.Text = string.Empty;
            lblTitle.Text = string.Empty;
            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;

            btnPlaylist.BackgroundColor = UIColor.Clear;
            btnRepeat.BackgroundColor = UIColor.Clear;
            btnShuffle.BackgroundColor = UIColor.Clear;
            btnPlaylist.SetImage(UIImage.FromBundle("Images/Buttons/playlist"), UIControlState.Normal);
            btnPlaylist.SetImage(UIImage.FromBundle("Images/Buttons/playlist_on"), UIControlState.Highlighted);
            btnRepeat.SetImage(UIImage.FromBundle("Images/Buttons/repeat"), UIControlState.Normal);
            btnShuffle.SetImage(UIImage.FromBundle("Images/Buttons/shuffle"), UIControlState.Normal);

            base.ViewDidLoad();            
        }

        partial void actionPlaylist(NSObject sender)
        {
            OnClickPlaylist();
        }

        partial void actionRepeat(NSObject sender)
        {
            OnToggleRepeat();
        }

        partial void actionShuffle(NSObject sender)
        {
            OnToggleShuffle();
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

        public Action OnClickPlaylist { get; set; }
        public Action OnToggleShuffle { get; set; }
        public Action OnToggleRepeat { get; set; }

        public void RefreshShuffle(bool shuffle)
        {
            InvokeOnMainThread(() => {
                if(shuffle)
                    btnShuffle.SetImage(UIImage.FromBundle("Images/Buttons/shuffle_on"), UIControlState.Normal);
                else
                    btnShuffle.SetImage(UIImage.FromBundle("Images/Buttons/shuffle"), UIControlState.Normal);
            });
        }

        public void RefreshRepeat(RepeatType repeatType)
        {
            InvokeOnMainThread(() => {
                if(repeatType == RepeatType.Off)
                    btnRepeat.SetImage(UIImage.FromBundle("Images/Buttons/repeat"), UIControlState.Normal);
                else if(repeatType == RepeatType.Playlist)
                    btnRepeat.SetImage(UIImage.FromBundle("Images/Buttons/repeat_on"), UIControlState.Normal);
                else
                    btnRepeat.SetImage(UIImage.FromBundle("Images/Buttons/repeat_song_on"), UIControlState.Normal);
            });
        }

        public void RefreshAudioFile(AudioFile audioFile)
        {
            InvokeOnMainThread(() => {
                if(audioFile == null)
                {
                    lblArtistName.Text = string.Empty;
                    lblAlbumTitle.Text = string.Empty;
                    lblTitle.Text = string.Empty;
                    
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
