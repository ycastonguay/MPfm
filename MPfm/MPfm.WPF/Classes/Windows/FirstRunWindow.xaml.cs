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
using System.Windows;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Config;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    public partial class FirstRunWindow : BaseWindow, IFirstRunView
    {
        public FirstRunWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void btnClose_OnClick(object sender, RoutedEventArgs e)
        {
            AppConfigManager.Instance.Root.IsFirstRun = false;
            AppConfigManager.Instance.Save();

            var navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
            navigationManager.CreateSplashView();
            Close();
        }
    }
}