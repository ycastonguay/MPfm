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
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;
using MPfm.Player.Objects;

namespace MPfm.iOS
{
    public partial class EqualizerPresetsViewController : BaseViewController, IEqualizerPresetsView
    {
        UIBarButtonItem _btnAdd;
        UIBarButtonItem _btnDone;
        string _cellIdentifier = "EqualizerPresetCell";
        List<EQPreset> _presets = new List<EQPreset>();

        public EqualizerPresetsViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "EqualizerPresetsViewController_iPhone" : "EqualizerPresetsViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            UILongPressGestureRecognizer longPress = new UILongPressGestureRecognizer(HandleLongPress);
            longPress.MinimumPressDuration = 0.7f;
            longPress.WeakDelegate = this;
            tableView.AddGestureRecognizer(longPress);

            viewOptions.BackgroundColor = GlobalTheme.BackgroundColor;
            lblBypass.TextColor = UIColor.White;
            lblBypass.Font = UIFont.FromName("HelveticaNeue", 14.0f);

            switchBypass.On = false;
            switchBypass.ValueChanged += HandleSwitchBypassValueChanged;

            var btnDone = new UIButton(UIButtonType.Custom);
            btnDone.SetTitle("Done", UIControlState.Normal);
            btnDone.Layer.CornerRadius = 8;
            btnDone.Layer.BackgroundColor = GlobalTheme.SecondaryColor.CGColor;
            btnDone.Font = UIFont.FromName("HelveticaNeue-Bold", 12.0f);
            btnDone.Frame = new RectangleF(0, 20, 60, 30);
            btnDone.TouchUpInside += (sender, e) => {
                NavigationController.DismissViewController(true, null);
            };
            _btnDone = new UIBarButtonItem(btnDone);

            var btnAdd = new UIButton(UIButtonType.Custom);
            btnAdd.SetTitle("+", UIControlState.Normal);
            btnAdd.Layer.CornerRadius = 8;
            btnAdd.Layer.BackgroundColor = GlobalTheme.SecondaryColor.CGColor;
            btnAdd.Font = UIFont.FromName("HelveticaNeue-Bold", 18.0f);
            btnAdd.Frame = new RectangleF(0, 12, 40, 30);
            btnAdd.TouchUpInside += (sender, e) => {
                OnAddPreset();
            };
            _btnAdd = new UIBarButtonItem(btnAdd);

            NavigationItem.SetLeftBarButtonItem(_btnDone, true);
            NavigationItem.SetRightBarButtonItem(_btnAdd, true);

            var navCtrl = (MPfmNavigationController)NavigationController;
            navCtrl.SetBackButtonVisible(false);

            base.ViewDidLoad();
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
            PointF pt = gestureRecognizer.LocationInView(tableView);
            NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(pt);
            if (indexPath != null)
                if(OnEditPreset != null)
                    OnEditPreset(_presets[indexPath.Row].EQPresetId);
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
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
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 16);
            cell.TextLabel.TextColor = UIColor.Black;
            cell.Accessory = UITableViewCellAccessory.Checkmark;
            cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
            
            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;
            
            return cell;
        }
        
        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if(OnLoadPreset != null)
            {
                OnLoadPreset(_presets[indexPath.Row].EQPresetId);
                tableView.DeselectRow(indexPath, true);
            }
        }

        #region IEqualizerPresetsView implementation

        public Action OnBypassEqualizer { get; set; }
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

        public void RefreshPresets(IEnumerable<EQPreset> presets)
        {
            InvokeOnMainThread(() => {
                _presets = presets.ToList();
                tableView.ReloadData();
            });
        }

        #endregion
    }
}
