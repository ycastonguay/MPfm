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
using System.Windows.Media;
using System.Windows.Threading;
using Sessions.WPF.Classes.Controls.Graphics;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls;
using Control = System.Windows.Controls.Control;

namespace Sessions.WPF.Classes.Controls
{
    public class OutputMeter : Control
    {
        private readonly OutputMeterControl _control;

        public OutputMeter()
        {
            _control = new OutputMeterControl();
            _control.OnInvalidateVisual += () => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(InvalidateVisual));
            _control.OnInvalidateVisualInRect += (rect) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => 
            {
                InvalidateVisual();
                // TODO: It seems you can't invalidate a specific rect in WPF? What?
                // http://stackoverflow.com/questions/2576599/possible-to-invalidatevisual-on-a-given-region-instead-of-entire-wpf-control                                                                                                                       
            }));
        }

        public void AddWaveDataBlock(float[] waveDataLeft, float[] waveDataRight)
        {
            _control.AddWaveDataBlock(waveDataLeft, waveDataRight);
        }

        public void Reset()
        {
            _control.Reset();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            // TODO: Fix this, dirty rects in WPF is still a mystery. It was simple in WinForms, why is it hidden in WPF?
            var dirtyRect = new BasicRectangle(0, 0, (float) ActualWidth, (float) ActualHeight); 
            var wrapper = new GraphicsContextWrapper(dc, (float) ActualWidth, (float) ActualHeight, dirtyRect);
            _control.Render(wrapper);
        }   
    }
}
