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
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.MVP.Models;
using MPfm.Library;
using MPfm.MVP.Navigation;
using MPfm.MVP.Bootstrap;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Sync menu view presenter.
	/// </summary>
    public class SyncMenuPresenter : BasePresenter<ISyncMenuView>, ISyncMenuPresenter
	{
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly NavigationManager _navigationManager;
        readonly ISyncClientService _syncClientService;
        readonly ISyncDeviceSpecifications _syncDeviceSpecifications;

	    SyncDevice _device;
        List<SyncMenuItemEntity> _items = new List<SyncMenuItemEntity>();
        List<AudioFile> _audioFilesToSync = new List<AudioFile>();

        public SyncMenuPresenter(ISyncClientService syncClientService, ISyncDeviceSpecifications syncDeviceSpecifications)
		{
            _syncClientService = syncClientService;
            _syncDeviceSpecifications = syncDeviceSpecifications;
            _syncClientService.OnDownloadIndexProgress += HandleOnDownloadIndexProgress;
            _syncClientService.OnReceivedIndex += HandleOnReceivedIndex;

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
		}

        public override void BindView(ISyncMenuView view)
        {
            view.OnSelectItems = SelectItems;
            view.OnRemoveItems = RemoveItems;
            view.OnExpandItem = ExpandItem;
            view.OnSync = Sync;
            view.OnSelectButtonClick = SelectButtonClick;
            view.OnSelectAll = SelectAll;
            view.OnRemoveAll = RemoveAll;
            base.BindView(view);

            Initialize();
        }

	    private void Initialize()
        {

        }

        private void HandleOnReceivedIndex(Exception exception)
        {
            try
            {
                if(exception != null)
                {
                    View.SyncMenuError(exception);
                    return;
                }

                // TODO: Check if this takes much memory with Instruments.
                Tracing.Log("SyncMenuPresenter - HandleOnReceivedIndex - Creating list of items...");
                var artistNames = _syncClientService.GetDistinctArtistNames();
                _items = new List<SyncMenuItemEntity>();
                foreach (string artistName in artistNames)
                    _items.Add(new SyncMenuItemEntity(){
                        ItemType = SyncMenuItemEntityType.Artist,
                        ArtistName = artistName
                    });

                Tracing.Log("SyncMenuPresenter - HandleOnReceivedIndex - Refrehsing view and sync totals...");
                View.RefreshLoading(false, 0);
                RefreshSyncTotal();
                RefreshItems();
            }
            catch(Exception ex)
            {
                Tracing.Log("SyncMenuPresenter - HandleOnReceivedIndex - Exception: {0}", ex);
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
                Tracing.Log("SyncMenuPresenter - HandleOnDownloadIndexProgress - Exception: {0}", ex);
            }
        }

        private void SelectButtonClick()
        {
            try
            {
                if(_audioFilesToSync.Count > 0)
                    RemoveAll();
                else
                    SelectAll();
            }
            catch(Exception ex)
            {
                Tracing.Log("SyncMenuPresenter - SelectButton - Exception: {0}", ex);
            }
        }

        private void Sync()
        {
            try
            {
                if (_audioFilesToSync.Count == 0)
                {
                    View.SyncEmptyError(new Exception("You must select at least one file to continue!"));
                    return;
                }

#if IOS || ANDROID
                _mobileNavigationManager.CreateSyncDownloadView(_device, _audioFilesToSync);
#else
                _navigationManager.CreateSyncDownloadView(_device, _audioFilesToSync);
#endif
            }
            catch(Exception ex)
            {
                Tracing.Log("SyncMenuPresenter - Sync - Exception: {0}", ex);
            }
        }

        private void SelectItems(List<SyncMenuItemEntity> items)
        {
#if IOS || ANDROID
            SelectItemsMobile(items);
#else
            SelectItemsDesktop(items);
#endif
        }

        private void SelectItemsDesktop(List<SyncMenuItemEntity> items)
        {
            // The main difference between desktop and mobile is that mobile uses a list view and desktop uses a tree view.
            // This makes a difference by removing selections directly on the artist/albums/songs rather than on a list view.
            try
            {
                foreach(var item in items)
                {
                    if(item.ItemType == SyncMenuItemEntityType.Artist)
                    {
                        // Add all songs from artist
                        var songsToAdd = _syncClientService.GetAudioFiles(item.ArtistName);
                        foreach(var songToAdd in songsToAdd)
                            if(!_audioFilesToSync.Contains(songToAdd))
                                _audioFilesToSync.Add(songToAdd);

                        // Update items
                        var itemsToUpdate = _items.Where(x => x.ArtistName == item.ArtistName).ToList();
                        foreach(var itemToUpdate in itemsToUpdate)
                            itemToUpdate.Selection = StateSelectionType.Selected;
                    }
                    else if(item.ItemType == SyncMenuItemEntityType.Album)
                    {
                        // Select the whole album
                        item.Selection = StateSelectionType.Selected;
                        var songsToAdd = _syncClientService.GetAudioFiles(item.ArtistName, item.AlbumTitle);
                        foreach(var songToAdd in songsToAdd)
                            if(!_audioFilesToSync.Contains(songToAdd))
                                _audioFilesToSync.Add(songToAdd);

                        // Update song selection
                        var itemsToSelect = _items.Where(x => x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle).ToList();
                        foreach(var itemToSelect in itemsToSelect)
                            itemToSelect.Selection = StateSelectionType.Selected;

                        // Update artist selection
                        var selection = IsArtistSelected(item.ArtistName);
                        var itemArtist = _items.FirstOrDefault(x => x.ArtistName == item.ArtistName && x.ItemType == SyncMenuItemEntityType.Artist);
                        itemArtist.Selection = selection;
                    }
                    else if(item.ItemType == SyncMenuItemEntityType.Song)
                    {
                        // Update song selection
                        if(item.Selection == StateSelectionType.Selected)
                            item.Selection = StateSelectionType.None;
                        else
                            item.Selection = StateSelectionType.Selected;

                        if(!_audioFilesToSync.Contains(item.Song))
                            _audioFilesToSync.Add(item.Song);

                        // Update artist selection
                        var selectionArtist = IsArtistSelected(item.ArtistName);
                        var itemArtist = _items.FirstOrDefault(x => x.ArtistName == item.ArtistName && x.ItemType == SyncMenuItemEntityType.Artist);
                        itemArtist.Selection = selectionArtist;

                        // Update album selection
                        var selectionAlbum = IsAlbumSelected(item.ArtistName, item.AlbumTitle);
                        var itemAlbum = _items.FirstOrDefault(x => x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle && x.ItemType == SyncMenuItemEntityType.Album);
                        if(itemAlbum != null)
                            itemAlbum.Selection = selectionAlbum;
                    }
                }

                RefreshSyncTotal();
                View.RefreshSelection(_audioFilesToSync);
            }
            catch(Exception ex)
            {
                Tracing.Log("SyncMenuPresenter - SelectItemDesktop - Exception: {0}", ex);
            }
        }

        private void SelectItemsMobile(List<SyncMenuItemEntity> items)
        {
            try
            {
                foreach(var item in items)
                {
                    if(item.ItemType == SyncMenuItemEntityType.Artist)
                    {
                        var selection = IsArtistSelected(item.ArtistName);
                        if(selection == StateSelectionType.None)
                        {
                            // Add all songs from artist
                            var songsToAdd = _syncClientService.GetAudioFiles(item.ArtistName);
                            foreach(var songToAdd in songsToAdd)
                                if(!_audioFilesToSync.Contains(songToAdd))
                                    _audioFilesToSync.Add(songToAdd);

                            // Update items
                            var itemsToUpdate = _items.Where(x => x.ArtistName == item.ArtistName).ToList();
                            foreach(var itemToUpdate in itemsToUpdate)
                                itemToUpdate.Selection = StateSelectionType.Selected;
                        }
                        else
                        {
                            // Remove all songs from artist
                            _audioFilesToSync.RemoveAll(x => x.ArtistName == item.ArtistName);

                            // Update items
                            var itemsToUpdate = _items.Where(x => x.ArtistName == item.ArtistName).ToList();
                            foreach(var itemToUpdate in itemsToUpdate)
                                itemToUpdate.Selection = StateSelectionType.None;
                        }
                    }
                    else if(item.ItemType == SyncMenuItemEntityType.Album)
                    {
                        // Determine if at least one song of this album is already selected
                        var songsToSync = _audioFilesToSync.Where(x => x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle).ToList();
                        if(songsToSync == null || songsToSync.Count == 0)
                        {
                            // Select the whole album
                            item.Selection = StateSelectionType.Selected;
                            var songsToAdd = _syncClientService.GetAudioFiles(item.ArtistName, item.AlbumTitle);
                            foreach(var songToAdd in songsToAdd)
                                if(!_audioFilesToSync.Contains(songToAdd))
                                    _audioFilesToSync.Add(songToAdd);

                            // Update song selection
                            var itemsToSelect = _items.Where(x => x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle).ToList();
                            foreach(var itemToSelect in itemsToSelect)
                                itemToSelect.Selection = StateSelectionType.Selected;
                        }
                        else
                        {
                            // Deselect the album
                            item.Selection = StateSelectionType.None;
                            foreach(var song in songsToSync)
                                _audioFilesToSync.Remove(song);

                            // Update song selection
                            var itemsToSelect = _items.Where(x => x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle).ToList();
                            foreach(var itemToSelect in itemsToSelect)
                                itemToSelect.Selection = StateSelectionType.None;
                        }

                        // Update artist selection
                        var selection = IsArtistSelected(item.ArtistName);
                        var itemArtist = _items.FirstOrDefault(x => x.ArtistName == item.ArtistName && x.ItemType == SyncMenuItemEntityType.Artist);
                        itemArtist.Selection = selection;
                    }
                    else if(item.ItemType == SyncMenuItemEntityType.Song)
                    {
                        // Update song selection
                        if(item.Selection == StateSelectionType.Selected)
                            item.Selection = StateSelectionType.None;
                        else
                            item.Selection = StateSelectionType.Selected;

                        if(_audioFilesToSync.Contains(item.Song))
                            _audioFilesToSync.Remove(item.Song);
                        else
                            _audioFilesToSync.Add(item.Song);

                        // Update artist selection
                        var selectionArtist = IsArtistSelected(item.ArtistName);
                        var itemArtist = _items.FirstOrDefault(x => x.ArtistName == item.ArtistName && x.ItemType == SyncMenuItemEntityType.Artist);
                        itemArtist.Selection = selectionArtist;

                        // Update album selection
                        var selectionAlbum = IsAlbumSelected(item.ArtistName, item.AlbumTitle);
                        var itemAlbum = _items.FirstOrDefault(x => x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle && x.ItemType == SyncMenuItemEntityType.Album);
                        if(itemAlbum != null)
                            itemAlbum.Selection = selectionAlbum;
                    }
                }

                RefreshSyncTotal();
                RefreshItems();
            }
            catch(Exception ex)
            {
                Tracing.Log("SyncMenuPresenter - SelectItemsMobile - Exception: {0}", ex);
            }
        }       

        private void RemoveItems(List<AudioFile> audioFiles)
        {
            foreach(var audioFile in audioFiles)
                _audioFilesToSync.Remove(audioFile);
            View.RefreshSelection(_audioFilesToSync);
            RefreshSyncTotal();
        }

        private void RemoveAll()
        {
            // Reset selection
            foreach (var item in _items)
                item.Selection = StateSelectionType.None;
            _audioFilesToSync.Clear();
            View.RefreshSelectButton("Select all");

            // Refresh view
            //RefreshItems();
            View.RefreshSelection(_audioFilesToSync);
            RefreshSyncTotal();
        }

        private void SelectAll()
        {
            // Select all items
            foreach (var item in _items)
                item.Selection = StateSelectionType.Selected;
            _audioFilesToSync.AddRange(_syncClientService.GetAudioFiles());
            View.RefreshSelectButton("Reset selection");

            // Refresh view
            //RefreshItems();
            View.RefreshSelection(_audioFilesToSync);
            RefreshSyncTotal();
        }

        private void ExpandItem(SyncMenuItemEntity item, object userData)
        {
            try
            {
                switch(item.ItemType)
                {
                    case SyncMenuItemEntityType.Artist:
                        if(item.IsExpanded)
                        {
                            int index = _items.FindIndex(x => x.ItemType == SyncMenuItemEntityType.Album && x.ArtistName == item.ArtistName);
                            int lastIndex = _items.FindLastIndex(x => x.ItemType == SyncMenuItemEntityType.Album || x.ItemType == SyncMenuItemEntityType.Song && 
                                                                       x.ArtistName == item.ArtistName);
                            if(index == -1 || lastIndex == -1)
                                return;

                            View.RemoveItems(index, lastIndex - index + 1, userData);
                        }
                        else
                        {
                            int index = _items.FindIndex(x => x.ItemType == SyncMenuItemEntityType.Artist && x.ArtistName == item.ArtistName);
                            if(index == -1)
                                return;

                            var items = new List<SyncMenuItemEntity>();
                            var albumTitles = _syncClientService.GetDistinctAlbumTitles(item.ArtistName);
                            foreach(string albumTitle in albumTitles)
                            {
                                var selection = IsAlbumSelected(item.ArtistName, albumTitle);
                                items.Add(new SyncMenuItemEntity(){
                                    ItemType = SyncMenuItemEntityType.Album,
                                    ArtistName = item.ArtistName,
                                    AlbumTitle = albumTitle,
                                    Selection = selection
                                });
                            }

                            //_items.InsertRange(index, items);
                            View.InsertItems(index + 1, item, items, userData);
                        }
                        break;
                    case SyncMenuItemEntityType.Album:
                        if(item.IsExpanded)
                        {
                            int index = _items.FindIndex(x => x.ItemType == SyncMenuItemEntityType.Song && x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle);
                            int lastIndex = _items.FindLastIndex(x => x.ItemType == SyncMenuItemEntityType.Song && x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle);
                            if(index == -1 || lastIndex == -1)
                                return;

                            View.RemoveItems(index, lastIndex - index + 1, userData);
                        }
                        else
                        {
                            int index = _items.FindIndex(x => x.ItemType == SyncMenuItemEntityType.Album && x.ArtistName == item.ArtistName && x.AlbumTitle == item.AlbumTitle);

                            // Fails on Mac because of subitems. is this check necessary?
                            //if(index == -1)
                            //    return;

                            var items = new List<SyncMenuItemEntity>();
                            var songs = _syncClientService.GetAudioFiles(item.ArtistName, item.AlbumTitle);
                            foreach(AudioFile song in songs)
                            {
                                int selectionCount = _audioFilesToSync.Count(x => x.Id == song.Id);
                                items.Add(new SyncMenuItemEntity(){
                                    ItemType = SyncMenuItemEntityType.Song,
                                    ArtistName = item.ArtistName,
                                    AlbumTitle = item.AlbumTitle,
                                    Song = song,
                                    Selection = (selectionCount == 0) ? StateSelectionType.None : StateSelectionType.Selected
                                });
                            }
                            View.InsertItems(index + 1, item, items, userData);
                        }
                        break;
                    case SyncMenuItemEntityType.Song:
                        break;
                }

                item.IsExpanded = !item.IsExpanded;
            }
            catch(Exception ex)
            {
                Tracing.Log("SyncMenuPresenter - ExpandItem - Exception: {0}", ex);
            }
        }

        private StateSelectionType IsArtistSelected(string artistName)
        {
            StateSelectionType selection = StateSelectionType.None;
            int audioFilesSelected = _audioFilesToSync.Count(x => x.ArtistName == artistName);
            var audioFilesArtist = _syncClientService.GetAudioFiles(artistName);

            if (audioFilesSelected == 0)  
                selection = StateSelectionType.None;
            else if (audioFilesSelected != audioFilesArtist.Count)                                
                selection = StateSelectionType.PartlySelected;
            else if (audioFilesSelected == audioFilesArtist.Count)
                selection = StateSelectionType.Selected;

            return selection;
        }

        private StateSelectionType IsAlbumSelected(string artistName, string albumTitle)
        {
            StateSelectionType selection = StateSelectionType.None;
            int audioFilesSelected = _audioFilesToSync.Count(x => x.ArtistName == artistName && x.AlbumTitle == albumTitle);
            var audioFilesAlbum = _syncClientService.GetAudioFiles(artistName, albumTitle);

            if (audioFilesSelected == 0)  
                selection = StateSelectionType.None;
            else if (audioFilesSelected != audioFilesAlbum.Count)                                
                selection = StateSelectionType.PartlySelected;
            else if (audioFilesSelected == audioFilesAlbum.Count)
                selection = StateSelectionType.Selected;

            return selection;
        }

        public void SetSyncDevice(SyncDevice device)
        {
            try
            {
                _device = device;
                _audioFilesToSync.Clear();
                _syncClientService.DownloadIndex(_device.Url);
                View.RefreshDevice(device);
                View.RefreshLoading(true, 0);
            }
            catch(Exception ex)
            {
                Tracing.Log("SyncMenuPresenter - SetUrl - Exception: {0}", ex);
            }
        }

        private void RefreshItems()
        {
            try
            {
                View.RefreshSelection(_audioFilesToSync);
                View.RefreshItems(_items);
            }
            catch (Exception ex)
            {
                Tracing.Log("SyncMenuPresenter - RefreshItems - Exception: {0}", ex);
            }
        }

        private void RefreshSyncTotal()
        {
            try
            {
                long totalSize = 0;
                long freeSpace = _syncDeviceSpecifications.GetFreeSpace();
                foreach(var audioFile in _audioFilesToSync)
                    totalSize += audioFile.FileSize;

                string title = string.Format("Total: {0} files ({1:0.0} MB)", _audioFilesToSync.Count, ((float)totalSize / 1048576f));
                string subtitle = string.Format("Free space: {0:0.0} MB", ((float)freeSpace / 1048576f));
                View.RefreshSyncTotal(title, subtitle, (totalSize < freeSpace));
            }
            catch(Exception ex)
            {
                Tracing.Log("SyncMenuPresenter - RefreshSyncTotal - Exception: {0}", ex);
            }
        }
    }
}
