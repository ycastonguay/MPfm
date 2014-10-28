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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Sessions.WPF.Classes.Controls.Graphics;
using Sessions.WPF.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls;
using Sessions.GenericControls.Services;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Control = System.Windows.Controls.Control;

namespace Sessions.WPF.Classes.Controls
{
    public class WaveForm : Control
    {
        private readonly WaveFormControl _control;

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

        public WaveFormControl.InputInteractionMode InteractionMode
        {
            get
            {
                return _control.InteractionMode;
            }
            set
            {
                _control.InteractionMode = value;
            }
        }

        public WaveFormDisplayType DisplayType
        {
            get
            {
                return _control.DisplayType;
            }
            set
            {
                _control.DisplayType = value;
            }
        }

        public event WaveFormControl.ChangePosition OnChangePosition;
        public event WaveFormControl.ChangePosition OnChangeSecondaryPosition;
        public event WaveFormControl.ChangeSegmentPosition OnChangingSegmentPosition;
        public event WaveFormControl.ChangeSegmentPosition OnChangedSegmentPosition;

        public WaveForm()
        {
            ClipToBounds = true;
            _control = new WaveFormControl();
            _control.OnInvalidateVisual += () =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(InvalidateVisual));
            };
            _control.OnInvalidateVisualInRect += (rect) => Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                InvalidateVisual();
                // TODO: It seems you can't invalidate a specific rect in WPF? What?
                // http://stackoverflow.com/questions/2576599/possible-to-invalidatevisual-on-a-given-region-instead-of-entire-wpf-control                                                                                                                       
            }));
            _control.OnChangePosition += (position) => OnChangePosition(position);
            _control.OnChangeSecondaryPosition += (position) => OnChangeSecondaryPosition(position);
            _control.OnChangingSegmentPosition += (segment, bytes) => OnChangingSegmentPosition(segment, bytes);
            _control.OnChangedSegmentPosition += (segment, bytes) => OnChangedSegmentPosition(segment, bytes);
            _control.OnChangeMouseCursorType += GenericControlHelper.ChangeMouseCursor;
        }

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            _control.SetMarkers(markers);
        }

        public void SetActiveMarker(Guid markerId)
        {
            _control.SetActiveMarker(markerId);
        }

        public void SetMarkerPosition(Marker marker)
        {
            _control.SetMarkerPosition(marker);
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            _control.Length = lengthBytes;
        }

        public void SetLoop(Loop loop)
        {
            _control.SetLoop(loop);
        }

        public void SetSegment(Segment segment)
        {
            _control.SetSegment(segment);
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            _control.LoadPeakFile(audioFile);
        }

        public void Reset()
        {
            _control.Reset();
        }

        public void InvalidateBitmaps()
        {
            _control.InvalidateBitmaps();
        }

        protected override void OnRender(DrawingContext dc)
        {
            //Console.WriteLine("WaveForm - OnRender - width: {0} height: {1}", ActualWidth, ActualHeight);
            base.OnRender(dc);
            // TODO: Fix this, dirty rects in WPF is still a mystery. It was simple in WinForms, why is it hidden in WPF?
            var dirtyRect = new BasicRectangle(0, 0, (float)ActualWidth, (float)ActualHeight); 
            var wrapper = new GraphicsContextWrapper(dc, (float) ActualWidth, (float) ActualHeight, dirtyRect);
            if (ActualWidth == 0 || ActualHeight == 0)
                return;
            _control.Render(wrapper);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            GenericControlHelper.MouseDown(e, this, _control);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            GenericControlHelper.MouseUp(e, this, _control);
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            GenericControlHelper.MouseMove(e, this, _control);
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            _control.MouseLeave();
            base.OnMouseLeave(e);
        }
    }
}
