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
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Delegates;
using MPfm.iOS.Classes.Objects;
using System.Linq;

namespace MPfm.iOS
{
    public partial class MarkersViewController : BaseViewController, IMarkersView
    {
        string _cellIdentifier = "MarkerCell";
        List<Marker> _markers;
		int _currentEditIndex = -1;

        public MarkersViewController()
            : base (UserInterfaceIdiomIsPhone ? "MarkersViewController_iPhone" : "MarkersViewController_iPad", null)
        {
            _markers = new List<Marker>();
        }

        public override void ViewDidLoad()
        {
			tableView.BackgroundColor = UIColor.Clear;
			tableView.DelaysContentTouches = true;
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;
			//btnAddMarker.BackgroundColor = GlobalTheme.PlayerPanelButtonColor;
			//btnAddMarker.Alpha = GlobalTheme.PlayerPanelButtonAlpha;
			btnAddMarker.GlyphImageView.Image = UIImage.FromBundle("Images/Player/add");

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindMarkersView(this);
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _markers.Count;
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
			var item = _markers[indexPath.Row];
			MPfmMarkerTableViewCell cell = (MPfmMarkerTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;                
				cell = new MPfmMarkerTableViewCell(cellStyle, _cellIdentifier);
				cell.OnLongPressMarker += HandleOnLongPressMarker;
				cell.OnDeleteMarker += HandleOnDeleteMarker;
				cell.OnPunchInMarker += HandleOnPunchInMarker;
				cell.OnUndoMarker += HandleOnUndoMarker;
				cell.OnChangeMarkerPosition += HandleOnChangeMarkerPosition;
				cell.OnSetMarkerPosition += HandleOnSetMarkerPosition;
            }

            cell.Tag = indexPath.Row;
            cell.BackgroundColor = UIColor.Clear;
            cell.IndexTextLabel.Text = Conversion.IndexToLetter(indexPath.Row).ToString();
			cell.TextLabel.Text = item.Name;
			cell.TextField.Text = item.Name;
			cell.DetailTextLabel.Text = item.Position;
			cell.Slider.Value = item.PositionPercentage;
			cell.MarkerId = item.MarkerId;

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

		[Export ("tableView:heightForRowAtIndexPath:")]
		public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			//Tracing.Log("MarkersViewController - HeightForRow - indexPath.Row: {0} indexPath.Section: {1} _currentEditIndex: {2}", indexPath.Row, indexPath.Section, _currentEditIndex);
			return indexPath.Row == _currentEditIndex ? 172 : 52;
		}

		[Export ("tableView:heightForFooterInSection:")]
		public float HeightForFooterInSection(UITableView tableView, int section)
		{
			// This will create an "invisible" footer
			return 0.01f;
			//return 1f;
		}

		[Export ("tableView:viewForFooterInSection:")]
		public UIView ViewForFooterInSection(UITableView tableview, int section)
		{
			// Remove extra separators
			return new UIView();
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

                OnAddMarkerWithTemplate((MarkerTemplateNameType)e.ButtonIndex);
            };

            // Must use the tab bar controller to spawn the action sheet correctly. Remember, we're in a UIScrollView...
            AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			actionSheet.ShowFromTabBar(appDelegate.MainViewController.TabBarController.TabBar);
        }

		private void HandleOnLongPressMarker(Guid markerId)
		{
			Tracing.Log("HandleOnLongPressMarker - markerId: {0}", markerId);
			int index = _markers.FindIndex(x => x.MarkerId == markerId);
			if (index == -1)
				return;

			NSIndexPath indexPath = NSIndexPath.FromRowSection(index, 0);
			NSIndexPath indexPathEdit = NSIndexPath.FromRowSection(_currentEditIndex, 0);
			if (indexPath != null)
			{
				Tracing.Log("MarkersViewController - HandleLongPress");

				var cell = (MPfmMarkerTableViewCell)tableView.CellAt(indexPath);
				var previousCell = (MPfmMarkerTableViewCell)tableView.CellAt(indexPathEdit);

				// Execute animation for new row height (as simple as that!)
				_currentEditIndex = _currentEditIndex == indexPath.Row ? -1 : indexPath.Row;
				tableView.BeginUpdates();
				tableView.EndUpdates();			

				if (previousCell != null)
					previousCell.CollapseCell();
				if (cell != null)
				{
					if (_currentEditIndex == -1)
					{
						if(indexPath.Row != indexPathEdit.Row)
							cell.CollapseCell();
					}
					else
					{
						cell.ExpandCell();
						tableView.ScrollToRow(indexPath, UITableViewScrollPosition.Top, true);
					}
				}
			}
		}

		private void HandleOnDeleteMarker(Guid markerId)
		{
			var item = _markers.FirstOrDefault(x => x.MarkerId == markerId);
			if (item == null)
				return;

			var alertView = new UIAlertView("Delete confirmation", string.Format("Are you sure you wish to delete {0}?", item.Name), null, "OK", new string[1] { "Cancel" });
			alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
				switch(e2.ButtonIndex)
				{
					case 0:
						OnDeleteMarker(item);
						break;
				}
				_currentEditIndex = -1;
			};
			alertView.Show();
		}

		private void HandleOnPunchInMarker(Guid markerId)
		{
			OnPunchInMarker(markerId);
		}

		private void HandleOnUndoMarker(Guid markerId)
		{
			OnUndoMarker(markerId);
		}

		private void HandleOnChangeMarkerPosition(Guid markerId, float newPositionPercentage)
		{
			OnChangeMarkerPosition(markerId, newPositionPercentage);
		}

		private void HandleOnSetMarkerPosition(Guid markerId, float newPositionPercentage)
		{
			OnSetMarkerPosition(markerId, newPositionPercentage);
		}

        #region IMarkersView implementation

        public Action OnAddMarker { get; set; }
        public Action<MarkerTemplateNameType> OnAddMarkerWithTemplate { get; set; }
        public Action<Marker> OnEditMarker { get; set; }
        public Action<Marker> OnSelectMarker { get; set; }
        public Action<Marker> OnDeleteMarker { get; set; }
		public Action<Marker> OnUpdateMarker { get; set; }
		public Action<Guid, float> OnChangeMarkerPosition { get; set; }
		public Action<Guid, float> OnSetMarkerPosition { get; set; }
		public Action<Guid> OnPunchInMarker { get; set; }
		public Action<Guid> OnUndoMarker { get; set; }

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

		public void RefreshMarkerPosition(Marker marker)
		{
			InvokeOnMainThread(() => {
				int index = _markers.FindIndex(x => x.MarkerId == marker.MarkerId);
				if(index >= 0)
					_markers[index] = marker;

				tableView.ReloadData();

//				var marker = _markers.FirstOrDefault(x => x.MarkerId == marker.MarkerId);
//				if(marker == null)
//					return;
//
//				marker.Position = position;
				//marker.PositionBytes
				//marker.PositionSamples

			});
		}

        #endregion
    }

//	public class MarkersLongPressGestureRecognizer : UILongPressGestureRecognizer
//	{
//		private bool _cancelTouches = false;
//
//		public MarkersLongPressGestureRecognizer(Action<UILongPressGestureRecognizer> action) 
//			: base(action)
//		{
//
//		}
//
//		public override bool CancelsTouchesInView
//		{
//			get
//			{
//				Tracing.Log("MarkersLongPressGR - CancelsTouchesInView - Get value _cancelTouches: {0}", _cancelTouches);
//				return _cancelTouches;
//			}
//			set
//			{
//				Tracing.Log("MarkersLongPressGR - CancelsTouchesInView - Set value: {0} _cancelTouches: {1}", value, _cancelTouches);
//				_cancelTouches = value;
//			}
//		}
//
//		public override PointF LocationInView(UIView view)
//		{
//			// Find a way to cancel long press on controls of MarkersTableViewCell
//			var location = base.LocationInView(view);
//			_cancelTouches = location.Y > 44;
//			Tracing.Log("MarkersLongPressGR - LocationInView - view: {0} - location: {1} cancelTouches: {2}", view.GetType().FullName, location, _cancelTouches);
//			return location;
//		}
//	}
}
