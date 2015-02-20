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
using Sessions.Core.Attributes;
using Sessions.Sound.AudioFiles;
using org.sessionsapp.player;

namespace Sessions.Player.Objects
{
    public class Loop
    {   
        public Guid LoopId { get; set; }
        public Guid AudioFileId { get; set; }
        public string Name { get; set; }

        [DatabaseField(false)]
        public string Index { get; set; }

        [DatabaseField(false)]
        public List<Segment> Segments { get; set; }

        [DatabaseField(false)]
        public int SegmentCount {
            get { return Segments.Count; }
            set { /* for WPF */ }
        }

        [DatabaseField(false)]
        public string StartPosition
        {
            get
            {
                var segment = GetStartSegment();
                if (segment != null)
                    return segment.Position;

                return "0:00.000";
            }
            set { /* for WPF */ }
        }

        [DatabaseField(false)]
        public string EndPosition
        {
            get
            {
                var segment = GetEndSegment();
                if (segment != null)
                    return segment.Position;

                return "0:00.000";
            }
            set { /* for WPF */ }
        }

        [DatabaseField(false)]
        public string TotalLength
        {
            get
            {
                var startSegment = GetStartSegment();
                var endSegment = GetEndSegment();
                if(startSegment == null || endSegment == null)
                    return "0:00.000";

                var msStart = ConvertAudio.ToMS(startSegment.Position);
                var msEnd = ConvertAudio.ToMS(endSegment.Position);
                long length = msEnd - msStart;
                return ConvertAudio.ToTimeString(length);
            }
            set { /* for WPF */ }
        }

        public Loop()
        {
            LoopId = Guid.NewGuid();
            AudioFileId = Guid.Empty;
            Name = string.Empty;
            Segments = new List<Segment>();
        }

        public void CreateStartEndSegments()
        {
            Segments.Clear();
            Segments.Add(new Segment(0) { LoopId = this.LoopId });
            Segments.Add(new Segment(1) { LoopId = this.LoopId });
        }

        public Segment GetStartSegment()
        {
            if (Segments.Count > 0)
                return Segments[0];

            return null;
        }

        public Segment GetEndSegment()
        {
            if (Segments.Count > 1)
                return Segments[1];

            return null;
        }

        public Segment GetNextSegment(int index)
        {
            if (Segments.Count == 0)
                return null;
            else if (Segments.Count == 1)
                return Segments[0];

            int newIndex = index + 1;
            if (index + 1 > Segments.Count - 1)
                newIndex = 0;
            return Segments[newIndex];
        }

        public Segment GetNextSegmentForPlayback(int index)
        {
            long positionBytes = Segments[index].PositionBytes;
            int segmentIndex = index;

            while (true)
            {
                if (segmentIndex < Segments.Count - 1)
                    segmentIndex++;
                else if (segmentIndex == index)
                    break;
                else
                    segmentIndex = 0;

                if (Segments[segmentIndex].PositionBytes > positionBytes)
                    break;
            }

            return Segments[segmentIndex];
        }

        public SSP_LOOP ToSSPLoop()
        {
            var loop = new SSP_LOOP();
            loop.startPosition = GetStartSegment().PositionBytes;
            loop.endPosition = GetEndSegment().PositionBytes;
            return loop;
        }
    }
}
