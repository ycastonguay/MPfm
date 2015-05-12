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
using Sessions.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using org.sessionsapp.player;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Controls.Cells;

namespace Sessions.iOS
{
    public partial class LoopsViewController : BaseViewController, ILoopsView
    {
        private const string _cellIdentifier = "LoopCell";
        private List<SSPLoop> _loops;
        private Guid _currentEditLoopId = Guid.Empty;

        public LoopsViewController()
			: base (UserInterfaceIdiomIsPhone ? "LoopsViewController_iPhone" : "LoopsViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            tableView.BackgroundColor = UIColor.Clear;
            tableView.DelaysContentTouches = false;
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;

            btnAddLoop.GlyphImageView.Image = UIImage.FromBundle("Images/Player/add");

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindLoopsView(this);
        }

        partial void actionAddLoop(NSObject sender)
        {
            if(OnAddLoop != null)
                OnAddLoop();
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _loops.Count;
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            //Tracing.Log("MarkersViewController - GetCell - indexPath.Row: {0}", indexPath.Row);
            var item = _loops[indexPath.Row];
            var cell = (SessionsMarkerTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                //Tracing.Log("MarkersViewController - GetCell - CREATING NEW cell - indexPath.Row: {0}", indexPath.Row);
                var cellStyle = UITableViewCellStyle.Subtitle;                
                cell = new SessionsMarkerTableViewCell(cellStyle, _cellIdentifier);
//                cell.OnLongPressMarker += HandleOnLongPressMarker;
//                cell.OnDeleteMarker += HandleOnDeleteMarker;
//                cell.OnPunchInMarker += HandleOnPunchInMarker;
//                cell.OnChangeMarkerName += HandleOnChangeMarkerName; 
//                cell.OnChangeMarkerPosition += HandleOnChangeMarkerPosition;
//                cell.OnSetMarkerPosition += HandleOnSetMarkerPosition;
            }
            else
            {
                //Tracing.Log("MarkersViewController - GetCell - REUSING cell - indexPath.Row: {0}", indexPath.Row);
            }

//            cell.Tag = indexPath.Row;
//            cell.BackgroundColor = UIColor.Clear;
//            cell.IndexTextLabel.Text = Conversion.IndexToLetter(indexPath.Row).ToString();
//            cell.TitleTextLabel.Text = item.Name;
//            cell.TextField.Text = item.Name;
//            cell.PositionTextLabel.Text = item.Position;
//            cell.Slider.Value = item.PositionPercentage;
//            cell.MarkerId = item.MarkerId;
//
//            // Check if the reused cell should be expanded
//            int editIndex = _markers.FindIndex(x => x.MarkerId == _currentEditMarkerId);
//            //Tracing.Log("!!! MarkersViewController - GetCell - indexPath.Row: {0} editIndex: {1} cellExpanded: {2}", indexPath.Row, editIndex, cell.IsExpanded);
//            if (cell.IsExpanded)
//            {
//                if (editIndex != indexPath.Row)
//                {
//                    //Tracing.Log("MarkersViewController - GetCell - COLLAPSING reused cell - indexPath.Row: {0}", indexPath.Row);
//                    cell.CollapseCell(false);
//                }
//            }
//            else
//            {
//                if (editIndex == indexPath.Row)
//                {
//                    //Tracing.Log("MarkersViewController - GetCell - EXPANDING reused cell - indexPath.Row: {0}", indexPath.Row);
//                    cell.ExpandCell(false);
//                }
//            }

            return cell;
        }
                        
        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if(OnSelectLoop != null)
            {
                OnSelectLoop(_loops[indexPath.Row]);
                tableView.DeselectRow(indexPath, true);
            }
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            int index = _loops.FindIndex(x => x.LoopId == _currentEditLoopId);
            //Tracing.Log("MarkersViewController - HeightForRow - indexPath.Row: {0} index: {1} _currentEditMarkerId: {2}", indexPath.Row, index, _currentEditMarkerId);
            return index == indexPath.Row ? 126 : 52;
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

        #region ILoopsView implementation

        public Action OnAddLoop { get; set; }
        public Action<SSPLoop> OnEditLoop { get; set; }
        public Action<SSPLoop> OnSelectLoop { get; set; }
        public Action<SSPLoop> OnDeleteLoop { get; set; }
        public Action<SSPLoop> OnUpdateLoop { get; set; }
        public Action<SSPLoop> OnPlayLoop { get; set; }

        public Action<SSPLoopSegmentType> OnPunchInLoopSegment { get; set; }
        public Action<SSPLoopSegmentType, float> OnChangingLoopSegmentPosition { get; set; }
        public Action<SSPLoopSegmentType, float> OnChangedLoopSegmentPosition { get; set; }

        public void LoopError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshLoops(List<SSPLoop> loops)
        {
            InvokeOnMainThread(() => {
                _loops = loops;
                tableView.ReloadData();
            });
        }

        public void RefreshCurrentlyPlayingLoop(SSPLoop loop)
        {
        }

        #endregion
    }
}
