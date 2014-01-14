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

using MPfm.GenericControls.Controls.Properties;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Interaction;

namespace MPfm.GenericControls.Controls
{
    public class OutputMeterControl : IControl
    {
        private readonly IOutputMeterProperties _properties;
        private readonly IControlInteraction _interaction;

        public OutputMeterControl(IOutputMeterProperties properties, IControlInteraction interaction)
        {
            _properties = properties;
            _interaction = interaction;
        }

        public void Render(IGraphicsContext context)
        {
            // The GC is passed every time 
            // Got to have context bitmap size
            context.DrawEllipsis();
        }
    }
}