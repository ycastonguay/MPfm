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
using Sessions.GenericControls.Graphics;
using MonoTouch.UIKit;
using System.Drawing;

namespace MPfm.iOS.Classes.Controls.Graphics
{
	public class MemoryGraphicsContextFactory : IMemoryGraphicsContextFactory
	{
		public IMemoryGraphicsContext CreateMemoryGraphicsContext(float width, float height)
		{
			//Console.WriteLine("MemoryGraphicsContextFactory - CreateMemoryGraphicsContext - width: {0} height: {1}", width, height);
			UIGraphics.BeginImageContextWithOptions(new SizeF(width, height), false, 0);
			var context = UIGraphics.GetCurrentContext();

			// Quartz2D uses a different coordinate system; the origin is in the lower left corner. Change origin to top left corner.
			context.TranslateCTM(0, height);
			context.ScaleCTM(1, -1);
			if (context == null)
			{
				Console.WriteLine("Error initializing image cache context!");
				throw new Exception("Error initializing image cache context!");
			}

			return new MemoryGraphicsContextWrapper(context, width, height);
		}
    }
}
