﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Graphics;

namespace Sessions.WPF.Classes.Controls.Graphics
{
    public class MemoryGraphicsContextWrapper : GraphicsContextWrapper, IMemoryGraphicsContext
    {
        private readonly DrawingVisual _drawingVisual;

        public MemoryGraphicsContextWrapper(DrawingVisual drawingVisual, DrawingContext context, float boundsWidth, float boundsHeight) 
            : base(context, boundsWidth, boundsHeight, new BasicRectangle(0, 0, boundsWidth, boundsHeight))
        {
            _drawingVisual = drawingVisual;            
        }

        public IBasicImage RenderToImageInMemory()
        {
            float density = 1;
            float dpi = 96;
            using (var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                dpi = g.DpiX;
                density = g.DpiX/96f;
            }
            var bitmap = new RenderTargetBitmap((int)(BoundsWidth * density), (int)(BoundsHeight * density), dpi, dpi, PixelFormats.Default);
            bitmap.Render(_drawingVisual);
            bitmap.Freeze();
            var image = new BasicImage(bitmap);
            return image;
        }
    }
}