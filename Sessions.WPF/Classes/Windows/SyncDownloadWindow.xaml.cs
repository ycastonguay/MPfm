// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Sessions.WPF.Classes.Windows.Base;
using Sessions.Library.Objects;
using Sessions.MVP.Views;

namespace Sessions.WPF.Classes.Windows
{
    public partial class SyncDownloadWindow : BaseWindow, ISyncDownloadView
    {
        public SyncDownloadWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }


        #region ISyncDownloadView implementation

        public Action OnCancelDownload { get; set; }

        public void SyncDownloadError(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in SyncDownload: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
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
