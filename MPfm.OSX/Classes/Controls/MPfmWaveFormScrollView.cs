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
using MPfm.OSX.Classes.Helpers;
using MonoMac.CoreGraphics;
using System.Drawing;
using MPfm.OSX.Classes.Controls.Helpers;
using MPfm.GenericControls.Basics;
using System.Timers;
using MPfm.GenericControls.Services;

namespace MPfm.OSX.Classes.Controls
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
            //Console.WriteLine("WaveFormScrollView - NSViewFrameDidChangeNotification - Bounds: {0} Frame: {1}", Bounds, Frame);
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
            float newContentOffsetX = cursorX - (Bounds.Width / 2f);
            newContentOffsetX = Math.Max(0, newContentOffsetX);
            newContentOffsetX = Math.Min(waveFormWidth - Bounds.Width, newContentOffsetX);
            SetContentOffsetX(newContentOffsetX);
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
                //float originalRange = (WaveFormView.Bounds.Width * Zoom) - WaveFormView.Bounds.Width;
                //float newRange = (WaveFormView.Bounds.Width * newZoom) - WaveFormView.Bounds.Width;

                // On Google Maps, the user pin points a lat/long with his mouse cursor.
                // It makes it a bit easier to zoom in and center on the lat/long. We don't have this for wave forms.
                // So we'll take the equivalent of song position (float 0 to 1) and try to restore that later.
                float positionPercentageOnWaveForm = (location.X + WaveFormView.ContentOffset.X) / (WaveFormView.Frame.Width * Zoom);
                //float positionPercentageCenterOnWaveForm = (WaveFormView.ContentOffset.X + (WaveFormView.Frame.Width / 2)) / (WaveFormView.Frame.Width * Zoom);
                float positionPercentageOnScreen = location.X / WaveFormView.Frame.Width;

                // the zooming perspective in this case is the mouse cursor
                // calculate how much offset there is to the left. all relative to the perspective point
                // find out what is the scale (multiplier) for the content offset, relative to the perspective point and the area available offscreen on the left.
                //float areaWidthOnLeft = location.X + WaveFormView.ContentOffset.X;
                //float daRatio = WaveFormView.ContentOffset.X / areaWidthOnLeft;
                //float daRatioTest = daRatio / Zoom;

                // what is the content offset x, when we want to restore the position on a specific x (perspective)

                //float contentOffsetRatio = WaveFormView.ContentOffset.X / (WaveFormView.Frame.Width * Zoom);
                //float newWaveFormWidth = WaveFormView.Bounds.Width * newZoom;
                //float cursorX = daRatioTest * newZoom;
                //float cursorX = positionPercentageOnWaveForm * newWaveFormWidth;

                //contentOffsetX = cursorX;// - (positionPercentageOnScreen * Bounds.Width);// - (Bounds.Width / 2f);
                //contentOffsetX = cursorX;// - (Bounds.Width / 2f);
                //contentOffsetX = Math.Max(0, contentOffsetX);
                //contentOffsetX = Math.Min(newWaveFormWidth - Bounds.Width, contentOffsetX);

                //Console.WriteLine("location: {0} positionPercentageOnWaveForm: {1} a: {2} b: {3}", location, positionPercentageOnWaveForm, location.X + WaveFormView.ContentOffset.X, WaveFormView.Frame.Width * Zoom);

//                float newOffset = 0;
//                if (originalRange > 0)
//                {
//                    // how do we adjust for mouse loc? start by testing to try to zoom into the center.
//                    // is it really possible to adjust the value BEFORE multiplying it to the new ratio?
//                    // i.e. does the mouse loc needs to be processed before or after? 
//                    // the idea is that the mouse pointer needs to point to the exact same location AFTER zooming in.
//                    // i.e. think google maps: if you point a specific street corner, when you'll zoom in, you'll keep the target on
//                    //      (almost, the target is slightly off when zooming in a lot, that's normal since the resolution at a lower zoom level is
//                    //       much lower, so it is hard to 'pin point' the exact x.
//
//                    // remind yourself that the content offset x must not be multiplied by the new zoom level. it HAS to change depending on mouse location.
//                    // what do we do with thef act that the current formula makes the zoom leaning to the left or right depending on the content offset?
//                    // i think that's just normal behavior...
//
//                    float theRealContentOffsetX = WaveFormView.ContentOffset.X; // adjust for mouse loc
//                    float contentOffsetRatio = theRealContentOffsetX / originalRange;
//                    newOffset = contentOffsetRatio * newRange;
//                }


                // if the mouse cursor is at x==0, then the content offset doesn't change (stuff gets pushed to the right)
                // if the mouse cursor is at x==width, then the content offset is * width (stuff gest pushed to the left)
                // if the mouse cursor is at x==width/2, then the content offset is * width/2?

                //float areaWidthOnLeft = location.X + WaveFormView.ContentOffset.X;

                // This solution works for keeping the same content offset
                //float contentOffsetRatio = WaveFormView.ContentOffset.X / (WaveFormView.Frame.Width * Zoom);
                //float mouseLocationRatio = (location.X / WaveFormView.Frame.Width) / Zoom;
                //float centerScreenRatio = WaveFormView.ContentOffset.X + (WaveFormView.Frame.Width / 2) / (WaveFormView.Frame.Width * Zoom);
                //float mouseLocationRatio = (location.X - WaveFormView.ContentOffset.X) / (WaveFormView.Frame.Width * Zoom);
                //float mouseLocationRatio = mouseLocationRatioOnScreen / Zoom;

                //float distanceBetweenCenterAndContentOffset = (WaveFormView.ContentOffset.X + (WaveFormView.Frame.Width / 2)) - WaveFormView.ContentOffset.X;
                //float distanceBetweenCenterAndContentOffset = WaveFormView.Frame.Width / 2;
                //float testyRatio = distanceBetweenCenterAndContentOffset / areaWidthOnLeft;
                //float distanceBetweenCenterAndContentOffsetRatio = distanceBetweenCenterAndContentOffset / Zoom;

                // what is the distance between the perspective point and th left point
                //float distanceToPerspectivePoint = WaveFormView.ContentOffset.X + distanceBetweenCenterAndContentOffset;
                //float contentOffsetRatioForPerspectivePointRatio = WaveFormView.ContentOffset.X / distanceToPerspectivePoint;
                //float newDistanceToPerspectivePoint = distanceToPerspectivePoint * deltaZoom;
                //float newContentOffsetRatioForPerspectivePoint = contentOffsetRatioForPerspectivePointRatio * newDistanceToPerspectivePoint;
                //float newContentOffsetRatioForPerspectivePoint = WaveFormView.ContentOffset.X * distanceToPerspectivePoint;
                //float newContentOffsetRatioForPerspectivePoint = WaveForm;

                // multiply the value of our perspective point (equal value because it's the center)
                //float testTesty = WaveFormView.ContentOffset.X + distanceBetweenCenterAndContentOffset;
                //float newWaveFormWidth = WaveFormView.Bounds.Width * newZoom;
                //float cursorX = testRatio * WaveFormView.ContentOffset.X;
                //float cursorX = contentOffsetRatio * newWaveFormWidth;
                //cursorX = cursorX + (mouseLocationRatio * newZoom);
                //cursorX = cursorX + (distanceBetweenCenterAndContentOffsetRatio * newZoom);
                //float cursorX = newContentOffsetRatioForPerspectivePoint;
                float newOffset = 0;//cursorX;
                //Console.WriteLine("newOffset: {0}", newOffset);

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
