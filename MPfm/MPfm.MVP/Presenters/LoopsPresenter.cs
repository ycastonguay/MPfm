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

using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Loops view presenter.
	/// </summary>
	public class LoopsPresenter : BasePresenter<ILoopsView>, ILoopsPresenter
	{
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly NavigationManager _navigationManager;

        public LoopsPresenter()
		{
#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
		}

        public override void BindView(ILoopsView view)
        {            
            view.OnAddLoop = OnAddLoop;
            view.OnEditLoop = OnEditLoop;

            base.BindView(view);
        }

        private void CreateLoopDetailsView()
        {
#if IOS || ANDROID
            var view = _mobileNavigationManager.CreateLoopDetailsView();
            _mobileNavigationManager.PushDialogView("Loop Details", View, view);
#else
            string a = string.Empty;
#endif
        }

        private void OnAddLoop()
        {
            CreateLoopDetailsView();
        }
        
        private void OnEditLoop(Loop loop)
        {
        }
    }
}

