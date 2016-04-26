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
using Sessions.GenericControls.Basics;
using Sessions.iOS.Classes.Controls.Buttons;
using Sessions.iOS.Classes.Controls.Cells.Base;

namespace Sessions.iOS.Classes.Controls.Cells
{
    [Register("SessionsEqualizerTableViewCell")]
    public class SessionsEqualizerTableViewCell : SessionsBaseTableViewCell
    {
        private const float GraphViewWidth = 60f;
        private const float GraphViewPadding = 4f;
        private const float CheckmarkWidth = 26f;//32f; //44f;
        public const float StandardCellHeight = 52f;
        public const float ExpandedCellHeight = 126f;

        public UILabel TitleTextLabel { get; private set; }
        public UILabel SubtitleTextLabel { get; private set; }
        public SessionsRoundImageView CheckmarkImageView { get; private set; }
        public SessionsEqualizerPresetGraphView GraphView { get; private set; }

		public SessionsSecondaryMenuButton EditButton { get; set; }
		public SessionsSecondaryMenuButton DuplicateButton { get; set; }
		public SessionsSecondaryMenuButton DeleteButton { get; set; }

        private bool _isPresetSelected;
        public bool IsPresetSelected
        {
            get 
            { 
                return _isPresetSelected; 
            }
            set 
            { 
                _isPresetSelected = value;
                CheckmarkImageView.Hidden = !_isPresetSelected;
                SetNeedsLayout();
            }
        }

		public bool IsDarkBackground { get; set; }

        public override bool UseContainerView { get { return true; } }

        public delegate void RightButtonTap(SessionsEqualizerTableViewCell cell);
        public event RightButtonTap OnRightButtonTap;

        public override UIEdgeInsets LayoutMargins
        {
            get
            {
                return UIEdgeInsets.Zero;
            }
            set
            {
            }
        }

        public SessionsEqualizerTableViewCell() : base()
        {
        }

        public SessionsEqualizerTableViewCell(IntPtr handle) : base(handle)
		{
		}

        public SessionsEqualizerTableViewCell(CGRect frame) : base(frame)
        {
        }

        public SessionsEqualizerTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsTextAnimationEnabled = true;

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                SeparatorInset = UIEdgeInsets.Zero;
                LayoutMargins = UIEdgeInsets.Zero;
            }
           
            TitleTextLabel = new UILabel();
            TitleTextLabel.Layer.AnchorPoint = new CGPoint(0, 0.5f);
            TitleTextLabel.BackgroundColor = UIColor.Clear;
            TitleTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            TitleTextLabel.TextColor = UIColor.Black;
            TitleTextLabel.HighlightedTextColor = UIColor.White;

            SubtitleTextLabel = new UILabel();
            SubtitleTextLabel.Layer.AnchorPoint = new CGPoint(0, 0.5f);
			SubtitleTextLabel.TextColor = UIColor.Gray;
			SubtitleTextLabel.HighlightedTextColor = UIColor.White;
            SubtitleTextLabel.BackgroundColor = UIColor.Clear;
            SubtitleTextLabel.Font = UIFont.FromName("HelveticaNeue", 12);
            ImageView.BackgroundColor = UIColor.Clear;

            // Make sure the text label is over all other subviews
            ImageView.RemoveFromSuperview();
			AddView(SubtitleTextLabel);
            AddView(ImageView);

            CheckmarkImageView = new SessionsRoundImageView();
            CheckmarkImageView.Hidden = true;
            CheckmarkImageView.BackgroundColor = UIColor.Clear;
            CheckmarkImageView.GlyphImageView.Image = UIImage.FromBundle("Images/ContextualButtons/checkmark_nobg");
            AddView(CheckmarkImageView);

            GraphView = new SessionsEqualizerPresetGraphView(new CGRect(0, 0, 60, 40));
            GraphView.ShowText = false;
            GraphView.ShowGuideLines = false;
            GraphView.BackgroundColor = UIColor.Clear;
            GraphView.ColorForeground = new BasicColor(0, 150, 0);
            GraphView.ColorBackground = new BasicColor(255, 255, 255, 0);
            GraphView.ColorMainLine = new BasicColor(50, 50, 50);
            GraphView.ColorSecondaryLine = new BasicColor(240, 240, 240);
            AddView(GraphView);

            // Make sure the text label is over all other subviews
            AddView(TitleTextLabel);

			// Maybe add icons only to iPad where there is enough space
			EditButton = new SessionsSecondaryMenuButton();
			EditButton.Frame = new CGRect(4, 53, 100, 64);
			EditButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/play"), UIControlState.Normal);
			EditButton.SetTitle("Edit", UIControlState.Normal); 
			EditButton.Font = UIFont.FromName("HelveticaNeue-Light", 12f);
            EditButton.Alpha = 0;
            AddView(EditButton);

			DuplicateButton = new SessionsSecondaryMenuButton();
			DuplicateButton.Frame = new CGRect(108, 53, 100, 64);
			DuplicateButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/add"), UIControlState.Normal);
			DuplicateButton.SetTitle("Duplicate", UIControlState.Normal);
			DuplicateButton.Font = UIFont.FromName("HelveticaNeue-Light", 12f);
            DuplicateButton.Alpha = 0;
            AddView(DuplicateButton);

			DeleteButton = new SessionsSecondaryMenuButton();
			DeleteButton.Frame = new CGRect(212, 53, 100, 64);
			DeleteButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/trash"), UIControlState.Normal);
			DeleteButton.SetTitle("Delete", UIControlState.Normal);
			DeleteButton.Font = UIFont.FromName("HelveticaNeue-Light", 12f);
            DeleteButton.Alpha = 0;
            AddView(DeleteButton);
        }

        private void HandleRightButtonTouchUpInside(object sender, EventArgs e)
        {
            if (OnRightButtonTap != null)
                OnRightButtonTap(this);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            const float padding = 8;

            ContainerBackgroundView.Frame = Bounds;
            GraphView.Frame = new CGRect(Bounds.Width - GraphViewPadding - GraphViewWidth, GraphViewPadding, GraphViewWidth, 44);

            // Determine width available for text
            nfloat textWidth = Bounds.Width;
            if (Accessory != UITableViewCellAccessory.None)
                textWidth -= 44;
            if (ImageView.Image != null && !ImageView.Hidden)
                textWidth -= 44 + padding;

            float x = 0;
            if (ImageView.Image != null)
            {
                ImageView.Frame = new CGRect(x, 4, 44, 44);
                x += 44 + padding;
            }
            else if (!CheckmarkImageView.Hidden)
            {
                x += padding;
                CheckmarkImageView.Frame = new CGRect(x, (StandardCellHeight - CheckmarkWidth) / 2, CheckmarkWidth, CheckmarkWidth);                
                x += CheckmarkWidth + padding;
            }
            else
            {
                x += padding + (padding / 2);
            }

            float titleY = 10 + 4;
            if (!string.IsNullOrEmpty(SubtitleTextLabel.Text))
                titleY = 2 + 4;

            TitleTextLabel.Frame = new CGRect(x, titleY, textWidth, 22);
            if (!string.IsNullOrEmpty(SubtitleTextLabel.Text))
                SubtitleTextLabel.Frame = new CGRect(x, 22 + 4, textWidth, 16);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
            Console.WriteLine("EQTableViewCell - SetHighlighted - highlighted: {0}", highlighted);

            SelectedBackgroundView.Alpha = 1;
            SelectedBackgroundView.Hidden = !highlighted;
			TitleTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Black;
            SubtitleTextLabel.Highlighted = highlighted;
			SubtitleTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Gray;
            
            GraphView.ColorMainLine = highlighted ? BasicColors.White : IsDarkBackground ? BasicColors.White : BasicColors.Black;
            GraphView.SetNeedsDisplay();

            base.SetHighlighted(highlighted, animated);
        }

        public override void SetSelected(bool selected, bool animated)
        {
            Console.WriteLine("EQTableViewCell - SetSelected - selected: {0}", selected);

            TitleTextLabel.TextColor = selected ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Black;
            SubtitleTextLabel.TextColor = selected ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Gray;

            GraphView.ColorMainLine = selected ? BasicColors.White : IsDarkBackground ? BasicColors.White : BasicColors.Black;
            GraphView.SetNeedsDisplay();

            if (!selected)
            {
                UIView.Animate(0.5, () => {
                    SelectedBackgroundView.Alpha = 0;
                }, () => {
                    SelectedBackgroundView.Hidden = true;
                });
            }
            else
            {
                SelectedBackgroundView.Hidden = false;
                SelectedBackgroundView.Alpha = 1;
            }

            base.SetSelected(selected, animated);
        }

        protected override void SetControlScaleForTouchAnimation(float scale)
        {
            TitleTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
            SubtitleTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
        }
    }
}
