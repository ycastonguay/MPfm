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

using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using Sessions.MVP.Services.Interfaces;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Player.Objects;
using TinyMessenger;
using Sessions.MVP.Messages;
using System.Collections.Generic;
using System;
using Sessions.Sound.AudioFiles;

namespace Sessions.MVP.Presenters
{
	/// <summary>
	/// Loops view presenter.
	/// </summary>
	public class LoopsPresenter : BasePresenter<ILoopsView>, ILoopsPresenter
	{
        private readonly ITinyMessengerHub _messageHub;
        private readonly ILibraryService _libraryService;
        private readonly IPlayerService _playerService;
        private readonly MobileNavigationManager _mobileNavigationManager;
        private readonly NavigationManager _navigationManager;
        private List<TinyMessageSubscriptionToken> _tokens = new List<TinyMessageSubscriptionToken>();
        private List<Loop> _loops = new List<Loop>();
        private Loop _loop;
        private Guid _audioFileId;
        private AudioFile _audioFile;
        private long _lengthBytes;
        
        public LoopsPresenter(ITinyMessengerHub messageHub, ILibraryService libraryService, IPlayerService playerService)
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

        public override void BindView(ILoopsView view)
        {            
            view.OnAddLoop = AddLoop;
            view.OnEditLoop = EditLoop;
            view.OnSelectLoop = SelectLoop;
            view.OnDeleteLoop = DeleteLoop;
            view.OnPlayLoop = PlayLoop;
            view.OnUpdateLoop = UpdateLoop;

            view.OnPunchInLoopSegment = PunchInLoopSegment;
            view.OnChangingLoopSegmentPosition = ChangingLoopSegmentPosition;
            view.OnChangedLoopSegmentPosition = ChangedLoopSegmentPosition;

            base.BindView(view);
            
            // Subscribe to messages
            _tokens.Add(_messageHub.Subscribe<LoopUpdatedMessage>((LoopUpdatedMessage m) => {
                _audioFileId = m.AudioFileId;
                RefreshLoops(_audioFileId);
            }));
            _tokens.Add(_messageHub.Subscribe<SegmentUpdatedMessage>((SegmentUpdatedMessage m) =>
            {
                _audioFileId = m.AudioFileId;
                RefreshLoops(_audioFileId);
            }));
            _tokens.Add(_messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>((PlayerPlaylistIndexChangedMessage m) => {
                _audioFileId = m.Data.AudioFileStarted.Id;
                RefreshLoops(_audioFileId);
            }));

            // Refresh initial data
            if (_playerService.CurrentPlaylistItem != null)
                RefreshLoops(_playerService.CurrentPlaylistItem.AudioFile.Id);
        }
        
        public override void ViewDestroyed()
        {
            foreach (TinyMessageSubscriptionToken token in _tokens)
                token.Dispose();

            base.ViewDestroyed();
        }

	    private void SetLoopIndexes()
	    {
	        for (int a = 0; a < _loops.Count; a++)
	        {
	            var loop = _loops[a];
	            loop.Index = string.Format("{0}", Conversion.IndexToLetter(a));
	        }
	    }

	    private void RefreshLoopsViewWithUpdatedIndexes()
	    {
            SetLoopIndexes();
	        View.RefreshLoops(_loops);
	    }

	    private void RefreshLoops(Guid audioFileId)
        {
            try
            {
                _loops = _libraryService.SelectLoopsIncludingSegments(audioFileId);
                _audioFile = _playerService.CurrentPlaylistItem.AudioFile;
                _lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
                RefreshLoopsViewWithUpdatedIndexes();
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while refreshing loops: " + ex.Message);
                View.LoopError(ex);
            }
        }

        private void AddLoop()
        {
            try
            {
                var loop = new Loop();
                loop.AudioFileId = _playerService.CurrentPlaylistItem.AudioFile.Id;
                loop.Name = "New Loop";
                loop.CreateStartEndSegments();

                var startSegment = loop.GetStartSegment();
                startSegment.SetPositionFromPercentage(0.1f, _playerService.CurrentPlaylistItem.LengthBytes, _playerService.CurrentPlaylistItem.AudioFile);

                var endSegment = loop.GetEndSegment();
                endSegment.SetPositionFromPercentage(0.9f, _playerService.CurrentPlaylistItem.LengthBytes, _playerService.CurrentPlaylistItem.AudioFile);

                _libraryService.InsertLoop(loop);
                _libraryService.InsertSegment(loop.Segments[0]);
                _libraryService.InsertSegment(loop.Segments[1]);
                _messageHub.PublishAsync(new LoopUpdatedMessage(this) { 
                    AudioFileId = loop.AudioFileId,
                    LoopId = loop.LoopId
                });
                _messageHub.PublishAsync<LoopBeingEditedMessage>(new LoopBeingEditedMessage(this, loop.LoopId));
                RefreshLoopsViewWithUpdatedIndexes();
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while adding loop: " + ex.Message);
                View.LoopError(ex);
            }
        }
        
        private void EditLoop(Loop loop)
        {
            _messageHub.PublishAsync<LoopBeingEditedMessage>(new LoopBeingEditedMessage(this, loop.LoopId));
        }

        private void SelectLoop(Loop loop)
        {
            _loop = loop;
            _audioFile = _playerService.CurrentPlaylistItem.AudioFile;
            _lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
        }
        
        private void DeleteLoop(Loop loop)
        {
            try
            {
                _loops.Remove(loop);
                foreach(var segment in loop.Segments)
                    _libraryService.DeleteSegment(segment.SegmentId);
                _libraryService.DeleteLoop(loop.LoopId);
                _messageHub.PublishAsync(new LoopUpdatedMessage(this) { 
                    AudioFileId = loop.AudioFileId,
                    LoopId = loop.LoopId
                });
                RefreshLoopsViewWithUpdatedIndexes();
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while deleting loop: " + ex.Message);
                View.LoopError(ex);
            }
        }
        
        private void PlayLoop(Loop loop)
        {
            try
            {
                if(_playerService.IsPlayingLoop)
                    _playerService.StopLoop();
                else
                    _playerService.StartLoop(loop);
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while playing loop: " + ex.Message);
                View.LoopError(ex);
            }            
        }

	    private void UpdateLoop(Loop loop)
	    {
            try
            {
                _libraryService.UpdateLoop(loop);
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while changing loop name: " + ex.Message);
                View.LoopError(ex);
            }	        
	    }

        private void PunchInLoopSegment(Segment segment)
        {
            try
            {
                var position = _playerService.GetPosition();
                float positionPercentage = (float)position.PositionBytes / (float)_lengthBytes;
                ChangeSegmentPosition(segment, positionPercentage, true);
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while punching in a segment: " + ex.Message);
                View.LoopError(ex);
            }
        }

        private void ChangedLoopSegmentPosition(Segment segment, float positionPercentage)
        {
            ChangeSegmentPosition(segment, positionPercentage, true);
        }

        private void ChangingLoopSegmentPosition(Segment segment, float positionPercentage)
        {
            ChangeSegmentPosition(segment, positionPercentage, false);
        }

        private void ChangeSegmentPosition(Segment segment, float positionPercentage, bool updateDatabase)
        {
            try
            {
                var startSegment = _loop.GetStartSegment();
                var endSegment = _loop.GetEndSegment();

                // Make sure the loop length doesn't get below 0
                if (segment == startSegment && positionPercentage > ((float)endSegment.PositionBytes / (float)_lengthBytes))
                {
                    positionPercentage = (float)endSegment.PositionBytes / (float)_lengthBytes;
                }
                else if (segment == endSegment && positionPercentage < ((float)startSegment.PositionBytes / (float)_lengthBytes))
                {
                    positionPercentage = (float)startSegment.PositionBytes / (float)_lengthBytes;
                }

                segment.SetPositionFromPercentage(positionPercentage, _lengthBytes, _audioFile);

                if (updateDatabase)
                {
                    _libraryService.UpdateSegment(segment);
                    _messageHub.PublishAsync(new LoopUpdatedMessage(this)
                    {
                        AudioFileId = _audioFile.Id,
                        LoopId = _loop.LoopId
                    });
                }

                View.RefreshLoopSegment(segment, _lengthBytes);
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while calculating the segment position: " + ex.Message);
                View.LoopError(ex);
            }
        }
    }
}
