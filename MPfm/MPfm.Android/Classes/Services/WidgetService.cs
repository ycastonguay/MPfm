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
using Android.Views;
using Android.Views.Animations;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Services.Interfaces;
using TinyMessenger;

namespace MPfm.Android.Classes.Services
{
    [Service]    
    public class WidgetService : Service
    {
        private ITinyMessengerHub _messengerHub;
        private IPlayerService _playerService;
        private Context _context;

        public override void OnStart(Intent intent, int startId)
        {
            Console.WriteLine(">>>>>>>>>>> WidgetService - OnStart - startId: {0}", startId);
            Initialize();
            base.OnStart(intent, startId);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Console.WriteLine(">>>>>>>>>> WidgetService - OnStartCommand - startId: {0}", startId);
            Initialize();
            return base.OnStartCommand(intent, flags, startId);
        }

        private void Initialize()
        {
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _playerService = Bootstrapper.GetContainer().Resolve<IPlayerService>();
            //_messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) =>
            //{
            //    Console.WriteLine(">>>>>>>>>> WidgetService - PlayerPlaylistIndexChangedMessage");
            //});
            //_messengerHub.Subscribe<PlayerStatusMessage>((message) =>
            //{
            //    Console.WriteLine(">>>>>>>>>> WidgetService - PlayerStatusMessage - Status=" + message.Status.ToString());
            //});

            // Force updating the view for the first time
            UpdateView();
        }

        private void UpdateView()
        {
        }

        public override IBinder OnBind(Intent intent)
        {
            // We don't need to bind to this service
            Console.WriteLine(">>>>>>>>>>> WidgetService - OnBind");
            return null;
        }
    }
}