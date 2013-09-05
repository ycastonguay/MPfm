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
using System.Linq;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;
using System.Collections.Generic;
using MPfm.Library.Services.Interfaces;

namespace MPfm.MVP.Presenters
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
            view.OnEditMarker = EditMarker;
            view.OnSelectMarker = SelectMarker;
            view.OnDeleteMarker = DeleteMarker;

            // This will bind the view; data can be refreshed afterwards
            base.BindView(view);

            // Subscribe to messages
            _tokens.Add(_messageHub.Subscribe<MarkerUpdatedMessage>((MarkerUpdatedMessage m) => {
                Console.WriteLine("MarkersPresenter - Received MarkerdUpdatedMessage - audioFileId: {0}", m.AudioFileId.ToString());
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

	    private void CreateMarkerDetailsView(Guid markerId)
        {
#if IOS || ANDROID
            _mobileNavigationManager.CreateMarkerDetailsView(View, markerId);
#else
	        _navigationManager.CreateMarkerDetailsView(markerId);
#endif
        }

        private void AddMarker(MarkerTemplateNameType markerTemplateNameType)
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
                marker.PositionSamples = (uint)ConvertAudio.ToPCM(marker.PositionBytes, (uint)_playerService.CurrentPlaylistItem.AudioFile.BitsPerSample, 2);
                int ms = (int)ConvertAudio.ToMS(marker.PositionSamples, (uint)_playerService.CurrentPlaylistItem.AudioFile.SampleRate);
                marker.Position = Conversion.MillisecondsToTimeString((ulong)ms);
                marker.AudioFileId = _playerService.CurrentPlaylistItem.AudioFile.Id;
                _libraryService.InsertMarker(marker);

                _messageHub.PublishAsync(new MarkerUpdatedMessage(this){ 
                    AudioFileId = marker.AudioFileId,
                    MarkerId = marker.MarkerId
                });
                //RefreshMarkers(_playerService.CurrentPlaylistItem.AudioFile.Id);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while adding a marker: " + ex.Message);
                View.MarkerError(ex);
            }
        }

        private void EditMarker(Marker marker)
        {
            CreateMarkerDetailsView(marker.MarkerId);
        }

        private void SelectMarker(Marker marker)
        {
            try
            {
                _playerService.GoToMarker(marker);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while selecting marker: " + ex.Message);
                View.MarkerError(ex);
            }
        }

        private void DeleteMarker(Marker marker)
        {
            try
            {
                _markers.Remove(marker);
                _libraryService.DeleteMarker(marker.MarkerId);
                View.RefreshMarkers(_markers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while deleting marker: " + ex.Message);
                View.MarkerError(ex);
            }
        }

        private void RefreshMarkers(Guid audioFileId)
        {
            try
            {
                _markers = _libraryService.SelectMarkers(audioFileId);
                View.RefreshMarkers(_markers);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while refreshing markera: " + ex.Message);
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

