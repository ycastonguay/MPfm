using System;
using Android.Content.Res;
using Android.Graphics;

namespace MPfm.Android.Classes.Helpers
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