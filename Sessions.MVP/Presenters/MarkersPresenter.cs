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
using System.Linq;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Messages;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sessions.MVP.Presenters
{
	/// <summary>
	/// Markers view presenter.
	/// </summary>
	public class MarkersPresenter : BasePresenter<IMarkersView>, IMarkersPresenter
	{
        readonly ITinyMessengerHub _messageHub;
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly NavigationManager _navigationManager;
        readonly ILibraryService _libraryService;
        readonly IPlayerService _playerService;
	    List<TinyMessageSubscriptionToken> _tokens = new List<TinyMessageSubscriptionToken>();
        Guid _audioFileId = Guid.Empty;
        List<Marker> _markers = new List<Marker>();

        public MarkersPresenter(ITinyMessengerHub messageHub, ILibraryService libraryService, IPlayerService playerService)
		{
            _messageHub = messageHub;
            _libraryService = libraryService;
            _playerService = playerService;

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
		}

        public override void BindView(IMarkersView view)
        {
            view.OnAddMarker = AddMarker;
            view.OnAddMarkerWithTemplate = AddMarkerWithTemplate;
            view.OnEditMarker = EditMarker;
            view.OnSelectMarker = SelectMarker;
            view.OnDeleteMarker = DeleteMarker;
            view.OnChangeMarkerName = ChangeMarkerName;
            view.OnChangeMarkerPosition = ChangeMarkerPosition;
            view.OnSetMarkerPosition = SetMarkerPosition;
            view.OnSetActiveMarker = SetActiveMarker;
            view.OnUpdateMarker = UpdateMarker;
            view.OnPunchInMarker = PunchInMarker;
            view.OnUndoMarker = UndoMarker;
            base.BindView(view);

            _tokens.Add(_messageHub.Subscribe<MarkerUpdatedMessage>((MarkerUpdatedMessage m) => {
                _audioFileId = m.AudioFileId;
                RefreshMarkers(_audioFileId);
            }));
            _tokens.Add(_messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>((PlayerPlaylistIndexChangedMessage m) => {
                _audioFileId = m.Data.AudioFileStarted.Id;
                RefreshMarkers(_audioFileId);
            }));

            // Refresh initial data
            if (_playerService.CurrentPlaylistItem != null)
                RefreshMarkers(_playerService.CurrentPlaylistItem.AudioFile.Id);
        }

	    public override void ViewDestroyed()
	    {
            foreach (TinyMessageSubscriptionToken token in _tokens)
                token.Dispose();

	        base.ViewDestroyed();
	    }

        private void SetMarkerIndexes()
        {
            for (int a = 0; a < _markers.Count; a++)
            {
                var marker = _markers[a];
                marker.Index = string.Format("{0}", Conversion.IndexToLetter(a));
            }
        }

        private void RefreshMarkersViewWithUpdatedIndexes()
        {
            SetMarkerIndexes();
            View.RefreshMarkers(_markers);
        }

	    private void AddMarker()
	    {
            try
            {
                var view = _mobileNavigationManager.CreateAddMarkerView();
                _mobileNavigationManager.PushDialogView(MobileDialogPresentationType.Standard, "Add Marker", View, view);
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while creating AddMarker view: " + ex.Message);
                View.MarkerError(ex);
            }
	    }

        private void AddMarkerWithTemplate(MarkerTemplateNameType markerTemplateNameType)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Create marker name from template type (check for markers sharing the same name)
                    List<string> similarMarkers = _markers.Select(x => x.Name).Where(x => x.ToUpper().StartsWith(markerTemplateNameType.ToString().ToUpper())).ToList();
                    string markerName = markerTemplateNameType.ToString() + " " + (similarMarkers.Count + 1).ToString();

                    // Create marker and add to database
                    Marker marker = new Marker();
                    marker.Name = markerName;
                    marker.PositionBytes = _playerService.GetPosition().PositionBytes;
                    marker.PositionSamples = ConvertAudio.ToPCM(marker.PositionBytes, _playerService.CurrentPlaylistItem.AudioFile.BitsPerSample, 2);
                    long ms = ConvertAudio.ToMS(marker.PositionSamples, _playerService.CurrentPlaylistItem.AudioFile.SampleRate);
                    marker.Position = ConvertAudio.ToTimeString(ms);
                    marker.AudioFileId = _playerService.CurrentPlaylistItem.AudioFile.Id;
                    marker.PositionPercentage = (float)marker.PositionBytes / (float)_playerService.CurrentPlaylistItem.LengthBytes;
                    _libraryService.InsertMarker(marker);

                    _messageHub.PublishAsync(new MarkerUpdatedMessage(this) { 
                        AudioFileId = marker.AudioFileId,
                        MarkerId = marker.MarkerId
                    });
                    //RefreshMarkers(_playerService.CurrentPlaylistItem.AudioFile.Id);
                } 
                catch (Exception ex)
                {
                    Tracing.Log("An error occured while adding a marker: " + ex.Message);
                    View.MarkerError(ex);
                }
            });
        }

        private void EditMarker(Marker marker)
        {
            //CreateMarkerDetailsView(marker.MarkerId);
            _messageHub.PublishAsync<MarkerBeingEditedMessage>(new MarkerBeingEditedMessage(this, marker.MarkerId));
        }

        private void SelectMarker(Marker marker)
        {
            try
            {
                _playerService.GoToMarker(marker);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while selecting marker: " + ex.Message);
                View.MarkerError(ex);
            }
        }

        private void DeleteMarker(Marker marker)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _markers.Remove(marker);
                    _libraryService.DeleteMarker(marker.MarkerId);
                    _messageHub.PublishAsync(new MarkerUpdatedMessage(this) { 
                        AudioFileId = marker.AudioFileId,
                        MarkerId = marker.MarkerId
                    });
                    RefreshMarkersViewWithUpdatedIndexes();
                } 
                catch (Exception ex)
                {
                    Tracing.Log("An error occured while deleting marker: " + ex.Message);
                    View.MarkerError(ex);
                }
            });
        }

        private void RefreshMarkers(Guid audioFileId)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _markers = _libraryService.SelectMarkers(audioFileId);

                    foreach (var marker in _markers)
                        marker.PositionPercentage = (float)marker.PositionBytes / (float)_playerService.CurrentPlaylistItem.LengthBytes;

                    RefreshMarkersViewWithUpdatedIndexes();
                } 
                catch (Exception ex)
                {
                    Tracing.Log("An error occured while refreshing markera: " + ex.Message);
                    View.MarkerError(ex);
                }
            });
        }

        private void SetActiveMarker(Guid markerId)
        {
            _messageHub.PublishAsync<MarkerActivatedMessage>(new MarkerActivatedMessage(this, markerId));
        }

        private void ChangeMarkerName(Guid markerId, string name)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var marker = _markers.FirstOrDefault(x => x.MarkerId == markerId);
                    marker.Name = name;
                    UpdateMarker(marker);
                } 
                catch (Exception ex)
                {
                    Tracing.Log("An error occured while change the marker name: " + ex.Message);
                    View.MarkerError(ex);
                }
            });
        }

        private void ChangeMarkerPosition(Guid markerId, float newPositionPercentage)
        {
            var lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
            long positionBytes = (long)(newPositionPercentage * lengthBytes);
            ChangeMarkerPosition(markerId, positionBytes);
        }

        private void ChangeMarkerPosition(Guid markerId, long positionBytes)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    //Tracing.Log("MarkersPresenter - ChangeMarkerPosition - markerId: {0} positionBytes: {1}", markerId, positionBytes);
                    // Instead of using FirstOrDefault, search in a simple loop so we don't have an exception that the list is modified
                    Marker marker = null;
                    for(int a = 0; a < _markers.Count; a++)
                    {
                        if(_markers[a].MarkerId == markerId)
                        {
                            marker = _markers[a];
                            break;
                        }
                    }

                    if(marker == null)
                        return;

                    var audioFile = _playerService.CurrentPlaylistItem.AudioFile;
                    var lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;

                    // Calculate new position from 0.0f/1.0f scale
                    long positionSamples = ConvertAudio.ToPCM(positionBytes, audioFile.BitsPerSample, audioFile.AudioChannels);
                    long positionMS = ConvertAudio.ToMS(positionSamples, audioFile.SampleRate);
                    string positionString = ConvertAudio.ToTimeString(positionMS);
                    float positionPercentage = (float)marker.PositionBytes / (float)lengthBytes;

                    // Update marker and update view
                    marker.Position = positionString;
                    marker.PositionBytes = positionBytes;
                    marker.PositionSamples = positionSamples;
                    marker.PositionPercentage = positionPercentage;
                    _messageHub.PublishAsync<MarkerPositionUpdatedMessage>(new MarkerPositionUpdatedMessage(this, marker));

                    // Check new index
                    int index = _markers.OrderBy(x => x.PositionBytes).ToList().FindIndex(x => x.MarkerId == markerId);
                    View.RefreshMarkerPosition(marker, index);
                } 
                catch (Exception ex)
                {
                    Tracing.Log("An error occured while calculating the marker position: " + ex.Message);
                    View.MarkerError(ex);
                }
            });
        }

        private void SetMarkerPosition(Guid markerId, float newPositionPercentage)
        {
            ChangeMarkerPosition(markerId, newPositionPercentage);
            var marker = _markers.FirstOrDefault(x => x.MarkerId == markerId);
            //Tracing.Log("MarkersPresenter - SetMarkerPosition - markerId: {0} position: {1}", markerId, marker.Position);
            UpdateMarker(marker);
        }

        private void UpdateMarker(Marker marker)
        {
            Task.Factory.StartNew(() =>
            {            
                try
                {
                        //Tracing.Log("MarkersPresenter - UpdateMarker - markerId: {0}", marker.MarkerId);
                    if (string.IsNullOrEmpty(marker.Name))
                    {
                        View.MarkerError(new ArgumentNullException("The marker name must not be empty!"));
                        return;
                    }

                    // Update marker and close view
                    _libraryService.UpdateMarker(marker);
                    _messageHub.PublishAsync(new MarkerUpdatedMessage(this) { 
                        AudioFileId = marker.AudioFileId,
                        MarkerId = marker.MarkerId
                    });
                } 
                catch (Exception ex)
                {
                    Tracing.Log("An error occured while updating a marker: " + ex.Message);
                    View.MarkerError(ex);
                }
            });
        }

        private void PunchInMarker(Guid markerId)
        {
//            Task.Factory.StartNew(() =>
//            {
                try
                {
                //Tracing.Log("MarkersPresenter - PunchInMarker - markerId: {0}", markerId);
                    var position = _playerService.GetPosition();
                    ChangeMarkerPosition(markerId, position.PositionBytes);
                    var marker = _markers.FirstOrDefault(x => x.MarkerId == markerId);
                    UpdateMarker(marker);
                } 
                catch (Exception ex)
                {
                    Tracing.Log("An error occured while punching in a marker: " + ex.Message);
                    View.MarkerError(ex);
                }
//            });
        }

        private void UndoMarker(Guid markerId)
        {
            try
            {
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while undoing a marker: " + ex.Message);
                View.MarkerError(ex);
            }
        }
    }

    public enum MarkerTemplateNameType
    {
        Verse = 0,
        Chorus = 1,
        Bridge = 2,
        Solo = 3,
        Other = 4,
        None = 5
    }
}

