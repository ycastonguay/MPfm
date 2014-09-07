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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Sessions.GenericControls.Controls.Themes;
using Sessions.WPF.Classes.Controls.Graphics;
using Sessions.WPF.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls;
using Sessions.WPF.Classes.Helpers;
using Control = System.Windows.Controls.Control;

namespace Sessions.WPF.Classes.Controls
{
    public class TrackBar : Panel
    {
        private TrackBarControl _control;
        private Popup _popup;
        private StackPanel _popupStackPanel;
        private Label _lblPopupTitle;
        private Label _lblPopupSubtitle;

        public TrackBarTheme Theme { get { return _control.Theme; } set { _control.Theme = value; } }
        public int Minimum { get { return _control.Minimum; } set { _control.Minimum = value; } }
        public int Maximum { get { return _control.Maximum; } set { _control.Maximum = value; } }
        public int Value { get { return _control.Value; } set { _control.Value = value; } }
        public int ValueWithoutEvent { get { return _control.ValueWithoutEvent; } set { _control.ValueWithoutEvent = value; } }

        public event TrackBarControl.TrackBarValueChangedDelegate OnTrackBarValueChanged;
        public TrackBar()
            : base()
        {
            _control = new TrackBarControl();
            _control.OnTrackBarValueChanged += () =>
            {
                if (OnTrackBarValueChanged != null)
                    OnTrackBarValueChanged();
            };
            _control.OnInvalidateVisual += () => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(InvalidateVisual));
            _control.OnInvalidateVisualInRect += (rect) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                InvalidateVisual();
                // TODO: It seems you can't invalidate a specific rect in WPF? What?
                // http://stackoverflow.com/questions/2576599/possible-to-invalidatevisual-on-a-given-region-instead-of-entire-wpf-control                                                                                                                       
            }));
            _control.OnScrubbingSpeedChanged += speed =>
            {
                _lblPopupSubtitle.Content = speed.Label;
            };

            CreatePopup();
        }

        private void CreatePopup()
        {
            _popup = new Popup();
            _popup.PlacementTarget = this;
            _popup.Placement = PlacementMode.Bottom;            
            _popup.VerticalOffset = 4;
            _popup.AllowsTransparency = true;

            _popupStackPanel = new StackPanel();
            _popupStackPanel.Orientation = Orientation.Vertical;
            _popupStackPanel.SetResourceReference(StackPanel.BackgroundProperty, "BrushPopupBackgroundColor");

            _lblPopupTitle = new Label();
            _lblPopupTitle.Padding = new Thickness(4, 4, 4, 1);
            _lblPopupTitle.FontFamily = new FontFamily("Roboto");
            _lblPopupTitle.HorizontalContentAlignment = HorizontalAlignment.Center;
            _lblPopupTitle.Content = "Drag your mouse down to adjust the scrubbing rate";
            _lblPopupTitle.Foreground = new SolidColorBrush(Colors.DarkGray);

            _lblPopupSubtitle = new Label();
            _lblPopupSubtitle.Padding = new Thickness(4, 1, 4, 4);
            _lblPopupSubtitle.FontFamily = new FontFamily("Roboto");
            _lblPopupSubtitle.HorizontalContentAlignment = HorizontalAlignment.Center;
            _lblPopupSubtitle.Content = "High-speed scrubbing";
            _lblPopupSubtitle.Foreground = new SolidColorBrush(Colors.White);

            _popupStackPanel.Children.Add(_lblPopupTitle);
            _popupStackPanel.Children.Add(_lblPopupSubtitle);
            _popup.Child = _popupStackPanel;

            Children.Add(_popup);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            // TODO: Fix this, dirty rects in WPF is still a mystery. It was simple in WinForms, why is it hidden in WPF?
            var dirtyRect = new BasicRectangle(0, 0, (float)ActualWidth, (float)ActualHeight); 
            var wrapper = new GraphicsContextWrapper(dc, (float)ActualWidth, (float)ActualHeight, dirtyRect);
            _control.Render(wrapper);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _popupStackPanel.Opacity = 0;
            _popup.IsOpen = true;
            UIHelper.FadeElement(_popupStackPanel, true, 150, null);
            GenericControlHelper.MouseDown(e, this, _control);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            UIHelper.FadeElement(_popupStackPanel, false, 150, () =>
            {
                _popup.IsOpen = false;                
            });
            GenericControlHelper.MouseUp(e, this, _control);
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            GenericControlHelper.MouseMove(e, this, _control);
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                _control.MouseWheel(2);
            else if (e.Delta < 0)
                _control.MouseWheel(-2);
            base.OnMouseWheel(e);
        }
    }
}
