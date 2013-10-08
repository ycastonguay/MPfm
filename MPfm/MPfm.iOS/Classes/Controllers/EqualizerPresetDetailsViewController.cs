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
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS
{
    public partial class EqualizerPresetDetailsViewController : BaseViewController, IEqualizerPresetDetailsView
    {
        bool _isPresetModified;
        EQPreset _preset;
        UIBarButtonItem _btnBack;
        UIBarButtonItem _btnSave;       
        UIBarButtonItem _btnReset;
        UIBarButtonItem _btnNormalize;
        List<MPfmEqualizerFaderView> _faderViews = new List<MPfmEqualizerFaderView>();
        
        public EqualizerPresetDetailsViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "EqualizerPresetDetailsViewController_iPhone" : "EqualizerPresetDetailsViewController_iPad", null)
        {
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

            var btnBack = new MPfmFlatButton();
            btnBack.Label.Text = "Back";
            btnBack.Frame = new RectangleF(0, 0, 70, 44);
            btnBack.OnButtonClick += HandleButtonBackClick;
            var btnBackView = new UIView(new RectangleF(0, 0, 70, 44));
            var rect = new RectangleF(btnBackView.Bounds.X + 16, btnBackView.Bounds.Y, btnBackView.Bounds.Width, btnBackView.Bounds.Height);
            btnBackView.Bounds = rect;
            btnBackView.AddSubview(btnBack);
            _btnBack = new UIBarButtonItem(btnBackView);

            var btnSave = new MPfmFlatButton();
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

            var btnReset = new MPfmButton();
            btnReset.SetTitle("Reset", UIControlState.Normal);
            btnReset.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            btnReset.Frame = new RectangleF(0, 12, 60, 30);
            btnReset.TouchUpInside += HandleButtonResetTouchUpInside;
            _btnReset = new UIBarButtonItem(btnReset);

            var btnNormalize = new MPfmButton();
            btnNormalize.SetTitle("Normalize", UIControlState.Normal);
            btnNormalize.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            btnNormalize.Frame = new RectangleF(0, 12, 80, 30);
            btnNormalize.TouchUpInside += HandleButtonNormalizeTouchUpInside;
            _btnNormalize = new UIBarButtonItem(btnNormalize);

            NavigationItem.SetLeftBarButtonItem(_btnBack, true);
            NavigationItem.SetRightBarButtonItem(_btnSave, true);
            toolbar.Items = new UIBarButtonItem[2]{ _btnNormalize, _btnReset };
            
            var navCtrl = (MPfmNavigationController)NavigationController;
            navCtrl.SetBackButtonVisible(false);
            navCtrl.SetTitle("Equalizer Preset", "16-Band Equalizer");

            for(int a = 0; a < 18; a++)
                AddFaderToScrollView(a.ToString() + ".0 kHz");
            
            base.ViewDidLoad();
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
            UIAlertView alertView = new UIAlertView("Equalizer preset will be normalized", "Are you sure you wish to normalize this equalizer preset?", null, "OK", new string[1] {"Cancel"});
            alertView.Dismissed += (sender2, e2) => {
                if(e2.ButtonIndex == 0)
                    OnNormalizePreset();
            };
            alertView.Show();
        }

        private void AddFaderToScrollView(string frequency)
        {
            MPfmEqualizerFaderView view = new MPfmEqualizerFaderView();
            view.Frame = new RectangleF(0, _faderViews.Count * 44, scrollView.Frame.Width, 44);
            view.SetValue(frequency, 0);
            view.ValueChanged += HandleFaderValueChanged;
            scrollView.AddSubview(view);
            scrollView.ContentSize = new SizeF(scrollView.Frame.Width, (_faderViews.Count + 1) * 44);
            _faderViews.Add(view);
        }

        private void HandleFaderValueChanged(object sender, MPfmEqualizerFaderValueChangedEventArgs e)
        {
            MPfmEqualizerFaderView view = (MPfmEqualizerFaderView)sender;

            var band = _preset.Bands.FirstOrDefault(x => x.CenterString == view.Frequency);
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

        public void RefreshPreset(EQPreset preset)
        {
            InvokeOnMainThread(() => {
                _isPresetModified = false;
                _preset = preset;
                txtPresetName.Text = preset.Name;
                for(int a = 0; a < preset.Bands.Count; a++)
                    _faderViews[a].SetValue(preset.Bands[a].CenterString, preset.Bands[a].Gain);

                presetGraph.Preset = _preset;
                presetGraph.SetNeedsDisplay();
            });
        }

        #endregion
    }
}
