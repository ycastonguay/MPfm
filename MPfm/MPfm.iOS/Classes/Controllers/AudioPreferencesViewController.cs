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
using System.Collections.Generic;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS
{
    public partial class AudioPreferencesViewController : BasePreferencesViewController, IAudioPreferencesView
    {
        string _cellIdentifier = "AudioPreferencesCell";
        List<PreferenceCellItem> _items = new List<PreferenceCellItem>();

        #region BasePreferencesViewController

        public override string CellIdentifier { get { return _cellIdentifier; } }
        public override UITableView TableView { get { return tableView; } }
        public override List<PreferenceCellItem> Items { get { return _items; } }

        #endregion

        public AudioPreferencesViewController()
            : base (UserInterfaceIdiomIsPhone ? "AudioPreferencesViewController_iPhone" : "AudioPreferencesViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            GenerateItems();
            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindAudioPreferencesView(this);
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Audio Preferences");
        }

        private void GenerateItems()
        {
            // We assume the items are in order for sections
            _items = new List<PreferenceCellItem>();
            _items.Add(new PreferenceCellItem()
            {
                Id = "buffer_size",
                CellType = PreferenceCellType.Slider,
                HeaderTitle = "Audio Mixer",
                Title = "Buffer Size",
                ScaleName = "ms",
                Value = 1000,
                MinValue = 100,
                MaxValue = 5000
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "update_period",
                CellType = PreferenceCellType.Slider,
                HeaderTitle = "Audio Mixer",
                Title = "Update Period",
                ScaleName = "ms",
                Value = 100,
                MinValue = 10,
                MaxValue = 1000,
                FooterTitle = "Warning: Lower values require more CPU and memory. Audio sample rate is locked at 44100Hz."
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
