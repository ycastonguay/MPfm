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
using MonoTouch.CoreGraphics;
using MPfm.iOS.Classes.Objects;
using MPfm.Core;

namespace MPfm.iOS.Classes.Controls
{
	[Register("MPfmAlbumHeaderView")]
	public class MPfmAlbumHeaderView : UITableViewHeaderFooterView
    {
		public UIImageView AlbumImageView { get; private set; }
		public UILabel ArtistNameLabel { get; private set; }
		public UILabel AlbumTitleLabel { get; private set; }
		public UILabel TotalTimeLabel { get; private set; }
		public UILabel SongCountLabel { get; private set; }

		public MPfmAlbumHeaderView() : base()
		{
			Initialize();
		}

		public MPfmAlbumHeaderView(IntPtr handle) : base(handle)
        {
			Initialize();
        }

		private void Initialize()
		{
			ContentView.BackgroundColor = UIColor.Clear;

			var view = new UIView(Bounds);
			view.BackgroundColor = GlobalTheme.BackgroundDarkColor.ColorWithAlpha(0.95f);
			BackgroundView = view;

			ArtistNameLabel = new UILabel();
			ArtistNameLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			ArtistNameLabel.BackgroundColor = UIColor.Clear;
			ArtistNameLabel.TextColor = UIColor.White;
			ArtistNameLabel.HighlightedTextColor = UIColor.White;
			ContentView.AddSubview(ArtistNameLabel);

			AlbumTitleLabel = new UILabel();
			AlbumTitleLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			AlbumTitleLabel.BackgroundColor = UIColor.Clear;
			AlbumTitleLabel.TextColor = UIColor.White;
			AlbumTitleLabel.HighlightedTextColor = UIColor.White;
			ContentView.AddSubview(AlbumTitleLabel);

			TotalTimeLabel = new UILabel();
			TotalTimeLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			TotalTimeLabel.BackgroundColor = UIColor.Clear;
			TotalTimeLabel.TextColor = UIColor.White;
			TotalTimeLabel.HighlightedTextColor = UIColor.White;
			ContentView.AddSubview(TotalTimeLabel);

			SongCountLabel = new UILabel();
			SongCountLabel.Layer.AnchorPoint = new PointF(0, 0.5f);
			SongCountLabel.BackgroundColor = UIColor.Clear;
			SongCountLabel.TextColor = UIColor.White;
			SongCountLabel.HighlightedTextColor = UIColor.White;
			ContentView.AddSubview(SongCountLabel);

			AlbumImageView = new UIImageView();
			AlbumImageView.BackgroundColor = GlobalTheme.BackgroundDarkerColor.ColorWithAlpha(0.7f);
			ContentView.AddSubview(AlbumImageView);

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				ArtistNameLabel.Font = UIFont.FromName("HelveticaNeue-Light", 18);
				AlbumTitleLabel.Font = UIFont.FromName("HelveticaNeue", 17);
				TotalTimeLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
				SongCountLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
			}
			else
			{
				ArtistNameLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 14);
				AlbumTitleLabel.Font = UIFont.FromName("HelveticaNeue", 14);
				TotalTimeLabel.Font = UIFont.FromName("HelveticaNeue-Light", 13);
				SongCountLabel.Font = UIFont.FromName("HelveticaNeue-Light", 13);
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			BackgroundView.Frame = Bounds;

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				//float x = AlbumImageView.Image == null ? 4 : 92;
				float x = 104;
				ArtistNameLabel.Frame = new RectangleF(x, 4, Bounds.Width - 80 - 28, 24);
				AlbumTitleLabel.Frame = new RectangleF(x, 29, Bounds.Width - 80 - 28, 22);
				SongCountLabel.Frame = new RectangleF(x, 52, Bounds.Width - 80 - 28, 18);
				TotalTimeLabel.Frame = new RectangleF(x, 72, Bounds.Width - 80 - 28, 18);
				AlbumImageView.Frame = new RectangleF(0, 0, 96, 96);
			}
			else
			{
				//float x = AlbumImageView.Image == null ? 4 : 92;
				float x = 92;
				ArtistNameLabel.Frame = new RectangleF(x, 4, Bounds.Width - 80 - 28, 18);
				AlbumTitleLabel.Frame = new RectangleF(x, 23, Bounds.Width - 80 - 28, 18);
				SongCountLabel.Frame = new RectangleF(x, 42, Bounds.Width - 80 - 28, 18);
				TotalTimeLabel.Frame = new RectangleF(x, 60, Bounds.Width - 80 - 28, 18);
				AlbumImageView.Frame = new RectangleF(0, 0, 84, 84);
			}
		}
    }
}
