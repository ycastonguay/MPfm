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
using Sessions.Core;
using System.Linq;

namespace Sessions.iOS.Classes.Controllers
{
    public partial class LoopsViewController : BaseViewController, ILoopsView
    {
        private const string _cellIdentifier = "LoopCell";
        private List<SSPLoop> _loops;
        private Guid _currentEditLoopId = Guid.Empty;
        private Guid _currentlyPlayingLoopId = Guid.Empty;

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
            return _loops != null ? _loops.Count : 0;
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            //Tracing.Log("MarkersViewController - GetCell - indexPath.Row: {0}", indexPath.Row);
            var item = _loops[indexPath.Row];
            var cell = (SessionsLoopTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                //Tracing.Log("MarkersViewController - GetCell - CREATING NEW cell - indexPath.Row: {0}", indexPath.Row);
                cell = new SessionsLoopTableViewCell(UITableViewCellStyle.Subtitle, _cellIdentifier);
                cell.OnLongPressLoop += HandleOnLongPressLoop;
                cell.OnDeleteLoop += HandleOnDeleteLoop;
                cell.OnPunchInLoop += HandleOnPunchInLoop;
                cell.OnChangeLoopName += HandleOnChangeLoopName; 
                cell.OnChangeLoopPosition += HandleOnChangeLoopPosition;
                cell.OnSetLoopPosition += HandleOnSetLoopPosition;
            }
            else
            {
                //Tracing.Log("MarkersViewController - GetCell - REUSING cell - indexPath.Row: {0}", indexPath.Row);
            }

            cell.Tag = indexPath.Row;
            cell.IndexTextLabel.Text = Conversion.IndexToLetter(indexPath.Row).ToString();
            cell.SetLoop(item, true);

            // Check if the reused cell should be expanded
            int editIndex = _loops.FindIndex(x => x.LoopId == _currentEditLoopId);
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
            if(OnSelectLoop != null)
            {
//                OnSelectLoop(_loops[indexPath.Row]);
                OnPlayLoop(_loops[indexPath.Row]);
                tableView.DeselectRow(indexPath, true);
            }
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            int index = _loops.FindIndex(x => x.LoopId == _currentEditLoopId);
            return index == indexPath.Row ? 182 : 52;
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

        private void HandleOnLongPressLoop(Guid loopId)
        {
            Tracing.Log("HandleOnLongPressLoop - loopId: {0}", loopId);
            int previousIndex = _loops.FindIndex(x => x.LoopId == _currentEditLoopId);
            int index = _loops.FindIndex(x => x.LoopId == loopId);
            if (index == -1)
                return;

            NSIndexPath indexPath = NSIndexPath.FromRowSection(index, 0);
            NSIndexPath indexPathEdit = NSIndexPath.FromRowSection(previousIndex, 0);
            if (indexPath != null)
            {
                Tracing.Log("LoopsViewController - HandleLongPress");

                var cell = (SessionsLoopTableViewCell)tableView.CellAt(indexPath);
                var previousCell = (SessionsLoopTableViewCell)tableView.CellAt(indexPathEdit);

                // Execute animation for new row height (as simple as that!)
                _currentEditLoopId = _currentEditLoopId == loopId ? Guid.Empty : loopId;
                tableView.BeginUpdates();
                tableView.EndUpdates();         

                if (previousCell != null)
                    previousCell.CollapseCell(true);

                if (cell != null)
                {
                    if (_currentEditLoopId == Guid.Empty)
                    {
                        OnSelectLoop(null);
                        if(indexPath.Row != indexPathEdit.Row)
                            cell.CollapseCell(true);
                    }
                    else
                    {
                        OnSelectLoop(_loops[index]);
                        cell.ExpandCell(true);
                        tableView.ScrollToRow(indexPath, UITableViewScrollPosition.Top, true);
                    }
                }
            }
        }

        private void HandleOnDeleteLoop(Guid loopId)
        {
            var item = _loops.FirstOrDefault(x => x.LoopId == loopId);
            if (item == null)
                return;

            var alertView = new UIAlertView("Delete confirmation", string.Format("Are you sure you wish to delete {0}?", item.Name), null, "OK", new string[1] { "Cancel" });
            alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
                switch(e2.ButtonIndex)
                {
                    case 0:
                        _currentEditLoopId = Guid.Empty;
                        OnDeleteLoop(item);
                        break;
                }
            };
            alertView.Show();
        }

        private void HandleOnPunchInLoop(Guid loopId, SSPLoopSegmentType segmentType)
        {
            OnPunchInLoopSegment(segmentType);
        }

        private void HandleOnChangeLoopName(Guid loopId, string newName)
        {
            OnChangeLoopName(loopId, newName);
        }

        private void HandleOnChangeLoopPosition(Guid loopId, SSPLoopSegmentType segmentType, float newPositionPercentage)
        {
            OnChangingLoopSegmentPosition(segmentType, newPositionPercentage);
        }

        private void HandleOnSetLoopPosition(Guid loopId, SSPLoopSegmentType segmentType, float newPositionPercentage)
        {
            OnChangedLoopSegmentPosition(segmentType, newPositionPercentage);
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

        public Action<Guid, string> OnChangeLoopName { get; set; }

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

        private void SetLoopCellIsPlaying(Guid loopId, bool isPlaying)
        {
            int index = _loops.FindIndex(x => x.LoopId == loopId);
            var cell = tableView.CellAt(NSIndexPath.FromRowSection(index, 0));
            if(cell != null)
            {
                var loopCell = cell as SessionsLoopTableViewCell;
                loopCell.SetLoopPlaying(isPlaying);
            }
        }

        public void RefreshPlayingLoop(SSPLoop loop, bool isPlaying)
        {
            InvokeOnMainThread(() => {
                SetLoopCellIsPlaying(_currentlyPlayingLoopId, false);
                _currentlyPlayingLoopId = loop != null ? loop.LoopId : Guid.Empty;
                SetLoopCellIsPlaying(_currentlyPlayingLoopId, isPlaying);
            });
        }

        public void RefreshCurrentlyEditedLoop(SSPLoop loop)
        {
            InvokeOnMainThread(() => {
//                Console.WriteLine("--------->>>>> RefreshCurrentlyPlayingLoop - start {0} end {1}", loop.StartPosition, loop.EndPosition);

                int index = _loops.FindIndex(x => x.LoopId == loop.LoopId);
                //Tracing.Log("MarkersViewController - RefreshMarkerPosition - markerId: {0} position: {1} index: {2} newIndex: {3}", marker.MarkerId, marker.Position, index, newIndex);

                // Update position
                var cell = tableView.CellAt(NSIndexPath.FromRowSection(index, 0));
                if(cell != null)
                {
                    var loopCell = cell as SessionsLoopTableViewCell;
                    loopCell.SetLoop(loop, false);
                }
            });
        }

        #endregion
    }
}
