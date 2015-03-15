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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Objects;

namespace Sessions.Android
{
    [Activity(Label = "Marker Details", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MarkerDetailsActivity : BaseActivity, IMarkerDetailsView
    {
        private MobileNavigationManager _navigationManager;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("MarkerDetailsActivity - OnCreate");
            base.OnCreate(bundle);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.MarkerDetails);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            //((AndroidNavigationManager)_navigationManager).SetMarkerDetailsActivityInstance(this);
            //_navigationManager.BindMarkerDetailsView(this, );
        }

        protected override void OnStart()
        {
            Console.WriteLine("MarkerDetailsActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("MarkerDetailsActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("MarkerDetailsActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("MarkerDetailsActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("MarkerDetailsActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("MarkerDetailsActivity - OnDestroy");
            base.OnDestroy();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    var intent = new Intent(this, typeof(PlayerActivity));
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                    this.StartActivity(intent);
                    this.Finish();
                    return true;
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
                    break;
            }
        }

        #region IMarkerDetailsView implementation

        public Action<float> OnChangePositionMarkerDetails { get; set; }
        public Action<Marker> OnUpdateMarkerDetails { get; set; }
        public Action OnDeleteMarkerDetails { get; set; }
        public Action OnPunchInMarkerDetails { get; set; }

        public void MarkerDetailsError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void DismissMarkerDetailsView()
        {
        }

        public void RefreshMarker(Marker marker, AudioFile audioFile)
        {
        }

        public void RefreshMarkerPosition(string position, float positionPercentage)
        {
        }

        #endregion
    }
}
