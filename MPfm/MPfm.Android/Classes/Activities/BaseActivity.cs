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
using Android.OS;
using MPfm.MVP.Views;
using org.sessionsapp.android;

namespace MPfm.Android
{
    [Activity(Icon = "@drawable/icon")]
    public class BaseActivity : Activity, IBaseView
    {
        protected Action<IBaseView> OnViewReady { get; set; }
        public Action<IBaseView> OnViewDestroy { get; set; }
        public void ShowView(bool shown)
        {
            // Ignore on Android
        }

        public BaseActivity()
        {
        }

        public BaseActivity(Action<IBaseView> onViewReady)
        {
            this.OnViewReady = onViewReady;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (OnViewDestroy != null) OnViewDestroy(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            
            // Start the widget service that will run in background when the activities are closed
            if (!IsNotificationServiceRunning())
            {
                Console.WriteLine("BaseActivity - Starting notification service...");
                Intent intent = new Intent(this, typeof(NotificationService));
                StartService(intent);
            }
        }

        protected bool IsNotificationServiceRunning()
        {
            ActivityManager manager = (ActivityManager)GetSystemService(ActivityService);
            var services = manager.GetRunningServices(int.MaxValue);
            foreach (ActivityManager.RunningServiceInfo serviceInfo in services)
            {
                Console.WriteLine("BaseActivity - IsNotificationServiceRunning - serviceInfo className: {0} started: {1} isForeground: {2}", serviceInfo.Service.ClassName, serviceInfo.Started, serviceInfo.Foreground);
                if (serviceInfo.Service.ClassName == "org.sessionsapp.android.NotificationService")
                    if (serviceInfo.Started)
                        return true;
                    else
                        return false;
            }

            return false;
        }
    }
}

