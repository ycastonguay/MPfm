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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.Library.UpdateLibrary;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Models;
using MPfm.MVP.Views;

namespace MPfm.iOS
{
    public partial class UpdateLibraryViewController : BaseViewController, IUpdateLibraryView
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public UpdateLibraryViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "UpdateLibraryViewController_iPhone" : "UpdateLibraryViewController_iPad", null)
        {
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
			
            // Perform any additional setup after loading the view, typically from a nib.
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

        #region IUpdateLibraryView implementation
        
        public Action<UpdateLibraryMode, List<string>, string> OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }
        
        public void RefreshStatus(UpdateLibraryEntity entity)
        {
        }
        
        public void AddToLog(string entry)
        {
        }
        
        public void ProcessEnded(bool canceled)
        {
        }
        
        #endregion
    }
}

