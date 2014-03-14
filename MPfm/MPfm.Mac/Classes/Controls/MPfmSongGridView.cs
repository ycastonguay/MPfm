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
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.Mac.Classes.Controls.Graphics;
using MPfm.Mac.Classes.Controls.Helpers;
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
        private HorizontalScrollBarWrapper _horizontalScrollBar;
        private VerticalScrollBarWrapper _verticalScrollBar;
        private NSMenu _menuItems;
        private NSMenu _menuHeader;
        private NSEvent _rightClickEvent;

        public List<SongGridViewItem> SelectedItems { get { return _control.SelectedItems; } }
        public Guid NowPlayingAudioFileId { get { return _control.NowPlayingAudioFileId; } set { _control.NowPlayingAudioFileId = value; } }

        //public override bool WantsDefaultClipping { get { return false; } }
        public override bool IsOpaque { get { return true; } }
        public override bool IsFlipped { get { return true; } }

        public event EventHandler DoubleClick;

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
            DoubleClick += (sender, e) => { };

            // Add tracking area to receive mouse move and mouse dragged events
            var opts = NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.InVisibleRect | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.EnabledDuringMouseDrag;
            var trackingArea = new NSTrackingArea(Bounds, opts, this, new NSDictionary());
            AddTrackingArea(trackingArea);

            _horizontalScrollBar = new HorizontalScrollBarWrapper();
            AddSubview(_horizontalScrollBar);

            _verticalScrollBar = new VerticalScrollBarWrapper();
            AddSubview(_verticalScrollBar);

            var disposableImageFactory = Bootstrapper.GetContainer().Resolve<IDisposableImageFactory>();
            _control = new SongGridViewControl(_horizontalScrollBar, _verticalScrollBar, disposableImageFactory);   
            _control.OnChangeMouseCursorType += GenericControlHelper.ChangeMouseCursor;
            _control.OnItemDoubleClick += (id, index) => DoubleClick(this, new EventArgs());
            _control.OnInvalidateVisual += () => InvokeOnMainThread(() => SetNeedsDisplayInRect(Bounds));
            _control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));
            _control.OnDisplayContextMenu += (contextMenuType, x, y) => 
            { 
                switch (contextMenuType)
                {
                    case SongGridViewControl.ContextMenuType.Item:
                        NSMenu.PopUpContextMenu(_menuItems, _rightClickEvent, this);
                        break;
                    case SongGridViewControl.ContextMenuType.Header:
                        NSMenu.PopUpContextMenu(_menuHeader, _rightClickEvent, this);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            SetFrame();
            PostsBoundsChangedNotifications = true;
            NSNotificationCenter.DefaultCenter.AddObserver(NSView.FrameChangedNotification, FrameDidChangeNotification, this);

            CreateContextMenus();
        }

        private void CreateContextMenus()
        {
            _menuItems = new NSMenu();
            var menuItemAddToPlaylist = new NSMenuItem("Add to playlist");
            var menuItemPlaySong = new NSMenuItem("Play song(s)");
            _menuItems.AddItem(menuItemAddToPlaylist);
            _menuItems.AddItem(menuItemPlaySong);

            _menuHeader = new NSMenu();
            foreach (var column in _control.Columns)
            {
                var menuItem = new NSMenuItem(column.FieldName);
                _menuHeader.AddItem(menuItem);
            }
        }

        private void FrameDidChangeNotification(NSNotification notification)
        {
            //Console.WriteLine("WaveFormScrollView - NSViewFrameDidChangeNotification - Bounds: {0} Frame: {1}", Bounds, Frame);
            SetFrame();
        }

        private void SetFrame()
        {
            _horizontalScrollBar.Frame = new RectangleF(0, Bounds.Height - 20, Bounds.Width, 20);
            _verticalScrollBar.Frame = new RectangleF(Bounds.Width - 20, 20, 20, Bounds.Height - 40);
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
        
        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);
            GenericControlHelper.MouseDown(this, _control, theEvent);
            if (theEvent.ClickCount == 1)
                GenericControlHelper.MouseClick(this, _control, theEvent);
            else if (theEvent.ClickCount == 2)
                GenericControlHelper.MouseDoubleClick(this, _control, theEvent);
        }

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            GenericControlHelper.MouseUp(this, _control, theEvent);
            Window.MakeFirstResponder(this);
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            base.MouseDragged(theEvent);
            GenericControlHelper.MouseMove(this, _control, theEvent);
        }

        public override void RightMouseDown(NSEvent theEvent)
        {
            base.RightMouseDown(theEvent);
            _rightClickEvent = theEvent;
            GenericControlHelper.MouseDown(this, _control, theEvent, true);
            GenericControlHelper.MouseClick(this, _control, theEvent, true);
        }

        public override void RightMouseUp(NSEvent theEvent)
        {
            base.RightMouseUp(theEvent);
            _rightClickEvent = theEvent;
            GenericControlHelper.MouseUp(this, _control, theEvent, true);
            Window.MakeFirstResponder(this);
        }

        public override void RightMouseDragged(NSEvent theEvent)
        {
            base.RightMouseDragged(theEvent);
            GenericControlHelper.MouseMove(this, _control, theEvent);
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
            //Console.WriteLine("ScrollWheel - deltaX: {0} deltaY: {1}", theEvent.DeltaX, theEvent.DeltaY);
            base.ScrollWheel(theEvent);

            if (theEvent.DeltaY > 0)
                _control.MouseWheel(2);
            else if (theEvent.DeltaY < 0)
                _control.MouseWheel(-2);
        }

        public override void KeyDown(NSEvent theEvent)
        {
            //base.KeyDown(theEvent); // This makes the OS beep
            GenericControlHelper.KeyDown(_control, theEvent);
        }

        public override void KeyUp(NSEvent theEvent)
        {
            base.KeyUp(theEvent);
            GenericControlHelper.KeyUp(_control, theEvent);
        }
    }
}
