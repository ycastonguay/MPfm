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

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmTableViewCell")]
    public class MPfmTableViewCell : UITableViewCell
    {
        public UILabel IndexTextLabel { get; private set; }
        public UIImageView RightImageView { get; private set; }

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
            // Create selected cell background view
            UIView backView = new UIView(Frame);
//            CAGradientLayer gradient = new CAGradientLayer();
//            gradient.Frame = Bounds;
//            gradient.Colors = new MonoTouch.CoreGraphics.CGColor[2] { new CGColor(1.0f, 1.0f, 1.0f, 1), new CGColor(0.95f, 0.95f, 0.95f, 1) };
//            backView.Layer.InsertSublayer(gradient, 0);
            backView.BackgroundColor = GlobalTheme.LightColor;
            BackgroundView = backView;
            
            // Create selected cell background view
            UIView backViewSelected = new UIView(Frame);
//            CAGradientLayer gradientSelected = new CAGradientLayer();
//            gradientSelected.Frame = Bounds;
//            gradientSelected.Colors = new MonoTouch.CoreGraphics.CGColor[2] { new CGColor(0.6f, 0.6f, 0.6f, 1), new CGColor(0.4f, 0.4f, 0.4f, 1) };
//            backViewSelected.Layer.InsertSublayer(gradientSelected, 0);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;

            TextLabel.BackgroundColor = UIColor.Clear;
            //TextLabel.Font = UIFont.FromName("OstrichSans-Medium", 20);
            TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 14);
            TextLabel.TextColor = UIColor.Black;
            TextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.BackgroundColor = UIColor.Clear;            
            DetailTextLabel.TextColor = UIColor.Gray;
            DetailTextLabel.HighlightedTextColor = UIColor.White;
            //DetailTextLabel.Font = UIFont.FromName("OstrichSans-Medium", 16);
            DetailTextLabel.Font = UIFont.FromName("HelveticaNeue", 12);
            ImageView.BackgroundColor = UIColor.White;

            IndexTextLabel = new UILabel();
            IndexTextLabel.BackgroundColor = UIColor.Clear;
            //IndexTextLabel.Font = UIFont.FromName("OstrichSans-Black", 20);
            IndexTextLabel.Font = UIFont.FromName("HelveticaNeue-Bold", 16);
            IndexTextLabel.TextColor = UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);
            IndexTextLabel.HighlightedTextColor = UIColor.White;
            AddSubview(IndexTextLabel);

            RightImageView = new UIImageView();
            RightImageView.Hidden = true;
            AddSubview(RightImageView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            // Check for subtitle
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
            {
                TextLabel.Frame = new RectangleF(53, 2, Bounds.Width - 86, 22);
                DetailTextLabel.Frame = new RectangleF(53, 22, Bounds.Width - 24, 16);
            }
            if (!string.IsNullOrEmpty(IndexTextLabel.Text))
            {
                TextLabel.Frame = new RectangleF(33, 2, Bounds.Width - 86, 22);
                DetailTextLabel.Frame = new RectangleF(33, 22, Bounds.Width - 24, 16);
                IndexTextLabel.Frame = new RectangleF(12, 2, 22, 38);
            }
            if (RightImageView.Image != null)
            {
                TextLabel.Frame = new RectangleF(33, 2, Bounds.Width - 86, 22);
                DetailTextLabel.Frame = new RectangleF(33, 22, Bounds.Width - 24, 16);
                IndexTextLabel.Frame = new RectangleF(12, 2, 22, 38);
                RightImageView.Frame = new RectangleF(Bounds.Width - 12, 2, 24, 24);
            }
        }
    }
}
