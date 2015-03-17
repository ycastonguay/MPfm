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
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Sessions.GenericControls.Controls;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.MVP.Bootstrap;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;
using Sessions.OSX.Classes.Controls.Graphics;
using Sessions.OSX.Classes.Controls.Helpers;
using Sessions.Sound.Player;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsPlaylistListView")]
    public class SessionsPlaylistListView : NSView
    {
        private PlaylistListViewControl _control;
        private HorizontalScrollBarWrapper _horizontalScrollBar;
        private VerticalScrollBarWrapper _verticalScrollBar;
        private NSMenu _menuItems;
        private NSEvent _rightClickEvent;

        public List<AudioFile> SelectedAudioFiles { get { return _control.SelectedAudioFiles; } }
        public int NowPlayingPlaylistItemId { get { return _control.NowPlayingPlaylistItemId; } set { _control.NowPlayingPlaylistItemId = value; } }

        //public override bool WantsDefaultClipping { get { return false; } }
        public override bool IsOpaque { get { return true; } }
        public override bool IsFlipped { get { return true; } }

        public delegate void MenuItemClickedDelegate(MenuItemType menuItemType);
        public event EventHandler DoubleClick;
        public event MenuItemClickedDelegate MenuItemClicked;

        [Export("init")]
        public SessionsPlaylistListView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsPlaylistListView(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        private void Initialize()
        {
            WantsLayer = true;
            DoubleClick += (sender, e) => { };
            MenuItemClicked += (menuItemType) => { };

            // Add tracking area to receive mouse move and mouse dragged events
            var opts = NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.InVisibleRect | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.EnabledDuringMouseDrag;
            var trackingArea = new NSTrackingArea(Bounds, opts, this, new NSDictionary());
            AddTrackingArea(trackingArea);

            _horizontalScrollBar = new HorizontalScrollBarWrapper();
            AddSubview(_horizontalScrollBar);

            _verticalScrollBar = new VerticalScrollBarWrapper();
            AddSubview(_verticalScrollBar);

            var albumArtRequestService = Bootstrapper.GetContainer().Resolve<IAlbumArtRequestService>();
            var albumArtCacheService = Bootstrapper.GetContainer().Resolve<IAlbumArtCacheService>();
            _control = new PlaylistListViewControl(_horizontalScrollBar, _verticalScrollBar, albumArtRequestService, albumArtCacheService);   
            _control.OnChangeMouseCursorType += GenericControlHelper.ChangeMouseCursor;
            _control.OnItemDoubleClick += (index) => DoubleClick(this, new EventArgs());
            _control.OnInvalidateVisual += () => InvokeOnMainThread(() => SetNeedsDisplayInRect(Bounds));
            _control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));
            _control.OnDisplayContextMenu += (contextMenuType, x, y) => 
            { 
                switch (contextMenuType)
                {
                    case PlaylistListViewControl.ContextMenuType.Item:
                        NSMenu.PopUpContextMenu(_menuItems, _rightClickEvent, this);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            SetFrame();
            PostsBoundsChangedNotifications = true;
            NSNotificationCenter.DefaultCenter.AddObserver(NSView.FrameChangedNotification, FrameDidChangeNotification, this);

            CreateContextualMenu();
        }

        private void CreateContextualMenu()
        {
            _menuItems = new NSMenu("Playlist");
            var menuItemRemoveSongs = new NSMenuItem("Remove song(s)", RemoveSongs);

            _menuItems.AddItem(menuItemRemoveSongs);
        }

        private void RemoveSongs(object sender, EventArgs args)
        {
            MenuItemClicked(MenuItemType.RemoveSongs);
        }

        private void FrameDidChangeNotification(NSNotification notification)
        {
            //Console.WriteLine("WaveFormScrollView - NSViewFrameDidChangeNotification - Bounds: {0} Frame: {1}", Bounds, Frame);
            SetFrame();
        }

        private void SetFrame()
        {
            _horizontalScrollBar.Frame = new RectangleF(0, Bounds.Height - 20, Bounds.Width, 20);
            _verticalScrollBar.Frame = new RectangleF(Bounds.Width - 20, 0, 20, Bounds.Height);
        }

        public void SetPlaylist(Playlist playlist)
        {
            _control.SetPlaylist(playlist);
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            //Console.WriteLine("SongGridView - DrawRect - dirtyRect: {0}", dirtyRect);
            base.DrawRect(dirtyRect);
            
            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
            context.SaveState();
            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height, GenericControlHelper.ToBasicRect(dirtyRect));
            _control.Render(wrapper);
            context.RestoreState();
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

        public enum MenuItemType
        {
            RemoveSongs = 0
        }
    }
}
