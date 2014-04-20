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
using MPfm.GenericControls.Controls;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controls.Graphics;
using MPfm.iOS.Classes.Objects;
using MPfm.Sound.AudioFiles;
using System.Collections.Generic;
using MPfm.Player.Objects;
using MPfm.iOS.Classes.Controls.Helpers;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormView")]
    public class MPfmWaveFormView : UIView
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

		public MPfmWaveFormView(IntPtr handle) 
			: base (handle)
		{
			Initialize();
		}

		public MPfmWaveFormView(RectangleF frame) 
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
			var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height);
			_control.Render(wrapper);
		}

		public void LoadPeakFile(AudioFile audioFile)
		{
			_control.LoadPeakFile(audioFile);
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
    }
}
