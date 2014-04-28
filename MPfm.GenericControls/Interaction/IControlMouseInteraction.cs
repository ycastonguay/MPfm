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

namespace MPfm.GenericControls.Interaction
{
    public interface IControlMouseInteraction
    {
        void MouseDown(float x, float y, MouseButtonType button, KeysHeld keysHeld);
        void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld);
        void MouseClick(float x, float y, MouseButtonType button, KeysHeld keysHeld);
        void MouseDoubleClick(float x, float y, MouseButtonType button, KeysHeld keysHeld);
        void MouseMove(float x, float y, MouseButtonType button);
        void MouseLeave();
        void MouseEnter();
        void MouseWheel(float delta);
    }

    public enum MouseButtonType
    {
        None = 0, Left = 1, Middle = 2, Right = 3
    }

    public enum MouseCursorType
    {
        Default = 0, HSplit = 1, VSplit = 2
    }

    public class KeysHeld
    {
        public bool IsCtrlKeyHeld { get; set; }
        public bool IsAltKeyHeld { get; set; }
        public bool IsShiftKeyHeld { get; set; }
    }
}
