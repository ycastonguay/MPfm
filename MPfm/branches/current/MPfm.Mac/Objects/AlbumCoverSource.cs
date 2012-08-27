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
using Ninject;
using MPfm.MVP;
using MPfm.Sound;

namespace MPfm.Mac
{
    /// <summary>
    /// Album Cover table view source.
    /// </summary>
    public class AlbumCoverSource : NSTableViewSource
    {
        List<MPfmAlbumCoverView> views = new List<MPfmAlbumCoverView>();
        ISongBrowserPresenter songBrowserPresenter;
        List<IGrouping<string, SongBrowserItem>> groups;

        public List<SongBrowserItem> Items { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.Mac.AlbumCoverSource"/> class.
        /// </summary>
        public AlbumCoverSource(ISongBrowserPresenter songBrowserPresenter, IEnumerable<AudioFile> audioFiles)
        {
            this.songBrowserPresenter = songBrowserPresenter;           

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
            MPfmAlbumCoverView view = (MPfmAlbumCoverView)tableView.MakeView("albumCoverView", this);
            view.SetItem(groups[row].ToList()[0]);
            return view;
        }       
    }
}