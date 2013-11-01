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
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.MVP.Bootstrap;

namespace MPfm.iOS
{
    public partial class EqualizerPresetsViewController : BaseViewController, IEqualizerPresetsView
    {
        MPVolumeView _volumeView;
        UIBarButtonItem _btnDone;
        UIBarButtonItem _btnAdd;
        string _cellIdentifier = "EqualizerPresetCell";
        List<EQPreset> _presets = new List<EQPreset>();
        Guid _selectedPresetId;

        public EqualizerPresetsViewController()
            : base (UserInterfaceIdiomIsPhone ? "EqualizerPresetsViewController_iPhone" : "EqualizerPresetsViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            var screenSize = UIKitHelper.GetDeviceSize();

            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            UILongPressGestureRecognizer longPress = new UILongPressGestureRecognizer(HandleLongPress);
            longPress.MinimumPressDuration = 0.7f;
            longPress.WeakDelegate = this;
            tableView.AddGestureRecognizer(longPress);

            viewOptions.BackgroundColor = GlobalTheme.BackgroundColor;
            switchBypass.OnTintColor = GlobalTheme.SecondaryColor;
            switchBypass.On = false;
            switchBypass.ValueChanged += HandleSwitchBypassValueChanged;

            //sliderMasterVolume.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            //sliderMasterVolume.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            //sliderMasterVolume.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            //sliderMasterVolume.ValueChanged += HandleSliderMasterVolumeValueChanged;

            var btnDone = new MPfmFlatButton();
            btnDone.Label.Text = "Done";
            btnDone.Frame = new RectangleF(0, 0, 70, 44);
            btnDone.OnButtonClick += () => {
                NavigationController.DismissViewController(true, null);
            };
            var btnDoneView = new UIView(new RectangleF(0, 0, 70, 44));
            var rect = new RectangleF(btnDoneView.Bounds.X + 16, btnDoneView.Bounds.Y, btnDoneView.Bounds.Width, btnDoneView.Bounds.Height);
            btnDoneView.Bounds = rect;
            btnDoneView.AddSubview(btnDone);
            _btnDone = new UIBarButtonItem(btnDoneView);

            var btnAdd = new MPfmFlatButton();
            btnAdd.LabelAlignment = UIControlContentHorizontalAlignment.Right;
            btnAdd.Label.Text = "Add";
            btnAdd.Label.TextAlignment = UITextAlignment.Right;
            btnAdd.Label.Frame = new RectangleF(0, 0, 44, 44);
            btnAdd.ImageChevron.Image = UIImage.FromBundle("Images/Tables/plus_blue");
            btnAdd.ImageChevron.Frame = new RectangleF(70 - 22, 0, 22, 44);
            btnAdd.Frame = new RectangleF(0, 0, 70, 44);
            btnAdd.OnButtonClick += HandleButtonAddTouchUpInside;
            var btnAddView = new UIView(new RectangleF(UIScreen.MainScreen.Bounds.Width - 70, 0, 70, 44));
            var rect2 = new RectangleF(btnAddView.Bounds.X - 16, btnAddView.Bounds.Y, btnAddView.Bounds.Width, btnAddView.Bounds.Height);
            btnAddView.Bounds = rect2;
            btnAddView.AddSubview(btnAdd);
            _btnAdd = new UIBarButtonItem(btnAddView);

            NavigationItem.SetLeftBarButtonItem(_btnDone, true);
            NavigationItem.SetRightBarButtonItem(_btnAdd, true);

            var navCtrl = (MPfmNavigationController)NavigationController;
            navCtrl.SetBackButtonVisible(false);

            // Create MPVolumeView (only visible on physical iOS device)
            RectangleF rectVolume;
            if (UserInterfaceIdiomIsPhone)
                rectVolume = new RectangleF(74, 25, 236, 46);
            else
                rectVolume = new RectangleF(74, 25, 236, 46);
            _volumeView = new MPVolumeView(rectVolume);
            _volumeView.SetMinimumVolumeSliderImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            _volumeView.SetMaximumVolumeSliderImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            _volumeView.SetVolumeThumbImage(UIImage.FromBundle("Images/Sliders/thumbbig"), UIControlState.Normal);
            this.View.AddSubview(_volumeView);

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindEqualizerPresetsView(null, this);
        }

//        private void HandleSliderMasterVolumeValueChanged(object sender, EventArgs e)
//        {
//            lblMasterVolumeValue.Text = sliderMasterVolume.Value.ToString("0") + " %";
//            OnSetVolume(sliderMasterVolume.Value / 100);
//        }

        private void HandleButtonAddTouchUpInside()
        {
            foreach (var visibleCell in tableView.VisibleCells)
                visibleCell.Accessory = UITableViewCellAccessory.None;

            OnAddPreset();
        }

        private void HandleSwitchBypassValueChanged(object sender, EventArgs e)
        {
            OnBypassEqualizer();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Equalizer Presets", "All Presets");
        }

        private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
                return;

            PointF pt = gestureRecognizer.LocationInView(tableView);
            NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(pt);
            if (indexPath != null)
            {
                SetCheckmarkCell(indexPath);
                OnEditPreset(_presets[indexPath.Row].EQPresetId);
            }
        }

        [Export ("tableView:commitEditingStyle:forRowAtIndexPath:")]
        public void CommitEditingStyleForRowAtIndexPath(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            if(editingStyle == UITableViewCellEditingStyle.Delete)
            {
                OnDeletePreset(_presets[indexPath.Row].EQPresetId);
            }
        }

        [Export ("tableView:canEditRowAtIndexPath:")]
        public bool CanEditRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableView, int section)
        {
            return _presets.Count;
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // Request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new UITableViewCell(cellStyle, _cellIdentifier);
            }

            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = _presets[indexPath.Row].Name;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            cell.TextLabel.TextColor = UIColor.Black;
            cell.TextLabel.HighlightedTextColor = UIColor.White;
            cell.Accessory = UITableViewCellAccessory.None;
            cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;

            if (_presets[indexPath.Row].EQPresetId == _selectedPresetId)
                cell.Accessory = UITableViewCellAccessory.Checkmark;
            
            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;
            
            return cell;
        }
        
        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            SetCheckmarkCell(indexPath);
            OnLoadPreset(_presets[indexPath.Row].EQPresetId);
            tableView.DeselectRow(indexPath, true);
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }

        private void SetCheckmarkCell(NSIndexPath indexPath)
        {
            _selectedPresetId = _presets[indexPath.Row].EQPresetId;

            // Reset checkmarks
            foreach (var visibleCell in tableView.VisibleCells)
                visibleCell.Accessory = UITableViewCellAccessory.None;

            // Set new checkmark (force reload row or the checkmark isn't always visible)
            var cell = tableView.CellAt(indexPath);
            if(cell != null)
                tableView.ReloadRows(new NSIndexPath[1] { indexPath }, UITableViewRowAnimation.None);
        }

        #region IEqualizerPresetsView implementation

        public Action OnBypassEqualizer { get; set; }
        public Action<float> OnSetVolume { get; set; }
        public Action OnAddPreset { get; set; }
        public Action<Guid> OnLoadPreset { get; set; }
        public Action<Guid> OnEditPreset { get; set; }
        public Action<Guid> OnDeletePreset { get; set; }

        public void EqualizerPresetsError(Exception ex)
        {
            InvokeOnMainThread(() => {
                UIAlertView alertView = new UIAlertView("Equalizer Presets Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshPresets(IEnumerable<EQPreset> presets, Guid selectedPresetId, bool isEQBypassed)
        {
            InvokeOnMainThread(() => {
                _selectedPresetId = selectedPresetId;
                _presets = presets.ToList();
                tableView.ReloadData();
                switchBypass.On = isEQBypassed;
            });
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
            InvokeOnMainThread(() => {
                outputMeter.AddWaveDataBlock(dataLeft, dataRight);
                outputMeter.SetNeedsDisplay();
            });
        }

        public void RefreshVolume(float volume)
        {
            InvokeOnMainThread(() => {
                //sliderMasterVolume.Value = volume * 100;
                //lblMasterVolumeValue.Text = (volume * 100).ToString("0") + " %";
            });
        }

        #endregion
    }
}
