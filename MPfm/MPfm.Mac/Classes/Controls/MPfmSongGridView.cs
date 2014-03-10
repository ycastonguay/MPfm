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
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.MVP;
using MPfm.Mac.Classes.Objects;
using MPfm.Mac.Classes.Helpers;
using MPfm.GenericControls.Controls;
using MPfm.Mac.Classes.Controls.Graphics;
using MPfm.Mac.Classes.Controls.Helpers;
using MPfm.GenericControls.Interaction;
using MPfm.GenericControls.Controls.Songs;
using MPfm.GenericControls.Graphics;
using MPfm.MVP.Bootstrap;
using MPfm.Sound.AudioFiles;

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmSongGridView")]
    public class MPfmSongGridView : NSView
    {
        private SongGridViewControl _control;

        public List<SongGridViewItem> SelectedItems { get { return _control.SelectedItems; } }
        public Guid NowPlayingAudioFileId { get { return _control.NowPlayingAudioFileId; } set { _control.NowPlayingAudioFileId = value; } }

        //public override bool WantsDefaultClipping { get { return false; } }
        public override bool IsOpaque { get { return true; } }
        public override bool IsFlipped { get { return true; } }
        
        [Export("init")]
        public MPfmSongGridView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmSongGridView(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        private void Initialize()
        {
            // Add tracking area to receive mouse move and mouse dragged events
            var opts = NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.InVisibleRect | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.EnabledDuringMouseDrag;
            var trackingArea = new NSTrackingArea(Bounds, opts, this, new NSDictionary());
            AddTrackingArea(trackingArea);

            var horizontalScrollBar = new HorizontalScrollBarWrapper();
            horizontalScrollBar.Frame = new RectangleF(0, 0, Bounds.Width, 20);
            AddSubview(horizontalScrollBar);

            var verticalScrollBar = new VerticalScrollBarWrapper();
            verticalScrollBar.Frame = new RectangleF(0, 0, 20, Bounds.Height);
            AddSubview(verticalScrollBar);

            var disposableImageFactory = Bootstrapper.GetContainer().Resolve<IDisposableImageFactory>();

            _control = new SongGridViewControl(horizontalScrollBar, verticalScrollBar, disposableImageFactory);   
            _control.OnInvalidateVisual += () => InvokeOnMainThread(() => SetNeedsDisplayInRect(Bounds));
            _control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));

        }

        public void ImportAudioFiles(List<AudioFile> audioFiles)
        {
            _control.ImportAudioFiles(audioFiles);
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            //Console.WriteLine("SongGridView - DrawRect - dirtyRect: {0}", dirtyRect);
            base.DrawRect(dirtyRect);
            
            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height);
            _control.Render(wrapper);
        }
        
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            GenericControlHelper.MouseUp(this, _control, theEvent);
        }
        
        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);
            GenericControlHelper.MouseDown(this, _control, theEvent);
        }
        
        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
            GenericControlHelper.MouseMove(this, _control, theEvent);
        }

        public override void MouseEntered(NSEvent theEvent)
        {
            base.MouseEntered(theEvent);
            _control.MouseEnter();
        }

        public override void MouseExited(NSEvent theEvent)
        {
            base.MouseExited(theEvent);
            _control.MouseLeave();
        }

        public override void ScrollWheel(NSEvent theEvent)
        {
            Console.WriteLine("ScrollWheel - deltaX: {0} deltaY: {1}", theEvent.DeltaX, theEvent.DeltaY);
            base.ScrollWheel(theEvent);

            if (theEvent.DeltaY > 0)
                _control.MouseWheel(2);
            else if (theEvent.DeltaY < 0)
                _control.MouseWheel(-2);
        }
    }
}
