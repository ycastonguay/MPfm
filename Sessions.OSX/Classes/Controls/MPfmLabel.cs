// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Sessions.OSX.Classes.Helpers;
using Sessions.OSX.Classes.Objects;
using MonoMac.CoreImage;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsLabel")]
    public class SessionsLabel : NSTextField
    {
        public delegate void LabelClicked(SessionsLabel label);
        public event LabelClicked OnLabelClicked;

        [Export("init")]
        public SessionsLabel() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsLabel(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        [Export("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            if(OnLabelClicked != null && Enabled)
                OnLabelClicked(this);
        }
    }
}
