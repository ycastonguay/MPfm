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
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MPfm.GenericControls.Graphics;

namespace MPfm.Mac.Classes.Controls.Graphics
{
    public class MemoryGraphicsContextWrapper : GraphicsContextWrapper, IMemoryGraphicsContext
    {
        private NSBitmapImageRep _bitmap;
        
        public MemoryGraphicsContextWrapper(CGContext context, NSBitmapImageRep bitmap, float boundsWidth, float boundsHeight) 
            : base(context, boundsWidth, boundsHeight)
        {
            _bitmap = bitmap;
        }

        public IDisposable RenderToImageInMemory()
        {
            NSImage image = null;
            InvokeOnMainThread(() => {
                Console.WriteLine("MemoryGraphicsContextWrapper - RenderToImageInMemory");
                NSGraphicsContext.GlobalRestoreGraphicsState();
                image = new NSImage(new SizeF(BoundsWidth, BoundsHeight));
                image.AddRepresentation(_bitmap);
            });
            return image;
        }
    }
}
