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
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MPfm.iOS
{
    public class ListTableViewSource : UITableViewSource 
    {
        Action<GenericListItem> onRowSelected;
        string cellIdentifier = "ListTableCell";
        List<GenericListItem> items;
        
        public ListTableViewSource(List<GenericListItem> items, Action<GenericListItem> onRowSelected)
        {
            this.onRowSelected = onRowSelected;
            this.items = items;
        }
        
        public override int RowsInSection(UITableView tableview, int section)
        {
            return items.Count;
        }
        
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // Request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
            
            // Set cell style
            var cellStyle = UITableViewCellStyle.Subtitle;
            
            // Create cell if cell could not be recycled
            if (cell == null)
                cell = new UITableViewCell(cellStyle, cellIdentifier);
            
            // Set title
            cell.TextLabel.Text = items[indexPath.Row].Title;
            cell.DetailTextLabel.Text = items[indexPath.Row].Subtitle;
            cell.ImageView.Image = items[indexPath.Row].Image;
            
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
        
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if(onRowSelected != null)
                onRowSelected(items[indexPath.Row]);
        }
    }
}
