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
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MPfm.iOS
{
    [Register("MPfmNavigationController")]
    public class MPfmNavigationController : UINavigationController
    {
        UILabel labelTitle;
        
        public MPfmNavigationController(string fontName, float fontSize) : base()
        {
            // Create title label
            labelTitle = new UILabel(new RectangleF(0, 3, UIScreen.MainScreen.Bounds.Width, 40));
            labelTitle.TextColor = UIColor.White;
            labelTitle.BackgroundColor = UIColor.Clear;
            labelTitle.Text = string.Empty;
            labelTitle.TextAlignment = UITextAlignment.Center;
            labelTitle.Font = UIFont.FromName(fontName, fontSize);
            
            // Add controls
            this.NavigationBar.AddSubview(labelTitle);
        }
        
        public void SetTitle(string title)
        {
            this.NavigationItem.Title = string.Empty;
            
            UIView.Animate(0.25f, delegate
            { 
                labelTitle.Alpha = 0;
            }, delegate
            {
                labelTitle.Text = title;
            });
            UIView.Animate(0.25f, delegate
            { 
                labelTitle.Alpha = 1;
            });
        }
    }
}
