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
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Android.Views.Animations;
using Android.Webkit;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Clients;
using MPfm.Android.Classes.Navigation;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.Android
{
    [Activity(Label = "About Sessions", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize, WindowSoftInputMode = SoftInput.StateHidden)]
    public class AboutActivity : BaseActivity, IAboutView
    {
        MobileNavigationManager _navigationManager;
        ProgressBar _progressBar;
        TextView _lblLoading;
        WebView _webView;
        MyWebViewClient _webViewClient;        

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("AboutActivity - OnCreate");
            base.OnCreate(bundle);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.About);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _progressBar = FindViewById<ProgressBar>(Resource.Id.about_progressBar);
            _lblLoading = FindViewById<TextView>(Resource.Id.about_lblLoading);
            _webView = FindViewById<WebView>(Resource.Id.about_webView);
            _webViewClient = new MyWebViewClient();
            _webViewClient.PageFinished += (sender, args) => {
                Animation anim = AnimationUtils.LoadAnimation(this, Resource.Animation.fade_out);
                anim.AnimationEnd += (animSender, animArgs) => {
                    _lblLoading.Visibility = ViewStates.Gone;
                };
                _lblLoading.StartAnimation(anim);

                Animation anim2 = AnimationUtils.LoadAnimation(this, Resource.Animation.fade_out);
                anim2.AnimationEnd += (animSender, animArgs) => {
                    _progressBar.Visibility = ViewStates.Gone;
                };
                _progressBar.StartAnimation(anim2);
            };
            _webView.SetWebViewClient(_webViewClient);

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            //((AndroidNavigationManager)_navigationManager).SetAboutActivityInstance(this);
            _navigationManager.BindAboutView(this);
        }

        protected override void OnStart()
        {
            Console.WriteLine("AboutActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("AboutActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("AboutActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("AboutActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("AboutActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("AboutActivity - OnDestroy");
            base.OnDestroy();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    var intent = new Intent(this, typeof (MainActivity));
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

        #region IAboutView implementation

        public void RefreshAboutContent(string version, string content)
        {
            RunOnUiThread(() => {
                _webView.LoadData(content, "text/html; charset=utf-8", "UTF-8");
            });
        }

        #endregion

    }
}
