//
// CocoaHelper.cs: Helper static class for miscellaneous helper methods.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Mono.Unix;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;

namespace MPfm.Mac
{
    /// <summary>
    /// Helper static class for miscellaneous helper methods.
    /// </summary>
    public static class CocoaHelper
    {
        public static void ShowCriticalAlert(string text)
        {
            // Display error in a message box
            using(NSAlert alert = new NSAlert())
            {
                // Display alert
                alert.MessageText = text;
                alert.AlertStyle = NSAlertStyle.Critical;
                alert.RunModal();
            }
        }
    }
}

