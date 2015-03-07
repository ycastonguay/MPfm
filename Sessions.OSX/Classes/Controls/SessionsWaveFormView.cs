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
using org.sessionsapp.player;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls;
using Sessions.GenericControls.Services;
using Sessions.OSX.Classes.Controls.Graphics;
using Sessions.OSX.Classes.Controls.Helpers;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Objects;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsWaveFormView")]
    public class SessionsWaveFormView : NSView
    {
        private WaveFormControl _control;
        private SizeF _currentSize = new SizeF(0, 0);

        public override bool WantsDefaultClipping { get { return false; } }
        public override bool IsOpaque { get { return true; } }
        public override bool IsFlipped { get { return true; } }

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
//        public event WaveFormControl.ChangeSegmentPosition OnChangingSegmentPosition;
//        public event WaveFormControl.ChangeSegmentPosition OnChangedSegmentPosition;
        public event WaveFormControl.ContentOffsetChanged OnContentOffsetChanged;

        [Export("init")]
        public SessionsWaveFormView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsWaveFormView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            WantsLayer = true;
            LayerContentsRedrawPolicy = NSViewLayerContentsRedrawPolicy.OnSetNeedsDisplay;
            _control = new WaveFormControl();    
            _control.OnChangePosition += (positionPercentage) => OnChangePosition(positionPercentage);
            _control.OnChangeSecondaryPosition += (positionPercentage) => OnChangeSecondaryPosition(positionPercentage);
//            _control.OnChangingSegmentPosition += (segment, positionPercentage) => OnChangingSegmentPosition(segment, positionPercentage);
//            _control.OnChangedSegmentPosition += (segment, positionPercentage) => OnChangedSegmentPosition(segment, positionPercentage);
            _control.OnContentOffsetChanged += (offset) => OnContentOffsetChanged(offset);
            _control.OnChangeMouseCursorType += GenericControlHelper.ChangeMouseCursor;
            _control.OnInvalidateVisual += () => InvokeOnMainThread(() => SetNeedsDisplayInRect(Bounds));
            _control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));

            // Add tracking area to receive mouse move and mouse dragged events
            var opts = NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.InVisibleRect | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.EnabledDuringMouseDrag;
            var trackingArea = new NSTrackingArea(Bounds, opts, this, new NSDictionary());
            AddTrackingArea(trackingArea);

            SetFrame();
            PostsBoundsChangedNotifications = true;
            NSNotificationCenter.DefaultCenter.AddObserver(NSView.FrameChangedNotification, FrameDidChangeNotification, this);
        }
        
        private void FrameDidChangeNotification(NSNotification notification)
        {
            //Console.WriteLine("WaveFormScrollView - NSViewFrameDidChangeNotification - Bounds: {0} Frame: {1}", Bounds, Frame);
            SetFrame();
            _control.InvalidateBitmaps();
        }

        private void SetFrame()
        {
            _control.Frame = new BasicRectangle(0, 0, Frame.Width, Frame.Height);
        }
        
        public override void DrawRect(RectangleF dirtyRect)
        {
            //Console.WriteLine("WaveFormView - DrawRect - dirtyRect: {0}", dirtyRect);
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
            context.SaveState();
            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height, GenericControlHelper.ToBasicRect(dirtyRect));
            _control.Render(wrapper);
            context.RestoreState();
            
            //stopwatch.Stop();
            //Console.WriteLine("WaveFormView - DrawRect - Render time: {0}", stopwatch.Elapsed);
        }
        
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            GenericControlHelper.MouseUp(this, _control, theEvent);
        }
        
        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);
            GenericControlHelper.MouseDown(this, _control, theEvent);
            GenericControlHelper.MouseClick(this, _control, theEvent);
        }
        
        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
            GenericControlHelper.MouseMove(this, _control, theEvent);
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            base.MouseDragged(theEvent);
            GenericControlHelper.MouseMove(this, _control, theEvent);
        }

        public override void MouseExited(NSEvent theEvent)
        {
            base.MouseExited(theEvent);
            _control.MouseLeave();
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

        public void SetLoop(SSPLoop loop)
        {
            _control.SetLoop(loop);
        }

//        public void SetSegment(Segment segment)
//        {
//            _control.SetSegment(segment);
//        }

        public void SetWaveFormLength(long lengthBytes)
        {
            _control.Length = lengthBytes;
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            _control.LoadPeakFile(audioFile);
        }

        public void Reset()
        {
            _control.Reset();
        }
    }
}
