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
using System.IO;
using System.Windows;
using System.Windows.Threading;
using MPfm.Library;
using MPfm.Library.Objects;
using MPfm.Library.Services;
using MPfm.MVP.Views;

namespace MPfm.WPF.Classes.Windows.Base
{
    public class BaseWindow : Window, IBaseView
    {
        protected Action<IBaseView> OnViewReady { get; set; }
        public Action<IBaseView> OnViewDestroy { get; set; }

        public BaseWindow()
        {
        }

        public BaseWindow(Action<IBaseView> onViewReady)
        {
            OnViewReady = onViewReady;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (OnViewDestroy != null) OnViewDestroy(this);
            base.OnClosed(e);
        }

        protected void ViewIsReady()
        {
            // Bind presenter to view and show window
            OnViewReady(this);
            Show();
        }

        public void ShowView(bool shown)
        {
            // Never called on WPF
        }

        protected void ShowErrorDialog(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }
    }
}
