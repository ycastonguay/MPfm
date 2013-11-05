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
using Android.App;
using Android.OS;
using Android.Preferences;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments.Base
{
    public class BasePreferenceFragment : PreferenceFragment, IBaseView
    {
        bool _isViewAlreadyBound = false;

        public BasePreferenceFragment()
        {
        }

        public override void OnResume()
        {
            base.OnResume();
            //if (OnViewReady != null && !_isViewAlreadyBound)
            //{
            //    // Since OnResume is called if the fragment is reactivated (i.e. not destroyed), we need a flag to know if the view was already bound
            //    _isViewAlreadyBound = true;
            //    OnViewReady(this);
            //}
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            if (OnViewDestroy != null) OnViewDestroy(this);
        }

        #region IBaseView implementation

        //public Action<IBaseView> OnViewReady { get; set; }
        public Action<IBaseView> OnViewDestroy { get; set; }
        public void ShowView(bool shown)
        {
            // Ignore on Android
        }

        #endregion
    }
}
