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
using MPfm.GenericControls.Basics;

namespace MPfm.Mac.Classes.Controls.Graphics
{
    public class BasicImage : IBasicImage
    {
        private NSImage _image = null;
        public IDisposable Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = (NSImage)value;
            }
        }

        public BasicRectangle ImageSize
        {
            get
            {
                return new BasicRectangle(0, 0, _image.Size.Width, _image.Size.Height);
            }
        }

        public BasicImage()
        {
        }

        public BasicImage(NSImage image)
        {
            _image = image;
        }
    }
}
