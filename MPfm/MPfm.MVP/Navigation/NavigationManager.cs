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
using System.Collections.Generic;
using MPfm.Core;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Bootstrap;
using MPfm.Player.Objects;
using TinyIoC;
using MPfm.MVP.Views;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.Library.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Navigation
{
    /// <summary>
    /// Manager class for managing view and presenter instances.
    /// </summary>
    public abstract class NavigationManager
    {
        ISplashView _splashView;
        ISplashPresenter _splashPresenter;

        IMainView _mainView;
        IMainPresenter _mainPresenter;
        IPlayerPresenter _playerPresenter;
        ILibraryBrowserPresenter _libraryBrowserPresenter;
        ISongBrowserPresenter _songBrowserPresenter;
        IMarkersPresenter _markersPresenter;
        ILoopsPresenter _loopsPresenter;
        ITimeShiftingPresenter _timeShiftingPresenter;
        IPitchShiftingPresenter _pitchShiftingPresenter;

        IResumePlaybackView _resumePlaybackView;
        IResumePlaybackPresenter _resumePlaybackPresenter;
        IStartResumePlaybackView _startResumePlaybackView;
        IStartResumePlaybackPresenter _startResumePlaybackPresenter;

        IMarkerDetailsView _markerDetailsView;
        IMarkerDetailsPresenter _markerDetailsPresenter;

        ILoopDetailsView _loopDetailsView;
        ILoopDetailsPresenter _loopDetailsPresenter;

        IFirstRunView _firstRunView;
        IFirstRunPresenter _firstRunPresenter;

        IEditSongMetadataView _editSongMetadataView;
        IEditSongMetadataPresenter _editSongMetadataPresenter;

        IDesktopPreferencesView _preferencesView;
        IAudioPreferencesPresenter _audioPreferencesPresenter;
        ICloudPreferencesPresenter _cloudPreferencesPresenter;
        IGeneralPreferencesPresenter _generalPreferencesPresenter;
        ILibraryPreferencesPresenter _libraryPreferencesPresenter;

        ICloudConnectView _cloudConnectView;
        ICloudConnectPresenter _cloudConnectPresenter;

        IPlaylistView _playlistView;
        IPlaylistPresenter _playlistPresenter;

        IUpdateLibraryView _updateLibraryView;
        IUpdateLibraryPresenter _updateLibraryPresenter;

        IDesktopEffectsView _effectsView;
        IEqualizerPresetsPresenter _equalizerPresetsPresenter;
        IEqualizerPresetDetailsPresenter _equalizerPresetDetailsPresenter;

        ISyncView _syncView;
        ISyncPresenter _syncPresenter;
        ISyncMenuView _syncMenuView;
        ISyncMenuPresenter _syncMenuPresenter;
        ISyncDownloadView _syncDownloadView;
        ISyncDownloadPresenter _syncDownloadPresenter;
        ISyncCloudView _syncCloudView;
        ISyncCloudPresenter _syncCloudPresenter;
        ISyncWebBrowserView _syncWebBrowserView;
        ISyncWebBrowserPresenter _syncWebBrowserPresenter;

        public virtual ISplashView CreateSplashView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action onInitDone = () =>
            {
                Tracing.Log("SplashInitDone");
                CreateMainView();
            };
            Action<IBaseView> onViewReady = (view) =>
            {
                _splashPresenter = Bootstrapper.GetContainer().Resolve<ISplashPresenter>();
                _splashPresenter.BindView((ISplashView)view);
                _splashPresenter.Initialize(onInitDone); // TODO: Should the presenter call NavMgr instead of using an action?
            };
            _splashView = Bootstrapper.GetContainer().Resolve<ISplashView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _splashView.OnViewDestroy = (view) =>
            {
                _splashPresenter.ViewDestroyed();
                _splashPresenter = null;
                _splashView = null;
            };
            return _splashView;
        }
        
        public virtual IMainView CreateMainView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) => {
                _mainPresenter = Bootstrapper.GetContainer().Resolve<IMainPresenter>();
                _mainPresenter.BindView((IMainView)view);                
                _playerPresenter = Bootstrapper.GetContainer().Resolve<IPlayerPresenter>();
                _playerPresenter.BindView((IPlayerView)view);
                _libraryBrowserPresenter = Bootstrapper.GetContainer().Resolve<ILibraryBrowserPresenter>();
                _libraryBrowserPresenter.BindView((ILibraryBrowserView)view);                
                _songBrowserPresenter = Bootstrapper.GetContainer().Resolve<ISongBrowserPresenter>();
                _songBrowserPresenter.BindView((ISongBrowserView)view);
                _markersPresenter = Bootstrapper.GetContainer().Resolve<IMarkersPresenter>();
                _markersPresenter.BindView((IMarkersView)view);
                _loopsPresenter = Bootstrapper.GetContainer().Resolve<ILoopsPresenter>();
                _loopsPresenter.BindView((ILoopsView)view);
                _timeShiftingPresenter = Bootstrapper.GetContainer().Resolve<ITimeShiftingPresenter>();
                _timeShiftingPresenter.BindView((ITimeShiftingView)view);
                _pitchShiftingPresenter = Bootstrapper.GetContainer().Resolve<IPitchShiftingPresenter>();
                _pitchShiftingPresenter.BindView((IPitchShiftingView)view);
            };            
            _mainView = Bootstrapper.GetContainer().Resolve<IMainView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _mainView.OnViewDestroy = (view) => {
                _mainView = null;
                _mainPresenter.ViewDestroyed();
                _mainPresenter = null;
                _playerPresenter.ViewDestroyed();
                _playerPresenter = null;
                _libraryBrowserPresenter.ViewDestroyed();
                _libraryBrowserPresenter = null;
                _songBrowserPresenter.ViewDestroyed();
                _songBrowserPresenter = null;
                _markersPresenter.ViewDestroyed();
                _markersPresenter = null;
                _loopsPresenter.ViewDestroyed();
                _loopsPresenter = null;
                _timeShiftingPresenter.ViewDestroyed();
                _timeShiftingPresenter = null;
                _pitchShiftingPresenter.ViewDestroyed();
                _pitchShiftingPresenter = null;
            };
            return _mainView;
        }
        
        public virtual IDesktopPreferencesView CreatePreferencesView()
        {
            if(_preferencesView != null)
            {
                _preferencesView.ShowView(true);
                return _preferencesView;
            }
            
            Action<IBaseView> onViewReady = (view) => {                    
                _audioPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IAudioPreferencesPresenter>();
                _audioPreferencesPresenter.BindView((IAudioPreferencesView)view);
                _cloudPreferencesPresenter = Bootstrapper.GetContainer().Resolve<ICloudPreferencesPresenter>();
                _cloudPreferencesPresenter.BindView((ICloudPreferencesView)view);
                _generalPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IGeneralPreferencesPresenter>();
                _generalPreferencesPresenter.BindView((IGeneralPreferencesView)view);
                _libraryPreferencesPresenter = Bootstrapper.GetContainer().Resolve<ILibraryPreferencesPresenter>();
                _libraryPreferencesPresenter.BindView((ILibraryPreferencesView)view);
            };
            
            _preferencesView = Bootstrapper.GetContainer().Resolve<IDesktopPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _preferencesView.OnViewDestroy = (view) => {
                _preferencesView = null;
                _audioPreferencesPresenter.ViewDestroyed();
                _audioPreferencesPresenter = null;
                _cloudPreferencesPresenter.ViewDestroyed();
                _cloudPreferencesPresenter = null;
                _generalPreferencesPresenter.ViewDestroyed();
                _generalPreferencesPresenter = null;
                _libraryBrowserPresenter.ViewDestroyed();
                _libraryPreferencesPresenter = null;
            };
            return _preferencesView;
        }

        public virtual IDesktopEffectsView CreateEffectsView()
        {
            if(_effectsView != null)
            {
                _effectsView.ShowView(true);
                return _effectsView;
            }

            Action<IBaseView> onViewReady = (view) => {                    
                _equalizerPresetsPresenter = Bootstrapper.GetContainer().Resolve<IEqualizerPresetsPresenter>();
                _equalizerPresetsPresenter.BindView((IEqualizerPresetsView)view);
                _equalizerPresetDetailsPresenter = Bootstrapper.GetContainer().Resolve<IEqualizerPresetDetailsPresenter>();
                _equalizerPresetDetailsPresenter.BindView((IEqualizerPresetDetailsView)view);
            };

            _effectsView = Bootstrapper.GetContainer().Resolve<IDesktopEffectsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _effectsView.OnViewDestroy = (view) => {
                _effectsView = null;
                _equalizerPresetsPresenter.ViewDestroyed();
                _equalizerPresetsPresenter = null;
                _equalizerPresetDetailsPresenter.ViewDestroyed();
                _equalizerPresetDetailsPresenter = null;
            };
            return _effectsView;
        }

        public virtual ISyncView CreateSyncView()
        {
            // If the view is still visible, just make it the top level window
            if(_syncView != null)
            {
                _syncView.ShowView(true);
                return _syncView;
            }
            
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
                {
                    _syncPresenter = Bootstrapper.GetContainer().Resolve<ISyncPresenter>();
                    _syncPresenter.BindView((ISyncView)view);
                };
            
            // Create view and manage view destruction
            _syncView = Bootstrapper.GetContainer().Resolve<ISyncView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _syncView.OnViewDestroy = (view) => {
                _syncPresenter.ViewDestroyed();
                _syncPresenter = null;
                _syncView = null;
            };
            return _syncView;
        }

        public virtual ISyncCloudView CreateSyncCloudView()
        {
            // If the view is still visible, just make it the top level window
            if (_syncCloudView != null)
            {
                _syncCloudView.ShowView(true);
                return _syncCloudView;
            }

            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _syncCloudPresenter = Bootstrapper.GetContainer().Resolve<ISyncCloudPresenter>();
                _syncCloudPresenter.BindView((ISyncCloudView)view);
            };

            // Create view and manage view destruction
            _syncCloudView = Bootstrapper.GetContainer().Resolve<ISyncCloudView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _syncCloudView.OnViewDestroy = (view) =>
            {
                _syncCloudPresenter.ViewDestroyed();
                _syncCloudPresenter = null;
                _syncCloudView = null;
            };
            return _syncCloudView;
        }

        public virtual ISyncWebBrowserView CreateSyncWebBrowserView()
        {
            // If the view is still visible, just make it the top level window
            if (_syncWebBrowserView != null)
            {
                _syncWebBrowserView.ShowView(true);
                return _syncWebBrowserView;
            }

            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _syncWebBrowserPresenter = Bootstrapper.GetContainer().Resolve<ISyncWebBrowserPresenter>();
                _syncWebBrowserPresenter.BindView((ISyncWebBrowserView)view);
            };

            // Create view and manage view destruction
            _syncWebBrowserView = Bootstrapper.GetContainer().Resolve<ISyncWebBrowserView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _syncWebBrowserView.OnViewDestroy = (view) =>
            {
                _syncWebBrowserPresenter.ViewDestroyed();
                _syncWebBrowserPresenter = null;
                _syncWebBrowserView = null;
            };
            return _syncWebBrowserView;
        }

        public virtual ISyncMenuView CreateSyncMenuView(SyncDevice device)
        {
            if(_syncMenuView != null)
            {
                _syncMenuView.ShowView(true);
                return _syncMenuView;
            }

            Action<IBaseView> onViewReady = (view) =>
            {                    
                _syncMenuPresenter = Bootstrapper.GetContainer().Resolve<ISyncMenuPresenter>();
                _syncMenuPresenter.BindView((ISyncMenuView)view);
                _syncMenuPresenter.SetSyncDevice(device);
            };

            _syncMenuView = Bootstrapper.GetContainer().Resolve<ISyncMenuView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _syncMenuView.OnViewDestroy = (view) => {
                _syncMenuPresenter.ViewDestroyed();
                _syncMenuPresenter = null;
                _syncMenuView = null;
            };
            return _syncMenuView;
        }

        public virtual ISyncDownloadView CreateSyncDownloadView(SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            if(_syncDownloadView != null)
            {
                _syncDownloadView.ShowView(true);
                return _syncDownloadView;
            }

            Action<IBaseView> onViewReady = (view) =>
            {                    
                _syncDownloadPresenter = Bootstrapper.GetContainer().Resolve<ISyncDownloadPresenter>();
                _syncDownloadPresenter.BindView((ISyncDownloadView)view);
                _syncDownloadPresenter.StartSync(device, audioFiles);
            };

            _syncDownloadView = Bootstrapper.GetContainer().Resolve<ISyncDownloadView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _syncDownloadView.OnViewDestroy = (view) => {
                _syncDownloadPresenter.ViewDestroyed();
                _syncDownloadPresenter = null;
                _syncDownloadView = null;
            };
            return _syncDownloadView;
        }

        public virtual IPlaylistView CreatePlaylistView()
        {
            if(_playlistView != null)
            {
                _playlistView.ShowView(true);
                return _playlistView;
            }

            Action<IBaseView> onViewReady = (view) =>
            {
                _playlistPresenter = Bootstrapper.GetContainer().Resolve<IPlaylistPresenter>();
                _playlistPresenter.BindView((IPlaylistView)view);
            };

            // Create view and manage view destruction
            _playlistView = Bootstrapper.GetContainer().Resolve<IPlaylistView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _playlistView.OnViewDestroy = (view) => {
                _playlistPresenter.ViewDestroyed();
                _playlistPresenter = null;
                _playlistView = null;
            };
            return _playlistView;
        }

        public virtual IUpdateLibraryView CreateUpdateLibraryView(List<string> filePaths, List<Folder> folderPaths)
        {
            if (_updateLibraryView != null)
            {
                _updateLibraryView.ShowView(true);
                return _updateLibraryView;
            }

            Action<IBaseView> onViewReady = (view) =>
            {
                _updateLibraryPresenter = Bootstrapper.GetContainer().Resolve<IUpdateLibraryPresenter>();
                _updateLibraryPresenter.BindView((IUpdateLibraryView)view);
                _updateLibraryPresenter.UpdateLibrary(filePaths, folderPaths);
            };

            _updateLibraryView = Bootstrapper.GetContainer().Resolve<IUpdateLibraryView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _updateLibraryView.OnViewDestroy = (view) =>
            {
                _updateLibraryPresenter.ViewDestroyed();
                _updateLibraryPresenter = null;
                _updateLibraryView = null;
            };
            return _updateLibraryView;
        }

        public virtual IResumePlaybackView CreateResumePlaybackView()
        {
            // If the view is still visible, just make it the top level window
            if (_resumePlaybackView != null)
            {
                _resumePlaybackView.ShowView(true);
                return _resumePlaybackView;
            }

            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _resumePlaybackPresenter = Bootstrapper.GetContainer().Resolve<IResumePlaybackPresenter>();
                _resumePlaybackPresenter.BindView((IResumePlaybackView)view);
            };

            // Create view and manage view destruction
            _resumePlaybackView = Bootstrapper.GetContainer().Resolve<IResumePlaybackView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _resumePlaybackView.OnViewDestroy = (view) =>
            {
                _resumePlaybackPresenter.ViewDestroyed();
                _resumePlaybackPresenter = null;
                _resumePlaybackView = null;
            };
            return _resumePlaybackView;
        }

        public virtual IStartResumePlaybackView CreateStartResumePlaybackView()
        {
            // If the view is still visible, just make it the top level window
            if (_startResumePlaybackView != null)
            {
                _startResumePlaybackView.ShowView(true);
                return _startResumePlaybackView;
            }

            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _startResumePlaybackPresenter = Bootstrapper.GetContainer().Resolve<IStartResumePlaybackPresenter>();
                _startResumePlaybackPresenter.BindView((IStartResumePlaybackView)view);
            };

            // Create view and manage view destruction
            _startResumePlaybackView = Bootstrapper.GetContainer().Resolve<IStartResumePlaybackView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _startResumePlaybackView.OnViewDestroy = (view) =>
            {
                _startResumePlaybackPresenter.ViewDestroyed();
                _startResumePlaybackPresenter = null;
                _startResumePlaybackView = null;
            };
            return _startResumePlaybackView;
        }

        public virtual ICloudConnectView CreateCloudConnectView()
        {
            // If the view is still visible, just make it the top level window
            if (_cloudConnectView != null)
            {
                _cloudConnectView.ShowView(true);
                return _cloudConnectView;
            }

            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _cloudConnectPresenter = Bootstrapper.GetContainer().Resolve<ICloudConnectPresenter>();
                _cloudConnectPresenter.BindView((ICloudConnectView)view);
            };

            // Create view and manage view destruction
            _cloudConnectView = Bootstrapper.GetContainer().Resolve<ICloudConnectView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _cloudConnectView.OnViewDestroy = (view) =>
            {
                _cloudConnectPresenter.ViewDestroyed();
                _cloudConnectPresenter = null;
                _cloudConnectView = null;
            };
            return _cloudConnectView;
        }

        public virtual IFirstRunView CreateFirstRunView()
        {
            if (_firstRunView != null)
            {
                _firstRunView.ShowView(true);
                return _firstRunView;
            }

            Action<IBaseView> onViewReady = (view) =>
            {
                _firstRunPresenter = Bootstrapper.GetContainer().Resolve<IFirstRunPresenter>();
                _firstRunPresenter.BindView((IFirstRunView)view);
            };

            _firstRunView = Bootstrapper.GetContainer().Resolve<IFirstRunView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _firstRunView.OnViewDestroy = (view) =>
            {
                _firstRunPresenter.ViewDestroyed();
                _firstRunPresenter = null;
                _firstRunView = null;
            };
            return _firstRunView;
        }

        public virtual IEditSongMetadataView CreateEditSongMetadataView(AudioFile audioFile)
        {
            if (_editSongMetadataView != null)
            {
                _editSongMetadataView.ShowView(true);
                return _editSongMetadataView;
            }

            Action<IBaseView> onViewReady = (view) =>
            {
                _editSongMetadataPresenter = Bootstrapper.GetContainer().Resolve<IEditSongMetadataPresenter>();
                _editSongMetadataPresenter.BindView((IEditSongMetadataView)view);
                _editSongMetadataPresenter.SetAudioFile(audioFile);
            };

            _editSongMetadataView = Bootstrapper.GetContainer().Resolve<IEditSongMetadataView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _editSongMetadataView.OnViewDestroy = (view) =>
            {
                _editSongMetadataPresenter.ViewDestroyed();
                _editSongMetadataPresenter = null;
                _editSongMetadataView = null;
            };
            return _editSongMetadataView;
        }

        public virtual IMarkerDetailsView CreateMarkerDetailsView(Guid markerId)
        {
            if (_markerDetailsView != null)
            {
                _markerDetailsView.ShowView(true);
                return _markerDetailsView;
            }

            Action<IBaseView> onViewReady = (view) =>
            {                
                _markerDetailsPresenter = Bootstrapper.GetContainer().Resolve<IMarkerDetailsPresenter>(new NamedParameterOverloads() { { "markerId", markerId } });
                _markerDetailsPresenter.BindView((IMarkerDetailsView)view);                
            };

            _markerDetailsView = Bootstrapper.GetContainer().Resolve<IMarkerDetailsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _markerDetailsView.OnViewDestroy = (view) =>
            {
                _markerDetailsPresenter.ViewDestroyed();
                _markerDetailsPresenter = null;
                _markerDetailsView = null;
            };
            return _markerDetailsView;
        }
    }
}
