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

namespace MPfm.Player.Objects
{
    public class Loop
    {   
        public Guid LoopId { get; set; }
        public Guid AudioFileId { get; set; }
        public string Name { get; set; }
        public List<Segment> Segments { get; set; }

        public Loop()
        {
            LoopId = Guid.NewGuid();
            AudioFileId = Guid.Empty;
            Name = string.Empty;
            Segments = new List<Segment>();
        }
    }
}
