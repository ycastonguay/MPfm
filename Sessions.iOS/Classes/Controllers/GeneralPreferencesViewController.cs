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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.MVP.Views;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.iOS.Classes.Objects;
using System.Collections.Generic;
using Sessions.MVP.Config.Models;
using System.Linq;

namespace Sessions.iOS
{
    public partial class GeneralPreferencesViewController : BasePreferencesViewController, IGeneralPreferencesView
    {
		private GeneralAppConfig _config;
		private string _peakFolderSize;
		private string _cellIdentifier = "CloudPreferencesCell";
        //CloudAppConfig _config;
		private List<PreferenceCellItem> _items = new List<PreferenceCellItem>();

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
            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindGeneralPreferencesView(this);
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            var navCtrl = (SessionsNavigationController)this.NavigationController;
            navCtrl.SetTitle("General Preferences");
        }

        private void GenerateItems()
        {
            // We assume the items are in order for sections
            _items = new List<PreferenceCellItem>();
            _items.Add(new PreferenceCellItem()
            {
                Id = "download_missing_album_art",
                CellType = PreferenceCellType.Boolean,
                HeaderTitle = "Download Album Art",
                Title = "Download Missing Album Art",
                Value = _config.DownloadAlbumArtFromTheInternet
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "download_album_art_on_cellular_network",
                CellType = PreferenceCellType.Boolean,
                HeaderTitle = "Download Album Art",
                Title = "Use Cellular Network (3G/LTE)",
                Value = _config.DownloadAlbumArtOnCellularNetwork
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "update_frequency_song_position",
                CellType = PreferenceCellType.Slider,
                HeaderTitle = "Update Frequency",
                Title = "Song Position",
                ScaleName = "ms",
				Value = _config.SongPositionUpdateFrequency,
                MinValue = 10,
                MaxValue = 100
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "update_frequency_output_meter",
                CellType = PreferenceCellType.Slider,
                HeaderTitle = "Update Frequency",
                Title = "Output Meter",
                ScaleName = "ms",
				Value = _config.OutputMeterUpdateFrequency,
                MinValue = 10,
                MaxValue = 100,
                FooterTitle = "Warning: Lower values require more CPU and memory."
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "peak_files_maximum_size",
                CellType = PreferenceCellType.Slider,
                HeaderTitle = "Peak Files",
                Title = "Maximum Peak Folder Size",
                ScaleName = "MB",
				Value = _config.MaximumPeakFolderSize,
                MinValue = 50,
                MaxValue = 1000
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "delete_peak_files",
                CellType = PreferenceCellType.Button,
                HeaderTitle = "Peak Files",
                Title = "Delete Peak Files",
				FooterTitle = "Peak file folder size: " + _peakFolderSize,
                IconName = "delete"
            });
        }

		public override void PreferenceValueChanged(PreferenceCellItem item)
		{
			var localItem = _items.FirstOrDefault(x => x.Id == item.Id);
			if (localItem == null)
				return;

			localItem.Value = item.Value;

			if (item.Id == "update_frequency_song_position")
				_config.SongPositionUpdateFrequency = (int)item.Value;
			else if (item.Id == "update_frequency_output_meter")
				_config.OutputMeterUpdateFrequency = (int)item.Value;
			else if (item.Id == "peak_files_maximum_size")
				_config.MaximumPeakFolderSize = (int)item.Value;
            else if (item.Id == "download_missing_album_art")
                _config.DownloadAlbumArtFromTheInternet = (bool)item.Value;
            else if (item.Id == "download_album_art_on_cellular_network")
                _config.DownloadAlbumArtOnCellularNetwork = (bool)item.Value;


            Console.WriteLine("===>>> PreferenceValueChanged - config.DownloadAlbumArtFromTheInternet: {0}", _config.DownloadAlbumArtFromTheInternet);
			OnSetGeneralPreferences(_config);
		}

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var distinct = _items.Select(x => x.HeaderTitle).Distinct().ToList();
			string headerTitle = distinct[indexPath.Section];
			var items = _items.Where(x => x.HeaderTitle == headerTitle).ToList();
			var item = items[indexPath.Row];
			tableView.DeselectRow(indexPath, true);

			if (item.Id == "delete_peak_files")
			{
				var alertView = new UIAlertView("Delete Peak Files", "Are you sure you wish to delete the peak files folder? Peak files can always be generated later.", null, "OK", new string[1]{ "Cancel" });
				alertView.Clicked += (sender2, e) =>
				{
					if (e.ButtonIndex == 0)
						OnDeletePeakFiles();
				};
				alertView.Show();
			}
		}

		#region IGeneralPreferencesView implementation  

		public Action<GeneralAppConfig> OnSetGeneralPreferences { get; set; }
		public Action OnDeletePeakFiles { get; set; }

		public void GeneralPreferencesError(Exception ex)
		{
			ShowErrorDialog(ex);
		}

		public void RefreshGeneralPreferences(GeneralAppConfig config, string peakFolderSize)
		{
            Console.WriteLine("===>>> RefreshGeneralPreferences - config.DownloadAlbumArtFromTheInternet: {0}", config.DownloadAlbumArtFromTheInternet);
			_config = config;
			_peakFolderSize = peakFolderSize;
			InvokeOnMainThread(() => {                
				GenerateItems();
				tableView.ReloadData();
			});
		}

		#endregion
    }
}
