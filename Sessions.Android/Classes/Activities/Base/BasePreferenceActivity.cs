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
using Android.Preferences;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Messages;
using Sessions.MVP.Views;
using TinyMessenger;

namespace Sessions.Android
{
    [Activity(Icon = "@drawable/icon")]
    public class BasePreferenceActivity : PreferenceActivity, IBaseView
    {
        ITinyMessengerHub _messengerHub;
        List<TinyMessageSubscriptionToken> _tokens = new List<TinyMessageSubscriptionToken>();
        public Action<IBaseView> OnViewDestroy { get; set; }
        public void ShowView(bool shown)
        {
            // Ignore on Android
        }

        public BasePreferenceActivity()
        {
            InitializeBase();
        }

        private void InitializeBase()
        {
            Console.WriteLine("BasePreferenceActivity - InitializeBase - Subcribing to ApplicationCloseMessage; activity of type {0}", this.GetType().FullName);
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _tokens.Add(_messengerHub.Subscribe<ApplicationCloseMessage>(message =>
            {
                Console.WriteLine("BasePreferenceActivity - InitializeBase - Received ApplicationCloseMessage; closing activity of type {0}", this.GetType().FullName);
                Finish();
            }));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Console.WriteLine("BasePreferenceActivity - OnDestroy - Unsubcribing to ApplicationCloseMessage; activity of type {0}", this.GetType().FullName);
            foreach (TinyMessageSubscriptionToken token in _tokens)
                token.Dispose();

            if (OnViewDestroy != null) OnViewDestroy(this);
        }

        protected void ShowErrorDialog(Exception ex)
        {
            RunOnUiThread(() =>
            {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

    }
}
