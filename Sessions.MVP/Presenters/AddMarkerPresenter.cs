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
using System.Linq;
using Sessions.MVP.Messages;
using Sessions.MVP.Models;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using TinyMessenger;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Add Marker presenter.
	/// </summary>
    public class AddMarkerPresenter : BasePresenter<IAddMarkerView>, IAddMarkerPresenter
	{
	    private readonly ITinyMessengerHub _messengerHub;
	    private readonly ILibraryService _libraryService;
	    private readonly IPlayerService _playerService;

	    public AddMarkerPresenter(ITinyMessengerHub messengerHub, ILibraryService libraryService, IPlayerService playerService)
	    {
	        _messengerHub = messengerHub;
	        _libraryService = libraryService;
	        _playerService = playerService;
	    }

        public override void BindView(IAddMarkerView view)
        {
            view.OnAddMarker = OnAddMarker;

            base.BindView(view);
        }

	    private void OnAddMarker(MarkerTemplateNameType template)
	    {
            try
            {
                // Create marker name from template type (check for markers sharing the same name)
                var markers = _libraryService.SelectMarkers(_playerService.CurrentPlaylistItem.AudioFile.Id);
                List<string> similarMarkers = markers.Select(x => x.Name).Where(x => x.ToUpper().StartsWith(template.ToString().ToUpper())).ToList();
                string markerName = template.ToString() + " " + (similarMarkers.Count + 1).ToString();

                // Create marker and add to database
                Marker marker = new Marker();
                marker.Name = markerName;
                marker.PositionBytes = _playerService.GetPosition().PositionBytes;
                marker.PositionSamples = ConvertAudio.ToPCM(marker.PositionBytes, _playerService.CurrentPlaylistItem.AudioFile.BitsPerSample, 2);
                var ms = ConvertAudio.ToMS(marker.PositionSamples, _playerService.CurrentPlaylistItem.AudioFile.SampleRate);
                marker.Position = ConvertAudio.ToTimeString(ms);
                marker.AudioFileId = _playerService.CurrentPlaylistItem.AudioFile.Id;
                _libraryService.InsertMarker(marker);

                _messengerHub.PublishAsync(new MarkerUpdatedMessage(this)
                {
                    AudioFileId = marker.AudioFileId,
                    MarkerId = marker.MarkerId
                });
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while saving marker: " + ex.Message);
                View.AddMarkerError(ex);
            }
        }
	}
}
