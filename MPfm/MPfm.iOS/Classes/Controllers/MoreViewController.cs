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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.Core;

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
            _items = new List<KeyValuePair<MobileOptionsMenuType, string>>();
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            base.ViewDidLoad();
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
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Default;
                cell = new UITableViewCell(cellStyle, _cellIdentifier);
            }
            
            cell.TextLabel.Text = _items[indexPath.Row].Value;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 16);
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;

            
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
            if(indexPath.Row == 3)
            {
                var remoteHostStatus = ReachabilityHelper.RemoteHostStatus ();
                var internetStatus = ReachabilityHelper.InternetConnectionStatus ();
                var localWifiStatus = ReachabilityHelper.LocalWifiConnectionStatus ();
                Console.WriteLine("remoteHostStatus: {0} - internetStatus: {1} - localWifiStatus: {2}", remoteHostStatus, internetStatus, localWifiStatus);

//                var webClient = new WebClient();
//                webClient.DownloadFile("http://192.168.1.101:8080/", Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "test.mp3"));
                return;
            }

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
