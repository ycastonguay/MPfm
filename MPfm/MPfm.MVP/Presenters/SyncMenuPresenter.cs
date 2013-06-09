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
using MPfm.Library.Objects;
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.MVP.Models;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Sync menu view presenter.
	/// </summary>
    public class SyncMenuPresenter : BasePresenter<ISyncMenuView>, ISyncMenuPresenter
	{
        readonly ISyncClientService _syncClientService;
        string _url;
        List<SyncMenuItemEntity> _items = new List<SyncMenuItemEntity>();
        List<AudioFile> _audioFilesToSync = new List<AudioFile>();

        public SyncMenuPresenter(ISyncClientService syncClientService)
		{
            _syncClientService = syncClientService;
            _syncClientService.OnDownloadIndexProgress += HandleOnDownloadIndexProgress;
            _syncClientService.OnReceivedIndex += HandleOnReceivedIndex;
		}

        public override void BindView(ISyncMenuView view)
        {
            view.OnSelectItem = SelectItem;
            view.OnExpandItem = ExpandItem;
            base.BindView(view);

            Initialize();
        }       

        private void Initialize()
        {

        }

        private void HandleOnReceivedIndex(object sender, EventArgs e)
        {
            try
            {
                // TODO: Check if this takes much memory with Instruments.
                var artistNames = _syncClientService.GetDistinctArtistNames();
                _items = new List<SyncMenuItemEntity>();
                foreach (string artistName in artistNames)
                    _items.Add(new SyncMenuItemEntity(){
                        ItemType = SyncMenuItemEntityType.Artist,
                        ArtistName = artistName
                    });

                View.RefreshLoading(false, 0);
                View.RefreshItems(_items);
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncMenuPresenter - HandleOnReceivedIndex - Exception: {0}", ex);
            }
        }

        private void HandleOnDownloadIndexProgress(int progressPercentage, long bytesReceived, long totalBytesToReceive)
        {
            try
            {
                View.RefreshLoading(true, progressPercentage);
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncMenuPresenter - HandleOnDownloadIndexProgress - Exception: {0}", ex);
            }
        }

        private void SelectItem(SyncMenuItemEntity item)
        {
            try
            {
                if(item.ItemType == SyncMenuItemEntityType.Artist)
                {
                    // Get all audio files from artist
                    //item.IsSelected = true;
                }
                else if(item.ItemType == SyncMenuItemEntityType.Album)
                {
                    // 1) Nothing is selected; all the album is selected
                    // 2) Everything is selected; all the album is deselected
                    // 3) One or several songs of the album are selected; all the album is deselected

                    // Check if at least one song has been already selected
                    var subitems = _items.Where(x => x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle).ToList();
                    foreach(var subitem in subitems)
                    {

                    }
                }
                else if(item.ItemType == SyncMenuItemEntityType.Song)
                {
                    item.IsSelected = !item.IsSelected;
                    _audioFilesToSync.Add(item.Song);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncMenuPresenter - SelectItem - Exception: {0}", ex);
            }
        }

        private void ExpandItem(SyncMenuItemEntity item)
        {
            try
            {
                switch(item.ItemType)
                {
                    case SyncMenuItemEntityType.Artist:
                        if(item.IsExpanded)
                        {
                            int index = _items.FindIndex(x => x.ItemType == SyncMenuItemEntityType.Album && x.ArtistName == item.ArtistName);
                            int lastIndex = _items.FindLastIndex(x => x.ItemType == SyncMenuItemEntityType.Album && x.ArtistName == item.ArtistName);
                            if(index == -1 || lastIndex == -1)
                                return;

                            View.RemoveItems(index, lastIndex - index + 1);
                        }
                        else
                        {
                            int index = _items.FindIndex(x => x.ItemType == SyncMenuItemEntityType.Artist && x.ArtistName == item.ArtistName);
                            if(index == -1)
                                return;

                            var items = new List<SyncMenuItemEntity>();
                            var albumTitles = _syncClientService.GetDistinctAlbumTitles(item.ArtistName);
                            foreach(string albumTitle in albumTitles)
                                items.Add(new SyncMenuItemEntity(){
                                    ItemType = SyncMenuItemEntityType.Album,
                                    ArtistName = item.ArtistName,
                                    AlbumTitle = albumTitle
                                });

                            View.InsertItems(index + 1, items);
                        }
                        break;
                    case SyncMenuItemEntityType.Album:
                        if(item.IsExpanded)
                        {
                            int index = _items.FindIndex(x => x.ItemType == SyncMenuItemEntityType.Song && x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle);
                            int lastIndex = _items.FindLastIndex(x => x.ItemType == SyncMenuItemEntityType.Song && x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle);
                            if(index == -1 || lastIndex == -1)
                                return;

                            View.RemoveItems(index, lastIndex - index + 1);
                        }
                        else
                        {
                            int index = _items.FindIndex(x => x.ItemType == SyncMenuItemEntityType.Album && x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle);
                            if(index == -1)
                                return;

                            var items = new List<SyncMenuItemEntity>();
                            var songs = _syncClientService.GetAudioFiles(item.ArtistName, item.AlbumTitle);
                            foreach(AudioFile song in songs)
                                items.Add(new SyncMenuItemEntity(){
                                    ItemType = SyncMenuItemEntityType.Song,
                                    ArtistName = item.ArtistName,
                                    AlbumTitle = item.AlbumTitle,
                                    Song = song
                                });

                            View.InsertItems(index + 1, items);
                        }
                        break;
                    case SyncMenuItemEntityType.Song:
                        break;
                }

                item.IsExpanded = !item.IsExpanded;
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncMenuPresenter - ExpandItem - Exception: {0}", ex);
            }
        }

        public void SetUrl(string url)
        {
            try
            {
                _url = url;
                _syncClientService.DownloadIndex(url);
                View.RefreshLoading(true, 0);
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncMenuPresenter - SetUrl - Exception: {0}", ex);
            }
        }
    }
}
