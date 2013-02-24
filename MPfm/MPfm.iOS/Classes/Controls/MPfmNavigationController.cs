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

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmNavigationController")]
    public class MPfmNavigationController : UINavigationController
    {
        UILabel _lblTitle;
        UILabel _lblSubtitle;
        UIButton _btnBack;
        UIButton _btnEffects;
        
        public MPfmNavigationController() : base()
        {
            this.WeakDelegate = this;

            _btnBack = new UIButton(UIButtonType.Custom);
            _btnBack.Frame = new RectangleF(4, 4, 36, 36);
            _btnBack.SetBackgroundImage(UIImage.FromBundle("Images/back.png"), UIControlState.Normal);
            _btnBack.TouchUpInside += (sender, e) => { 
                if(ViewControllers.Length > 1)
                {
                    PopViewControllerAnimated(true);
                }
            };

            _btnEffects = new UIButton(UIButtonType.RoundedRect);
            _btnEffects.Frame = new RectangleF(this.NavigationBar.Frame.Width - 4 - 36, 4, 36, 36);
            _btnEffects.SetBackgroundImage(UIImage.FromBundle("Images/effects.png"), UIControlState.Normal);

            _lblTitle = new UILabel(new RectangleF(50, 6, UIScreen.MainScreen.Bounds.Width - 100, 20));
            _lblTitle.TextColor = UIColor.White;
            _lblTitle.BackgroundColor = UIColor.Clear;
            _lblTitle.Text = "MPfm";
            _lblTitle.TextAlignment = UITextAlignment.Left;
            _lblTitle.Font = UIFont.FromName("OstrichSans-Black", 20);
            //_lblTitle.Font = UIFont.FromName("LeagueGothic-Regular", 20);

            _lblSubtitle = new UILabel(new RectangleF(50, 23, UIScreen.MainScreen.Bounds.Width - 100, 20));
            _lblSubtitle.LineBreakMode = UILineBreakMode.HeadTruncation;
            _lblSubtitle.TextColor = UIColor.LightGray;
            _lblSubtitle.BackgroundColor = UIColor.Clear;
            _lblSubtitle.Text = "Library Browser";
            _lblSubtitle.TextAlignment = UITextAlignment.Left;
            _lblSubtitle.Font = UIFont.FromName("OstrichSans-Black", 14);
            //_lblSubtitle.Font = UIFont.FromName("LeagueGothic-Regular", 16);

            this.NavigationBar.AddSubview(_btnBack);
            this.NavigationBar.AddSubview(_btnEffects);
            this.NavigationBar.AddSubview(_lblTitle);
            this.NavigationBar.AddSubview(_lblSubtitle);
        }

        [Export("navigationBar:shouldPushItem:")]
        public bool ShouldPushItem(UINavigationItem item)
        {
            if (ViewControllers.Length > 1)
            {
                UIView.Animate(0.25, () => { 
                    _btnBack.SetBackgroundImage(UIImage.FromBundle("Images/back_wide.png"), UIControlState.Normal);
                    _btnBack.Frame = new RectangleF(4, 4, 43, 36);
                    _lblTitle.Frame = new RectangleF(57, 6, UIScreen.MainScreen.Bounds.Width - 60, 20);
                    _lblSubtitle.Frame = new RectangleF(57, 23, UIScreen.MainScreen.Bounds.Width - 60, 20);
                });
            }

            return true;
        }

        [Export("navigationBar:shouldPopItem:")]
        public bool ShouldPopItem(UINavigationItem item)
        {
            if (ViewControllers.Length == 1)
            {
                UIView.Animate(0.25, () => { 
                    _btnBack.SetBackgroundImage(UIImage.FromBundle("Images/back.png"), UIControlState.Normal);
                    _btnBack.Frame = new RectangleF(4, 4, 36, 36);
                    _lblTitle.Frame = new RectangleF(50, 6, UIScreen.MainScreen.Bounds.Width - 60, 20);
                    _lblSubtitle.Frame = new RectangleF(50, 23, UIScreen.MainScreen.Bounds.Width - 60, 20);
                });
            }

            return true;
        }

        public void SetSubtitle(string subtitle)
        {
            UIView.Animate(0.25f, delegate
            { 
                _lblSubtitle.Alpha = 0;
            }, delegate
            {
                _lblSubtitle.Text = subtitle;
            });
            UIView.Animate(0.25f, delegate
            { 
                _lblSubtitle.Alpha = 1;
            });
        }
    }
}
