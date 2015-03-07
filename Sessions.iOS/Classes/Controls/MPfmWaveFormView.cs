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
using Sessions.GenericControls.Controls;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controls.Graphics;
using Sessions.iOS.Classes.Objects;
using Sessions.Sound.AudioFiles;
using System.Collections.Generic;
using Sessions.iOS.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;
using Sessions.Sound.Objects;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsWaveFormView")]
    public class SessionsWaveFormView : UIView
    {
		private WaveFormControl _control;

		public long Length
		{
			get
			{
				return _control.Length;
			}
			set
			{
				_control.Length = value;
			}
		}

		public long Position
		{
			get
			{
				return _control.Position;
			}
			set
			{
				_control.Position = value;
			}
		}

		public bool ShowSecondaryPosition
		{
			get
			{
				return _control.ShowSecondaryPosition;
			}
			set
			{
				_control.ShowSecondaryPosition = value;
			}
		}

		public long SecondaryPosition
		{
			get
			{
				return _control.SecondaryPosition;
			}
			set
			{
				_control.SecondaryPosition = value;
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

        public BasicRectangle ContentSize
        {
            get
            {
                return _control.ContentSize;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _control.IsEmpty;
            }
        }

		public SessionsWaveFormView(IntPtr handle) 
			: base (handle)
		{
			Initialize();
		}

		public SessionsWaveFormView(RectangleF frame) 
			: base(frame)
		{
			Initialize();
		}

		private void Initialize()
		{
			BackgroundColor = GlobalTheme.BackgroundColor;
			_control = new WaveFormControl();
			_control.FontFace = "HelveticaNeue";
			_control.LetterFontFace = "HelveticaNeue";
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

		public void LoadPeakFile(AudioFile audioFile)
		{
            Console.WriteLine("SessionsWaveFormView - LoadPeakFile");
			_control.LoadPeakFile(audioFile);
		}

        public void CancelPeakFile()
        {
            _control.CancelPeakFile();
        }

		public void SetActiveMarker(Guid markerId)
		{
			_control.SetActiveMarker(markerId);
		}

		public void SetMarkers(IEnumerable<Marker> markers)
		{
			_control.SetMarkers(markers);
		}

		public void SetMarkerPosition(Marker marker)
		{
			_control.SetMarkerPosition(marker);
		}

		public void InvalidateBitmaps()
		{
			_control.InvalidateBitmaps();
		}
    }
}
