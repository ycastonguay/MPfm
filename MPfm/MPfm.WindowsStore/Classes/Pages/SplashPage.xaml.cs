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
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsStore.Classes.Pages.Base;

namespace MPfm.WindowsStore.Classes.Pages
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class SplashPage : BasePage, ISplashView
    {
        private Action<IBaseView> _onViewReady;

        public SplashPage(Action<IBaseView> onViewReady)
        {
            _onViewReady = onViewReady;
            this.InitializeComponent();
        }

        public void SetImageLocation(Rect imageLocation)
        {
            // Position the extended splash screen image in the same location as the splash screen image.
            this.extendedSplashImage.SetValue(Canvas.LeftProperty, imageLocation.X);
            this.extendedSplashImage.SetValue(Canvas.TopProperty, imageLocation.Y);
            this.extendedSplashImage.Height = imageLocation.Height;
            this.extendedSplashImage.Width = imageLocation.Width;

            this.lblStatus.SetValue(Canvas.LeftProperty, imageLocation.X + (imageLocation.Width / 2) - 100);
            this.lblStatus.SetValue(Canvas.TopProperty, imageLocation.Y + imageLocation.Height + 72);

            // Position the extended splash screen's progress ring.
            this.ProgressRing.SetValue(Canvas.TopProperty, imageLocation.Y + imageLocation.Height + 12);
            this.ProgressRing.SetValue(Canvas.LeftProperty, imageLocation.X + (imageLocation.Width / 2) - 15);

            _onViewReady(this);
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                lblStatus.Text = message;
            });  
        }

        public void InitDone(bool isAppFirstStart)
        {
        }
        
        #endregion

    }
}
