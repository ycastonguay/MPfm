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
using System.Linq;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.Sound;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.iOS.Managers;
using MPfm.iOS.Managers.Events;
using MPfm.Player.Objects;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormScaleView")]
    public class MPfmWaveFormScaleView : UIView
    {
//        private CGColor _colorGradient1 = GlobalTheme.BackgroundColor.CGColor;
//        private CGColor _colorGradient2 = GlobalTheme.BackgroundColor.CGColor;
        private CGColor _colorGradient1 = UIColor.Blue.CGColor;
        private CGColor _colorGradient2 = UIColor.Blue.CGColor;

        private AudioFile _audioFile = null;
        public AudioFile AudioFile
        {
            get
            {
                return _audioFile;
            }
        }

        private long _length;
        public long Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }

        private float _zoom = 1.0f;
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
            }
        }

        public MPfmWaveFormScaleView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public MPfmWaveFormScaleView(RectangleF frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.BackgroundColor = UIColor.Blue;
        }

        public override void Draw(RectangleF rect)
        {
            // Leave empty! Actual drawing is in DrawLayer
        }

        [Export ("drawLayer:inContext:")]
        public void DrawLayer(CALayer layer, CGContext context)
        {
            CoreGraphicsHelper.FillRect(context, Bounds, _colorGradient1);
        }
    }
}
