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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.Library.UpdateLibrary;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.MVP.Models;
using Sessions.MVP.Views;
using Sessions.iOS.Classes.Objects;
using Sessions.Library.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.iOS.Classes.Delegates;

namespace Sessions.iOS
{
    public partial class UpdateLibraryViewController : BaseViewController, IUpdateLibraryView
    {
        public UpdateLibraryViewController()
			: base (UserInterfaceIdiomIsPhone ? "UpdateLibraryViewController_iPhone" : "UpdateLibraryViewController_iPad", null)
        {
        }
		
        public override void ViewDidLoad()
        {
            View.BackgroundColor = GlobalTheme.BackgroundColor;
			//btnClose.GlyphImageView.Image = UIImage.FromBundle("Images/Player/down");

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindUpdateLibraryView(this);
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			OnStartUpdateLibrary();
		}

		partial void actionClose(NSObject sender)
		{
			Close();
		}

		private void Close()
		{
			//OnCancelUpdateLibrary();
			var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			appDelegate.RemoveChildFromMainViewController(this);
		}

        #region IUpdateLibraryView implementation
        
		public Action<List<string>> OnAddFilesToLibrary { get; set; }
		public Action<string> OnAddFolderToLibrary { get; set; }
        public Action OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }
		public Action<string> OnSaveLog { get; set; }
        
		private float _percentageDone;
        public void RefreshStatus(UpdateLibraryEntity entity)
        {
            InvokeOnMainThread(() => {
				if(lblSubtitle.Text == entity.Title)
					return;

				lblTitle.Text = "Updating library";
				lblSubtitle.Text = entity.Title + "...";
            });
        }
        
        public void AddToLog(string entry)
        {
        }

		public void ProcessStarted()
		{
			InvokeOnMainThread(() => {
				lblTitle.Text = "Initializing...";
				lblSubtitle.Text = string.Empty;
				btnClose.Alpha = 1;
				lblTitle.Alpha = 1;
				activityIndicator.StartAnimating();
				activityIndicator.Alpha = 1;					
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;

				if(!UserInterfaceIdiomIsPhone)
					lblTitle.Frame = new RectangleF(46, lblTitle.Frame.Y, lblTitle.Frame.Width, lblTitle.Frame.Height);
				else
					lblTitle.Frame = new RectangleF(44, lblTitle.Frame.Y, lblTitle.Frame.Width, lblTitle.Frame.Height);
			});
		}
        
        public void ProcessEnded(bool canceled)
        {
            InvokeOnMainThread(() => {
                lblTitle.Text = "Update library successful.";
                lblSubtitle.Text = string.Empty;
				//button.SetTitle("OK", UIControlState.Normal);
                activityIndicator.StopAnimating();
				//activityIndicator.Hidden = true;
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;

				UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
					activityIndicator.Alpha = 0;
					btnClose.Alpha = 0;
					lblTitle.Frame = new RectangleF(12, lblTitle.Frame.Y, lblTitle.Frame.Width, lblTitle.Frame.Height);
				}, () => {
					UIView.Animate(1.0, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
						//btnClose.Alpha = 0;
						lblTitle.Alpha = 0;
					}, () => {
						Close();
					});
				});
            });
        }
        
        #endregion
    }
}

