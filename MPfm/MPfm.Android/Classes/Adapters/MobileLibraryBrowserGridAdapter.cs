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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments;
using MPfm.MVP.Models;
using org.sessionsapp.android;

namespace MPfm.Android.Classes.Adapters
{
    public class MobileLibraryBrowserGridAdapter : BaseAdapter<LibraryBrowserEntity>
    {
        readonly Activity _context;
        MobileLibraryBrowserFragment _fragment;
        GridView _gridView;
        List<LibraryBrowserEntity> _items;

        public MobileLibraryBrowserGridAdapter(Activity context, MobileLibraryBrowserFragment fragment, GridView gridView, List<LibraryBrowserEntity> items)
        {
            _context = context;
            _fragment = fragment;
            _gridView = gridView;
            _items = items;
        }

        public void SetData(IEnumerable<LibraryBrowserEntity> items)
        {
            _items = items.ToList();
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override LibraryBrowserEntity this[int position]
        {
            get { return _items[position]; }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {            
            //Console.WriteLine(">>>>>>>>> MobileLibraryBrowserGridAdapter - GetView - position: {0}", position);
            var mainActivity = (MainActivity)_context;
            var item = _items[position];
            string key = item.Query.ArtistName + "_" + item.Query.AlbumTitle;
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.AlbumCell, null);

            var artistName = view.FindViewById<TextView>(Resource.Id.albumCell_artistName);
            var albumTitle = view.FindViewById<TextView>(Resource.Id.albumCell_albumTitle);
            var imageView = view.FindViewById<ImageView>(Resource.Id.albumCell_image);
            artistName.Text = _items[position].Title;
            albumTitle.Text = _items[position].Subtitle;

            //if (imageView.Drawable != null)
            //{
            //    // Check if the bitmap is still in memory cache
            //    //if (!mainActivity.BitmapCache.KeyExists(key))
            //    //{
            //        // Recycle the bitmap ONLY when the bitmap has been cleared from the memory cache, or the application will crash!
            //        imageView.Drawable.Callback = null;
            //        Bitmap bitmap = ((BitmapDrawable) imageView.Drawable).Bitmap;
            //        if (bitmap != null)
            //        {
            //            mainActivity.BitmapCache.Remove(key);
            //            Console.WriteLine("##############>> MobileLibraryBrowserGridAdapter - Recycling bitmap key {0}", key);
            //            //bitmap.Recycle();
            //            bitmap.Dispose();
            //            bitmap = null;
            //        }
            //    //}
            //}
            imageView.SetImageBitmap(null);

            // Try to limit the crazy Android code that spams requests for views that aren't even visible!
            if (position < _gridView.FirstVisiblePosition - 2) // -2 == number of columns
            {
                //Console.WriteLine("#################> MLBGA - GetView - View doesn't seem to be visible!!! position: {0} firstVisiblePosition: {1}", position, _gridView.FirstVisiblePosition);
                return view;
            }

            // Check if bitmap is in cache before requesting album art (Android likes to request GetView extremely often for no good reason)
            //Console.WriteLine(">>>>>>>>> MobileLibraryBrowserGridAdapter - Loading album art - position: {0} artistName: {1} albumTitle: {2}", position, _items[position].Query.ArtistName, _items[position].Query.AlbumTitle);
            if(mainActivity.BitmapCache.KeyExists(key))
            {
                //Console.WriteLine(">>>>>>>>> MobileLibraryBrowserGridAdapter - Getting album art from cache - position: {0} artistName: {1} albumTitle: {2}", position, _items[position].Query.ArtistName, _items[position].Query.AlbumTitle);
                imageView.SetImageBitmap(mainActivity.BitmapCache.GetBitmapFromMemoryCache(key));                
            }
            else
            {
                Task.Factory.StartNew(() => {
                    //Console.WriteLine(">>>>>>>>> MobileLibraryBrowserGridAdapter - Requesting album art from presenter - position: {0} artistName: {1} albumTitle: {2}", position, _items[position].Query.ArtistName, _items[position].Query.AlbumTitle);
                    _fragment.OnRequestAlbumArt(item.Query.ArtistName, item.Query.AlbumTitle);
                });
            }

            return view;
        }

        public void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData)
        {
            try
            {
                var mainActivity = (MainActivity)_context;
                int index = _items.FindIndex(x => x.Query.ArtistName == artistName && x.Query.AlbumTitle == albumTitle);
                //Console.WriteLine(">>>>>>>>>MobileLibraryBrowserGridAdapter - *RECEIVED* album art for {0}/{1} - index: {2}", artistName, albumTitle, index);
                if (index >= 0)
                {
                    // Another Android WTF: GetChildAt index 0 == first visible cell index, not the cell index!
                    int visibleCellIndex = index - _gridView.FirstVisiblePosition;
                    var view = _gridView.GetChildAt(visibleCellIndex);
                    //Console.WriteLine(">>>>>>>>>MobileLibraryBrowserGridAdapter - *RECEIVED* album art for {0}/{1} - index: {2} visibleCellIndex: {3} firstVisiblePosition: {4}", artistName, albumTitle, index, visibleCellIndex, _gridView.FirstVisiblePosition);
                    if (view != null)
                    {
                        Task.Factory.StartNew(() => {
                            //Console.WriteLine(">>>>>>>>>MobileLibraryBrowserGridAdapter - *LOADING BITMAP* from byte array for {0}/{1} - Index found: {2}", artistName, albumTitle, index);
                            var image = view.FindViewById<ImageView>(Resource.Id.albumCell_image);
                            mainActivity.BitmapCache.LoadBitmapFromByteArray(albumArtData, artistName + "_" + albumTitle, image);
                        });
                    }
                    else
                    {
                        //Console.WriteLine(">>>>>>>>>MobileLibraryBrowserGridAdapter - *GRID VIEW CHILD IS NULL* for {0}/{1} - Index found: {2}", artistName, albumTitle, index);
                    }
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine("MobileLibraryBrowserGridAdapter - Failed to load album art: {0}", ex);
            }
        }
    }
}
