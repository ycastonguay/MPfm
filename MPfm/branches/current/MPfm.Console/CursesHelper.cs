//
// CursesHelper.cs: This is an helper class for NCurses.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Timers;
using System.Threading;
using System.Text;
using Mono;
using Mono.Terminal;

namespace MPfm.Console
{
    /// <summary>
    /// This is an helper class for NCurses.
    /// </summary>
    public static class CursesHelper
    {
        //public static void WriteStr(string str, int colorPair)
        public static void WriteStr()
        {
//            Curses.attron(Curses.ColorPair(colorPair));
//            Curses.addstr(str);
//            Curses.attroff(Curses.ColorPair(colorPair));

            Curses.attron(Curses.ColorPair(2));// | Curses.A_DIM);
            Curses.addstr("F1");
            Curses.attroff(Curses.ColorPair(2));// | Curses.A_DIM);
            Curses.attron(Curses.ColorPair(3));
            Curses.addstr("Play    ");
            Curses.attroff(Curses.ColorPair(3));
        }
    }
}
