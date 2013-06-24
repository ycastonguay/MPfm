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
            UIView backView = new UIView(Frame);
            backView.BackgroundColor = GlobalTheme.LightColor;
            BackgroundView = backView;
            BackgroundColor = UIColor.White;
            
            UIView backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;           

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
            RightButton.Frame = new RectangleF(Bounds.Width - Bounds.Height, 0, Bounds.Height, Bounds.Height);
            RightButton.TouchUpInside += HandleRightButtonTouchUpInside;
            AddSubview(RightButton);

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron"));
            ImageChevron.BackgroundColor = UIColor.Clear;
            ImageChevron.Hidden = true;
            ImageChevron.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 22, 0, 22, 44);
            AddSubview(ImageChevron);

            RightImage = new UIImageView(UIImage.FromBundle("Images/Icons/icon_speaker"));
            RightImage.Alpha = 0.7f;
            RightImage.BackgroundColor = UIColor.Clear;
            RightImage.Hidden = true;
            RightImage.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 66, 0, 44, 44);
            AddSubview(RightImage);
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
                textWidth -= Bounds.Height + padding;
            if (RightButton.ImageView.Image != null)
                textWidth -= Bounds.Height + padding;
            if (!string.IsNullOrEmpty(IndexTextLabel.Text))
                textWidth -= 22 + padding + padding;
            if (ImageChevron.Image != null && !ImageChevron.Hidden)
                textWidth -= 22;
            if (RightImage.Image != null && !RightImage.Hidden)
                textWidth -= 44;

            float x = 0;
            if (ImageView.Image != null)
            {
                ImageView.Frame = new RectangleF(x, 0, Bounds.Height, Bounds.Height);
                x += Bounds.Height + padding;
            } 
            else if (!string.IsNullOrEmpty(IndexTextLabel.Text))
            {
                x += padding;
                IndexTextLabel.Frame = new RectangleF(x, 2, 22, 38);
                x += 22 + padding;
            } 
            else
            {
                x += padding;
            }

            float titleY = 10;
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                titleY = 2;

            TextLabel.Frame = new RectangleF(x, titleY, textWidth, 22);
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                DetailTextLabel.Frame = new RectangleF(x, 22, textWidth, 16);

            if (RightButton.ImageView.Image != null)
                RightButton.Frame = new RectangleF(Bounds.Width - Bounds.Height, 0, Bounds.Height, Bounds.Height);

            ImageChevron.Frame = new RectangleF(screenSize.Width - 22, 0, 22, 44);

            if(ImageChevron.Hidden)
                RightImage.Frame = new RectangleF(screenSize.Width - 44, 0, 44, 44);
            else
                RightImage.Frame = new RectangleF(screenSize.Width - 66, 0, 44, 44);
        }
    }
}
