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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using org.sessionsapp.player;
using Sessions.Core;
using Sessions.Library.Objects;
using Sessions.Library.Services.Interfaces;
using Sessions.MVP.Config;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Sound;
using Timer = System.Timers.Timer;

namespace Sessions.MVP.Presenters
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
                //Console.WriteLine("===>> OnDeviceInfosDownloadProgress - {0}", progress);
                View.RefreshStatus(string.Format("Syncing data from cloud ({0:0} %)...", progress * 100));
	        };

            _timerDownloadFiles = new Timer(5000);
            _timerDownloadFiles.Elapsed += TimerDownloadFilesOnElapsed;
		}

	    public void Initialize(Action onInitDone)
	    {
	        _onInitDone = onInitDone;
            if (_playerService.State != SSPPlayerState.Uninitialized)
            {
                onInitDone();
                return;
            }

            View.RefreshStatus("Initializing player...");
	        InitializationService.CreateDirectories();
            AppConfigManager.Instance.Load();

	        try
	        {
                // Try to inialize device with configured settings. 
                InitializeDevice(false);
	        }
	        catch
	        {
                // Failed to initialize; use the default device. If this fails, an exception will be thrown
                InitializeDevice(true);
	        }
            //View.RefreshStatus("Init player done");

#if LINUX
            // Mono on Linux crashes for some reason if FromCurrentSynchronizationContext is used... weird!            
            _taskScheduler = TaskScheduler.Default;
#else
            _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
#endif

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
            });
	    }

	    private void InitializeDevice(bool useDefaultDevice)
	    {
            try
            {
                var device = new SSPDevice();
                int sampleRate = 44100;
                int bufferSize = 1000;
                int updatePeriod = 100;
                if (!useDefaultDevice)
                {
                    sampleRate = AppConfigManager.Instance.Root.Audio.SampleRate;
                    bufferSize = AppConfigManager.Instance.Root.Audio.BufferSize;
                    updatePeriod = AppConfigManager.Instance.Root.Audio.UpdatePeriod;
                    var configuredDevice = AppConfigManager.Instance.Root.Audio.AudioDevice;

                    var devices = _playerService.GetOutputDevices();
                    var foundConfiguredDevice = devices.FirstOrDefault(x => x.Name.ToUpper() == configuredDevice.Name);
                    if (foundConfiguredDevice != null)
                        device = foundConfiguredDevice;
                }

                _playerService.InitDevice(device, sampleRate, bufferSize, updatePeriod);
            }
            catch (Exception ex)
            {
                Tracing.Log("Failed to initialize player: {0}", ex);
                throw;
            }
	    }

	    private void CloudLibraryServiceOnDeviceInfosAvailable(IEnumerable<CloudDeviceInfo> deviceInfos)
        {
            Tracing.Log("SplashPresenter - CloudLibraryServiceOnDeviceInfosAvailable - deviceInfos.Count: {0}", deviceInfos.Count());
            Close();
        }

        private void TimerDownloadFilesOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Tracing.Log("SplashPresenter - TimerDownloadFilesOnElapsed");
            View.RefreshStatus("Skipping resume playback because download time is taking too long...");
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

                    _hasFinishedInitialization = true;
	                View.InitDone(true);
	                _onInitDone();
	                View.RefreshStatus("Opening app...");
	            }
	        }, token, TaskCreationOptions.None, _taskScheduler);
	    }
	}
}
