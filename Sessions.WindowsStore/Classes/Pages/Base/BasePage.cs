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
using Windows.UI.Xaml.Navigation;
using MPfm.MVP.Views;
using MPfm.WindowsStore.Common;

namespace MPfm.WindowsStore.Classes.Pages.Base
{
    public class BasePage : LayoutAwarePage, IBaseView
    {
        public Action<IBaseView> OnViewDestroy { get; set; }

        public void ShowView(bool shown)
        {
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            OnViewDestroy(this);
            base.OnNavigatedFrom(e);
        }
    }
}
