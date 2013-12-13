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
using MPfm.Core;

namespace MPfm.iOS.Classes.Controls
{
	[Register("MPfmMarkerTableViewCell")]
	public class MPfmMarkerTableViewCell : UITableViewCell
    {
		public delegate void DeleteMarker(Guid markerId);
		public delegate void PunchInMarker(Guid markerId);
		public delegate void UndoMarker(Guid markerId);
		public delegate void ChangeMarkerPosition(Guid markerId, float newPositionPercentage);
		public delegate void SetMarkerPosition(Guid markerId, float newPositionPercentage);
		public event DeleteMarker OnDeleteMarker;
		public event PunchInMarker OnPunchInMarker;
		public event UndoMarker OnUndoMarker;
		public event ChangeMarkerPosition OnChangeMarkerPosition;
		public event SetMarkerPosition OnSetMarkerPosition;

        private bool _isTextLabelAllowedToChangeFrame = true;

        public UILabel IndexTextLabel { get; private set; }
        //public UIView SecondaryMenuBackground { get; private set; }

		public MPfmSemiTransparentRoundButton DeleteButton { get; set; }
		public MPfmSemiTransparentRoundButton PunchInButton { get; set; }
		public MPfmSemiTransparentRoundButton UndoButton { get; set; }
		public UISlider Slider { get; set; }
		public UITextField TextField { get; set; }
		public UILabel TitleLabel { get; set; }

        public bool IsTextAnimationEnabled { get; set; }
        public float RightOffset { get; set; }

		public Guid MarkerId { get; set; }

		public MPfmMarkerTableViewCell() : base()
        {
            Initialize();
        }

		public MPfmMarkerTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

		public MPfmMarkerTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public void Initialize()
        {
            IsTextAnimationEnabled = false;
            SelectionStyle = UITableViewCellSelectionStyle.None;
           
//            UIView backView = new UIView(Frame);
//            backView.BackgroundColor = GlobalTheme.LightColor;
//            BackgroundView = backView;
//            BackgroundColor = UIColor.White;
            
            UIView backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;     
            SelectedBackgroundView.Hidden = true;
            AddSubview(SelectedBackgroundView);

            TextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			TextLabel.BackgroundColor = UIColor.Purple;
			TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
			TextLabel.TextColor = UIColor.White;
			TextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			DetailTextLabel.Font = UIFont.FromName("HelveticaNeue", 15);
			DetailTextLabel.TextColor = UIColor.White;
			DetailTextLabel.HighlightedTextColor = UIColor.White;
			DetailTextLabel.TextAlignment = UITextAlignment.Right;
			TextLabel.BackgroundColor = UIColor.Purple;
            ImageView.BackgroundColor = UIColor.Clear;		

            // Make sure the text label is over all other subviews
            DetailTextLabel.RemoveFromSuperview();
            ImageView.RemoveFromSuperview();
            AddSubview(DetailTextLabel);
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

			TitleLabel = new UILabel();
			TitleLabel.Text = "Position";
			TitleLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
			TitleLabel.TextColor = UIColor.FromRGB(0.8f, 0.8f, 0.8f);
			AddSubview(TitleLabel);

			// Add padding to text field
			UIView paddingView = new UIView(new RectangleF(0, 0, 4, 20));
			TextField.LeftView = paddingView;
			TextField.LeftViewMode = UITextFieldViewMode.Always;

			// Make sure the Done key closes the keyboard
			TextField.ShouldReturn = (a) => {
				TextLabel.Text = TextField.Text;
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

			DeleteButton = new MPfmSemiTransparentRoundButton();
			DeleteButton.Alpha = 0;
			DeleteButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/remove");
			DeleteButton.TouchUpInside += HandleOnDeleteButtonClick;
			AddSubview(DeleteButton);

			PunchInButton = new MPfmSemiTransparentRoundButton();
			PunchInButton.Alpha = 0;
			PunchInButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/punch_in");
			PunchInButton.TouchUpInside += HandleOnPunchInButtonClick;
			AddSubview(PunchInButton);

			UndoButton = new MPfmSemiTransparentRoundButton();
			UndoButton.Alpha = 0;
			UndoButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/undo");
			UndoButton.TouchUpInside += HandleOnUndoButtonClick;
			AddSubview(UndoButton);

            // Make sure the text label is over all other subviews
            TextLabel.RemoveFromSuperview();
            AddSubview(TextLabel);

			Slider = new UISlider(new RectangleF(0, 0, 10, 10));
			Slider.Alpha = 0;
			Slider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
			Slider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
			Slider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider_gray").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
			Slider.ValueChanged += (sender, e) => OnChangeMarkerPosition(MarkerId, Slider.Value);
			Slider.TouchUpInside += (sender, e) => OnSetMarkerPosition(MarkerId, Slider.Value);
			AddSubview(Slider);

//            SecondaryMenuBackground = new UIView();
//            SecondaryMenuBackground.BackgroundColor = UIColor.White;
//            SecondaryMenuBackground.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width, 4, 188, 44);
//            //SecondaryMenuBackground.Alpha = 0;
//            AddSubview(SecondaryMenuBackground);
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
            //BackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height - 1);
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
				TextLabel.Frame = new RectangleF(x, 6, Bounds.Width - 120, 38);
				TitleLabel.Frame = new RectangleF(padding, 6 + 35, Bounds.Width, 38);
				if (PunchInButton.Alpha > 0)
					DetailTextLabel.Frame = new RectangleF(Bounds.Width - 128 - 48, 6, 120, 38);
				else
					DetailTextLabel.Frame = new RectangleF(Bounds.Width - 128, 6, 120, 38);
			}

			TextField.Frame = new RectangleF(x - 4, 7, Bounds.Width - 120 - 48, 38);
			Slider.Frame = new RectangleF(8, 38 + 34, Bounds.Width - 12, 36);
			DeleteButton.Frame = new RectangleF(Bounds.Width - 44, 6, 44, 44);
			//UndoButton.Frame = new RectangleF((Bounds.Width - 50 - 14 - 50) / 2, 76 + 34, 50, 50);
			//PunchInButton.Frame = new RectangleF(((Bounds.Width - 50 - 14 - 50) / 2) + 50 + 14, 76 + 34, 50, 50);
			UndoButton.Frame = new RectangleF((Bounds.Width - 44 - 14 - 44) / 2, 76 + 34, 44, 44);
			PunchInButton.Frame = new RectangleF(((Bounds.Width - 44 - 14 - 44) / 2) + 44 + 14, 76 + 34, 44, 44);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
			Tracing.Log("MarkerTableViewCell - SetHighlighted - highlighted: {0} animated: {1}", highlighted, animated);
//            SelectedBackgroundView.Alpha = 1;
//            SelectedBackgroundView.Hidden = !highlighted;
            DetailTextLabel.Highlighted = highlighted;
            IndexTextLabel.Highlighted = highlighted;

			if (!highlighted)
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

            base.SetHighlighted(highlighted, animated);
        }

        public override void SetSelected(bool selected, bool animated)
        {
			Tracing.Log("MarkerTableViewCell - SetSelected - selected: {0} animated: {1}", selected, animated);

            //if(selected)
                //SecondaryMenuBackground.BackgroundColor = GlobalTheme.SecondaryColor;

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

		public void ExpandCell()
		{
			Tracing.Log("MarkerTableViewCell - ExpandCell - title: {0}", TextLabel.Text);
			UIView.Animate(0.2, () =>
			{
				TextField.Alpha = 1;
				TextLabel.Alpha = 0;
				Slider.Alpha = 1;
				DeleteButton.Alpha = 1;
				PunchInButton.Alpha = 1;
				UndoButton.Alpha = 1;
				DetailTextLabel.Frame = new RectangleF(Bounds.Width - 128 - 48, 6, 120, 38);
			}, null);
		}

		public void CollapseCell()
		{
			Tracing.Log("MarkerTableViewCell - CollapseCell - title: {0}", TextLabel.Text);
			UIView.Animate(0.2, () =>
			{
				TextField.Alpha = 0;
				TextLabel.Alpha = 1;
				Slider.Alpha = 0;
				DeleteButton.Alpha = 0;
				PunchInButton.Alpha = 0;
				UndoButton.Alpha = 0;
				DetailTextLabel.Frame = new RectangleF(Bounds.Width - 128, 6, 120, 38);
			}, null);
			TextLabel.Text = TextField.Text;
			TextField.ResignFirstResponder();
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
                    if(TextLabel.Transform.xx < 0.95f) return;

                    TextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    IndexTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                }, null);
            }
            else
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    TextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                }, null);
            }
        }
    }
}
