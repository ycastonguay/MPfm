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
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MPfm.Core;
using MPfm.WPF.Classes.Controls.Graphics;
using Control = System.Windows.Controls.Control;

namespace MPfm.WPF.Classes.Controls
{
    public class TestControl : Control
    {
        private DrawingVisual _drawingVisual;
        private DrawingContext _drawingContext;
        private RenderTargetBitmap _bitmap;

        public TestControl()
        {
        }

        public void CreateBitmap()
        {
            int width = 100;
            int height = 100;
            int dpi = 96;

            Tracing.Log(">> CreateBitmap");
            var thread = new Thread(new ThreadStart(() => 
            {
                Tracing.Log(">> CreateBitmap - Thread start; creating drawing visual");
                //Dispatcher.Invoke(new Action(() => {
                _drawingVisual = new DrawingVisual();
                _drawingContext = _drawingVisual.RenderOpen();
                //}));

                Tracing.Log(">> CreateBitmap - Drawing to context");
                _drawingContext.DrawRectangle(new SolidColorBrush(Colors.HotPink), new Pen(), new Rect(0, 0, 50, 50));
                _drawingContext.DrawRectangle(new SolidColorBrush(Colors.Blue), new Pen(), new Rect(50, 0, 50, 50));
                _drawingContext.DrawRectangle(new SolidColorBrush(Colors.Orange), new Pen(), new Rect(0, 50, 50, 50));
                _drawingContext.DrawRectangle(new SolidColorBrush(Colors.DarkRed), new Pen(), new Rect(50, 50, 50, 50));
                _drawingContext.Close();

                Tracing.Log(">> CreateBitmap - Finished drawing; creating render target bitmap");
                _bitmap = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Default);
                _bitmap.Render(_drawingVisual);
                Tracing.Log(">> CreateBitmap - Finished work");
                _bitmap.Freeze();
            }));
            //thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_bitmap == null)
                return;

            Tracing.Log(">> CreateBitmap - OnRender - Drawing bitmap");
            dc.DrawImage(_bitmap, new Rect(0, 0, 100, 100));
        }   
    }
}
