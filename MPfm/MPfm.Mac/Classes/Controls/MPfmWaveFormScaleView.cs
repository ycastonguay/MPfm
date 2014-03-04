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

using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.GenericControls.Controls;
using MPfm.Mac.Classes.Controls.Graphics;
using MPfm.Mac.Classes.Controls.Helpers;
using System;
using MPfm.Sound.AudioFiles;

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmWaveFormScaleView")]
    public class MPfmWaveFormScaleView : NSView
    {
        private WaveFormScaleControl _control;

        public AudioFile AudioFile
        {
            get
            {
                return _control.AudioFile;
            }
            set
            {
                _control.AudioFile = value;
            }
        }

        public long AudioFileLength
        {
            get
            {
                return _control.AudioFileLength;
            }
            set
            {
                _control.AudioFileLength = value;
            }
        }
        [Export("init")]
        public MPfmWaveFormScaleView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmWaveFormScaleView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            _control = new WaveFormScaleControl();    
            // TODO: Could these be moved inside a generic helper or something?
            _control.OnInvalidateVisual += () => {
                SetNeedsDisplayInRect(Bounds);
            };
            _control.OnInvalidateVisualInRect += (rect) => {
                SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect));
            };
        }
        
        public override void DrawRect(RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);
            
            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height);
            _control.Render(wrapper);
        }
    }
}
