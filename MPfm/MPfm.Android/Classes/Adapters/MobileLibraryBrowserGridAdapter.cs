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
using Android.Views.Animations;
using Android.Widget;
using MPfm.Android.Classes.Fragments;
using MPfm.MVP.Models;
using org.sessionsapp.android;

namespace MPfm.Android.Classes.Adapters
{
    public class MobileLibraryBrowserGridAdapter : BaseAdapter<LibraryBrowserEntity>, View.IOnClickListener, AbsListView.IRecyclerListener
    {
        readonly Activity _context;
        readonly MobileLibraryBrowserFragment _fragment;
        GridView _gridView;
        List<LibraryBrowserEntity> _items;
        int _buttonsRowPosition;

        public MobileLibraryBrowserGridAdapter(Activity context, MobileLibraryBrowserFragment fragment, GridView gridView, List<LibraryBrowserEntity> items)
        {
            _context = context;
            _fragment = fragment;
            _gridView = gridView;
            _items = items;
            gridView.SetRecyclerListener(this);
        }

        public void SetData(IEnumerable<LibraryBrowserEntity> items)
        {
            _buttonsRowPosition = -1;
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
            //Console.WriteLine("MobileLibraryBrowserGridAdapter - GetView - position: {0}", position);
            //var mainActivity = (MainActivity)_context;
            var item = _items[position];
            string bitmapKey = item.Query.ArtistName + "_" + item.Query.AlbumTitle;
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.AlbumCell, null);

            var artistName = view.FindViewById<TextView>(Resource.Id.albumCell_artistName);
            var albumTitle = view.FindViewById<TextView>(Resource.Id.albumCell_albumTitle);
            var imageView = view.FindViewById<ImageView>(Resource.Id.albumCell_image);
            var layoutButtons = view.FindViewById<FrameLayout>(Resource.Id.albumCell_layoutButtons);
            artistName.Text = _items[position].Title;            
            albumTitle.Text = _items[position].Subtitle;            

            // Try to limit the crazy Android code that spams requests for views that aren't even visible!
            if (position < _gridView.FirstVisiblePosition - 2) // -2 == number of columns
            {
                //Console.WriteLine("MobileLibraryBrowserGridAdapter - GetView - View doesn't seem to be visible!!! position: {0} firstVisiblePosition: {1}", position, _gridView.FirstVisiblePosition);
                return view;
            }

            layoutButtons.Tag = position;
            layoutButtons.SetOnClickListener(this);
            layoutButtons.Visibility = position == _buttonsRowPosition ? ViewStates.Visible : ViewStates.Gone;

            //if (imageView.Drawable != null)
            //{
            //    // Check if the bitmap is still in memory cache
            //    string currentBitmapKey = imageView.Tag != null ? imageView.Tag.ToString() : "";
            //    if (!mainActivity.BitmapCache.KeyExists(bitmapKey))
            //    {                    
            //        Console.WriteLine("MobileLibraryBrowserGridAdapter - OH NO!!! Bitmap key {0} isn't in cache", currentBitmapKey);
            //        // Recycle the bitmap ONLY when the bitmap has been cleared from the memory cache, or the application will crash!
            //        imageView.Drawable.Callback = null;
            //        Bitmap bitmap = ((BitmapDrawable)imageView.Drawable).Bitmap;
            //        if (bitmap != null)
            //        {
            //            Console.WriteLine("MobileLibraryBrowserGridAdapter - Recycling bitmap key {0}", currentBitmapKey);
            //            //bitmap.Recycle();
            //            bitmap.Dispose();
            //            bitmap = null;
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine("MobileLibraryBrowserGridAdapter - Bitmap key {0} *IS IN CACHE*", currentBitmapKey);                    
            //    }                
            //}
            imageView.SetImageBitmap(null);

            // Check if bitmap is in cache before requesting album art (Android likes to request GetView extremely often for no good reason)
            //Console.WriteLine("MobileLibraryBrowserGridAdapter - Loading album art - position: {0} artistName: {1} albumTitle: {2}", position, _items[position].Query.ArtistName, _items[position].Query.AlbumTitle);
            //if(mainActivity.BitmapCache.KeyExists(bitmapKey))
            if(_fragment.BitmapCache.KeyExists(bitmapKey))
            {
                //Console.WriteLine("MobileLibraryBrowserGridAdapter - Getting album art from cache - position: {0} artistName: {1} albumTitle: {2}", position, _items[position].Query.ArtistName, _items[position].Query.AlbumTitle);
                //Task.Factory.StartNew(() => {
                    imageView.Tag = bitmapKey;
                    //imageView.SetImageBitmap(mainActivity.BitmapCache.GetBitmapFromMemoryCache(bitmapKey));
                    imageView.SetImageBitmap(_fragment.BitmapCache.GetBitmapFromMemoryCache(bitmapKey));
                //});
            }
            else
            {
                //Task.Factory.StartNew(() => {
                    //Console.WriteLine("MobileLibraryBrowserGridAdapter - Requesting album art from presenter - position: {0} artistName: {1} albumTitle: {2}", position, _items[position].Query.ArtistName, _items[position].Query.AlbumTitle);
                    _fragment.OnRequestAlbumArt(item.Query.ArtistName, item.Query.AlbumTitle);
                //});
            }

            return view;
        }

        public void ShowCellButtons(int position)
        {
            int oldPosition = _buttonsRowPosition;
            _buttonsRowPosition = position;

            var viewOldPosition = _gridView.GetChildAt(oldPosition - _gridView.FirstVisiblePosition);
            if (viewOldPosition != null)
            {
                var oldLayout = viewOldPosition.FindViewById<FrameLayout>(Resource.Id.albumCell_layoutButtons);
                Animation animOld = AnimationUtils.LoadAnimation(_context, Resource.Animation.fade_out);
                animOld.AnimationEnd += (sender, args) => {
                    oldLayout.Visibility = ViewStates.Gone;
                };
                oldLayout.StartAnimation(animOld);
            }

            var view = _gridView.GetChildAt(position - _gridView.FirstVisiblePosition);
            if (view == null)
                return;

            // If the user long presses on the same item, fade out buttons
            if (position == oldPosition)
                return;

            var layout = view.FindViewById<FrameLayout>(Resource.Id.albumCell_layoutButtons);
            layout.Visibility = ViewStates.Visible;
            Animation anim = AnimationUtils.LoadAnimation(_context, Resource.Animation.fade_in);
            layout.StartAnimation(anim);
        }

        public void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData)
        {
            try
            {
                //var mainActivity = (MainActivity)_context;
                int index = _items.FindIndex(x => x.Query.ArtistName == artistName && x.Query.AlbumTitle == albumTitle);
                //Console.WriteLine("MobileLibraryBrowserGridAdapter - *RECEIVED* album art for {0}/{1} - index: {2}", artistName, albumTitle, index);
                if (index >= 0)
                {
                    int visibleCellIndex = index - _gridView.FirstVisiblePosition;
                    var view = _gridView.GetChildAt(visibleCellIndex);
                    //Console.WriteLine("MobileLibraryBrowserGridAdapter - *RECEIVED* album art for {0}/{1} - index: {2} visibleCellIndex: {3} firstVisiblePosition: {4}", artistName, albumTitle, index, visibleCellIndex, _gridView.FirstVisiblePosition);
                    if (view != null)
                    {
                        //Task.Factory.StartNew(() => {
                            //Console.WriteLine("MobileLibraryBrowserGridAdapter - *LOADING BITMAP* from byte array for {0}/{1} - Index found: {2}", artistName, albumTitle, index);
                            var image = view.FindViewById<ImageView>(Resource.Id.albumCell_image);
                            image.Tag = artistName + "_" + albumTitle;
                            //mainActivity.BitmapCache.LoadBitmapFromByteArray(albumArtData, artistName + "_" + albumTitle, image);
                            _fragment.BitmapCache.LoadBitmapFromByteArray(albumArtData, artistName + "_" + albumTitle, image);
                        //});
                    }
                    else
                    {
                        //Console.WriteLine("MobileLibraryBrowserGridAdapter - *GRID VIEW CHILD IS NULL* for {0}/{1} - Index found: {2}", artistName, albumTitle, index);
                    }
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine("MobileLibraryBrowserGridAdapter - Failed to load album art: {0}", ex);
            }
        }

        public void OnClick(View v)
        {
            // Do not block click on cells that doesn't have buttons shown
            int position = (int)v.Tag;
            if (position != _buttonsRowPosition)
                return;

            int oldPosition = _buttonsRowPosition;
            _buttonsRowPosition = -1;

            var viewOldPosition = _gridView.GetChildAt(oldPosition - _gridView.FirstVisiblePosition);
            if (viewOldPosition != null)
            {
                var oldLayout = viewOldPosition.FindViewById<FrameLayout>(Resource.Id.albumCell_layoutButtons);
                Animation animOld = AnimationUtils.LoadAnimation(_context, Resource.Animation.fade_out);
                animOld.AnimationEnd += (sender, args) => {
                    oldLayout.Visibility = ViewStates.Gone;
                };
                oldLayout.StartAnimation(animOld);
            }
        }

        public void OnMovedToScrapHeap(View view)
        {
            //ImageView imageView = view.FindViewById<ImageView>(Resource.Id.albumCell_image);
            //Console.WriteLine("OnMovedtoScrapHeap view.Tag: {0} view type: {1} imageView.Tag: {2}", view.Tag != null ? view.Tag.ToString() : "null", view.GetType().FullName, imageView != null && imageView.Tag != null ? imageView.Tag : "null");
        }
    }
}
