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
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MPfm.GenericControls.Graphics;

namespace MPfm.WPF.Classes.Controls.Graphics
{
    public class MemoryGraphicsContextFactory : IMemoryGraphicsContextFactory
    {
        public IMemoryGraphicsContext CreateMemoryGraphicsContext(float width, float height)
        {
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            var context = new MemoryGraphicsContextWrapper(drawingVisual, drawingContext, width, height);
            return context;
        }

        public IDisposable CreateImageFromByteArray(byte[] data)
        {
            int width = 100;
            int height = 100;
            float density = 1;
            float dpi = 96;
            using (var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                dpi = g.DpiX;
                density = g.DpiX/96f;
            }
            var pixelFormat = PixelFormats.Pbgra32;
            var bytesPerPixel = (pixelFormat.BitsPerPixel + 7) / 8;
            var stride = bytesPerPixel * width;
            var bitmap = BitmapSource.Create(width, height, dpi, dpi, pixelFormat, null, data, stride);
            var disposableBitmap = new DisposableBitmap(bitmap);
            return disposableBitmap;
        }
    }
}
