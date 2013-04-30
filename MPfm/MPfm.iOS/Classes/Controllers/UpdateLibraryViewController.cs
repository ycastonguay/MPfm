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
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS
{
    public partial class UpdateLibraryViewController : BaseViewController, IUpdateLibraryView
    {
        public UpdateLibraryViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "UpdateLibraryViewController_iPhone" : "UpdateLibraryViewController_iPad", null)
        {
        }
		
        public override void ViewDidLoad()
        {
            this.View.BackgroundColor = GlobalTheme.BackgroundColor;
            button.BackgroundColor = GlobalTheme.SecondaryColor;
            button.Layer.CornerRadius = 8;

            lblTitle.Font = UIFont.FromName("HelveticaNeue", 16);
            lblSubtitle.Font = UIFont.FromName("HelveticaNeue", 14);
            button.Font = UIFont.FromName("HelveticaNeue-Bold", 16);

            lblTitle.Text = "Initializing...";
            lblSubtitle.Text = string.Empty;

            base.ViewDidLoad();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            string musicPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            OnStartUpdateLibrary(UpdateLibraryMode.SpecificFolder, null, musicPath);
        }

        partial void actionButtonClicked(NSObject sender)
        {
            if(button.Title(UIControlState.Normal) == "OK")
            {
                DismissViewController(true, null);
            }
            else
            {
                OnCancelUpdateLibrary();
            }
        }

        #region IUpdateLibraryView implementation
        
        public Action<UpdateLibraryMode, List<string>, string> OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }
        
        public void RefreshStatus(UpdateLibraryEntity entity)
        {
            InvokeOnMainThread(() => {
                lblTitle.Text = entity.Title;
                lblSubtitle.Text = entity.Subtitle;
            });
        }
        
        public void AddToLog(string entry)
        {
        }
        
        public void ProcessEnded(bool canceled)
        {
            InvokeOnMainThread(() => {
                lblTitle.Text = "Update library successful.";
                lblSubtitle.Text = string.Empty;
                button.SetTitle("OK", UIControlState.Normal);
                activityIndicator.StopAnimating();
                activityIndicator.Hidden = true;
            });
        }
        
        #endregion
    }
}

