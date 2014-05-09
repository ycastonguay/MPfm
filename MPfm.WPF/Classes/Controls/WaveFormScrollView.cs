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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MPfm.Core;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Controls;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.WPF.Classes.Controls.Helpers;

namespace MPfm.WPF.Classes.Controls
{
    public class WaveFormScrollView : StackPanel
    {
        private bool _isDragging;
        private float _startDragContentOffsetX;
        private Point _startDragLocation;
        private DateTime _lastZoomUpdate;
        private Timer _timerFadeOutZoomLabel;
        private Grid _grid;
        private Label _lblZoom;
        private RowDefinition _rowScale;
        private RowDefinition _rowWaveForm;
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

        public WaveFormScrollView()
        {
            WaveFormView = new WaveForm();
            WaveFormView.OnChangePosition += (position) => OnChangePosition(position);
            WaveFormView.OnChangeSecondaryPosition += (position) => OnChangeSecondaryPosition(position);
            WaveFormView.MinHeight = 60;
            WaveFormScaleView = new WaveFormScale();

            _lblZoom = new Label();
            _lblZoom.Background = new SolidColorBrush(Color.FromArgb(140, 32, 40, 46));
            _lblZoom.Foreground = new SolidColorBrush(Colors.White);
            _lblZoom.Padding = new Thickness(4);
            _lblZoom.Content = "100%";
            _lblZoom.FontFamily = new FontFamily("Roboto");
            _lblZoom.FontSize = 11;
            _lblZoom.Width = Double.NaN;
            _lblZoom.Height = Double.NaN;

            var stackPanel = new StackPanel();
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;
            stackPanel.Children.Add(_lblZoom);

            var panel = new Grid();
            panel.Children.Add(WaveFormView);
            panel.Children.Add(stackPanel);

            _grid = new Grid();
            _rowScale = new RowDefinition();
            _rowWaveForm = new RowDefinition();
            _rowScale.Height = new GridLength(22);
            _grid.RowDefinitions.Add(_rowScale);
            _grid.RowDefinitions.Add(_rowWaveForm);
            _grid.Children.Add(WaveFormScaleView);
            _grid.Children.Add(panel);
            Grid.SetRow(WaveFormScaleView, 0);
            Grid.SetRow(panel, 1);
            Children.Add(_grid);

            _timerFadeOutZoomLabel = new Timer(100);
            _timerFadeOutZoomLabel.Elapsed += HandleTimerFadeOutZoomLabelElapsed;
            _timerFadeOutZoomLabel.Start();
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

        private void SetContentOffsetX(float x)
        {
            float contentOffsetX = x;
            float maxX = (float) ((ActualWidth * Zoom) - ActualWidth);
            contentOffsetX = Math.Max(contentOffsetX, 0);
            contentOffsetX = Math.Min(contentOffsetX, maxX);
            WaveFormView.ContentOffset = new BasicPoint(contentOffsetX, 0);
            WaveFormScaleView.ContentOffset = new BasicPoint(contentOffsetX, 0);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            //Console.WriteLine("WaveFormScrollView - MeasureOverride - constraint: {0} actualSize: {1},{2}", constraint, ActualWidth, ActualHeight);
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
                float newZoom = Math.Max(1, Zoom + (e.Delta / 30f));
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
