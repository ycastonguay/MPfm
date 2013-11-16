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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.MVP.Views;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.iOS.Classes.Objects;
using System.Collections.Generic;

namespace MPfm.iOS
{
    public partial class GeneralPreferencesViewController : BasePreferencesViewController, IGeneralPreferencesView
    {
        string _cellIdentifier = "CloudPreferencesCell";
        //CloudAppConfig _config;
        List<PreferenceCellItem> _items = new List<PreferenceCellItem>();

        #region BasePreferencesViewController

        public override string CellIdentifier { get { return _cellIdentifier; } }
        public override UITableView TableView { get { return tableView; } }
        public override List<PreferenceCellItem> Items { get { return _items; } }

        #endregion

        public GeneralPreferencesViewController()
            : base (UserInterfaceIdiomIsPhone ? "GeneralPreferencesViewController_iPhone" : "GeneralPreferencesViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            GenerateItems();
            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindGeneralPreferencesView(this);
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("General Preferences");
        }

        private void GenerateItems()
        {
            // We assume the items are in order for sections
            _items = new List<PreferenceCellItem>();
            _items.Add(new PreferenceCellItem()
            {
                Id = "update_frequency_song_position",
                CellType = PreferenceCellType.Frequency,
                HeaderTitle = "Update Frequency",
                Title = "Song Position"
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "update_frequency_output_meter",
                CellType = PreferenceCellType.Frequency,
                HeaderTitle = "Update Frequency",
                Title = "Output Meter",
                FooterTitle = "Warning: Lower values require more CPU and memory."
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "peak_files_folder_size",
                CellType = PreferenceCellType.Text,
                HeaderTitle = "Peak Files",
                Title = "Peak file folder size: 1425 MB"
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "delete_peak_files",
                CellType = PreferenceCellType.Button,
                HeaderTitle = "Peak Files",
                Title = "Delete Peak Files",
                IconName = "dropbox"
            });
        }

        public override void PreferenceValueChanged(PreferenceCellItem item)
        {
            //            var localItem = _items.FirstOrDefault(x => x.Id == item.Id);
            //            if (localItem == null)
            //                return;
            //
            //            localItem.Value = item.Value;
            //
            //            if (item.Id == "enable_dropbox_resume_playback")
            //                _config.IsDropboxResumePlaybackEnabled = (bool)item.Value;
            //            else if (item.Id == "enable_dropbox_resume_playback_wifi_only")
            //                _config.IsDropboxResumePlaybackWifiOnlyEnabled = (bool)item.Value;
            //
            //            OnSetCloudPreferences(_config);
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //            var item = _items[indexPath.Row];
            //            if (item.Id == "login_dropbox")
            //                OnDropboxLoginLogout();
        }  
    }
}
