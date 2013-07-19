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
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Util;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;

namespace MPfm.Android.Classes.Helpers
{
    public class BitmapCache
    {
        private Activity activity;
        private LruCache memoryCache;
        public int MaxWidth { get; private set; }
        public int MaxHeight { get; private set; }

        public BitmapCache(Activity activity, int memorySize, int maxWidth, int maxHeight)
        {
            memoryCache = new LruCache(memorySize);
            this.activity = activity;
            this.MaxWidth = maxWidth;
            this.MaxHeight = maxHeight;
        }

        private void AddBitmapToMemoryCache(string key, Bitmap bitmap)
        {
            if (GetBitmapFromMemoryCache(key) == null)
            {
                memoryCache.Put(key, bitmap);
            }
        }

        private Bitmap GetBitmapFromMemoryCache(string key)
        {
            return (Bitmap)memoryCache.Get(key);
        }

        public void LoadBitmapFromByteArray(byte[] bytes, string key, ImageView imageView)
        {
            Bitmap bitmap = GetBitmapFromMemoryCache(key);
            if (bitmap != null)
            {
                imageView.SetImageBitmap(bitmap);
            }
            else
            {
                Task.Factory.StartNew(() => {
                    Console.WriteLine("BitmapCache - Adding bitmap to cache...");
                    bitmap = BitmapHelper.DecodeFromByteArray(bytes, MaxWidth, MaxHeight);
                    AddBitmapToMemoryCache(key, bitmap);
                    activity.RunOnUiThread(() => {
                        Console.WriteLine("BitmapCache - Setting album art on image view...");
                        imageView.SetImageBitmap(bitmap);
                        Animation animation = AnimationUtils.LoadAnimation(activity, Resource.Animation.fade_in);
                        imageView.StartAnimation(animation);
                    });
                });
            }
        }

        //public void LoadBitmapFromResource(int resId, ImageView imageView)
        //{
        //    string imageKey = resId.ToString();
        //    Bitmap bitmap = GetBitmapFromMemoryCache(imageKey);
        //    if (bitmap != null)
        //    {
        //        imageView.SetImageBitmap(bitmap);
        //    }
        //    else
        //    {
        //        Task.Factory.StartNew(() =>
        //            {
        //                bitmap = BitmapHelper.DecodeSampledBitmapFromResource(Resource, resId, MaxWidth, MaxHeight);
        //                AddBitmapToMemoryCache(imageKey, bitmap);
        //                activity.RunOnUiThread(() => imageView.SetImageBitmap(bitmap));
        //            });
        //    }
        //}
    }
}
