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
using Android.OS;
using Android.Views;
using Sessions.Android.Classes.Fragments.Base;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using Sessions.Player.Objects;

namespace Sessions.Android.Classes.Fragments
{
    public class LoopsFragment : BaseFragment, ILoopsView
    {        
        private View _view;

        // Leave an empty constructor or the application will crash at runtime
        public LoopsFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Loops, container, false);
            return _view;
        }

        public override void OnResume()
        {
            base.OnResume();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindLoopsView(this);
        }

        #region ILoopsView implementation

        public Action OnAddLoop { get; set; }
        public Action<Loop> OnEditLoop { get; set; }
        public Action<Loop> OnDeleteLoop { get; set; }
        public Action<Loop> OnPlayLoop { get; set; }

        public void LoopError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshLoops(List<Loop> loops)
        {
        }

        #endregion
    }
}
