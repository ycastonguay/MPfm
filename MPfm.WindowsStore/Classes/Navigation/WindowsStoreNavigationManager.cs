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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MPfm.Library.Objects;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsStore.Classes.Pages;
using MPfm.WindowsStore.Classes.Pages.Base;

namespace MPfm.WindowsStore.Classes.Navigation
{
    public class WindowsStoreNavigationManager : MobileNavigationManager
    {
        public Rect SplashImageLocation { get; set; }

        private Action<IBaseView> _onSyncViewReady;
        private Action<IBaseView> _onSyncMenuViewReady;
        private Action<IBaseView> _onSyncDownloadViewReady;

        public override void ShowSplash(ISplashView view)
        {
            var splashPage = (SplashPage) view;
            splashPage.SetImageLocation(SplashImageLocation);
            Window.Current.Content = splashPage;
            Window.Current.Activate();
        }

        public override void HideSplash()
        {
            var mainPage = new MainPage();
            Window.Current.Content = mainPage;
        }

        public override void AddTab(MobileNavigationTabType type, string title, IBaseView view)
        {
        }

        public override void AddTab(MobileNavigationTabType type, string title, MobileLibraryBrowserType browserType, LibraryQuery query,
            IBaseView view)
        {
        }

        public override void PushTabView(MobileNavigationTabType type, IBaseView view)
        {
        }

        public override void PushTabView(MobileNavigationTabType type, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view)
        {
        }

        public override void PushDialogView(MobileDialogPresentationType presentationType, string viewTitle, IBaseView sourceView, IBaseView view)
        {
        }

        public override void PushDialogSubview(MobileDialogPresentationType presentationType, string parentViewTitle, IBaseView view)
        {
        }

        public override void PushPlayerSubview(IPlayerView playerView, IBaseView view)
        {
        }

        public override void PushPreferencesSubview(IPreferencesView preferencesView, IBaseView view)
        {
        }

        public override void NotifyMobileLibraryBrowserQueryChange(MobileNavigationTabType type, MobileLibraryBrowserType browserType,
            LibraryQuery query)
        {
        }

        protected override void CreateSyncViewInternal(Action<IBaseView> onViewReady)
        {
            _onSyncViewReady = onViewReady;
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(SyncPage));
        }

        protected override void CreateSyncMenuViewInternal(Action<IBaseView> onViewReady, SyncDevice device)
        {
            _onSyncMenuViewReady = onViewReady;
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(SyncMenuPage));
        }

        protected override void CreateSyncDownloadViewInternal(Action<IBaseView> onViewReady, SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            _onSyncDownloadViewReady = onViewReady;
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(SyncDownloadPage));
        }

        public void SetViewInstance(BasePage page)
        {
            if (page is SyncPage)
                _onSyncViewReady(page);
            else if (page is SyncMenuPage)
                _onSyncMenuViewReady(page);
            else if (page is SyncDownloadPage)
                _onSyncDownloadViewReady(page);
        }
    }
}
