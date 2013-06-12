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
using MPfm.Library.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Views
{
	/// <summary>
	/// Sync menu view interface.
	/// </summary>
	public interface ISyncMenuView : IBaseView
	{
        Action<SyncMenuItemEntity> OnExpandItem { get; set; }
        Action<SyncMenuItemEntity> OnSelectItem { get; set; }
        Action OnSync { get; set; }
        Action OnSelectButtonClick { get; set; }

        void SyncMenuError(Exception ex);
        void RefreshLoading(bool isLoading, int progressPercentage);
        void RefreshSelectButton(string text);
        void RefreshItems(List<SyncMenuItemEntity> items);
        void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace);
        void InsertItems(int index, List<SyncMenuItemEntity> items);
        void RemoveItems(int index, int count);
	}
}
