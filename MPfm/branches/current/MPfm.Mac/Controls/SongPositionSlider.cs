//
// SongPositionSlider.cs: Main Window: Song Position slider.
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.MVP;
using Ninject;

namespace MPfm.Mac
{
    /// <summary>
    /// Main Window: Song Position slider.
    /// </summary>
    [Register("SongPositionSlider")]
    public class SongPositionSlider : NSSlider
    {
        private ILibraryBrowserPresenter libraryBrowserPresenter = null;

        [Export("init")]
        public SongPositionSlider() : base(NSObjectFlag.Empty)
        {
            libraryBrowserPresenter = Bootstrapper.GetKernel().Get<ILibraryBrowserPresenter>();
        }

        // Called when created from unmanaged code
        public SongPositionSlider(IntPtr handle) : base (handle)
        {
            libraryBrowserPresenter = Bootstrapper.GetKernel().Get<ILibraryBrowserPresenter>();
        }

        [Export("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);



            //NSView view = this.Superview;
            //view.controll

        }
    }
}

