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
using MPfm.MVP;
using MPfm.MVP.Views;
using System.Drawing;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS.Classes.Controllers.Base
{
	public abstract class BasePreferencesViewController : BaseViewController
	{
        public abstract string CellIdentifier { get; }
        public abstract UITableView TableView { get; }
        public abstract List<PreferenceCellItem> Items { get; }

        public abstract void PreferenceValueChanged(PreferenceCellItem item);

        //string _cellIdentifier = "CloudPreferencesCell";
        //List<PreferenceCellItem> _items;

        public BasePreferencesViewController(string nibName, NSBundle bundle) 
            : base(nibName, bundle) 
        {
        }

        public override void ViewDidLoad()
        {
            TableView.WeakDataSource = this;
            TableView.WeakDelegate = this;
            TableView.BackgroundColor = UIColor.FromRGB(0.85f, 0.85f, 0.85f);
            TableView.BackgroundView = null;

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                NavigationController.InteractivePopGestureRecognizer.WeakDelegate = this;
                NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            }

            base.ViewDidLoad();
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

        [Export ("tableView:viewForFooterInSection:")]
        public UIView ViewForFooterInSection(UITableView tableview, int section)
        {
            string sectionTitle = TitleForFooterInSection(tableview, section);
            if(string.IsNullOrEmpty(sectionTitle))
                return null;

            var label = new UILabel();
            label.Frame = new RectangleF(12, 4, View.Frame.Width - 24, 48);
            label.BackgroundColor = UIColor.Clear;
            label.TextColor = UIColor.FromRGB(0.5f, 0.5f, 0.5f);
            label.Font = UIFont.FromName("HelveticaNeue-Light", 13);
            label.Text = sectionTitle;
            label.Lines = 3;

            var view = new UIView();
            //view.BackgroundColor = UIColor.Yellow;
            view.AddSubview(label);

            return view;
        }

        [Export ("tableView:titleForHeaderInSection:")]
        public string TitleForHeaderInSection(UITableView tableview, int section)
        {
            var distinct = Items.Select(x => x.HeaderTitle).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            return distinct[section].ToUpper();
        }

        [Export ("tableView:titleForFooterInSection:")]
        public string TitleForFooterInSection(UITableView tableview, int section)
        {
            var distinct = Items.Select(x => x.FooterTitle).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            if(distinct.Count > 0 && section <= distinct.Count - 1)
                return distinct[section];

            return string.Empty;
        }

        [Export ("numberOfSectionsInTableView:")]
        public int SectionsInTableView(UITableView tableview)
        {
            var distinct = Items.Select(x => x.HeaderTitle).Distinct().ToList();
            return distinct.Count;
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            var distinct = Items.Select(x => x.HeaderTitle).Distinct().ToList();
            string headerTitle = distinct[section];
            return Items.Count(x => x.HeaderTitle == headerTitle);
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var distinct = Items.Select(x => x.HeaderTitle).Distinct().ToList();
            string headerTitle = distinct[indexPath.Section];
            var items = Items.Where(x => x.HeaderTitle == headerTitle).ToList();
            var item = items[indexPath.Row];

            MPfmPreferenceTableViewCell cell = (MPfmPreferenceTableViewCell)tableView.DequeueReusableCell(CellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new MPfmPreferenceTableViewCell(cellStyle, CellIdentifier);
                cell.OnPreferenceValueChanged += PreferenceValueChanged;
            }

            if (!string.IsNullOrEmpty(item.IconName))
            {
                cell.ImageView.Alpha = 0.7f;
                cell.ImageView.Image = UIImage.FromBundle(string.Format("/Images/Icons/{0}", item.IconName));
            }

            cell.Tag = indexPath.Row;
            cell.Accessory = UITableViewCellAccessory.None;
            cell.SetItem(item);

            return cell;
        }           

        [Export ("tableView:heightForHeaderInSection:")]
        public float HeightForHeaderInSection(UITableView tableView, int section)
        {
            return 52;
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }

	}
}
