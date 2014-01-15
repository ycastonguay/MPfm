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
using System.Reflection;
using Gtk;
using Pango;
using MPfm.GenericControls.Controls;
using MPfm.GTK.Classes.Controls.Graphics;

namespace MPfm.GTK.Classes.Controls
{
    public class OutputMeter : DrawingArea
    {
        private OutputMeterControl _control;

        public OutputMeter() : base()
        {
            Initialize();
        }

        public OutputMeter(IntPtr raw) : base(raw)
        {
            Initialize();
        }

        private void Initialize()
        {
            _control = new OutputMeterControl(null);
        }

        public void AddWaveDataBlock(float[] waveDataLeft, float[] waveDataRight)
        {
            _control.AddWaveDataBlock(waveDataLeft, waveDataRight);
        }

        protected override bool OnExposeEvent(Gdk.EventExpose args)
        {
            Cairo.Context cr = Gdk.CairoHelper.Create(GdkWindow);
            //var wrapper = new GraphicsContextWrapper(cr, Allocation.Width, Allocation.Height);
            //_control.Render(wrapper);

//            int width, height;
//            width = Allocation.Width;
//            height = Allocation.Height;
//
//            cr.SetSourceRGB(0.7, 0.2, 0.0);
//            cr.Rectangle(0, 0, width, height);
//            //cr.Clip();
//            cr.Fill();

//            cr.LineWidth = 9;
//            cr.SetSourceRGB(0.7, 0.2, 0.0);
//
//            cr.Translate(width/2, height/2);
//            cr.Arc(0, 0, (width < height ? width : height) / 2 - 10, 0, 2*Math.PI);
//            cr.StrokePreserve();
//
//            cr.SetSourceRGB(0.3, 0.4, 0.6);
//            cr.Fill();

            ((IDisposable) cr.GetTarget()).Dispose();                                      
            ((IDisposable) cr).Dispose();

            return true;
        }
    }
}
