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
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;

namespace MPfm.iOS
{
    public partial class PreferencesViewController : BaseViewController, IPreferencesView
    {
        string _cellIdentifier = "PreferencesCell";
        List<string> _items = new List<string>();

        public PreferencesViewController()
            : base (UserInterfaceIdiomIsPhone ? "PreferencesViewController_iPhone" : "PreferencesViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                NavigationController.InteractivePopGestureRecognizer.WeakDelegate = this;
                NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            }

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindPreferencesView(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Preferences");
        }

        public override void ViewDidDisappear(bool animated)
        {
            tableView.DeselectRow(tableView.IndexPathForSelectedRow, false);
            base.ViewDidDisappear(animated);
        }        

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _items.Count;
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _items[indexPath.Row];
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new MPfmTableViewCell(cellStyle, _cellIdentifier);
            }

            cell.ImageView.Alpha = 0.7f;
            if(item.ToUpper().StartsWith("AUDIO"))
                cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_audio");
            else if(item.ToUpper().StartsWith("CLOUD"))
                cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_cloud");
            else if(item.ToUpper().StartsWith("GENERAL"))
                cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_settings");
            else if(item.ToUpper().StartsWith("LIBRARY"))
                cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_library");

            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = item;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            cell.Accessory = UITableViewCellAccessory.None;
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
            cell.ImageChevron.Hidden = false;
            
            return cell;
        }
        
        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            OnSelectItem(_items[indexPath.Row]);
        }

        [Export ("tableView:didHighlightRowAtIndexPath:")]
        public void DidHighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_white");
        }

        [Export ("tableView:didUnhighlightRowAtIndexPath:")]
        public void DidUnhighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }

        #region IPreferencesView implementation

        public Action<string> OnSelectItem { get; set; }
        
        public void RefreshItems(List<string> items)
        {
            InvokeOnMainThread(() => {
                _items = items;
                tableView.ReloadData();
            });
        }

        public void PushSubView(IBaseView view)
        {

        }

        #endregion
    }
}
