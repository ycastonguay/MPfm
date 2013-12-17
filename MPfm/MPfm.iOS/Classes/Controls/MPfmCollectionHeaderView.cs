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
	[Register("MPfmCollectionHeaderView")]
	public class MPfmCollectionHeaderView : UICollectionReusableView
    {
		public UILabel TextLabel { get; set; }
		public MPfmFlatButton PlayButton { get; set; }

		public MPfmCollectionHeaderView() : base()
        {
            Initialize();
        }

		public MPfmCollectionHeaderView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        [Export ("initWithFrame:")]
		public MPfmCollectionHeaderView(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
			BackgroundColor = GlobalTheme.MainDarkColor;

			float fontSize = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? 16f : 18f;
			TextLabel = new UILabel();
			TextLabel.BackgroundColor = UIColor.Clear;
			TextLabel.Font = UIFont.FromName("HelveticaNeue", fontSize);
			TextLabel.TextColor = UIColor.White;
			TextLabel.LineBreakMode = UILineBreakMode.TailTruncation;
			AddSubview(TextLabel);

			PlayButton = new MPfmFlatButton();
			PlayButton.Label.Text = "Play all";
			//PlayButton.GlyphImageView.Image = UIImage.FromBundle("Images/Player/play");
			//PlayButton.TouchUpInside += HandleOnPlayButtonClick;
			PlayButton.LabelAlignment = UIControlContentHorizontalAlignment.Right;
			PlayButton.Label.TextAlignment = UITextAlignment.Right;
			PlayButton.Label.Frame = new RectangleF(0, 0, 54, 44);
			PlayButton.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_blue");
			PlayButton.ImageChevron.Frame = new RectangleF(80 - 22, 0, 22, 44);
			PlayButton.OnButtonClick += () => {
			};

			AddSubview(PlayButton);
        }

        public override void LayoutSubviews()
        {
			float y = (Bounds.Height - 44) / 2f;
			TextLabel.Frame = new RectangleF(y * 2, 0, Bounds.Width - 80 - 12, Bounds.Height);
			//PlayButton.Frame = new RectangleF(Bounds.Width - 44 - y, y, 44, 44);
			PlayButton.Frame = new RectangleF(Bounds.Width - 80, y, 80, 44);
        }

		private void HandleOnPlayButtonClick(object sender, EventArgs e)
		{
		}
    }
}
