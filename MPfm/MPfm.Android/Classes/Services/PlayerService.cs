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
using System.Threading.Tasks;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Cache;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using TinyMessenger;

namespace MPfm.Android.Classes.Services
{
    [Service(Name = ".PlayerService", Label = "Sessions Music Player Service", Process = ":.player.process")]    
    public class PlayerService : Service
    {
        private ITinyMessengerHub _messengerHub;
        private IPlayerService _playerService;

        public override void OnStart(Intent intent, int startId)
        {
            Console.WriteLine(">>>>>>>>>>> PlayerService - OnStart - startId: {0}", startId);
            base.OnStart(intent, startId);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Console.WriteLine(">>>>>>>>>> PlayerService - OnStartCommand - startId: {0}", startId);
            return StartCommandResult.Sticky;
        }

        public override void OnCreate()
        {
            Console.WriteLine(">>>>>>>>>> PlayerService - ONCREATE");
            Initialize();
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            Console.WriteLine(">>>>>>>>>> PlayerService - DESTROY");
            base.OnDestroy();
        }

        private void Initialize()
        {
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _playerService = Bootstrapper.GetContainer().Resolve<IPlayerService>();
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) =>
            {
            });
            _messengerHub.Subscribe<PlayerStatusMessage>((message) =>
            {
            });
        }

        public override IBinder OnBind(Intent intent)
        {
            // We don't need to bind to this service
            Console.WriteLine(">>>>>>>>>>> PlayerService - OnBind");
            return null;
        }

    }
}