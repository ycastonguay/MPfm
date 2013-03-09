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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MonoTouch.MediaPlayer;

namespace MPfm.iOS
{
    public partial class PlayerMetadataViewController : BaseViewController, IPlayerMetadataView
    {
        public PlayerMetadataViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "PlayerMetadataViewController_iPhone" : "PlayerMetadataViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {                
            // Set fonts
            lblArtistName.Font = UIFont.FromName("OstrichSans-Black", 28);
            lblAlbumTitle.Font = UIFont.FromName("OstrichSans-Medium", 24);
            lblTitle.Font = UIFont.FromName("OstrichSans-Medium", 18);

            base.ViewDidLoad();            
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
                            Artwork = new MPMediaItemArtwork(image)
                        };
                    }
                }
            });
        }
    }
}
