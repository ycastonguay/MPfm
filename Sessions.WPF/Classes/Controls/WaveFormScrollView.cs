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
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Sessions.WPF.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls;
using Sessions.GenericControls.Services;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;

namespace Sessions.WPF.Classes.Controls
{
    public class WaveFormScrollView : DockPanel
    {
        private bool _isDragging;
        private long _waveFormLength;
        private float _startDragContentOffsetX;
        private Point _startDragLocation;
        private DateTime _lastZoomUpdate;
        private Timer _timerFadeOutZoomLabel;
        private Grid _grid;
        private Grid _gridWaveForm;
        private StackPanel _stackPanelZoom;
        private Label _lblZoom;
        private RowDefinition _rowScale;
        private RowDefinition _rowWaveForm;
        private ContextMenu _contextMenuItems;
        private MenuItem _menuItemSelect;
        private MenuItem _menuItemAutoScroll;
        private MenuItem _menuItemZoomIn;
        private MenuItem _menuItemZoomOut;
        private MenuItem _menuItemResetZoom;
        private MenuItem _menuItemDisplayType;
        private MenuItem _menuItemDisplayTypeStereo;
        private MenuItem _menuItemDisplayTypeMono;
        private MenuItem _menuItemDisplayTypeMonoLeft;
        private MenuItem _menuItemDisplayTypeMonoRight;

        public WaveForm WaveFormView { get; private set; }
        public WaveFormScale WaveFormScaleView { get; private set; }
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
                _lblZoom.Content = string.Format("{0:0}%", value * 100);
                _zoom = value;
                WaveFormView.Zoom = value;
                WaveFormScaleView.Zoom = value;
                _lastZoomUpdate = DateTime.Now;

                if (_lblZoom.Opacity == 0)
                {
                    var anim = new DoubleAnimation();
                    anim.From = 0;
                    anim.To = 1;
                    anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
                    _lblZoom.BeginAnimation(OpacityProperty, anim);
                }
            }
        }

        public event WaveFormControl.ChangePosition OnChangePosition;
        public event WaveFormControl.ChangePosition OnChangeSecondaryPosition;
        public event WaveFormControl.ChangeSegmentPosition OnChangingSegmentPosition;
        public event WaveFormControl.ChangeSegmentPosition OnChangedSegmentPosition;

        public WaveFormScrollView()
        {
            WaveFormView = new WaveForm();
            WaveFormView.OnChangePosition += (position) => OnChangePosition(position);
            WaveFormView.OnChangeSecondaryPosition += (position) => OnChangeSecondaryPosition(position);
            WaveFormView.OnChangingSegmentPosition += (segment, bytes) => OnChangingSegmentPosition(segment, bytes);
            WaveFormView.OnChangedSegmentPosition += (segment, bytes) => OnChangedSegmentPosition(segment, bytes);
            
            WaveFormView.MinHeight = 60;
            WaveFormScaleView = new WaveFormScale();

            var typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,/"), "/Resources/Fonts/#Roboto Regular"), FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
            _lblZoom = new Label();
            _lblZoom.Background = new SolidColorBrush(Color.FromArgb(140, 32, 40, 46));
            _lblZoom.Foreground = new SolidColorBrush(Colors.White);
            _lblZoom.Padding = new Thickness(4);
            _lblZoom.Content = "100%";
            _lblZoom.FontFamily = typeface.FontFamily;
            _lblZoom.FontSize = 11;
            _lblZoom.Width = Double.NaN;
            _lblZoom.Height = Double.NaN;

            _stackPanelZoom = new StackPanel();
            _stackPanelZoom.HorizontalAlignment = HorizontalAlignment.Center;
            _stackPanelZoom.VerticalAlignment = VerticalAlignment.Center;
            _stackPanelZoom.Children.Add(_lblZoom);

            _gridWaveForm = new Grid();
            _gridWaveForm.Height = Double.NaN;
            _gridWaveForm.Children.Add(WaveFormView);
            _gridWaveForm.Children.Add(_stackPanelZoom);

            _grid = new Grid();
            _rowScale = new RowDefinition();
            _rowScale.Height = new GridLength(22);
            _rowWaveForm = new RowDefinition();
            //_rowWaveForm.Height = GridLength.Auto;
            _grid.RowDefinitions.Add(_rowScale);
            _grid.RowDefinitions.Add(_rowWaveForm);
            _grid.Children.Add(WaveFormScaleView);
            _grid.Children.Add(_gridWaveForm);
            Grid.SetRow(WaveFormScaleView, 0);
            Grid.SetRow(_gridWaveForm, 1);
            Children.Add(_grid);

            _timerFadeOutZoomLabel = new Timer(100);
            _timerFadeOutZoomLabel.Elapsed += HandleTimerFadeOutZoomLabelElapsed;
            _timerFadeOutZoomLabel.Start();

            CreateContextualMenu();
        }

        private void CreateContextualMenu()
        {
            _contextMenuItems = new ContextMenu();

            _menuItemSelect = new MenuItem();
            _menuItemSelect.Header = "Select";
            _menuItemSelect.Click += MenuItemSelectOnClick;
            _contextMenuItems.Items.Add(_menuItemSelect);

            _menuItemZoomIn = new MenuItem();
            _menuItemZoomIn.Header = "Zoom in";
            _menuItemZoomIn.InputGestureText = "Alt-MouseWheelUp";
            _menuItemZoomIn.Click += MenuItemZoomInOnClick;
            _contextMenuItems.Items.Add(_menuItemZoomIn);

            _menuItemZoomOut = new MenuItem();
            _menuItemZoomOut.Header = "Zoom out";
            _menuItemZoomOut.InputGestureText = "Alt-MouseWheelDown";
            _menuItemZoomOut.Click += MenuItemZoomOutOnClick;
            _contextMenuItems.Items.Add(_menuItemZoomOut);
            _contextMenuItems.Items.Add(new Separator());

            _menuItemResetZoom = new MenuItem();
            _menuItemResetZoom.Header = "Reset zoom";
            _menuItemResetZoom.Click += MenuItemResetZoomOnClick;
            _contextMenuItems.Items.Add(_menuItemResetZoom);

            _menuItemAutoScroll = new MenuItem();
            _menuItemAutoScroll.Header = "Enable automatic scrolling";
            _menuItemAutoScroll.Click += MenuItemAutoScrollOnClick;
            _contextMenuItems.Items.Add(_menuItemAutoScroll);

            _menuItemDisplayType = new MenuItem();
            _menuItemDisplayType.Header = "Display type";
            _contextMenuItems.Items.Add(_menuItemDisplayType);

            _menuItemDisplayTypeStereo = new MenuItem();
            _menuItemDisplayTypeStereo.Header = "Stereo";
            _menuItemDisplayTypeStereo.Click += MenuItemDisplayTypeOnClick;
            _menuItemDisplayType.Items.Add(_menuItemDisplayTypeStereo);

            _menuItemDisplayTypeMono = new MenuItem();
            _menuItemDisplayTypeMono.Header = "Mono (Mix)";
            _menuItemDisplayTypeMono.Click += MenuItemDisplayTypeOnClick;
            _menuItemDisplayType.Items.Add(_menuItemDisplayTypeMono);

            _menuItemDisplayTypeMonoLeft = new MenuItem();
            _menuItemDisplayTypeMonoLeft.Header = "Mono (Left)";
            _menuItemDisplayTypeMonoLeft.Click += MenuItemDisplayTypeOnClick;
            _menuItemDisplayType.Items.Add(_menuItemDisplayTypeMonoLeft);

            _menuItemDisplayTypeMonoRight = new MenuItem();
            _menuItemDisplayTypeMonoRight.Header = "Mono (Right)";
            _menuItemDisplayTypeMonoRight.Click += MenuItemDisplayTypeOnClick;
            _menuItemDisplayType.Items.Add(_menuItemDisplayTypeMonoRight);
        }

        private void ResetMenuSelection()
        {
            _menuItemSelect.IsChecked = false;
            _menuItemZoomIn.IsChecked = false;
            _menuItemZoomOut.IsChecked = false;
        }

        private void ResetMenuDisplayTypeSelection()
        {
            _menuItemDisplayTypeStereo.IsChecked = false;
            _menuItemDisplayTypeMono.IsChecked = false;
            _menuItemDisplayTypeMonoLeft.IsChecked = false;
            _menuItemDisplayTypeMonoRight.IsChecked = false;
        }

        private void MenuItemSelectOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            ResetMenuSelection();
            _menuItemSelect.IsChecked = true;
            WaveFormView.InteractionMode = WaveFormControl.InputInteractionMode.Select;
        }

        private void MenuItemZoomInOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            ResetMenuSelection();
            _menuItemZoomIn.IsChecked = true;
            WaveFormView.InteractionMode = WaveFormControl.InputInteractionMode.ZoomIn;
        }

        private void MenuItemZoomOutOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            ResetMenuSelection();
            _menuItemZoomOut.IsChecked = true;
            WaveFormView.InteractionMode = WaveFormControl.InputInteractionMode.ZoomOut;
        }

        private void MenuItemResetZoomOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Zoom = 1;
            SetContentOffsetX(0);
        }

        private void MenuItemAutoScrollOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            IsAutoScrollEnabled = !IsAutoScrollEnabled;
            _menuItemAutoScroll.IsChecked = IsAutoScrollEnabled;
        }

        private void MenuItemDisplayTypeOnClick(object sender, RoutedEventArgs routedEventArgs)
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
            var menuItem = sender as MenuItem;
            if (menuItem != null)
                menuItem.IsChecked = true;
        }

        private void HandleTimerFadeOutZoomLabelElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                //Console.WriteLine("HandleTimerFadeOutZoomLabelElapsed - _lblZoom.AlphaValue: {0} - timeSpan since last update: {1}", _lblZoom.AlphaValue, DateTime.Now - _lastZoomUpdate);
                if (_lblZoom.Opacity == 1 && DateTime.Now - _lastZoomUpdate > new TimeSpan(0, 0, 0, 0, 700))
                {
                    var anim = new DoubleAnimation();
                    anim.From = 1;
                    anim.To = 0;
                    anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
                    _lblZoom.BeginAnimation(OpacityProperty, anim);

                    //WaveFormView.RefreshWaveFormBitmap();
                }
            }));
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            WaveFormView.LoadPeakFile(audioFile);
            WaveFormScaleView.AudioFile = audioFile;
        }

        public void Reset()
        {
            WaveFormView.Reset();
            WaveFormScaleView.Reset();
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
            if (IsAutoScrollEnabled)
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

        public void SetActiveMarker(Guid markerId)
        {
            WaveFormView.SetActiveMarker(markerId);
        }

        public void SetMarkerPosition(Marker marker)
        {
            WaveFormView.SetMarkerPosition(marker);
            ProcessAutoScroll((long)(marker.PositionPercentage * _waveFormLength));
        }

        public void SetLoop(Loop loop)
        {
            WaveFormView.SetLoop(loop);
        }

        public void SetSegment(Segment segment)
        {
            WaveFormView.SetSegment(segment);
        }

        private void SetContentOffsetX(float x)
        {
            float contentOffsetX = x;
            float maxX = (float) ((ActualWidth * Zoom) - ActualWidth);
            contentOffsetX = Math.Max(contentOffsetX, 0);
            contentOffsetX = Math.Min(contentOffsetX, maxX);
            WaveFormView.ContentOffset = new BasicPoint(contentOffsetX, 0);
            WaveFormScaleView.ContentOffset = new BasicPoint(contentOffsetX, 0);
        }

        private void ProcessAutoScroll(long position)
        {
            if (_zoom == 1)
                return;

            double waveFormWidth = WaveFormView.ActualWidth * Zoom;
            double positionPercentage = (double)position / (double)_waveFormLength;
            double cursorX = positionPercentage * waveFormWidth;
            double newContentOffsetX = cursorX - (ActualWidth / 2f);
            newContentOffsetX = Math.Max(0, newContentOffsetX);
            newContentOffsetX = Math.Min(waveFormWidth - ActualWidth, newContentOffsetX);
            SetContentOffsetX((float) newContentOffsetX);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Console.WriteLine("WaveFormScrollView - MeasureOverride - constraint: {0} actualSize: {1}x{2} waveFormView.ActualSize: {3}x{4} gridWaveForm.ActualSize: {5}x{6} grid.ActualSize: {7}x{8}", constraint, ActualWidth, ActualHeight, WaveFormView.ActualWidth, WaveFormView.ActualHeight, _gridWaveForm.ActualWidth, _gridWaveForm.ActualHeight, _grid.ActualWidth, _grid.ActualHeight);
            //WaveFormView.RefreshWaveFormBitmap((int)ActualWidth);
            WaveFormView.InvalidateBitmaps();
            return base.MeasureOverride(constraint);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            var location = e.GetPosition(this);
            if (location.Y <= 20)
            {
                _isDragging = true;
                _startDragContentOffsetX = WaveFormView.ContentOffset.X;
                _startDragLocation = location;
            }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            _contextMenuItems.Placement = PlacementMode.MousePoint;
            _contextMenuItems.PlacementTarget = this;
            _contextMenuItems.Visibility = Visibility.Visible;
            _contextMenuItems.IsOpen = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Focus();
            base.OnMouseUp(e);
            if (_isDragging)
            {
                _isDragging = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                var location = e.GetPosition(this);
                float delta = (float) (location.X - _startDragLocation.X);
                float x = _startDragContentOffsetX + delta;
                SetContentOffsetX(x);
                //Console.WriteLine("location: {0} delta: {1}", location, delta);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);            

            //Console.WriteLine("ScrollWheel - deltaX: {0} deltaY: {1}", theEvent.DeltaX, theEvent.DeltaY);
            float contentOffsetX = 0;
            var keysHeld = GenericControlHelper.GetKeysHeld();
            if (keysHeld.IsAltKeyHeld)
            {
                // Zoom in/out
                var location = e.GetPosition(this);
                //float newZoom = Math.Max(1, Zoom + (e.Delta / 30f));
                float newZoom = Math.Max(1, Zoom + (e.Delta / 500f));
                float deltaZoom = newZoom / Zoom;
                float originPointX = (float) (IsAutoScrollEnabled ? WaveFormView.ContentOffset.X + (ActualWidth / 2) : location.X + WaveFormView.ContentOffset.X);
                float distanceToOffsetX = originPointX - WaveFormView.ContentOffset.X;
                contentOffsetX = (originPointX * deltaZoom) - distanceToOffsetX;
                Zoom = newZoom;
            }
            else
            {
                // Scroll left/right
                contentOffsetX = WaveFormView.ContentOffset.X + (e.Delta * 1);
            }

            SetContentOffsetX(contentOffsetX);
            //Console.WriteLine("WaveFormScrollView - ScrollWheel - deltaY: {0} zoom: {1}", theEvent.DeltaY, _zoom);  
        }
    }
}
