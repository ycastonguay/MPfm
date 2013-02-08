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
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.MVP;
using MPfm.MVP.Presenters.Interfaces;

namespace MPfm.Mac
{
    /// <summary>
    /// Main Window: Song Position slider.
    /// </summary>
    [Register("SongPositionSlider")]
    public class SongPositionSlider : NSSlider
    {
        private IPlayerPresenter playerPresenter = null;

        private bool isMouseDown = false;

        [Export("init")]
        public SongPositionSlider() : base(NSObjectFlag.Empty)
        {
            playerPresenter = Bootstrapper.GetContainer().Resolve<IPlayerPresenter>();
            this.Continuous = true;
        }

        // Called when created from unmanaged code
        public SongPositionSlider(IntPtr handle) : base (handle)
        {
            playerPresenter = Bootstrapper.GetContainer().Resolve<IPlayerPresenter>();
            this.Continuous = true;
        }

        [Export("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            // Set flag
            isMouseDown = true;

            base.MouseDown(theEvent);

            // Call mouse up 
            this.MouseUp(theEvent);
        }

        [Export("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            // Call super class
            base.MouseUp(theEvent);

            // Get value
            float value = this.FloatValue;

            // Set flag
            isMouseDown = false;

            // Set player position
            playerPresenter.SetPosition(value / 100);
        }

        [Export("didChangeValue:")]
        public override void DidChangeValue(string forKey)
        {
            base.DidChangeValue(forKey);
        }       

        public void SetPosition(float position)
        {
            if(!isMouseDown)
                this.FloatValue = position;
        }
    }
}

