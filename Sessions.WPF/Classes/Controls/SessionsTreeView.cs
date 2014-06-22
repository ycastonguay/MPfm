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
using System.Windows.Media;

namespace Sessions.WPF.Classes.Controls
{
    public class SessionsTreeView : TreeView
    {
        private ScrollViewer _scrollViewer;
        public ScrollViewer ScrollViewer
        {
            get
            {
                if (_scrollViewer == null)
                {
                    DependencyObject border = VisualTreeHelper.GetChild(this, 0);
                    if (border != null)
                    {
                        _scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
                    }
                }

                return _scrollViewer;
            }
        }
    }
}
