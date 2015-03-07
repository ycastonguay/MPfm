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
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Helpers;
using Sessions.GenericControls.Controls;
using Sessions.iOS.Classes.Controls.Helpers;
using Sessions.iOS.Classes.Controls.Graphics;
using Sessions.GenericControls.Basics;
using org.sessionsapp.player;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsEqualizerPresetGraphView")]
    public class SessionsEqualizerPresetGraphView : UIView
    {
        private EqualizerPresetGraphControl _control;

        public SSPEQPreset Preset { get { return _control.Preset; } set { _control.Preset = value; } }
        public BasicColor ColorBackground { get { return _control.ColorBackground; } set { _control.ColorBackground = value; } }
        public BasicColor ColorForeground { get { return _control.ColorForeground; } set { _control.ColorForeground = value; } }
        public BasicColor ColorMainLine { get { return _control.ColorMainLine; } set { _control.ColorMainLine = value; } }
        public BasicColor ColorSecondaryLine { get { return _control.ColorSecondaryLine; } set { _control.ColorSecondaryLine = value; } }
        public bool ShowText { get { return _control.ShowText; } set { _control.ShowText = value; } }
        public bool ShowGuideLines { get { return _control.ShowGuideLines; } set { _control.ShowGuideLines = value; } }

        public SessionsEqualizerPresetGraphView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public SessionsEqualizerPresetGraphView(RectangleF frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.BackgroundColor = UIColor.Black;
            _control = new EqualizerPresetGraphControl();
            //_control.FontFace = "HelveticaNeue";
            //_control.LetterFontFace = "HelveticaNeue";
            _control.OnInvalidateVisual += () => InvokeOnMainThread(SetNeedsDisplay);
            _control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));
        }

        public override void Draw(RectangleF rect)
        {
            var context = UIGraphics.GetCurrentContext();
            context.SaveState();
            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height, GenericControlHelper.ToBasicRect(rect));
            _control.Render(wrapper);
            context.RestoreState();
        }
    }
}
