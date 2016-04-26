// Copyright � 2011-2013 Yanick Castonguay
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

namespace Sessions.GenericControls.Basics
{
    public class BasicGradientBrush : BasicBrush
    {
        public BasicColor Color2 { get; set; }
        public float Angle { get; set; }
        public BasicPoint StartPoint { get; set; }
        public BasicPoint EndPoint { get; set; }

        public BasicGradientBrush()
        {
        }

        public BasicGradientBrush(BasicColor color, BasicColor color2, float angle) : base(color)
        {
            Color2 = color2;
            Angle = angle;
            StartPoint = new BasicPoint(0, 0);
            EndPoint = new BasicPoint(0, 0);
        }

        public BasicGradientBrush(BasicColor color, BasicColor color2, BasicPoint startPoint, BasicPoint endPoint)
            : base(color)
        {
            Color2 = color2;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }
}