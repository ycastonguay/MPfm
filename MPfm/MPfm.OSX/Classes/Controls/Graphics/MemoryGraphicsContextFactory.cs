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
using MonoMac.AppKit;
using MPfm.GenericControls.Graphics;
using MonoMac.Foundation;
using MPfm.GenericControls.Basics;

namespace MPfm.Mac.Classes.Controls.Graphics
{
    public class MemoryGraphicsContextFactory : NSObject, IMemoryGraphicsContextFactory
    {
        public IMemoryGraphicsContext CreateMemoryGraphicsContext(float width, float height)
        {
            MemoryGraphicsContextWrapper wrapper = null;
            InvokeOnMainThread(() => {
                //http://stackoverflow.com/questions/10627557/mac-os-x-drawing-into-an-offscreen-nsgraphicscontext-using-cgcontextref-c-funct
                var bitmap = new NSBitmapImageRep(IntPtr.Zero, (int)width, (int)height, 8, 4, true, false, NSColorSpace.DeviceRGB, NSBitmapFormat.AlphaFirst, 0, 0);
                var context = NSGraphicsContext.FromBitmap(bitmap);
                NSGraphicsContext.GlobalSaveGraphicsState();
                NSGraphicsContext.CurrentContext = context;
                wrapper = new MemoryGraphicsContextWrapper(context.GraphicsPort, bitmap, width, height, new BasicRectangle(0, 0, width, height));
            });
            
            return wrapper;
        }
    }
}
