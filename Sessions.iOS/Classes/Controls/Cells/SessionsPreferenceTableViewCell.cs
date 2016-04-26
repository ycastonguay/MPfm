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
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;
using Sessions.iOS.Classes.Controls.Cells.Base;

namespace Sessions.iOS.Classes.Controls.Cells
{
    [Register("SessionsPreferenceTableViewCell")]
    public class SessionsPreferenceTableViewCell : SessionsBaseTableViewCell
    {
        private PreferenceCellItem _item;

        public UILabel TitleTextLabel { get; private set; }
        public UILabel SubtitleTextLabel { get; private set; }
        public UIImageView ImageChevron { get; private set; }

        public UITextField ValueTextField { get; private set; }
        public UILabel ValueTextLabel { get; private set; }
        public UILabel MinValueTextLabel { get; private set; }
        public UILabel MaxValueTextLabel { get; private set; }
        public UISlider Slider { get; private set; }
        public UISwitch Switch { get; private set; }

        public bool IsLargeIcon { get; set; }

        public override bool UseContainerView { get { return true; } }

        public delegate void PreferenceValueChanged(PreferenceCellItem item);
        public event PreferenceValueChanged OnPreferenceValueChanged;

        public SessionsPreferenceTableViewCell() : base()
        {
        }

        public SessionsPreferenceTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public SessionsPreferenceTableViewCell(CGRect frame) : base(frame)
        {
        }

        public SessionsPreferenceTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsTextAnimationEnabled = true;
           
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

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron"));
            ImageChevron.BackgroundColor = UIColor.Clear;
            ImageChevron.Hidden = true;
            ImageChevron.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - 22, 4, 22, 44);
            AddView(ImageChevron);           

            // Make sure the text label is over all other subviews
            AddView(TitleTextLabel);

            CreatePreferenceControls();
        }

        private void CreatePreferenceControls()
        {
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
            AddView(ValueTextField);

            ValueTextLabel = new UILabel();
            ValueTextLabel.Hidden = true;
            ValueTextLabel.BackgroundColor = UIColor.Clear;
            ValueTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            ValueTextLabel.TextColor = UIColor.Gray;
            ValueTextLabel.TextAlignment = UITextAlignment.Right;
            ValueTextLabel.HighlightedTextColor = UIColor.White;
            AddView(ValueTextLabel);

            MinValueTextLabel = new UILabel();
            MinValueTextLabel.Hidden = true;
            MinValueTextLabel.BackgroundColor = UIColor.Clear;
            MinValueTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            MinValueTextLabel.TextColor = UIColor.LightGray;
            MinValueTextLabel.HighlightedTextColor = UIColor.White;
            AddView(MinValueTextLabel);

            MaxValueTextLabel = new UILabel();
            MaxValueTextLabel.Hidden = true;
            MaxValueTextLabel.BackgroundColor = UIColor.Clear;
            MaxValueTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            MaxValueTextLabel.TextColor = UIColor.LightGray;
            MaxValueTextLabel.TextAlignment = UITextAlignment.Right;
            MaxValueTextLabel.HighlightedTextColor = UIColor.White;
            AddView(MaxValueTextLabel);

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
            Slider.MinimumTrackTintColor = GlobalTheme.SecondaryColor;
            AddView(Slider);

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
            AddView(Switch);

            // Make sure the Done key closes the keyboard
            ValueTextField.ShouldReturn = (a) => ValidateAndSaveValue();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            const float padding = 8;
            var screenSize = UIKitHelper.GetDeviceSize();

            ContainerBackgroundView.Frame = Bounds;

            nfloat textWidth = Bounds.Width;
            if (ImageView.Image != null && !ImageView.Hidden)
                textWidth -= 44 + padding;

            float x = 12;
            if (ImageView.Image != null && IsTextLabelAllowedToChangeFrame)
            {
                if (IsLargeIcon)
                {
                    x = 8;
                    ImageView.Frame = new CGRect(x, 6, 40, 40);
                    x += 44 + padding;
                }
                else
                {
                    x = 12;
                    ImageView.Frame = new CGRect(x, 14, 24, 24);
                    x += 32 + padding;
                }
            }

            float titleY = 10 + 4;
            if (!string.IsNullOrEmpty(SubtitleTextLabel.Text))
                titleY = 2 + 4;

            if(IsTextLabelAllowedToChangeFrame)
                TitleTextLabel.Frame = new CGRect(x, titleY, textWidth - 52, 22);

            ValueTextLabel.Frame = new CGRect(0, titleY, Bounds.Width - 12, 22);
            ValueTextField.Frame = new CGRect(Bounds.Width - 112, titleY, 100, 22);

            if (!string.IsNullOrEmpty(SubtitleTextLabel.Text))
                SubtitleTextLabel.Frame = new CGRect(x, 22 + 4, textWidth - 52, 16);

            ImageChevron.Frame = new CGRect(screenSize.Width - 22, 4, 22, 44);
            MinValueTextLabel.Frame = new CGRect(12, 48, 60, 44);
            MaxValueTextLabel.Frame = new CGRect(Bounds.Width - 60 - 12, 48, 60, 44);
            Slider.Frame = new CGRect(12 + 60, 48, Bounds.Width - 24 - 120, 44);

            if(UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                Switch.Frame = new CGRect(screenSize.Width - 62, 10, 60, 44);
            else
                Switch.Frame = new CGRect(screenSize.Width - 93, 12, 91, 44);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
            if(_item != null && _item.CellType != PreferenceCellType.Button)
                return;

            SelectedBackgroundView.Alpha = 1;
            SelectedBackgroundView.Hidden = !highlighted;
            TitleTextLabel.TextColor = highlighted ? UIColor.White : UIColor.Black;
            SubtitleTextLabel.Highlighted = highlighted;

            base.SetHighlighted(highlighted, animated);
        }

        public override void SetSelected(bool selected, bool animated)
        {
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

        protected override void SetControlScaleForTouchAnimation(float scale)
        {
            TitleTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
            SubtitleTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
            ImageView.Transform = CGAffineTransform.MakeScale(scale, scale);
        }

        public void SetItem(PreferenceCellItem item)
        {
            _item = item;

            TitleTextLabel.Text = item.Title;
            TitleTextLabel.TextColor = item.Enabled ? UIColor.Black : UIColor.FromRGB(0.7f, 0.7f, 0.7f);
            SubtitleTextLabel.Text = item.Description;
            SubtitleTextLabel.TextColor = item.Enabled ? UIColor.Gray : UIColor.FromRGB(0.85f, 0.85f, 0.85f);
            Switch.Hidden = item.CellType != PreferenceCellType.Boolean;
            Switch.Enabled = item.Enabled;
            Slider.Hidden = item.CellType != PreferenceCellType.Slider;
            SelectionStyle = item.CellType != PreferenceCellType.Boolean && item.CellType != PreferenceCellType.Slider && item.Enabled ? UITableViewCellSelectionStyle.Default : UITableViewCellSelectionStyle.None;
            ValueTextField.Hidden = item.CellType != PreferenceCellType.Integer;
            ValueTextLabel.Hidden = item.CellType != PreferenceCellType.Slider;
            MinValueTextLabel.Hidden = item.CellType != PreferenceCellType.Slider;
            MaxValueTextLabel.Hidden = item.CellType != PreferenceCellType.Slider;

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
