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
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;

namespace MPfm.Mac.Classes.Controls
{
    /// <summary>
    /// Custom table cell view for view-based NSTableView. Displays an animation to indicate that the song is currently playing.
    /// </summary>
    [Register("MPfmIsPlayingTableCellView")]
    public class MPfmIsPlayingTableCellView : NSTableCellView
    {
        int animationFrame = 0;
        float padding = 6;
        NSTimer timer;

        public bool IsPlaying { get; private set; }
        public CGColor GradientColor1 { get; set; }
        public CGColor GradientColor2 { get; set; }

        [Export("init")]
        public MPfmIsPlayingTableCellView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmIsPlayingTableCellView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        void Initialize()
        {
            // Set default colors
            GradientColor1 = new CGColor(1.0f, 1.0f, 1.0f);
            GradientColor2 = new CGColor(0.8f, 0.8f, 0.8f);

            // Activate layers
            this.WantsLayer = true;
            //this.Layer.BackgroundColor = new CGColor(0.9f, 0.9f, 0.9f);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public override void ViewWillDraw()
        {
            base.ViewWillDraw();
        }

        public void SetIsPlaying(bool isPlaying)
        {
            this.IsPlaying = isPlaying;
            SetNeedsDisplayInRect(Bounds);
//
//            if (!isPlaying)
//            {
//                if(timer != null && timer.IsValid)
//                    timer.Invalidate();
//            }
//            else
//            {
//                if(Layer.AnchorPoint.X == 0 && Layer.AnchorPoint.Y == 0)
//                {
//                    CATransaction.Begin();
//                    CATransaction.SetValueForKey(new NSNumber(0.00005f), CATransaction.AnimationDurationKey); 
//                    Layer.AnchorPoint = new PointF(0.5f, 0.5f);
//                    Layer.Position = new PointF(Layer.Position.X + (Layer.Bounds.Size.Width * (Layer.AnchorPoint.X - 0f)), 
//                                                Layer.Position.Y + (Layer.Bounds.Size.Height * (Layer.AnchorPoint.Y - 0f)));
//    
//                    CATransaction.Commit();                
//                }
//
//                if(Layer != null)
//                {
//                    animationFrame = 2;
//                    timer = NSTimer.CreateRepeatingScheduledTimer(0.75f, delegate {
//
//                        float radians = 0;
//
//                        if(animationFrame < 3)
//                            animationFrame++;
//                        else
//                            animationFrame = 0;
//
//                        radians = (float)Math.PI * (animationFrame * 90.0f) / 180.0f;
//                        Console.WriteLine("====> AnimationFrame: " + animationFrame.ToString() + " -- Radians: " + radians.ToString());
//                        CATransaction.Begin();
//                        CATransaction.SetValueForKey(new NSNumber(0.75f), CATransaction.AnimationDurationKey); 
//                        CATransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
//
//                        CATransform3D transform = Layer.Transform;
//                        CATransform3D transform1 = Layer.Transform.Rotate(radians, 0, 0, 1.0f);
//                        CATransform3D transform2 = Layer.Transform.Rotate(radians, 0, 0, 1.0f);
//                        transform = transform.Concat(transform1);
//                        //transform = transform.Concat(transform2);
//                        this.Layer.Transform = transform;
//
//                        CATransaction.Commit();
//
//                        //a += 4;
//    //                    float radians = (float)Math.PI * a / 180.0f;
//    //                    Console.WriteLine("Rotating to " + radians.ToString("0.00"));
//    //
//    //                    CATransaction.Begin();
//    //                    CATransaction.SetValueForKey(new NSNumber(1), CATransaction.AnimationDurationKey); 
//    //
//    //                    CATransform3D transformRotation = this.Layer.Transform.Rotate(radians, 0.0f, 0.0f, 1.0f);
//    //                    this.Layer.Transform = transformRotation;
//    //
//    //                    CATransaction.Commit();
//
//                    });
//                }
//            }
        }

        public override void DrawRect(System.Drawing.RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);

            if(!IsPlaying)
                return;           

            // Save context state
            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
            context.SaveState();

            // Calculate size
            float size = (Bounds.Width > Bounds.Height) ? Bounds.Height : Bounds.Width;
            float circleRadius = (size - 4) / 2;
            RectangleF rect = new RectangleF(padding / 2, padding / 2, size - padding, size - padding);

            //CGColor color1 = new CGColor(0.0f, 0.5f, 0.0f);
            //CGColor color2 = new CGColor(0.0f, 1.0f, 0.0f);
            CGColor color1 = new CGColor(0.0f, 0.3f, 0.0f);
            CGColor color2 = new CGColor(0.65f, 1.0f, 0.65f);
            CGGradient gradientBackground;
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();            
            float[] locationListBackground = new float[] { 1.0f, 0.0f };
            List<float> colorListBackground = new List<float>();
            colorListBackground.AddRange(color1.Components);
            colorListBackground.AddRange(color2.Components);
            gradientBackground = new CGGradient(colorSpace, colorListBackground.ToArray(), locationListBackground);

            // Create path
            CGPath path = new CGPath();
            float x = Bounds.Width / 2;
            float y = Bounds.Height / 2;
            path.AddArc(x, y, circleRadius, 0, 2 * (float)Math.PI, false);
            path.AddArc(x, y, circleRadius / 3, 0, 2 * (float)Math.PI, false);
            context.AddPath(path);

            // Draw outline
            context.SetLineWidth(1.5f);
            //context.SetStrokeColor(new CGColor(0.35f, 0.6f, 0.35f));
            context.SetStrokeColor(new CGColor(0.65f, 0.65f, 0.65f));
            context.StrokePath();

            // Clip path and draw gradient inside
            context.AddPath(path);
            context.EOClip();
            context.DrawLinearGradient(gradientBackground, new PointF(0, 0), new PointF(0, Bounds.Height), CGGradientDrawingOptions.DrawsAfterEndLocation);

//            // Draw outline
//            context.AddPath(path);
//            //context.SetLineWidth(1.5f);
//            context.SetLineWidth(1.25f);
//            //context.SetStrokeColor(new CGColor(0.0f, 0.25f, 0.0f));
//            //context.SetStrokeColor(new CGColor(0.0f, 0.0f, 0.0f));
//            context.SetStrokeColor(new CGColor(0.65f, 0.65f, 0.65f));
//            context.StrokePath();

            // Restore state
            context.RestoreState();
        }
    }
}
