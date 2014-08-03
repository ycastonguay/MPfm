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
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using Sessions.MVP.Messages;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using System.Linq;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Segment details view presenter.
	/// </summary>
    public class SegmentDetailsPresenter : BasePresenter<ISegmentDetailsView>, ISegmentDetailsPresenter
	{
        private long _lengthBytes;
        private Guid _segmentId;
        private Segment _segment;
        private AudioFile _audioFile;
        private List<Marker> _markers;
        private List<TinyMessageSubscriptionToken> _tokens = new List<TinyMessageSubscriptionToken>();
        private readonly ILibraryService _libraryService;
        private readonly IPlayerService _playerService;
        private readonly ITinyMessengerHub _messageHub;

        public SegmentDetailsPresenter(ITinyMessengerHub messageHub, ILibraryService libraryService, IPlayerService playerService)
        {
            _messageHub = messageHub;
            _libraryService = libraryService;
            _playerService = playerService;
        }

        public override void BindView(ISegmentDetailsView view)
        {            
            view.OnChangePositionSegmentDetails = ChangePosition;
            view.OnPunchInPositionSegmentDetails = PunchInPosition;
            view.OnUpdateSegmentDetails = UpdateSegmentDetails;
            view.OnLinkToMarkerSegmentDetails = LinkToMarker;
            base.BindView(view);

            _tokens.Add(_messageHub.Subscribe<SegmentBeingEditedMessage>(SegmentBeingEdited));
            _tokens.Add(_messageHub.Subscribe<MarkerUpdatedMessage>(MarkerUpdated));
        }

	    public override void ViewDestroyed()
	    {
            foreach (TinyMessageSubscriptionToken token in _tokens)
                token.Dispose();

	        base.ViewDestroyed();
	    }

	    private void SegmentBeingEdited(SegmentBeingEditedMessage message)
        {
            _segmentId = message.SegmentId;
            RefreshSegment();
        }

        private void MarkerUpdated(MarkerUpdatedMessage message)
        {
            RefreshMarkers();
        }

        private void ChangePosition(float position)
        {
            try
            {
                // Calculate new position from 0.0f/1.0f scale
                long positionBytes = (long)(position * _lengthBytes);
                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)_audioFile.BitsPerSample, _audioFile.AudioChannels);
                int positionMS = (int)ConvertAudio.ToMS(positionSamples, (uint)_audioFile.SampleRate);
                string positionString = Conversion.MillisecondsToTimeString((ulong)positionMS);

                // Update local segment and update view
                _segment.Position = positionString;
                _segment.PositionBytes = (uint)positionBytes;
                _segment.PositionSamples = (uint)positionSamples;
                float positionPercentage = (float)_segment.PositionBytes / (float)_lengthBytes;
                View.RefreshSegmentPosition(positionString, positionPercentage);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while calculating the segment start position: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }

        private void PunchInPosition()
        {
            try
            {
                var position = _playerService.GetPosition();
                _segment.Position = position.Position;
                _segment.PositionBytes = (uint)position.PositionBytes;
                _segment.PositionSamples = (uint)position.PositionSamples;
                View.RefreshSegmentPosition(position.Position, position.PositionPercentage);
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while punching in a segment: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }

        private void UpdateSegmentDetails(Segment segment)
        {
            try
            {
                // Copy everything except position
                _segment.MarkerId = segment.MarkerId;

                // Update segment and close view
                _libraryService.UpdateSegment(_segment);
                _messageHub.PublishAsync(new SegmentUpdatedMessage(this){ 
                    AudioFileId = _audioFile.Id,
                    SegmentId = _segment.SegmentId
                });
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while updating a segment: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }

        private void LinkToMarker(Guid markerId)
        {
            try
            {
                var marker = _markers.FirstOrDefault(x => x.MarkerId == markerId);
                if(marker == null)
                    return;

                _segment.MarkerId = markerId;
                _segment.Position = marker.Position;
                _segment.PositionBytes = (uint)marker.PositionBytes;
                _segment.PositionSamples = marker.PositionSamples;
                float positionPercentage = ((float)_segment.PositionBytes / (float)_lengthBytes) * 100;
                View.RefreshSegmentPosition(marker.Position, positionPercentage);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while linking a segment to a marker: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }

        private void RefreshSegment()
        {
            try
            {
                // Make a local copy of data in case the song changes
                _segment = _libraryService.SelectSegment(_segmentId);
                _lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
                _audioFile = _playerService.CurrentPlaylistItem.AudioFile;

                float positionPercentage = ((float)_segment.PositionBytes / (float)_lengthBytes) * 100;
                RefreshMarkers();
                View.RefreshSegmentDetails(_segment, _lengthBytes);
                View.RefreshSegmentPosition(_segment.Position, positionPercentage);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while refreshing a segment: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }

        private void RefreshMarkers()
        {
            try
            {
                if(_audioFile == null)
                    return;

                _markers = _libraryService.SelectMarkers(_audioFile.Id);
                View.RefreshSegmentMarkers(_markers);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while refreshing segment markers: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }
    }
}
