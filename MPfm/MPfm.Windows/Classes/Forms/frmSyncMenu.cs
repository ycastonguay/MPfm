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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Library.Objects;
using MPfm.MVP.Models;
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    public partial class frmSyncMenu : BaseForm, ISyncMenuView
    {
        public frmSyncMenu(Action<IBaseView> onViewReady)
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        #region ISyncMenuView implementation

        public Action<SyncMenuItemEntity> OnExpandItem { get; set; }
        public Action<SyncMenuItemEntity> OnSelectItem { get; set; }
        public Action OnSync { get; set; }
        public Action OnSelectButtonClick { get; set; }

        public void SyncMenuError(Exception ex)
        {
        }

        public void SyncEmptyError(Exception ex)
        {
        }

        public void RefreshDevice(SyncDevice device)
        {
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
        }

        public void RefreshSelectButton(string text)
        {
        }

        public void RefreshItems(List<SyncMenuItemEntity> items)
        {
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
        }

        public void InsertItems(int index, List<SyncMenuItemEntity> items)
        {
        }

        public void RemoveItems(int index, int count)
        {
        }

        #endregion

    }
}
