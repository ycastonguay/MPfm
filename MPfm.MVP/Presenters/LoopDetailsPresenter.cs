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
using MPfm.Library.Services.Interfaces;
using MPfm.Player.Objects;
using TinyMessenger;
using MPfm.MVP.Messages;
using System;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Services.Interfaces;
using MPfm.Core;

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
            view.OnEditSegment = EditSegment;
            view.OnDeleteSegment = DeleteSegment;
            view.OnUpdateLoopDetails = UpdateLoopDetails;            
            base.BindView(view);

            _messageHub.Subscribe<LoopBeingEditedMessage>(LoopBeingEdited);
        }

        private void LoopBeingEdited(LoopBeingEditedMessage message)
        {
            _loopId = message.LoopId;
            RefreshLoop();
        }

        private void AddSegment()
        {
            try
            {

            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while adding segment: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void EditSegment(Segment segment)
        {
            try
            {

            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while editing segment: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void DeleteSegment(Segment segment)
        {
            try
            {

            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while deleting segment: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }

        private void UpdateLoopDetails(Loop loop)
        {
        }

        private void RefreshLoop()
        {
            try
            {
                if(_loopId == Guid.Empty)
                    return;

                // Make a local copy of data in case the song changes
                _loop = _libraryService.SelectLoop(_loopId);
                _audioFile = _playerService.CurrentPlaylistItem.AudioFile;
                _lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
                //float positionPercentage = ((float)_rker.PositionBytes / (float)_lengthBytes) * 100;
                View.RefreshLoopDetails(_loop, _audioFile);
                //View.RefreshMarkerPosition(_marker.Position, positionPercentage);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while refreshing a loop: " + ex.Message);
                View.LoopDetailsError(ex);
            }
        }
    }
}
