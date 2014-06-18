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
using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using Sessions.Core;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using MPfm.Player.Objects;
using MPfm.Library.Services.Interfaces;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Marker details view presenter.
	/// </summary>
	public class MarkerDetailsPresenter : BasePresenter<IMarkerDetailsView>, IMarkerDetailsPresenter
	{
        private Marker _marker;
        private AudioFile _audioFile;
        private long _lengthBytes;
        private Guid _markerId;
        private readonly ILibraryService _libraryService;
        private readonly IPlayerService _playerService;
        private readonly ITinyMessengerHub _messageHub;

        public MarkerDetailsPresenter(Guid markerId, ITinyMessengerHub messageHub, ILibraryService libraryService, IPlayerService playerService)
		{
            _markerId = markerId;
            _messageHub = messageHub;
            _libraryService = libraryService;
            _playerService = playerService;
		}

        public override void BindView(IMarkerDetailsView view)
        {            
            // Subscribe to view actions
            view.OnChangePositionMarkerDetails = ChangePosition;
            view.OnUpdateMarkerDetails = UpdateMarker;
            view.OnDeleteMarkerDetails = DeleteMarker;      
            view.OnPunchInMarkerDetails = PunchInMarker;
            base.BindView(view);

            _messageHub.Subscribe<MarkerBeingEditedMessage>(MarkerBeingEdited);
            RefreshMarker();
        }

        private void MarkerBeingEdited(MarkerBeingEditedMessage message)
        {
            _markerId = message.MarkerId;
            RefreshMarker();
        }

        private void ChangePosition(float newPositionPercentage)
        {
            try
            {
                // Calculate new position from 0.0f/1.0f scale
                long positionBytes = (long)(newPositionPercentage * _lengthBytes);
                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)_audioFile.BitsPerSample, _audioFile.AudioChannels);
                int positionMS = (int)ConvertAudio.ToMS(positionSamples, (uint)_audioFile.SampleRate);
                string positionString = Conversion.MillisecondsToTimeString((ulong)positionMS);

                // Update local marker and update view
                _marker.Position = positionString;
                _marker.PositionBytes = positionBytes;
                _marker.PositionSamples = (uint)positionSamples;
                float positionPercentage = (float)_marker.PositionBytes / (float)_lengthBytes;
                View.RefreshMarkerPosition(positionString, positionPercentage);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while calculating the marker position: " + ex.Message);
                View.MarkerDetailsError(ex);
            }
        }

        private void UpdateMarker(Marker marker)
        {
            try
            {
                // Validate marker name
                if(string.IsNullOrEmpty(marker.Name))
                {
                    View.MarkerDetailsError(new ArgumentNullException("The marker name must not be empty!"));
                    return;
                }

                // Copy everything except position
                _marker.Name = marker.Name;
                _marker.Comments = marker.Comments;

                // Update marker and close view
                _libraryService.UpdateMarker(_marker);
                _messageHub.PublishAsync(new MarkerUpdatedMessage(this){ 
                    AudioFileId = _audioFile.Id,
                    MarkerId = _markerId
                });
                View.DismissMarkerDetailsView();
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while updating a marker: " + ex.Message);
                View.MarkerDetailsError(ex);
            }
        }

        private void DeleteMarker()
        {
            try
            {
                _libraryService.DeleteMarker(_markerId);
                _messageHub.PublishAsync(new MarkerUpdatedMessage(this){ 
                    AudioFileId = _audioFile.Id,
                    MarkerId = _markerId
                });
                View.DismissMarkerDetailsView();
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while deleting a marker: " + ex.Message);
                View.MarkerDetailsError(ex);
            }
        }

        private void PunchInMarker()
        {
            try
            {
                var position = _playerService.GetPosition();
                _marker.Position = position.Position;
                _marker.PositionBytes = position.PositionBytes;
                _marker.PositionPercentage = position.PositionPercentage;
                _marker.PositionSamples = (uint)position.PositionSamples;
                View.RefreshMarkerPosition(position.Position, position.PositionPercentage);
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while punching in a marker: " + ex.Message);
                View.MarkerDetailsError(ex);
            }
        }

        private void RefreshMarker()
        {
            try
            {
                if(_markerId == Guid.Empty)
                    return;

                // Make a local copy of data in case the song changes
                _marker = _libraryService.SelectMarker(_markerId);
                _audioFile = _playerService.CurrentPlaylistItem.AudioFile;
                _lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
                float positionPercentage = ((float)_marker.PositionBytes / (float)_lengthBytes) * 100;
                View.RefreshMarker(_marker, _audioFile);
                View.RefreshMarkerPosition(_marker.Position, positionPercentage);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while refreshing a marker: " + ex.Message);
                View.MarkerDetailsError(ex);
            }
        }
    }
}

