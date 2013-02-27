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
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmSlider")]
    public class MPfmSlider : UISlider
    {
        private bool _isTouchDown = false;

        public Action<float> OnTouchesMoved;

        public MPfmSlider(IntPtr handle) : base (handle)
        {
            this.Continuous = true;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            _isTouchDown = true;
            Console.WriteLine("Slider - TouchesBegan");
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("Slider - TouchesMoved");

            if (OnTouchesMoved != null)
                OnTouchesMoved(this.Value);

            base.TouchesMoved(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            _isTouchDown = false;
            Console.WriteLine("Slider - TouchesEnded");
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("Slider - TouchesCancelled");
            base.TouchesCancelled(touches, evt);
        }

//        [Export("mouseUp:")]
//        public override void MouseUp(NSEvent theEvent)
//        {
//            // Call super class
//            base.MouseUp(theEvent);
//            
//            // Get value
//            float value = this.FloatValue;
//            
//            // Set flag
//            isMouseDown = false;
//            
//            // Set player position
//            playerPresenter.SetPosition(value / 100);
//        }
//        
//        [Export("didChangeValue:")]
//        public override void DidChangeValue(string forKey)
//        {
//            base.DidChangeValue(forKey);
//        }       
        
        public void SetPosition(float position)
        {
            if (!_isTouchDown)
                this.Value = position;
        }

    }
}
