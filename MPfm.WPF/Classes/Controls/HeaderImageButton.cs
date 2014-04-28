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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MPfm.WPF.Classes.Controls
{
    public class HeaderImageButton : Button
    {
        private StackPanel _stackPanel;
        private Image _imageIcon;
        private TextBlock _lblTitle;

        public ImageSource ImageSource
        {
            get
            {
                return _imageIcon.Source;
            }
            set
            {
                _imageIcon.Source = value;
            }
        }

        public string Title
        {
            get
            {
                return _lblTitle.Text;
            }
            set
            {
                _lblTitle.Text = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return base.IsEnabled;
            }
            set
            {
                base.IsEnabled = value;
                double opacity = value ? 1 : 0.5;
                _imageIcon.Opacity = opacity;
                _lblTitle.Opacity = opacity;
            }
        }

        public HeaderImageButton()
        {
            Initialize();
        }

        private void Initialize()
        {
            _stackPanel = new StackPanel();
            _stackPanel.Orientation = Orientation.Horizontal;
            _stackPanel.Margin = new Thickness(4);
            Content = _stackPanel;

            _imageIcon = new Image();
            _imageIcon.Stretch = Stretch.None;
            _stackPanel.Children.Add(_imageIcon);

            _lblTitle = new TextBlock();
            _lblTitle.Margin = new Thickness(6, 2, 0, 0);
            _stackPanel.Children.Add(_lblTitle);
        }
    }
}
