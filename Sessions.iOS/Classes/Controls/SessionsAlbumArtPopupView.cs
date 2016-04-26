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
using CoreGraphics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsAlbumArtPopupView")]
    public class SessionsAlbumArtPopupView : UIView
    {
        private SessionsPopupView _viewImageDownloading;
        private SessionsPopupView _viewImageDownloaded;
        private SessionsPopupView _viewImageDownloadError;

        public event SessionsPopupView.ButtonClick OnButtonClick;

        public SessionsAlbumArtPopupView() 
            : base()
        {
            Initialize();
        }

        public SessionsAlbumArtPopupView(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        public SessionsAlbumArtPopupView(CGRect frame)
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            BackgroundColor = UIColor.Clear;
            TintColor = UIColor.White;
            UserInteractionEnabled = true;

            // Background + indicator + label
            _viewImageDownloading = new SessionsPopupView(new CGRect(0, 0, Frame.Width, 60));
            _viewImageDownloading.SetTheme(SessionsPopupView.ThemeType.LabelWithActivityIndicator);
            _viewImageDownloading.LabelTitle = "Downloading image from the internet...";
            _viewImageDownloading.Alpha = 0;
            AddSubview(_viewImageDownloading);

            // Background + label + 2 buttons
            _viewImageDownloaded = new SessionsPopupView(new CGRect(0, 0, Frame.Width, 60));
            _viewImageDownloaded.SetTheme(SessionsPopupView.ThemeType.LabelWithButtons);
            _viewImageDownloaded.LabelTitle = "This image has been downloaded from the internet.";
            _viewImageDownloaded.Alpha = 0;
            _viewImageDownloaded.OnButtonClick += HandleOnButtonClick;
            AddSubview(_viewImageDownloaded);

            // Background + label
            _viewImageDownloadError = new SessionsPopupView(new CGRect(0, 0, Frame.Width, 60));
            _viewImageDownloadError.SetTheme(SessionsPopupView.ThemeType.Label);
            _viewImageDownloadError.LabelTitle = "Error downloading image from the internet.";
            _viewImageDownloadError.Alpha = 0;            
            AddSubview(_viewImageDownloadError);
        }

        private void HandleOnButtonClick()
        {
            if(OnButtonClick != null)
                OnButtonClick();
        }

        public void ShowDownloadingView()
        {
            InvokeOnMainThread(() =>
            {
                UIView.Animate(0.2, () =>
                {
                    _viewImageDownloadError.Alpha = 0;
                    _viewImageDownloaded.Alpha = 0;
                });
                _viewImageDownloading.AnimateIn();
            });
        }

        public void ShowDownloadErrorView()
        {
            InvokeOnMainThread(() =>
            {
                _viewImageDownloading.AnimateOut(() => {
                    _viewImageDownloadError.AnimateIn();
                });
            });
        }

        public void ShowDownloadedView(Action completed)
        {
            InvokeOnMainThread(() =>
            {
                _viewImageDownloading.AnimateOut(() =>
                {
                    _viewImageDownloaded.AnimateIn();
                    if(completed != null)
                        completed();
                }); 
            });
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float padding = 4;
            nfloat popupWidth = Frame.Width - (padding * 2);
            float popupSmallHeight = 32;
            float popupLargeHeight = 48;

            _viewImageDownloading.Frame = new CGRect(padding, padding, popupWidth, popupSmallHeight);
            _viewImageDownloaded.Frame = new CGRect(padding, padding, popupWidth, popupLargeHeight);
            _viewImageDownloadError.Frame = new CGRect(padding, padding, popupWidth, popupSmallHeight);
        }
    }
}
