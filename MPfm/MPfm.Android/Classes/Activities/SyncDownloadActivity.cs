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
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Navigation;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.Android
{
    [Activity(Label = "Sync Download", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SyncDownloadActivity : BaseActivity, ISyncDownloadView
    {
        MobileNavigationManager _navigationManager;
        TextView _lblTitle;
        TextView _lblFileName;
        TextView _lblCompletedValue;
        TextView _lblCurrentFileProgressValue;
        TextView _lblDownloadSpeedValue;
        TextView _lblErrorsValue;
        TextView _lblFilesDownloadedValue;
        TextView _lblTotalFilesValue;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("SyncDownloadActivity - OnCreate");
            base.OnCreate(bundle);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.SyncDownload);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _lblTitle = FindViewById<TextView>(Resource.Id.syncDownload_lblTitle);
            _lblFileName = FindViewById<TextView>(Resource.Id.syncDownload_lblFileName);
            _lblCompletedValue = FindViewById<TextView>(Resource.Id.syncDownload_lblCompletedValue);
            _lblCurrentFileProgressValue = FindViewById<TextView>(Resource.Id.syncDownload_lblCurrentFileProgressValue);
            _lblDownloadSpeedValue = FindViewById<TextView>(Resource.Id.syncDownload_lblDownloadSpeedValue);            
            _lblErrorsValue = FindViewById<TextView>(Resource.Id.syncDownload_lblErrorsValue); 
            _lblFilesDownloadedValue = FindViewById<TextView>(Resource.Id.syncDownload_lblFilesDownloadedValue);            
            _lblTotalFilesValue = FindViewById<TextView>(Resource.Id.syncDownload_lblTotalFilesValue);
            
            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            ((AndroidNavigationManager)_navigationManager).SetSyncDownloadActivityInstance(this);
        }

        protected override void OnStart()
        {
            Console.WriteLine("SyncDownloadActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("SyncDownloadActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("SyncDownloadActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("SyncDownloadActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("SyncDownloadActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("SyncDownloadActivity - OnDestroy");
            base.OnDestroy();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    ConfirmExitActivity();
                    return true;

                    //var intent = new Intent(this, typeof (MainActivity));
                    //intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                    //this.StartActivity(intent);
                    //this.Finish();
                    //return true;
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
                    break;
            }        
        }

        public override void OnBackPressed()
        {
            ConfirmExitActivity();
        }

        private void ConfirmExitActivity()
        {
            AlertDialog ad = new AlertDialog.Builder(this)
                .SetIconAttribute(global::Android.Resource.Attribute.AlertDialogIcon)
                .SetTitle("Sync download will be canceled")
                .SetMessage("Are you sure you wish to exit this screen and cancel the download?")
                .SetCancelable(true)
                .SetPositiveButton("OK", (sender, args) => {
                    OnCancelDownload();
                    Finish();
                })
                .SetNegativeButton("Cancel", (sender, args) => {})
                .Create();
            ad.Show();
        }

        #region ISyncDownloadView implementation

        public Action OnCancelDownload { get; set; }

        public void SyncDownloadError(Exception ex)
        {
            RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in SyncDownload: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        public void RefreshDevice(SyncDevice device)
        {
            RunOnUiThread(() =>
            {
                ActionBar.Title = "Syncing Library With " + device.Name;
            });
        }

        public void RefreshStatus(SyncClientDownloadAudioFileProgressEntity entity)
        {
            RunOnUiThread(() =>
            {
                //progressView.Progress = entity.PercentageDone/100f;
                _lblTitle.Text = entity.Status;
                _lblCompletedValue.Text = string.Format("{0:0.0}%", entity.PercentageDone);
                _lblCurrentFileProgressValue.Text = string.Format("{0:0.0}%", entity.DownloadPercentageDone);
                _lblFilesDownloadedValue.Text = string.Format("{0}", entity.FilesDownloaded);
                _lblTotalFilesValue.Text = string.Format("{0}", entity.TotalFiles);
                _lblDownloadSpeedValue.Text = entity.DownloadSpeed;
                _lblErrorsValue.Text = string.Format("{0}", entity.Errors);
                _lblFileName.Text = entity.DownloadFileName;
                //textViewLog.Text = entity.Log;
            });
        }

        public void SyncCompleted()
        {
            RunOnUiThread(() => {
                _lblTitle.Text = "Sync completed";
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage("Sync has completed successfully.");
                ad.SetButton("OK", (sender, args) => {
                    ad.Dismiss();
                    Finish();
                });
                ad.Show();
            });
        }

        #endregion
    }
}
