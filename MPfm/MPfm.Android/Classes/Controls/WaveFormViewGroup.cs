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
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Controls.Graphics;
using MPfm.Android.Classes.Controls.Helpers;
using MPfm.GenericControls.Controls;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace org.sessionsapp.android
{
    public class WaveFormViewGroup : LinearLayout //ViewGroup
    {
        public WaveFormScaleView ScaleView { get; private set; }
        public WaveFormView View { get; private set; }

        public WaveFormViewGroup(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public WaveFormViewGroup(Context context) : base(context)
        {
            Initialize();
        }

        public WaveFormViewGroup(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public WaveFormViewGroup(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            SetBackgroundColor(Color.HotPink);

            // Is it right to add/create views here? do we need attrs? No idea
            var stupidView = new View(Context);
            stupidView.SetBackgroundColor(Color.Blue);
            AddView(stupidView);

            // Only one view is visible for some reason. What a waste of time.
            var retardView = new View(Context);
            retardView.SetBackgroundColor(Color.Yellow);
            AddView(retardView);

            // Those views are never visible, completely clueless why after 2 hours of completely wasting my time
            View = new WaveFormView(Context);
            ScaleView = new WaveFormScaleView(Context);            

            AddView(View);
            AddView(ScaleView);
        }

        //protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        //{
        //    Console.WriteLine("WaveFormViewGroup - OnMeasure - widthMeasureSpec: {0} heightMeasureSpec: {1}", widthMeasureSpec, heightMeasureSpec);
        //    base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

        //    for (int a = 0; a < ChildCount; a++)
        //    {
        //        var view = GetChildAt(a);
        //        Console.WriteLine("WaveFormViewGroup - OnMeasure - index: {0} viewType: {1}", a, view.GetType().FullName);
        //        view.Measure(widthMeasureSpec, heightMeasureSpec);
        //    }
        //}

        //protected override void OnLayout(bool changed, int l, int t, int r, int b)
        //{
        //    Console.WriteLine("WaveFormViewGroup - OnLayout - changed: {0} l: {1} t: {2} r: {3} b: {4}", changed, l, t, r, b);
        //    for (int a = 0; a < ChildCount; a++)
        //    {
        //        var view = GetChildAt(a);
        //        var layoutParams = view.LayoutParameters;                
        //        int width = view.MeasuredWidth;
        //        int height = view.MeasuredHeight;
        //        Console.WriteLine("WaveFormViewGroup - OnLayout - index: {0} viewType: {1} measuredWidth: {2} measuredHeight: {3} layoutParams.Width: {4} layoutParams.Height: {5}", a, view.GetType().FullName, width, height, layoutParams.Width, layoutParams.Height);
        //        if(view is WaveFormScaleView)
        //            view.Layout(0, 0, 20, 20);
        //        else if(view is WaveFormView)
        //            //view.Layout(l, t, r, b - 40);
        //            //view.Layout(0, 0, r - l, 40);
        //            view.Layout(20, 20, 100, 100);
        //    }
        //}
    }
}