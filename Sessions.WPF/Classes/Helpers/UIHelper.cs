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

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Sessions.WPF.Classes.Helpers
{
    public static class UIHelper
    {
        public static FrameworkElement FindByName(string name, FrameworkElement root)
        {
            var tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }

            return null;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        public static void ShowControlInsideListViewCellTemplate(ListView listView, int row, int column, string name, Visibility visibility)
        {
            var frameworkElement = GetControlInsideListViewCellTemplate(listView, row, column, name);
            if (frameworkElement != null)
                frameworkElement.Visibility = visibility;
        }

        public static FrameworkElement GetControlInsideListViewCellTemplate(ListView listView, int row, int column, string name)
        {
            var gridView = listView.View as GridView;
            var listViewItem = listView.ItemContainerGenerator.ContainerFromIndex(row);
            if (listViewItem != null)
            {
                var rowPresenter = UIHelper.FindVisualChild<GridViewRowPresenter>(listViewItem);
                if (rowPresenter != null)
                {
                    var templatedParent = VisualTreeHelper.GetChild(rowPresenter, column) as ContentPresenter;
                    var dataTemplate = gridView.Columns[column].CellTemplate;
                    if (dataTemplate != null && templatedParent != null)
                    {
                        var frameworkElement = dataTemplate.FindName(name, templatedParent) as FrameworkElement;
                        return frameworkElement;
                    }
                }
            }

            return null;
        }

        public static void ListView_PreviewMouseDown_RemoveSelectionIfNotClickingOnAListViewItem(ListView listView, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is ListViewItem))
                dep = VisualTreeHelper.GetParent(dep);

            if (dep == null)
                listView.SelectedIndex = -1;
        }

        public static void ListView_PreviewMouseDown_UnselectListViewItem(ListView listView, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is ListViewItem))
                dep = VisualTreeHelper.GetParent(dep);

            if (dep == null)
                return;

            var item = (ListViewItem)dep;
            if (item.IsSelected)
            {
                item.IsSelected = !item.IsSelected;
                e.Handled = true;
            }
        }

        public static void FadeElement(FrameworkElement element, bool show, int durationMS, Action onAnimationCompleted)
        {
            FadeElement(element, show, durationMS, 0, onAnimationCompleted);
        }

        public static void FadeElement(FrameworkElement element, bool show, int durationMS, int delayMS, Action onAnimationCompleted)
        {
            //if (element.Visibility == Visibility.Visible && show)
            //    return;

            if ((show && element.Opacity == 1 && element.Visibility == Visibility.Visible) ||
               (!show && element.Opacity == 0 && element.Visibility == Visibility.Hidden))
            {
                if (onAnimationCompleted != null)
                    onAnimationCompleted();

                return;
            }

            element.Opacity = show ? 0 : 1;
            element.Visibility = Visibility.Visible;
            var animOpacity = new DoubleAnimation();
            animOpacity.From = show ? 0 : 1;
            animOpacity.To = show ? 1 : 0;
            if (delayMS > 0)
                animOpacity.BeginTime = TimeSpan.FromMilliseconds(delayMS);
            animOpacity.Duration = TimeSpan.FromMilliseconds(durationMS);
            animOpacity.Completed += (sender, args) =>
            {
                if(!show)
                    element.Visibility = Visibility.Hidden;

                if (onAnimationCompleted != null)
                    onAnimationCompleted();
            };
            element.BeginAnimation(UIElement.OpacityProperty, animOpacity);
        }
    }
}

