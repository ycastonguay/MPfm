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
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Sessions.WPF.Classes.Windows.Base;
using Sessions.MVP.Models;
using Sessions.MVP.Views;

namespace Sessions.WPF.Classes.Windows
{
    public partial class CloudConnectWindow : BaseWindow, ICloudConnectView
    {
        private bool _canCheckForAuthentication = false;

        public CloudConnectWindow(Action<IBaseView> onViewReady) 
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
            // Add canceling when it will be implemented
            Close();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (!_canCheckForAuthentication)
                return;

            OnCheckIfAccountIsLinked();
        }

        #region ICloudConnectView implementation

        public Action OnCheckIfAccountIsLinked { get; set; }

        public void CloudConnectError(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in SyncDownload: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void RefreshStatus(CloudConnectEntity entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                string title = string.Format("Connect to {0}", entity.CloudServiceName);
                this.Title = title;
                lblTitle.Content = title;

                _canCheckForAuthentication = entity.CurrentStep > 1;
                lblStep1.Foreground = entity.CurrentStep > 1 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Color.FromRgb(153, 153, 153));
                lblStep2.Foreground = entity.CurrentStep > 2 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Color.FromRgb(153, 153, 153));
                lblStep2B.Foreground = entity.CurrentStep > 2 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Color.FromRgb(153, 153, 153));
                lblStep3.Foreground = entity.CurrentStep > 3 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Color.FromRgb(153, 153, 153));
                lblStep4.Foreground = entity.IsAuthenticated ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Color.FromRgb(153, 153, 153));
                btnOK.IsEnabled = entity.IsAuthenticated;
                lblButtonOK.Opacity = entity.IsAuthenticated ? 1 : 0.5;
                imageButtonOK.Opacity = entity.IsAuthenticated ? 1 : 0.5;

                if (entity.IsAuthenticated)
                {
                    progressBar.IsIndeterminate = false;
                    progressBar.Value = 100;
                }
            }));
        }

        #endregion

    }
}
