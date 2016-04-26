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
using Foundation;
using UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.MVP.Views;
using Sessions.MVP.Navigation;
using Sessions.MVP.Bootstrap;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;

namespace Sessions.iOS.Classes.Controllers
{
	public partial class QueueViewController : BaseViewController, IQueueView
    {
        public QueueViewController()
			: base(UserInterfaceIdiomIsPhone ? "QueueViewController_iPhone" : "QueueViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
			View.BackgroundColor = GlobalTheme.AlternateColor1.ColorWithAlpha(0.95f);
			btnPlay.GlyphImageView.Image = UIImage.FromBundle("Images/Player/play");
			btnRemoveAll.GlyphImageView.Image = UIImage.FromBundle("Images/Player/close");
			SetButtonColors();

			lblTitle.Text = "Queue";
			lblSubtitle.Text = "0 songs / 0:00";

			lblTitle.TextColor = UIKitHelper.ColorWithBrightness(GlobalTheme.AlternateColor1, 5f);
			lblSubtitle.TextColor = UIColor.White;

			lblTitle.Font = UIFont.FromName("HelveticaNeue-Light", 16);
			lblSubtitle.Font = UIFont.FromName("HelveticaNeue", 14);

			btnPlay.TouchUpInside += (sender, e) => {
				OnQueueStartPlayback();
				Close();
			};
			btnRemoveAll.TouchUpInside += (sender, e) => Close();

            base.ViewDidLoad();
			
			var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
			navigationManager.BindQueueView(this);
		}

		private void SetButtonColors()
		{
			var fillColor = UIKitHelper.ColorWithBrightness(GlobalTheme.AlternateColor1, 0.825f);
			var fillColorOn = UIColor.White.ColorWithAlpha(0.25f);
			//var strokeColor = UIKitHelper.ColorWithBrightness(GlobalTheme.AlternateColor1, 0.725f);
			var strokeColor = UIColor.White.ColorWithAlpha(0.2f);
			var strokeColorOn = UIColor.White.ColorWithAlpha(0.45f);
			float glyphAlpha = 0.95f;
			float glyphAlphaOn = 1;

			btnPlay.SetTheme(fillColor, strokeColor, fillColorOn, strokeColorOn, glyphAlpha, glyphAlphaOn);
			btnRemoveAll.SetTheme(fillColor, strokeColor, fillColorOn, strokeColorOn, glyphAlpha, glyphAlphaOn);
		}

		private void Close()
		{
			UIView.Animate(0.2, () => {
				View.Alpha = 0;
			});
		}

		#region IQueueView implementation

		public Action OnQueueStartPlayback { get; set; }
		public Action OnQueueRemoveAll { get; set; }

		public void QueueError(Exception ex)
		{
			ShowErrorDialog(ex);
		}

		public void RefreshQueue(int songCount, string totalLength)
		{
			InvokeOnMainThread(() => lblSubtitle.Text = string.Format("{0} songs / {1}", songCount, totalLength));
		}

		#endregion
    }
}
