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
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Marker details view presenter.
	/// </summary>
	public class MarkerDetailsPresenter : BasePresenter<IMarkerDetailsView>, IMarkerDetailsPresenter
	{
        private readonly Guid _markerId;
        private readonly ILibraryService _libraryService;
        private readonly ITinyMessengerHub _messageHub;

        public MarkerDetailsPresenter(Guid markerId, ITinyMessengerHub messageHub, ILibraryService libraryService)
		{
            _markerId = markerId;
            _messageHub = messageHub;
            _libraryService = libraryService;
		}

        public override void BindView(IMarkerDetailsView view)
        {            
            // Subscribe to view actions
            view.OnDeleteMarker = DeleteMarker;           

            base.BindView(view);
        }

        public void DeleteMarker()
        {
            try
            {
                _libraryService.DeleteMarker(_markerId);
                _messageHub.PublishAsync(new MarkerDeletedMessage(this){ 
                    MarkerId = _markerId
                });
                View.DismissView();
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while deleting a marker: " + ex.Message);
                View.MarkerDetailsError(ex);
            }
        }

        private void RefreshMarker()
        {
            try
            {
                var marker = _libraryService.SelectMarker(_markerId);
                View.RefreshMarker(marker);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while refreshing a marker: " + ex.Message);
                View.MarkerDetailsError(ex);
            }
        }
    }
}

