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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.GenericControls.Controls.Interfaces;
using Sessions.GenericControls.Controls.Items;
using Sessions.GenericControls.Helpers;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsSlider")]
    public class SessionsSlider : UISlider, IScrubbingSpeedSupport
    {
        private bool _isTouchDown = false;
        private float _touchDownScrubbingValue;
        private float _touchDownScrubbingX;
        private float _touchDownY;
        private PointF _beginTrackingPosition;
        private List<ScrubbingSpeed> _scrubbingSpeeds;

        public event EventHandler TouchesBeganEvent;
        public event EventHandler TouchesMovedEvent;
        public event EventHandler TouchesEndedEvent;
        public event EventHandler BeginTrackingEvent;
        public event EventHandler ContinueTrackingEvent;
        public event EventHandler EndTrackingEvent;
        public event ScrubbingSpeedChangedDelegate OnScrubbingSpeedChanged;

        private ScrubbingSpeed _currentScrubbingSpeed;
        public ScrubbingSpeed CurrentScrubbingSpeed
        {
            get { return _currentScrubbingSpeed; }
        }

        public SessionsSlider(IntPtr handle) 
            : base (handle)
        {
            this.Continuous = true;
            CreateScrubbingSpeeds();
        }

        public SessionsSlider(RectangleF frame) 
            : base (frame)
        {
            this.Continuous = true;
            CreateScrubbingSpeeds();
        }

        private void CreateScrubbingSpeeds()
        {
            _scrubbingSpeeds = ScrubbingSpeedHelper.GetScrubbingSpeeds(2.85f);
            _currentScrubbingSpeed = _scrubbingSpeeds[0];
        }

        protected void ScrubbingSpeedChanged(ScrubbingSpeed scrubbingSpeed)
        {
            if (OnScrubbingSpeedChanged != null)
                OnScrubbingSpeedChanged(scrubbingSpeed);
        }

        protected virtual void OnTouchesBeganEvent(EventArgs e)
        {
            if(TouchesBeganEvent != null)
                TouchesBeganEvent(this, e);
        }

        protected virtual void OnTouchesMovedEvent(EventArgs e)
        {
            if(TouchesMovedEvent != null)
                TouchesMovedEvent(this, e);
        }

        protected virtual void OnTouchesEndedEvent(EventArgs e)
        {
            if(TouchesEndedEvent != null)
                TouchesEndedEvent(this, e);
        }

        protected virtual void OnBeginTrackingEvent(EventArgs e)
        {
            if(BeginTrackingEvent != null)
                BeginTrackingEvent(this, e);
        }

        protected virtual void OnContinueTrackingEvent(EventArgs e)
        {
            if(ContinueTrackingEvent != null)
                ContinueTrackingEvent(this, e);
        }

        protected virtual void OnEndTrackingEvent(EventArgs e)
        {
            if(EndTrackingEvent != null)
                EndTrackingEvent(this, e);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            _isTouchDown = true;
            OnTouchesBeganEvent(new EventArgs());
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            OnTouchesMovedEvent(new EventArgs());
            base.TouchesMoved(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            _isTouchDown = false;
            OnTouchesEndedEvent(new EventArgs());
            base.TouchesEnded(touches, evt);
        }

        public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
        {
            OnBeginTrackingEvent(new EventArgs());
            _beginTrackingPosition = uitouch.LocationInView(this);

            _touchDownScrubbingX = _beginTrackingPosition.X;
            _touchDownY = _beginTrackingPosition.Y;
            _touchDownScrubbingValue = Value;

            //Console.WriteLine("*>  BeginTracking - pt: {0}", _beginTrackingPosition);
            return true;
        }

        public override bool ContinueTracking(UITouch uitouch, UIEvent uievent)
        {
            //PointF ptPrev = uitouch.PreviousLocationInView(this);
            PointF pt = uitouch.LocationInView(this);

            float deltaY = pt.Y - _beginTrackingPosition.Y;
            var scrubbingSpeed = ScrubbingSpeedHelper.IdentifyScrubbingSpeed(deltaY, _scrubbingSpeeds);
            if (_currentScrubbingSpeed != scrubbingSpeed)
            {
                _currentScrubbingSpeed = scrubbingSpeed;
                ScrubbingSpeedChanged(_currentScrubbingSpeed);

                // Set a new reference when changing scrubbing speed
                _touchDownScrubbingValue = Value;
                _touchDownScrubbingX = pt.X;
            }

            // Calculate new value
            float valueRange = (MaxValue - MinValue) + 1;
            float trackWidth = Bounds.Width; // in fact it should include the padding...
            float valuePerPixel = (valueRange / trackWidth) * _currentScrubbingSpeed.Speed;
            float value = _touchDownScrubbingValue + (pt.X - _touchDownScrubbingX) * valuePerPixel;
            value = Math.Max(value, MinValue);
            value = Math.Min(value, MaxValue);
            if (value != Value)
            {
                Value = (int) value;
            }

            //Console.WriteLine("*> ContinueTracking - pt: {0} - _beginTrackingPt: {1} - valuePerPixel: {2} value: {3}", pt, _beginTrackingPosition, valuePerPixel, value);
            return true;
        }

        public override void EndTracking(UITouch uitouch, UIEvent uievent)
        {
            if(Tracking)
            {
                SendActionForControlEvents(UIControlEvent.ValueChanged);
            }

            OnEndTrackingEvent(new EventArgs());
            _currentScrubbingSpeed = _scrubbingSpeeds[0];
            //PointF pt = uitouch.LocationInView(this);
            //Console.WriteLine("*> EndTracking - pt: {0}", pt);
        }

        public override void CancelTracking(UIEvent uievent)
        {
            base.CancelTracking(uievent);
        }

        public override UIView HitTest(PointF point, UIEvent uievent)
        {
            // This trick makes it easier to use the thumb button of a slider inside a scroll view
            // http://stackoverflow.com/questions/4600290/uislider-and-uiscrollview
            RectangleF trackRect = TrackRectForBounds(Bounds);
            RectangleF thumbRect = ThumbRectForBounds(Bounds, trackRect, Value);

            if (thumbRect.Contains(point))
                return base.HitTest(point, uievent);
            else
                return Superview.HitTest(point, uievent);
        }

        public int IndexOfLowerScrubbingSpeed(List<float> scrubbingSpeedPositions, float verticalOffset)
        {
            for(int i = 0; i < scrubbingSpeedPositions.Count; i++)
            {
                float scrubbingSpeedOffset = scrubbingSpeedPositions[i];
                if(verticalOffset < scrubbingSpeedOffset)
                    return i;
            }

            return -1;
        }
        
        public void SetPosition(float position)
        {
            if (!_isTouchDown)
                this.Value = position;
        }

    }
}
