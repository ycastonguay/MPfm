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

namespace Stetic
{
	internal class Gui
	{
		private static bool initialized;
		
		internal static void Initialize (Gtk.Widget iconRenderer)
		{
			if ((Stetic.Gui.initialized == false)) {
				Stetic.Gui.initialized = true;
				global::Gtk.IconFactory w1 = new global::Gtk.IconFactory ();
				global::Gtk.IconSet w2 = new global::Gtk.IconSet (global::Stetic.IconLoader.LoadIcon (iconRenderer, "stock_repeat", global::Gtk.IconSize.Menu));
				w1.Add ("stock_repeat", w2);
				global::Gtk.IconSet w3 = new global::Gtk.IconSet (global::Stetic.IconLoader.LoadIcon (iconRenderer, "stock_volume", global::Gtk.IconSize.Menu));
				w1.Add ("stock_volume", w3);
				w1.AddDefault ();
			}
		}
	}
	
	internal class IconLoader
	{
		public static Gdk.Pixbuf LoadIcon (Gtk.Widget widget, string name, Gtk.IconSize size)
		{
			Gdk.Pixbuf res = widget.RenderIcon (name, size, null);
			if ((res != null)) {
				return res;
			} else {
				int sz;
				int sy;
				global::Gtk.Icon.SizeLookup (size, out  sz, out  sy);
				try {
					return Gtk.IconTheme.Default.LoadIcon (name, sz, 0);
				} catch (System.Exception) {
					if ((name != "gtk-missing-image")) {
						return Stetic.IconLoader.LoadIcon (widget, "gtk-missing-image", size);
					} else {
						Gdk.Pixmap pmap = new Gdk.Pixmap (Gdk.Screen.Default.RootWindow, sz, sz);
						Gdk.GC gc = new Gdk.GC (pmap);
						gc.RgbFgColor = new Gdk.Color (255, 255, 255);
						pmap.DrawRectangle (gc, true, 0, 0, sz, sz);
						gc.RgbFgColor = new Gdk.Color (0, 0, 0);
						pmap.DrawRectangle (gc, false, 0, 0, (sz - 1), (sz - 1));
						gc.SetLineAttributes (3, Gdk.LineStyle.Solid, Gdk.CapStyle.Round, Gdk.JoinStyle.Round);
						gc.RgbFgColor = new Gdk.Color (255, 0, 0);
						pmap.DrawLine (gc, (sz / 4), (sz / 4), ((sz - 1) - (sz / 4)), ((sz - 1) - (sz / 4)));
						pmap.DrawLine (gc, ((sz - 1) - (sz / 4)), (sz / 4), (sz / 4), ((sz - 1) - (sz / 4)));
						return Gdk.Pixbuf.FromDrawable (pmap, pmap.Colormap, 0, 0, 0, 0, sz, sz);
					}
				}
			}
		}
	}
	
	internal class ActionGroups
	{
		public static Gtk.ActionGroup GetActionGroup (System.Type type)
		{
			return Stetic.ActionGroups.GetActionGroup (type.FullName);
		}
		
		public static Gtk.ActionGroup GetActionGroup (string name)
		{
			return null;
		}
	}
}
