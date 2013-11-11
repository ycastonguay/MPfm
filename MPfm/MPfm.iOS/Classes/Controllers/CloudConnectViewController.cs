using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Views;
using MPfm.MVP.Models;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;

namespace MPfm.iOS
{
    public partial class CloudConnectViewController : BaseViewController, ICloudConnectView
    {
        public CloudConnectViewController()
			: base (UserInterfaceIdiomIsPhone ? "CloudConnectViewController_iPhone" : "CloudConnectViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindCloudConnectView(this);
        }

        partial void actionOK(NSObject sender)
        {

        }

        #region ICloudConnectView implementation

        public Action OnCheckIfAccountIsLinked { get; set; }

        public void CloudConnectError(Exception ex)
        {
        }

        public void RefreshStatus(CloudConnectEntity entity)
        {
        }

        #endregion
    }
}
