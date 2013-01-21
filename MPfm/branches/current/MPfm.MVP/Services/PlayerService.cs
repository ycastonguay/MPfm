//
// PlayerService.cs: Service used for interacting with a single instance of a Player.
//
// Copyright © 2011-2012 Yanick Castonguay
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

using MPfm.MVP.Messages;
using MPfm.MVP.Services.Interfaces;
using MPfm.Sound.Bass.Net;
using MPfm.Player;
using TinyMessenger;

namespace MPfm.MVP.Services
{
    /// <summary>
    /// Service used for interacting with a single instance of a Player.
    /// </summary>
    public class PlayerService : IPlayerService
    {
        readonly ITinyMessengerHub messageHub;

        public PlayerStatusType Status { get; private set; }
        public IPlayer Player { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerService"/> class.
        /// </summary>
		public PlayerService(ITinyMessengerHub messageHub)
		{
            this.messageHub = messageHub;
		}

        private void UpdatePlayerStatus(PlayerStatusType status)
        {
            this.Status = status;
            messageHub.PublishAsync(new PlayerStatusMessage(this){
                Status = status
            });
        }

        public void Initialize(Device device, int sampleRate, int bufferSize, int updatePeriod)
        {
            // Initialize player
            Player = new MPfm.Player.Player(device, sampleRate, bufferSize, updatePeriod, true);
        }

        public void Play()
        {

        }

        /// <summary>
        /// Releases all resource used by the <see cref="PlayerService"/> object.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Dispose"/> when you are finished using the <see cref="PlayerService"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="PlayerService"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="PlayerService"/>
        /// so the garbage collector can reclaim the memory that the <see cref="PlayerService"/> was occupying.
        /// </remarks>
        public void Dispose()
        {
            // Dispose player
            if (Player != null)
            {
                Player.Dispose();
                Player = null;
            }
        }
    }
}
