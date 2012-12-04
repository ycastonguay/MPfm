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

namespace MPfm.Mac
{
    /// <summary>
    /// Navigation manager for Mac.
    /// </summary>
    public class MacNavigationManager : NavigationManager
    {
        public override void Start()
        {
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate
                {
                    base.Start();
                });
            }
        }
        
        public override void CreateSplashWindow()
        {
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate
                {
                    base.CreateSplashWindow();
                });
            }
        }
        
        public override void CreateMainWindow()
        {
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate
                {
                    base.CreateMainWindow();
                }); 
            }
        }
        
        public override void CreatePreferencesWindow()
        {
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(delegate
                {
                    base.CreatePreferencesWindow();
                }); 
            }
        }
    }
}
