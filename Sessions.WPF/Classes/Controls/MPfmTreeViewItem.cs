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

using System.Windows;
using System.Windows.Controls;
using Sessions.MVP.Models;

namespace Sessions.WPF.Classes.Controls
{
    /// <summary>
    /// http://stackoverflow.com/questions/15640263/how-to-add-a-before-expanding-event-to-the-wpf-treeview-without-using-threads
    /// </summary> 
    public class SessionsTreeViewItem : TreeViewItem
    {
        public LibraryBrowserEntity Entity { get; set; }
        public bool IsDummyNode { get; set; }

        public static readonly RoutedEvent CollapsingEvent = EventManager.RegisterRoutedEvent("Collapsing", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SessionsTreeViewItem));
        public static readonly RoutedEvent ExpandingEvent = EventManager.RegisterRoutedEvent("Expanding", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SessionsTreeViewItem));

        public event RoutedEventHandler Collapsing
        {
            add { AddHandler(CollapsingEvent, value); }
            remove { RemoveHandler(CollapsingEvent, value); }
        }

        public event RoutedEventHandler Expanding
        {
            add { AddHandler(ExpandingEvent, value); }
            remove { RemoveHandler(ExpandingEvent, value); }
        }

        protected override void OnExpanded(RoutedEventArgs e)
        {
            OnExpanding(new RoutedEventArgs(ExpandingEvent, this));
            base.OnExpanded(e);
        }

        protected override void OnCollapsed(RoutedEventArgs e)
        {
            OnCollapsing(new RoutedEventArgs(CollapsingEvent, this));
            base.OnCollapsed(e);
        }

        protected virtual void OnCollapsing(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        protected virtual void OnExpanding(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }
    }
}
