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
using Android.Graphics;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Graphics;

namespace Sessions.Android.Classes.Controls.Graphics
{
    public class MemoryGraphicsContextWrapper : GraphicsContextWrapper, IMemoryGraphicsContext
    {
        private readonly Bitmap _bitmap;

        public MemoryGraphicsContextWrapper(Canvas canvas, Bitmap bitmap, float boundsWidth, float boundsHeight, float density) 
            : base(canvas, boundsWidth, boundsHeight, density, new BasicRectangle(0, 0, boundsWidth, boundsHeight))
        {
            _bitmap = bitmap;
        }

        public IBasicImage RenderToImageInMemory()
        {
            //Console.WriteLine("MemoryGraphicsContextWrapper - RenderToImageInMemory");
            var image = new BasicImage(_bitmap);
            return image;
        }
    }
}