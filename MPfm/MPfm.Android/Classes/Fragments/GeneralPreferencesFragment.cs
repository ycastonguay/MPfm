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
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class GeneralPreferencesFragment : BaseFragment, IGeneralPreferencesView, View.IOnClickListener
    {        
        private View _view;
        private TextView _lblTitle;

        // Leave an empty constructor or the application will crash at runtime
        public GeneralPreferencesFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.GeneralPreferences, container, false);
            _lblTitle = _view.FindViewById<TextView>(Resource.Id.fragment_generalSettings_lblTitle);
            return _view;
        }

        public void OnClick(View v)
        {
            
        }

        public override void OnResume()
        {
            Console.WriteLine("GeneralPreferencesFragment - OnResume");
            base.OnResume();
        }

        public override void OnStart()
        {
            Console.WriteLine("GeneralPreferencesFragment - OnStart");
            base.OnStart();
        }

        public override void OnStop()
        {
            Console.WriteLine("GeneralPreferencesFragment - OnStop");
            base.OnStop();
        }

        public override void OnDestroyView()
        {
            Console.WriteLine("GeneralPreferencesFragment - OnDestroyView");
            base.OnDestroyView();
        }

        public override void OnPause()
        {
            Console.WriteLine("GeneralPreferencesFragment - OnPause");
            base.OnPause();
        }

        public override void OnDestroy()
        {
            Console.WriteLine("GeneralPreferencesFragment - OnDestroy");
            base.OnDestroy();
        }
    }
}
