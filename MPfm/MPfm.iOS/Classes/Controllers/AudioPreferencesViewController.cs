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
                Id = "login_dropbox",
                CellType = PreferenceCellType.Button,
                HeaderTitle = "Dropbox",
                Title = "Login to Dropbox",
                IconName = "dropbox"
            });
            _items.Add(new PreferenceCellItem()
            {
                Id = "enable_dropbox_resume_playback",
                CellType = PreferenceCellType.Boolean,
                HeaderTitle = "Dropbox",
                FooterTitle = "This will take a small amount of bandwidth (about 1 kilobyte) every time the player switches to a new song.",
                Title = "Enable Resume Playback"
            });
        }

        public override void PreferenceValueChanged(PreferenceCellItem item)
        {
        }
    }
}
