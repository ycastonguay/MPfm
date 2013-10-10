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
    [Register("MPfmTableViewCell")]
    public class MPfmTableViewCell : UITableViewCell
    {
        public UILabel IndexTextLabel { get; private set; }
        public UIButton RightButton { get; private set; }
        public UIImageView RightImage { get; private set; }
        public UIImageView ImageChevron { get; private set; }
        public UIImageView ImageAlbum1 { get; private set; }
        public UIImageView ImageAlbum2 { get; private set; }
        public UIImageView ImageAlbum3 { get; private set; }
        public UILabel AlbumCountLabel { get; private set; }
        //public UIView SecondaryMenuBackground { get; private set; }

        public MPfmImageButton PlayButton { get; set; }
        public MPfmImageButton AddButton { get; set; }
        public MPfmImageButton DeleteButton { get; set; }

        public bool IsTextAnimationEnabled { get; set; }

        public float RightOffset { get; set; }

        public delegate void RightButtonTap(MPfmTableViewCell cell);
        public event RightButtonTap OnRightButtonTap;

        public MPfmTableViewCell() : base()
        {
            Initialize();
        }

        public MPfmTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        public MPfmTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public void Initialize()
        {
            IsTextAnimationEnabled = false;
            SelectionStyle = UITableViewCellSelectionStyle.None;
            var screenSize = UIKitHelper.GetDeviceSize();

//            UIView backView = new UIView(Frame);
//            backView.BackgroundColor = GlobalTheme.LightColor;
//            BackgroundView = backView;
//            BackgroundColor = UIColor.White;
            
            UIView backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;     
            SelectedBackgroundView.Hidden = true;
            AddSubview(SelectedBackgroundView);

            ImageAlbum1 = new UIImageView();
            ImageAlbum1.BackgroundColor = UIColor.White;
            ImageAlbum1.Hidden = true;
            ImageAlbum1.Alpha = 0.75f;
            ImageAlbum1.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44);
            AddSubview(ImageAlbum1);

            ImageAlbum2 = new UIImageView();
            ImageAlbum2.BackgroundColor = UIColor.White;
            ImageAlbum2.Hidden = true;
            ImageAlbum2.Alpha = 0.4f;
            ImageAlbum2.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 130, 4, 44, 44);
            AddSubview(ImageAlbum2);

            ImageAlbum3 = new UIImageView();
            ImageAlbum3.BackgroundColor = UIColor.White;
            ImageAlbum3.Hidden = true;
            ImageAlbum3.Alpha = 0.2f;
            ImageAlbum3.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 182, 4, 44, 44);
            AddSubview(ImageAlbum3);

            AlbumCountLabel = new UILabel();
            AlbumCountLabel.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44);
            AlbumCountLabel.Alpha = 0.75f;
            AlbumCountLabel.BackgroundColor = GlobalTheme.MainColor;
            AlbumCountLabel.Font = UIFont.FromName("HelveticaNeue-Light", 18);
            AlbumCountLabel.Hidden = true;
            AlbumCountLabel.Text = "+98";
            AlbumCountLabel.TextColor = UIColor.White;
            AlbumCountLabel.TextAlignment = UITextAlignment.Center;
            AlbumCountLabel.HighlightedTextColor = UIColor.White;
            AddSubview(AlbumCountLabel);

            TextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            TextLabel.BackgroundColor = UIColor.Clear;
            TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 14);
            TextLabel.TextColor = UIColor.Black;
            TextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            DetailTextLabel.TextColor = UIColor.Gray;
            DetailTextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Font = UIFont.FromName("HelveticaNeue", 12);
            ImageView.BackgroundColor = UIColor.Clear;

            // Make sure the text label is over all other subviews
            DetailTextLabel.RemoveFromSuperview();
            ImageView.RemoveFromSuperview();
            AddSubview(DetailTextLabel);
            AddSubview(ImageView);

            IndexTextLabel = new UILabel();
            IndexTextLabel.BackgroundColor = UIColor.Clear;
            IndexTextLabel.Font = UIFont.FromName("HelveticaNeue", 16);
            IndexTextLabel.TextColor = UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);
            IndexTextLabel.TextAlignment = UITextAlignment.Center;
            IndexTextLabel.HighlightedTextColor = UIColor.White;
            AddSubview(IndexTextLabel);

            RightButton = new UIButton(UIButtonType.Custom);
            RightButton.Hidden = true;
            RightButton.Frame = new RectangleF(screenSize.Width - Bounds.Height, 4, Bounds.Height, Bounds.Height);
            RightButton.TouchUpInside += HandleRightButtonTouchUpInside;
            AddSubview(RightButton);

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron"));
            ImageChevron.BackgroundColor = UIColor.Clear;
            ImageChevron.Hidden = true;
            ImageChevron.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 22, 4, 22, 44);
            AddSubview(ImageChevron);           

            RightImage = new UIImageView(UIImage.FromBundle("Images/Icons/icon_speaker"));
            RightImage.Alpha = 0.7f;
            RightImage.BackgroundColor = UIColor.Clear;
            RightImage.Hidden = true;
            RightImage.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 66, 4, 44, 44);
            AddSubview(RightImage);

            // Make sure the text label is over all other subviews
            TextLabel.RemoveFromSuperview();
            AddSubview(TextLabel);

//            SecondaryMenuBackground = new UIView();
//            SecondaryMenuBackground.BackgroundColor = UIColor.White;
//            SecondaryMenuBackground.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width, 4, 188, 44);
//            //SecondaryMenuBackground.Alpha = 0;
//            AddSubview(SecondaryMenuBackground);

            PlayButton = new MPfmImageButton(new RectangleF(UIScreen.MainScreen.Bounds.Width - 182, 4, 44, 44));
            PlayButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/play"), UIControlState.Normal);
            PlayButton.Alpha = 0;
            AddSubview(PlayButton);

            AddButton = new MPfmImageButton(new RectangleF(UIScreen.MainScreen.Bounds.Width - 130, 4, 44, 44));
            AddButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/add"), UIControlState.Normal);
            AddButton.Alpha = 0;
            AddSubview(AddButton);

            DeleteButton = new MPfmImageButton(new RectangleF(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44));
            DeleteButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/trash"), UIControlState.Normal);
            DeleteButton.Alpha = 0;
            AddSubview(DeleteButton);
        }

        private void HandleRightButtonTouchUpInside(object sender, EventArgs e)
        {
            if (OnRightButtonTap != null)
                OnRightButtonTap(this);
        }

        public override void LayoutSubviews()
        {
            //BackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            SelectedBackgroundView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);

            var screenSize = UIKitHelper.GetDeviceSize();
            float padding = 8;

            // Determine width available for text
            float textWidth = Bounds.Width;
            if (Accessory != UITableViewCellAccessory.None)
                textWidth -= 44;
            if (ImageView.Image != null && !ImageView.Hidden)
                textWidth -= 44 + padding;
            if (RightButton.ImageView.Image != null)
                textWidth -= 44 + padding;
            if (!string.IsNullOrEmpty(IndexTextLabel.Text))
                textWidth -= 22 + padding + padding;
            if (ImageChevron.Image != null && !ImageChevron.Hidden)
                textWidth -= 22;
            if (RightImage.Image != null && !RightImage.Hidden)
                textWidth -= 44;

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
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                titleY = 2 + 4;

            if (_isTextLabelAllowedToChangeFrame)
                TextLabel.Frame = new RectangleF(x, titleY, textWidth, 22);
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                DetailTextLabel.Frame = new RectangleF(x, 22 + 4, textWidth, 16);

            if (RightButton.ImageView.Image != null || !string.IsNullOrEmpty(RightButton.Title(UIControlState.Normal)))
                RightButton.Frame = new RectangleF(screenSize.Width - 44, 4, 44, 44);

            ImageChevron.Frame = new RectangleF(screenSize.Width - 22 - RightOffset, 4, 22, 44);

            if(ImageChevron.Hidden)
                RightImage.Frame = new RectangleF(screenSize.Width - 44 - RightOffset, 4, 44, 44);
            else
                RightImage.Frame = new RectangleF(screenSize.Width - 66 - RightOffset, 4, 44, 44);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
            SelectedBackgroundView.Alpha = 1;
            SelectedBackgroundView.Hidden = !highlighted;
            TextLabel.TextColor = highlighted ? UIColor.White : UIColor.Black;
            DetailTextLabel.Highlighted = highlighted;
            IndexTextLabel.Highlighted = highlighted;
            DetailTextLabel.TextColor = highlighted ? UIColor.White : UIColor.Gray;
            //IndexTextLabel.TextColor = highlighted ? UIColor.White : UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);
            //SecondaryMenuBackground.BackgroundColor = highlighted ? GlobalTheme.SecondaryColor : GlobalTheme.LightColor;

            base.SetHighlighted(highlighted, animated);
        }

        public override void SetSelected(bool selected, bool animated)
        {
            //if(selected)
                //SecondaryMenuBackground.BackgroundColor = GlobalTheme.SecondaryColor;

            TextLabel.TextColor = selected ? UIColor.White : UIColor.Black;
            DetailTextLabel.TextColor = selected ? UIColor.White : UIColor.Gray;
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

        private bool _isTextLabelAllowedToChangeFrame = true;

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
