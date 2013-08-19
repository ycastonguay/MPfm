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
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Bootstrap;
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

        IDesktopPreferencesView _preferencesView;
        IAudioPreferencesPresenter _audioPreferencesPresenter;
        IGeneralPreferencesPresenter _generalPreferencesPresenter;
        ILibraryPreferencesPresenter _libraryPreferencesPresenter;

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

        public virtual ISplashView CreateSplashView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action onInitDone = () =>
            {
                Console.WriteLine("SplashInitDone");
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
                _splashView = null;
                _splashPresenter = null;
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
                _mainPresenter = null;
                _playerPresenter.Dispose(); // Dispose unmanaged stuff (i.e. BASS)
                _playerPresenter = null;
                _libraryBrowserPresenter = null;
                _songBrowserPresenter = null;
                _markersPresenter = null;
                _loopsPresenter = null;
                _timeShiftingPresenter = null;
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
                _generalPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IGeneralPreferencesPresenter>();
                _generalPreferencesPresenter.BindView((IGeneralPreferencesView)view);
                _libraryPreferencesPresenter = Bootstrapper.GetContainer().Resolve<ILibraryPreferencesPresenter>();
                _libraryPreferencesPresenter.BindView((ILibraryPreferencesView)view);
            };
            
            _preferencesView = Bootstrapper.GetContainer().Resolve<IDesktopPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _preferencesView.OnViewDestroy = (view) => {
                _preferencesView = null;
                _audioPreferencesPresenter = null;
                _generalPreferencesPresenter = null;
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
                _equalizerPresetsPresenter = null;
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
                _syncView = null;
                _syncPresenter = null;
            };
            return _syncView;
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
                _syncMenuView = null;
                _syncMenuPresenter = null;
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
                _syncDownloadView = null;
                _syncDownloadPresenter = null;
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
                _playlistView = null;
                _playlistPresenter = null;
            };
            return _playlistView;
        }

        public virtual IUpdateLibraryView CreateUpdateLibraryView(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
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
                _updateLibraryPresenter.UpdateLibrary(mode, filePaths, folderPath);
            };

            _updateLibraryView = Bootstrapper.GetContainer().Resolve<IUpdateLibraryView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _updateLibraryView.OnViewDestroy = (view) =>
            {
                _updateLibraryView = null;
                _updateLibraryPresenter = null;
            };
            return _updateLibraryView;
        }
    }
}
