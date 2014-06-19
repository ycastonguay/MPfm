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
using System.Drawing;
using Sessions.Sound.AudioFiles;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Objects;
using Sessions.GenericControls.Controls;
using Sessions.iOS.Classes.Controls.Graphics;
using Sessions.iOS.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsWaveFormScaleView")]
    public class SessionsWaveFormScaleView : UIView
    {
		private WaveFormScaleControl _control;

        public AudioFile AudioFile
        {
            get
            {
				return _control.AudioFile;
            }
            set
            {
				_control.AudioFile = value;
            }
        }

        public long AudioFileLength
        {
            get
            {
				return _control.AudioFileLength;
            }
            set
            {
				_control.AudioFileLength = value;
            }
        }
        
        public float Zoom
        {
            get
            {
                return _control.Zoom;
            }
            set
            {
                _control.Zoom = value;
            }
        }

		public BasicPoint ContentOffset
		{
			get
			{
				return _control.ContentOffset;
			}
			set
			{
				_control.ContentOffset = value;
			}
		}

        public SessionsWaveFormScaleView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public SessionsWaveFormScaleView(RectangleF frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
			BackgroundColor = GlobalTheme.BackgroundColor;
			_control = new WaveFormScaleControl();            
			_control.FontFace = "HelveticaNeue";
			_control.FontSize = 10;
			_control.OnInvalidateVisual += () => InvokeOnMainThread(SetNeedsDisplay);
			_control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));
        }

        public override void Draw(RectangleF rect)
        {
			var context = UIGraphics.GetCurrentContext();
			var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height, GenericControlHelper.ToBasicRect(rect));
			_control.Render(wrapper);
        }
    }
}
