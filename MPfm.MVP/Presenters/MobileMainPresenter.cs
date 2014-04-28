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

using System.Collections.Generic;
using System.Linq;
using MPfm.Library.Objects;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Config;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Mobile Main window presenter.
	/// </summary>
	public class MobileMainPresenter : BasePresenter<IMobileMainView>, IMobileMainPresenter
	{
		readonly MobileNavigationManager _navigationManager;

        public MobileMainPresenter(MobileNavigationManager navigationManager)
		{
			_navigationManager = navigationManager;          
		}
		
		public override void BindView(IMobileMainView view)
		{            
			base.BindView(view);
		}
	}
}
