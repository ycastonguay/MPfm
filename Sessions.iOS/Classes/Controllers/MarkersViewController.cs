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
using System.Collections.Generic;
using System.Drawing;
using Sessions.Core;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters;
using Sessions.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Delegates;
using Sessions.iOS.Classes.Objects;
using System.Linq;
using Sessions.Sound.Objects;
using Sessions.iOS.Classes.Controls.Cells;

namespace Sessions.iOS.Classes.Controllers
{
    public partial class MarkersViewController : BaseViewController, IMarkersView
    {
        private const string _cellIdentifier = "MarkerCell";
        private List<Marker> _markers;
        private Guid _currentEditMarkerId = Guid.Empty;

        public MarkersViewController()
            : base (UserInterfaceIdiomIsPhone ? "MarkersViewController_iPhone" : "MarkersViewController_iPad", null)
        {
            _markers = new List<Marker>();
        }

        public override void ViewDidLoad()
        {
			tableView.BackgroundColor = UIColor.Clear;
			tableView.DelaysContentTouches = false;
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;
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
			//Tracing.Log("MarkersViewController - GetCell - indexPath.Row: {0}", indexPath.Row);
			var item = _markers[indexPath.Row];
			var cell = (SessionsMarkerTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
			if (cell == null)
			{
				//Tracing.Log("MarkersViewController - GetCell - CREATING NEW cell - indexPath.Row: {0}", indexPath.Row);
                cell = new SessionsMarkerTableViewCell(UITableViewCellStyle.Subtitle, _cellIdentifier);
				cell.OnLongPressMarker += HandleOnLongPressMarker;
				cell.OnDeleteMarker += HandleOnDeleteMarker;
				cell.OnPunchInMarker += HandleOnPunchInMarker;
				cell.OnChangeMarkerName += HandleOnChangeMarkerName; 
				cell.OnChangeMarkerPosition += HandleOnChangeMarkerPosition;
				cell.OnSetMarkerPosition += HandleOnSetMarkerPosition;
			}
			else
			{
				//Tracing.Log("MarkersViewController - GetCell - REUSING cell - indexPath.Row: {0}", indexPath.Row);
			}

            cell.Tag = indexPath.Row;
            cell.IndexTextLabel.Text = Conversion.IndexToLetter(indexPath.Row).ToString();
            cell.TitleTextLabel.Text = item.Name;
			cell.TextField.Text = item.Name;
			cell.PositionTextLabel.Text = item.Position;
			cell.Slider.Value = item.PositionPercentage;
			cell.MarkerId = item.MarkerId;

			// Check if the reused cell should be expanded
			int editIndex = _markers.FindIndex(x => x.MarkerId == _currentEditMarkerId);
			//Tracing.Log("!!! MarkersViewController - GetCell - indexPath.Row: {0} editIndex: {1} cellExpanded: {2}", indexPath.Row, editIndex, cell.IsExpanded);
			if (cell.IsExpanded)
			{
				if (editIndex != indexPath.Row)
				{
					//Tracing.Log("MarkersViewController - GetCell - COLLAPSING reused cell - indexPath.Row: {0}", indexPath.Row);
					cell.CollapseCell(false);
				}
			}
			else
			{
				if (editIndex == indexPath.Row)
				{
					//Tracing.Log("MarkersViewController - GetCell - EXPANDING reused cell - indexPath.Row: {0}", indexPath.Row);
					cell.ExpandCell(false);
				}
			}

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
			int index = _markers.FindIndex(x => x.MarkerId == _currentEditMarkerId);
			//Tracing.Log("MarkersViewController - HeightForRow - indexPath.Row: {0} index: {1} _currentEditMarkerId: {2}", indexPath.Row, index, _currentEditMarkerId);
			return index == indexPath.Row ? 126 : 52;
		}

		[Export ("tableView:heightForFooterInSection:")]
		public float HeightForFooterInSection(UITableView tableView, int section)
		{
			// This will create an "invisible" footer
			return 0.01f;
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
            var actionSheet = new UIActionSheet("Select a marker name template:", null, "Cancel", null, new string[4] { "Verse", "Chorus", "Bridge", "Solo" });
            actionSheet.Style = UIActionSheetStyle.BlackTranslucent;
            actionSheet.Clicked += (eventSender, e) => {

                // Check for cancel
                if(e.ButtonIndex == 4)
                    return;

                OnAddMarkerWithTemplate((MarkerTemplateNameType)e.ButtonIndex);
            };

            // Must use the tab bar controller to spawn the action sheet correctly. Remember, we're in a UIScrollView...
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			actionSheet.ShowFromTabBar(appDelegate.MainViewController.TabBarController.TabBar);
        }

		private void HandleOnLongPressMarker(Guid markerId)
		{
			Tracing.Log("HandleOnLongPressMarker - markerId: {0}", markerId);
			int previousIndex = _markers.FindIndex(x => x.MarkerId == _currentEditMarkerId);
			int index = _markers.FindIndex(x => x.MarkerId == markerId);
			if (index == -1)
				return;

			NSIndexPath indexPath = NSIndexPath.FromRowSection(index, 0);
			NSIndexPath indexPathEdit = NSIndexPath.FromRowSection(previousIndex, 0);
			if (indexPath != null)
			{
				Tracing.Log("MarkersViewController - HandleLongPress");

				var cell = (SessionsMarkerTableViewCell)tableView.CellAt(indexPath);
				var previousCell = (SessionsMarkerTableViewCell)tableView.CellAt(indexPathEdit);

				// Execute animation for new row height (as simple as that!)
				_currentEditMarkerId = _currentEditMarkerId == markerId ? Guid.Empty : markerId;
				tableView.BeginUpdates();
				tableView.EndUpdates();			

				if (previousCell != null)
					previousCell.CollapseCell(true);
				if (cell != null)
				{
					OnSetActiveMarker(_currentEditMarkerId);
					if (_currentEditMarkerId == Guid.Empty)
					{
						if(indexPath.Row != indexPathEdit.Row)
							cell.CollapseCell(true);
					}
					else
					{
						cell.ExpandCell(true);
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
						_currentEditMarkerId = Guid.Empty;
						OnDeleteMarker(item);
						break;
				}
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

		private void HandleOnChangeMarkerName(Guid markerId, string newName)
		{
			OnChangeMarkerName(markerId, newName);
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
		public Action<Guid, string> OnChangeMarkerName { get; set; }
		public Action<Guid, float> OnChangeMarkerPosition { get; set; }
		public Action<Guid, float> OnSetMarkerPosition { get; set; }
		public Action<Guid> OnSetActiveMarker { get; set; }
		public Action<Guid> OnPunchInMarker { get; set; }
		public Action<Guid> OnUndoMarker { get; set; }

        public void MarkerError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshMarkers(List<Marker> markers)
        {
			//Tracing.Log("MarkersViewController - RefreshMarkers - markers.Count: {0}", markers.Count);
            InvokeOnMainThread(() => {
                _markers = markers;
                tableView.ReloadData();
            });
        }

		public void RefreshMarkerPosition(Marker marker, int newIndex)
		{
			InvokeOnMainThread(() => {
				int index = _markers.FindIndex(x => x.MarkerId == marker.MarkerId);
				//Tracing.Log("MarkersViewController - RefreshMarkerPosition - markerId: {0} position: {1} index: {2} newIndex: {3}", marker.MarkerId, marker.Position, index, newIndex);
				if(index >= 0)
					_markers[index] = marker;

				// Update position
				var cell = tableView.CellAt(NSIndexPath.FromRowSection(index, 0)) as SessionsMarkerTableViewCell;
				if(cell != null)
					cell.PositionTextLabel.Text = marker.Position;

				// Check for row movement
				if(index != newIndex)
				{
					//tableView.MoveRow(NSIndexPath.FromRowSection(index, 0), NSIndexPath.FromRowSection(newIndex, 0));
					tableView.ScrollToRow(NSIndexPath.FromRowSection(newIndex, 0), UITableViewScrollPosition.Top, true);
				}
			});
		}

        #endregion
    }
}
