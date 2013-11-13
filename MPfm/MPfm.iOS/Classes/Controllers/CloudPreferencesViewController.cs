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
using System.Drawing;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Config.Models;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.iOS.Classes.Controls;
using System.Collections.Generic;
using System.Linq;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS
{
    public partial class CloudPreferencesViewController : BasePreferencesViewController, ICloudPreferencesView
    {
        string _cellIdentifier = "CloudPreferencesCell";
        CloudAppConfig _config;
        List<PreferenceCellItem> _items;

        public CloudPreferencesViewController()
			: base (UserInterfaceIdiomIsPhone ? "CloudPreferencesViewController_iPhone" : "CloudPreferencesViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            // TO DO: Move generic stuff to BasePreferencesVC

            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            //tableView.SeparatorColor = GlobalTheme.BackgroundColor;
            //tableView.BackgroundColor = GlobalTheme.BackgroundColor;
            //tableView.BackgroundColor = GlobalTheme.LightColor;
            tableView.BackgroundColor = UIColor.FromRGB(0.85f, 0.85f, 0.85f);
            tableView.BackgroundView = null;

            base.ViewDidLoad();

            //btnLoginDropbox.SetImage(UIImage.FromBundle("Images/Buttons/dropbox"));
            GenerateItems();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindCloudPreferencesView(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Cloud Preferences");
        }

        private void GenerateItems()
        {
            // We assume the items are in order for sections
            _items = new List<PreferenceCellItem>();
            _items.Add(new PreferenceCellItem()
            {
                Id = "login_dropbox",
                CellType = PreferenceCellType.Button,
                SectionTitle = "Dropbox",
                Title = "Login to Dropbox"
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "enable_dropbox_resume_playback",
                CellType = PreferenceCellType.Boolean,
                SectionTitle = "Dropbox",
                Title = "Enable Resume Playback"
            });
            _items.Add(new PreferenceCellItem()
            {
                CellType = PreferenceCellType.Text,
                SectionTitle = "Dropbox",
                Title = "This will take a small amount of bandwidth (about 1 kilobyte) every time the player switches to a new song."
            });
        }
        
        [Export ("tableView:viewForHeaderInSection:")]
        public UIView ViewForHeaderInSection(UITableView tableview, int section)
        {
            string sectionTitle = TitleForHeaderInSection(tableView, section);
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

        [Export ("tableView:titleForHeaderInSection:")]
        public string TitleForHeaderInSection(UITableView tableview, int section)
        {
            var distinct = _items.Select(x => x.SectionTitle).Distinct().ToList();
            return distinct[section].ToUpper();
        }

        [Export ("numberOfSectionsInTableView:")]
        public int SectionsInTableView(UITableView tableview)
        {
            var distinct = _items.Select(x => x.SectionTitle).Distinct().ToList();
            return distinct.Count;
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            var distinct = _items.Select(x => x.SectionTitle).Distinct().ToList();
            string sectionTitle = distinct[section];
            return _items.Count(x => x.SectionTitle == sectionTitle);
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _items[indexPath.Row];
            MPfmPreferenceTableViewCell cell = (MPfmPreferenceTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new MPfmPreferenceTableViewCell(cellStyle, _cellIdentifier);
            }

            cell.ImageView.Alpha = 0.7f;
            if (item.CellType == PreferenceCellType.Button)
                cell.ImageView.Image = UIImage.FromBundle("/Images/Icons/icon_cloud");

            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = item.Title;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            cell.Accessory = UITableViewCellAccessory.None;

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //OnSelectItem(_items[indexPath.Row]);
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

        #region ICloudPreferencesView implementation

        public Action<CloudAppConfig> OnSetCloudPreferences { get; set; }
        public Action OnDropboxLoginLogout { get; set; }

        public void CloudPreferencesError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("CloudPreferences error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshCloudPreferences(CloudAppConfig config)
        {
            _config = config;

            // Is there a way to get AppConfig properties and generate it into a flat list of settings with reflection?
        }

        public void RefreshCloudPreferencesState(CloudPreferencesStateEntity entity)
        {
            InvokeOnMainThread(() =>
            {
                //btnLoginDropbox.TitleLabel.Text = entity.IsDropboxLinkedToApp ? "Logout from Dropbox" : "Login to Dropbox";
                //btnLoginDropbox.UpdateLayout();
            });
        }

        #endregion
    }

    public enum PreferenceCellType
    {
        Text = 0,
        Button = 1,
        Boolean = 2,
        String = 3,
        Integer = 4,
        Frequency = 5
    }

    public class PreferenceCellItem
    {
        public string Id { get; set; }
        public PreferenceCellType CellType { get; set; }
        public string SectionTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
