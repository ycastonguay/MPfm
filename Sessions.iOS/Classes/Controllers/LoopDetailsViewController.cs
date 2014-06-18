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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.MVP.Views;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.iOS
{
    public partial class LoopDetailsViewController : BaseViewController, ILoopDetailsView
    {
        public LoopDetailsViewController()
            : base (UserInterfaceIdiomIsPhone ? "LoopDetailsViewController_iPhone" : "LoopDetailsViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            //navigationManager.BindView(this);
        }

        partial void actionClose(NSObject sender)
        {
            this.DismissViewController(true, null);
        }
        
        partial void actionDeleteLoop(NSObject sender)
        {
        }

		#region ILoopDetailsView implementation

		public Action OnAddSegment { get; set; }
		public Action<Guid, int> OnAddSegmentFromMarker { get; set; }
		public Action<Segment> OnEditSegment { get; set; }
		public Action<Segment> OnDeleteSegment { get; set; }
		public Action<Loop> OnUpdateLoopDetails { get; set; }
		public Action<Segment, int> OnChangeSegmentOrder { get; set; }
		public Action<Segment, Guid> OnLinkSegmentToMarker { get; set; }

		public void LoopDetailsError(Exception ex)
		{
		}

		public void RefreshLoopDetails(Loop loop, AudioFile audioFile)
		{
		}

		#endregion
    }
}

