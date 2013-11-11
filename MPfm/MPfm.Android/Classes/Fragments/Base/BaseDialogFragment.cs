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
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments.Base
{
    public class BaseDialogFragment : DialogFragment, IBaseView
    {
        public Action<IBaseView> OnViewDestroy { get; set; }
        public void ShowView(bool shown)
        {
            // Ignore on Android
        }

        public BaseDialogFragment()
        {
        }

        public override void OnStart()
        {
            base.OnStart();
            //if (OnViewReady != null) OnViewReady(this);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            if (OnViewDestroy != null) OnViewDestroy(this);
        }

        protected void ShowErrorDialog(Exception ex)
        {
            Activity.RunOnUiThread(() =>
            {
                AlertDialog ad = new AlertDialog.Builder(Activity).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }
    }
}
