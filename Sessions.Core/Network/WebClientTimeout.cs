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

#if !PCL && !WINDOWSSTORE

using System;
using System.Net;

namespace Sessions.Core.Network
{
    /// <summary>
    /// This class is based on WebClient and adds an easy way to customize the timeout.
    /// </summary>
    public class WebClientTimeout : WebClient
    {
        /// <summary>
        /// Timout in milliseconds.
        /// </summary>
        public int Timeout { get; set; }

        public WebClientTimeout() : this(60000) { }

        public WebClientTimeout(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            // No Timeout property for WebRequest in WP8?
#if !WINDOWS_PHONE
            if (request != null)
                request.Timeout = this.Timeout;                 
#endif
            return request;
        }
    }
}

#endif