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

using System.Windows.Media;
using MPfm.GenericControls.Controls;
using MPfm.WPF.Classes.Controls.Graphics;
using Control = System.Windows.Controls.Control;

namespace MPfm.WPF.Classes.Controls
{
    public class WaveFormScale : Control
    {
        private readonly WaveFormScaleControl _control;

        public WaveFormScale()
        {
            _control = new WaveFormScaleControl();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            var wrapper = new GraphicsContextWrapper(dc, (float) ActualWidth, (float) ActualHeight);
            _control.Render(wrapper);
        }   
    }
}
