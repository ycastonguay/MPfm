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
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.MVP;
using MPfm.MVP.Views;

namespace MPfm.iOS.Classes.Controllers.Base
{
	public abstract class BaseViewController : UIViewController, IBaseView
	{
        #region IBaseView implementation

        public void ShowView(bool shown)
        {
        }

        public Action<IBaseView> OnViewDestroy { get; set; }

        #endregion

        protected Action<IBaseView> OnViewReady { get; set; }

        public BaseViewController(Action<IBaseView> onViewReady, string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
            OnViewReady = onViewReady;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            OnViewReady(this);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            OnViewDestroy(this);
        }
	}
}

