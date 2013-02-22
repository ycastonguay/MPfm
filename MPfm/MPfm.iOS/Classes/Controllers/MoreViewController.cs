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
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;

namespace MPfm.iOS
{
    public partial class MoreViewController : BaseViewController, IMobileOptionsMenuView
    {
        string _cellIdentifier = "MoreCell";
        List<KeyValuePair<MobileOptionsMenuType, string>> _items;

        public MoreViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "MoreViewController_iPhone" : "MoreViewController_iPad", null)
        {
        }
		
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _items = new List<KeyValuePair<MobileOptionsMenuType, string>>();
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;
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
            // Request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
            
            // Set cell style
            var cellStyle = UITableViewCellStyle.Default;
            
            // Create cell if cell could not be recycled
            if (cell == null)
                cell = new UITableViewCell(cellStyle, _cellIdentifier);
            
            // Set title
            cell.TextLabel.Text = _items[indexPath.Row].Value;
            
            // Set font
            cell.TextLabel.Font = UIFont.FromName("OstrichSans-Medium", 26);
            
            // Set chevron
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            
            //            // Check this is the version cell (remove all user interaction)
            //            if (viewModel.Items[indexPath.Row].ItemType == MoreItemType.Version)
            //            {
            //                cell.Accessory = UITableViewCellAccessory.None;
            //                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            //                cell.TextLabel.TextColor = UIColor.Gray;
            //                cell.TextLabel.TextAlignment = UITextAlignment.Center;
            //                cell.TextLabel.Font = UIFont.FromName("Asap", 16);
            //            }
            
            return cell;
        }
        
        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            OnItemClick(_items[indexPath.Row].Key);
        }

        #region IMobileOptionsMenuView implementation

        public Action<MobileOptionsMenuType> OnItemClick { get; set; }
        
        public void RefreshMenu(List<KeyValuePair<MobileOptionsMenuType, string>> options)
        {
            InvokeOnMainThread(() => {
                _items = options;
                tableView.ReloadData();
            });
        }
        
        #endregion
    }
}
