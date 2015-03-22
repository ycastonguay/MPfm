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
using Sessions.GenericControls.Basics;

namespace Sessions.GenericControls.Controls.Themes
{
    public class TrackBarTheme : IControlTheme
    {
        public event ControlThemeChanged OnControlThemeChanged;

        public TrackBarTheme()
        {
            OnControlThemeChanged += () => { };
        }

        private BasicColor _backgroundColor = new BasicColor(32, 40, 46);
        public BasicColor BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                OnControlThemeChanged();
            }
        }

        private BasicColor _centerLineColor = new BasicColor(24, 24, 24, 150);
        public BasicColor CenterLineColor
        {
            get { return _centerLineColor; }
            set
            {
                _centerLineColor = value;
                OnControlThemeChanged();
            }
        }

        private BasicColor _centerLineShadowColor = new BasicColor(169, 169, 169, 80);
        public BasicColor CenterLineShadowColor
        {
            get { return _centerLineShadowColor; }
            set
            {
                _centerLineShadowColor = value;
                OnControlThemeChanged();
            }
        }

        private BasicColor _faderColor = new BasicColor(255, 255, 255);
        public BasicColor FaderColor
        {
            get { return _faderColor; }
            set
            {
                _faderColor = value;
                OnControlThemeChanged();
            }
        }

        private BasicColor _faderShadowColor = new BasicColor(188, 188, 188);
        public BasicColor FaderShadowColor
        {
            get { return _faderShadowColor; }
            set
            {
                _faderShadowColor = value;
                OnControlThemeChanged();
            }
        }
    }
}