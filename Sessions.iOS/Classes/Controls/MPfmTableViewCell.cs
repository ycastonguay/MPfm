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

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsTableViewCell")]
    public class SessionsTableViewCell : UITableViewCell
    {
        private bool _isTextLabelAllowedToChangeFrame = true;

		public UIView ContainerView { get; private set; }
		public UIView ContainerBackgroundView { get; private set; }

		public UIView BehindView { get; private set; }
		public UIImageView ImageCheckmark { get; private set; }
		public UIImageView ImageCheckmarkConfirm { get; private set; }
		public UIImageView ImageAddToPlaylist { get; private set; }
		public UIImageView ImageAddedToPlaylist { get; private set; }
		public UILabel AddToPlaylistLabel { get; private set; }
		public UILabel AddedToPlaylistLabel { get; private set; }

        public UILabel IndexTextLabel { get; private set; }
        public UIButton RightButton { get; private set; }
        public UIImageView RightImage { get; private set; }
        public UIImageView ImageChevron { get; private set; }
        public UIImageView ImageAlbum1 { get; private set; }
        public UIImageView ImageAlbum2 { get; private set; }
        public UIImageView ImageAlbum3 { get; private set; }
        public UILabel AlbumCountLabel { get; private set; }

		public SessionsSecondaryMenuButton PlayButton { get; set; }
		public SessionsSecondaryMenuButton AddButton { get; set; }
		public SessionsSecondaryMenuButton DeleteButton { get; set; }

		public bool IsQueued { get; set; }
        public bool IsTextAnimationEnabled { get; set; }
		public bool IsDarkBackground { get; set; }

        public float RightOffset { get; set; }

        public delegate void RightButtonTap(SessionsTableViewCell cell);
        public event RightButtonTap OnRightButtonTap;

        public SessionsTableViewCell() : base()
        {
            Initialize();
        }

		// Keep this or cell reuse won't work for the first items
		public SessionsTableViewCell(IntPtr handle) : base(handle)
		{
			Initialize();
		}

        public SessionsTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        public SessionsTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public void Initialize()
        {
            IsTextAnimationEnabled = false;
            SelectionStyle = UITableViewCellSelectionStyle.None;
            var screenSize = UIKitHelper.GetDeviceSize();
           
			BehindView = new UIView(Bounds);
			BehindView.BackgroundColor = UIColor.FromRGB(47, 129, 183);
			AddSubview(BehindView);

			ImageAddToPlaylist = new UIImageView();
			ImageAddToPlaylist.Frame = new RectangleF(10, 14, 24, 24);
			ImageAddToPlaylist.Image = UIImage.FromBundle("Images/ContextualButtons/add");
			ImageAddToPlaylist.BackgroundColor = UIColor.Clear;
			ImageAddToPlaylist.Alpha = 0.1f;
			BehindView.AddSubview(ImageAddToPlaylist);

			ImageAddedToPlaylist = new UIImageView();
			ImageAddedToPlaylist.Frame = new RectangleF(10, 14, 24, 24);
			ImageAddedToPlaylist.Image = UIImage.FromBundle("Images/ContextualButtons/checkmark");
			ImageAddedToPlaylist.BackgroundColor = UIColor.Clear;
			ImageAddedToPlaylist.Alpha = 0f;
			//BehindView.AddSubview(ImageAddedToPlaylist);

			ImageCheckmark = new UIImageView();
			ImageCheckmark.Frame = new RectangleF((UIScreen.MainScreen.Bounds.Width / 2f) + 14f, 14, 24, 24);
			ImageCheckmark.Image = UIImage.FromBundle("Images/ContextualButtons/checkmark");
			ImageCheckmark.BackgroundColor = UIColor.Clear;
			ImageCheckmark.Alpha = 1f;
			BehindView.AddSubview(ImageCheckmark);

			ImageCheckmarkConfirm = new UIImageView();
			ImageCheckmarkConfirm.Frame = new RectangleF((UIScreen.MainScreen.Bounds.Width / 2f) + 14f, 14, 24, 24);
			ImageCheckmarkConfirm.Image = UIImage.FromBundle("Images/ContextualButtons/checkmark_nobg");
			ImageCheckmarkConfirm.BackgroundColor = UIColor.Clear;
			ImageCheckmarkConfirm.Alpha = 0f;
			BehindView.AddSubview(ImageCheckmarkConfirm);

			AddToPlaylistLabel = new UILabel();
			AddToPlaylistLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			AddToPlaylistLabel.Frame = new RectangleF(40, 10, 150, 32);
			AddToPlaylistLabel.Text = "Add to queue";
			AddToPlaylistLabel.BackgroundColor = UIColor.Clear;
			AddToPlaylistLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
			AddToPlaylistLabel.TextColor = UIColor.White;
			AddToPlaylistLabel.TextAlignment = UITextAlignment.Left;
			AddToPlaylistLabel.HighlightedTextColor = UIColor.White;
			BehindView.AddSubview(AddToPlaylistLabel);

			AddedToPlaylistLabel = new UILabel();
			AddedToPlaylistLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			AddedToPlaylistLabel.Frame = new RectangleF(12, 62, 150, 32);
			AddedToPlaylistLabel.Alpha = 0;
			AddedToPlaylistLabel.Text = "Added to queue!";
			AddedToPlaylistLabel.BackgroundColor = UIColor.Clear;
			AddedToPlaylistLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
			AddedToPlaylistLabel.TextColor = UIColor.White;
			AddedToPlaylistLabel.TextAlignment = UITextAlignment.Left;
			AddedToPlaylistLabel.HighlightedTextColor = UIColor.White;
			BehindView.AddSubview(AddedToPlaylistLabel);

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

            ImageAlbum1 = new UIImageView();
            ImageAlbum1.BackgroundColor = UIColor.Clear;
            ImageAlbum1.Hidden = true;
            ImageAlbum1.Alpha = 0.75f;
            ImageAlbum1.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44);
			ContainerView.AddSubview(ImageAlbum1);

            ImageAlbum2 = new UIImageView();
            ImageAlbum2.BackgroundColor = UIColor.Clear;
            ImageAlbum2.Hidden = true;
            ImageAlbum2.Alpha = 0.4f;
            ImageAlbum2.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 130, 4, 44, 44);
			ContainerView.AddSubview(ImageAlbum2);

            ImageAlbum3 = new UIImageView();
            ImageAlbum3.BackgroundColor = UIColor.Clear;
            ImageAlbum3.Hidden = true;
            ImageAlbum3.Alpha = 0.2f;
            ImageAlbum3.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 182, 4, 44, 44);
			ContainerView.AddSubview(ImageAlbum3);

            AlbumCountLabel = new UILabel();
            AlbumCountLabel.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44);
            AlbumCountLabel.Alpha = 0.75f;
            AlbumCountLabel.BackgroundColor = UIColor.Clear;
            AlbumCountLabel.Font = UIFont.FromName("HelveticaNeue-Light", 18);
            AlbumCountLabel.Hidden = true;
            AlbumCountLabel.Text = "+98";
            AlbumCountLabel.TextColor = UIColor.Black;
            AlbumCountLabel.TextAlignment = UITextAlignment.Center;
            AlbumCountLabel.HighlightedTextColor = UIColor.White;
			ContainerView.AddSubview(AlbumCountLabel);

            TextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
            TextLabel.BackgroundColor = UIColor.Orange;
            TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 14);
			TextLabel.TextColor = UIColor.Black;
			TextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			DetailTextLabel.TextColor = UIColor.Gray;
			DetailTextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.BackgroundColor = UIColor.Yellow;
            DetailTextLabel.Font = UIFont.FromName("HelveticaNeue", 12);
            ImageView.BackgroundColor = UIColor.Clear;

            // Make sure the text label is over all other subviews
            DetailTextLabel.RemoveFromSuperview();
            ImageView.RemoveFromSuperview();
			ContainerView.AddSubview(DetailTextLabel);
			ContainerView.AddSubview(ImageView);

            IndexTextLabel = new UILabel();
            IndexTextLabel.BackgroundColor = UIColor.Clear;
            IndexTextLabel.Font = UIFont.FromName("HelveticaNeue", 16);
            IndexTextLabel.TextColor = UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);
            IndexTextLabel.TextAlignment = UITextAlignment.Center;
            IndexTextLabel.HighlightedTextColor = UIColor.White;
			ContainerView.AddSubview(IndexTextLabel);

            RightButton = new UIButton(UIButtonType.Custom);
            RightButton.Hidden = true;
            RightButton.Frame = new RectangleF(screenSize.Width - Bounds.Height, 4, Bounds.Height, Bounds.Height);
            RightButton.TouchUpInside += HandleRightButtonTouchUpInside;
			ContainerView.AddSubview(RightButton);

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron"));
            ImageChevron.BackgroundColor = UIColor.Clear;
            ImageChevron.Hidden = true;
            ImageChevron.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 22, 4, 22, 44);
			ContainerView.AddSubview(ImageChevron);           

            RightImage = new UIImageView(UIImage.FromBundle("Images/Icons/icon_speaker"));
            RightImage.Alpha = 0.7f;
            RightImage.BackgroundColor = UIColor.Clear;
            RightImage.Hidden = true;
            RightImage.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 66, 4, 44, 44);
			ContainerView.AddSubview(RightImage);

            // Make sure the text label is over all other subviews
            TextLabel.RemoveFromSuperview();
			ContainerView.AddSubview(TextLabel);

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
			BehindView.Frame = Bounds;
			ContainerView.Frame = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
			ContainerBackgroundView.Frame = new RectangleF(IsQueued ? 4 : 0, 0, Bounds.Width, Bounds.Height);
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

			//if (_isTextLabelAllowedToChangeFrame)
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

//			ImageAlbum1.Frame = new RectangleF(screenSize.Width - 78, 4, 44, 44);
//			ImageAlbum2.Frame = new RectangleF(screenSize.Width - 130, 4, 44, 44);
//			ImageAlbum3.Frame = new RectangleF(screenSize.Width - 182, 4, 44, 44);
			//AlbumCountLabel.Frame = new RectangleF(screenSize.Width - 78, 4, 44, 44);

//			PlayButton.Frame = new RectangleF(4, 56, 100, 64);
//			AddButton.Frame = new RectangleF(108, 56, 100, 64);
//			DeleteButton.Frame = new RectangleF(212, 56, 100, 64);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
            SelectedBackgroundView.Alpha = 1;
            SelectedBackgroundView.Hidden = !highlighted;
			TextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Black;
            DetailTextLabel.Highlighted = highlighted;
            IndexTextLabel.Highlighted = highlighted;
			AlbumCountLabel.Highlighted = highlighted;
			DetailTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Gray;
			IndexTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);
			AlbumCountLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Black;

            base.SetHighlighted(highlighted, animated);
        }

        public override void SetSelected(bool selected, bool animated)
        {
            TextLabel.TextColor = selected ? UIColor.White : UIColor.Black;
            DetailTextLabel.TextColor = selected ? UIColor.White : UIColor.Gray;
			AlbumCountLabel.TextColor = selected ? UIColor.White : UIColor.Black;
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
                    if(TextLabel.Transform.xx < 0.95f) return;

                    TextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    IndexTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
					AlbumCountLabel.Transform = CGAffineTransform.MakeScale(1, 1);
					ImageAlbum1.Transform = CGAffineTransform.MakeScale(1, 1);
					ImageAlbum2.Transform = CGAffineTransform.MakeScale(1, 1);
					ImageAlbum3.Transform = CGAffineTransform.MakeScale(1, 1);
                }, null);
            }
            else
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    TextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    DetailTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                    IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
					AlbumCountLabel.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
					ImageAlbum1.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
					ImageAlbum2.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
					ImageAlbum3.Transform = CGAffineTransform.MakeScale(0.96f, 0.96f);
                }, null);
            }
        }
    }
}
