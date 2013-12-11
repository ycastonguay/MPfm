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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.BassNetWrapper;
using Timer = System.Timers.Timer;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Splash screen presenter.
	/// </summary>
	public class SplashPresenter : BasePresenter<ISplashView>, ISplashPresenter
	{
        private readonly object _locker = new object();
	    private readonly Timer _timerDownloadFiles;
        private Action _onInitDone;
        private TaskScheduler _taskScheduler;
	    private bool _hasFinishedInitialization = false;

        private readonly IInitializationService _initializationService;
        private readonly IPlayerService _playerService;
	    private readonly ICloudLibraryService _cloudLibraryService;

	    public SplashPresenter(IInitializationService initializationService, IPlayerService playerService, ICloudLibraryService cloudLibraryService)
		{
            _initializationService = initializationService;
            _playerService = playerService;
		    _cloudLibraryService = cloudLibraryService;
            _cloudLibraryService.OnDeviceInfosAvailable += CloudLibraryServiceOnDeviceInfosAvailable;
	        _cloudLibraryService.OnDeviceInfosDownloadProgress += (progress) =>
	        {
                // TODO: For some reason, the UI isn't updated frequently enough for the progress to display correctly.
                //View.RefreshStatus(string.Format("{0}", progress));
	        };

            _timerDownloadFiles = new Timer(5000);
            _timerDownloadFiles.Elapsed += TimerDownloadFilesOnElapsed;
		}

	    public void Initialize(Action onInitDone)
	    {
	        _onInitDone = onInitDone;
            if (_playerService.IsInitialized)
            {
                onInitDone();
                return;
            }

            // Initialize player
            View.RefreshStatus("Initializing player...");
            Device device = new Device()
            {
                DriverType = DriverType.DirectSound,
                Id = -1
            };
            _playerService.Initialize(device, 44100, 1000, 100);
            View.RefreshStatus("Init player done");

#if LINUX
            // Mono on Linux crashes for some reason if FromCurrentSynchronizationContext is used... weird!            
            _taskScheduler = TaskScheduler.Default;
#else
            _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
#endif

            var cancellationToken = new CancellationToken();
            Task.Factory.StartNew(() =>
            {
                View.RefreshStatus("Initializing app...");
                _initializationService.Initialize();

                try
                {
                    View.RefreshStatus("Syncing data from cloud...");
                    _timerDownloadFiles.Start();
                    _cloudLibraryService.PullDeviceInfos();
                } 
                catch (Exception ex)
                {
                    // If the cloud service fails, launch the app anyway, this is not vital for running the app   
                    Tracing.Log("SplashPresenter - Initialize - PullDeviceInfos exception: {0}", ex);
                    Close();
                }
            //};
            }, cancellationToken, TaskCreationOptions.LongRunning, _taskScheduler);
	    }

        private void CloudLibraryServiceOnDeviceInfosAvailable(IEnumerable<CloudDeviceInfo> deviceInfos)
        {
            Tracing.Log("SplashPresenter - CloudLibraryServiceOnDeviceInfosAvailable - deviceInfos.Count: {0}", deviceInfos.Count());
            Close();
        }

        private void TimerDownloadFilesOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Tracing.Log("SplashPresenter - TimerDownloadFilesOnElapsed");
            _timerDownloadFiles.Stop();
            Close();
        }

	    private void Close()
	    {
	        var token = new CancellationToken();
	        Task.Factory.StartNew(() =>
	        {
	            lock (_locker)
	            {
                    if (_hasFinishedInitialization)
                        return;
                    
	                View.InitDone(true);
	                _onInitDone();
	                View.RefreshStatus("Opening app...");
	                _hasFinishedInitialization = true;
	            }
	        }, token, TaskCreationOptions.None, _taskScheduler);
	    }
	}
}
