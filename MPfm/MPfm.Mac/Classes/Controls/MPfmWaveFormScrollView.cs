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

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmWaveFormScrollView")]
    public class MPfmWaveFormScrollView : NSView
    {
        //private bool _isDragging;
        //private PointF _startDragLocation;
        private NSTextField _lblZoom;
        private NSMenu _menu;
        private NSMenuItem _menuItemSelect;
        private NSMenuItem _menuItemZoomIn;
        private NSMenuItem _menuItemZoomOut;
        private Timer _timerFadeOutZoomLabel;
        private DateTime _lastZoomUpdate;
        
        public MPfmWaveFormView WaveFormView { get; private set; }
        public MPfmWaveFormScaleView WaveFormScaleView { get; private set; }
        public override bool IsFlipped { get { return true; } }

        private float _zoom = 1;
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _lblZoom.StringValue = string.Format("{0:0.0}%", value * 100);
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
            _lblZoom.Font = NSFont.FromFontName("Roboto Medium", 10f);
            _lblZoom.Alignment = NSTextAlignment.Center;
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
                    
                    WaveFormView.RefreshWaveFormBitmap();
                }
            });
        }
        
        private void CreateContextualMenu()
        {
            _menu = new NSMenu("Wave Form");
            _menuItemSelect = new NSMenuItem("Select", MenuSelect);
            _menuItemSelect.State = NSCellStateValue.On;
            _menuItemZoomIn = new NSMenuItem("Zoom in", MenuZoomIn);
            _menuItemZoomOut = new NSMenuItem("Zoom out", MenuZoomOut);
            _menu.AddItem(_menuItemSelect);
            _menu.AddItem(_menuItemZoomIn);
            _menu.AddItem(_menuItemZoomOut);
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
        
        private void ResetMenuSelection()
        {
            _menuItemSelect.State = NSCellStateValue.Off;
            _menuItemZoomIn.State = NSCellStateValue.Off;
            _menuItemZoomOut.State = NSCellStateValue.Off;
        }

        private void FrameDidChangeNotification(NSNotification notification)
        {
            //Console.WriteLine("WaveFormScrollView - NSViewFrameDidChangeNotification - Bounds: {0} Frame: {1}", Bounds, Frame);
            SetFrame();
        }

        private void SetFrame()
        {
            _lblZoom.Frame = new RectangleF((Frame.Width - 42) / 2f, ((Frame.Height - 16 - 22) / 2f) + 22, 42, 16);
            WaveFormScaleView.Frame = new RectangleF(0, 0, Frame.Width, 22);
            WaveFormView.Frame = new RectangleF(0, 22, Frame.Width, Frame.Height - 22);
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            WaveFormView.LoadPeakFile(audioFile);
            WaveFormScaleView.AudioFile = audioFile;
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            WaveFormView.SetWaveFormLength(lengthBytes);
            WaveFormScaleView.AudioFileLength = lengthBytes;
        }

        public void SetPosition(long position)
        {
            WaveFormView.Position = position;
        }

        public void SetSecondaryPosition(long position)
        {
            WaveFormView.SecondaryPosition = position;
        }

        public void ShowSecondaryPosition(bool show)
        {
            WaveFormView.ShowSecondaryPosition = show;
        }

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            WaveFormView.SetMarkers(markers);
        }
        
        public override void ScrollWheel(NSEvent theEvent)
        {
            base.ScrollWheel(theEvent);
            
            Console.WriteLine("ScrollWheel - deltaX: {0} deltaY: {1}", theEvent.DeltaX, theEvent.DeltaY);
            if (theEvent.DeltaX > 0.2f || theEvent.DeltaX < -0.2f)
            {
                // Scroll left/right using a trackpad
                SetContentOffsetX(WaveFormView.ContentOffset.X + (theEvent.DeltaX * 2));
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
                float newZoom = Zoom + (theEvent.DeltaY / 30f);
                if(newZoom < 1)
                    newZoom = 1;
                if(newZoom > 16)
                    newZoom = 16;
                Zoom = newZoom;

                // Adjust content offset with new zoom value
                // TODO: Adjust content offset X when zooming depending on mouse location
                contentOffsetX = WaveFormView.ContentOffset.X + (WaveFormView.ContentOffset.X * (newZoom - Zoom));
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
                
//        public override void MouseDown(NSEvent theEvent)
//        {
//            base.MouseDown(theEvent);
//            var keysHeld = GenericControlHelper.GetKeysHeld(theEvent);
//            if (keysHeld.IsAltKeyHeld)
//            {
//                _isDragging = true;
//                _startDragLocation = GenericControlHelper.GetMouseLocation(this, theEvent);                
//            }
//        }
//        
//        public override void MouseUp(NSEvent theEvent)
//        {
//            base.MouseUp(theEvent);
//            if (_isDragging)
//            {
//                _isDragging = false;
//            }
//        }
//        
//        public override void MouseMoved(NSEvent theEvent)
//        {
//            base.MouseMoved(theEvent);
//            if (_isDragging)
//            {
//                var location = GenericControlHelper.GetMouseLocation(this, theEvent);
//                Console.WriteLine("location: {0}", location);
//            }
//        }
//
//        public override void MouseDragged(NSEvent theEvent)
//        {
//            base.MouseDragged(theEvent);
//            if (_isDragging)
//            {
//                var location = GenericControlHelper.GetMouseLocation(this, theEvent);
//                float delta = location.X - _startDragLocation.X;
//                Console.WriteLine("location: {0} delta: {1}", location, delta);
//            }
//        }
        
        public override void RightMouseDown(NSEvent theEvent)
        {
            base.RightMouseDown(theEvent);
            NSMenu.PopUpContextMenu(_menu, theEvent, this);
        }
    }
}
