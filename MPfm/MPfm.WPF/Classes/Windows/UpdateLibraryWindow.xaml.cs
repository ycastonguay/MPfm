﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Windows;
using System.Windows.Threading;
using MPfm.Library.Objects;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Views;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    public partial class UpdateLibraryWindow : BaseWindow, IUpdateLibraryView
    {
        public UpdateLibraryWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void btnOK_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            OnCancelUpdateLibrary();
        }

        private void btnSaveLog_OnClick(object sender, RoutedEventArgs e)
        {
        }

        #region IUpdateLibraryView implementation

        public Action<List<string>, List<Folder>> OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }
        public Action<string> OnSaveLog { get; set; }

        public void RefreshStatus(UpdateLibraryEntity entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblTitle.Text = entity.Title;
                lblSubtitle.Text = entity.Subtitle;
                progressBar.Value = entity.PercentageDone*100;
            }));
        }

        public void AddToLog(string entry)
        {
        }

        public void ProcessEnded(bool canceled)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblTitle.Text = "Update library completed successfully";
                lblSubtitle.Text = string.Empty;
                btnCancel.IsEnabled = false;
                btnOK.IsEnabled = true;
                btnSaveLog.IsEnabled = true;
            }));
        }

        #endregion

    }
}