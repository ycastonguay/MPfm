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

            activityIndicator.StartAnimating();
            btnOK.TitleLabel.Text = "Cancel";
            btnOK.SetImage(UIImage.FromBundle("Images/Buttons/cancel"));
            viewPanel.Layer.CornerRadius = 8;

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindCloudConnectView(this);
        }

        partial void actionOK(NSObject sender)
        {
            WillMoveToParentViewController(null);
            UIView.Animate(0.2f, () => {
                this.View.Alpha = 0;
            }, () => {
                View.RemoveFromSuperview();
                RemoveFromParentViewController();
            });
        }

        #region ICloudConnectView implementation

        public Action OnCheckIfAccountIsLinked { get; set; }

        public void CloudConnectError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshStatus(CloudConnectEntity entity)
        {
            InvokeOnMainThread(() =>
            {
                if(entity.HasAuthenticationFailed)
                {
                    lblStatus.Hidden = true;
                    lblStatusCenter.Hidden = false;
                    lblStatusCenter.Text = "Authentication failed.";
                    btnOK.TitleLabel.Text = "OK";
                    btnOK.SetImage(UIImage.FromBundle("Images/Buttons/select"));
                    btnOK.UpdateLayout();
                    activityIndicator.Hidden = true;
                }
                else if(entity.IsAuthenticated)
                {
                    lblStatus.Hidden = true;
                    lblStatusCenter.Hidden = false;
                    lblStatusCenter.Text = "Authentication successful!";
                    btnOK.TitleLabel.Text = "OK";
                    btnOK.SetImage(UIImage.FromBundle("Images/Buttons/select"));
                    btnOK.UpdateLayout();
                    activityIndicator.Hidden = true;
                }
                else
                {
                    lblStatus.Hidden = false;
                    lblStatusCenter.Hidden = true;
                }
            });
        }

        #endregion
    }
}
