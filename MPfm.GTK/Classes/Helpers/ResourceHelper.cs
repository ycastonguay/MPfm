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
using System.Reflection;

namespace MPfm.GTK.Helpers
{
    public static class ResourceHelper
    {
//        public static void TestLoadResources()
//        {
//            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
//            foreach(var name in names)
//            {
//                var info = Assembly.GetExecutingAssembly().GetManifestResourceInfo(name);
//                Console.WriteLine("Assembly Resource - name: {0} fileName: {1}", name, info.FileName);
//            }
//        }
        /// <summary>
        /// Gets an embedded image resource in the current executing assembly.
        /// </summary>
        /// <returns>
        /// Embedded image
        /// </returns>
        /// <param name='name'>
        /// Image name (example: 'icon_linux.png' will load 'MPfm.GTK.Resources.icon_linux.png')
        /// </param>
        public static Gdk.Pixbuf GetEmbeddedImageResource(string name)
        {
            return new Gdk.Pixbuf(Assembly.GetExecutingAssembly().GetManifestResourceStream("MPfm.GTK.Resources." + name));
        }
	}
}
