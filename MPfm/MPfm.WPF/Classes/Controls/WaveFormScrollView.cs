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
        private RowDefinition _rowScale;
        private RowDefinition _rowWaveForm;
        public WaveForm WaveFormView { get; private set; }
        public WaveFormScale WaveFormScaleView { get; private set; }

        private float _zoom = 1;
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                //_lblZoom.StringValue = string.Format("{0:0}%", value * 100);
                _zoom = value;
                WaveFormView.Zoom = value;
                WaveFormScaleView.Zoom = value;
                _lastZoomUpdate = DateTime.Now;

                //if (_lblZoom.AlphaValue == 0)
                //{
                //    NSAnimationContext.BeginGrouping();
                //    NSAnimationContext.CurrentContext.Duration = 0.2;
                //    (_lblZoom.Animator as NSTextField).AlphaValue = 1;
                //    NSAnimationContext.EndGrouping();
                //}
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

            _grid = new Grid();
            _rowScale = new RowDefinition();
            _rowWaveForm = new RowDefinition();
            _rowScale.Height = new GridLength(22);
            _grid.RowDefinitions.Add(_rowScale);
            _grid.RowDefinitions.Add(_rowWaveForm);
            _grid.Children.Add(WaveFormScaleView);
            _grid.Children.Add(WaveFormView);
            Grid.SetRow(WaveFormScaleView, 0);
            Grid.SetRow(WaveFormView, 1);
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
                //if (_lblZoom.AlphaValue == 1 && DateTime.Now - _lastZoomUpdate > new TimeSpan(0, 0, 0, 0, 700))
                if (DateTime.Now - _lastZoomUpdate > new TimeSpan(0, 0, 0, 0, 700))
                {
                    ////Console.WriteLine("HandleTimerFadeOutZoomLabelElapsed - Fade out");
                    //NSAnimationContext.BeginGrouping();
                    //NSAnimationContext.CurrentContext.Duration = 0.2;
                    //(_lblZoom.Animator as NSTextField).AlphaValue = 0;
                    //NSAnimationContext.EndGrouping();

                    WaveFormView.RefreshWaveFormBitmap();
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
            WaveFormView.RefreshWaveFormBitmap((int)ActualWidth);            
            return base.MeasureOverride(constraint);
        }

        //protected override void OnMouseWheel(MouseWheelEventArgs e)
        //{            
        //    var position = e.GetPosition(this);
        //    int factor = (int)(e.Delta / 120f);
        //    Console.WriteLine("OnMouseWheel - position: {0} delta: {1} factor: {2}", position, e.Delta, factor);
        //    //var matrix = _grid.LayoutTransform.Value;
        //    var matrix = _grid.RenderTransform.Value;
        //    if (e.Delta > 0)
        //        matrix.ScaleAt(1.5, 1, position.X, position.Y);
        //    else
        //        matrix.ScaleAt(1.0 / 1.5, 1, position.X, position.Y);
        //    //_grid.LayoutTransform = new MatrixTransform(matrix);
        //    _grid.RenderTransform = new MatrixTransform(matrix);
        //    base.OnMouseWheel(e);
        //}

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
                // Zoom
                float newZoom = Zoom + (e.Delta / 300f); //(e.DeltaY / 30f);
                if (newZoom < 1)
                    newZoom = 1;
                if (newZoom > 32)
                    newZoom = 32;
                float deltaZoom = newZoom / Zoom;
                Zoom = newZoom;

                // Adjust content offset with new zoom value
                // TODO: Adjust content offset X when zooming depending on mouse location
                //contentOffsetX = WaveFormView.ContentOffset.X + (WaveFormView.ContentOffset.X * (newZoom - Zoom));
                contentOffsetX = WaveFormView.ContentOffset.X * deltaZoom;
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
