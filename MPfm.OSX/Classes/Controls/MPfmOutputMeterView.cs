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
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.OpenGL;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.GenericControls.Controls;
using MPfm.OSX.Classes.Controls.Graphics;
using MPfm.OSX.Classes.Controls.Helpers;

namespace MPfm.OSX.Classes.Controls
{
    [Register("MPfmOutputMeterView")]
    public class MPfmOutputMeterView : NSOpenGLView
    {
        private OutputMeterControl _control;

        // https://developer.apple.com/library/mac/documentation/performance/conceptual/Drawing/Articles/CocoaDrawingTips.html
        //public override bool WantsDefaultClipping { get { return false; } }
        public override bool IsOpaque { get { return true; } }
        public override bool IsFlipped { get { return true; } }

        [Export("init")]
        public MPfmOutputMeterView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmOutputMeterView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        //public MPfmOutputMeterView(IntPtr handle) : base

        private void Initialize()
        {
            //WantsLayer = true;
            _control = new OutputMeterControl();    
            _control.OnInvalidateVisual += () => InvokeOnMainThread(() => SetNeedsDisplayInRect(Bounds));
            _control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));
        }
        
        public override void DrawRect(RectangleF dirtyRect)
        {
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            TestDraw();
            GL.Flush();
            //base.DrawRect(dirtyRect);
//            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
//            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height, GenericControlHelper.ToBasicRect(dirtyRect));
//            _control.Render(wrapper);
        }

        private void TestDraw()
        {
            GL.Color3(1.0f, 0.85f, 0.35f);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex3(0.0, 0.6, 0.0);
            GL.Vertex3(-0.2, -0.3, 0.0);
            GL.Vertex3(0.2, -0.3, 0.0);
            GL.End();
        }

        public void AddWaveDataBlock(float[] waveDataLeft, float[] waveDataRight)
        {
            _control.AddWaveDataBlock(waveDataLeft, waveDataRight);
        }
    }
}
