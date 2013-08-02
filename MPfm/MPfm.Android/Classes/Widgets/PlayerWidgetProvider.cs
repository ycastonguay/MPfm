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
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Util;
using MPfm.Android.Classes.Services;

namespace MPfm.Android.Classes.Widgets
{
    [BroadcastReceiver(Label = "Sessions")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widget_player")]    
    public class PlayerWidgetProvider : AppWidgetProvider
    {
        private PendingIntent _pendingIntentWidgetService;
        private PendingIntent _pendingIntentClick;

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            //Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% PlayerWidgetProvider - OnUpdate - appWidgetIds.length: {0}", appWidgetIds.Length);
            //context.StartService(new Intent(context, typeof(WidgetService)));
            //PendingIntent _pendingIntentWidgetService = PendingIntent.GetService(context, 0, intent, 0);
            
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            Intent intent = new Intent(context, typeof(WidgetService));
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);
            if (_pendingIntentWidgetService == null)
                _pendingIntentWidgetService = PendingIntent.GetService(context, 0, intent, PendingIntentFlags.CancelCurrent);

            if (_pendingIntentClick == null)
                _pendingIntentClick = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            Calendar time = Calendar.Instance;
            time.Set(CalendarField.Minute, 0);
            time.Set(CalendarField.Second, 0);
            time.Set(CalendarField.Millisecond, 0);
            alarmManager.SetRepeating(AlarmType.Rtc, time.Time.Time, 1000 * 10, _pendingIntentWidgetService);
            //alarmManager.SetRepeating(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime(), 1000, _pendingIntentWidgetService);

            for (int a = 0; a < appWidgetIds.Length; a++)
            {
                int appWidgetId = appWidgetIds[a];
                RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.WidgetPlayer);
                views.SetOnClickPendingIntent(Resource.Id.widgetPlayer, _pendingIntentClick);
                appWidgetManager.UpdateAppWidget(appWidgetId, views);
            }
        }

        public override void OnEnabled(Context context)
        {
            //Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% PlayerWidgetProvider - OnEnabled");
            base.OnEnabled(context);
        }

        public override void OnDisabled(Context context)
        {
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% PlayerWidgetProvider - OnDisabled");
            AlarmManager alarmManager = (AlarmManager) context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(_pendingIntentWidgetService);
            base.OnDisabled(context);
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            //Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% PlayerWidgetProvider - OnDeleted");
            base.OnDeleted(context, appWidgetIds);
        }

        //public override void OnReceive(Context context, Intent intent)
        //{
        //    Console.WriteLine("PlayerWidgetProvider - OnReceive");
        //    base.OnReceive(context, intent);
        //    Console.WriteLine("PlayerWidgetProvider - OnReceive - intent.action: {0}", intent.Action);
        //}
    }
}