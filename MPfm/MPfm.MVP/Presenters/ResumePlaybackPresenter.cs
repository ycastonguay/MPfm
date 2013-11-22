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
using System.Threading;
using System.Threading.Tasks;
using MPfm.Library.Objects;
using MPfm.Library.Services.Exceptions;
using MPfm.Library.Services.Interfaces;
using TinyMessenger;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using System.Collections.Generic;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Resume Playback view presenter.
	/// </summary>
    public class ResumePlaybackPresenter : BasePresenter<IResumePlaybackView>, IResumePlaybackPresenter
	{
        private readonly MobileNavigationManager _mobileNavigationManager;
        private readonly NavigationManager _navigationManager;
        private readonly ITinyMessengerHub _messengerHub;
        private readonly ICloudLibraryService _cloudLibrary;
	    private readonly IPlayerService _playerService;
	    private readonly IAudioFileCacheService _audioFileCacheService;
	    private bool _canRefreshCloudLoginStatus;

	    public ResumePlaybackPresenter(ITinyMessengerHub messengerHub, IAudioFileCacheService audioFileCacheService, ICloudLibraryService cloudLibrary, IPlayerService playerService)
		{
            _messengerHub = messengerHub;
            _audioFileCacheService = audioFileCacheService;
            _cloudLibrary = cloudLibrary;
            _playerService = playerService;
            _cloudLibrary.OnDeviceInfoUpdated += CloudLibraryOnDeviceInfoUpdated;

            #if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            #else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
            #endif
		}

	    private void CloudLibraryOnDeviceInfoUpdated(IEnumerable<CloudDeviceInfo> deviceInfos)
	    {
            //_cloudLibrary.OnCloudDataChanged += (data) => {
            //    Task.Factory.StartNew(() => {
            //        Console.WriteLine("ResumePlaybackPresenter - OnCloudDataChanged - Sleeping...");
            //        Thread.Sleep(500); // TODO: Wait for download to finish with listener (see Dropbox docs)
            //        Console.WriteLine("ResumePlaybackPresenter - OnCloudDataChanged - Fetching device infos...");
            //        RefreshDevices();
            //    });
            //};
        }

	    public override void BindView(IResumePlaybackView view)
        {
            view.OnResumePlayback = ResumePlayback;
            view.OnOpenPreferencesView = OpenPreferencesView;
            view.OnCheckCloudLoginStatus = CheckCloudLoginStatus;
            base.BindView(view);

            Task.Factory.StartNew(RefreshDevices);
        }

	    private void OpenPreferencesView()
	    {            
#if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager.CreateCloudPreferencesView();
#else
	        _navigationManager.CreatePreferencesView();
#endif
	    }

        private void CheckCloudLoginStatus()
        {
            if (!_canRefreshCloudLoginStatus)
                return;

            View.RefreshAppLinkedStatus(_cloudLibrary.HasLinkedAccount);
        }

	    private void RefreshDevices()
	    {
            //// Prevent login status change during loading
            //_canRefreshCloudLoginStatus = false;

            //Task.Factory.StartNew(() =>
            //{

            //    try
            //    {
            //        var devices = _cloudLibrary.PullDeviceInfos();
            //        var entities = new List<ResumePlaybackEntity>();
            //        foreach (var device in devices)
            //        {
            //            var audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == device.AudioFileId);
            //            if (audioFile == null)
            //            {
            //                audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.ArtistName.ToUpper() == device.ArtistName.ToUpper() &&
            //                x.AlbumTitle.ToUpper() == device.AlbumTitle.ToUpper() &&
            //                x.Title.ToUpper() == device.SongTitle.ToUpper());
            //            }
            //            var audioFiles = _audioFileCacheService.SelectAudioFiles(new LibraryQuery() {
            //                ArtistName = device.ArtistName,
            //                AlbumTitle = device.AlbumTitle
            //            });

            //            bool canResume = audioFiles.Count() > 0;

            //            entities.Add(new ResumePlaybackEntity() {
            //                DeviceInfo = device,
            //                LocalAudioFilePath = audioFile == null ? string.Empty : audioFile.FilePath,
            //                CanResumePlayback = canResume
            //            });
            //        }

            //        View.RefreshDevices(entities.OrderByDescending(x => x.CanResumePlayback).ThenBy(x => x.DeviceInfo.DeviceName).ToList());
            //        View.RefreshAppLinkedStatus(true);
            //    } catch (CloudAppNotLinkedException ex)
            //    {
            //        View.RefreshAppLinkedStatus(false);
            //    } catch (Exception ex)
            //    {
            //        View.ResumePlaybackError(ex);
            //    }

            //    _canRefreshCloudLoginStatus = true;
            //});
        }

        private void ResumePlayback(ResumePlaybackEntity entity)
        {
            var audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == entity.DeviceInfo.AudioFileId);
            if (audioFile == null)
            {
                audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.ArtistName.ToUpper() == entity.DeviceInfo.ArtistName.ToUpper() &&
                                                                             x.AlbumTitle.ToUpper() == entity.DeviceInfo.AlbumTitle.ToUpper() &&
                                                                             x.Title.ToUpper() == entity.DeviceInfo.SongTitle.ToUpper());
            }
            var audioFiles = _audioFileCacheService.SelectAudioFiles(new LibraryQuery() {
                ArtistName = entity.DeviceInfo.ArtistName,
                AlbumTitle = entity.DeviceInfo.AlbumTitle
            });

            if (audioFiles.Count() == 0)
            {
                View.AudioFilesNotFoundError("No audio files found", "Unfortunately, you cannot resume this playlist because none of the audio files could be found on your device.");
                return;
            }

            _playerService.Play(audioFiles, audioFile != null ? audioFile.FilePath : string.Empty, 0, false, false);

            // Only need to create the Player view on mobile devices
            #if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager.CreatePlayerView(MobileNavigationTabType.More);
            #endif
        }
	}
}
