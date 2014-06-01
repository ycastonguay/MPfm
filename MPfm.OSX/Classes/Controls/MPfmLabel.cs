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
using System.Drawing;
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.OSX.Classes.Helpers;
using MPfm.OSX.Classes.Objects;
using MonoMac.CoreImage;

namespace MPfm.OSX.Classes.Controls
{
    [Register("MPfmLabel")]
    public class MPfmLabel : NSTextField
    {
        public delegate void LabelClicked(MPfmLabel label);
        public event LabelClicked OnLabelClicked;

        [Export("init")]
        public MPfmLabel() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmLabel(IntPtr handle) : base (handle)
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
