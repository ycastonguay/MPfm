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
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments;
using MPfm.MVP.Models;

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
            Console.WriteLine(">>>>>>>>> MobileLibraryBrowserGridAdapter - GetView - position: {0}", position);
            var item = _items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.AlbumCell, null);

            var artistName = view.FindViewById<TextView>(Resource.Id.albumCell_artistName);
            var albumTitle = view.FindViewById<TextView>(Resource.Id.albumCell_albumTitle);
            var image = view.FindViewById<ImageView>(Resource.Id.albumCell_image);

            artistName.Text = _items[position].Title;
            albumTitle.Text = _items[position].Subtitle;
            image.SetBackgroundColor(Color.White);

            Task.Factory.StartNew(() => {
                // WTF Android #549381: GetView position 0 gets called extremely often for no reason. Another job well done, Google. Why can't you optimize your code!?!?
                Console.WriteLine(">>>>>>>>> MobileLibraryBrowserGridAdapter - Loading album art - position: {0} artistName: {1} albumTitle: {2}", position, _items[position].Query.ArtistName, _items[position].Query.AlbumTitle);
                
                // Check if bitmap is in cache before requesting album art (Android likes to request GetView extremely often for no good reason)
                //_fragment.OnRequestAlbumArt(_items[position].Query.ArtistName, _items[position].Query.AlbumTitle);
            });

            return view;
        }

        public void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData)
        {
            try
            {
                var mainActivity = (MainActivity)_context;
                Console.WriteLine("MobileLibraryBrowserGridAdapter - Received album art for {0}/{1}", artistName, albumTitle);

                int index = _items.FindIndex(x => x.Query.ArtistName == artistName && x.Query.AlbumTitle == albumTitle);
                if (index >= 0)
                {
                    var view = _gridView.GetChildAt(index);
                    if (view != null)
                    {
                        var image = view.FindViewById<ImageView>(Resource.Id.albumCell_image);
                        mainActivity.BitmapCache.LoadBitmapFromByteArray(albumArtData, artistName + "_" + albumTitle, image);
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
