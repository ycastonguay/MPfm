// Copyri3w2qght © 2011-2013 Yanick Castonguay
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
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Interaction;
using Sessions.GenericControls.Wrappers;

namespace Sessions.GenericControls.Controls.Base
{
    /// <summary>
    /// Base class for controls.
    /// </summary>
    public abstract class ControlBase : IControl
    {
        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;

        public BasicRectangle Frame { get; set; }

        protected ControlBase()
        {
            Frame = new BasicRectangle();
        }

        protected void InvalidateVisual()
        {
            if (OnInvalidateVisual != null)
                OnInvalidateVisual();
        }

        protected void InvalidateVisualInRect(BasicRectangle rect)
        {
            if (OnInvalidateVisualInRect != null)
                OnInvalidateVisualInRect(rect);
        }

        public virtual void Render(IGraphicsContext context)
        {
            Frame = new BasicRectangle(context.BoundsWidth, context.BoundsHeight);
        }
    }
}
