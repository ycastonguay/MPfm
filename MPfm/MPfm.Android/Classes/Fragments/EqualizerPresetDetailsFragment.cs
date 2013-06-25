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
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.Android.Classes.Fragments
{
    public class EqualizerPresetDetailsFragment : BaseFragment, IEqualizerPresetDetailsView, View.IOnClickListener
    {        
        private View _view;

        // Leave an empty constructor or the application will crash at runtime
        public EqualizerPresetDetailsFragment() : base(null) { }

        public EqualizerPresetDetailsFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.EqualizerPresetDetails, container, false);
            return _view;
        }

        public void OnClick(View v)
        {
            
        }

        #region IEqualizerPresetDetails implementation

        public Action OnResetPreset { get; set; }
        public Action OnNormalizePreset { get; set; }
        public Action OnRevertPreset { get; set; }
        public Action<string> OnSavePreset { get; set; }
        public Action<string, float> OnSetFaderGain { get; set; }
        
        public void EqualizerPresetDetailsError(Exception ex)
        {
        }

        public void ShowMessage(string title, string message)
        {
        }

        public void RefreshPreset(EQPreset preset)
        {
        }

        #endregion
    }
}
