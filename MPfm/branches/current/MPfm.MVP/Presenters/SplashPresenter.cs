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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;

namespace MPfm.MVP
{
	/// <summary>
	/// Splash screen presenter.
	/// </summary>
	public class SplashPresenter : ISplashPresenter
	{       
        public delegate void InitializeSplashDelegate();

        ISplashView view;
        InitializeSplashDelegate initializeSplashDelegate;
        readonly IInitializationService initializationService;

		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.MVP.SplashPresenter"/> class.
		/// </summary>
		public SplashPresenter(IInitializationService initializationService)
		{
            this.initializationService = initializationService;
            initializeSplashDelegate = new InitializeSplashDelegate(InitializeAsync);
		}

		#endregion		
		
		#region ISplashPresenter implementation
		
		/// <summary>
		/// Binds the view to its implementation.
		/// </summary>
		/// <param name='view'>Splash screen view implementation</param>	
		public void BindView(ISplashView view)
		{
			// Validate parameters 
			if(view == null)			
				throw new ArgumentNullException("The view parameter is null!");
			
			// Set property
			this.view = view;
		}

        public void Initialize()
        {
            // Initialize configuration and library
            //initializationService.Initialize();
            initializeSplashDelegate.BeginInvoke(InitializeAsyncCallback, initializeSplashDelegate);
        }
		
		#endregion

        public void InitializeAsync()
        {
            initializationService.Initialize();
        }
        
        public void InitializeAsyncCallback(IAsyncResult result)
        {
            object state = result.AsyncState;
            view.InitDone();
        }
	}
}
