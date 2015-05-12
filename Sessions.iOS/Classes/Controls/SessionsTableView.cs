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
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.Core;

namespace Sessions.iOS.Classes.Controls
{
	[Register("SessionsTableView")]
	public class SessionsTableView : UITableView
    {
		public bool BlockContentOffsetChange { get; set; }

		public SessionsTableView() 
			: base()
		{
			Initialize();
		}

		public SessionsTableView(IntPtr handle) 
			: base(handle)
		{
			Initialize();
		}

		private void Initialize()
		{
			BlockContentOffsetChange = false;
		}

		public override bool CanCancelContentTouches
		{
			get
			{
				return true;
			}
			set
			{

			}
		}

		public override PointF ContentOffset
		{
			get
			{
				//Tracing.Log("TableView - ContentOffset __GET {0}", base.ContentOffset);
				return base.ContentOffset;
			}
			set
			{
				//Tracing.Log("TableView - ContentOffset ::SET {0}", value);
				if(!BlockContentOffsetChange)
					base.ContentOffset = value;
			}
		}

		public override bool TouchesShouldCancelInContentView(UIView view)
		{
			//Tracing.Log("TableView - TouchesShouldCancelInContentView - viewType: {0}", view.GetType().FullName);
			if (view is UISlider)
				return false;

			return true;
		}

//		public override bool TouchesShouldBegin(NSSet touches, UIEvent withEvent, UIView inContentView)
//		{
//			Tracing.Log("TableView - TouchesShouldBegin - viewType: {0}", inContentView.GetType().FullName);
//
//			if (inContentView is UISlider)
//			{
//				var touch = (UITouch)withEvent.AllTouches.AnyObject;
//				var location = touch.LocationInView(inContentView);
//				var slider = (UISlider)inContentView;
//				var trackRect = slider.TrackRectForBounds(slider.Bounds);
//				var thumbRect = slider.ThumbRectForBounds(slider.Bounds, trackRect, slider.Value);
//				if (thumbRect.Contains(location))
//				{
//					Tracing.Log("TableView - TouchesShouldBegin - Touches should NOT BEGIN on slider");
//					return false;
//				}
//			}
//
//			return true;
//
//			//return base.TouchesShouldBegin(touches, withEvent, inContentView);
//		}
//
//		public override UIView HitTest(PointF point, UIEvent uievent)
//		{
//			var view = base.HitTest(point, uievent);
//			Tracing.Log("TableView - HitTest - point: {0} eventType: {1} view is slider: {2}", point, uievent.Type, view is UISlider);
//			ScrollEnabled = !(view is UISlider);
//			return view;
//		}
//
//		public override bool PointInside(PointF point, UIEvent uievent)
//		{
//			bool value = base.PointInside(point, uievent);
//			Tracing.Log("TableView - PointInside - point: {0} eventType: {1} baseValue: {2}", point, uievent.Type, value);
////			foreach (var view in Subviews)
////			{
////				if (!view.Hidden && view.UserInteractionEnabled && view.PointInside(ConvertPointToView(point, view), uievent))
////				{
////					Tracing.Log("TableView - PointInside __TRUE__ - point: {0} eventType: {1} baseValue: {2}", point, uievent.Type, value);
////					return true;
////				}
////			}
////			Tracing.Log("TableView - PointInside **FALSE** - point: {0} eventType: {1} baseValue: {2}", point, uievent.Type, value);
////			return false;
//
//			return value;
//		}
	}
}
