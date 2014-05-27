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

using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Library.Services.Interfaces;
using TinyMessenger;
using MPfm.MVP.Services.Interfaces;
using MPfm.Core;
using System;
using MPfm.MVP.Messages;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Presenters
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
            view.OnChangeStartPositionSegmentDetails = ChangeStartPosition;
            view.OnChangeEndPositionSegmentDetails = ChangeEndPosition;
            view.OnPunchInStartPositionSegmentDetails = PunchInStartPosition;
            view.OnPunchInEndPositionSegmentDetails = PunchInEndPosition;
            view.OnUpdateSegmentDetails = UpdateSegmentDetails;
            base.BindView(view);

            _messageHub.Subscribe<SegmentBeingEditedMessage>(SegmentBeingEdited);
        }

        private void SegmentBeingEdited(SegmentBeingEditedMessage message)
        {
            _segmentId = message.SegmentId;
            RefreshSegment();
        }

        private void ChangeStartPosition(float position)
        {
            try
            {
                // Calculate new position from 0.0f/1.0f scale
                long positionBytes = (long)(position * _lengthBytes);
                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)_audioFile.BitsPerSample, _audioFile.AudioChannels);
                int positionMS = (int)ConvertAudio.ToMS(positionSamples, (uint)_audioFile.SampleRate);
                string positionString = Conversion.MillisecondsToTimeString((ulong)positionMS);

                // Update local segment and update view
                _segment.StartPosition = positionString;
                _segment.StartPositionBytes = (uint)positionBytes;
                _segment.StartPositionSamples = (uint)positionSamples;
                float positionPercentage = (float)_segment.StartPositionBytes / (float)_lengthBytes;
                View.RefreshSegmentStartPosition(positionString, positionPercentage);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while calculating the segment start position: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }

        private void ChangeEndPosition(float position)
        {
            try
            {
                // Calculate new position from 0.0f/1.0f scale
                long positionBytes = (long)(position * _lengthBytes);
                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)_audioFile.BitsPerSample, _audioFile.AudioChannels);
                int positionMS = (int)ConvertAudio.ToMS(positionSamples, (uint)_audioFile.SampleRate);
                string positionString = Conversion.MillisecondsToTimeString((ulong)positionMS);

                // Update local segment and update view
                _segment.EndPosition = positionString;
                _segment.EndPositionBytes = (uint)positionBytes;
                _segment.EndPositionSamples = (uint)positionSamples;
                float positionPercentage = (float)_segment.EndPositionBytes / (float)_lengthBytes;
                View.RefreshSegmentEndPosition(positionString, positionPercentage);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while calculating the segment end position: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }

        private void PunchInStartPosition()
        {
            try
            {
                var position = _playerService.GetPosition();
                _segment.StartPosition = position.Position;
                _segment.StartPositionBytes = (uint)position.PositionBytes;
                //_segment.StartPositionPercentage = position.PositionPercentage;
                _segment.StartPositionSamples = (uint)position.PositionSamples;
                View.RefreshSegmentStartPosition(position.Position, position.PositionPercentage);
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while punching in a segment: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }

        private void PunchInEndPosition()
        {
            try
            {
                var position = _playerService.GetPosition();
                _segment.EndPosition = position.Position;
                _segment.EndPositionBytes = (uint)position.PositionBytes;
                //_segment.EndPositionPercentage = position.PositionPercentage;
                _segment.EndPositionSamples = (uint)position.PositionSamples;
                View.RefreshSegmentEndPosition(position.Position, position.PositionPercentage);
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
                // Validate that the start position is lower than the end position
                if(segment.StartPositionBytes > segment.EndPositionBytes)
                {
                    View.SegmentDetailsError(new Exception("The start position cannot be after the end position!"));
                    return;
                }

                // Copy everything except position
                _segment.StartPositionMarkerId = segment.StartPositionMarkerId;
                _segment.EndPositionMarkerId = segment.EndPositionMarkerId;

                // Update segment and close view
                _libraryService.UpdateSegment(_segment);
                _messageHub.PublishAsync(new SegmentUpdatedMessage(this){ 
                    AudioFileId = _audioFile.Id,
                    SegmentId = _segment.SegmentId
                });
                //View.DismissMarkerDetailsView();
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while updating a segment: " + ex.Message);
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

                float startPositionPercentage = ((float)_segment.StartPositionBytes / (float)_lengthBytes) * 100;
                float endPositionPercentage = ((float)_segment.EndPositionBytes / (float)_lengthBytes) * 100;
                View.RefreshSegmentDetails(_segment, _lengthBytes);
                View.RefreshSegmentStartPosition(_segment.StartPosition, startPositionPercentage);
                View.RefreshSegmentEndPosition(_segment.EndPosition, endPositionPercentage);

                var markers = _libraryService.SelectMarkers(_audioFile.Id);
                View.RefreshSegmentMarkers(markers);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while refreshing a segment: " + ex.Message);
                View.SegmentDetailsError(ex);
            }
        }
    }
}
