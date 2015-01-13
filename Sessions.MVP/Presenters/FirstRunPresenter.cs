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

using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services;
using Sessions.MVP.Views;
using Sessions.MVP.Config;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// First Run presenter.
	/// </summary>
    public class FirstRunPresenter : BasePresenter<IFirstRunView>, IFirstRunPresenter
	{
        public FirstRunPresenter()
		{	
		}

        public override void BindView(IFirstRunView view)
        {
            view.OnCloseView = CloseView;
            base.BindView(view);

            // make sure directories exist before initializing configuration (TODO: Remove this and do proper init in InitializationService)
            InitializationService.CreateDirectories(); 
        }

        private void CloseView()
        {
            AppConfigManager.Instance.Root.IsFirstRun = false;
            AppConfigManager.Instance.Save();
        }
	}
}
