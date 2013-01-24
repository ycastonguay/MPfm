using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Util;
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
                Task.Factory.StartNew(() =>
                    {
                        bitmap = BitmapHelper.DecodeFromByteArray(bytes, MaxWidth, MaxHeight);
                        AddBitmapToMemoryCache(key, bitmap);
                        activity.RunOnUiThread(() => imageView.SetImageBitmap(bitmap));
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