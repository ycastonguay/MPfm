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

namespace Sessions.GenericControls.Interaction
{
    public interface IControlKeyboardInteraction
    {
        void KeyDown(char key, SpecialKeys specialKeys, ModifierKeys modifierKeys, bool isRepeat);
        void KeyUp(char key, SpecialKeys specialKeys, ModifierKeys modifierKeys, bool isRepeat);        
    }

    public enum ModifierKeys
    {
        None = 0, Shift = 1, Alt = 2, Ctrl = 3
    }

    public enum SpecialKeys
    {
        None = 0, Enter = 1, Space = 2, Down = 3, Up = 4, Left = 5, Right = 6, PageUp = 7, PageDown = 8, Home = 9, End = 10
    }
}
