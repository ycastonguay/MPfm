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
using Android.Content.Res;
using Android.Graphics;

namespace Sessions.Android.Classes.Helpers
{
    public static class BitmapHelper
    {
        public static Bitmap DecodeFromByteArray(byte[] bytes)
        {
            return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
        }

        public static Bitmap DecodeFromByteArray(byte[] bytes, int reqWidth, int reqHeight)
        {
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);
            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);
        }    

        public static Bitmap DecodeSampledBitmapFromResource(Resources res, int resId, int reqWidth, int reqHeight)
        {
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeResource(res, resId, options);
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);
            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeResource(res, resId, options);
        }

        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            int width = options.OutWidth;
            int height = options.OutHeight;
            int inSampleSize = 1;

            if (width > reqWidth || height > reqHeight)
            {
                int widthRatio = (int)Math.Round((double)width / (double)reqWidth);
                int heightRatio = (int)Math.Round((double)height / (double)reqHeight);

                inSampleSize = heightRatio < widthRatio ? heightRatio : widthRatio;
            }

            return inSampleSize;
        }
    }
}
