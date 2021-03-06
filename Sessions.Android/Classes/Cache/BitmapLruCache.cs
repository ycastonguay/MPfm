// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;

namespace Sessions.Android.Classes.Cache
{
    public class BitmapLruCache : LruCache
    {
        protected BitmapLruCache(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public BitmapLruCache(int maxSize) : base(maxSize)
        {
        }

        protected override int SizeOf(Java.Lang.Object key, Java.Lang.Object value)
        {
            // The cache size is measured in kilobytes rather than the number of items.
            var bitmap = (Bitmap) value;
            return bitmap.ByteCount / 1024;
        }
    }

}
