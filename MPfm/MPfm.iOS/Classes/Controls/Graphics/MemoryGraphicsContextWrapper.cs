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
using MonoTouch.CoreGraphics;
using MPfm.GenericControls.Graphics;
using MonoTouch.UIKit;
using System.Drawing;

namespace MPfm.iOS.Classes.Controls.Graphics
{
	public class MemoryGraphicsContextWrapper : GraphicsContextWrapper, IMemoryGraphicsContext
	{
		public MemoryGraphicsContextWrapper(CGContext context, float boundsWidth, float boundsHeight) 
			: base(context, boundsWidth, boundsHeight)
		{
		}

		public object RenderToImageInMemory()
		{
			// Weird, no reference to the context?
			//Console.WriteLine("MemoryGraphicsContextWrapper - RenderToImageInMemory");
			var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
			return image;
		}
    }
}
