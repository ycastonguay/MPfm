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
using MPfm.GenericControls.Controls;
using MPfm.Sound.AudioFiles;
using System.Collections.Generic;
using MPfm.Player.Objects;
using MPfm.Mac.Classes.Helpers;
using MonoMac.CoreGraphics;
using System.Drawing;

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmWaveFormScrollView")]
    public class MPfmWaveFormScrollView : NSView
    {
        public MPfmWaveFormView WaveFormView { get; private set; }
        public MPfmWaveFormScaleView WaveFormScaleView { get; private set; }
        public override bool IsFlipped { get { return true; } }

        public event WaveFormControl.ChangePosition OnChangePosition;
        public event WaveFormControl.ChangePosition OnChangeSecondaryPosition;

        [Export("init")]
        public MPfmWaveFormScrollView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmWaveFormScrollView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            PostsBoundsChangedNotifications = true;
//            NSNotificationCenter.DefaultCenter.AddObserver(NSView.NSViewFrameDidChangeNotification, (notification) =>
//            { 
//                    Console.WriteLine("WaveFormScrollView - NSViewFrameDidChangeNotification");
//                }, this);
            NSNotificationCenter.DefaultCenter.AddObserver(NSView.NSViewFrameDidChangeNotification, FrameDidChangeNotification, this);

            WaveFormView = new MPfmWaveFormView();
            WaveFormView.OnChangePosition += (position) => OnChangePosition(position);
            WaveFormView.OnChangeSecondaryPosition += (position) => OnChangeSecondaryPosition(position);
            AddSubview(WaveFormView);

            WaveFormScaleView = new MPfmWaveFormScaleView();
            AddSubview(WaveFormScaleView);

            //Console.WriteLine("WaveFormScrollView - Initialize - Bounds: {0} Frame: {1}", Bounds, Frame);
            SetFrame();
        }

        private void FrameDidChangeNotification(NSNotification notification)
        {
            //Console.WriteLine("WaveFormScrollView - NSViewFrameDidChangeNotification - Bounds: {0} Frame: {1}", Bounds, Frame);
            SetFrame();
        }

        private void SetFrame()
        {
            WaveFormScaleView.Frame = new RectangleF(0, 0, Frame.Width, 22);
            WaveFormView.Frame = new RectangleF(0, 22, Frame.Width, Frame.Height - 22);
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            WaveFormView.LoadPeakFile(audioFile);
            WaveFormScaleView.AudioFile = audioFile;
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            WaveFormView.SetWaveFormLength(lengthBytes);
            WaveFormScaleView.AudioFileLength = lengthBytes;
        }

        public void SetPosition(long position)
        {
            WaveFormView.Position = position;
        }

        public void SetSecondaryPosition(long position)
        {
            WaveFormView.SecondaryPosition = position;
        }

        public void ShowSecondaryPosition(bool show)
        {
            WaveFormView.ShowSecondaryPosition = show;
        }

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            WaveFormView.SetMarkers(markers);
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);
////            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
////            //CoreGraphicsHelper.FillRect(context, Bounds, new CGColor(0, 255, 0));
            //Console.WriteLine("WaveFormScrollView - DrawRect - Bounds: {0} Frame: {1}", Bounds, Frame);
        }
    }
}
