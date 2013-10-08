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
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmCollectionAlbumViewCell")]
    public class MPfmCollectionAlbumViewCell : UICollectionViewCell
    {
        UIView _labelBackgroundView;
        UIImageView _imageView;
        UILabel _lblTitle;
        UILabel _lblSubtitle;

        public MPfmImageButton PlayButton { get; set; }
        public MPfmImageButton AddButton { get; set; }
        public MPfmImageButton DeleteButton { get; set; }

        public UIImage Image
        {
            get 
            {
                return _imageView.Image;
            }
            set 
            {
                _imageView.Image = value;
            }
        }

        public string Title
        {
            get
            {
                return _lblTitle.Text;
            }
            set
            {
                _lblTitle.Text = value;
            }
        }

        public string Subtitle
        {
            get
            {
                return _lblSubtitle.Text;
            }
            set
            {
                _lblSubtitle.Text = value;
            }
        }

        public MPfmCollectionAlbumViewCell() : base()
        {
            Initialize();
        }

        public MPfmCollectionAlbumViewCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        [Export ("initWithFrame:")]
        public MPfmCollectionAlbumViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            //Frame = new RectangleF(0, 0, 160, 160);
            BackgroundView = new UIView{BackgroundColor = GlobalTheme.BackgroundColor};
            SelectedBackgroundView = new UIView{BackgroundColor = GlobalTheme.SecondaryColor};
            ContentView.BackgroundColor = GlobalTheme.BackgroundColor;
            ContentView.AutosizesSubviews = true;

            _labelBackgroundView = new UIView(new RectangleF(0, Frame.Height - 36, Frame.Width, 36));
            _labelBackgroundView.BackgroundColor = new UIColor(0, 0, 0, 0.5f);

            _lblTitle = new UILabel(new RectangleF(8, Frame.Height - 36, Frame.Width - 16, 20));
            _lblTitle.BackgroundColor = UIColor.Clear;
            _lblTitle.Font = UIFont.FromName("HelveticaNeue", 12);
            _lblTitle.TextColor = UIColor.White;
            _lblTitle.TextAlignment = UITextAlignment.Center;

            _lblSubtitle = new UILabel(new RectangleF(8, Frame.Height - 20, Frame.Width - 16, 18));
            _lblSubtitle.BackgroundColor = UIColor.Clear;
            _lblSubtitle.Font = UIFont.FromName("HelveticaNeue-Light", 12);
            _lblSubtitle.TextColor = new UIColor(0.8f, 0.8f, 0.8f, 1);
            _lblSubtitle.TextAlignment = UITextAlignment.Center;
           
            PlayButton = new MPfmImageButton(new RectangleF(((Frame.Width - 44) / 2) - 52, (Frame.Height - 44) / 2, 44, 44));
            PlayButton.BackgroundColor = UIColor.FromRGBA(80, 80, 80, 225);
            PlayButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/play"), UIControlState.Normal);
            PlayButton.Layer.CornerRadius = 4;
            PlayButton.Alpha = 0;

            AddButton = new MPfmImageButton(new RectangleF((Frame.Width - 44) / 2, (Frame.Height - 44) / 2, 44, 44));
            AddButton.BackgroundColor = UIColor.FromRGBA(80, 80, 80, 225);
            AddButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/add"), UIControlState.Normal);
            AddButton.Layer.CornerRadius = 4;
            AddButton.Alpha = 0;

            DeleteButton = new MPfmImageButton(new RectangleF(((Frame.Width - 44) / 2) + 52, (Frame.Height - 44) / 2, 44, 44));
            DeleteButton.BackgroundColor = UIColor.FromRGBA(80, 80, 80, 225);
            DeleteButton.SetImage(UIImage.FromBundle("Images/ContextualButtons/trash"), UIControlState.Normal);
            DeleteButton.Layer.CornerRadius = 4;
            DeleteButton.Alpha = 0;

            _imageView = new UIImageView(new RectangleF(0, 0, Frame.Width, Frame.Height));
            _imageView.Alpha = 0;
            _imageView.BackgroundColor = GlobalTheme.BackgroundColor;
            _imageView.Center = ContentView.Center;
            _imageView.AutoresizingMask = UIViewAutoresizing.None;
            _imageView.ClipsToBounds = true;

            ContentView.AddSubview(_imageView);
            ContentView.AddSubview(_labelBackgroundView);
            ContentView.AddSubview(_lblSubtitle);
            ContentView.AddSubview(_lblTitle);
            ContentView.AddSubview(PlayButton);
            ContentView.AddSubview(AddButton);
            ContentView.AddSubview(DeleteButton);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        }

        public void SetImage(UIImage image)
        {
            _imageView.Alpha = 0;
            _imageView.Image = image;
            UIView.Animate(0.2, () => {
                _imageView.Alpha = 1;
            });
        }

        public void SetHighlight(bool highlighted)
        {
            if (highlighted)
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    _labelBackgroundView.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                    _lblTitle.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                    _lblSubtitle.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);

                    _imageView.Frame = new RectangleF(8, 8, Frame.Width - 16, Frame.Height - 16);
                    _labelBackgroundView.Frame = new RectangleF(_labelBackgroundView.Frame.X - 8, _labelBackgroundView.Frame.Y - 2, _labelBackgroundView.Frame.Width + 16, _labelBackgroundView.Frame.Height - 2);
                    _lblTitle.Frame = new RectangleF(_lblTitle.Frame.X, _lblTitle.Frame.Y - 1, _lblTitle.Frame.Width, _lblTitle.Frame.Height - 1);
                    _lblSubtitle.Frame = new RectangleF(_lblSubtitle.Frame.X, _lblSubtitle.Frame.Y - 4, _lblSubtitle.Frame.Width, _lblSubtitle.Frame.Height - 2);
                }, null);
            }
            else
            {
                // TODO: When quick tapping the cell, the animation will start right away from the "pressed" state. Try to find a way to not animate the cell.
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    // Don't change the order, it is important to set the frame before transform!
                    _labelBackgroundView.Frame = new RectangleF(_labelBackgroundView.Frame.X + 8, _labelBackgroundView.Frame.Y + 2, _labelBackgroundView.Frame.Width - 16, _labelBackgroundView.Frame.Height + 2);
                    _lblTitle.Frame = new RectangleF(_lblTitle.Frame.X, _lblTitle.Frame.Y + 1, _lblTitle.Frame.Width, _lblTitle.Frame.Height + 1);
                    _lblSubtitle.Frame = new RectangleF(_lblSubtitle.Frame.X, _lblSubtitle.Frame.Y + 4, _lblSubtitle.Frame.Width, _lblSubtitle.Frame.Height + 2);
                    _imageView.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);

                    _labelBackgroundView.Transform = CGAffineTransform.MakeScale(1, 1);
                    _lblTitle.Transform = CGAffineTransform.MakeScale(1, 1);
                    _lblSubtitle.Transform = CGAffineTransform.MakeScale(1, 1);
                }, null);
            }
        }       
    }
}
