using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MPfm.iOS
{
    public partial class ListViewController : BaseViewController
    {
        private UIBarButtonItem btnBack;
        private Action<GenericListItem> actionOnItemSelected;
        private ListTableViewSource tableViewSource;
        private string title;
        public List<GenericListItem> Items { get; private set; }

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public ListViewController(string title, List<GenericListItem> items, Action<GenericListItem> actionOnItemSelected)
            : base (UserInterfaceIdiomIsPhone ? "ListViewController_iPhone" : "ListViewController_iPad", null)
        {
            if (this.TabBarItem != null)
            {
                this.TabBarItem.Title = title;
                this.TabBarItem.Image = UIImage.FromBundle("Images/Tabs/more");
            }
            this.title = title;
            this.Items = items;
            this.actionOnItemSelected = actionOnItemSelected;
        }
        
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

//            // Set NavCtrl title if available
//            if(this.NavigationController != null)                
//                this.NavigationController.Title = this.TabBarItem.Title;

            // Create text attributes for navigation bar button
            UITextAttributes attr = new UITextAttributes();
            attr.Font = UIFont.FromName("OstrichSans-Black", 16);
            attr.TextColor = UIColor.White;
            attr.TextShadowColor = UIColor.DarkGray;
            attr.TextShadowOffset = new UIOffset(0, 0);
            
            // Set back button for navigation bar
            btnBack = new UIBarButtonItem(title, UIBarButtonItemStyle.Plain, null, null);
            btnBack.SetTitleTextAttributes(attr, UIControlState.Normal);
            this.NavigationItem.BackBarButtonItem = btnBack;

            // Load data source
            tableViewSource = new ListTableViewSource(Items, actionOnItemSelected);
            tableView.Source = tableViewSource;
        }
        
        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            
            // Clear any references to subviews of the main view in order to
            // allow the Garbage Collector to collect them sooner.
            //
            // e.g. myOutlet.Dispose (); myOutlet = null;
            
            ReleaseDesignerOutlets();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle(title);
        }
        
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            if (UserInterfaceIdiomIsPhone)
            {
                return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
            } else
            {
                return true;
            }
        }
    }
}

