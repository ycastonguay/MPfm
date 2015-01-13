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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;
using Sessions.iOS.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsEqualizerTableViewCell")]
    public class SessionsEqualizerTableViewCell : UITableViewCell
    {
        private const float GraphViewWidth = 60f;
        private const float GraphViewPadding = 4f;

        private bool _isTextLabelAllowedToChangeFrame = true;

		public UIView ContainerView { get; private set; }
		public UIView ContainerBackgroundView { get; private set; }

        public UILabel TitleTextLabel { get; private set; }
        public UILabel SubtitleTextLabel { get; private set; }
        public UILabel IndexTextLabel { get; private set; }
        public SessionsEqualizerPresetGraphView GraphView { get; private set; }

		public SessionsSecondaryMenuButton PlayButton { get; set; }
		public SessionsSecondaryMenuButton AddButton { get; set; }
		public SessionsSecondaryMenuButton DeleteButton { get; set; }

		public bool IsQueued { get; set; }
        public bool IsTextAnimationEnabled { get; set; }
		public bool IsDarkBackground { get; set; }

        public float RightOffset { get; set; }

        public delegate void RightButtonTap(SessionsEqualizerTableViewCell cell);
        public event RightButtonTap OnRightButtonTap;

        public SessionsEqualizerTableViewCell() : base()
        {
            Initialize();
        }

		// Keep this or cell reuse won't work for the first items
        public SessionsEqualizerTableViewCell(IntPtr handle) : base(handle)
		{
			Initialize();
		}

        public SessionsEqualizerTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        public SessionsEqualizerTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public void Initialize()
        {
            IsTextAnimationEnabled = false;
            SelectionStyle = UITableViewCellSelectionStyle.None;
           
			ContainerView = new UIView(Bounds);
			ContainerView.BackgroundColor = UIColor.Clear;
			AddSubview(ContainerView);

			ContainerBackgroundView = new UIView(Bounds);
			ContainerBackgroundView.BackgroundColor = UIColor.White;
			ContainerView.AddSubview(ContainerBackgroundView);

            UIView backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;     
            SelectedBackgroundView.Hidden = true;
			ContainerView.AddSubview(SelectedBackgroundView);

            TitleTextLabel = new UILabel();
            TitleTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            TitleTextLabel.BackgroundColor = UIColor.Clear;
            TitleTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            TitleTextLabel.TextColor = UIColor.Black;
            TitleTextLabel.HighlightedTextColor = UIColor.White;
            SubtitleTextLabel = new UILabel();
            SubtitleTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			SubtitleTextLabel.TextColor = UIColor.Gray;
			SubtitleTextLabel.HighlightedTextColor = UIColor.White;
            SubtitleTextLabel.BackgroundColor = UIColor.Clear;
            SubtitleTextLabel.Font = UIFont.FromName("HelveticaNeue", 12);
            ImageView.BackgroundColor = UIColor.Clear;

            // Make sure the text label is over all other subviews
            ImageView.RemoveFromSuperview();
			ContainerView.AddSubview(SubtitleTextLabel);
			ContainerView.AddSubview(ImageView);

            IndexTextLabel = new UILabel();
            IndexTextLabel.BackgroundColor = UIColor.Clear;
            IndexTextLabel.Font = UIFont.FromName("HelveticaNeue", 16);
            IndexTextLabel.TextColor = UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);
            IndexTextLabel.TextAlignment = UITextAlignment.Center;
            IndexTextLabel.HighlightedTextColor = UIColor.White;
			ContainerView.AddSubview(IndexTextLabel);

            GraphView = new SessionsEqualizerPresetGraphView(new RectangleF(0, 0, 60, 40));
            GraphView.ShowText = false;
            GraphView.BackgroundColor = UIColor.White;
            GraphView.ColorForeground = new BasicColor(0, 150, 0);
            GraphView.ColorBackground = new BasicColor(255, 255, 255);
            //GraphView.ColorMainLine = new BasicColor(150, 255, 0);
            GraphView.ColorMainLine = new BasicColor(50, 50, 50);
            GraphView.ColorSecondaryLine = new BasicColor(240, 240, 240);

            ContainerView.AddSubview(GraphView);

            // Make sure the text label is over all other subviews
            //TextLabel.RemoveFromSuperview();
			//ContainerView.AddSubview(TextLabel);
            ContainerView.AddSubview(TitleTextLabel);

			// Maybe add icons only to iPad where there is enough space
			PlayButton = new SessionsSecondaryMenuButton();
			PlayButton.Frame = new RectangleF(4, 53, 100, 64);
			PlayButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/play"), UIControlState.Normal);
			PlayButton.SetTitle("Play", UIControlState.Normal); 
			PlayButton.Font = UIFont.FromName("HelveticaNeue-Light", 12f);
            PlayButton.Alpha = 0;
			ContainerView.AddSubview(PlayButton);

			AddButton = new SessionsSecondaryMenuButton();
			AddButton.Frame = new RectangleF(108, 53, 100, 64);
			AddButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/add"), UIControlState.Normal);
			AddButton.SetTitle("Add to playlist", UIControlState.Normal);
			AddButton.Font = UIFont.FromName("HelveticaNeue-Light", 12f);
            AddButton.Alpha = 0;
			ContainerView.AddSubview(AddButton);

			DeleteButton = new SessionsSecondaryMenuButton();
			DeleteButton.Frame = new RectangleF(212, 53, 100, 64);
			DeleteButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/trash"), UIControlState.Normal);
			DeleteButton.SetTitle("Delete", UIControlState.Normal);
			DeleteButton.Font = UIFont.FromName("HelveticaNeue-Light", 12f);
            DeleteButton.Alpha = 0;
			ContainerView.AddSubview(DeleteButton);
        }

        private void HandleRightButtonTouchUpInside(object sender, EventArgs e)
        {
            if (OnRightButtonTap != null)
                OnRightButtonTap(this);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            //BackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height - 1);
			ContainerView.Frame = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
			ContainerBackgroundView.Frame = new RectangleF(IsQueued ? 4 : 0, 0, Bounds.Width, Bounds.Height);
            SelectedBackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            GraphView.Frame = new RectangleF(Bounds.Width - GraphViewPadding - GraphViewWidth, GraphViewPadding, GraphViewWidth, Bounds.Height - GraphViewPadding * 2);

            float padding = 8;

            // Determine width available for text
            float textWidth = Bounds.Width;
            if (Accessory != UITableViewCellAccessory.None)
                textWidth -= 44;
            if (ImageView.Image != null && !ImageView.Hidden)
                textWidth -= 44 + padding;
            if (!string.IsNullOrEmpty(IndexTextLabel.Text))
                textWidth -= 22 + padding + padding;

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
                x += padding + (padding / 2);
            }

            float titleY = 10 + 4;
            if (!string.IsNullOrEmpty(SubtitleTextLabel.Text))
                titleY = 2 + 4;

			//if (_isTextLabelAllowedToChangeFrame)
                TitleTextLabel.Frame = new RectangleF(x, titleY, textWidth, 22);
            if (!string.IsNullOrEmpty(SubtitleTextLabel.Text))
                SubtitleTextLabel.Frame = new RectangleF(x, 22 + 4, textWidth, 16);

//			PlayButton.Frame = new RectangleF(4, 56, 100, 64);
//			AddButton.Frame = new RectangleF(108, 56, 100, 64);
//			DeleteButton.Frame = new RectangleF(212, 56, 100, 64);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
            SelectedBackgroundView.Alpha = 1;
            SelectedBackgroundView.Hidden = !highlighted;
			TitleTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Black;
            SubtitleTextLabel.Highlighted = highlighted;
            IndexTextLabel.Highlighted = highlighted;
			SubtitleTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Gray;
			IndexTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);

            base.SetHighlighted(highlighted, animated);
        }

        public override void SetSelected(bool selected, bool animated)
        {
            TitleTextLabel.TextColor = selected ? UIColor.White : UIColor.Black;
            SubtitleTextLabel.TextColor = selected ? UIColor.White : UIColor.Gray;
            //IndexTextLabel.TextColor = selected ? UIColor.White : UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);

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

            _isTextLabelAllowedToChangeFrame = !on;
            if (!on)
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    // Ignore when scale is lower; it was done on purpose and will be restored to 1 later.
                    if(TitleTextLabel.Transform.xx < 0.95f) return;

                    TitleTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    SubtitleTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    IndexTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                }, null);
            }
            else
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    TitleTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    SubtitleTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                }, null);
            }
        }
    }
}