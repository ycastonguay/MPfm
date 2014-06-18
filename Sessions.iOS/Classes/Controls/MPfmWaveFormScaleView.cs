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
using System.Drawing;
using MPfm.Sound.AudioFiles;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.GenericControls.Controls;
using MPfm.iOS.Classes.Controls.Graphics;
using MPfm.iOS.Classes.Controls.Helpers;
using MPfm.GenericControls.Basics;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormScaleView")]
    public class MPfmWaveFormScaleView : UIView
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

        public MPfmWaveFormScaleView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public MPfmWaveFormScaleView(RectangleF frame) 
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
