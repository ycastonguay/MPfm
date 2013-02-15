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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Classes.Delegates;
using MPfm.iOS.Classes.Controls;
using MPfm.MVP.Views;

namespace MPfm.iOS.Classes.Controllers
{
    public partial class MobileLibraryBrowserViewController : BaseViewController, IMobileLibraryBrowserView
    {
        #region IMobileLibraryBrowserView implementation

        public void RefreshLibraryBrowser(IEnumerable<MPfm.MVP.Models.LibraryBrowserEntity> entities)
        {
        }

        public MobileLibraryBrowserType BrowserType { get; set; }
        public string Filter { get; set; }
        public Action<int> OnItemClick { get; set; }

        #endregion

//        private UIBarButtonItem btnBack;
//        private Action<GenericListItem> actionOnItemSelected;
//        private ListTableViewSource tableViewSource;
//        private string title;
//        public List<GenericListItem> Items { get; private set; }
        private List<GenericListItem> _items;
        private string _cellIdentifier = "MobileLibraryBrowserCell";

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public MobileLibraryBrowserViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "ListViewController_iPhone" : "ListViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _items = new List<GenericListItem>();
            _items.Add(new GenericListItem() {
                Title = "Hello",
                Image = UIImage.FromBundle("/Images/icon114.png")
            });
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            tableView.DeselectRow(tableView.IndexPathForSelectedRow, false);
            base.ViewDidDisappear(animated);
        }
        
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            if (UserInterfaceIdiomIsPhone)
            {
                return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
            } else
            {
                return true;
            }
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
            var cellStyle = UITableViewCellStyle.Subtitle;
            
            // Create cell if cell could not be recycled
            if (cell == null)
                cell = new UITableViewCell(cellStyle, _cellIdentifier);
            
            // Set title
            cell.TextLabel.Text = _items[indexPath.Row].Title;
            cell.DetailTextLabel.Text = _items[indexPath.Row].Subtitle;
            cell.ImageView.Image = _items[indexPath.Row].Image;
            
            // Set font
            //cell.TextLabel.Font = UIFont.FromName("Junction", 20);
            cell.TextLabel.Font = UIFont.FromName("OstrichSans-Medium", 26);
            cell.DetailTextLabel.Font = UIFont.FromName("OstrichSans-Medium", 18);
            
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
            OnItemClick(indexPath.Row);
        }
    }
}

