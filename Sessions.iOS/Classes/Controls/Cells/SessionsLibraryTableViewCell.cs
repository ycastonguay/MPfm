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
using Sessions.iOS.Classes.Controls.Cells.Base;
using Sessions.iOS.Helpers;
using Sessions.iOS.Classes.Controls.Buttons;

namespace Sessions.iOS.Classes.Controls.Cells
{
    [Register("SessionsLibraryTableViewCell")]
    public class SessionsLibraryTableViewCell : SessionsBaseTableViewCell
    {        
		public UIImageView ImageCheckmark { get; private set; }
		public UIImageView ImageCheckmarkConfirm { get; private set; }
		public UIImageView ImageAddToPlaylist { get; private set; }
		public UIImageView ImageAddedToPlaylist { get; private set; }
		public UILabel AddToPlaylistLabel { get; private set; }
		public UILabel AddedToPlaylistLabel { get; private set; }

        public UILabel TitleTextLabel { get; private set; }
        public UILabel SubtitleTextLabel { get; private set; }
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
		public bool IsDarkBackground { get; set; }
        public float RightOffset { get; set; }

        public override bool UseContainerView { get { return true; } }

        public delegate void RightButtonTap(SessionsLibraryTableViewCell cell);
        public event RightButtonTap OnRightButtonTap;

        public SessionsLibraryTableViewCell() : base()
        {
        }

		public SessionsLibraryTableViewCell(IntPtr handle) : base(handle)
		{
		}

        public SessionsLibraryTableViewCell(CGRect frame) : base(frame)
        {
        }

        public SessionsLibraryTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsTextAnimationEnabled = true;

            CreateContainerViewControls();
            CreateBehindViewControls();
            CreateDrawerControls();

            ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
        }

        private void CreateContainerViewControls()
        {
            ImageAlbum1 = new UIImageView();
            ImageAlbum1.BackgroundColor = UIColor.Clear;
            ImageAlbum1.Hidden = true;
            ImageAlbum1.Alpha = 0.75f;
            ImageAlbum1.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44);
            AddView(ImageAlbum1);

            ImageAlbum2 = new UIImageView();
            ImageAlbum2.BackgroundColor = UIColor.Clear;
            ImageAlbum2.Hidden = true;
            ImageAlbum2.Alpha = 0.4f;
            ImageAlbum2.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - 130, 4, 44, 44);
            AddView(ImageAlbum2);

            ImageAlbum3 = new UIImageView();
            ImageAlbum3.BackgroundColor = UIColor.Clear;
            ImageAlbum3.Hidden = true;
            ImageAlbum3.Alpha = 0.2f;
            ImageAlbum3.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - 182, 4, 44, 44);
            AddView(ImageAlbum3);

            AlbumCountLabel = new UILabel();
            AlbumCountLabel.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44);
            AlbumCountLabel.Alpha = 0.75f;
            AlbumCountLabel.BackgroundColor = UIColor.Clear;
            AlbumCountLabel.Font = UIFont.FromName("HelveticaNeue-Light", 18);
            AlbumCountLabel.Hidden = true;
            AlbumCountLabel.Text = "+98";
            AlbumCountLabel.TextColor = UIColor.Black;
            AlbumCountLabel.TextAlignment = UITextAlignment.Center;
            AlbumCountLabel.HighlightedTextColor = UIColor.White;
            AddView(AlbumCountLabel);

            TitleTextLabel = new UILabel();
            TitleTextLabel.Layer.AnchorPoint = new CGPoint(0, 0.5f);
            TitleTextLabel.BackgroundColor = UIColor.Clear;
            TitleTextLabel.Font = UIFont.FromName("HelveticaNeue", 14);
            TitleTextLabel.TextColor = UIColor.Black;
            TitleTextLabel.HighlightedTextColor = UIColor.White;

            SubtitleTextLabel = new UILabel();
            SubtitleTextLabel.Layer.AnchorPoint = new CGPoint(0, 0.5f);
            SubtitleTextLabel.TextColor = UIColor.Gray;
            SubtitleTextLabel.HighlightedTextColor = UIColor.White;
            SubtitleTextLabel.BackgroundColor = UIColor.Clear;
            SubtitleTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 12);

            ImageView.BackgroundColor = UIColor.Clear;
            ImageView.AutoresizingMask = UIViewAutoresizing.None;
            ImageView.ClipsToBounds = true;

            // Make sure the text label is over all other subviews
            ImageView.RemoveFromSuperview();
            AddView(SubtitleTextLabel);
            AddView(ImageView);

            IndexTextLabel = new UILabel();
            IndexTextLabel.BackgroundColor = UIColor.Clear;
            IndexTextLabel.Font = UIFont.FromName("HelveticaNeue", 16);
            IndexTextLabel.TextColor = UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);
            IndexTextLabel.TextAlignment = UITextAlignment.Center;
            IndexTextLabel.HighlightedTextColor = UIColor.White;
            AddView(IndexTextLabel);

            RightButton = new UIButton(UIButtonType.Custom);
            RightButton.Hidden = true;
            RightButton.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - Bounds.Height, 4, Bounds.Height, Bounds.Height);
            RightButton.TouchUpInside += HandleRightButtonTouchUpInside;
            AddView(RightButton);

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron"));
            ImageChevron.BackgroundColor = UIColor.Clear;
            ImageChevron.Hidden = true;
            ImageChevron.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - 22, 4, 22, 44);
            AddView(ImageChevron);           

            RightImage = new UIImageView(UIImage.FromBundle("Images/Icons/icon_speaker"));
            RightImage.Alpha = 0.7f;
            RightImage.BackgroundColor = UIColor.Clear;
            RightImage.Hidden = true;
            RightImage.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - 66, 4, 44, 44);
            AddView(RightImage);

            // Make sure the text label is over all other subviews
            AddView(TitleTextLabel);
        }

        private void CreateBehindViewControls()
        {
            ImageAddToPlaylist = new UIImageView();
            ImageAddToPlaylist.Frame = new CGRect(10, 14, 24, 24);
            ImageAddToPlaylist.Image = UIImage.FromBundle("Images/ContextualButtons/add");
            ImageAddToPlaylist.BackgroundColor = UIColor.Clear;
            ImageAddToPlaylist.Alpha = 0.1f;
            BehindView.AddSubview(ImageAddToPlaylist);

            ImageAddedToPlaylist = new UIImageView();
            ImageAddedToPlaylist.Frame = new CGRect(10, 14, 24, 24);
            ImageAddedToPlaylist.Image = UIImage.FromBundle("Images/ContextualButtons/checkmark");
            ImageAddedToPlaylist.BackgroundColor = UIColor.Clear;
            ImageAddedToPlaylist.Alpha = 0f;
            //BehindView.AddSubview(ImageAddedToPlaylist);

            ImageCheckmark = new UIImageView();
            ImageCheckmark.Frame = new CGRect((UIScreen.MainScreen.Bounds.Width / 2f) + 14f, 14, 24, 24);
            ImageCheckmark.Image = UIImage.FromBundle("Images/ContextualButtons/checkmark");
            ImageCheckmark.BackgroundColor = UIColor.Clear;
            ImageCheckmark.Alpha = 1f;
            BehindView.AddSubview(ImageCheckmark);

            ImageCheckmarkConfirm = new UIImageView();
            ImageCheckmarkConfirm.Frame = new CGRect((UIScreen.MainScreen.Bounds.Width / 2f) + 14f, 14, 24, 24);
            ImageCheckmarkConfirm.Image = UIImage.FromBundle("Images/ContextualButtons/checkmark_nobg");
            ImageCheckmarkConfirm.BackgroundColor = UIColor.Clear;
            ImageCheckmarkConfirm.Alpha = 0f;
            BehindView.AddSubview(ImageCheckmarkConfirm);

            AddToPlaylistLabel = new UILabel();
            AddToPlaylistLabel.Layer.AnchorPoint = new CGPoint(0, 0.5f);
            AddToPlaylistLabel.Frame = new CGRect(40, 10, 150, 32);
            AddToPlaylistLabel.Text = "Add to queue";
            AddToPlaylistLabel.BackgroundColor = UIColor.Clear;
            AddToPlaylistLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            AddToPlaylistLabel.TextColor = UIColor.White;
            AddToPlaylistLabel.TextAlignment = UITextAlignment.Left;
            AddToPlaylistLabel.HighlightedTextColor = UIColor.White;
            BehindView.AddSubview(AddToPlaylistLabel);

            AddedToPlaylistLabel = new UILabel();
            AddedToPlaylistLabel.Layer.AnchorPoint = new CGPoint(0, 0.5f);
            AddedToPlaylistLabel.Frame = new CGRect(12, 62, 150, 32);
            AddedToPlaylistLabel.Alpha = 0;
            AddedToPlaylistLabel.Text = "Added to queue!";
            AddedToPlaylistLabel.BackgroundColor = UIColor.Clear;
            AddedToPlaylistLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            AddedToPlaylistLabel.TextColor = UIColor.White;
            AddedToPlaylistLabel.TextAlignment = UITextAlignment.Left;
            AddedToPlaylistLabel.HighlightedTextColor = UIColor.White;
            BehindView.AddSubview(AddedToPlaylistLabel);
        }

        private void CreateDrawerControls()
        {
            // Maybe add icons only to iPad where there is enough space
            PlayButton = new SessionsSecondaryMenuButton();
            PlayButton.Frame = new CGRect(4, 53, 100, 64);
            PlayButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/play"), UIControlState.Normal);
            PlayButton.SetTitle("Play", UIControlState.Normal); 
            PlayButton.Font = UIFont.FromName("HelveticaNeue-Light", 12f);
            PlayButton.Alpha = 0;
            ContainerView.AddSubview(PlayButton);

            AddButton = new SessionsSecondaryMenuButton();
            AddButton.Frame = new CGRect(108, 53, 100, 64);
            AddButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/add"), UIControlState.Normal);
            AddButton.SetTitle("Add to playlist", UIControlState.Normal);
            AddButton.Font = UIFont.FromName("HelveticaNeue-Light", 12f);
            AddButton.Alpha = 0;
            ContainerView.AddSubview(AddButton);

            DeleteButton = new SessionsSecondaryMenuButton();
            DeleteButton.Frame = new CGRect(212, 53, 100, 64);
            DeleteButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/trash"), UIControlState.Normal);
            DeleteButton.SetTitle("Delete", UIControlState.Normal);
            DeleteButton.Font = UIFont.FromName("HelveticaNeue-Light", 12f);
            DeleteButton.Alpha = 0;
            ContainerView.AddSubview(DeleteButton);
        }

        public void SetIndexText(string text)
        {
            IndexTextLabel.Text = text;
            IndexTextLabel.Font = UIFont.FromName("HelveticaNeue", text.Length > 1 ? 12 : 16);
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
            var screenSize = UIKitHelper.GetDeviceSize();

			ContainerBackgroundView.Frame = new CGRect(IsQueued ? 4 : 0, 0, Bounds.Width, Bounds.Height);

            // Determine width available for text
            nfloat textWidth = Bounds.Width;
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
                ImageView.Frame = new CGRect(x, 4, 44, 44);
                x += 44 + padding;
            } 
            else if (!string.IsNullOrEmpty(IndexTextLabel.Text))
            {
                x += padding;
                IndexTextLabel.Frame = new CGRect(x, 6, 22, 38);
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
                TitleTextLabel.Frame = new CGRect(x, titleY, textWidth, 22);
            if (!string.IsNullOrEmpty(SubtitleTextLabel.Text))
                SubtitleTextLabel.Frame = new CGRect(x, 22 + 4, textWidth, 16);

            if (RightButton.ImageView.Image != null || !string.IsNullOrEmpty(RightButton.Title(UIControlState.Normal)))
                RightButton.Frame = new CGRect(screenSize.Width - 44, 4, 44, 44);

            ImageChevron.Frame = new CGRect(screenSize.Width - 22 - RightOffset, 4, 22, 44);

            if(ImageChevron.Hidden)
                RightImage.Frame = new CGRect(screenSize.Width - 44 - RightOffset, 4, 44, 44);
            else
                RightImage.Frame = new CGRect(screenSize.Width - 66 - RightOffset, 4, 44, 44);
        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
            SelectedBackgroundView.Alpha = 1;
            SelectedBackgroundView.Hidden = !highlighted;
			TitleTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Black;
            SubtitleTextLabel.Highlighted = highlighted;
            IndexTextLabel.Highlighted = highlighted;
			AlbumCountLabel.Highlighted = highlighted;
			SubtitleTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Gray;
			IndexTextLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);
			AlbumCountLabel.TextColor = highlighted ? UIColor.White : IsDarkBackground ? UIColor.White : UIColor.Black;

            base.SetHighlighted(highlighted, animated);
        }

        public override void SetSelected(bool selected, bool animated)
        {
            TitleTextLabel.TextColor = selected ? UIColor.White : UIColor.Black;
            SubtitleTextLabel.TextColor = selected ? UIColor.White : UIColor.Gray;
			AlbumCountLabel.TextColor = selected ? UIColor.White : UIColor.Black;
//            IndexTextLabel.TextColor = selected ? UIColor.White : UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 1);

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
            IndexTextLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
            AlbumCountLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
            ImageAlbum1.Transform = CGAffineTransform.MakeScale(scale, scale);
            ImageAlbum2.Transform = CGAffineTransform.MakeScale(scale, scale);
            ImageAlbum3.Transform = CGAffineTransform.MakeScale(scale, scale);
        }
    }
}
