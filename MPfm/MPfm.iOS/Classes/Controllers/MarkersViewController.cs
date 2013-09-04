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
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Delegates;
using MPfm.iOS.Classes.Objects;
using MPfm.Core;

namespace MPfm.iOS
{
    public partial class MarkersViewController : BaseViewController, IMarkersView
    {
        string _cellIdentifier = "MarkerCell";
        List<Marker> _markers;

        public MarkersViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "MarkersViewController_iPhone" : "MarkersViewController_iPad", null)
        {
            _markers = new List<Marker>();
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;
            btnAddMarker.BackgroundColor = GlobalTheme.PlayerPanelButtonColor;
            btnAddMarker.Alpha = GlobalTheme.PlayerPanelButtonAlpha;

            UILongPressGestureRecognizer longPress = new UILongPressGestureRecognizer(HandleLongPress);
            longPress.MinimumPressDuration = 0.7f;
            longPress.WeakDelegate = this;
            tableView.AddGestureRecognizer(longPress);

            base.ViewDidLoad();
        }

        private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
                return;

            PointF pt = gestureRecognizer.LocationInView(tableView);
            NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(pt);
            if (indexPath != null)
                if(OnEditMarker != null)
                    OnEditMarker(_markers[indexPath.Row]);
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _markers.Count;
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;                
                cell = new MPfmTableViewCell(cellStyle, _cellIdentifier);
            }

            cell.Tag = indexPath.Row;
            cell.BackgroundColor = UIColor.Clear;
            cell.BackgroundView = null;
            cell.IndexTextLabel.Text = Conversion.IndexToLetter(indexPath.Row).ToString();
            cell.IndexTextLabel.BackgroundColor = UIColor.FromRGBA(1, 0, 0, 0.7f);
            cell.IndexTextLabel.TextColor = UIColor.White;
            cell.TextLabel.Text = _markers[indexPath.Row].Name;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
            cell.TextLabel.TextColor = UIColor.White;
            cell.DetailTextLabel.Text = _markers[indexPath.Row].Position;
            cell.DetailTextLabel.Font = UIFont.FromName("HelveticaNeue", 12);
            cell.DetailTextLabel.TextColor = UIColor.LightGray;

            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;
            
            return cell;
        }
        
        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if(OnSelectMarker != null)
            {
                OnSelectMarker(_markers[indexPath.Row]);
                tableView.DeselectRow(indexPath, true);
            }
        }

        partial void actionAddMarker(NSObject sender)
        {
            // Show a list of templates for the marker name
            UIActionSheet actionSheet = new UIActionSheet("Select a marker name template:", null, "Cancel", null, new string[4] { "Verse", "Chorus", "Bridge", "Solo" });
            actionSheet.Style = UIActionSheetStyle.BlackTranslucent;
            actionSheet.Clicked += (eventSender, e) => {

                // Check for cancel
                if(e.ButtonIndex == 4)
                    return;

                // Add marker
                if(OnAddMarker != null)
                    OnAddMarker((MarkerTemplateNameType)e.ButtonIndex);
            };

            // Must use the tab bar controller to spawn the action sheet correctly. Remember, we're in a UIScrollView...
            AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            actionSheet.ShowFromTabBar(appDelegate.TabBarController.TabBar);
        }

        #region IMarkersView implementation

        public Action<MarkerTemplateNameType> OnAddMarker { get; set; }
        public Action<Marker> OnEditMarker { get; set; }
        public Action<Marker> OnSelectMarker { get; set; }
        public Action<Marker> OnDeleteMarker { get; set; }

        public void MarkerError(Exception ex)
        {
            InvokeOnMainThread(() => {
                UIAlertView alertView = new UIAlertView("Marker Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshMarkers(List<Marker> markers)
        {
            InvokeOnMainThread(() => {
                _markers = markers;
                tableView.ReloadData();
            });
        }

        #endregion
    }
}
