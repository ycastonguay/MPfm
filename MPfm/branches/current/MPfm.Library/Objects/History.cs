//
// History.cs: Object representing an audio file playback history event.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPfm.Library
{
    /// <summary>
    /// Object representing an audio file playback history event.
    /// </summary>
    public class History
    {
        /// <summary>
        /// Unique identifier for the audio file playback history event.
        /// </summary>
        public Guid HistoryId { get; set; }
        
        /// <summary>
        /// Unique identifier for the audio file related to the history event.
        /// </summary>
        public Guid AudioFileId { get; set; }

        /// <summary>
        /// History event date/time.
        /// </summary>
        public DateTime EventDateTime { get; set;}

        /// <summary>
        /// Default constructor for the History class.
        /// </summary>
        public History()
        {
            // Set default values
            HistoryId = Guid.NewGuid();
            AudioFileId = Guid.Empty;
            EventDateTime = DateTime.Now;
        }
    }
}
