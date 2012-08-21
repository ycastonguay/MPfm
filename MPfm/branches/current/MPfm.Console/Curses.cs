//
// binding.cs.in: Core binding for curses.
//
// Authors:
//   Miguel de Icaza (miguel.de.icaza@gmail.com)
//
// Copyright (C) 2007 Novell (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Runtime.InteropServices;

namespace MPfm.Console
{
    public class Curses 
    {

//        [StructLayout (LayoutKind.Sequential)]
//        public struct MouseEvent {
//            public int ID;
//            public int X, Y, Z;
//            public Event ButtonState;
//        }

#region Screen initialization

        [DllImport ("libncurses.5.dylib")]
        extern static internal IntPtr initscr ();


        //[DllImport ("libncurses.5.dylib", EntryPoint="initscr")]
        //extern static internal IntPtr real_initscr ();
//        static int lines, cols;
//
//        static Window main_window;
//        
//        static public Window initscr ()
//        {
//            main_window = new Window (real_initscr ());
//            console_sharp_get_dims (out lines, out cols);
//            return main_window;
//        }
//
//        public static int Lines {   
//            get {
//                return lines;
//            }
//        }
//
//        public static int Cols {
//            get {
//                return cols;
//            }
//        }

//        static internal bool CheckWinChange ()
//        {
//            int l, c;
//            
//            console_sharp_get_dims (out l, out c);
//            if (l != lines || c != cols){
//                lines = l;
//                cols = c;
//                return true;
//            }
//            return false;
//        }
        
        [DllImport ("libncurses.5.dylib")]
        extern static public int endwin ();

        [DllImport ("libncurses.5.dylib")]
        extern static public bool isendwin ();

        //
        // Screen operations are flagged as internal, as we need to
        // catch all changes so we can update newscr, curscr, stdscr
        //
        [DllImport ("libncurses.5.dylib")]
        extern static public IntPtr internal_newterm (string type, IntPtr file_outfd, IntPtr file_infd);

        [DllImport ("libncurses.5.dylib")]
        extern static public IntPtr internal_set_term (IntPtr newscreen);

        [DllImport ("libncurses.5.dylib")]
            extern static internal void internal_delscreen (IntPtr sp);
#endregion

#region Input Options
        [DllImport ("libncurses.5.dylib")]
        extern static public int cbreak ();
        
        [DllImport ("libncurses.5.dylib")]
        extern static public int nocbreak ();
        
        [DllImport ("libncurses.5.dylib")]
        extern static public int echo ();
        
        [DllImport ("libncurses.5.dylib")]
        extern static public int noecho ();
        
        [DllImport ("libncurses.5.dylib")]
        extern static public int halfdelay (int t);

        [DllImport ("libncurses.5.dylib")]
        extern static public int raw ();

        [DllImport ("libncurses.5.dylib")]
        extern static public int noraw ();
        
        [DllImport ("libncurses.5.dylib")]
        extern static public void noqiflush ();
        
        [DllImport ("libncurses.5.dylib")]
        extern static public void qiflush ();

        [DllImport ("libncurses.5.dylib")]
        extern static public int typeahead (IntPtr fd);

        [DllImport ("libncurses.5.dylib")]
        extern static public int timeout (int delay);

        //
        // Internal, as they are exposed in Window
        //
        [DllImport ("libncurses.5.dylib")]
        extern static internal int wtimeout (IntPtr win, int delay);
           
        [DllImport ("libncurses.5.dylib")]
        extern static internal int notimeout (IntPtr win, bool bf);

        [DllImport ("libncurses.5.dylib")]
        extern static internal int keypad (IntPtr win, bool bf);
        
        [DllImport ("libncurses.5.dylib")]
        extern static internal int meta (IntPtr win, bool bf);
        
        [DllImport ("libncurses.5.dylib")]
        extern static internal int intrflush (IntPtr win, bool bf);
#endregion

#region Output Options
        [DllImport ("libncurses.5.dylib")]
        extern internal static int clearok (IntPtr win, bool bf);
        [DllImport ("libncurses.5.dylib")]
        extern internal static int idlok (IntPtr win, bool bf);
        [DllImport ("libncurses.5.dylib")]
        extern internal static void idcok (IntPtr win, bool bf);
        [DllImport ("libncurses.5.dylib")]
        extern internal static void immedok (IntPtr win, bool bf);
        [DllImport ("libncurses.5.dylib")]
        extern internal static int leaveok (IntPtr win, bool bf);
        [DllImport ("libncurses.5.dylib")]
        extern internal static int wsetscrreg (IntPtr win, int top, int bot);
        [DllImport ("libncurses.5.dylib")]
        extern internal static int scrollok (IntPtr win, bool bf);
        
        [DllImport ("libncurses.5.dylib")]
        extern public static int nl();
        [DllImport ("libncurses.5.dylib")]
        extern public static int nonl();
        [DllImport ("libncurses.5.dylib")]
        extern public static int setscrreg (int top, int bot);
        
#endregion

#region refresh functions

        [DllImport ("libncurses.5.dylib")]
        extern public static int refresh ();
        [DllImport ("libncurses.5.dylib")]
        extern public static int doupdate();

        [DllImport ("libncurses.5.dylib")]
        extern internal static int wrefresh (IntPtr win);
        [DllImport ("libncurses.5.dylib")]
        extern internal static int redrawwin (IntPtr win);
        [DllImport ("libncurses.5.dylib")]
        extern internal static int wredrawwin (IntPtr win, int beg_line, int num_lines);
        [DllImport ("libncurses.5.dylib")]
        extern internal static int wnoutrefresh (IntPtr win);
#endregion

#region Output
        [DllImport ("libncurses.5.dylib")]
        extern public static int move (int line, int col);


        [DllImport ("libncurses.5.dylib")]
        extern public static int addch (int ch);
        [DllImport ("libncurses.5.dylib")]
        extern public static int addstr (string s);

        public static int addstr (string format, params object [] args)
        {
            var s = string.Format (format, args);
            return addstr (s);
        }
        
        [DllImport ("libncurses.5.dylib")]
        extern internal static int wmove (IntPtr win, int line, int col);

        [DllImport ("libncurses.5.dylib")]
        extern internal static int waddch (IntPtr win, int ch);
#endregion

#region Attributes
        [DllImport ("libncurses.5.dylib")]
        extern public static int attron (int attrs);
        [DllImport ("libncurses.5.dylib")]
        extern public static int attroff (int attrs);
        [DllImport ("libncurses.5.dylib")]
        extern public static int attrset (int attrs);
#endregion

#region Input
        [DllImport ("libncurses.5.dylib")]
        extern internal static int getch ();

        [DllImport ("libncurses.5.dylib")]
        extern internal static int ungetch (int ch);

        [DllImport ("libncurses.5.dylib")]
        extern internal static int mvgetch (int y, int x);
#endregion
        
#region Colors
        [DllImport ("libncurses.5.dylib")]
        extern internal static bool has_colors ();

        [DllImport ("libncurses.5.dylib")]
        extern internal static int start_color ();

        [DllImport ("libncurses.5.dylib")]
        extern internal static int init_pair (short pair, short f, short b);

        [DllImport ("libncurses.5.dylib")]
        extern internal static int use_default_colors ();
        
#endregion
        
#region Mouse
#endregion

//#region Helpers
//        [DllImport ("mono-curses")]
//        internal extern static IntPtr console_sharp_get_stdscr ();
//
//        [DllImport ("mono-curses")]
//        internal extern static IntPtr console_sharp_get_curscr ();
//
//        [DllImport ("mono-curses")]
//        internal extern static void console_sharp_get_dims (out int lines, out int cols);
//
//        [DllImport ("mono-curses")]
//        internal extern static void console_sharp_sendsigtstp ();
//
//        [DllImport ("mono-curses")]
//        internal extern static Event console_sharp_mouse_mask (Event newmask, out Event oldmask);
//
//        [DllImport ("mono-curses")]
//        internal extern static uint console_sharp_getmouse (out MouseEvent ev);
//
//        [DllImport ("mono-curses")]
//        internal extern static uint console_sharp_ungetmouse (ref MouseEvent ev);
//#endregion

        // We encode ESC + char (what Alt-char generates) as 0x2000 + char
        public const int KeyAlt = 0x2000;

        static public int IsAlt (int key)
        {
            if ((key & KeyAlt) != 0)
                return key & ~KeyAlt;
            return 0;
        }
    }
}
