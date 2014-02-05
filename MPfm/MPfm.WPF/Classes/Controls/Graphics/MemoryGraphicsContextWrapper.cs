﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MPfm.GenericControls.Graphics;

namespace MPfm.WPF.Classes.Controls.Graphics
{
    public class MemoryGraphicsContextWrapper : GraphicsContextWrapper, IMemoryGraphicsContext
    {
        private readonly DrawingVisual _drawingVisual;

        public MemoryGraphicsContextWrapper(DrawingVisual drawingVisual, DrawingContext context, float boundsWidth, float boundsHeight) 
            : base(context, boundsWidth, boundsHeight)
        {
            _drawingVisual = drawingVisual;            
        }

        public IDisposable RenderToImageInMemory()
        {
            int dpi = 96;
            Console.WriteLine("MemoryGraphicsContextWrapper - RenderToImageInMemory");
            var bitmap = new RenderTargetBitmap((int) BoundsWidth, (int) BoundsHeight, dpi, dpi, PixelFormats.Default);
            Console.WriteLine("MemoryGraphicsContextWrapper - RenderToImageInMemory (2)");
            bitmap.Render(_drawingVisual);
            Console.WriteLine("MemoryGraphicsContextWrapper - RenderToImageInMemory (3)");
            bitmap.Freeze();
            Console.WriteLine("MemoryGraphicsContextWrapper - RenderToImageInMemory (4)");
            return new DisposableBitmap(bitmap);
        }
    }
}