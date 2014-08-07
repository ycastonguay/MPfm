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
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Services;
using Sessions.Sound.AudioFiles;
using System.Threading.Tasks;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsAlbumArtView")]
    public class SessionsAlbumArtView : UIView
    {
        private IDownloadImageService _downloadImageService;
        private StatusType _statusType;
        private UIImageView _imageViewAlbum;
        private SessionsPopupView _viewImageDownloading;
        private SessionsPopupView _viewImageDownloaded;
        private SessionsPopupView _viewImageDownloadError;

        private string _currentAlbumArtKey;

        public event SessionsPopupView.ButtonClick OnButton1Click;
        public event SessionsPopupView.ButtonClick OnButton2Click;

        public SessionsAlbumArtView() 
            : base()
        {
            Initialize();
        }

        public SessionsAlbumArtView(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        public SessionsAlbumArtView(RectangleF frame)
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            _downloadImageService = new DownloadImageService();
            BackgroundColor = GlobalTheme.BackgroundColor;
            TintColor = UIColor.White;
            UserInteractionEnabled = true;

            _imageViewAlbum = new UIImageView(new RectangleF(0, 0, Frame.Width, Frame.Height));
            _imageViewAlbum.Alpha = 0;
            _imageViewAlbum.BackgroundColor = GlobalTheme.BackgroundDarkColor;
            _imageViewAlbum.AutoresizingMask = UIViewAutoresizing.None;
            _imageViewAlbum.ClipsToBounds = true;
            AddSubview(_imageViewAlbum);

            // Background + indicator + label
            _viewImageDownloading = new SessionsPopupView(new RectangleF(0, 0, Frame.Width, 60));
            _viewImageDownloading.SetTheme(SessionsPopupView.ThemeType.LabelWithActivityIndicator);
            _viewImageDownloading.LabelTitle = "Downloading image from the internet...";
            _viewImageDownloading.Alpha = 0;
            AddSubview(_viewImageDownloading);

            // Background + label + 2 buttons
            _viewImageDownloaded = new SessionsPopupView(new RectangleF(0, 0, Frame.Width, 60));
            _viewImageDownloaded.SetTheme(SessionsPopupView.ThemeType.LabelWithButtons);
            _viewImageDownloaded.LabelTitle = "This image has been downloaded from the internet.";
            _viewImageDownloaded.Alpha = 0;
            _viewImageDownloaded.OnButton1Click += HandleOnButton1Click;
            _viewImageDownloaded.OnButton2Click += HandleOnButton2Click;
            AddSubview(_viewImageDownloaded);

            // Background + label
            _viewImageDownloadError = new SessionsPopupView(new RectangleF(0, 0, Frame.Width, 60));
            _viewImageDownloadError.SetTheme(SessionsPopupView.ThemeType.Label);
            _viewImageDownloadError.LabelTitle = "Error downloading image from the internet.";
            _viewImageDownloadError.Alpha = 0;
            AddSubview(_viewImageDownloadError);

            //var stupidButton = new UIButton(UIButtonType.System);
            var stupidButton = new UIButton(new RectangleF(0, 60, 200, 40));
            stupidButton.UserInteractionEnabled = true;
            stupidButton.SetTitle("BLEEEH", UIControlState.Normal);
            stupidButton.BackgroundColor = UIColor.Purple;
            //stupidButton.Frame = new RectangleF(0, 60, 200, 40);
            stupidButton.TouchUpInside += (sender, e) => {
                Console.WriteLine("ASDFASDFASODFIASIDOFAISODFOIASIFOASIODFIASIODFASDF");
            };
            AddSubview(stupidButton);

        }

        private void HandleOnButton1Click()
        {
            if(OnButton1Click != null)
                OnButton1Click();
        }

        private void HandleOnButton2Click()
        {
            if(OnButton2Click != null)
                OnButton2Click();            
        }

        public void SetImage(UIImage image)
        {
            _imageViewAlbum.Alpha = 0;
            _imageViewAlbum.Image = image;      
            UIView.Animate(0.2, () =>
            {
                _imageViewAlbum.Alpha = 1;
            });  
        }

        public async void DownloadImage(AudioFile audioFile)
        {
            string key = audioFile.ArtistName.ToUpper() + "_" + audioFile.AlbumTitle.ToUpper();
            if (_currentAlbumArtKey == key)
                return;

            UIView.Animate(0.2, () =>
            {
                _imageViewAlbum.Alpha = 0;
                _viewImageDownloadError.Alpha = 0;
                _viewImageDownloaded.Alpha = 0;
            });

            _viewImageDownloading.AnimateIn();

            var task = _downloadImageService.DownloadAlbumArt(audioFile);
            task.Start();
            var result = await task;
            if (result == null)
            {
                Console.WriteLine("AlbumArtView - Error downloading image!");
                _viewImageDownloading.AnimateOut(() => {
                    _viewImageDownloadError.AnimateIn();
                });
            }
            else
            {
                // Load album art + resize in another thread
                Console.WriteLine("AlbumArtView - Downloaded image successfully!");
                var task2 = Task<UIImage>.Factory.StartNew(() =>
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
                                        InvokeOnMainThread(() => {
                                            _currentAlbumArtKey = key;                                    
                                            imageResized = CoreGraphicsHelper.ScaleImage(imageFullSize, (int)Frame.Height);
                                        });
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
                        Console.WriteLine("AlbumArtView - DownloadImage - Failed to process image: {0}", ex);
                    }
                    
                    return null;
                });

                UIImage image = await task2;
                InvokeOnMainThread(() => {

                    try
                    {
                        if (image == null)
                        {
                            Console.WriteLine("AlbumArtView - Error resizing image!");
                            _viewImageDownloading.AnimateOut(() => {
                                _viewImageDownloadError.AnimateIn();
                            });
                        }
                        else
                        {                    
                            Console.WriteLine("AlbumArtView - Setting album art...");
                            _imageViewAlbum.Image = image;

                            _viewImageDownloading.AnimateOut(() => {
                                _viewImageDownloaded.AnimateIn();
                                UIView.Animate(0.2, () => {
                                    _imageViewAlbum.Alpha = 1;
                                });                            
                            });                        
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("AlbumArtView - DownloadImage - Failed to set image after processing: {0}", ex);
                    }
                });
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float padding = 4;
            float popupWidth = Frame.Width - (padding * 2);
            float popupSmallHeight = 32;
            float popupLargeHeight = 48;

            _imageViewAlbum.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            _viewImageDownloading.Frame = new RectangleF(padding, padding, popupWidth, popupSmallHeight);
            _viewImageDownloaded.Frame = new RectangleF(padding, padding, popupWidth, popupLargeHeight);
            _viewImageDownloadError.Frame = new RectangleF(padding, padding, popupWidth, popupSmallHeight);
        }

        public enum StatusType
        {
            Normal = 0,
            ImageDownloading = 1,
            ImageDownloaded = 2,
            ImageDownloadError = 3
        }
    }
}
