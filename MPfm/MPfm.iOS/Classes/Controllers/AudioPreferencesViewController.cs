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

namespace MPfm.iOS
{
    public partial class AudioPreferencesViewController : BasePreferencesViewController, IAudioPreferencesView
    {
        string _cellIdentifier = "AudioPreferencesCell";
        List<string> _items = new List<string>();

        public AudioPreferencesViewController()
            : base (UserInterfaceIdiomIsPhone ? "AudioPreferencesViewController_iPhone" : "AudioPreferencesViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            _items.Add("Bacon");
            _items.Add("Drumstick");

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                NavigationController.InteractivePopGestureRecognizer.WeakDelegate = this;
                NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            }

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

//        [Export ("tableView:viewForHeaderInSection:")]
//        public UIView ViewForHeaderInSection(UITableView tableview, int section)
//        {
//        }

        [Export ("tableView:titleForHeaderInSection:")]
        public string TitleForHeaderInSection(UITableView tableview, int section)
        {
            return "Audio Mixer";
        }

        [Export ("numberOfSectionsInTableView:")]
        public int SectionsInTableView(UITableView tableview)
        {
            return 1;
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _items.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _items[indexPath.Row];
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new MPfmTableViewCell(cellStyle, _cellIdentifier);
            }

            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = item;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            cell.Accessory = UITableViewCellAccessory.None;

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //OnSelectItem(_items[indexPath.Row]);
        }       

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }
    }
}
