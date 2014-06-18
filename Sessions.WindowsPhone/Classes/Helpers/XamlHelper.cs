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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MPfm.WindowsPhone.Classes.Helpers
{
    public static class XamlHelper
    {
        /// <summary> 
        /// Recursive get the item. 
        /// </summary> 
        /// <typeparam name="T">The item to get.</typeparam> 
        /// <param name="parents">Parent container.</param> 
        /// <param name="objectList">Item list</param> 
        public static void GetItemsRecursive<T>(DependencyObject parents, ref List<T> objectList) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parents);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parents, i);
                if (child is T)
                    objectList.Add(child as T);

                GetItemsRecursive<T>(child, ref objectList);
            }
        }
    }
}
