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
using System.Drawing;

namespace MPfm.Mac.Classes.Controls.Graphics
{
    public class DisposableImageFactory : NSObject, IDisposableImageFactory
    {
        public IDisposable CreateImageFromByteArray(byte[] data)
        {
            NSImage image = null;
            //InvokeOnMainThread(() => {
                Console.WriteLine("DisposableImageFactory - CreateImageFromByteArray");
                //image = new NSImage(new SizeF(BoundsWidth, BoundsHeight));
                //image.AddRepresentation(_bitmap);

                try
                {
                    NSGraphicsContext.GlobalRestoreGraphicsState();
                    using (NSData imageData = NSData.FromArray(data))
                    {
                        InvokeOnMainThread(() => {
                            image = new NSImage(imageData);
//                        using (NSImage imageFullSize = new NSImage(imageData))
//                        {
//                            if (imageFullSize != null)
//                            {
//                                try
//                                {
//                                    _currentAlbumArtKey = key;                                    
//                                    //UIImage imageResized = CoreGraphicsHelper.ScaleImage(imageFullSize, height);
//                                    //return imageResized;
//                                    image = imageFullSize;
//                                }
//                                catch (Exception ex)
//                                {
//                                    Console.WriteLine("Error resizing image " + audioFile.ArtistName + " - " + audioFile.AlbumTitle + ": " + ex.Message);
//                                }
//                            }
//                        }
                        });
                    }
                    return image;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to process image: {0}", ex);
                }
            //});
            return image;
        }
    }
}
