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
        public UILabel LabelLastUpdated { get; private set; }
        public UILabel LabelArtistName { get; private set; }
        public UILabel LabelAlbumTitle { get; private set; }
        public UILabel LabelSongTitle { get; private set; }
        public UIView ViewSeparator { get; private set; }
        public UIView ViewOverlay { get; private set; }
        public UILabel LabelCellDisabled { get; private set; }
        public UIImageView ImageChevron { get; private set; }
        public UIImageView ImageIcon { get; private set; }
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
            ImageAlbum.BackgroundColor = UIColor.Clear;
            AddSubview(ImageAlbum);

            ImageIcon = new UIImageView();
            ImageIcon.BackgroundColor = UIColor.Clear;
            AddSubview(ImageIcon);

            TextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            TextLabel.BackgroundColor = UIColor.Clear;
            TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 14);
            TextLabel.TextColor = UIColor.Black;
            TextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            DetailTextLabel.TextColor = UIColor.DarkGray;
            DetailTextLabel.BackgroundColor = UIColor.Clear;
            DetailTextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            ImageView.Hidden = true;

            // Make sure the text label is over all other subviews
            DetailTextLabel.RemoveFromSuperview();
            ImageView.RemoveFromSuperview();
            AddSubview(DetailTextLabel);
            AddSubview(ImageView);

            LabelLastUpdated = new UILabel();
            LabelLastUpdated.Layer.AnchorPoint = new PointF(0, 0.5f);
            LabelLastUpdated.BackgroundColor = UIColor.Clear;
            LabelLastUpdated.Font = UIFont.FromName("HelveticaNeue-Light", 12);
            LabelLastUpdated.TextColor = UIColor.Gray;
            LabelLastUpdated.LineBreakMode = UILineBreakMode.TailTruncation;
            LabelLastUpdated.HighlightedTextColor = UIColor.White;
            AddSubview(LabelLastUpdated);

            LabelArtistName = new UILabel();
            LabelArtistName.Layer.AnchorPoint = new PointF(0, 0.5f);
            LabelArtistName.BackgroundColor = UIColor.Clear;
            LabelArtistName.Font = UIFont.FromName("HelveticaNeue", 13);
            LabelArtistName.TextColor = UIColor.Black;
            LabelArtistName.LineBreakMode = UILineBreakMode.TailTruncation;
            LabelArtistName.HighlightedTextColor = UIColor.White;
            AddSubview(LabelArtistName);

            LabelAlbumTitle = new UILabel();
            LabelAlbumTitle.Layer.AnchorPoint = new PointF(0, 0.5f);
            LabelAlbumTitle.BackgroundColor = UIColor.Clear;
            LabelAlbumTitle.Font = UIFont.FromName("HelveticaNeue", 12);
            LabelAlbumTitle.TextColor = UIColor.FromRGBA(0.1f, 0.1f, 0.1f, 1);
            LabelAlbumTitle.LineBreakMode = UILineBreakMode.TailTruncation;
            LabelAlbumTitle.HighlightedTextColor = UIColor.White;
            AddSubview(LabelAlbumTitle);

            LabelSongTitle = new UILabel();
            LabelSongTitle.Layer.AnchorPoint = new PointF(0, 0.5f);
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

            ViewOverlay = new UIView();
            ViewOverlay.BackgroundColor = UIColor.FromRGBA(0.85f, 0.85f, 0.85f, 0.95f);
            ViewOverlay.Alpha = 0;
            AddSubview(ViewOverlay);

            ViewSeparator = new UIView();
            ViewSeparator.BackgroundColor = UIColor.FromRGBA(0.75f, 0.75f, 0.75f, 0.5f);
            AddSubview(ViewSeparator);

            LabelCellDisabled = new UILabel();
            LabelCellDisabled.BackgroundColor = UIColor.Clear;
            LabelCellDisabled.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            LabelCellDisabled.TextColor = UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 1);
            LabelCellDisabled.HighlightedTextColor = UIColor.White;
            LabelCellDisabled.TextAlignment = UITextAlignment.Center;
            LabelCellDisabled.Text = "Cannot resume playback from this device because the playlist audio files could not be found on the local hard disk.";
            LabelCellDisabled.Lines = 3;
            LabelCellDisabled.Alpha = 0;
            AddSubview(LabelCellDisabled);
        }

        public override void LayoutSubviews()
        {
            //base.LayoutSubviews();

            BackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            SelectedBackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);

            TextLabel.Frame = new RectangleF(44, 6, Frame.Width - 60, 20);
            DetailTextLabel.Frame = new RectangleF(44, 24, Frame.Width - 60, 20);
            LabelArtistName.Frame = new RectangleF(74, 52, Frame.Width - 90, 18);
            LabelAlbumTitle.Frame = new RectangleF(74, 68, Frame.Width - 90, 18);
            LabelSongTitle.Frame = new RectangleF(74, 84, Frame.Width - 90, 18);
            LabelLastUpdated.Frame = new RectangleF(12, 106, Frame.Width - 24, 20);
            ImageIcon.Frame = new RectangleF(12, 12, 30, 30);
            ImageAlbum.Frame = new RectangleF(12, 50, 54, 54);
            ImageChevron.Frame = new RectangleF(Frame.Width - 22, 43, 22, 44);
            ViewOverlay.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            ViewSeparator.Frame = new RectangleF(0, Frame.Height - 1, Frame.Width, 1);
            LabelCellDisabled.Frame = new RectangleF(12, 0, Frame.Width - 24, Frame.Height);
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
//            if (!IsTextAnimationEnabled)
//                return;

            if (!on)
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    // Ignore when scale is lower; it was done on purpose and will be restored to 1 later.
                    if(TextLabel.Transform.xx < 0.95f) return;

                    TextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    LabelArtistName.Transform = CGAffineTransform.MakeScale(1, 1);
                    LabelAlbumTitle.Transform = CGAffineTransform.MakeScale(1, 1);
                    LabelSongTitle.Transform = CGAffineTransform.MakeScale(1, 1);
                    LabelLastUpdated.Transform = CGAffineTransform.MakeScale(1, 1);
                    ImageIcon.Transform = CGAffineTransform.MakeScale(1, 1);
                    ImageAlbum.Transform = CGAffineTransform.MakeScale(1, 1);
                }, null);
            }
            else
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    TextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    LabelArtistName.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    LabelAlbumTitle.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    LabelSongTitle.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    LabelLastUpdated.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    ImageIcon.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    ImageAlbum.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                }, null);
            }
        }
    }
}
