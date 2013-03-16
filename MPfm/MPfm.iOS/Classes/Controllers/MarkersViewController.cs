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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.iOS
{
    public partial class MarkersViewController : BaseViewController, IMarkersView
    {
        private string _cellIdentifier = "MarkerCell";
        private List<Marker> _markers;

        public MarkersViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "MarkersViewController_iPhone" : "MarkersViewController_iPad", null)
        {
            _markers = new List<Marker>();
        }

        public override void ViewDidLoad()
        {
            lblTitle.Font = UIFont.FromName("OstrichSans-Black", 28);

            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            UILongPressGestureRecognizer longPress = new UILongPressGestureRecognizer(HandleLongPress);
            longPress.MinimumPressDuration = 1.0f;
            longPress.WeakDelegate = this;
            tableView.AddGestureRecognizer(longPress);

            base.ViewDidLoad();
        }

        private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
        {
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
            // Request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
            
            // Set cell style
            var cellStyle = UITableViewCellStyle.Subtitle;
            
            // Create cell if cell could not be recycled
            if (cell == null)
                cell = new UITableViewCell(cellStyle, _cellIdentifier);
            
            // Set title            
            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = _markers[indexPath.Row].Name;
            cell.DetailTextLabel.Text = _markers[indexPath.Row].Position;

            // Set theme
            cell.TextLabel.TextColor = UIColor.White;
            cell.DetailTextLabel.TextColor = UIColor.White;
            cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
            
            return cell;
        }
        
        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            Console.WriteLine("Selected marker");
            //OnItemClick(indexPath.Row);
        }


        partial void actionAddMarker(NSObject sender)
        {
            if(OnAddMarker != null)
                OnAddMarker();
        }

        #region IMarkersView implementation

        public Action OnAddMarker { get; set; }
        public Action<Marker> OnEditMarker { get; set; }

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
