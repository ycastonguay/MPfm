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
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class TimeShiftingFragment : BaseFragment, ITimeShiftingView, View.IOnClickListener
    {        
        private View _view;

        // Leave an empty constructor or the application will crash at runtime
        public TimeShiftingFragment() : base(null) { }

        public TimeShiftingFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.TimeShifting, container, false);
            return _view;
        }

        public void OnClick(View v)
        {
            
        }

        #region ITimeShiftingView implementation

        public Action<float> OnSetTimeShifting { get; set; }
        public Action OnResetTimeShifting { get; set; }
        public Action OnUseDetectedTempo { get; set; }
        public Action OnIncrementTempo { get; set; }
        public Action OnDecrementTempo { get; set; }

        public void RefreshTimeShifting(PlayerTimeShiftingEntity entity)
        {
        }   

        public void TimeShiftingError(Exception ex)
        {
        }

        #endregion

    }
}
