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
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace org.sessionsapp.android
{
    /// <summary>
    /// This custom list view is based on the Android list view but adds a few things such as the IsScrollable property and the ability to change row order
    /// (because Google is clearly too lazy to do anything like that).
    /// </summary>
    public class CustomListView : ListView
    {
        View _viewItemMove;

        public bool CanItemsBeMoved { get; set; }
        public bool IsScrollable { get; set; }
        public bool IsMovingItem { get; private set; }

        protected CustomListView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public CustomListView(Context context) : base(context)
        {
            Initialize();
        }

        public CustomListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public CustomListView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            IsScrollable = true;
            CanItemsBeMoved = true;
            IsMovingItem = false;
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            //Console.WriteLine("CustomListView - DispatchTouchEvent - action: {0} buttonState: {1} downTime: {2} eventTime: {3} x,y: ({4},{5}) width: {6} density: {7} canItemsBeMoved: {8} isMovingItem: {9} isScrollable: {10}", e.Action, e.ButtonState, e.DownTime, e.EventTime, e.GetX(), e.GetY(), Width, Resources.DisplayMetrics.Density, CanItemsBeMoved, IsMovingItem, IsScrollable);
            Console.WriteLine("CustomListView - DispatchTouchEvent - action: {0} x,y: ({1},{2})", e.Action, e.GetX(), e.GetY());

            float density = Resources.DisplayMetrics.Density;
            float x = e.GetX();
            float y = e.GetY();

            if (CanItemsBeMoved && !IsMovingItem && x >= Width - 48 * density)
            {
                // The user is trying to move an item using the right hand 'button'.
                Console.WriteLine("CustomListView - DispatchTouchEvent - Starting to move item...");
                _viewItemMove = GetChildAtPosition(x, y);
                if (_viewItemMove != null)
                {
                    IsMovingItem = true;
                    IsScrollable = false;
                    int tag = -1;
                    if (_viewItemMove.Tag != null)
                        tag = (int)_viewItemMove.Tag;

                    Console.WriteLine("CustomListView - DispatchTouchEvent - Found moving item! tag: {0}", tag);
                }
                else
                {
                    Console.WriteLine("CustomListView - DispatchTouchEvent - Did NOT find moving item :-(");
                }                
            }

            // Keep cancel on top because the flag can contain both move and cancel.
            // if (e.Action.HasFlag(MotionEventActions.Cancel)) // This was cancel when using OnTouchListener on a child view
            if(e.Action.HasFlag(MotionEventActions.Up))
            {
                //Console.WriteLine("CustomListView - DispatchTouchEvent - Up - (x,y): ({0},{1})", x, y);
                Console.WriteLine("CustomListView - DispatchTouchEvent - CANCELLING MOVE...");
                _viewItemMove = null;
                IsMovingItem = false;
                IsScrollable = true;
            }
            else if (e.Action.HasFlag(MotionEventActions.Move))
            {
                if (_viewItemMove != null)
                {
                    Console.WriteLine("CustomListView - DispatchTouchEvent - Moving item to ({0},{1})...", x, y);
                    _viewItemMove.SetX(x);
                    _viewItemMove.SetY(y);                    
                }

                // Block scroll
                if(!IsScrollable)
                    return true;
            }

            // Try to find the item over the finger
            View view = GetChildAtPosition(x, y);
            if (view != null)
            {
                int tag = -1;
                if (view.Tag != null)
                    tag = (int)view.Tag;

                Console.WriteLine("CustomListView - DispatchTouchEvent - Found finger over view! tag: {0}", tag);
            }
            else
            {
                Console.WriteLine("CustomListView - DispatchTouchEvent - Did NOT find finger over view :-(");
            }

            return base.DispatchTouchEvent(e);
        }

        private View GetChildAtPosition(float x, float y)
        {
            View returnView = null;
            int lastIndex = LastVisiblePosition - FirstVisiblePosition;
            for (int a = 0; a < lastIndex; a++)
            {
                View view = GetChildAt(a);
                if (view != null)
                {
                    Rect rect = new Rect();
                    view.GetHitRect(rect);
                    bool isOverItem = y >= rect.Top && y <= rect.Bottom;
                    //Console.WriteLine("CustomListView - GetChildAtPosition - Finding rects - position: {0} hitRect(x,y): ({1},{2})", a, rect.Left, rect.Top);
                    if (isOverItem)
                    {
                        Console.WriteLine("CustomListView - GetChildAtPosition - FOUND CHILD - position: {0} hitRect(x,y): ({1},{2})", a, rect.Left, rect.Top);
                        returnView = view;
                        break;
                    }
                }
            }

            return returnView;
        }
    }
}