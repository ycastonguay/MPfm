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
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Helpers;
using MPfm.iOS.Managers;
using MPfm.iOS.Managers.Events;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormScrollView")]
    public class MPfmWaveFormScrollView : UIScrollView
    {
        private float _zoomScale;
        private float _offsetRatio;

        // TODO: Make this entirely private and add methods to set wave forms
        public MPfmWaveFormView WaveFormView { get; private set; }

        public MPfmWaveFormScrollView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        public MPfmWaveFormScrollView(RectangleF frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            MinimumZoomScale = 1.0f;
            MaximumZoomScale = 8.0f;
            ShowsHorizontalScrollIndicator = true;
            ShowsVerticalScrollIndicator = false;
            AlwaysBounceHorizontal = false;
            BouncesZoom = true;
            BackgroundColor = GlobalTheme.BackgroundColor;

            UITapGestureRecognizer doubleTap = new UITapGestureRecognizer((recognizer) => {
                _zoomScale = 1.0f;
                UpdateZoomScale();
            });
            doubleTap.DelaysTouchesBegan = true;
            doubleTap.NumberOfTapsRequired = 2;
            AddGestureRecognizer(doubleTap);

            WaveFormView = new MPfmWaveFormView(Bounds);
            AddSubview(WaveFormView);

            ContentSize = WaveFormView.Bounds.Size;

            this.ViewForZoomingInScrollView = delegate {
                _offsetRatio = (ContentOffset.X / ContentSize.Width);
                return WaveFormView;
            };

//            this.ZoomingStarted += delegate {
//                Console.WriteLine("MPfmWaveFormScrollView - ZoomingStarted");
//            };

            this.ZoomingEnded += delegate(object sender, ZoomingEndedEventArgs e) {
                //Console.WriteLine("MPfmWaveFormScrollView - ZoomingEnded");
                WaveFormView.RefreshWaveFormBitmap();
            };

            this.DidZoom += delegate(object sender, EventArgs e) {
                UpdateZoomScale();
            };
        }

        private void UpdateZoomScale()
        {
            var originalZoomScale = ZoomScale;
            _zoomScale *= ZoomScale;
            _zoomScale = (_zoomScale < MinimumZoomScale) ? MinimumZoomScale : _zoomScale;
            _zoomScale = (_zoomScale > MaximumZoomScale) ? MaximumZoomScale : _zoomScale;
            ZoomScale = 1.0f;
            //Console.WriteLine("MPfmWaveFormScrollView - DidZoom ZoomScale: " + originalZoomScale.ToString() + " _zoomScale: " + _zoomScale.ToString());
            
            WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, 320 * _zoomScale, WaveFormView.Frame.Height);
            ContentSize = new SizeF(WaveFormView.Frame.Width, Bounds.Height);
            ContentOffset = new PointF(WaveFormView.Frame.Width * _offsetRatio, 0);
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            WaveFormView.Frame = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
            ContentSize = Bounds.Size;
            ContentOffset = new PointF(0, 0);
            WaveFormView.LoadPeakFile(audioFile);
        }
    }
}
