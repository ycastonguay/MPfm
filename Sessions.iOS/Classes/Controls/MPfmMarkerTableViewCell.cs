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
using Sessions.Core;

namespace Sessions.iOS.Classes.Controls
{
	[Register("SessionsMarkerTableViewCell")]
	public class SessionsMarkerTableViewCell : UITableViewCell
    {
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
		public event UndoMarker OnUndoMarker;
		public event ChangeMarkerPosition OnChangeMarkerPosition;
		public event ChangeMarkerName OnChangeMarkerName;
		public event SetMarkerPosition OnSetMarkerPosition;

        private bool _isTextLabelAllowedToChangeFrame = true;
		private float _sliderValue;
		private UIView _backgroundView;

        public UILabel IndexTextLabel { get; private set; }
        public UILabel TitleTextLabel { get; private set; }
        public UILabel PositionTextLabel { get; private set; }
		public SessionsSemiTransparentRoundButton DeleteButton { get; set; }
		public SessionsSemiTransparentRoundButton PunchInButton { get; set; }
		//public SessionsSemiTransparentRoundButton UndoButton { get; set; }
        public UISlider Slider { get; private set; }
        public UITextField TextField { get; private set; }
		public UILabel PositionTitleLabel { get; private set; }

        public bool IsTextAnimationEnabled { get; set; }
        public float RightOffset { get; set; }
		public bool IsExpanded { get; private set; }
		public Guid MarkerId { get; set; }

		public SessionsMarkerTableViewCell() : base()
        {
            Initialize();
        }

		public SessionsMarkerTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

		public SessionsMarkerTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public void Initialize()
        {
            IsTextAnimationEnabled = false;
            SelectionStyle = UITableViewCellSelectionStyle.None;
           
//            UIView backView = new UIView(Frame);
//			backView.BackgroundColor = UIColor.Blue;
//            BackgroundView = backView;

			_backgroundView = new UIView(Frame);
			_backgroundView.BackgroundColor = UIColor.Clear;
			AddSubview(_backgroundView);

            TextLabel.RemoveFromSuperview();
            DetailTextLabel.RemoveFromSuperview();
            
			// Adding gesture recognizer to BackgroundView doesn't work
			var longPress = new UILongPressGestureRecognizer(HandleLongPress);
			longPress.MinimumPressDuration = 0.7f;
			longPress.WeakDelegate = this;
			_backgroundView.AddGestureRecognizer(longPress);

            var backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;     
            SelectedBackgroundView.Hidden = true;
            AddSubview(SelectedBackgroundView);

            TitleTextLabel = new UILabel();
            TitleTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            TitleTextLabel.BackgroundColor = UIColor.Purple;
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

            ImageView.BackgroundColor = UIColor.Clear;		

            // Make sure the text label is over all other subviews
            //PositionTextLabel.RemoveFromSuperview();
            ImageView.RemoveFromSuperview();
            AddSubview(PositionTextLabel);
            AddSubview(ImageView);

			TextField = new UITextField();
			TextField.Layer.CornerRadius = 8;
			TextField.Alpha = 0;
			TextField.BackgroundColor = UIColor.FromRGBA(0.8f, 0.8f, 0.8f, 0.075f);
			TextField.Font = UIFont.FromName("HelveticaNeue-Light", 16);
			TextField.TextColor = UIColor.White;
			TextField.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			TextField.ReturnKeyType = UIReturnKeyType.Done;
			AddSubview(TextField);

			PositionTitleLabel = new UILabel();
			PositionTitleLabel.Text = "Position";
			PositionTitleLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
			PositionTitleLabel.TextColor = UIColor.FromRGB(0.8f, 0.8f, 0.8f);
            PositionTitleLabel.Alpha = 0;
			AddSubview(PositionTitleLabel);

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
            AddSubview(IndexTextLabel);

			DeleteButton = new SessionsSemiTransparentRoundButton();
			DeleteButton.Alpha = 0;
			DeleteButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/remove");
			DeleteButton.TouchUpInside += HandleOnDeleteButtonClick;
			AddSubview(DeleteButton);

			PunchInButton = new SessionsSemiTransparentRoundButton();
			PunchInButton.Alpha = 0;
			PunchInButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/punch_in");
			PunchInButton.TouchUpInside += HandleOnPunchInButtonClick;
			AddSubview(PunchInButton);

//			UndoButton = new SessionsSemiTransparentRoundButton();
//			UndoButton.Alpha = 0;
//			UndoButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/undo");
//			UndoButton.TouchUpInside += HandleOnUndoButtonClick;
//			AddSubview(UndoButton);

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
				//Tracing.Log("MarkerTableViewCell - TouchDown");
				// There's a bug in UISlider inside a UITableView inside a UIScrollView; the table view offset will change when changing slider value
				var tableView = (SessionsTableView)GetTableView();
				tableView.BlockContentOffsetChange = true;
			};
			Slider.TouchUpInside += (sender, e) => {
				//Tracing.Log("MarkerTableViewCell - TouchUpInside");
				var tableView = (SessionsTableView)GetTableView();
				tableView.BlockContentOffsetChange = false;

				// Take the last value from ValueChanged to prevent getting a slightly different value when the finger leaves the screen
				OnSetMarkerPosition(MarkerId, _sliderValue); 
			};
			AddSubview(Slider);
        }

		private UITableView GetTableView()
		{
			UIView view = Superview;
			while (!(view is UITableView))
				view = view.Superview;
			return (UITableView)view;
		}

		private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
		{
			Tracing.Log("MarkerTableViewCell - HandleLongPress - state: {0}", gestureRecognizer.State);
			if (gestureRecognizer.State != UIGestureRecognizerState.Began)
				return;

			//PointF pt = gestureRecognizer.LocationInView(BackgroundView);
			//Tracing.Log("MarkerTableViewCell - HandleLongPress - point: {0}", pt);
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

		private void HandleOnUndoButtonClick(object sender, EventArgs e)
		{
			if (OnUndoMarker != null)
				OnUndoMarker(MarkerId);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			float padding = 8;
			_backgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            SelectedBackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);

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

			if (_isTextLabelAllowedToChangeFrame)
			{
                TitleTextLabel.Frame = new RectangleF(x, 6, Bounds.Width - 120, 38);
				PositionTitleLabel.Frame = new RectangleF(padding, 6 + 35, Bounds.Width, 38);
				if (PunchInButton.Alpha > 0)
					PositionTextLabel.Frame = new RectangleF(Bounds.Width - 128 - 48, 6, 120, 38);
				else
					PositionTextLabel.Frame = new RectangleF(Bounds.Width - 128, 6, 120, 38);
			}

			TextField.Frame = new RectangleF(x - 4, 7, Bounds.Width - 120 - 48, 38);
			Slider.Frame = new RectangleF(8, 38 + 34, Bounds.Width - 12 - 44 - 12, 36);
			DeleteButton.Frame = new RectangleF(Bounds.Width - 44, 6, 44, 44);
			PunchInButton.Frame = new RectangleF(Bounds.Width - 44, 68, 44, 44);
			//UndoButton.Frame = new RectangleF((Bounds.Width - 44 - 14 - 44) / 2, 76 + 34, 44, 44);
			//PunchInButton.Frame = new RectangleF(((Bounds.Width - 44 - 14 - 44) / 2) + 44 + 14, 76 + 34, 44, 44);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
			Tracing.Log("MarkerTableViewCell - SetHighlighted - highlighted: {0} animated: {1}", highlighted, animated);
//            SelectedBackgroundView.Alpha = 1;
//            SelectedBackgroundView.Hidden = !highlighted;
            PositionTextLabel.Highlighted = highlighted;
            IndexTextLabel.Highlighted = highlighted;

			if (!highlighted)
			{
				UIView.Animate(0.5, () => SelectedBackgroundView.Alpha = 0, () => SelectedBackgroundView.Hidden = true);
			}
			else
			{
				SelectedBackgroundView.Hidden = false;
				SelectedBackgroundView.Alpha = 1;
			}

            base.SetHighlighted(highlighted, animated);
        }

        public override void SetSelected(bool selected, bool animated)
        {
			Tracing.Log("MarkerTableViewCell - SetSelected - selected: {0} animated: {1}", selected, animated);
            if (!selected)
            {
                UIView.Animate(0.5, () => SelectedBackgroundView.Alpha = 0, () => SelectedBackgroundView.Hidden = true);
            }
            else
            {
                SelectedBackgroundView.Hidden = false;
                SelectedBackgroundView.Alpha = 1;
            }

            base.SetSelected(selected, animated);
        }

		public void ExpandCell(bool animated)
		{
            Tracing.Log("MarkerTableViewCell - ExpandCell - title: {0}", TitleTextLabel.Text);
			if (animated)
			{
				UIView.Animate(0.2, ExpandCell, () => IsExpanded = true);
			}
			else
			{
				ExpandCell();
				IsExpanded = true;
			}
		}

		private void ExpandCell()
		{
            SetControlVisibility(true);
		}

		public void CollapseCell(bool animated)
		{
            Tracing.Log("MarkerTableViewCell - CollapseCell - title: {0}", TitleTextLabel.Text);
			if (animated)
			{
				UIView.Animate(0.2, CollapseCell, () => IsExpanded = false);
			}
			else
			{
				CollapseCell();
				IsExpanded = false;
			}
		}

		private void CollapseCell()
		{
            SetControlVisibility(false);
            TitleTextLabel.Text = TextField.Text;
			TextField.ResignFirstResponder();
		}

        private void SetControlVisibility(bool isExpanded)
        {
            TextField.Alpha = isExpanded ? 1 : 0;
            TitleTextLabel.Alpha = isExpanded ? 0 : 1;
            PositionTitleLabel.Alpha = isExpanded ? 1 : 0;
            Slider.Alpha = isExpanded ? 1 : 0;
            DeleteButton.Alpha = isExpanded ? 1 : 0;
            PunchInButton.Alpha = isExpanded ? 1 : 0;
            //UndoButton.Alpha = isExpanded ? 1 : 0;

            float padding = IsExpanded ? 48f : 0;
            PositionTextLabel.Frame = new RectangleF(Bounds.Width - 128 - padding, 6, 120, 38);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
			//Tracing.Log("MarkerTableViewCell - TouchesBegan - eventType: {0}", evt.Type);
            AnimatePress(true);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
			//Tracing.Log("MarkerTableViewCell - TouchesEnded - eventType: {0}", evt.Type);
            AnimatePress(false);
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
			//Tracing.Log("MarkerTableViewCell - TouchesCancelled - eventType: {0}", evt.Type);
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
                    PositionTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    IndexTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                }, null);
            }
            else
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    TitleTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    PositionTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                }, null);
            }
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
