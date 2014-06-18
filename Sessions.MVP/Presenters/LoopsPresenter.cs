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

using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Services.Interfaces;
using Sessions.Core;
using TinyMessenger;
using MPfm.MVP.Messages;
using System.Collections.Generic;
using System;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Loops view presenter.
	/// </summary>
	public class LoopsPresenter : BasePresenter<ILoopsView>, ILoopsPresenter
	{
        readonly ITinyMessengerHub _messageHub;
        readonly ILibraryService _libraryService;
        readonly IPlayerService _playerService;
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly NavigationManager _navigationManager;
        List<TinyMessageSubscriptionToken> _tokens = new List<TinyMessageSubscriptionToken>();
        List<Loop> _loops = new List<Loop>();
        Guid _audioFileId;
        
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
            view.OnDeleteLoop = DeleteLoop;
            view.OnPlayLoop = PlayLoop;

            base.BindView(view);
            
            // Subscribe to messages
            _tokens.Add(_messageHub.Subscribe<LoopUpdatedMessage>((LoopUpdatedMessage m) => {
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
                _libraryService.InsertLoop(loop);
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
                _playerService.StartLoop(loop);
            } 
            catch (Exception ex)
            {
                Tracing.Log("An error occured while playing loop: " + ex.Message);
                View.LoopError(ex);
            }            
        }
    }
}
