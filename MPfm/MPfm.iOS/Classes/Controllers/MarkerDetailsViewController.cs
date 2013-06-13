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
using System.Drawing;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS
{
    public partial class MarkerDetailsViewController : BaseViewController, IMarkerDetailsView
    {
        Marker _marker = null;
        UIBarButtonItem _btnDone;

        public MarkerDetailsViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "MarkerDetailsViewController_iPhone" : "MarkerDetailsViewController_iPad", null)
        {
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

            // Add navigation controller buttons
            var button = new UIButton(UIButtonType.Custom);
            button.SetTitle("Done", UIControlState.Normal);
            button.Layer.CornerRadius = 8;
            button.Layer.BackgroundColor = GlobalTheme.SecondaryColor.CGColor;
            button.Font = UIFont.FromName("HelveticaNeue-Bold", 12.0f);
            button.Frame = new RectangleF(0, 20, 60, 30);
            button.TouchUpInside += (sender, e) => {
                this.DismissViewController(true, null);
            };
            _btnDone = new UIBarButtonItem(button);
            NavigationItem.SetLeftBarButtonItem(_btnDone, true);
            
            var navCtrl = (MPfmNavigationController)NavigationController;
            navCtrl.SetBackButtonVisible(false);
            navCtrl.SetTitle("Marker Details", "");

            base.ViewDidLoad();
        }

        private void HandleSliderPositionValueChanged(object sender, EventArgs e)
        {
            // Calculate new position by sending a request to the presenter
            OnChangePosition(sliderPosition.Value);
        }

        partial void actionClose(NSObject sender)
        {
            // TODO: Calculate position from slider
            _marker.Name = txtName.Text;
            _marker.Comments = textViewComments.Text;
            _marker.Position = lblPosition.Text;
            OnUpdateMarker(_marker);
        }

        partial void actionDeleteMarker(NSObject sender)
        {
            if(OnDeleteMarker != null)
                OnDeleteMarker();
        }

        #region IMarkerDetailsView implementation

        public Action<float> OnChangePosition { get; set; }
        public Action<Marker> OnUpdateMarker { get; set; }
        public Action OnDeleteMarker { get; set; }

        public void MarkerDetailsError(Exception ex)
        {
            InvokeOnMainThread(() => {
                UIAlertView alertView = new UIAlertView("Marker Details Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void DismissView()
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

