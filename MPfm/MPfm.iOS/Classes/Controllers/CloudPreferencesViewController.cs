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
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Config.Models;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS
{
    public partial class CloudPreferencesViewController : BasePreferencesViewController, ICloudPreferencesView
    {
        string _cellIdentifier = "CloudPreferencesCell";
        CloudAppConfig _config;
        List<PreferenceCellItem> _items = new List<PreferenceCellItem>();

        #region BasePreferencesViewController

        public override string CellIdentifier { get { return _cellIdentifier; } }
        public override UITableView TableView { get { return tableView; } }
        public override List<PreferenceCellItem> Items { get { return _items; } }

        #endregion

        public CloudPreferencesViewController()
			: base (UserInterfaceIdiomIsPhone ? "CloudPreferencesViewController_iPhone" : "CloudPreferencesViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

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
                HeaderTitle = "Cloud Services",
                Title = "Login to Dropbox",
                IconName = "dropbox"
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "enable_resume_playback",
                CellType = PreferenceCellType.Boolean,
                HeaderTitle = "Sync",
                Title = "Enable Resume Playback",
                Description = "Resume playback from other devices",
                Value = _config.IsDropboxResumePlaybackEnabled
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "enable_playlist_sync",
                CellType = PreferenceCellType.Boolean,
                HeaderTitle = "Sync",
                Title = "Sync Playlists",
                Value = _config.IsDropboxResumePlaybackEnabled
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "enable_equalizer_presets_sync",
                CellType = PreferenceCellType.Boolean,
                HeaderTitle = "Sync",
                Title = "Sync Equalizer Presets",
                Value = _config.IsDropboxResumePlaybackEnabled
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "enable_resume_playback_wifi_only",
                CellType = PreferenceCellType.Boolean,
                HeaderTitle = "Sync",
                Title = "Synchronize only on Wi-Fi",
                FooterTitle = "The cloud provider will be used to synchronize data between devices, excluding audio files. A small amount of bandwidth (≈1kb/call) is used every time you update a playlist, update a preset or skip to a different song.",
                Value = _config.IsDropboxResumePlaybackWifiOnlyEnabled
            });
        }

        public override void PreferenceValueChanged(PreferenceCellItem item)
        {
            var localItem = _items.FirstOrDefault(x => x.Id == item.Id);
            if (localItem == null)
                return;

            localItem.Value = item.Value;

            if (item.Id == "enable_resume_playback")
                _config.IsDropboxResumePlaybackEnabled = (bool)item.Value;
            else if (item.Id == "enable_resume_playback_wifi_only")
                _config.IsDropboxResumePlaybackWifiOnlyEnabled = (bool)item.Value;

            OnSetCloudPreferences(_config);
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _items[indexPath.Row];
            if (item.Id == "login_dropbox")
                OnDropboxLoginLogout();
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
            InvokeOnMainThread(() => {                
                GenerateItems();
                tableView.ReloadData();
            });
        }

        public void RefreshCloudPreferencesState(CloudPreferencesStateEntity entity)
        {
            InvokeOnMainThread(() =>
            {
                foreach(var item in _items)
                {
                    if(item.Id == "login_dropbox")
                        item.Title = entity.IsDropboxLinkedToApp ? "Logout from Dropbox" : "Login to Dropbox";
                    else
                        item.Enabled = entity.IsDropboxLinkedToApp;
                }
                tableView.ReloadData();
            });
        }

        #endregion
    }       
}
