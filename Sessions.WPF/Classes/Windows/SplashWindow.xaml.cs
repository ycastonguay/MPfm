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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MPfm.WPF.Classes.Windows.Base;
using Sessions.MVP.Views;

namespace MPfm.WPF.Classes.Windows
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : BaseWindow, ISplashView
    {
        public SplashWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();

            imageLogo.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Splash/splash_logo_clear.png"));
            imageLogoFull.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Splash/splash_logo_full.png"));

            var anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 1;
            anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));
            anim.BeginTime = new TimeSpan(0, 0, 0, 0, 250);
            imageLogoFull.BeginAnimation(OpacityProperty, anim);
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblStatus.Content = message;
            }));
        }

        public void InitDone(bool isAppFirstStart)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(Close));
        }

        public void DestroyView()
        {
        }

        #endregion
    }
}
