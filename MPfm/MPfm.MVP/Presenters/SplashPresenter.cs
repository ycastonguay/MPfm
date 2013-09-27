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
        readonly IInitializationService _initializationService;
        readonly IPlayerService _playerService;

		public SplashPresenter(IInitializationService initializationService, IPlayerService playerService)
		{
            _initializationService = initializationService;
            _playerService = playerService;
		}

#if IOS
        // iOS: Use async because ContinueWith has a strange behavior in Xamarin.iOS 7+. TODO: Check for a solution with Xamarin
        public async void Initialize(Action onInitDone)
        {
            if (_playerService.IsInitialized)
            {
                onInitDone.Invoke();
                return;
            }

            var task = Task<string>.Factory.StartNew(() =>
                                                     {
                Console.WriteLine("SplashPresenter - Initialize - Initializing service on another thread...");
                View.RefreshStatus("Loading...");
                _initializationService.Initialize();
                Console.WriteLine("SplashPresenter - Initialize - Initializing service on another thread... DONE!");
                return string.Empty;
            });
            Console.WriteLine("SplashPresenter - Initialize - Before task");
            string test = await task;
            Console.WriteLine("SplashPresenter - Initialize - After task");

            // Initialize player
            Console.WriteLine("SplashPresenter - Initialize - Initializing player on main thread...");
            Device device = new Device(){
                DriverType = DriverType.DirectSound,
                Id = -1
            };
            _playerService.Initialize(device, 44100, 1000, 100);
            View.InitDone(true);
            onInitDone.Invoke();
            Console.WriteLine("SplashPresenter - Initialize - Initializing player on main thread... DONE!");
            View.RefreshStatus("Opening app...");

            //            Task.Factory.StartNew(() =>
            //            {
            //                Console.WriteLine("SplashPresenter - Initialize - Initializing service on another thread...");
            //                View.RefreshStatus("Loading...");
            //                _initializationService.Initialize();
            //            }).ContinueWith((a) =>
            //            {
            //                // Initialize player
            //                Console.WriteLine("SplashPresenter - Initialize - Initializing player on main thread...");
            //                Device device = new Device(){
            //                    DriverType = DriverType.DirectSound,
            //                    Id = -1
            //                };
            //                _playerService.Initialize(device, 44100, 1000, 100);
            //                View.InitDone(true);
            //                onInitDone.Invoke();
            //                Console.WriteLine("SplashPresenter - Initialize - Initializing player on main thread... DONE!");
            //                View.RefreshStatus("Opening app...");
            //            }, taskScheduler);

        }
#else
        public void Initialize(Action onInitDone)
        {
            if (_playerService.IsInitialized)
            {
                onInitDone.Invoke();
                return;
            }

            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            #if LINUX
            // Mono on Linux crashes for some reason if FromCurrentSynchronizationContext is used... weird!            
            taskScheduler = TaskScheduler.Default;
            #endif
            Task.Factory.StartNew(() =>
                                  {
                View.RefreshStatus("Loading...");
                _initializationService.Initialize();
            }).ContinueWith((a) =>
                            {
                // Initialize player
                Device device = new Device(){
                    DriverType = DriverType.DirectSound,
                    Id = -1
                };
                _playerService.Initialize(device, 44100, 1000, 100);
                View.InitDone(true);
                onInitDone.Invoke();
                View.RefreshStatus("Opening app...");
            }, taskScheduler);
        }
#endif

	}
}
