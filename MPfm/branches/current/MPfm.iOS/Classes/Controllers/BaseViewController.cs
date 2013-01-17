using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.MVP;

namespace MPfm.iOS
{
	public abstract class BaseViewController : UIViewController//, IBaseView
	{
        //public Action OnViewDestroy { get; set; } // Is this useful on iOS?
        //protected Action<IBaseView> OnViewReady { get; set; }

        public BaseViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

//        public void ShowView(bool shown)
//        {
//            this.View.Hidden = !shown;
//        }
	}
}

