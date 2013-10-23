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
    [Register("MPfmResumePlaybackTableViewCell")]
	public class MPfmResumePlaybackTableViewCell : UITableViewCell
    {
        public bool IsTextAnimationEnabled { get; set; }
        public UILabel LabelArtistName { get; private set; }
        public UILabel LabelAlbumTitle { get; private set; }
        public UILabel LabelSongTitle { get; private set; }
        public UIImageView ImageChevron { get; private set; }
        public UIImageView ImageAlbum { get; private set; }

		public MPfmResumePlaybackTableViewCell() : base()
        {
            Initialize();
        }

		public MPfmResumePlaybackTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

		public MPfmResumePlaybackTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.Default;

            UIView backView = new UIView(Frame);
            backView.BackgroundColor = GlobalTheme.LightColor;
            BackgroundView = backView;
            BackgroundColor = UIColor.White;

            UIView backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;    

            ImageAlbum = new UIImageView();
            ImageAlbum.BackgroundColor = UIColor.DarkGray;
            AddSubview(ImageAlbum);

            TextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            TextLabel.BackgroundColor = UIColor.Clear;
            TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 14);
            TextLabel.TextColor = UIColor.Black;
            TextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            DetailTextLabel.TextColor = UIColor.Gray;
            DetailTextLabel.BackgroundColor = UIColor.Clear;
            DetailTextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            ImageView.Hidden = true;
            ImageView.BackgroundColor = UIColor.Clear;

            // Make sure the text label is over all other subviews
            DetailTextLabel.RemoveFromSuperview();
            ImageView.RemoveFromSuperview();
            AddSubview(DetailTextLabel);
            AddSubview(ImageView);

            LabelArtistName = new UILabel();
            LabelArtistName.BackgroundColor = UIColor.Clear;
            LabelArtistName.Font = UIFont.FromName("HelveticaNeue", 13);
            LabelArtistName.TextColor = UIColor.Black;
            LabelArtistName.LineBreakMode = UILineBreakMode.TailTruncation;
            LabelArtistName.HighlightedTextColor = UIColor.White;
            AddSubview(LabelArtistName);

            LabelAlbumTitle = new UILabel();
            LabelAlbumTitle.BackgroundColor = UIColor.Clear;
            LabelAlbumTitle.Font = UIFont.FromName("HelveticaNeue", 12);
            LabelAlbumTitle.TextColor = UIColor.FromRGBA(0.1f, 0.1f, 0.1f, 1);
            LabelAlbumTitle.LineBreakMode = UILineBreakMode.TailTruncation;
            LabelAlbumTitle.HighlightedTextColor = UIColor.White;
            AddSubview(LabelAlbumTitle);

            LabelSongTitle = new UILabel();
            LabelSongTitle.BackgroundColor = UIColor.Clear;
            LabelSongTitle.Font = UIFont.FromName("HelveticaNeue-Light", 12);
            LabelSongTitle.TextColor = UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 1);
            LabelSongTitle.LineBreakMode = UILineBreakMode.TailTruncation;
            LabelSongTitle.HighlightedTextColor = UIColor.White;
            AddSubview(LabelSongTitle);

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron"));
            ImageChevron.BackgroundColor = UIColor.Clear;
            AddSubview(ImageChevron); 

            // Make sure the text label is over all other subviews
            TextLabel.RemoveFromSuperview();
            AddSubview(TextLabel);
        }

        public override void LayoutSubviews()
        {
            //BackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            SelectedBackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);

            TextLabel.Frame = new RectangleF(12, 6, 300, 20);
            DetailTextLabel.Frame = new RectangleF(12, 24, 300, 20);
            LabelArtistName.Frame = new RectangleF(74, 52, 232, 18);
            LabelAlbumTitle.Frame = new RectangleF(74, 68, 232, 18);
            LabelSongTitle.Frame = new RectangleF(74, 84, 232, 18);
            ImageAlbum.Frame = new RectangleF(12, 50, 54, 54);
            ImageChevron.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 22, 36, 22, 44);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            AnimatePress(true);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);
            base.TouchesCancelled(touches, evt);
        }

        private void AnimatePress(bool on)
        {
            if (!IsTextAnimationEnabled)
                return;

            if (!on)
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    // Ignore when scale is lower; it was done on purpose and will be restored to 1 later.
                    if(TextLabel.Transform.xx < 0.95f) return;

                    TextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    //IndexTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                }, null);
            }
            else
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    TextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    //IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                }, null);
            }
        }
    }
}
