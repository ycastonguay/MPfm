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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Controls;
using MPfm.Library.Objects;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsPhone.Classes.Pages;
using MPfm.WindowsPhone.Classes.Pages.Base;

namespace MPfm.WindowsPhone.Classes.Navigation
{
    public class WindowsPhoneNavigationManager : MobileNavigationManager
    {
        private Action<IBaseView> _onSyncViewReady;
        private Action<IBaseView> _onSyncWebBrowserViewReady;
        private Action<IBaseView> _onPreferencesViewReady;
        private Action<IBaseView> _onSyncMenuViewReady;
        private Action<IBaseView> _onSyncDownloadViewReady;

        public override void ShowSplash(ISplashView view)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - ShowSplash");
        }

        public override void HideSplash()
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - HideSplash");
        }

        public override void AddTab(MobileNavigationTabType type, string title, IBaseView view)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - AddTab");
        }

        public override void AddTab(MobileNavigationTabType type, string title, MobileLibraryBrowserType browserType, LibraryQuery query,
            IBaseView view)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - AddTab");
        }

        public override void PushTabView(MobileNavigationTabType type, IBaseView view)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - PushTabView");
        }

        public override void PushTabView(MobileNavigationTabType type, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - PushTabView");
        }

        public override void PushDialogView(string viewTitle, IBaseView sourceView, IBaseView view)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - PushDialogView");
        }

        public override void PushDialogSubview(string parentViewTitle, IBaseView view)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - PushDialogSubview");
        }

        public override void PushPlayerSubview(IPlayerView playerView, IBaseView view)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - PushPlayerSubview");
        }

        public override void PushPreferencesSubview(IPreferencesView preferencesView, IBaseView view)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - PushPreferencesSubview");
        }

        public override void NotifyMobileLibraryBrowserQueryChange(MobileNavigationTabType type, MobileLibraryBrowserType browserType,
            LibraryQuery query)
        {
            Debug.WriteLine("WindowsPhoneNavigationManager - NotifyMobileLibraryBrowserQueryChange");
        }

        protected override void CreateSyncViewInternal(Action<IBaseView> onViewReady)
        {
            _onSyncViewReady = onViewReady;
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri("/Classes/Pages/SyncPage.xaml", UriKind.Relative));
        }
        
        protected override void CreateSyncMenuViewInternal(Action<IBaseView> onViewReady, SyncDevice device)
        {
            _onSyncMenuViewReady = onViewReady;
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri("/Classes/Pages/SyncMenuPage.xaml", UriKind.Relative));
        }

        protected override void CreateSyncDownloadViewInternal(Action<IBaseView> onViewReady, SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            _onSyncDownloadViewReady = onViewReady;
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri("/Classes/Pages/SyncDownloadPage.xaml", UriKind.Relative));
        }

        protected override void CreateSyncWebBrowserViewInternal(Action<IBaseView> onViewReady)
        {
            _onSyncWebBrowserViewReady = onViewReady;
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri("/Classes/Pages/SyncWebBrowserPage.xaml", UriKind.Relative));
        }

        protected override void CreatePreferencesViewInternal(Action<IBaseView> onViewReady)
        {
            _onPreferencesViewReady = onViewReady;
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri("/Classes/Pages/PreferencesPage.xaml", UriKind.Relative));
        }

        public void SetViewInstance(BasePage page)
        {
            if(page is SyncPage)
                _onSyncViewReady(page);
            else if (page is SyncMenuPage)
                _onSyncMenuViewReady(page);
            else if (page is SyncDownloadPage)
                _onSyncDownloadViewReady(page);
            else if (page is SyncWebBrowserPage)
                _onSyncWebBrowserViewReady(page);
            else if (page is PreferencesPage)
                _onPreferencesViewReady(page);
        }
    }
}
