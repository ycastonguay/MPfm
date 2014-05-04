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
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.GenericControls.Controls;
using MPfm.Sound.AudioFiles;
using System.Collections.Generic;
using MPfm.Player.Objects;
using MPfm.Mac.Classes.Helpers;
using MonoMac.CoreGraphics;
using System.Drawing;
using MPfm.Mac.Classes.Controls.Helpers;
using MPfm.GenericControls.Basics;
using System.Timers;
using MPfm.GenericControls.Services;

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmWaveFormScrollView")]
    public class MPfmWaveFormScrollView : NSView
    {
        private bool _isDragging;
        private float _startDragContentOffsetX;
        private PointF _startDragLocation;
        private NSTextField _lblZoom;
        private Timer _timerFadeOutZoomLabel;
        private DateTime _lastZoomUpdate;
        private long _waveFormLength = 0;

        private NSMenu _menu;
        private NSMenu _menuDisplayType;
        private NSMenuItem _menuItemSelect;
        private NSMenuItem _menuItemZoomIn;
        private NSMenuItem _menuItemZoomOut;
        private NSMenuItem _menuItemResetZoom;
        private NSMenuItem _menuItemAutoScroll;
        private NSMenuItem _menuItemDisplayType;
        private NSMenuItem _menuItemDisplayTypeStereo;
        private NSMenuItem _menuItemDisplayTypeMono;
        private NSMenuItem _menuItemDisplayTypeMonoLeft;
        private NSMenuItem _menuItemDisplayTypeMonoRight;

        public MPfmWaveFormView WaveFormView { get; private set; }
        public MPfmWaveFormScaleView WaveFormScaleView { get; private set; }
        public override bool IsFlipped { get { return true; } }
        public bool IsAutoScrollEnabled { get; set; }

        private float _zoom = 1;
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _lblZoom.StringValue = string.Format("{0:0}%", value * 100);
                _zoom = value;
                WaveFormView.Zoom = value;
                WaveFormScaleView.Zoom = value;
                _lastZoomUpdate = DateTime.Now;
                
                if (_lblZoom.AlphaValue == 0)
                {
                    NSAnimationContext.BeginGrouping();
                    NSAnimationContext.CurrentContext.Duration = 0.2;
                    (_lblZoom.Animator as NSTextField).AlphaValue = 1;
                    NSAnimationContext.EndGrouping();
                }
            }
        }

        public event WaveFormControl.ChangePosition OnChangePosition;
        public event WaveFormControl.ChangePosition OnChangeSecondaryPosition;

        [Export("init")]
        public MPfmWaveFormScrollView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmWaveFormScrollView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
//            // Add tracking area to receive mouse move and mouse dragged events
//            var opts = NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.InVisibleRect | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.EnabledDuringMouseDrag;
//            var trackingArea = new NSTrackingArea(Bounds, opts, this, new NSDictionary());
//            AddTrackingArea(trackingArea);

            PostsBoundsChangedNotifications = true;
            NSNotificationCenter.DefaultCenter.AddObserver(NSView.FrameChangedNotification, FrameDidChangeNotification, this);
            
            WaveFormView = new MPfmWaveFormView();
            WaveFormView.OnChangePosition += (position) => OnChangePosition(position);
            WaveFormView.OnChangeSecondaryPosition += (position) => OnChangeSecondaryPosition(position);
            WaveFormView.OnContentOffsetChanged += (offset) => SetContentOffsetX(offset.X);
            AddSubview(WaveFormView);

            WaveFormScaleView = new MPfmWaveFormScaleView();
            AddSubview(WaveFormScaleView); 
            
            _lblZoom = new NSTextField();
            _lblZoom.WantsLayer = true;
            _lblZoom.AlphaValue = 0;
            _lblZoom.Bezeled = false;
            _lblZoom.Editable = false;
            _lblZoom.Selectable = false;
            _lblZoom.StringValue = "100.0%";
            _lblZoom.Font = NSFont.FromFontName("Roboto Medium", 11f);
            _lblZoom.Alignment = NSTextAlignment.Center;
            _lblZoom.BackgroundColor = NSColor.Clear;
            //_lblZoom.BackgroundColor = NSColor.FromDeviceRgba(0.1f, 0.1f, 0.1f, 0.75f);
            _lblZoom.Layer.BackgroundColor = NSColor.FromDeviceRgba(0.2f, 0.2f, 0.2f, 0.6f).CGColor;
            _lblZoom.TextColor = NSColor.White;
            AddSubview(_lblZoom);
            
            _timerFadeOutZoomLabel = new Timer(100);
            _timerFadeOutZoomLabel.Elapsed += HandleTimerFadeOutZoomLabelElapsed;
            _timerFadeOutZoomLabel.Start();
            
            CreateContextualMenu();
            SetFrame();
        }

        private void HandleTimerFadeOutZoomLabelElapsed(object sender, ElapsedEventArgs e)
        {
            InvokeOnMainThread(() => {
                //Console.WriteLine("HandleTimerFadeOutZoomLabelElapsed - _lblZoom.AlphaValue: {0} - timeSpan since last update: {1}", _lblZoom.AlphaValue, DateTime.Now - _lastZoomUpdate);
                if (_lblZoom.AlphaValue == 1 && DateTime.Now - _lastZoomUpdate > new TimeSpan(0, 0, 0, 0, 700))
                {
                    //Console.WriteLine("HandleTimerFadeOutZoomLabelElapsed - Fade out");
                    NSAnimationContext.BeginGrouping();
                    NSAnimationContext.CurrentContext.Duration = 0.2;
                    (_lblZoom.Animator as NSTextField).AlphaValue = 0;
                    NSAnimationContext.EndGrouping();
                    
                    //WaveFormView.RefreshWaveFormBitmap();
                }
            });
        }
        
        private void CreateContextualMenu()
        {
            // Need to create member variables or the application will crash
            _menu = new NSMenu("Wave Form");
            _menuItemSelect = new NSMenuItem("Select", MenuSelect);
            _menuItemAutoScroll = new NSMenuItem("Enable automatic scrolling", MenuAutoScroll);
            _menuItemSelect.State = NSCellStateValue.On;
            _menuItemZoomIn = new NSMenuItem("Zoom in", MenuZoomIn);
            _menuItemZoomOut = new NSMenuItem("Zoom out", MenuZoomOut);
            _menuItemResetZoom = new NSMenuItem("Reset zoom", MenuResetZoom);
            _menuItemDisplayType = new NSMenuItem("Display type");
            _menuItemDisplayTypeStereo = new NSMenuItem("Stereo", MenuDisplayType);
            _menuItemDisplayTypeStereo.State = NSCellStateValue.On;
            _menuItemDisplayTypeMono = new NSMenuItem("Mono (Mix)", MenuDisplayType);
            _menuItemDisplayTypeMonoLeft = new NSMenuItem("Mono (Left)", MenuDisplayType);
            _menuItemDisplayTypeMonoRight = new NSMenuItem("Mono (Right)", MenuDisplayType);

            _menuDisplayType = new NSMenu("Display Type");
            _menuDisplayType.AddItem(_menuItemDisplayTypeStereo);
            _menuDisplayType.AddItem(_menuItemDisplayTypeMono);
            _menuDisplayType.AddItem(_menuItemDisplayTypeMonoLeft);
            _menuDisplayType.AddItem(_menuItemDisplayTypeMonoRight);
            _menuItemDisplayType.Submenu = _menuDisplayType;

            _menu.AddItem(_menuItemSelect);
            _menu.AddItem(_menuItemZoomIn);
            _menu.AddItem(_menuItemZoomOut);
            _menu.AddItem(NSMenuItem.SeparatorItem);
            _menu.AddItem(_menuItemResetZoom);
            _menu.AddItem(_menuItemAutoScroll);
            _menu.AddItem(_menuItemDisplayType);
        }

        private void MenuSelect(object sender, EventArgs args)
        {
            ResetMenuSelection();
            _menuItemSelect.State = NSCellStateValue.On;
            WaveFormView.InteractionMode = WaveFormControl.InputInteractionMode.Select;
        }

        private void MenuZoomIn(object sender, EventArgs args)
        {
            ResetMenuSelection();
            _menuItemZoomIn.State = NSCellStateValue.On;
            WaveFormView.InteractionMode = WaveFormControl.InputInteractionMode.ZoomIn;
        }

        private void MenuZoomOut(object sender, EventArgs args)
        {
            ResetMenuSelection();
            _menuItemZoomOut.State = NSCellStateValue.On;
            WaveFormView.InteractionMode = WaveFormControl.InputInteractionMode.ZoomOut;
        }

        private void MenuResetZoom(object sender, EventArgs args)
        {
            Zoom = 1;
            SetContentOffsetX(0);
        }

        private void MenuAutoScroll(object sender, EventArgs args)
        {
            IsAutoScrollEnabled = !IsAutoScrollEnabled;
            _menuItemAutoScroll.State = IsAutoScrollEnabled ? NSCellStateValue.On : NSCellStateValue.Off;
        }

        private void MenuDisplayType(object sender, EventArgs args)
        {
            var displayType = WaveFormDisplayType.Stereo;
            if (sender == _menuItemDisplayTypeMono)
                displayType = WaveFormDisplayType.Mix;
            else if (sender == _menuItemDisplayTypeMonoLeft)
                displayType = WaveFormDisplayType.LeftChannel;
            else if (sender == _menuItemDisplayTypeMonoRight)
                displayType = WaveFormDisplayType.RightChannel;
            WaveFormView.DisplayType = displayType;

            ResetMenuDisplayTypeSelection();
            var menuItem = sender as NSMenuItem;
            if(menuItem != null)
                menuItem.State = NSCellStateValue.On;
        }
        
        private void ResetMenuSelection()
        {
            _menuItemSelect.State = NSCellStateValue.Off;
            _menuItemZoomIn.State = NSCellStateValue.Off;
            _menuItemZoomOut.State = NSCellStateValue.Off;
        }

        private void ResetMenuDisplayTypeSelection()
        {
            _menuItemDisplayTypeStereo.State = NSCellStateValue.Off;
            _menuItemDisplayTypeMono.State = NSCellStateValue.Off;
            _menuItemDisplayTypeMonoLeft.State = NSCellStateValue.Off;
            _menuItemDisplayTypeMonoRight.State = NSCellStateValue.Off;
        }

        private void FrameDidChangeNotification(NSNotification notification)
        {
            Console.WriteLine("WaveFormScrollView - NSViewFrameDidChangeNotification - Bounds: {0} Frame: {1}", Bounds, Frame);
            //WaveFormView.
            SetFrame();
        }

        private void SetFrame()
        {
            _lblZoom.Frame = new RectangleF((Frame.Width - 42) / 2f, ((Frame.Height - 16 - 22) / 2f) + 22, 42, 20);
            WaveFormScaleView.Frame = new RectangleF(0, 0, Frame.Width, 22);
            WaveFormView.Frame = new RectangleF(0, 22, Frame.Width, Frame.Height - 22);
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            WaveFormView.LoadPeakFile(audioFile);
            WaveFormScaleView.AudioFile = audioFile;
            WaveFormView.ContentOffset.X = 0;                
            WaveFormView.Zoom = 1;
            WaveFormScaleView.ContentOffset.X = 0;                
            WaveFormScaleView.Zoom = 1;
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            _waveFormLength = lengthBytes;
            WaveFormView.SetWaveFormLength(lengthBytes);
            WaveFormScaleView.AudioFileLength = lengthBytes;
        }

        public void SetPosition(long position)
        {
            WaveFormView.Position = position;
            if(IsAutoScrollEnabled)
                ProcessAutoScroll(position);
        }

        public void SetSecondaryPosition(long position)
        {
            WaveFormView.SecondaryPosition = position;
            ProcessAutoScroll(position);
        }

        public void ShowSecondaryPosition(bool show)
        {
            WaveFormView.ShowSecondaryPosition = show;
        }

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            WaveFormView.SetMarkers(markers);
        }

        private void ProcessAutoScroll(long position)
        {
            if (_zoom == 1)
                return;

            float waveFormWidth = WaveFormView.Bounds.Width * Zoom;
            float positionPercentage = (float)position / (float)_waveFormLength;
            float cursorX = positionPercentage * waveFormWidth;
            //float scrollStartX = WaveFormView.ContentOffset.X;
            float scrollCenterX = WaveFormView.ContentOffset.X + (Bounds.Width / 2);
            //float scrollEndX = Bounds.Width + WaveFormView.ContentOffset.X;
            //Console.WriteLine("WaveFormScrollView - AutoScroll - positionPct: {0} cursorX: {1} contentOffset.X: {2} waveFormView.Width: {3} scrollStartX: {4} scrollCenterX: {5} scrollEndX: {6}", positionPercentage, cursorX, WaveFormView.ContentOffset.X, WaveFormView.Bounds.Width, scrollStartX, scrollCenterX, scrollEndX);

            if (cursorX != scrollCenterX)
            {
                if (cursorX < scrollCenterX)
                {
                    //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is left of center X!");
                    if (WaveFormView.ContentOffset.X >= 0)
                    {
                        float newContentOffsetX = cursorX - (Bounds.Width / 2f);
                        newContentOffsetX = newContentOffsetX < 0 ? 0 : newContentOffsetX;
                        //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the left; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", WaveFormView.ContentOffset.X, newContentOffsetX);
                        SetContentOffsetX(newContentOffsetX);
                    }
                }
                else if(cursorX > scrollCenterX)
                {
                    //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is right of center X!");
                    if (WaveFormView.ContentOffset.X < waveFormWidth - Bounds.Width)
                    {
                        float newContentOffsetX = cursorX - (Bounds.Width / 2f);
                        newContentOffsetX = newContentOffsetX >  waveFormWidth - Bounds.Width ?  waveFormWidth - Bounds.Width : newContentOffsetX;
                        //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the right; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", WaveFormView.ContentOffset.X, newContentOffsetX);
                        SetContentOffsetX(newContentOffsetX);
                    }
                }
            }
        }

        public override void ScrollWheel(NSEvent theEvent)
        {
            base.ScrollWheel(theEvent);
            var location = GenericControlHelper.GetMouseLocation(this, theEvent);
            //Console.WriteLine("====> Wheel location: {0}", location);

            //Console.WriteLine("ScrollWheel - deltaX: {0} deltaY: {1}", theEvent.DeltaX, theEvent.DeltaY);
            if (theEvent.DeltaX > 0.2f || theEvent.DeltaX < -0.2f)
            {
                // Scroll left/right using a trackpad
                float deltaX = -theEvent.DeltaX * 2;
                SetContentOffsetX(WaveFormView.ContentOffset.X + deltaX);
                return;
            }
            
            // For some reason, the deltaY is always zero when holding SHIFT... 
            // Is it possible to know if a trackpad or a mouse? 
            // DeltaY on a trackpad would not require holding SHIFT. Maybe use specific trackpad events for this.
            float contentOffsetX = 0;
            var keysHeld = GenericControlHelper.GetKeysHeld(theEvent);
            if (keysHeld.IsAltKeyHeld)
            {
                // Zoom
                float newZoom = Math.Max(1, Zoom + (theEvent.DeltaY / 30f));
                float deltaZoom = newZoom / Zoom;

                //float visibleAreaWidth = (1 / Zoom) * Frame.Width;
                //float visibleAreaX = (1 / Zoom) * WaveFormView.ContentOffset.X;

                //float center = visibleAreaX + (visibleAreaWidth / 2);
                //float test = location.X / 

                // Adjust content offset with new zoom value
//                float mouseCursorOffset = location.X - (WaveFormView.Bounds.Width / 2);
//                float x = WaveFormView.ContentOffset.X;// + (mouseCursorOffset / deltaZoom);
//                contentOffsetX = (x * deltaZoom) + (mouseCursorOffset / Zoom);
                //contentOffsetX = (x * deltaZoom);// + (mouseCursorOffset / Zoom);

                float originalRange = (WaveFormView.Bounds.Width * Zoom) - WaveFormView.Bounds.Width;
                float newRange = (WaveFormView.Bounds.Width * newZoom) - WaveFormView.Bounds.Width;
                //float center = newRange / 2;
                float centerOffsetX = WaveFormView.ContentOffset.X + (WaveFormView.Bounds.Width / 2);

                float offsetOriginal = originalRange > 0 ? centerOffsetX / originalRange : 0;
                float newOffset = offsetOriginal * newRange;
//
//                // find the center of the currently visible area (i.e.: later from the mouse cursor) 
//                float startX = WaveFormView.ContentOffset.X;
//                float endX = startX + WaveFormView.Bounds.Width;
//                float center = (endX - startX) / 2;
////                float newX = startX + center;
////                float newXPct = newX / (WaveFormView.Bounds.Width * Zoom);
//
//                float centerRelative = center - endX;
//                float newXPct = centerRelative / WaveFormView.Bounds.Width;
//
//                float offsetPct = newXPct * newRange;
//                Console.WriteLine(">>> contentOffsetX: {0} originalRange: {1} newRange: {2} zoom: {3} waveFormWidth: {4} centerRelative: {5} newXPct: {6} offsetPct: {7}", contentOffsetX, originalRange, newRange, _zoom, WaveFormView.Bounds.Width * _zoom, centerRelative, newXPct, offsetPct);
                //newOffset = newOffset + offsetPct;


                //float mouseCursorOffset = location.X - (WaveFormView.Bounds.Width / 2);
                //float delta = mouseCursorOffset / (WaveFormView.Bounds.Width / 2);
                //Console.WriteLine("delta: {0}", delta);

                // center
                // do we start from the principle that we zoom to the center? then we adjust the offset?
                // google maps zooms to the center by default, but this is not google maps. 
                // if i am at the start of the wave form, i expect it to stay there. maybe use the old way instead.
                //contentOffsetX = newRange / 2;
                contentOffsetX = newOffset;

                //Console.WriteLine(">>> contentOffsetX: {0} originalRange: {1} newRange: {2} zoom: {3} waveFormWidth: {4}", contentOffsetX, originalRange, newRange, _zoom, WaveFormView.Bounds.Width * _zoom);
                //contentOffsetX = (WaveFormView.ContentOffset.X * deltaZoom);// + (delta * realRange);
                Zoom = newZoom;
            } 
            else
            {
                // Scroll left/right
                contentOffsetX = WaveFormView.ContentOffset.X + (theEvent.DeltaY * 1.5f);
            }            

            SetContentOffsetX(contentOffsetX);
            //Console.WriteLine("WaveFormScrollView - ScrollWheel - deltaY: {0} zoom: {1}", theEvent.DeltaY, _zoom);                
        }
        
        private void SetContentOffsetX(float x)
        {
            float contentOffsetX = x;
            float maxX = (Frame.Width * Zoom) - Frame.Width;
            contentOffsetX = Math.Max(contentOffsetX, 0);
            contentOffsetX = Math.Min(contentOffsetX, maxX);
            WaveFormView.ContentOffset = new BasicPoint(contentOffsetX, 0);
            WaveFormScaleView.ContentOffset = new BasicPoint(contentOffsetX, 0);
        }
                
        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);
            var location = GenericControlHelper.GetMouseLocation(this, theEvent);
            if (location.Y <= 20)
            {
                _isDragging = true;
                _startDragContentOffsetX = WaveFormView.ContentOffset.X;
                _startDragLocation = location;
            }
        }
        
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            if (_isDragging)
            {
                _isDragging = false;
            }
        }
        
        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
            if (_isDragging)
            {
                var location = GenericControlHelper.GetMouseLocation(this, theEvent);
                Console.WriteLine("location: {0}", location);
            }
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            base.MouseDragged(theEvent);
            if (_isDragging)
            {
                var location = GenericControlHelper.GetMouseLocation(this, theEvent);
                float delta = location.X - _startDragLocation.X;
                float x = _startDragContentOffsetX + delta;
                SetContentOffsetX(x);
                //Console.WriteLine("location: {0} delta: {1}", location, delta);
            }
        }

        // maybe add mouse drag over scale to scroll left/right? keep scrollbar at the bottom

        public override void RightMouseDown(NSEvent theEvent)
        {
            base.RightMouseDown(theEvent);
            NSMenu.PopUpContextMenu(_menu, theEvent, this);
        }
    }
}
