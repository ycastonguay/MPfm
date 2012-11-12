//
// BasePresenter.cs: Base presenter.
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
using MPfm.Core;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using AutoMapper;

namespace MPfm.MVP
{
	/// <summary>
	/// Base presenter.
	/// </summary>
	public class BasePresenter : IBasePresenter
	{
		// Private variables
        //IBaseView view;

		#region Constructor and Dispose

		public BasePresenter()
		{	
		}

		#endregion
		
//		/// <summary>
//		/// Binds the view to its implementation.
//		/// </summary>
//		/// <param name='view'>View implementation</param>		
//		public void BindView(IBaseView view)
//		{
//			// Validate parameters
//			if(view == null)			
//				throw new ArgumentNullException("The view parameter is null!");			
//						
//			// Set properties
//			this.view = view;	
//		}
	}
}
