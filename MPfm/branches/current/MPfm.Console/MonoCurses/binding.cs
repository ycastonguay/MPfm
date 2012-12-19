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

namespace Mono.Terminal {

	public partial class Curses {

		[StructLayout (LayoutKind.Sequential)]
		public struct MouseEvent {
			public int ID;
			public int X, Y, Z;
			public Event ButtonState;
		}

#region Screen initialization
	
		[DllImport ("libncurses.dylib", EntryPoint="initscr")]
		extern static internal IntPtr real_initscr ();
		static int lines, cols;

		static Window main_window;
		
		static public Window initscr ()
		{
			main_window = new Window (real_initscr ());
			try {
				console_sharp_get_dims (out lines, out cols);
			} catch (DllNotFoundException){
				endwin ();
				Console.Error.WriteLine ("Unable to find the libmono-curses.dylib native library\n" + 
							 "this is different than the managed mono-curses.dll\n\n" +
							 "Typically you need to install to a LD_LIBRARY_PATH directory\n" +
							 "or DYLD_LIBRARY_PATH directory or run /sbin/ldconfig");
				Environment.Exit (1);
			}
			return main_window;
		}

		public static int Lines {	
			get {
				return lines;
			}
		}

		public static int Cols {
			get {
				return cols;
			}
		}

		static internal bool CheckWinChange ()
		{
			int l, c;
			
			console_sharp_get_dims (out l, out c);
			if (l != lines || c != cols){
				lines = l;
				cols = c;
				return true;
			}
			return false;
		}
		
		[DllImport ("libncurses.dylib")]
		extern static public int endwin ();

		[DllImport ("libncurses.dylib")]
		extern static public bool isendwin ();

		//
		// Screen operations are flagged as internal, as we need to
		// catch all changes so we can update newscr, curscr, stdscr
		//
		[DllImport ("libncurses.dylib")]
		extern static public IntPtr internal_newterm (string type, IntPtr file_outfd, IntPtr file_infd);

		[DllImport ("libncurses.dylib")]
		extern static public IntPtr internal_set_term (IntPtr newscreen);

		[DllImport ("libncurses.dylib")]
	        extern static internal void internal_delscreen (IntPtr sp);
#endregion

#region Input Options
		[DllImport ("libncurses.dylib")]
		extern static public int cbreak ();
		
		[DllImport ("libncurses.dylib")]
		extern static public int nocbreak ();
		
		[DllImport ("libncurses.dylib")]
		extern static public int echo ();
		
		[DllImport ("libncurses.dylib")]
		extern static public int noecho ();
		
		[DllImport ("libncurses.dylib")]
		extern static public int halfdelay (int t);

		[DllImport ("libncurses.dylib")]
		extern static public int raw ();

		[DllImport ("libncurses.dylib")]
		extern static public int noraw ();
		
		[DllImport ("libncurses.dylib")]
		extern static public void noqiflush ();
		
		[DllImport ("libncurses.dylib")]
		extern static public void qiflush ();

		[DllImport ("libncurses.dylib")]
		extern static public int typeahead (IntPtr fd);

		[DllImport ("libncurses.dylib")]
		extern static public int timeout (int delay);

		//
		// Internal, as they are exposed in Window
		//
		[DllImport ("libncurses.dylib")]
		extern static internal int wtimeout (IntPtr win, int delay);
	       
		[DllImport ("libncurses.dylib")]
		extern static internal int notimeout (IntPtr win, bool bf);

		[DllImport ("libncurses.dylib")]
		extern static internal int keypad (IntPtr win, bool bf);
		
		[DllImport ("libncurses.dylib")]
		extern static internal int meta (IntPtr win, bool bf);
		
		[DllImport ("libncurses.dylib")]
		extern static internal int intrflush (IntPtr win, bool bf);
#endregion

#region Output Options
        [DllImport ("libncurses.dylib")]
        extern internal static int clear (); // YC: My addition
        [DllImport ("libncurses.dylib")]
		extern internal static int clearok (IntPtr win, bool bf);
		[DllImport ("libncurses.dylib")]
		extern internal static int idlok (IntPtr win, bool bf);
		[DllImport ("libncurses.dylib")]
		extern internal static void idcok (IntPtr win, bool bf);
		[DllImport ("libncurses.dylib")]
		extern internal static void immedok (IntPtr win, bool bf);
		[DllImport ("libncurses.dylib")]
		extern internal static int leaveok (IntPtr win, bool bf);
		[DllImport ("libncurses.dylib")]
		extern internal static int wsetscrreg (IntPtr win, int top, int bot);
		[DllImport ("libncurses.dylib")]
		extern internal static int scrollok (IntPtr win, bool bf);
		
		[DllImport ("libncurses.dylib")]
		extern public static int nl();
		[DllImport ("libncurses.dylib")]
		extern public static int nonl();
		[DllImport ("libncurses.dylib")]
		extern public static int setscrreg (int top, int bot);
		
#endregion

#region refresh functions

		[DllImport ("libncurses.dylib")]
		extern public static int refresh ();
		[DllImport ("libncurses.dylib")]
		extern public static int doupdate();

		[DllImport ("libncurses.dylib")]
		extern internal static int wrefresh (IntPtr win);
		[DllImport ("libncurses.dylib")]
		extern internal static int redrawwin (IntPtr win);
		[DllImport ("libncurses.dylib")]
		extern internal static int wredrawwin (IntPtr win, int beg_line, int num_lines);
		[DllImport ("libncurses.dylib")]
		extern internal static int wnoutrefresh (IntPtr win);
#endregion

#region Output
		[DllImport ("libncurses.dylib")]
		extern public static int move (int line, int col);

		[DllImport ("libncurses.dylib", EntryPoint="addch")]
		extern internal static int _addch (int ch);
		
		[DllImport ("libncurses.dylib")]
		extern public static int addstr (string s);

		public static int addstr (string format, params object [] args)
		{
			var s = string.Format (format, args);
			return addstr (s);
		}

		static char [] r = new char [1];

		//
		// Have to wrap the native addch, as it can not
		// display unicode characters, we have to use addstr
		// for that.   but we need addch to render special ACS
		// characters
		//
		public static int addch (int ch)
		{
			if (ch < 127 || ch > 0xffff )
				return _addch (ch);
			char c = (char) ch;
			return addstr (new String (c, 1));
		}
		
		[DllImport ("libncurses.dylib")]
		extern internal static int wmove (IntPtr win, int line, int col);

		[DllImport ("libncurses.dylib")]
		extern internal static int waddch (IntPtr win, int ch);
#endregion

#region Attributes
		[DllImport ("libncurses.dylib")]
		extern public static int attron (int attrs);
		[DllImport ("libncurses.dylib")]
		extern public static int attroff (int attrs);
		[DllImport ("libncurses.dylib")]
		extern public static int attrset (int attrs);
#endregion

#region Input
		[DllImport ("libncurses.dylib")]
		extern public static int getch ();

		[DllImport ("libncurses.dylib")]
		extern public static int ungetch (int ch);

		[DllImport ("libncurses.dylib")]
		extern public static int mvgetch (int y, int x);
#endregion
		
#region Colors
		[DllImport ("libncurses.dylib")]
		extern internal static bool has_colors ();

		[DllImport ("libncurses.dylib")]
		extern internal static int start_color ();

		[DllImport ("libncurses.dylib")]
		extern internal static int init_pair (short pair, short f, short b);

		[DllImport ("libncurses.dylib")]
		extern internal static int use_default_colors ();
		
#endregion
		
#region Mouse
#endregion

#region Helpers
		[DllImport ("libmono-curses.dylib")]
		internal extern static IntPtr console_sharp_get_stdscr ();

		[DllImport ("libmono-curses.dylib")]
		internal extern static IntPtr console_sharp_get_curscr ();

		[DllImport ("libmono-curses.dylib")]
		internal extern static void console_sharp_get_dims (out int lines, out int cols);

		[DllImport ("libmono-curses.dylib")]
		internal extern static void console_sharp_sendsigtstp ();

		[DllImport ("libmono-curses.dylib")]
		internal extern static Event console_sharp_mouse_mask (Event newmask, out Event oldmask);

		[DllImport ("libmono-curses.dylib")]
		internal extern static uint console_sharp_getmouse (out MouseEvent ev);

		[DllImport ("libmono-curses.dylib")]
		internal extern static uint console_sharp_ungetmouse (ref MouseEvent ev);
#endregion

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
