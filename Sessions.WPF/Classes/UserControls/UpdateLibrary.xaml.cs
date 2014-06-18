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
using System.Windows;
using System.Windows.Controls;
using MPfm.Library.Objects;
using MPfm.MVP.Views;

namespace MPfm.WPF.Classes.UserControls
{
    public partial class UpdateLibrary : UserControl, IUpdateLibraryView
    {
        public UpdateLibrary(Action<IBaseView> onViewReady)
        {
            InitializeComponent();
            onViewReady(this);
        }

        private void btnOK_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void btnCancel_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void btnSaveLog_OnClick(object sender, RoutedEventArgs e)
        {
        }

        #region IUpdateLibraryView implementation

        public Action<IBaseView> OnViewDestroy { get; set; }
        public void ShowView(bool shown)
        {
            Console.WriteLine("Hello world!!!!!!!!!!!!!");
        }

        public Action OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }
        public Action<string> OnSaveLog { get; set; }

        public void RefreshStatus(UpdateLibraryEntity entity)
        {
        }

        public void AddToLog(string entry)
        {
        }

        public void ProcessStarted()
        {
        }

        public void ProcessEnded(bool canceled)
        {
        }

        #endregion
    }
}
