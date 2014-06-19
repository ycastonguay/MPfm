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
    [Register("SessionsPreferenceTableViewCell")]
    public class SessionsPreferenceTableViewCell : UITableViewCell
    {
        public delegate void PreferenceValueChanged(PreferenceCellItem item);
        public event PreferenceValueChanged OnPreferenceValueChanged;

        private PreferenceCellItem _item;
        private bool _isTextLabelAllowedToChangeFrame = true;

        public bool IsLargeIcon { get; set; }
        public UIButton RightButton { get; private set; }
        public UIImageView ImageChevron { get; private set; }
		public UITextField ValueTextField { get; private set; }
        public UILabel ValueTextLabel { get; private set; }
        public UILabel MinValueTextLabel { get; private set; }
        public UILabel MaxValueTextLabel { get; private set; }
        public UIView ViewSeparator { get; private set; }
        public UISlider Slider { get; private set; }
        public UISwitch Switch { get; private set; }

        public SessionsPreferenceTableViewCell() : base()
        {
            Initialize();
        }

        public SessionsPreferenceTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        public SessionsPreferenceTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public void Initialize()
        {
            var screenSize = UIKitHelper.GetDeviceSize();

            UIView backView = new UIView(Frame);
            backView.BackgroundColor = GlobalTheme.LightColor;
            BackgroundView = backView;
            //BackgroundColor = UIColor.White;
            
            UIView backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;           

            TextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            TextLabel.BackgroundColor = UIColor.Yellow;
            TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            TextLabel.TextAlignment = UITextAlignment.Left;
            TextLabel.TextColor = UIColor.Black;
            TextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            DetailTextLabel.BackgroundColor = UIColor.Clear;
            DetailTextLabel.TextColor = UIColor.Gray;
            DetailTextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 12);
            ImageView.BackgroundColor = UIColor.Clear;

			ValueTextField = new UITextField();
			ValueTextField.WeakDelegate = this;
			ValueTextField.Hidden = true;
			ValueTextField.UserInteractionEnabled = false;
			ValueTextField.BackgroundColor = UIColor.Clear;
			ValueTextField.Font = UIFont.FromName("HelveticaNeue-Light", 16);
			ValueTextField.TextColor = UIColor.Gray;
			ValueTextField.TextAlignment = UITextAlignment.Right;
			ValueTextField.Placeholder = "53551";
			ValueTextField.KeyboardType = UIKeyboardType.NumberPad;
			ValueTextField.ReturnKeyType = UIReturnKeyType.Done;
			AddSubview(ValueTextField);

			ValueTextLabel = new UILabel();
            ValueTextLabel.BackgroundColor = UIColor.Clear;
            ValueTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            ValueTextLabel.TextColor = UIColor.Gray;
            ValueTextLabel.TextAlignment = UITextAlignment.Right;
            ValueTextLabel.HighlightedTextColor = UIColor.White;
            AddSubview(ValueTextLabel);

            MinValueTextLabel = new UILabel();
            MinValueTextLabel.BackgroundColor = UIColor.Clear;
            MinValueTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            MinValueTextLabel.TextColor = UIColor.LightGray;
            MinValueTextLabel.HighlightedTextColor = UIColor.White;
            AddSubview(MinValueTextLabel);

            MaxValueTextLabel = new UILabel();
            MaxValueTextLabel.BackgroundColor = UIColor.Clear;
            MaxValueTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            MaxValueTextLabel.TextColor = UIColor.LightGray;
            MaxValueTextLabel.TextAlignment = UITextAlignment.Right;
            MaxValueTextLabel.HighlightedTextColor = UIColor.White;
            AddSubview(MaxValueTextLabel);

            RightButton = new UIButton(UIButtonType.Custom);
            RightButton.Hidden = true;
            RightButton.Frame = new RectangleF(screenSize.Width - Bounds.Height, 4, Bounds.Height, Bounds.Height);
            AddSubview(RightButton);

            ViewSeparator = new UIView();
            ViewSeparator.BackgroundColor = UIColor.FromRGBA(0.75f, 0.75f, 0.75f, 0.5f);
            AddSubview(ViewSeparator);

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron"));
            ImageChevron.BackgroundColor = UIColor.Clear;
            ImageChevron.Hidden = true;
            ImageChevron.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 22, 4, 22, 44);
            AddSubview(ImageChevron);   

            Slider = new UISlider();
            Slider.Hidden = true;
            Slider.ValueChanged += (sender, e) => {
                _item.Value = (int)Slider.Value;
                ValueTextLabel.Text = string.Format("{0} {1}", (int)_item.Value, _item.ScaleName);
            };
			Slider.TouchUpInside += (sender, e) => {
				if(OnPreferenceValueChanged != null)
					OnPreferenceValueChanged(_item);       			
			};
			if(!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
			{
				Slider.MinimumTrackTintColor = GlobalTheme.SecondaryColor;
//				Slider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
//				Slider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
//				Slider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
			}
			AddSubview(Slider);

            Switch = new UISwitch();
            Switch.Hidden = true;
            Switch.ValueChanged += (sender, e) => {
                _item.Value = Switch.On;
                if(OnPreferenceValueChanged != null)
                    OnPreferenceValueChanged(_item);               
            };
			if(!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
				Switch.OnTintColor = GlobalTheme.SecondaryColor;
			else
				Switch.TintColor = GlobalTheme.SecondaryColor;
            AddSubview(Switch);

            // Make sure the text label is over all other subviews
            TextLabel.RemoveFromSuperview();
            AddSubview(TextLabel);

			// Make sure the Done key closes the keyboard
			ValueTextField.ShouldReturn = (a) => {
				return ValidateAndSaveValue();
			};
        }

        public override void LayoutSubviews()
        {
            //base.LayoutSubviews();

            var screenSize = UIKitHelper.GetDeviceSize();
            float padding = 8;

            ViewSeparator.Frame = new RectangleF(0, Frame.Height - 1, Frame.Width, 1);

            if(BackgroundView != null)
                BackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            if(SelectedBackgroundView != null)
                SelectedBackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);

            // Determine width available for text
            float textWidth = Bounds.Width;
            if (Accessory != UITableViewCellAccessory.None)
                textWidth -= 44;
            if (ImageView.Image != null && !ImageView.Hidden)
                textWidth -= 44 + padding;
            if (RightButton.ImageView.Image != null)
                textWidth -= 44 + padding;

            float x = 12;
            if (ImageView.Image != null && _isTextLabelAllowedToChangeFrame)
            {
                if (IsLargeIcon)
                {
                    x = 8;
                    ImageView.Frame = new RectangleF(x, 6, 40, 40);
                    x += 44 + padding;
                }
                else
                {
                    x = 12;
                    ImageView.Frame = new RectangleF(x, 14, 24, 24);
                    x += 32 + padding;
                }
            }

            float titleY = 10 + 4;
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                titleY = 2 + 4;

            if(_isTextLabelAllowedToChangeFrame)
                TextLabel.Frame = new RectangleF(x, titleY, textWidth - 52, 22);
            ValueTextLabel.Frame = new RectangleF(0, titleY, Bounds.Width - 12, 22);
			ValueTextField.Frame = new RectangleF(Bounds.Width - 112, titleY, 100, 22);

            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                DetailTextLabel.Frame = new RectangleF(x, 22 + 4, textWidth - 52, 16);

            if (RightButton.ImageView.Image != null || !string.IsNullOrEmpty(RightButton.Title(UIControlState.Normal)))
                RightButton.Frame = new RectangleF(screenSize.Width - 44, 4, 44, 44);

            ImageChevron.Frame = new RectangleF(screenSize.Width - 22, 4, 22, 44);
            MinValueTextLabel.Frame = new RectangleF(12, 48, 60, 44);
            MaxValueTextLabel.Frame = new RectangleF(Bounds.Width - 60 - 12, 48, 60, 44);
            Slider.Frame = new RectangleF(12 + 60, 48, Bounds.Width - 24 - 120, 44);

			if(UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
				Switch.Frame = new RectangleF(screenSize.Width - 62, 10, 60, 44);
			else
				Switch.Frame = new RectangleF(screenSize.Width - 93, 12, 91, 44);
        }

		public override void SetSelected(bool selected, bool animated)
		{
			//Tracing.Log("PreferenceTableViewCell - SetSelected - selected: {0}", selected);
			base.SetSelected(selected, animated);		

			if (_item == null)
				return;

			if (selected && _item.CellType == PreferenceCellType.Integer && _item.Enabled)
			{
				if (!ValueTextField.IsFirstResponder)
				{
					ValueTextField.UserInteractionEnabled = true;
					ValueTextField.BecomeFirstResponder();
				}
				else
				{
					ValidateAndSaveValue();
				}
			}
		}

        public void SetItem(PreferenceCellItem item)
        {
            _item = item;
            BackgroundView.BackgroundColor = item.Enabled ? UIColor.White : UIColor.FromRGB(0.95f, 0.95f, 0.95f);
            TextLabel.Text = item.Title;
            TextLabel.TextColor = item.Enabled ? UIColor.Black : UIColor.FromRGB(0.7f, 0.7f, 0.7f);
            DetailTextLabel.Text = item.Description;
            DetailTextLabel.TextColor = item.Enabled ? UIColor.Gray : UIColor.FromRGB(0.85f, 0.85f, 0.85f);
            Switch.Hidden = item.CellType != PreferenceCellType.Boolean;
            Switch.Enabled = item.Enabled;
            Slider.Hidden = item.CellType != PreferenceCellType.Slider;
            SelectionStyle = item.CellType != PreferenceCellType.Boolean && item.CellType != PreferenceCellType.Slider && item.Enabled ? UITableViewCellSelectionStyle.Default : UITableViewCellSelectionStyle.None;
			ValueTextField.Hidden = item.CellType != PreferenceCellType.Integer;

			ValueTextLabel.Text = string.Empty;
			ValueTextField.Text = string.Empty;
			ValueTextField.Tag = (int)item.CellType;

            if (item.Value == null)
                return;

            switch (item.CellType)
            {
                case PreferenceCellType.Button:
                    break;
                case PreferenceCellType.Boolean:
                    Switch.On = (bool)item.Value;
                    break;
                case PreferenceCellType.String:
                    break;
                case PreferenceCellType.Integer:
					ValueTextField.Text = string.Format("{0} {1}", (int)item.Value, item.ScaleName);
                    break;
                case PreferenceCellType.Slider:
                    ValueTextLabel.Text = string.Format("{0} {1}", (int)item.Value, item.ScaleName);
                    MinValueTextLabel.Text = string.Format("{0} {1}", item.MinValue, item.ScaleName);
                    MaxValueTextLabel.Text = string.Format("{0} {1}", item.MaxValue, item.ScaleName);
                    Slider.MinValue = item.MinValue;
                    Slider.MaxValue = item.MaxValue;
                    Slider.Value = (int)item.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            _isTextLabelAllowedToChangeFrame = !on;

            if (!on)
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    // Ignore when scale is lower; it was done on purpose and will be restored to 1 later.
                    if(TextLabel.Transform.xx < 0.95f) return;

                    TextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    ImageView.Transform = CGAffineTransform.MakeScale(1, 1);
                }, null);
            }
            else
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    TextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    ImageView.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
                }, null);
            }
        }

		[Export("textFieldShouldEndEditing:")]
		private bool TextFieldShouldEndEditing(UITextField textField)
		{
			return ValidateAndSaveValue();
		}

		private bool ValidateAndSaveValue()
		{
			bool validated = ValidateValue();
			if (validated)
			{
				int value = 0;
				int.TryParse(ValueTextField.Text, out value);
				_item.Value = value;
				ValueTextField.ResignFirstResponder();
				ValueTextField.UserInteractionEnabled = false;

				if(OnPreferenceValueChanged != null)
					OnPreferenceValueChanged(_item);
			}

			return validated;
		}

		private bool ValidateValue()
		{
			if (_item.ValidateValueDelegate != null)
			{
				bool validated = _item.ValidateValueDelegate(ValueTextField.Text);
				if (!validated)
				{
					var alertView = new UIAlertView("Validation failed", _item.ValidateFailErrorMessage, null, "OK", null);
					alertView.Show();
				}
				return validated;
			}
			return true;
		}
    }
}
 