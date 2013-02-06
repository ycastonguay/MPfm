//
// Main.cs: Main application method. Creates an NSApplication.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using MPfm.MVP;
using TinyIoC;
using MPfm.MVP.Views;

namespace MPfm.Mac
{
    /// <summary>
    /// Navigation manager for Mac.
    /// The idea is to make sure that any view creation is done on the main thread, or the windows will not always be visible.
    /// </summary>
    public class MacNavigationManager : NavigationManager
    {
        public override void BindSplashView(ISplashView view, Action onInitDone)
        {
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate
                {
                    base.BindSplashView(view, onInitDone);
                });
            }
        }

        public override ISplashView CreateSplashView()
        {
            ISplashView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate
                {
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
                pool.InvokeOnMainThread(delegate
                {
                    view = base.CreateMainView();
                });
            }
            return view;
        }

        public override IPreferencesView CreatePreferencesView()
        {
            IPreferencesView view = null;
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate
                {
                    view = base.CreatePreferencesView();
                });
            }
            return view;
        }
    }
}