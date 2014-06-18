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
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;

namespace Sessions.MVP.Presenters
{
	/// <summary>
	/// Base presenter.
	/// </summary>
	public class BasePresenter<T> : IBasePresenter<T> where T : IBaseView
	{
        public T View { get; private set; }

		public BasePresenter()
		{	
		}

		/// <summary>
		/// Binds the view to its implementation.
		/// </summary>
		/// <param name='view'>View implementation</param>		
		public virtual void BindView(T view)
		{
			if(view == null)			
				throw new ArgumentNullException("The view parameter is null!");			
						
			this.View = view;
		}

	    public virtual void ViewDestroyed()
	    {
	    }
	}
}
