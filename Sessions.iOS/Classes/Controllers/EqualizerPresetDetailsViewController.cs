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
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.iOS.Helpers;
using Sessions.Core;
using org.sessionsapp.player;

namespace Sessions.iOS
{
    public partial class EqualizerPresetDetailsViewController : BaseViewController, IEqualizerPresetDetailsView
    {
        Guid _presetId;
        bool _isPresetModified;
        SSPEQPreset _preset;
        UIBarButtonItem _btnBack;
        UIBarButtonItem _btnSave;       
        UIBarButtonItem _btnReset;
        UIBarButtonItem _btnNormalize;
        List<SessionsEqualizerFaderView> _faderViews = new List<SessionsEqualizerFaderView>();
        
        public EqualizerPresetDetailsViewController(Guid presetId)
            : base (UserInterfaceIdiomIsPhone ? "EqualizerPresetDetailsViewController_iPhone" : "EqualizerPresetDetailsViewController_iPad", null)
        {
            _presetId = presetId;
        }
        
        public override void ViewDidLoad()
        {
			scrollView.BackgroundColor = GlobalTheme.BackgroundColor;
            toolbar.BackgroundColor = GlobalTheme.MainColor;
            viewOptions.BackgroundColor = GlobalTheme.BackgroundColor;
            lblPresetName.TextColor = UIColor.White;
            
            // Add padding to text field (http://stackoverflow.com/questions/3727068/set-padding-for-uitextfield-with-uitextborderstylenone)
            UIView paddingView = new UIView(new RectangleF(0, 0, 5, 20));
            txtPresetName.LeftView = paddingView;
            txtPresetName.LeftViewMode = UITextFieldViewMode.Always;

            // Make sure the Done key closes the keyboard
            txtPresetName.ShouldReturn = (a) => {
                txtPresetName.ResignFirstResponder();
                return true;
            };

            var btnBack = new SessionsFlatButton();
            btnBack.Label.Text = "Back";
            btnBack.Frame = new RectangleF(0, 0, 70, 44);
            btnBack.OnButtonClick += HandleButtonBackClick;
            var btnBackView = new UIView(new RectangleF(0, 0, 70, 44));
            var rect = new RectangleF(btnBackView.Bounds.X + 16, btnBackView.Bounds.Y, btnBackView.Bounds.Width, btnBackView.Bounds.Height);
            btnBackView.Bounds = rect;
            btnBackView.AddSubview(btnBack);
            _btnBack = new UIBarButtonItem(btnBackView);

            var btnSave = new SessionsFlatButton();
            btnSave.LabelAlignment = UIControlContentHorizontalAlignment.Right;
            btnSave.Label.Text = "Save";
            btnSave.Label.TextAlignment = UITextAlignment.Right;
            btnSave.Label.Frame = new RectangleF(0, 0, 44, 44);
            btnSave.ImageChevron.Hidden = true;
            btnSave.Frame = new RectangleF(0, 0, 60, 44);
            btnSave.OnButtonClick += HandleButtonSaveTouchUpInside;
            var btnSaveView = new UIView(new RectangleF(UIScreen.MainScreen.Bounds.Width - 60, 0, 60, 44));
            var rect2 = new RectangleF(btnSaveView.Bounds.X - 16, btnSaveView.Bounds.Y, btnSaveView.Bounds.Width, btnSaveView.Bounds.Height);
            btnSaveView.Bounds = rect2;
            btnSaveView.AddSubview(btnSave);
            _btnSave = new UIBarButtonItem(btnSaveView);

            var btnReset = new SessionsButton();
            btnReset.SetTitle("Reset", UIControlState.Normal);
            btnReset.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            btnReset.Frame = new RectangleF(0, 12, 60, 30);
            btnReset.TouchUpInside += HandleButtonResetTouchUpInside;
            _btnReset = new UIBarButtonItem(btnReset);

            var btnNormalize = new SessionsButton();
            btnNormalize.SetTitle("Normalize", UIControlState.Normal);
            btnNormalize.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            btnNormalize.Frame = new RectangleF(0, 12, 80, 30);
            btnNormalize.TouchUpInside += HandleButtonNormalizeTouchUpInside;
            _btnNormalize = new UIBarButtonItem(btnNormalize);

            NavigationItem.SetLeftBarButtonItem(_btnBack, true);
            NavigationItem.SetRightBarButtonItem(_btnSave, true);
            toolbar.Items = new UIBarButtonItem[2]{ _btnNormalize, _btnReset };
            
            var navCtrl = (SessionsNavigationController)NavigationController;
            navCtrl.SetBackButtonVisible(false);
            navCtrl.SetTitle("Equalizer Preset");

            for(int a = 0; a < 18; a++)
                AddFaderToScrollView(a.ToString() + ".0 kHz");

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                NavigationController.InteractivePopGestureRecognizer.WeakDelegate = this;
                NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            }

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindEqualizerPresetDetailsView(null, this, _presetId);
        }

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			var screenSize = UIKitHelper.GetDeviceSize();
			scrollView.ContentSize = new SizeF(screenSize.Width, _faderViews.Count * 44);

			//Tracing.Log("EqualizerPresetDetailsVC - ViewDidLayoutSubviews - width: {0} faderCount: {1}", screenSize.Width, _faderViews.Count);
			float y = 0;
			for (int a = 0; a < scrollView.Subviews.Count(); a++)
			{
				//Tracing.Log("EqualizerPresetDetailsVC - ViewDidLayoutSubviews - a: {0} a*44: {1}", a, (a * 44));
				var view = scrollView.Subviews[a];
				if (view is SessionsEqualizerFaderView)
				{
					view.Frame = new RectangleF(0, y, screenSize.Width, 44);
					y += 44;
				}
			}
		}

        private void HandleButtonSaveTouchUpInside()
        {
            _isPresetModified = false;
            OnSavePreset(txtPresetName.Text);
        }

        private void HandleButtonBackClick()
        {
            if (_isPresetModified)
            {
                UIAlertView alertView = new UIAlertView("Equalizer preset has been modified", "Are you sure you wish to exit this screen without saving?", null, "OK", new string[1] {"Cancel"});
                alertView.Dismissed += (sender2, e2) => {
                    if(e2.ButtonIndex == 0)
                    {
                        OnRevertPreset();
                        NavigationController.PopViewControllerAnimated(true);
                    }
                };
                alertView.Show();
            }
            else
            {
                NavigationController.PopViewControllerAnimated(true);
            }
        }

        private void HandleButtonResetTouchUpInside(object sender, EventArgs e)
        {
            UIAlertView alertView = new UIAlertView("Equalizer preset will be reset", "Are you sure you wish to reset this equalizer preset?", null, "OK", new string[1] {"Cancel"});
            alertView.Dismissed += (sender2, e2) => {
                if(e2.ButtonIndex == 0)
                    OnResetPreset();                    
            };
            alertView.Show();
        }

        private void HandleButtonNormalizeTouchUpInside(object sender, EventArgs e)
        {
            var alertView = new UIAlertView("Equalizer preset will be normalized", "Are you sure you wish to normalize this equalizer preset?", null, "OK", new string[1] {"Cancel"});
            alertView.Dismissed += (sender2, e2) => {
                if(e2.ButtonIndex == 0)
                    OnNormalizePreset();
            };
            alertView.Show();
        }

        private void AddFaderToScrollView(string frequency)
        {
			//Tracing.Log("EqualizerPresetDetailsVC - AddFaderToScrollView - frequency: {0} faderCount: {1}", frequency, _faderViews.Count);
            var view = new SessionsEqualizerFaderView();
            view.Frame = new RectangleF(0, _faderViews.Count * 44, scrollView.Frame.Width, 44);
            view.SetValue(frequency, 0);
            view.ValueChanged += HandleFaderValueChanged;
            scrollView.AddSubview(view);
            scrollView.ContentSize = new SizeF(scrollView.Frame.Width, (_faderViews.Count + 1) * 44);
            _faderViews.Add(view);
        }

        private void HandleFaderValueChanged(object sender, SessionsEqualizerFaderValueChangedEventArgs e)
        {
            var view = (SessionsEqualizerFaderView)sender;
            var band = _preset.Bands.FirstOrDefault(x => x.Label == view.Frequency);
            band.Gain = e.Value;

            _isPresetModified = true;
            OnSetFaderGain(view.Frequency, e.Value);

            presetGraph.Preset = _preset;
            presetGraph.SetNeedsDisplay();
        }
        
        #region IEqualizerPresetDetailsView implementation

        public Action<Guid> OnChangePreset { get; set; }
        public Action OnResetPreset { get; set; }
        public Action OnNormalizePreset { get; set; }
        public Action OnRevertPreset { get; set; }
        public Action<string> OnSavePreset { get; set; }
        public Action<string, float> OnSetFaderGain { get; set; }

        public void EqualizerPresetDetailsError(Exception ex)
        {
            InvokeOnMainThread(() => {
                UIAlertView alertView = new UIAlertView("Equalizer Preset Details Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void ShowMessage(string title, string message)
        {
            InvokeOnMainThread(() => {
                UIAlertView alertView = new UIAlertView(title, message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshPreset(SSPEQPreset preset)
        {
            InvokeOnMainThread(() => {
                _isPresetModified = false;
                _preset = preset;
                txtPresetName.Text = preset.Name;
                for(int a = 0; a < preset.Bands.Length; a++)
                    _faderViews[a].SetValue(preset.Bands[a].Label, preset.Bands[a].Gain);

                presetGraph.Preset = _preset;
                presetGraph.SetNeedsDisplay();
            });
        }

        #endregion
    }
}
