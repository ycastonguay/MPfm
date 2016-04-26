// Copyright � 2011-2013 Yanick Castonguay
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

namespace Sessions.GenericControls.Wrappers
{
    /// <summary>
    /// This interface is wrapper around a native scrollbar control; it does not have generic rendering or mouse interaction.
    /// This is used inside generic controls to control scrolling.
    /// </summary>
    public interface IVerticalScrollBarWrapper
    {
        event ScrollValueChanged OnScrollValueChanged;

        bool Visible { get; set; }
        bool Enabled { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        int Value { get; set; }
        int Maximum { get; set; }
        int Minimum { get; set; }
        int SmallChange { get; set; }
        int LargeChange { get; set; }
    }
}