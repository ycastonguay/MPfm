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
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmTableViewCell")]
    public class MPfmTableViewCell : UITableViewCell
    {
        public UILabel IndexTextLabel { get; private set; }
        public UIButton RightButton { get; private set; }
        public UIImageView RightImage { get; private set; }
        public UIImageView ImageChevron { get; private set; }
        public UIImageView ImageAlbum1 { get; private set; }
        public UIImageView ImageAlbum2 { get; private set; }
        public UIImageView ImageAlbum3 { get; private set; }
        public UILabel AlbumCountLabel { get; private set; }

        public float RightOffset { get; set; }

        public delegate void RightButtonTap(MPfmTableViewCell cell);
        public event RightButtonTap OnRightButtonTap;

        public MPfmTableViewCell() : base()
        {
            Initialize();
        }

        public MPfmTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        public MPfmTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public void Initialize()
        {
            var screenSize = UIKitHelper.GetDeviceSize();

            UIView backView = new UIView(Frame);
            backView.BackgroundColor = GlobalTheme.LightColor;
            BackgroundView = backView;
            BackgroundColor = UIColor.White;
            
            UIView backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;           

            ImageAlbum1 = new UIImageView();
            ImageAlbum1.BackgroundColor = UIColor.White;
            ImageAlbum1.Hidden = true;
            ImageAlbum1.Alpha = 0.75f;
            ImageAlbum1.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44);
            AddSubview(ImageAlbum1);

            ImageAlbum2 = new UIImageView();
            ImageAlbum2.BackgroundColor = UIColor.White;
            ImageAlbum2.Hidden = true;
            ImageAlbum2.Alpha = 0.4f;
            ImageAlbum2.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 132, 4, 44, 44);
            AddSubview(ImageAlbum2);

            ImageAlbum3 = new UIImageView();
            ImageAlbum3.BackgroundColor = UIColor.White;
            ImageAlbum3.Hidden = true;
            ImageAlbum3.Alpha = 0.15f;
            ImageAlbum3.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 186, 4, 44, 44);
            AddSubview(ImageAlbum3);

            AlbumCountLabel = new UILabel();
            AlbumCountLabel.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44);
            AlbumCountLabel.Alpha = 0.75f;
            AlbumCountLabel.BackgroundColor = GlobalTheme.MainColor;
            AlbumCountLabel.Font = UIFont.FromName("HelveticaNeue-Light", 18);
            AlbumCountLabel.Hidden = true;
            AlbumCountLabel.Text = "+98";
            AlbumCountLabel.TextColor = UIColor.White;
            AlbumCountLabel.TextAlignment = UITextAlignment.Center;
            AlbumCountLabel.HighlightedTextColor = UIColor.White;
            AddSubview(AlbumCountLabel);

            TextLabel.BackgroundColor = UIColor.FromWhiteAlpha(0, 0);
            TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 14);
            TextLabel.TextColor = UIColor.Black;
            TextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.TextColor = UIColor.Gray;
            DetailTextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Font = UIFont.FromName("HelveticaNeue", 12);
            ImageView.BackgroundColor = UIColor.White;

            IndexTextLabel = new UILabel();
            IndexTextLabel.BackgroundColor = UIColor.Clear;
            IndexTextLabel.Font = UIFont.FromName("HelveticaNeue", 16);
            IndexTextLabel.TextColor = UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);
            IndexTextLabel.TextAlignment = UITextAlignment.Center;
            IndexTextLabel.HighlightedTextColor = UIColor.White;
            AddSubview(IndexTextLabel);

            RightButton = new UIButton(UIButtonType.Custom);
            RightButton.Hidden = true;
            RightButton.Frame = new RectangleF(screenSize.Width - Bounds.Height, 4, Bounds.Height, Bounds.Height);
            RightButton.TouchUpInside += HandleRightButtonTouchUpInside;
            AddSubview(RightButton);

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron"));
            ImageChevron.BackgroundColor = UIColor.Clear;
            ImageChevron.Hidden = true;
            ImageChevron.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 22, 4, 22, 44);
            AddSubview(ImageChevron);           

            RightImage = new UIImageView(UIImage.FromBundle("Images/Icons/icon_speaker"));
            RightImage.Alpha = 0.7f;
            RightImage.BackgroundColor = UIColor.Clear;
            RightImage.Hidden = true;
            RightImage.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 66, 4, 44, 44);
            AddSubview(RightImage);

            // Make sure the text label is over all other subviews
            TextLabel.RemoveFromSuperview();
            AddSubview(TextLabel);
        }

        private void HandleRightButtonTouchUpInside(object sender, EventArgs e)
        {
            if (OnRightButtonTap != null)
                OnRightButtonTap(this);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var screenSize = UIKitHelper.GetDeviceSize();
            float padding = 8;

            // Determine width available for text
            float textWidth = Bounds.Width;
            if (Accessory != UITableViewCellAccessory.None)
                textWidth -= 44;
            if (ImageView.Image != null && !ImageView.Hidden)
                textWidth -= 44 + padding;
            if (RightButton.ImageView.Image != null)
                textWidth -= 44 + padding;
            if (!string.IsNullOrEmpty(IndexTextLabel.Text))
                textWidth -= 22 + padding + padding;
            if (ImageChevron.Image != null && !ImageChevron.Hidden)
                textWidth -= 22;
            if (RightImage.Image != null && !RightImage.Hidden)
                textWidth -= 44;

            float x = 0;
            if (ImageView.Image != null)
            {
                ImageView.Frame = new RectangleF(x, 4, 44, 44);
                x += 44 + padding;
            } 
            else if (!string.IsNullOrEmpty(IndexTextLabel.Text))
            {
                x += padding;
                IndexTextLabel.Frame = new RectangleF(x, 6, 22, 38);
                x += 22 + padding;
            } 
            else
            {
                x += padding;
            }

            float titleY = 10 + 4;
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                titleY = 2 + 4;

            TextLabel.Frame = new RectangleF(x, titleY, textWidth, 22);
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                DetailTextLabel.Frame = new RectangleF(x, 22 + 4, textWidth, 16);

            if (RightButton.ImageView.Image != null || !string.IsNullOrEmpty(RightButton.Title(UIControlState.Normal)))
                RightButton.Frame = new RectangleF(screenSize.Width - 44, 4, 44, 44);

            ImageChevron.Frame = new RectangleF(screenSize.Width - 22 - RightOffset, 4, 22, 44);

            if(ImageChevron.Hidden)
                RightImage.Frame = new RectangleF(screenSize.Width - 44 - RightOffset, 4, 44, 44);
            else
                RightImage.Frame = new RectangleF(screenSize.Width - 66 - RightOffset, 4, 44, 44);
        }
    }
}
