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

namespace MPfm.GenericControls.Wrappers
{
    /// <summary>
    /// This interface is wrapper around a native scrollbar control; it does not have generic rendering or mouse interaction.
    /// This is used inside generic controls to control scrolling.
    /// </summary>
    public interface IVerticalScrollBarWrapper
    {
        event ScrollValueChanged OnScrollValueChanged;

        int Value { get; set; }
        int Maximum { get; set; }
        int LargeChange { get; set; }
    }
}