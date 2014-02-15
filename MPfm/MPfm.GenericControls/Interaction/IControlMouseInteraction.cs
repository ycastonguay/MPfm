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
        void MouseDown(float x, float y, MouseButtonType button);
        void MouseUp(float x, float y, MouseButtonType button);
        void MouseDoubleClick(float x, float y, MouseButtonType button);
        void MouseMove(float x, float y, MouseButtonType button);
        void MouseLeave();
        void MouseEnter();
    }

    public enum MouseButtonType
    {
        Left = 0, Middle = 1, Right = 2
    }
}
