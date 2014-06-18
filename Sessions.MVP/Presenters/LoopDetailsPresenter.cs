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

using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using MPfm.MVP.Messages;
using System;
using MPfm.MVP.Services.Interfaces;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Loop details view presenter.
	/// </summary>
	public class LoopDetailsPresenter : BasePresenter<ILoopDetailsView>, ILoopDetailsPresenter
	{
        Guid _loopId;
        Loop _loop;
        AudioFile _audioFile;
        long _lengthBytes;
        readonly ITinyMessengerHub _messageHub;
        readonly ILibraryService _libraryService;
        readonly IPlayerService _playerService;
        
        public LoopDetailsPresenter(ITinyMessengerHub messageHub, ILibraryService libraryService, IPlayerService playerService)
		{
            _messageHub = messageHub;
            _libraryService = libraryService;
            _playerService = playerService;
		}

        public override void BindView(ILoopDetailsView view)
        {            
            // Subscribe to view actions
            view.OnAddSegment = AddSegment;
            view.OnAddSegmentFromMarker = AddSegmentFromMarker;
            view.OnEditSegment = EditSegment;
            view.OnDeleteSegment = DeleteSegment;
            view.OnUpdateLoopDetails = UpdateLoopDetails;            
            view.OnChangeSegmentOrder = ChangeSegmentOrder;
            view.OnLinkSegmentToMarker = LinkSegmentToMarker;
            base.BindView(view);

            _messageHub.Subscribe<LoopBeingEditedMessage>(LoopBeingEdited);
            _messageHub.Subscribe<SegmentUpdatedMessage>(SegmentUpdated);
        }

        private void LoopBeingEdited(LoopBeingEditedMessage message)
        {
            _loopId = message.LoopId;
            RefreshLoop();
        }

        private void SegmentUpdated(SegmentUpdatedMessage message)
        {
            UpdateLoopSegmentsOrder(true);
            RefreshLoop();
        }

        private void SetSegmentIndexes()
        {
            for (int a = 0; a < _loop.Segments.Count; a++)
            {
                var segment = _loop.Segments[a];
                segment.Index = string.Format("{0}", a);
            }
        }

        private void RefreshLoopDetailsViewWithUpdatedIndexes()
        {
            SetSegmentIndexes();
            View.RefreshLoopDetails(_loop, _audioFile);
        }

        private void AddSegment()
        {
            try
            {
                var segment = new Segment();
                segment.LoopId = _loopId;
                _loop.Segments.Add(segment);
                _libraryService.InsertSegment(segment);
                _messageHub.PublishAsync(new LoopUpdatedMessage(this) { 
                    AudioFileId = _loop.AudioFileId,
                    LoopId = _loopId
                });
                UpdateLoopSegmentsOrder(true);
                _messageHub.PublishAsync(new SegmentBeingEditedMessage(this, segment.SegmentId));
                RefreshLoopDetailsViewWithUpdatedIndexes();
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while adding a segment: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void AddSegmentFromMarker(Guid markerId, int row)
        {
            try
            {
                // Get marker from database
                var marker = _libraryService.SelectMarker(markerId);
                if(marker == null)
                    throw new NullReferenceException("The marker could not be found in the database!");

                // Add segment
                var segment = new Segment();
                segment.LoopId = _loopId;
                segment.MarkerId = marker.MarkerId;
                segment.Position = marker.Position;
                segment.PositionBytes = (uint)marker.PositionBytes;
                segment.PositionSamples = marker.PositionSamples;

                _loop.Segments.Insert(row, segment);
                _libraryService.InsertSegment(segment);
                _messageHub.PublishAsync(new LoopUpdatedMessage(this) { 
                    AudioFileId = _loop.AudioFileId,
                    LoopId = _loopId
                });
                UpdateLoopSegmentsOrder(true);
                RefreshLoopDetailsViewWithUpdatedIndexes();
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while adding a segment from a marker: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void EditSegment(Segment segment)
        {
            try
            {
                _messageHub.PublishAsync(new SegmentBeingEditedMessage(this, segment.SegmentId));
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while editing a segment: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void DeleteSegment(Segment segment)
        {
            try
            {
                _loop.Segments.Remove(segment);
                _libraryService.DeleteSegment(segment.SegmentId);
                _messageHub.PublishAsync(new SegmentUpdatedMessage(this) { 
                    AudioFileId = _loop.AudioFileId,
                    LoopId = _loop.LoopId,
                    SegmentId = segment.SegmentId
                });
                RefreshLoopDetailsViewWithUpdatedIndexes();
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while deleting segment: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void ChangeSegmentOrder(Segment segment, int newIndex)
        {
            try
            {
                //Console.WriteLine("LoopDetailsPresenter - ChangeSegmentOrder - segmentPosition: {0} newIndex: {1}", segment.Position, newIndex);
                int index = Math.Min(newIndex, _loop.Segments.Count - 1);
                _loop.Segments.Remove(segment);
                _loop.Segments.Insert(index, segment);
                UpdateLoopSegmentsOrder(false);
                RefreshLoopDetailsViewWithUpdatedIndexes();
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while changing segment order: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void LinkSegmentToMarker(Segment segment, Guid markerId)
        {
            try
            {
                // Get marker from database
                var marker = _libraryService.SelectMarker(markerId);
                if(marker == null)
                    throw new NullReferenceException("The marker could not be found in the database!");

                // Update segment
                segment.MarkerId = markerId;
                segment.Position = marker.Position;
                segment.PositionBytes = (uint)marker.PositionBytes;
                segment.PositionSamples = marker.PositionSamples;
                float positionPercentage = ((float)segment.PositionBytes / (float)_lengthBytes) * 100;
                _libraryService.UpdateSegment(segment);

                RefreshLoop();
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while linking a segment to a marker: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void UpdateLoopSegmentsOrder(bool refreshLoopFromDatabase)
        {
            // We need to fetch the segments again from the database to make sure we don't undo changes from other presenters
            if(refreshLoopFromDatabase)
                _loop = _libraryService.SelectLoopIncludingSegments(_loopId);

            for (int i = 0; i < _loop.Segments.Count; i++)
            {
                var updateSegment = _loop.Segments[i];
                updateSegment.SegmentIndex = i;
                _libraryService.UpdateSegment(updateSegment);
            }
        }

        private void UpdateLoopDetails(Loop loop)
        {
            try
            {
                if(string.IsNullOrEmpty(loop.Name))
                {
                    View.LoopDetailsError(new ArgumentNullException("The loop name must not be empty!"));
                    return;
                }

                // Copy everything except position
                _loop.Name = loop.Name;

                // Update marker and close view
                _libraryService.UpdateLoop(_loop);
                _messageHub.PublishAsync(new LoopUpdatedMessage(this){ 
                    AudioFileId = _audioFile.Id,
                    LoopId = _loopId
                });
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while updating a loop: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void RefreshLoop()
        {
            try
            {
                if(_loopId == Guid.Empty)
                    return;

                // Make a local copy of data in case the song changes
                _loop = _libraryService.SelectLoopIncludingSegments(_loopId);
                _audioFile = _playerService.CurrentPlaylistItem.AudioFile;
                _lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
                //float positionPercentage = ((float)_rker.PositionBytes / (float)_lengthBytes) * 100;
                RefreshLoopDetailsViewWithUpdatedIndexes();                
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while refreshing a loop: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }
    }
}
