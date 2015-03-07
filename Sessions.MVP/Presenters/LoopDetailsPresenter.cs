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

using System.Collections.Generic;
using System.Linq;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using Sessions.MVP.Messages;
using System;
using Sessions.MVP.Services.Interfaces;
using org.sessionsapp.player;
using Sessions.Sound.Objects;

namespace Sessions.MVP.Presenters
{
	/// <summary>
	/// Loop details view presenter.
	/// </summary>
	public class LoopDetailsPresenter : BasePresenter<ILoopDetailsView>, ILoopDetailsPresenter
	{
        private Guid _loopId;
        private SSPLoop _loop;
        private AudioFile _audioFile;
        private List<TinyMessageSubscriptionToken> _tokens = new List<TinyMessageSubscriptionToken>();
        private readonly ITinyMessengerHub _messageHub;
        private readonly ILibraryService _libraryService;
        private readonly IPlayerService _playerService;
	    private List<Marker> _markers;

	    public LoopDetailsPresenter(ITinyMessengerHub messageHub, ILibraryService libraryService, IPlayerService playerService)
		{
            _messageHub = messageHub;
            _libraryService = libraryService;
            _playerService = playerService;
		}

        public override void BindView(ILoopDetailsView view)
        {            
//            view.OnAddSegment = AddSegment;
//            view.OnAddSegmentFromMarker = AddSegmentFromMarker;
//            view.OnEditSegment = EditSegment;
//            view.OnDeleteSegment = DeleteSegment;
//            view.OnUpdateLoopDetails = UpdateLoopDetails;            
//            view.OnLinkSegmentToMarker = LinkSegmentToMarker;
//            view.OnChangeSegmentOrder = ChangeSegmentOrder;
//            view.OnChangingSegmentPosition = ChangingSegmentPosition;
//            view.OnChangedSegmentPosition = ChangedSegmentPosition;
//            view.OnPunchInSegment = PunchInSegment;
            base.BindView(view);

            _tokens.Add(_messageHub.Subscribe<LoopBeingEditedMessage>(LoopBeingEdited));            
            _tokens.Add(_messageHub.Subscribe<SegmentUpdatedMessage>(SegmentUpdated));
            _tokens.Add(_messageHub.Subscribe<MarkerUpdatedMessage>(MarkerUpdated));
        }

	    public override void ViewDestroyed()
        {
            foreach (TinyMessageSubscriptionToken token in _tokens)
                token.Dispose();

            base.ViewDestroyed();
        }

        private void LoopBeingEdited(LoopBeingEditedMessage message)
        {
            _loopId = message.LoopId;
//            RefreshLoop();
        }

        private void SegmentUpdated(SegmentUpdatedMessage message)
        {
//            UpdateLoopSegmentsOrder(true);
//            RefreshLoop();
        }

        private void MarkerUpdated(MarkerUpdatedMessage message)
        {
//            RefreshMarkers();
        }
//
//        private void SetSegmentIndexes()
//        {
//            for (int a = 0; a < _loop.Segments.Count; a++)
//            {
//                var segment = _loop.Segments[a];
//                //segment.Index = string.Format("{0}", a);
//                segment.Index = a == 0 ? "Start" : "End";
//            }
//        }
//
//	    private void SetSegmentMarkers()
//	    {
//	        var markers = _libraryService.SelectMarkers(_audioFile.Id);
//	        foreach (var segment in _loop.Segments)
//	        {
//	            var marker = markers.FirstOrDefault(x => x.MarkerId == segment.MarkerId);
//	            if (marker != null)
//	                segment.Marker = marker.Name;
//	        }
//	    }
//
//        private void RefreshLoopDetailsViewWithUpdatedMetadata()
//        {
//            SetSegmentIndexes();
//            SetSegmentMarkers();
//            View.RefreshLoopDetails(_loop, _audioFile, _audioFile.LengthBytes);
//        }
//
//        private void AddSegment()
//        {
//            try
//            {
//                var segment = new Segment();
//                segment.LoopId = _loopId;
//                _loop.Segments.Add(segment);
//                _libraryService.InsertSegment(segment);
//                _messageHub.PublishAsync(new LoopUpdatedMessage(this) { 
//                    AudioFileId = _loop.AudioFileId,
//                    LoopId = _loopId
//                });
//                UpdateLoopSegmentsOrder(true);
//                _messageHub.PublishAsync(new SegmentBeingEditedMessage(this, segment.SegmentId));
//                RefreshLoopDetailsViewWithUpdatedMetadata();
//            } 
//            catch (Exception ex)
//            {
//                Tracing.Log("An error occured while adding a segment: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//        private void AddSegmentFromMarker(Guid markerId, int row)
//        {
//            try
//            {
//                // Get marker from database
//                var marker = _libraryService.SelectMarker(markerId);
//                if(marker == null)
//                    throw new NullReferenceException("The marker could not be found in the database!");
//
//                // Add segment
//                var segment = new Segment();
//                segment.LoopId = _loopId;
//                segment.MarkerId = marker.MarkerId;
//                segment.Position = marker.Position;
//                segment.PositionBytes = marker.PositionBytes;
//                segment.PositionSamples = marker.PositionSamples;
//
//                _loop.Segments.Insert(row, segment);
//                _libraryService.InsertSegment(segment);
//                _messageHub.PublishAsync(new LoopUpdatedMessage(this) { 
//                    AudioFileId = _loop.AudioFileId,
//                    LoopId = _loopId
//                });
//                UpdateLoopSegmentsOrder(true);
//                RefreshLoopDetailsViewWithUpdatedMetadata();
//            } 
//            catch (Exception ex)
//            {
//                Tracing.Log("An error occured while adding a segment from a marker: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//        private void EditSegment(Segment segment)
//        {
//            try
//            {
//                _messageHub.PublishAsync(new SegmentBeingEditedMessage(this, segment.SegmentId));
//            } 
//            catch (Exception ex)
//            {
//                Tracing.Log("An error occured while editing a segment: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//        private void DeleteSegment(Segment segment)
//        {
//            try
//            {
//                _loop.Segments.Remove(segment);
//                _libraryService.DeleteSegment(segment.SegmentId);
//                _messageHub.PublishAsync(new SegmentUpdatedMessage(this) { 
//                    AudioFileId = _loop.AudioFileId,
//                    LoopId = _loop.LoopId,
//                    SegmentId = segment.SegmentId
//                });
//                RefreshLoopDetailsViewWithUpdatedMetadata();
//            } 
//            catch (Exception ex)
//            {
//                Tracing.Log("An error occured while deleting segment: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//        private void ChangeSegmentOrder(Segment segment, int newIndex)
//        {
//            try
//            {
//                //Console.WriteLine("LoopDetailsPresenter - ChangeSegmentOrder - segmentPosition: {0} newIndex: {1}", segment.Position, newIndex);
//                int index = Math.Min(newIndex, _loop.Segments.Count - 1);
//                _loop.Segments.Remove(segment);
//                _loop.Segments.Insert(index, segment);
//                UpdateLoopSegmentsOrder(false);
//                RefreshLoopDetailsViewWithUpdatedMetadata();
//            } 
//            catch (Exception ex)
//            {
//                Tracing.Log("An error occured while changing segment order: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//	    private void ChangedSegmentPosition(Segment segment, float positionPercentage)
//	    {
//	        ChangeSegmentPosition(segment, positionPercentage, true);
//	    }
//
//	    private void ChangingSegmentPosition(Segment segment, float positionPercentage)
//	    {
//            ChangeSegmentPosition(segment, positionPercentage, false);
//	    }
//
//        private void ChangeSegmentPosition(Segment segment, float positionPercentage, bool updateDatabase)
//        {
//            try
//            {
//                var startSegment = _loop.GetStartSegment();
//                var endSegment = _loop.GetEndSegment();
//
//                // Make sure the loop length doesn't get below 0
//                if (segment == startSegment && positionPercentage > ((float)endSegment.PositionBytes / (float)_audioFile.LengthBytes))
//                {
//                    positionPercentage = (float)endSegment.PositionBytes / (float)_audioFile.LengthBytes;
//                }
//                else if (segment == endSegment && positionPercentage < ((float)startSegment.PositionBytes / (float)_audioFile.LengthBytes))
//                {
//                    positionPercentage = (float)startSegment.PositionBytes / (float)_audioFile.LengthBytes;
//                }
//
//                segment.SetPositionFromPercentage(positionPercentage, _audioFile.LengthBytes, _audioFile);
//
//                if (updateDatabase)
//                {
//                    _libraryService.UpdateSegment(segment);
//                    _messageHub.PublishAsync(new LoopUpdatedMessage(this)
//                    {
//                        AudioFileId = _audioFile.Id,
//                        LoopId = _loopId
//                    });
//                }
//
//                View.RefreshLoopDetailsSegment(segment, _audioFile.LengthBytes);
//            }
//            catch (Exception ex)
//            {
//                Tracing.Log("An error occured while calculating the segment position: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//        private void LinkSegmentToMarker(Segment segment, Guid markerId)
//        {
//            try
//            {
//                var marker = _libraryService.SelectMarker(markerId);
//                segment.MarkerId = markerId;
//
//                if (marker != null)
//                {
//                    segment.Position = marker.Position;
//                    segment.PositionBytes = marker.PositionBytes;
//                    segment.PositionSamples = marker.PositionSamples;
//                }
//                _libraryService.UpdateSegment(segment);
//                View.RefreshLoopDetailsSegment(segment, _audioFile.LengthBytes);
//            }
//            catch(Exception ex)
//            {
//                Tracing.Log("An error occured while linking a segment to a marker: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//        private void PunchInSegment(Segment segment)
//        {
//            try
//            {
//                var position = _playerService.GetPosition();
//                float positionPercentage = (float)position.Bytes / (float)_audioFile.LengthBytes;
//                ChangeSegmentPosition(segment, positionPercentage, true);
//            }
//            catch (Exception ex)
//            {
//                Tracing.Log("An error occured while punching in a segment: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//        private void UpdateLoopSegmentsOrder(bool refreshLoopFromDatabase)
//        {
//            // We need to fetch the segments again from the database to make sure we don't undo changes from other presenters
//            if(refreshLoopFromDatabase)
//                _loop = _libraryService.SelectLoopIncludingSegments(_loopId);
//
//            for (int i = 0; i < _loop.Segments.Count; i++)
//            {
//                var updateSegment = _loop.Segments[i];
//                updateSegment.SegmentIndex = i;
//                _libraryService.UpdateSegment(updateSegment);
//            }
//        }
//
//        private void UpdateLoopDetails(Loop loop)
//        {
//            try
//            {
//                if(string.IsNullOrEmpty(loop.Name))
//                {
//                    View.LoopDetailsError(new ArgumentNullException("The loop name must not be empty!"));
//                    return;
//                }
//
//                // Copy everything except position
//                _loop.Name = loop.Name;
//
//                // Update marker and close view
//                _libraryService.UpdateLoop(_loop);
//                _messageHub.PublishAsync(new LoopUpdatedMessage(this){ 
//                    AudioFileId = _audioFile.Id,
//                    LoopId = _loopId
//                });
//            }
//            catch(Exception ex)
//            {
//                Tracing.Log("An error occured while updating a loop: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//        private void RefreshLoop()
//        {
//            try
//            {
//                if(_loopId == Guid.Empty)
//                    return;
//
//                // Make a local copy of data in case the song changes
//                _loop = _libraryService.SelectLoopIncludingSegments(_loopId);
//                _audioFile = _playerService.CurrentAudioFile;
//                //float positionPercentage = ((float)_rker.PositionBytes / (float)_lengthBytes) * 100;
//                RefreshLoopDetailsViewWithUpdatedMetadata();
//                RefreshMarkers();
//            }
//            catch(Exception ex)
//            {
//                Tracing.Log("An error occured while refreshing a loop: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
//
//        private void RefreshMarkers()
//        {
//            try
//            {
//                if (_audioFile == null)
//                    return;
//
//                _markers = _libraryService.SelectMarkers(_audioFile.Id);
//                View.RefreshLoopMarkers(_markers);
//            }
//            catch (Exception ex)
//            {
//                Tracing.Log("An error occured while refreshing loop markers: " + ex.Message);
//                View.LoopDetailsError(ex);
//            }
//        }
    }
}
