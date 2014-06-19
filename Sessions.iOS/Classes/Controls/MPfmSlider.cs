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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmSlider")]
    public class MPfmSlider : UISlider
    {
        private bool _isTouchDown = false;
        private PointF _beginTrackingPosition;
        private List<float> _scrubbingSpeedChangePositions = new List<float>() { 0, 50, 100, 150 };
        private List<float> _scrubbingSpeeds = new List<float>() { 1, 0.5f, 0.25f, 0.1f };

        public event EventHandler ScrubbingTypeChanged;
        public event EventHandler TouchesBeganEvent;
        public event EventHandler TouchesMovedEvent;
        public event EventHandler TouchesEndedEvent;
        public event EventHandler BeginTrackingEvent;
        public event EventHandler ContinueTrackingEvent;
        public event EventHandler EndTrackingEvent;
        public SliderScrubbingType ScrubbingType { get; private set; }

        public MPfmSlider(IntPtr handle) 
            : base (handle)
        {
            this.Continuous = true;
        }

        public MPfmSlider(RectangleF frame) 
            : base (frame)
        {
            this.Continuous = true;
        }

        protected virtual void OnScrubbingTypeChanged(EventArgs e)
        {
            if(ScrubbingTypeChanged != null)
                ScrubbingTypeChanged(this, e);
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
//            UITouch touch = touches.AnyObject as UITouch;
//            PointF pt = touch.LocationInView(this);
//
//            Console.WriteLine("Slider - TouchesMoved (" + pt.X.ToString() + ", " + pt.Y.ToString());
//
//            // Determine type of scrubbing
//            var scrubbingType = SliderScrubbingType.HighSpeed;
//            if(pt.Y - Frame.Y + Frame.Height > 40)
//            {
//                scrubbingType = SliderScrubbingType.HalfSpeed;
//            }
//            else if(pt.Y - Frame.Y + Frame.Height > 80)
//            {
//                scrubbingType = SliderScrubbingType.QuarterSpeed;
//            }
//            else if(pt.Y - Frame.Y + Frame.Height > 120)
//            {
//                scrubbingType = SliderScrubbingType.Fine;
//            }
//
//            // Check if event needs to be raised
//            if(scrubbingType != ScrubbingType)
//            {
//                ScrubbingType = scrubbingType;
//            }

            // High-speed scrubbing
            // Half-speed
            // Quarter-speed
            // Fine

            OnTouchesMovedEvent(new EventArgs());
            base.TouchesMoved(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            _isTouchDown = false;
            OnTouchesEndedEvent(new EventArgs());
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
        }

        private float scrubbingSpeed;
        private float realPositionValue;
        private PointF beganTrackingLocation;

        public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
        {
            bool beginTracking = base.BeginTracking(uitouch, uievent);
            if(beginTracking)
            {
                // Set the beginning tracking location to the centre of the current
                // position of the thumb. This ensures that the thumb is correctly re-positioned
                // when the touch position moves back to the track after tracking in one
                // of the slower tracking zones.
                RectangleF thumbRect = ThumbRectForBounds(Bounds, TrackRectForBounds(Bounds), Value);
                beganTrackingLocation = new PointF(thumbRect.X + thumbRect.Size.Width / 2.0f, thumbRect.Y + thumbRect.Size.Height / 2.0f);
                realPositionValue = Value;
            }

            OnBeginTrackingEvent(new EventArgs());
            return beginTracking;

            //Console.WriteLine("BeginTracking");
            //_beginTrackingPosition = uitouch.LocationInView(this);
            //return true;
        }

        public override bool ContinueTracking(UITouch uitouch, UIEvent uievent)
        {

//            PointF previousLocation = uitouch.PreviousLocationInView(this);
//            PointF currentLocation = uitouch.LocationInView(this);
//            float trackingOffset = currentLocation.X - previousLocation.X;
//
//            // Find the scrubbing speed that corresponds to the touch's vertical offset
//            float verticalOffset = Math.Abs(currentLocation.Y - beganTrackingLocation.Y);
//            int scrubbingSpeedChangePosIndex = IndexOfLowerScrubbingSpeed(_scrubbingSpeedChangePositions, verticalOffset);
//            if(scrubbingSpeedChangePosIndex == -1)
//                scrubbingSpeedChangePosIndex = _scrubbingSpeeds.Count;
//            this.scrubbingSpeed = _scrubbingSpeeds[scrubbingSpeedChangePosIndex - 1];
//
//            RectangleF trackRect = TrackRectForBounds(Bounds);
//            this.realPositionValue = this.realPositionValue * (MaxValue - MinValue) * (trackingOffset / trackRect.Size.Width);
//
//            float valueAdjustment = this.scrubbingSpeed * (MaxValue - MinValue) * (trackingOffset / trackRect.Size.Width);
//            float thumbAdjustment = 0;
//            if(((this.beganTrackingLocation.Y < currentLocation.Y) && (currentLocation.Y < previousLocation.Y)) ||
//               ((this.beganTrackingLocation.Y > currentLocation.Y) && (currentLocation.Y > previousLocation.Y)))
//            {
//                thumbAdjustment = (this.realPositionValue - Value) / (1 + Math.Abs(currentLocation.Y - this.beganTrackingLocation.Y));
//            }
//            Value += valueAdjustment + thumbAdjustment;
//
//            if(Continuous)
//                SendActionForControlEvents(UIControlEvent.ValueChanged);
//
//            OnContinueTrackingEvent(new EventArgs());
//            return Tracking;


            PointF ptPrev = uitouch.PreviousLocationInView(this);
            PointF pt = uitouch.LocationInView(this);

            //float relativeX = pt.X - Frame.X;
            //float ratioX = relativeX / Frame.Width;
            float widthWithFinger = Frame.Width + 44;
            float normalizedX = (pt.X < 0) ? 0 : pt.X;
            normalizedX = (pt.X > widthWithFinger) ? widthWithFinger : normalizedX;
            float ratioX = normalizedX / widthWithFinger;

            // Determine type of scrubbing
            var scrubbingType = SliderScrubbingType.HighSpeed;
            if(pt.Y - Frame.Y + Frame.Height > 300)
            {
                scrubbingType = SliderScrubbingType.Fine;
            }
            else if(pt.Y - Frame.Y + Frame.Height > 200)
            {
                scrubbingType = SliderScrubbingType.QuarterSpeed;
            }
            else if(pt.Y - Frame.Y + Frame.Height > 100)
            {
                scrubbingType = SliderScrubbingType.HalfSpeed;
            }
            
            // Check if event needs to be raised
            if(scrubbingType != ScrubbingType)
            {
                //Console.WriteLine("Slider - Changed scrubbing type to " + scrubbingType.ToString());
                ScrubbingType = scrubbingType;
                OnScrubbingTypeChanged(new EventArgs());
            }

            // Calculate new value
            float newValueDelta = (ratioX * 10000) - Value;
            switch(scrubbingType)
            {
                case SliderScrubbingType.HalfSpeed:
                    newValueDelta = newValueDelta * 0.5f;
                    break;
                case SliderScrubbingType.QuarterSpeed:
                    newValueDelta = newValueDelta * 0.25f;
                    break;
                case SliderScrubbingType.Fine:
                    newValueDelta = newValueDelta * 0.1f;
                    break;
            }
//            float newValue = Value + newValueDelta;
//            if(newValue < MinValue)
//                newValue = MinValue;
//            if(newValue > MaxValue)
//                newValue = MaxValue;
//
//            Value = newValue;
//
//            //SetValue(newValue, true);
//            Console.WriteLine("Slider - ContinueTracking - newValue: " + newValue.ToString() + " newValueDelta: " + newValueDelta.ToString() + " (" + ptPrev.X.ToString() + ", " + ptPrev.Y.ToString() + ") (" + pt.X.ToString() + ", " + pt.Y.ToString() + ") normalizedX: " + normalizedX.ToString() + " ratioX: " + ratioX.ToString());
//            
//
//
//            if(Continuous)
//                SendActionForControlEvents(UIControlEvent.ValueChanged);
//
//            return true;

            return base.ContinueTracking(uitouch, uievent);
        }

        public override void EndTracking(UITouch uitouch, UIEvent uievent)
        {
            if(Tracking)
            {
                scrubbingSpeed = _scrubbingSpeeds[0];
                SendActionForControlEvents(UIControlEvent.ValueChanged);
            }

            OnEndTrackingEvent(new EventArgs());
            //base.EndTracking(uitouch, uievent);
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

    public enum SliderScrubbingType
    {
        HighSpeed = 0,
        HalfSpeed = 1,
        QuarterSpeed = 2,
        Fine = 3
    }
}
