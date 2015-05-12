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

namespace Sessions.iOS.Classes.Controls.Cells
{
	[Register("SessionsMarkerTableViewCell")]
    public class SessionsMarkerTableViewCell : SessionsBaseExpandableTableViewCell
    {
        private float _sliderValue;
    
		public delegate void LongPressMarker(Guid markerId);
		public delegate void DeleteMarker(Guid markerId);
		public delegate void PunchInMarker(Guid markerId);
		public delegate void UndoMarker(Guid markerId);
		public delegate void ChangeMarkerPosition(Guid markerId, float newPositionPercentage);
		public delegate void ChangeMarkerName(Guid markerId, string newName);
		public delegate void SetMarkerPosition(Guid markerId, float newPositionPercentage);
		public event LongPressMarker OnLongPressMarker;
		public event DeleteMarker OnDeleteMarker;
		public event PunchInMarker OnPunchInMarker;
		public event ChangeMarkerPosition OnChangeMarkerPosition;
		public event ChangeMarkerName OnChangeMarkerName;
		public event SetMarkerPosition OnSetMarkerPosition;

        public UILabel IndexTextLabel { get; private set; }
        public UILabel TitleTextLabel { get; private set; }
        public UILabel PositionTextLabel { get; private set; }
		public SessionsSemiTransparentRoundButton DeleteButton { get; set; }
		public SessionsSemiTransparentRoundButton PunchInButton { get; set; }
        public UISlider Slider { get; private set; }
        public UITextField TextField { get; private set; }
		public UILabel PositionTitleLabel { get; private set; }

        public float RightOffset { get; set; }
		public Guid MarkerId { get; set; }

        public override bool UseContainerView { get { return false; } }

        public SessionsMarkerTableViewCell() : base()
        {
        }

        public SessionsMarkerTableViewCell(RectangleF frame) : base(frame)
        {
        }

        public SessionsMarkerTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
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

            PositionTextLabel = new UILabel();
            PositionTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			PositionTextLabel.Font = UIFont.FromName("HelveticaNeue", 15);
			PositionTextLabel.TextColor = UIColor.White;
			PositionTextLabel.HighlightedTextColor = UIColor.White;
			PositionTextLabel.TextAlignment = UITextAlignment.Right;

            // Make sure the text label is over all other subviews
            ImageView.RemoveFromSuperview();
            AddView(TitleTextLabel);
            AddView(PositionTextLabel);
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

			PositionTitleLabel = new UILabel();
			PositionTitleLabel.Text = "Position";
			PositionTitleLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
			PositionTitleLabel.TextColor = UIColor.FromRGB(0.8f, 0.8f, 0.8f);
            PositionTitleLabel.Alpha = 0;
            AddView(PositionTitleLabel);

			// Add padding to text field
			var paddingView = new UIView(new RectangleF(0, 0, 4, 20));
			TextField.LeftView = paddingView;
			TextField.LeftViewMode = UITextFieldViewMode.Always;

			// Make sure the Done key closes the keyboard
			TextField.ShouldReturn = (a) => {
				if(OnChangeMarkerName != null)
					OnChangeMarkerName(MarkerId, TextField.Text);

                TitleTextLabel.Text = TextField.Text;
				TextField.ResignFirstResponder();
				return true;
			};

            IndexTextLabel = new UILabel();
			IndexTextLabel.BackgroundColor = UIColor.FromRGBA(1, 0, 0, 0.7f);
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

			PunchInButton = new SessionsSemiTransparentRoundButton();
			PunchInButton.Alpha = 0;
			PunchInButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/punch_in");
			PunchInButton.TouchUpInside += HandleOnPunchInButtonClick;
            AddView(PunchInButton);

			Slider = new UISlider(new RectangleF(0, 0, 10, 10));
			Slider.ExclusiveTouch = true;
			Slider.Alpha = 0;
			Slider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
			Slider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
			Slider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider_gray").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
			Slider.ValueChanged += (sender, e) =>
			{
				_sliderValue = Slider.Value;
				OnChangeMarkerPosition(MarkerId, Slider.Value);
			};
			Slider.TouchDown += (sender, e) => {
				// There's a bug in UISlider inside a UITableView inside a UIScrollView; the table view offset will change when changing slider value
				var tableView = (SessionsTableView)GetTableView();
				tableView.BlockContentOffsetChange = true;
			};
			Slider.TouchUpInside += (sender, e) => {
				var tableView = (SessionsTableView)GetTableView();
				tableView.BlockContentOffsetChange = false;

				// Take the last value from ValueChanged to prevent getting a slightly different value when the finger leaves the screen
				OnSetMarkerPosition(MarkerId, _sliderValue); 
			};
            AddView(Slider);
        }

		private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State != UIGestureRecognizerState.Began)
				return;

			if (OnLongPressMarker != null)
				OnLongPressMarker(MarkerId);
		}

		private void HandleOnDeleteButtonClick(object sender, EventArgs e)
		{
			if (OnDeleteMarker != null)
				OnDeleteMarker(MarkerId);
		}

		private void HandleOnPunchInButtonClick(object sender, EventArgs e)
		{
			if (OnPunchInMarker != null)
				OnPunchInMarker(MarkerId);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			const float leftPadding = 8;
            const float topPadding = 6;
            const float buttonSize = 44;
            const float textHeight = 38;

            float x = leftPadding;
            IndexTextLabel.Frame = new RectangleF(x, 6, 22, textHeight);
            x += 22 + leftPadding;

			if (IsTextLabelAllowedToChangeFrame)
			{
                float positionTextLabelX = Bounds.Width - 128;
				if (PunchInButton.Alpha > 0)
                    positionTextLabelX -= 48;

                PositionTextLabel.Frame = new RectangleF(positionTextLabelX, topPadding, 120, textHeight);
                PositionTitleLabel.Frame = new RectangleF(leftPadding, topPadding + 35, Bounds.Width, textHeight);
                TitleTextLabel.Frame = new RectangleF(x, topPadding, Bounds.Width - 120, textHeight);
			}

			TextField.Frame = new RectangleF(x - 4, topPadding + 1, Bounds.Width - 120 - 48, 38);
            Slider.Frame = new RectangleF(leftPadding, textHeight + 34, Bounds.Width - 12 - buttonSize - 12, 36);
            DeleteButton.Frame = new RectangleF(Bounds.Width - buttonSize, topPadding, buttonSize, buttonSize);
            PunchInButton.Frame = new RectangleF(Bounds.Width - buttonSize, 68, buttonSize, buttonSize);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
            base.SetHighlighted(highlighted, animated);

            PositionTextLabel.Highlighted = highlighted;
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
            PositionTitleLabel.Alpha = isExpanded ? 1 : 0;
            Slider.Alpha = isExpanded ? 1 : 0;
            DeleteButton.Alpha = isExpanded ? 1 : 0;
            PunchInButton.Alpha = isExpanded ? 1 : 0;

            float padding = isExpanded ? 0 : 48f;
            PositionTextLabel.Frame = new RectangleF(Bounds.Width - 128 - padding, 6, 120, 38);
        }

        protected override void SetControlScaleForTouchAnimation(float scale)
        {
            TitleTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
            PositionTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
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
