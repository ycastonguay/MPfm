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
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Sessions.OSX.Classes.Helpers;
using Sessions.OSX.Classes.Objects;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsOverlayView")]
    public class SessionsOverlayView : NSView
    {
        private ThemeType _themeType;
        private NSTextField _label;
        private SessionsRoundButton _button;
        private SessionsProgressBarView _progressBar;

        public override bool WantsDefaultClipping { get { return false; } }
        public override bool IsOpaque { get { return true; } }
        public override bool IsFlipped { get { return true; } }

        public string LabelTitle
        {
            get
            {
                return _label.StringValue;
            }
            set
            {
                _label.StringValue = value;
            }
        }

        public event SessionsRoundButton.ButtonSelected OnButtonSelected;

        [Export("init")]
        public SessionsOverlayView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsOverlayView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        public SessionsOverlayView(RectangleF frameRect) : base(frameRect)
        {
            Initialize();
        }

        public SessionsOverlayView(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public SessionsOverlayView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        private void Initialize()
        {
            WantsLayer = true;
            //Layer.BackgroundColor = new CGColor(0.2f, 0.7f);

            _label = new NSTextField(new RectangleF(0, 0, Bounds.Width, 40));
            _label.Editable = false;
            _label.Bezeled = false;
            //_label.DrawsBackground = false;
            _label.BackgroundColor = NSColor.Clear;
            //_label.BackgroundColor = NSColor.Blue;
            _label.TextColor = NSColor.White;
            _label.Font = NSFont.FromFontName("Roboto", 11);
            AddSubview(_label);

            _button = new SessionsRoundButton(new RectangleF(0, 0, 44, 44));
            _button.Hidden = true;
            _button.OnButtonSelected += HandleOnButtonSelected;
            AddSubview(_button);

            _progressBar = new SessionsProgressBarView(new RectangleF(0, 40, Bounds.Width, 6));
            _progressBar.IsIndeterminate = true;
            _progressBar.Hidden = true;
            AddSubview(_progressBar);
        }

        private void HandleOnButtonSelected(SessionsRoundButton button)
        {
            if (OnButtonSelected != null)
                OnButtonSelected(button);
        }

        public void SetTheme(ThemeType themeType)
        {
            float labelHeight = 44;
            float progressBarHeight = 8;
            float buttonSize = 44;
            float padding = 4;

            _themeType = themeType;
            switch (_themeType)
            {
                case ThemeType.Label:
                    _button.Hidden = true;
                    _progressBar.Hidden = true;

                    _label.Frame = new RectangleF(0, (Bounds.Height - labelHeight) / 2f, Bounds.Width, labelHeight);
                    break;
                case ThemeType.LabelWithActivityIndicator:
                    _button.Hidden = true;
                    _progressBar.Hidden = false;

                    float centerY = Bounds.Height / 2f;
                    _label.Frame = new RectangleF(0, centerY - labelHeight - padding, Bounds.Width, labelHeight);
                    _progressBar.Frame = new RectangleF(0, centerY + progressBarHeight + padding, Bounds.Width, progressBarHeight);
                    break;
                case ThemeType.LabelWithButtons:
                    _button.Hidden = false;
                    _progressBar.Hidden = true;

                    _label.Frame = new RectangleF(padding, Bounds.Height - labelHeight, Bounds.Width - buttonSize - (padding * 2), labelHeight);
                    _button.Frame = new RectangleF(Bounds.Width - buttonSize - padding, Bounds.Height - buttonSize, buttonSize, buttonSize);
                    break;
            }
        }

        public enum ThemeType
        {
            Label = 0,
            LabelWithActivityIndicator = 1,
            LabelWithButtons = 2
        }
    }
}
