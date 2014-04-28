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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.WindowsPhone.Classes.Navigation;
using MPfm.WindowsPhone.Classes.Pages.Base;

namespace MPfm.WindowsPhone.Classes.Pages
{
    public partial class PreferencesPage : BasePage, IPreferencesView
    {
        public PreferencesPage()
        {
            Debug.WriteLine("PreferencesPage - Ctor");
            InitializeComponent();
            SetTheme(LayoutRoot);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("PreferencesPage - OnNavigatedTo");
            base.OnNavigatedTo(e);

            var navigationManager = (WindowsPhoneNavigationManager)Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.SetViewInstance(this);
        }

        #region IPreferencesView implementation

        public Action<string> OnSelectItem { get; set; }

        public void RefreshItems(List<string> items)
        {
        }

        #endregion

    }
}