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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;
using DropBoxSync.iOS;
using Sessions.MVP.Navigation;
using Sessions.MVP.Models;

namespace Sessions.iOS
{
    public partial class MoreViewController : BaseViewController, IMobileOptionsMenuView
    {
        string _cellIdentifier = "MoreCell";
        List<MobileOptionsMenuEntity> _items;

        public MoreViewController()
			: base (UserInterfaceIdiomIsPhone ? "MoreViewController_iPhone" : "MoreViewController_iPad", null)
        {
        }
		
        public override void ViewDidLoad()
        {
            _items = new List<MobileOptionsMenuEntity>();
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;
            tableView.BackgroundColor = UIColor.FromRGB(0.85f, 0.85f, 0.85f);
            tableView.BackgroundView = null;

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindOptionsMenuView(this);
        }

        public override void ViewDidDisappear(bool animated)
        {
            tableView.DeselectRow(tableView.IndexPathForSelectedRow, false);
            base.ViewDidDisappear(animated);
        }        

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SessionsNavigationController navCtrl = (SessionsNavigationController)this.NavigationController;
            navCtrl.SetTitle("More Options");
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var distinct = _items.Select(x => x.HeaderTitle).Distinct().ToList();
            string headerTitle = distinct[indexPath.Section];
            var items = _items.Where(x => x.HeaderTitle == headerTitle).ToList();
            var item = items[indexPath.Row];

            SessionsPreferenceTableViewCell cell = (SessionsPreferenceTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new SessionsPreferenceTableViewCell(cellStyle, _cellIdentifier);
            }

            cell.ImageView.Alpha = 0.7f;
            switch (item.MenuType)
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
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/wifi");
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
                case MobileOptionsMenuType.ResumePlayback:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_resume");
                    break;
                case MobileOptionsMenuType.AudioPreferences:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_audio");
                    break;   
                case MobileOptionsMenuType.CloudPreferences:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_cloud");
                    break;   
                case MobileOptionsMenuType.GeneralPreferences:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_settings");
                    break;   
                case MobileOptionsMenuType.LibraryPreferences:
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_library");
                    break;   
            }
            
            cell.TextLabel.Text = item.Title;
            cell.Accessory = UITableViewCellAccessory.None;
            cell.IsLargeIcon = true;
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
            cell.ImageChevron.Hidden = false;

            return cell;
        }

        [Export ("tableView:viewForHeaderInSection:")]
        public UIView ViewForHeaderInSection(UITableView tableview, int section)
        {
            string sectionTitle = TitleForHeaderInSection(tableview, section);
            if(string.IsNullOrEmpty(sectionTitle))
                return null;

            var label = new UILabel();
            label.Frame = new RectangleF(12, 18, View.Frame.Width, 34);
            label.BackgroundColor = UIColor.Clear;
            label.TextColor = UIColor.FromRGB(0.5f, 0.5f, 0.5f);
            label.Font = UIFont.FromName("HelveticaNeue", 14);
            label.Text = sectionTitle;

            var view = new UIView();
            //view.BackgroundColor = UIColor.Yellow;
            view.AddSubview(label);

            return view;
        }

//        [Export ("tableView:viewForFooterInSection:")]
//        public UIView ViewForFooterInSection(UITableView tableview, int section)
//        {
//            string sectionTitle = TitleForFooterInSection(tableview, section);
//            if(string.IsNullOrEmpty(sectionTitle))
//                return null;
//
//            var label = new UILabel();
//            label.Frame = new RectangleF(12, 8, View.Frame.Width - 24, 48);
//            label.BackgroundColor = UIColor.Clear;
//            label.TextColor = UIColor.FromRGB(0.5f, 0.5f, 0.5f);
//            label.Font = UIFont.FromName("HelveticaNeue-Light", 13);
//            label.Text = sectionTitle;
//            label.Lines = 5;
//            label.SizeToFit();
//
//            var view = new UIView();
//            //view.BackgroundColor = UIColor.Yellow;
//            view.AddSubview(label);
//
//            return view;
//        }

        [Export ("tableView:titleForHeaderInSection:")]
        public string TitleForHeaderInSection(UITableView tableview, int section)
        {
            var distinct = _items.Select(x => x.HeaderTitle).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            return distinct[section].ToUpper();
        }

//        [Export ("tableView:titleForFooterInSection:")]
//        public string TitleForFooterInSection(UITableView tableview, int section)
//        {
//            var distinct = _items.Select(x => x.FooterTitle).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
//
//            if(distinct.Count > 0 && section <= distinct.Count - 1)
//                return distinct[section];
//
//            return string.Empty;
//        }

        [Export ("numberOfSectionsInTableView:")]
        public int SectionsInTableView(UITableView tableview)
        {
            var distinct = _items.Select(x => x.HeaderTitle).Distinct().ToList();
            return distinct.Count;
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            var distinct = _items.Select(x => x.HeaderTitle).Distinct().ToList();
            string headerTitle = distinct[section];
            return _items.Count(x => x.HeaderTitle == headerTitle);
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var distinct = _items.Select(x => x.HeaderTitle).Distinct().ToList();
            string headerTitle = distinct[indexPath.Section];
            var items = _items.Where(x => x.HeaderTitle == headerTitle).ToList();
            var item = items[indexPath.Row];
            tableView.DeselectRow(indexPath, true);

            OnItemClick(item.MenuType);
        }

        [Export ("tableView:didHighlightRowAtIndexPath:")]
        public void DidHighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (SessionsPreferenceTableViewCell)tableView.CellAt(indexPath);
            if(cell != null)
                cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_white");
        }

        [Export ("tableView:didUnhighlightRowAtIndexPath:")]
        public void DidUnhighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (SessionsPreferenceTableViewCell)tableView.CellAt(indexPath);
            if(cell != null)
                cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }

        [Export ("tableView:heightForHeaderInSection:")]
        public float HeightForHeaderInSection(UITableView tableView, int section)
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
        
        public void RefreshMenu(List<MobileOptionsMenuEntity> options)
        {
            InvokeOnMainThread(() => {
                _items = options;
                tableView.ReloadData();
            });
        }
        
        #endregion
    }
}
