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
using Sessions.MVP.Views;
using Sessions.Player.Objects;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;

namespace Sessions.iOS
{
    public partial class LoopsViewController : BaseViewController, ILoopsView
    {
        public LoopsViewController()
			: base (UserInterfaceIdiomIsPhone ? "LoopsViewController_iPhone" : "LoopsViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
			tableView.BackgroundColor = UIColor.Clear;
            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;
            btnAddLoop.Layer.CornerRadius = 8;
            btnAddLoop.Layer.BackgroundColor = GlobalTheme.PlayerPanelButtonColor.CGColor;
            btnAddLoop.Alpha = GlobalTheme.PlayerPanelButtonAlpha;

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindLoopsView(this);
        }

        partial void actionAddLoop(NSObject sender)
        {
            if(OnAddLoop != null)
                OnAddLoop();
        }

        #region ILoopsView implementation

        public Action OnAddLoop { get; set; }
        public Action<Loop> OnEditLoop { get; set; }
        public Action<Loop> OnSelectLoop { get; set; }
        public Action<Loop> OnDeleteLoop { get; set; }
        public Action<Loop> OnUpdateLoop { get; set; }
        public Action<Loop> OnPlayLoop { get; set; }

        public Action<Segment> OnPunchInLoopSegment { get; set; }
        public Action<Segment, float> OnChangingLoopSegmentPosition { get; set; }
        public Action<Segment, float> OnChangedLoopSegmentPosition { get; set; }

        public void LoopError(Exception ex)
        {
            InvokeOnMainThread(() => {
                UIAlertView alertView = new UIAlertView("Loop Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshLoops(List<Loop> loops)
        {
        }

        public void RefreshLoopSegment(Segment segment, long audioFileLength)
        {
        }

        #endregion
    }
}
