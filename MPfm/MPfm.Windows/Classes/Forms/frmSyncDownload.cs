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
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    public partial class frmSyncDownload : BaseForm, ISyncDownloadView
    {
        public frmSyncDownload(Action<IBaseView> onViewReady)
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        #region ISyncDownloadView implementation

        public Action OnCancelDownload { get; set; }

        public void SyncDownloadError(Exception ex)
        {
        }

        public void RefreshDevice(SyncDevice device)
        {
        }

        public void RefreshStatus(SyncClientDownloadAudioFileProgressEntity entity)
        {
        }

        public void SyncCompleted()
        {
        }

        #endregion

    }
}
