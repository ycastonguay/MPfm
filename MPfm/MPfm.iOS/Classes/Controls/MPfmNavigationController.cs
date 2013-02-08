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
