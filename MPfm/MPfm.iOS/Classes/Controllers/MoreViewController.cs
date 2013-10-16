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
using MPfm.Core;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using DropBoxSync.iOS;

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

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("More Options", "");
        }
        
        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _items.Count;
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new MPfmTableViewCell(cellStyle, _cellIdentifier);
            }

            cell.ImageView.Alpha = 0.7f;
            switch (_items[indexPath.Row].Key)
            {
                case MobileOptionsMenuType.About:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_info");
                    break;
                case MobileOptionsMenuType.EqualizerPresets:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_eq");
                    break;
                case MobileOptionsMenuType.Preferences:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_settings");
                    break;
                case MobileOptionsMenuType.SyncLibrary:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_mobile");
                    break;
                case MobileOptionsMenuType.SyncLibraryWebBrowser:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_web");
                    break;
                case MobileOptionsMenuType.SyncLibraryCloud:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_cloud");
                    break;
                case MobileOptionsMenuType.SyncLibraryFileSharing:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_share");
                    break;
            }
            
            cell.TextLabel.Text = _items[indexPath.Row].Value;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            cell.Accessory = UITableViewCellAccessory.None;
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
            cell.ImageChevron.Hidden = false;

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
//            if (_items[indexPath.Row].Key == MobileOptionsMenuType.SyncLibraryCloud)
//            {
//                DBAccountManager.SharedManager.LinkFromController(this);
//                ListFiles("");
//                return;
//            }

            OnItemClick(_items[indexPath.Row].Key);
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

        void ListFiles(string path)
        {
            DBError error;

            var contents = DBFilesystem.SharedFilesystem.ListFolder(new DBPath(path), out error);
            foreach (DBFileInfo info in contents) {
                Console.WriteLine(info.Path);
            }   
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
