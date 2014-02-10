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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MPfm.Core;
using MPfm.GenericControls.Controls;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.WPF.Classes.Controls
{
    public class WaveFormScrollViewer : ScrollViewer
    {
        private Grid _grid;
        private RowDefinition _rowScale;
        private RowDefinition _rowWaveForm;
        public WaveForm WaveFormView { get; private set; }
        public WaveFormScale WaveFormScaleView { get; private set; }

        public event WaveFormControl.ChangePosition OnChangePosition;

        public WaveFormScrollViewer()
        {
            //MinHeight = 82;            
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            WaveFormView = new WaveForm();
            WaveFormView.OnChangePosition += (position) => OnChangePosition(position);
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
            Content = _grid;
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

        protected override Size MeasureOverride(Size constraint)
        {
            //Console.WriteLine("WaveFormScrollViewer - MeasureOverride - constraint: {0} actualSize: {1},{2}", constraint, ActualWidth, ActualHeight);
            WaveFormView.RefreshWaveFormBitmap((int)ActualWidth);            
            return base.MeasureOverride(constraint);
        }
    }
}
