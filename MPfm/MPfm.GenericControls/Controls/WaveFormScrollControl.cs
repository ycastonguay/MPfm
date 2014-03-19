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
using System.Data.SqlClient;
using System.Linq;
using MPfm.Core;
using MPfm.GenericControls.Interaction;
using MPfm.MVP.Bootstrap;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Managers;
using MPfm.GenericControls.Managers.Events;

namespace MPfm.GenericControls.Controls
{
    public class WaveFormScrollControl : IControl, IControlMouseInteraction
    {
        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;

        public WaveFormScrollControl()
        {
            Initialize();
        }

        private void Initialize()
        {
            OnInvalidateVisual += () => { };
            OnInvalidateVisualInRect += (rect) => { };

            // How do we manage this? We don't actually draw anything, this control contains two children (wave form + scale).
            // So this isn't really a IControl, this might be a IPanel or something like that.
            // Need a wrapper for adding/managing child controls and their frame

            // What work needs to be done by this class?
            // 1) Intercept all interaction and give it to children
            //    i.e. selecting a new position vs scrolling the wave form
            // 2) Manage content offset and size;
            //    the children don't actually move, but the offset/zoom is updated in both controls 
            // 3) Update/receive data from scroll bar wrapper
            // 4) Manage frame/child controls resize

            // Is it more complicated to make this generic than a simple implementation on each platform?
        }

        public void Render(IGraphicsContext context)
        {
        }

        public void MouseDown(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
        }

        public void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
        }

        public void MouseClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
        }

        public void MouseDoubleClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
        }

        public void MouseMove(float x, float y, MouseButtonType button)
        {
        }

        public void MouseLeave()
        {
        }

        public void MouseEnter()
        {
        }

        public void MouseWheel(float delta)
        {
        }
    }
}