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
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmPlaylistTableViewCell")]
    public class MPfmPlaylistTableViewCell : UITableViewCell
    {
        public UIButton RightButton { get; private set; }

        public MPfmPlaylistTableViewCell() : base()
        {
            Initialize();
        }

        public MPfmPlaylistTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        public MPfmPlaylistTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        public void Initialize()
        {
            var screenSize = UIKitHelper.GetDeviceSize();

            UIView backView = new UIView(Frame);
            backView.BackgroundColor = GlobalTheme.BackgroundColor;
            BackgroundView = backView;
            BackgroundColor = UIColor.White;
            
            UIView backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;           

            TextLabel.BackgroundColor = UIColor.FromWhiteAlpha(0, 0);
            TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 14);
            TextLabel.TextColor = UIColor.White;
            TextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.TextColor = UIColor.Gray;
            DetailTextLabel.HighlightedTextColor = UIColor.White;
            DetailTextLabel.Font = UIFont.FromName("HelveticaNeue", 12);
            ImageView.BackgroundColor = UIColor.White;

            RightButton = new UIButton(UIButtonType.Custom);
            RightButton.Hidden = true;
            RightButton.Frame = new RectangleF(screenSize.Width - Bounds.Height, 4, Bounds.Height, Bounds.Height);
            AddSubview(RightButton);

            // Make sure the text label is over all other subviews
            TextLabel.RemoveFromSuperview();
            AddSubview(TextLabel);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var screenSize = UIKitHelper.GetDeviceSize();
            float padding = 8;

            // Determine width available for text
            float textWidth = Bounds.Width;
            if (Accessory != UITableViewCellAccessory.None)
                textWidth -= 44;
            if (ImageView.Image != null && !ImageView.Hidden)
                textWidth -= 44 + padding;
            if (RightButton.ImageView.Image != null)
                textWidth -= 44 + padding;

            float x = 0;
            if (ImageView.Image != null)
            {
                ImageView.Frame = new RectangleF(x, 4, 44, 44);
                x += 44 + padding;
            } 
            else
            {
                x += padding;
            }

            float titleY = 10 + 4;
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                titleY = 2 + 4;

            //TextLabel.Frame = new RectangleF(x, titleY, textWidth, 22);
            if (!string.IsNullOrEmpty(DetailTextLabel.Text))
                DetailTextLabel.Frame = new RectangleF(x, 22 + 4, textWidth, 16);

            if (RightButton.ImageView.Image != null || !string.IsNullOrEmpty(RightButton.Title(UIControlState.Normal)))
                RightButton.Frame = new RectangleF(screenSize.Width - 44, 4, 44, 44);

        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            AnimatePress(true);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);
            base.TouchesCancelled(touches, evt);
        }

        private void AnimatePress(bool on)
        {
            if (!on)
            {
                UIView.Animate(0.2, () => {
                    //BackgroundColor = GlobalTheme.SecondaryColor;
                    //                    if (LabelAlignment == UIControlContentHorizontalAlignment.Left)
                    //                        TitleLabel.Frame = new RectangleF(TitleLabel.Frame.X + 8, TitleLabel.Frame.Y, TitleLabel.Frame.Width, TitleLabel.Frame.Height);
                    //                    else if (LabelAlignment == UIControlContentHorizontalAlignment.Right)
                    //                        TitleLabel.Frame = new RectangleF(TitleLabel.Frame.X - 4, TitleLabel.Frame.Y, TitleLabel.Frame.Width, TitleLabel.Frame.Height);

                    //TextLabel.Frame = new RectangleF(TextLabel.Frame.X - 26, TextLabel.Frame.Y, TextLabel.Frame.Width, TextLabel.Frame.Height);
                    //Console.WriteLine(">>>>>>>>>>> TVC - Scale 1");
                    if(TextLabel.Transform.xx < 0.95f) return; // Ignore when scale is lower; it was done on purpose and will be restored to 1 later.
                    TextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    //Image.Transform = CGAffineTransform.MakeScale(1, 1);
                });
            }
            else
            {
                UIView.Animate(0.2, () => {
                    //BackgroundColor = GlobalTheme.SecondaryDarkColor;
                    //Console.WriteLine(">>>>>>>>>>> TVC - Scale 0.95f");
                    TextLabel.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
                    //Image.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
                    //TextLabel.Frame = new RectangleF(TextLabel.Frame.X + 26, TextLabel.Frame.Y, TextLabel.Frame.Width, TextLabel.Frame.Height);
                    //                    if(LabelAlignment == UIControlContentHorizontalAlignment.Left)
                                            //TitleLabel.Frame = new RectangleF(TitleLabel.Frame.X - 8, TitleLabel.Frame.Y, TitleLabel.Frame.Width, TitleLabel.Frame.Height);
                    //                    else if(LabelAlignment == UIControlContentHorizontalAlignment.Right)
                    //                        TitleLabel.Frame = new RectangleF(TitleLabel.Frame.X + 4, TitleLabel.Frame.Y, TitleLabel.Frame.Width, TitleLabel.Frame.Height);
                });
            }
        }
    }
}
