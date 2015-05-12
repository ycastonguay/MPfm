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
using Sessions.iOS.Classes.Controls.Cells.Base;
using Sessions.iOS.Classes.Controls.Buttons;
using org.sessionsapp.player;

namespace Sessions.iOS.Classes.Controls.Cells
{
	[Register("SessionsLoopTableViewCell")]
    public class SessionsLoopTableViewCell : SessionsBaseExpandableTableViewCell
    {
        private float _sliderValue;
    
		public delegate void LongPressLoop(Guid loopId);
        public delegate void DeleteLoop(Guid loopId);
        public delegate void PunchInLoop(Guid loopId, SSPLoopSegmentType segmentType);
        public delegate void ChangeLoopPosition(Guid loopId, SSPLoopSegmentType segmentType, float newPositionPercentage);
        public delegate void ChangeLoopName(Guid loopId, string newName);
        public delegate void SetLoopPosition(Guid loopId, SSPLoopSegmentType segmentType, float newPositionPercentage);
		public event LongPressLoop OnLongPressLoop;
		public event DeleteLoop OnDeleteLoop;
		public event PunchInLoop OnPunchInLoop;
		public event ChangeLoopPosition OnChangeLoopPosition;
		public event ChangeLoopName OnChangeLoopName;
		public event SetLoopPosition OnSetLoopPosition;

        public UILabel IndexTextLabel { get; private set; }
        public UILabel TitleTextLabel { get; private set; }
        public UILabel StartPositionTextLabel { get; private set; }
        public UILabel EndPositionTextLabel { get; private set; }
		public SessionsSemiTransparentRoundButton DeleteButton { get; set; }
		public SessionsSemiTransparentRoundButton StartPositionPunchInButton { get; set; }
        public SessionsSemiTransparentRoundButton EndPositionPunchInButton { get; set; }
        public UISlider StartPositionSlider { get; private set; }
        public UISlider EndPositionSlider { get; private set; }
        public UITextField TextField { get; private set; }
		public UILabel StartPositionTitleLabel { get; private set; }
        public UILabel EndPositionTitleLabel { get; private set; }

        public float RightOffset { get; set; }
		public Guid LoopId { get; set; }

        public override bool UseContainerView { get { return false; } }

        public SessionsLoopTableViewCell() : base()
        {
        }

        public SessionsLoopTableViewCell(RectangleF frame) : base(frame)
        {
        }

        public SessionsLoopTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsTextAnimationEnabled = true;

            TextLabel.RemoveFromSuperview();
            DetailTextLabel.RemoveFromSuperview();

			var longPress = new UILongPressGestureRecognizer(HandleLongPress);
			longPress.MinimumPressDuration = 0.7f;
			longPress.WeakDelegate = this;
			BackgroundView.AddGestureRecognizer(longPress);

            TitleTextLabel = new UILabel();
            TitleTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            TitleTextLabel.BackgroundColor = UIColor.Clear;
            TitleTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            TitleTextLabel.TextColor = UIColor.White;
            TitleTextLabel.HighlightedTextColor = UIColor.White;
            TitleTextLabel.Alpha = 1;

            StartPositionTextLabel = new UILabel();
            StartPositionTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            StartPositionTextLabel.Font = UIFont.FromName("HelveticaNeue", 15);
            StartPositionTextLabel.TextColor = UIColor.White;
            StartPositionTextLabel.HighlightedTextColor = UIColor.White;
            StartPositionTextLabel.TextAlignment = UITextAlignment.Right;

            EndPositionTextLabel = new UILabel();
            EndPositionTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            EndPositionTextLabel.Font = UIFont.FromName("HelveticaNeue", 15);
            EndPositionTextLabel.TextColor = UIColor.White;
            EndPositionTextLabel.HighlightedTextColor = UIColor.White;
            EndPositionTextLabel.TextAlignment = UITextAlignment.Right;

            // Make sure the text label is over all other subviews
            ImageView.RemoveFromSuperview();
            AddView(TitleTextLabel);
            AddView(StartPositionTextLabel);
            AddView(EndPositionTextLabel);
            AddView(ImageView);

			TextField = new UITextField();
			TextField.Layer.CornerRadius = 8;
			TextField.Alpha = 0;
			TextField.BackgroundColor = UIColor.FromRGBA(0.8f, 0.8f, 0.8f, 0.075f);
			TextField.Font = UIFont.FromName("HelveticaNeue-Light", 16);
			TextField.TextColor = UIColor.White;
			TextField.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			TextField.ReturnKeyType = UIReturnKeyType.Done;
            AddView(TextField);

			StartPositionTitleLabel = new UILabel();
            StartPositionTitleLabel.Text = "Start Position";
            StartPositionTitleLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            StartPositionTitleLabel.TextColor = UIColor.FromRGB(0.8f, 0.8f, 0.8f);
            StartPositionTitleLabel.Alpha = 0;
            AddView(StartPositionTitleLabel);

            EndPositionTitleLabel = new UILabel();
            EndPositionTitleLabel.Text = "End Position";
            EndPositionTitleLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            EndPositionTitleLabel.TextColor = UIColor.FromRGB(0.8f, 0.8f, 0.8f);
            EndPositionTitleLabel.Alpha = 0;
            AddView(EndPositionTitleLabel);

			// Add padding to text field
			var paddingView = new UIView(new RectangleF(0, 0, 4, 20));
			TextField.LeftView = paddingView;
			TextField.LeftViewMode = UITextFieldViewMode.Always;

			// Make sure the Done key closes the keyboard
			TextField.ShouldReturn = (a) => {
				if(OnChangeLoopName != null)
					OnChangeLoopName(LoopId, TextField.Text);

                TitleTextLabel.Text = TextField.Text;
				TextField.ResignFirstResponder();
				return true;
			};

            IndexTextLabel = new UILabel();
			IndexTextLabel.BackgroundColor = UIColor.FromRGBA(0, 0, 1, 0.7f);
			IndexTextLabel.Font = UIFont.FromName("HelveticaNeue", 16);
			IndexTextLabel.TextColor = UIColor.White;
            IndexTextLabel.TextAlignment = UITextAlignment.Center;
            IndexTextLabel.HighlightedTextColor = UIColor.White;
            AddView(IndexTextLabel);

			DeleteButton = new SessionsSemiTransparentRoundButton();
			DeleteButton.Alpha = 0;
			DeleteButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/remove");
			DeleteButton.TouchUpInside += HandleOnDeleteButtonClick;
            AddView(DeleteButton);

			StartPositionPunchInButton = new SessionsSemiTransparentRoundButton();
            StartPositionPunchInButton.Alpha = 0;
            StartPositionPunchInButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/punch_in");
            StartPositionPunchInButton.TouchUpInside += HandleOnStartPositionPunchInButtonClick;
            AddView(StartPositionPunchInButton);

            EndPositionPunchInButton = new SessionsSemiTransparentRoundButton();
            EndPositionPunchInButton.Alpha = 0;
            EndPositionPunchInButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/punch_in");
            EndPositionPunchInButton.TouchUpInside += HandleOnEndPositionPunchInButtonClick;
            AddView(EndPositionPunchInButton);

			StartPositionSlider = new UISlider(new RectangleF(0, 0, 10, 10));
            StartPositionSlider.ExclusiveTouch = true;
            StartPositionSlider.Alpha = 0;
            StartPositionSlider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            StartPositionSlider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            StartPositionSlider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider_gray").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            StartPositionSlider.ValueChanged += (sender, e) =>
			{
                _sliderValue = StartPositionSlider.Value;
                OnChangeLoopPosition(LoopId, SSPLoopSegmentType.Start, StartPositionSlider.Value);
			};
            StartPositionSlider.TouchDown += (sender, e) => {
				// There's a bug in UISlider inside a UITableView inside a UIScrollView; the table view offset will change when changing slider value
				var tableView = (SessionsTableView)GetTableView();
				tableView.BlockContentOffsetChange = true;
			};
            StartPositionSlider.TouchUpInside += (sender, e) => {
				var tableView = (SessionsTableView)GetTableView();
				tableView.BlockContentOffsetChange = false;

				// Take the last value from ValueChanged to prevent getting a slightly different value when the finger leaves the screen
				OnSetLoopPosition(LoopId, SSPLoopSegmentType.Start, _sliderValue); 
			};
            AddView(StartPositionSlider);

            EndPositionSlider = new UISlider(new RectangleF(0, 0, 10, 10));
            EndPositionSlider.ExclusiveTouch = true;
            EndPositionSlider.Alpha = 0;
            EndPositionSlider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            EndPositionSlider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            EndPositionSlider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider_gray").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            EndPositionSlider.ValueChanged += (sender, e) =>
            {
                _sliderValue = EndPositionSlider.Value;
                OnChangeLoopPosition(LoopId, SSPLoopSegmentType.End, EndPositionSlider.Value);
            };
            EndPositionSlider.TouchDown += (sender, e) => {
                // There's a bug in UISlider inside a UITableView inside a UIScrollView; the table view offset will change when changing slider value
                var tableView = (SessionsTableView)GetTableView();
                tableView.BlockContentOffsetChange = true;
            };
            EndPositionSlider.TouchUpInside += (sender, e) => {
                var tableView = (SessionsTableView)GetTableView();
                tableView.BlockContentOffsetChange = false;

                // Take the last value from ValueChanged to prevent getting a slightly different value when the finger leaves the screen
                OnSetLoopPosition(LoopId, SSPLoopSegmentType.End, _sliderValue); 
            };
            AddView(EndPositionSlider);
        }

		private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State != UIGestureRecognizerState.Began)
				return;

			if (OnLongPressLoop != null)
				OnLongPressLoop(LoopId);
		}

		private void HandleOnDeleteButtonClick(object sender, EventArgs e)
		{
			if (OnDeleteLoop != null)
				OnDeleteLoop(LoopId);
		}

		private void HandleOnStartPositionPunchInButtonClick(object sender, EventArgs e)
		{
			if (OnPunchInLoop != null)
				OnPunchInLoop(LoopId, SSPLoopSegmentType.Start);
		}

        private void HandleOnEndPositionPunchInButtonClick(object sender, EventArgs e)
        {
            if (OnPunchInLoop != null)
                OnPunchInLoop(LoopId, SSPLoopSegmentType.End);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			const float leftPadding = 8;
            const float topPadding = 6;
            const float buttonSize = 44;
            const float textHeight = 38;
            const float sliderAndTextHeight = 68;

            float x = leftPadding;
            IndexTextLabel.Frame = new RectangleF(x, 6, 22, textHeight);
            x += 22 + leftPadding;

			if (IsTextLabelAllowedToChangeFrame)
			{
                float positionTextLabelX = Bounds.Width - 128;
				if (StartPositionPunchInButton.Alpha > 0)
                    positionTextLabelX -= 48;

                StartPositionTextLabel.Frame = new RectangleF(positionTextLabelX, topPadding, 120, textHeight);
                EndPositionTextLabel.Frame = new RectangleF(positionTextLabelX, topPadding, 120, textHeight);

                StartPositionTitleLabel.Frame = new RectangleF(leftPadding, topPadding + 35, Bounds.Width, textHeight);
                EndPositionTitleLabel.Frame = new RectangleF(leftPadding, topPadding + 35 + sliderAndTextHeight, Bounds.Width, textHeight);

                TitleTextLabel.Frame = new RectangleF(x, topPadding, Bounds.Width - 120, textHeight);
			}

			TextField.Frame = new RectangleF(x - 4, topPadding + 1, Bounds.Width - 120 - 48, 38);

            StartPositionSlider.Frame = new RectangleF(leftPadding, textHeight + 34, Bounds.Width - 12 - buttonSize - 12, 36);
            EndPositionSlider.Frame = new RectangleF(leftPadding, textHeight + 34 + sliderAndTextHeight, Bounds.Width - 12 - buttonSize - 12, 36);

            DeleteButton.Frame = new RectangleF(Bounds.Width - buttonSize, topPadding, buttonSize, buttonSize);

            StartPositionPunchInButton.Frame = new RectangleF(Bounds.Width - buttonSize, 68, buttonSize, buttonSize);
            EndPositionPunchInButton.Frame = new RectangleF(Bounds.Width - buttonSize, 68 + sliderAndTextHeight, buttonSize, buttonSize);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
            base.SetHighlighted(highlighted, animated);

            StartPositionTextLabel.Highlighted = highlighted;
            EndPositionTextLabel.Highlighted = highlighted;
            IndexTextLabel.Highlighted = highlighted;
        }

		protected override void CollapseCell()
		{
            base.CollapseCell();

            TitleTextLabel.Text = TextField.Text;
			TextField.ResignFirstResponder();
		}

        protected override void SetControlVisibilityForExpand(bool isExpanded)
        {
            TextField.Alpha = isExpanded ? 1 : 0;
            TitleTextLabel.Alpha = isExpanded ? 0 : 1;
            StartPositionTitleLabel.Alpha = isExpanded ? 1 : 0;
            EndPositionTitleLabel.Alpha = isExpanded ? 1 : 0;
            StartPositionSlider.Alpha = isExpanded ? 1 : 0;
            EndPositionSlider.Alpha = isExpanded ? 1 : 0;
            DeleteButton.Alpha = isExpanded ? 1 : 0;
            StartPositionPunchInButton.Alpha = isExpanded ? 1 : 0;
            EndPositionPunchInButton.Alpha = isExpanded ? 1 : 0;

            float padding = isExpanded ? 0 : 48f;
            StartPositionTextLabel.Frame = new RectangleF(Bounds.Width - 128 - padding, 6, 120, 38);
            EndPositionTextLabel.Frame = new RectangleF(Bounds.Width - 128 - padding, 6, 120, 38);
        }

        protected override void SetControlScaleForTouchAnimation(float scale)
        {
            TitleTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
            StartPositionTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
            EndPositionTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
            IndexTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
        }

//		public override UIView HitTest(PointF point, UIEvent uievent)
//		{
//			Tracing.Log("MarkerTableViewCell - HitTest - point: {0} eventType: {1}", point, uievent.Type);
//			return base.HitTest(point, uievent);
//
////			var thumbFrame = GetThumbRect();
////			if (thumbFrame.Contains(point))
////			{
////				Tracing.Log("Slider - HitTest (inside thumb) - point: {0} eventType: {1}", point, uievent.Type);
////				return base.HitTest(point, uievent);
////			}
////			else
////			{
////				Tracing.Log("Slider - HitTest (outside thumb) - point: {0} eventType: {1}", point, uievent.Type);
////				return Superview.HitTest(point, uievent);
////			}
//		}
//
//		public override bool PointInside(PointF point, UIEvent uievent)
//		{
//			bool value = base.PointInside(point, uievent);
//			Tracing.Log("MarkerTableViewCell - PointInside - point: {0} eventType: {1} baseValue: {2}", point, uievent.Type, value);
//			foreach (var view in Subviews)
//			{
//				Tracing.Log(".. MarkerTableViewCell - PointInside - subview: {0}", view.GetType().FullName);
////				if (!view.Hidden && view.UserInteractionEnabled && view.PointInside(ConvertPointToView(point, view), uievent))
////				{
////					Tracing.Log("Slider - PointInside TRUE - point: {0} eventType: {1} baseValue: {2}", point, uievent.Type, value);
////					return true;
////				}
//			}
////			Tracing.Log("Slider - PointInside FALSE - point: {0} eventType: {1} baseValue: {2}", point, uievent.Type, value);
////			return false;
//
//			return value;
//		}
    }
}
