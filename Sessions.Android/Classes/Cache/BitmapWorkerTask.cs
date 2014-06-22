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
using Android.Widget;
using Sessions.Android.Classes.Fragments;
using Sessions.Android.Classes.Helpers;

namespace Sessions.Android.Classes.Cache
{
    public class BitmapWorkerTask : AsyncTask<string, int, Bitmap>
    {
        private WeakReference _imageViewReference;
        private MobileLibraryBrowserFragment _fragment;
        
        public string Key { get; private set; }

        public BitmapWorkerTask(ImageView imageView, MobileLibraryBrowserFragment fragment)
        {
            _imageViewReference = new WeakReference(imageView);
            _fragment = fragment;
        }

        //protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] native_parms)
        //{
        //    Console.WriteLine("*************************** BitmapWorkerTask - DOINBACKGROUND");
        //    Key = native_parms[0].ToString();
        //    byte[] bytes = _fragment.OnRequestAlbumArtSynchronously(string.Empty, Key);
        //    Bitmap bitmap = BitmapHelper.DecodeFromByteArray(bytes, 400, 400);
        //    return bitmap;

        //    //_fragment.BitmapCache.LoadBitmapFromByteArray(bytes, Key, imageView);
        //    //return base.DoInBackground(native_parms);
        //}

        protected override Bitmap RunInBackground(params string[] theParams)
        {
            string artistName = (string)theParams[0];
            string albumTitle = (string)theParams[1];
            Console.WriteLine("*************************** BitmapWorkerTask - RUNINBACKGROUND - Artist: {0} Album: {1}", artistName, albumTitle);
            //byte[] bytes = _fragment.OnRequestAlbumArtSynchronously(artistName, albumTitle);
            //Bitmap bitmap = BitmapHelper.DecodeFromByteArray(bytes, 400, 400);
            //return bitmap;
            return null;
        }

        protected override void OnPostExecute(Bitmap bitmap)
        {
            //base.OnPostExecute(result);
            if (_imageViewReference != null && bitmap != null)
            {
                ImageView imageView = (ImageView)_imageViewReference.Target;
                if(imageView != null)
                    imageView.SetImageBitmap(bitmap);
            }
        }
    }
}
