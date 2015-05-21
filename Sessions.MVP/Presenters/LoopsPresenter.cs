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
using TinyMessenger;
using Sessions.MVP.Messages;
using System.Collections.Generic;
using System;
using Sessions.Sound.AudioFiles;
using org.sessionsapp.player;
using System.Linq;

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
        private List<SSPLoop> _loops = new List<SSPLoop>();
        private SSPLoop _loop;
        private Guid _audioFileId;
        private AudioFile _audioFile;
        
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

            view.OnChangeLoopName = ChangeLoopName;
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

            _playerService.OnLoopPlaybackStarted += HandleOnLoopPlaybackStarted;
            _playerService.OnLoopPlaybackStopped += HandleOnLoopPlaybackStopped;

            // Refresh initial data
            if (_playerService.CurrentAudioFile != null)
                RefreshLoops(_playerService.CurrentAudioFile.Id);
        }

        private void HandleOnLoopPlaybackStarted(IntPtr user)
        {
            View.RefreshCurrentlyPlayingLoop(_loop);
        }

        private void HandleOnLoopPlaybackStopped(IntPtr user)
        {
            View.RefreshCurrentlyPlayingLoop(null);
        }

        public override void ViewDestroyed()
        {
            foreach (TinyMessageSubscriptionToken token in _tokens)
                token.Dispose();

            base.ViewDestroyed();
        }

	    private void SetLoopIndexesAndPositionPercentages()
	    {
	        for (int a = 0; a < _loops.Count; a++)
	        {
	            var loop = _loops[a];
	            loop.Index = string.Format("{0}", Conversion.IndexToLetter(a));
                loop.StartPositionPercentage = (float)loop.StartPositionBytes / (float)_playerService.CurrentAudioFile.LengthBytes;
                loop.EndPositionPercentage = (float)loop.EndPositionBytes / (float)_playerService.CurrentAudioFile.LengthBytes;
	        }
	    }

	    private void RefreshLoopsViewWithUpdatedIndexesAndPositionPercentages()
	    {
            SetLoopIndexesAndPositionPercentages();
	        View.RefreshLoops(_loops);
	    }

	    private void RefreshLoops(Guid audioFileId)
        {
            try
            {
                //_loops = _libraryService.SelectLoopsIncludingSegments(audioFileId);
                _loops = _libraryService.SelectLoops(audioFileId);
                _audioFile = _playerService.CurrentAudioFile;
                RefreshLoopsViewWithUpdatedIndexesAndPositionPercentages();
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
                var loop = new SSPLoop();
                loop.AudioFileId = _playerService.CurrentAudioFile.Id;
                loop.Name = "New Loop";

                var startPosition = _playerService.GetPositionFromBytes(0);
                loop.StartPosition = startPosition.Str;
                loop.StartPositionBytes = startPosition.Bytes;
                loop.StartPositionMS = startPosition.MS;
                loop.StartPositionSamples = startPosition.Samples;

                var endPosition = _playerService.GetPositionFromBytes(_audioFile.LengthBytes);
                loop.EndPosition = endPosition.Str;
                loop.EndPositionBytes = endPosition.Bytes;
                loop.EndPositionMS = endPosition.MS;
                loop.EndPositionSamples = endPosition.Samples;

                _libraryService.InsertLoop(loop);
                _messageHub.PublishAsync(new LoopUpdatedMessage(this) { 
                    AudioFileId = loop.AudioFileId,
                    LoopId = loop.LoopId
                });
                _messageHub.PublishAsync<LoopBeingEditedMessage>(new LoopBeingEditedMessage(this, loop.LoopId));
                RefreshLoopsViewWithUpdatedIndexesAndPositionPercentages();
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while adding loop: " + ex.Message);
                View.LoopError(ex);
            }
        }
        
        private void EditLoop(SSPLoop loop)
        {
            _messageHub.PublishAsync<LoopBeingEditedMessage>(new LoopBeingEditedMessage(this, loop.LoopId));
        }

        private void ChangeLoopName(Guid loopId, string name)
        {
            try
            {
                var loop = _loops.FirstOrDefault(x => x.LoopId == loopId);
                loop.Name = name;
                UpdateLoop(loop);
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while changing the loop name: " + ex.Message);
                View.LoopError(ex);
            }
        }

        private void SelectLoop(SSPLoop loop)
        {
            try
            {
                // If there is a loop currently playing, stop the current loop
                if (_loop != null && _loop.LoopId != loop.LoopId && _playerService.Loop != null)
                {
                    if (_playerService.IsPlayingLoop && _playerService.Loop.LoopId == _loop.LoopId)
                    {
                        _playerService.StopLoop();
                    }
                }

                _loop = loop;
                _audioFile = _playerService.CurrentAudioFile;
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while setting the active loop: " + ex.Message);
                View.LoopError(ex);
            }
        }
        
        private void DeleteLoop(SSPLoop loop)
        {
            try
            {
                _loops.Remove(loop);
                _libraryService.DeleteLoop(loop.LoopId);
                _messageHub.PublishAsync(new LoopUpdatedMessage(this) { 
                    AudioFileId = loop.AudioFileId,
                    LoopId = loop.LoopId
                });
                RefreshLoopsViewWithUpdatedIndexesAndPositionPercentages();
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while deleting loop: " + ex.Message);
                View.LoopError(ex);
            }
        }
        
        private void PlayLoop(SSPLoop loop)
        {
            try
            {
                if (_playerService.IsPlayingLoop)
                {
                    _playerService.StopLoop();
                }
                else
                {
                    _playerService.StartLoop(loop);
                }
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while playing loop: " + ex.Message);
                View.LoopError(ex);
            }            
        }

        private void UpdateLoop(SSPLoop loop)
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

        private void PunchInLoopSegment(SSPLoopSegmentType segmentType)
        {
            try
            {
                var position = _playerService.GetPosition();
                float positionPercentage = (float)position.Bytes / (float)_audioFile.LengthBytes;
                ChangeSegmentPosition(segmentType, positionPercentage, true);
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while punching in a segment: " + ex.Message);
                View.LoopError(ex);
            }
        }

        private void ChangedLoopSegmentPosition(SSPLoopSegmentType segmentType, float positionPercentage)
        {
            ChangeSegmentPosition(segmentType, positionPercentage, true);
            UpdateLoop(_loop);

            if (_playerService.IsPlayingLoop)
                _playerService.UpdateLoop(_loop);
        }

        private void ChangingLoopSegmentPosition(SSPLoopSegmentType segmentType, float positionPercentage)
        {
            ChangeSegmentPosition(segmentType, positionPercentage, false);
        }

        private void ChangeSegmentPosition(SSPLoopSegmentType segmentType, float positionPercentage, bool updateDatabase)
        {
            try
            {
                // Make sure the loop length doesn't get below 0
                if (segmentType == SSPLoopSegmentType.Start && positionPercentage > ((float)_loop.EndPositionBytes / (float)_audioFile.LengthBytes))
                {
                    positionPercentage = (float)_loop.EndPositionBytes / (float)_audioFile.LengthBytes;
                }
                else if (segmentType == SSPLoopSegmentType.End && positionPercentage < ((float)_loop.StartPositionBytes / (float)_audioFile.LengthBytes))
                {
                    positionPercentage = (float)_loop.StartPositionBytes / (float)_audioFile.LengthBytes;
                }

                var pos = _playerService.GetPositionFromPercentage(positionPercentage);
                switch (segmentType)
                {
                    case SSPLoopSegmentType.Start:
                        _loop.StartPosition = pos.Str;
                        _loop.StartPositionBytes = pos.Bytes;
                        _loop.StartPositionSamples = pos.Samples;
                        _loop.StartPositionMS = pos.MS;
                        break;
                    case SSPLoopSegmentType.End:
                        _loop.EndPosition = pos.Str;
                        _loop.EndPositionBytes = pos.Bytes;
                        _loop.EndPositionSamples = pos.Samples;
                        _loop.EndPositionMS = pos.MS;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("segmentType");
                }

                long loopLength = _loop.EndPositionBytes - _loop.StartPositionBytes;
                var length = _playerService.GetPositionFromBytes(loopLength);
                _loop.Length = length.Str;
                _loop.LengthBytes = length.Bytes;
                _loop.LengthMS = length.MS;
                _loop.LengthSamples = length.Samples;

                if (updateDatabase)
                {
                    _libraryService.UpdateLoop(_loop);
                    _messageHub.PublishAsync(new LoopUpdatedMessage(this)
                    {
                        AudioFileId = _audioFile.Id,
                        LoopId = _loop.LoopId
                    });
                }

//                if (_playerService.IsPlayingLoop)
//                {
                    View.RefreshCurrentlyPlayingLoop(_loop);
//                }
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while calculating the segment position: " + ex.Message);
                View.LoopError(ex);
            }
        }
    }
}
