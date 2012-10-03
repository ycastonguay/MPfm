//
// AlbumCoverSource.cs: Album Cover table view source.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.IO;
using System.Linq;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.Sound;
using MPfm.Core;

namespace MPfm.Mac
{
    /// <summary>
    /// Album Cover table view source.
    /// </summary>
    public class AlbumCoverSource : NSTableViewSource
    {
        public delegate AlbumCoverAsyncResponse FetchAlbumCoverDelegate(SongBrowserItem item, MPfmAlbumCoverView view);

        AlbumCoverCacheService albumCoverCacheService;
        List<IGrouping<string, SongBrowserItem>> groups;

        public List<SongBrowserItem> Items { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.Mac.AlbumCoverSource"/> class.
        /// </summary>
        public AlbumCoverSource(AlbumCoverCacheService albumCoverCacheService, IEnumerable<AudioFile> audioFiles)
        {
            this.albumCoverCacheService = albumCoverCacheService;

            // Create list of items
            Items = new List<SongBrowserItem>();
            foreach(AudioFile audioFile in audioFiles)
                Items.Add(new SongBrowserItem(audioFile));

            // Group albums
            groups = Items.GroupBy(x => x.AudioFile.AlbumTitle).ToList();
        }       

        public override float GetRowHeight(NSTableView tableView, int row)
        {
            return groups[row].Count() * 18;
        }

        public override int GetRowCount(NSTableView tableView)
        {           
            return groups.Count;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            // Create view
            MPfmAlbumCoverView view = (MPfmAlbumCoverView)tableView.MakeView("albumCoverView", this);

            // Get item for view
            SongBrowserItem item = groups[row].ToList()[0];
            Tracing.Log("AlbumCoverSource - GetViewForItem " + item.AudioFile.FilePath);
            FetchAlbumCoverDelegate fetchAlbumCoverDelegate = new FetchAlbumCoverDelegate(FetchAlbumCoverAsync);
            fetchAlbumCoverDelegate.BeginInvoke(item, view, FetchAlbumCoverAsyncCallback, fetchAlbumCoverDelegate);
            Console.WriteLine("GetViewForItem " + row.ToString());
            view.SetItem(item, null);
            return view;
        }       

        //public Tuple<SongBrowserItem, NSImage, MPfmAlbumCoverView> FetchAlbumCoverAsync(SongBrowserItem item, MPfmAlbumCoverView view)
        public AlbumCoverAsyncResponse FetchAlbumCoverAsync(SongBrowserItem item, MPfmAlbumCoverView view)
        {
            Tracing.Log("FetchAlbumCoverAsync " + item.AudioFile.FilePath);
            NSImage image = albumCoverCacheService.TryGetAlbumCover(item.AudioFile.FilePath, item.AudioFile.ArtistName, item.AudioFile.AlbumTitle);
            Tracing.Log("FetchAlbumCoverAsync - got cover " + item.AudioFile.FilePath);
            AlbumCoverAsyncResponse response = new AlbumCoverAsyncResponse(){
                Item = item,
                Image = image,
                View = view
            };
            return response;
        }
        
        void FetchAlbumCoverAsyncCallback(IAsyncResult r)
        {
            // Get result
            FetchAlbumCoverDelegate fetchAlbumCover = (FetchAlbumCoverDelegate)r.AsyncState;
            AlbumCoverAsyncResponse response = fetchAlbumCover.EndInvoke(r);

            if (response != null && response.View != null)
            {
                try
                {
                    Tracing.Log("FetchAlbumCoverAsyncCallback - Setting item " + response.Item.AudioFile.FilePath);
                    response.View.SetItem(response.Item, response.Image);
                    Tracing.Log("FetchAlbumCoverAsyncCallback - Refreshing item " + response.Item.AudioFile.FilePath);
                    if(response.Image != null)
                        response.View.SetNeedsDisplayInRect(response.View.Bounds);
                }
                catch(Exception ex)
                {
                    Tracing.Log(ex);
                }
            }
        }
    }

    public class AlbumCoverAsyncResponse
    {
        public SongBrowserItem Item { get; set; }
        public NSImage Image { get; set; }
        public MPfmAlbumCoverView View { get; set; }
    }
}