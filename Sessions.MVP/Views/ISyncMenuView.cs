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
using MPfm.MVP.Models;
using Sessions.Library.Objects;
using Sessions.Sound.AudioFiles;

namespace MPfm.MVP.Views
{
	/// <summary>
	/// Sync menu view interface.
	/// </summary>
	public interface ISyncMenuView : IBaseView
	{
        Action<SyncMenuItemEntity, object> OnExpandItem { get; set; }
        Action<List<SyncMenuItemEntity>> OnSelectItems { get; set; }
        Action<List<AudioFile>> OnRemoveItems { get; set; }
        Action OnSync { get; set; }
        Action OnSelectButtonClick { get; set; }
        Action OnSelectAll { get; set; }
        Action OnRemoveAll { get; set; }

        void SyncMenuError(Exception ex);
	    void SyncEmptyError(Exception ex);
	    void RefreshDevice(SyncDevice device);
        void RefreshLoading(bool isLoading, int progressPercentage);
        void RefreshSelectButton(string text);
        void RefreshItems(List<SyncMenuItemEntity> items);
        void RefreshSelection(List<AudioFile> audioFiles);
        void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace);
        void InsertItems(int index, SyncMenuItemEntity parentItem, List<SyncMenuItemEntity> items, object userData);
        void RemoveItems(int index, int count, object userData);
	}
}
