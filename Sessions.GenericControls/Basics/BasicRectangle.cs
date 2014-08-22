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

namespace Sessions.GenericControls.Basics
{
    public class BasicRectangle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public float Top { get { return Y; } }
        public float Bottom { get { return Y + Height; } }
        public float Left { get { return X; } }
        public float Right { get { return X + Width; } }

        public BasicRectangle()
        {
        }

        public BasicRectangle(float width, float height)
        {
            X = 0;
            Y = 0;
            Width = width;
            Height = height;
        }

        public BasicRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public BasicPoint Center()
        {
            return new BasicPoint(X + Width / 2, Y + Height / 2);
        }

        public void Merge(BasicRectangle rect)
        {
            var mergedRect = BasicRectangle.Merge(this, rect);
            this.X = mergedRect.X;
            this.Y = mergedRect.Y;
            this.Width = mergedRect.Width;
            this.Height = mergedRect.Height;
        }

        public static BasicRectangle Merge(BasicRectangle rectA, BasicRectangle rectB)
        {
            var mergedRect = new BasicRectangle();
            mergedRect.X = Math.Min(rectA.X, rectB.X);
            mergedRect.Y = Math.Min(rectA.Y, rectB.Y);
            mergedRect.Width = Math.Max(rectA.Right, rectB.Right) - mergedRect.X;
            mergedRect.Height = Math.Max(rectA.Bottom, rectB.Bottom) - mergedRect.Y;
            return mergedRect;
        }

        public override string ToString()
        {
            return string.Format("(x:{0:0.0}, y:{1:0.0}, w:{2:0.0}, h:{3:0.0})", X, Y, Width, Height);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(BasicRectangle))
                return false;

            var rect = obj as BasicRectangle;
            return rect.X == X && rect.Y == Y && rect.Width == Width && rect.Height == Height;
        }
    }
}