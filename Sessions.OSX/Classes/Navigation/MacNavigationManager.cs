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
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using TinyIoC;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using Sessions.Library.Objects;
using System.Collections.Generic;
using Sessions.Sound.AudioFiles;

namespace MPfm.OSX.Classes.Navigation
{
    /// <summary>
    /// Navigation manager for Mac.
    /// The idea is to make sure that any view creation is done on the main thread, or the windows will not always be visible.
    /// </summary>
    public class MacNavigationManager : NavigationManager
    {
        private ISyncView _syncView = null;

        public override ISplashView CreateSplashView()
        {
            ISplashView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateSplashView();
                });
            }
            return view;
        }
        
        public override IMainView CreateMainView()
        {
            IMainView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateMainView();
                });
            }
            return view;
        }
        
        public override IDesktopPreferencesView CreatePreferencesView()
        {
            IDesktopPreferencesView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreatePreferencesView();
                });
            }
            return view;
        }

        public override ICloudConnectView CreateCloudConnectView()
        {
            ICloudConnectView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateCloudConnectView();
                });
            }
            return view;
        }

        public override IDesktopEffectsView CreateEffectsView()
        {
            IDesktopEffectsView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateEffectsView();
                });
            }
            return view;
        }

        public override IPlaylistView CreatePlaylistView()
        {
            IPlaylistView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreatePlaylistView();
                });
            }
            return view;
        }

        public override IFirstRunView CreateFirstRunView()
        {
            IFirstRunView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateFirstRunView();
                });
            }
            return view;
        }

        public override ISyncView CreateSyncView()
        {
            ISyncView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateSyncView();
                });
            }
            return view;

//            if (_syncView == null)
//            {
//                using (var pool = new NSAutoreleasePool())
//                {
//                    pool.InvokeOnMainThread(delegate
//                    {
//                        _syncView = base.CreateSyncView();
//                    });
//                }
//            } 
//            else
//            {
//                _syncView.ShowView(true);
//            }
//            return _syncView;
        }

        public override ISyncCloudView CreateSyncCloudView()
        {
            ISyncCloudView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateSyncCloudView();
                });
            }
            return view;
        }

        public override ISyncMenuView CreateSyncMenuView(SyncDevice device)
        {
            ISyncMenuView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateSyncMenuView(device);
                });
            }
            return view;
        }

        public override ISyncDownloadView CreateSyncDownloadView(SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            ISyncDownloadView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateSyncDownloadView(device, audioFiles);
                });
            }
            return view;
        }

        public override ISyncWebBrowserView CreateSyncWebBrowserView()
        {
            ISyncWebBrowserView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateSyncWebBrowserView();
                });
            }
            return view;
        }

        public override IStartResumePlaybackView CreateStartResumePlaybackView()
        {
            IStartResumePlaybackView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateStartResumePlaybackView();
                });
            }
            return view;
        }

        public override IEditSongMetadataView CreateEditSongMetadataView(AudioFile audioFile)
        {
            IEditSongMetadataView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate {
                    view = base.CreateEditSongMetadataView(audioFile);
                });
            }
            return view;
        }
    }
}
