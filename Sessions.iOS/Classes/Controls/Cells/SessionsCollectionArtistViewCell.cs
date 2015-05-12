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
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Objects;

namespace Sessions.iOS.Classes.Controls.Cells
{
    [Register("SessionsCollectionArtistViewCell")]
    public class SessionsCollectionArtistViewCell : UICollectionViewCell
    {
        UIImageView _imageView;

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

        public SessionsCollectionArtistViewCell() : base()
        {
            Initialize();
        }

        public SessionsCollectionArtistViewCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        [Export ("initWithFrame:")]
        public SessionsCollectionArtistViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            //Frame = new RectangleF(0, 0, 160, 160);
            BackgroundView = new UIView{BackgroundColor = UIColor.Orange};
            SelectedBackgroundView = new UIView{BackgroundColor = UIColor.Blue};
            ContentView.BackgroundColor = UIColor.Yellow;

            _imageView = new UIImageView(new RectangleF(0, 0, Frame.Width, Frame.Height));
            _imageView.BackgroundColor = GlobalTheme.MainDarkColor;
            //_imageView.Image = UIImage.FromBundle("Images/Icons/app_icon_114");
            _imageView.Center = ContentView.Center;

            ContentView.AddSubview(_imageView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        }
    }
}
