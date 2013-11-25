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
using Android;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util.Concurrent;
using Javax.Microedition.Khronos.Opengles;
using Object = Java.Lang.Object;
using Orientation = Android.Widget.Orientation;

namespace org.sessionsapp.android
{
    public class SeekBarPreference : Preference, SeekBar.IOnSeekBarChangeListener
    {
        private const string XmlNamespaceAndroid = "http://schemas.android.com/apk/res/android";
	    private const string XmlNamespaceSessions = "http://sessionsapp.org";

        private int _maxValue = 100;
        private int _minValue = 10;
        private int _value = 0;
        private string _units = string.Empty;
        private string _title = string.Empty;

        private SeekBar _seekBar;
        private TextView _lblTitle;
        private TextView _lblMinValue;
        private TextView _lblMaxValue;
        private TextView _lblValue;

        protected SeekBarPreference(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SeekBarPreference(Context context) : base(context)
        {
            Initialize(context, null);
        }

        public SeekBarPreference(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs);
        }

        public SeekBarPreference(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize(context, attrs);
        }

        private void Initialize(Context context, IAttributeSet attrs)
        {
            LoadValuesFromXml(attrs);
            WidgetLayoutResource = MPfm.Android.Resource.Layout.seekbar_preference;
        }

        private void LoadValuesFromXml(IAttributeSet attrs)
        {
            _title = attrs.GetAttributeValue(XmlNamespaceAndroid, "title");
            _units = attrs.GetAttributeValue(XmlNamespaceSessions, "units");
        }

        protected override View OnCreateView(ViewGroup parent)
        {
            var view = base.OnCreateView(parent);
            _seekBar = view.FindViewById<SeekBar>(MPfm.Android.Resource.Id.seekBarPreference_seekBar);
            _lblTitle = view.FindViewById<TextView>(MPfm.Android.Resource.Id.seekBarPreference_lblTitle);
            _lblMinValue = view.FindViewById<TextView>(MPfm.Android.Resource.Id.seekBarPreference_lblMinValue);
            _lblMaxValue = view.FindViewById<TextView>(MPfm.Android.Resource.Id.seekBarPreference_lblMaxValue);
            _lblValue = view.FindViewById<TextView>(MPfm.Android.Resource.Id.seekBarPreference_lblValue);

            _lblTitle.Text = _title;
            _seekBar.Max = _maxValue;
            _seekBar.SetOnSeekBarChangeListener(this);

            var layout = (LinearLayout) view;
            layout.SetPadding(0, 0, 0, 0);
            layout.Orientation = Orientation.Vertical;

            // Hide standard preference widgets
            int childCount = layout.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                var v = layout.GetChildAt(i);
                if(v.Id != Android.Resource.Id.WidgetFrame)
                    v.Visibility = ViewStates.Gone;
            }

            return view;
        }

        protected override void OnBindView(View view)
        {
            base.OnBindView(view);
        }

        protected override Object OnGetDefaultValue(TypedArray a, int index)
        {
            return base.OnGetDefaultValue(a, index);
        }

        protected override void OnSetInitialValue(bool restorePersistedValue, Object defaultValue)
        {
            base.OnSetInitialValue(restorePersistedValue, defaultValue);
        }

        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {
        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
        }
    }
}
