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
using System.Drawing;
using Sessions.MVP.Config.Models;
using Sessions.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Controls;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using System.Collections.Generic;
using Sessions.iOS.Classes.Objects;
using System.Linq;
using Sessions.Library.Objects;

namespace Sessions.iOS
{
    public partial class LibraryPreferencesViewController : BasePreferencesViewController, ILibraryPreferencesView
    {
		private string _cellIdentifier = "CloudPreferencesCell";
		private LibraryAppConfig _config;
		private string _librarySize;
		private List<PreferenceCellItem> _items = new List<PreferenceCellItem>();

        #region BasePreferencesViewController

        public override string CellIdentifier { get { return _cellIdentifier; } }
        public override UITableView TableView { get { return tableView; } }
        public override List<PreferenceCellItem> Items { get { return _items; } }

        #endregion

        public LibraryPreferencesViewController()
            : base (UserInterfaceIdiomIsPhone ? "LibraryPreferencesViewController_iPhone" : "LibraryPreferencesViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindLibraryPreferencesView(this);
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            SessionsNavigationController navCtrl = (SessionsNavigationController)this.NavigationController;
            navCtrl.SetTitle("Library Preferences");
        }

        private void GenerateItems()
        {
            // We assume the items are in order for sections
            _items = new List<PreferenceCellItem>();
            _items.Add(new PreferenceCellItem()
            {
				Id = "sync_service_enabled",
                CellType = PreferenceCellType.Boolean,
                HeaderTitle = "Sync Service",
                Title = "Enable Sync Service",
				Description = "Allows remote devices to access this library",
				Value = _config.IsSyncServiceEnabled
            });
            _items.Add(new PreferenceCellItem()
            {
				Id = "sync_service_port",
                CellType = PreferenceCellType.Integer,
                HeaderTitle = "Sync Service",
                Title = "HTTP Port",
                FooterTitle = "Note: The sync service is only used when Wi-Fi is available.",
				Enabled = !_config.IsSyncServiceEnabled,
				Value = _config.SyncServicePort,
				ValidateFailErrorMessage = "The sync service HTTP port must be between 80 and 65535.",
				ValidateValueDelegate = (value) => {
					int newPort = 0;
					int.TryParse(value, out newPort);
					return (newPort >= 80 && newPort <= 65535);
				}
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "library_reset",
                CellType = PreferenceCellType.Button,
                HeaderTitle = "Library",
                Title = "Reset Library",
                IconName = "reset"
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "library_update",
                CellType = PreferenceCellType.Button,
                HeaderTitle = "Library",
                Title = "Update Library",
				FooterTitle = "Total library size: " + _librarySize,             
                IconName = "update"
            });
        }

        public override void PreferenceValueChanged(PreferenceCellItem item)
        {
            var localItem = _items.FirstOrDefault(x => x.Id == item.Id);
            if (localItem == null)
                return;

            localItem.Value = item.Value;

			if (item.Id == "sync_service_enabled")
				_config.IsSyncServiceEnabled = (bool)item.Value;
			else if (item.Id == "sync_service_port")
				_config.SyncServicePort = (int)item.Value;

			OnSetLibraryPreferences(_config);
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var distinct = _items.Select(x => x.HeaderTitle).Distinct().ToList();
            string headerTitle = distinct[indexPath.Section];
            var items = _items.Where(x => x.HeaderTitle == headerTitle).ToList();
            var item = items[indexPath.Row];
            tableView.DeselectRow(indexPath, true);

			if (item.Id == "library_reset")
			{
				var alertView = new UIAlertView("Reset Library", "Are you sure you wish to reset your library?", null, "OK", new string[1]{ "Cancel" });
				alertView.Clicked += (sender2, e) =>
				{
					if (e.ButtonIndex == 0)
						OnResetLibrary();
				};
				alertView.Show();
			}
			else if (item.Id == "library_update")
			{
				OnUpdateLibrary();
			}
        }

        #region ILibraryPreferencesView implementation

        public Action<LibraryAppConfig> OnSetLibraryPreferences { get; set; }
        public Action OnResetLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }
        public Action OnSelectFolders { get; set; }
        public Action<string, bool> OnAddFolder { get; set; }
        public Action<IEnumerable<Folder>, bool> OnRemoveFolders { get; set; }
		public Action<bool> OnEnableSyncListener { get; set; }
        public Action<int> OnSetSyncListenerPort { get; set; }

        public void LibraryPreferencesError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Library Preferences error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

		public void RefreshLibraryPreferences(LibraryAppConfig config, string librarySize)
        {
			_config = config;
			_librarySize = librarySize;
			InvokeOnMainThread(() => {                
				GenerateItems();
				tableView.ReloadData();
			});
        }

        #endregion
    }
}
