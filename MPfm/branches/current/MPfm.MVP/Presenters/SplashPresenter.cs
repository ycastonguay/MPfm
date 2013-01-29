//
// SplashPresenter.cs: Splash screen presenter.
//
// Copyright © 2011-2012 Yanick Castonguay
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

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Splash screen presenter.
	/// </summary>
	public class SplashPresenter : BasePresenter<ISplashView>, ISplashPresenter
	{       
        readonly IInitializationService initializationService;

		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="SplashPresenter"/> class.
		/// </summary>
		public SplashPresenter(IInitializationService initializationService)
		{
            this.initializationService = initializationService;
		}

		#endregion		
		
		#region ISplashPresenter implementation
		
        public void Initialize(Action onInitDone)
        {
            Task.Factory.StartNew(() => {
                Console.WriteLine("SplashPresenter - Starting initialization service...");
                View.RefreshStatus("Loading...");
                initializationService.Initialize();
                View.RefreshStatus("Initialization done!");
                Console.WriteLine("SplashPresenter - Initialization service done!");
            }).ContinueWith((a) => {
                Console.WriteLine("SplashPresenter - Notifying view...");
                View.InitDone();        
                Console.WriteLine("SplashPresenter - Raising action...");
                onInitDone.Invoke();
                Console.WriteLine("SplashPresenter - Action raised successfully!");
            });
        }
		
		#endregion

	}
}
