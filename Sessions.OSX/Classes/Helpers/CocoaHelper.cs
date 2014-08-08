// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreAnimation;

namespace Sessions.OSX.Classes.Helpers
{
    /// <summary>
    /// Helper static class for miscellaneous helper methods.
    /// </summary>
    public static class CocoaHelper
    {
        public static void ShowAlert(string title, string text, NSAlertStyle alertStyle)
        {
            using(NSAlert alert = new NSAlert())
            {
                alert.MessageText = title;
                alert.InformativeText = text;
                alert.AlertStyle = alertStyle;
                alert.RunModal();
            }
        }

        public static void FadeIn(NSObject target, double duration, Action completed = null)
        {
            var dict = new NSMutableDictionary();
            dict.Add(NSViewAnimation.TargetKey, target);
            dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeInEffect);
            var anim = new NSViewAnimation(new List<NSMutableDictionary>(){ dict }.ToArray());
            anim.Duration = duration;
            anim.AnimationDidEnd += (sender, e) => {
                if(completed != null)
                    completed();
            };
            anim.StartAnimation();
        }

        public static void FadeIn2(NSView target, double duration, Action completed = null)
        {
            var anim = CABasicAnimation.FromKeyPath("alphaValue");
            anim.Duration = duration;
            anim.From = NSValue.FromObject(0);
            anim.To = NSValue.FromObject(1);
            //åtargetanim
        }

        public static void FadeOut(NSObject target, double duration, Action completed = null)
        {
            var dict = new NSMutableDictionary();
            dict.Add(NSViewAnimation.TargetKey, target);
            dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeOutEffect);
            var anim = new NSViewAnimation(new List<NSMutableDictionary>(){ dict }.ToArray());
            anim.Duration = duration;
            anim.AnimationDidEnd += (sender, e) => {
                if(completed != null)
                    completed();
            };
            anim.StartAnimation();
        }
    }
}
