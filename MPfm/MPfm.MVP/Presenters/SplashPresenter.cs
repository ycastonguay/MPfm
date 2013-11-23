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
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Splash screen presenter.
	/// </summary>
	public class SplashPresenter : BasePresenter<ISplashView>, ISplashPresenter
	{
        private readonly IInitializationService _initializationService;
        private readonly IPlayerService _playerService;
	    private readonly ICloudLibraryService _cloudLibraryService;

	    public SplashPresenter(IInitializationService initializationService, IPlayerService playerService, ICloudLibraryService cloudLibraryService)
		{
            _initializationService = initializationService;
            _playerService = playerService;
		    _cloudLibraryService = cloudLibraryService;
		}

        public void Initialize(Action onInitDone)
        {
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

            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            #if LINUX
            // Mono on Linux crashes for some reason if FromCurrentSynchronizationContext is used... weird!            
            taskScheduler = TaskScheduler.Default;
            #endif
            Task.Factory.StartNew(() =>
            {
                View.RefreshStatus("Starting initialization service...");
                _initializationService.Initialize();

                try
                {                    
                    _cloudLibraryService.PullDeviceInfos();
                }
                catch (Exception ex)
                {
                    // If the cloud service fails, launch the app anyway, this is not vital for running the app   
                    Tracing.Log("SplashPresenter - Initialize - PullDeviceInfos exception: {0}", ex);
                }

                View.RefreshStatus("Starting initialization service (2)...");
            }).ContinueWith((a) =>
            {
                View.InitDone(true);
                onInitDone.Invoke();
                View.RefreshStatus("Opening app...");
            }, taskScheduler);
        }
	}
}
