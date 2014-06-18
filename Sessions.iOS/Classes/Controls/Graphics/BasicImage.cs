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
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Basics;
using System.Collections.Generic;
using MPfm.iOS.Classes.Controls.Helpers;

namespace MPfm.iOS.Classes.Controls.Graphics
{
	public class BasicImage : IBasicImage
	{
		private UIImage _image = null;
		public IDisposable Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = (UIImage)value;
			}
		}

		public BasicRectangle ImageSize
		{
			get
			{
				return new BasicRectangle(0, 0, _image.Size.Width, _image.Size.Height);
			}
		}

		public BasicImage(UIImage image)
		{
			_image = image;
		}
    }
}
