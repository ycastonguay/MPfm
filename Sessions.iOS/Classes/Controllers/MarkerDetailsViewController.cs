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
using System.Drawing;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.Sound.Objects;
using Sessions.iOS.Classes.Controls.Buttons;

namespace Sessions.iOS.Classes.Controllers
{
    public partial class MarkerDetailsViewController : BaseViewController, IMarkerDetailsView
    {
        Guid _markerId = Guid.Empty;
        Marker _marker = null;
        UIBarButtonItem _btnDone;

        public MarkerDetailsViewController(Guid markerId)
            : base (UserInterfaceIdiomIsPhone ? "MarkerDetailsViewController_iPhone" : "MarkerDetailsViewController_iPad", null)
        {
            _markerId = markerId;
        }
        
        public override void ViewDidLoad()
        {
            this.View.BackgroundColor = GlobalTheme.BackgroundColor;
            btnDeleteMarker.BackgroundColor = GlobalTheme.SecondaryColor;
            btnDeleteMarker.Layer.CornerRadius = 8;

            sliderPosition.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            sliderPosition.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            sliderPosition.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);

            sliderPosition.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
            sliderPosition.Frame = new RectangleF(90, sliderPosition.Frame.Y + 1, sliderPosition.Frame.Width * 1.4f, sliderPosition.Frame.Height);

            // Add padding to text field (http://stackoverflow.com/questions/3727068/set-padding-for-uitextfield-with-uitextborderstylenone)
            UIView paddingView = new UIView(new RectangleF(0, 0, 5, 20));
            txtName.LeftView = paddingView;
            txtName.LeftViewMode = UITextFieldViewMode.Always;

            // Make sure the Done key closes the keyboard
            txtName.ShouldReturn = (a) => {
                txtName.ResignFirstResponder();
                return true;
            };
            textViewComments.ShouldBeginEditing = (a) => {
                UIView.Animate(0.2f, () => {
                    View.Bounds = new RectangleF(View.Bounds.X, View.Bounds.Y + 100, View.Bounds.Width, View.Bounds.Height);
                });
                return true;
            };
            textViewComments.ShouldEndEditing = (a) => {
                textViewComments.ResignFirstResponder();
                UIView.Animate(0.2f, () => {
                    View.Bounds = new RectangleF(View.Bounds.X, View.Bounds.Y - 100, View.Bounds.Width, View.Bounds.Height);
                });
                return true;
            };

            sliderPosition.ValueChanged += HandleSliderPositionValueChanged;

            var btnDone = new SessionsFlatButton();
            btnDone.Label.Text = "Done";
            btnDone.Frame = new RectangleF(0, 0, 70, 44);
            btnDone.OnButtonClick += () => {
                _marker.Name = txtName.Text;
                _marker.Comments = textViewComments.Text;
                _marker.Position = lblPosition.Text;
				OnUpdateMarkerDetails(_marker);
                this.DismissViewController(true, null);
            };
            var btnBackView = new UIView(new RectangleF(0, 0, 70, 44));
            var rect = new RectangleF(btnBackView.Bounds.X + 5, btnBackView.Bounds.Y, btnBackView.Bounds.Width, btnBackView.Bounds.Height);
            btnBackView.Bounds = rect;
            btnBackView.AddSubview(btnDone);
            _btnDone = new UIBarButtonItem(btnBackView);
            NavigationItem.SetLeftBarButtonItem(_btnDone, true);
            
            var navCtrl = (SessionsNavigationController)NavigationController;
            navCtrl.SetBackButtonVisible(false);
            navCtrl.SetTitle("Marker Details");

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindMarkerDetailsView(this, _markerId);
        }

        private void HandleSliderPositionValueChanged(object sender, EventArgs e)
        {
            // Calculate new position by sending a request to the presenter
			OnChangePositionMarkerDetails(sliderPosition.Value);
        }

        partial void actionDeleteMarker(NSObject sender)
        {
			if(OnDeleteMarkerDetails != null)
				OnDeleteMarkerDetails();
        }

        #region IMarkerDetailsView implementation

		public Action<float> OnChangePositionMarkerDetails { get; set; }
		public Action<Marker> OnUpdateMarkerDetails { get; set; }
		public Action OnDeleteMarkerDetails { get; set; }
		public Action OnPunchInMarkerDetails { get; set; }

        public void MarkerDetailsError(Exception ex)
        {
            InvokeOnMainThread(() => {
                UIAlertView alertView = new UIAlertView("Marker Details Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

		public void DismissMarkerDetailsView()
        {
            InvokeOnMainThread(() => {
                DismissViewController(true, null);
            });
        }

		public void RefreshMarker(Marker marker, AudioFile audioFile)
        {
            InvokeOnMainThread(() => {
                _marker = marker;
                txtName.Text = marker.Name;
                textViewComments.Text = marker.Comments;
                //lblPosition.Text = marker.Position;
                lblLength.Text = audioFile.Length;
            });
        }

		public void RefreshMarkerPosition(string position, float positionPercentage)
        {
            InvokeOnMainThread(() => {
                lblPosition.Text = position;
                sliderPosition.Value = positionPercentage;
            });
        }

        #endregion
    }
}

