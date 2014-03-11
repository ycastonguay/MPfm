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
using System.IO;
using System.Windows.Media.Imaging;
using MPfm.GenericControls.Graphics;

namespace MPfm.WPF.Classes.Controls.Graphics
{
    public class DisposableImageFactory : IDisposableImageFactory
    {
        public IDisposable CreateImageFromByteArray(byte[] data, int width, int height)
        {
            BitmapImage bitmap = null;
            try
            {
                var stream = new MemoryStream(data);
                stream.Seek(0, SeekOrigin.Begin);

                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("DisposableImageFactory - Failed to create bitmap from byte array: {0}", ex);
                return null;
            }

            var disposableBitmap = new DisposableBitmap(bitmap);
            return disposableBitmap;
        }
    }
}
