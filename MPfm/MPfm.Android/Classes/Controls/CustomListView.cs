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
using Android.Content;
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
        public bool IsScrollable { get; set; }

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
            Console.WriteLine("CustomListView - Initialize");
            IsScrollable = true;
        }

        public override bool DispatchTouchEvent(Android.Views.MotionEvent e)
        {
            // Cancel scrolling
            if(!IsScrollable && e.Action.HasFlag(MotionEventActions.Move))
                return true;

            return base.DispatchTouchEvent(e);
        }
    }
}