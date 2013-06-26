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
    public class SplashFragment : BaseDialogFragment, ISplashView, View.IOnClickListener
    {        
        private View _view;
        private TextView _textView;

        // Leave an empty constructor or the application will crash at runtime
        public SplashFragment() : base(null) {}

        public SplashFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Splash, container, false);
            _textView = _view.FindViewById<TextView>(Resource.Id.splash_text);
            Dialog.RequestWindowFeature((int) WindowFeatures.NoTitle);
            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //SetStyle(DialogFragmentStyle.NoFrame, Resource.Style.SplashTheme);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.SplashTheme);
        }

        public void OnClick(View v)
        {
            
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
            Console.WriteLine("SplashFragment - RefreshStatus");
            Activity.RunOnUiThread(() =>
            {
                _textView.Text = message;
            });
        }

        public void InitDone()
        {
            //this.Dismiss();
        }

        #endregion

    }
}
