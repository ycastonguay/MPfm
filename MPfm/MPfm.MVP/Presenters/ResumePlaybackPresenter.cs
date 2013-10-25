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
using MPfm.Library.Objects;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.Services.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Messages;
using TinyMessenger;
using System.Linq;

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
        private readonly IAudioFileCacheService _audioFileCacheService;

        public ResumePlaybackPresenter(ITinyMessengerHub messengerHub, IAudioFileCacheService audioFileCacheService, ICloudLibraryService cloudLibrary)
		{
            _messengerHub = messengerHub;
            _audioFileCacheService = audioFileCacheService;
            _cloudLibrary = cloudLibrary;
            _cloudLibrary.OnCloudDataChanged += (data) => {
                Task.Factory.StartNew(() => {
                    Console.WriteLine("ResumePlaybackPresenter - OnCloudDataChanged - Sleeping...");
                    Thread.Sleep(500); // TODO: Wait for download to finish with listener (see Dropbox docs)
                    Console.WriteLine("ResumePlaybackPresenter - OnCloudDataChanged - Fetching device infos...");
                    RefreshDevices();
                });
            };

            #if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            #else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
            #endif
		}

        public override void BindView(IResumePlaybackView view)
        {
            base.BindView(view);

            view.OnResumePlayback = ResumePlayback;

            Task.Factory.StartNew(() => {
                RefreshDevices();
            });
        }     

        private void RefreshDevices()
        {
            try
            {
                var devices = _cloudLibrary.PullDeviceInfos();
                View.RefreshDevices(devices.OrderBy(x => x.DeviceName).ToList());
            } 
            catch (Exception ex)
            {
                View.ResumePlaybackError(ex);
            }
        }

        private void ResumePlayback(CloudDeviceInfo device)
        {
            var audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == device.AudioFileId);
            if (audioFile == null)
            {
                audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.ArtistName.ToUpper() == device.ArtistName.ToUpper() &&
                                                                                x.AlbumTitle.ToUpper() == device.AlbumTitle.ToUpper() &&
                                                                                x.Title.ToUpper() == device.SongTitle.ToUpper());
            }
            Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this) 
            {
                Query = new LibraryQuery() {
                    ArtistName = device.ArtistName,
                    AlbumTitle = device.AlbumTitle
                }, 
                FilePath = audioFile != null ? audioFile.FilePath : string.Empty
            });

            // Only need to create the Player view on mobile devices
            #if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager.CreatePlayerView(MobileNavigationTabType.More, onViewBindedToPresenter);
            #endif
        }
	}
}
