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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MPfm.Sound.AudioFiles;

namespace MPfm.WPF.Classes.Controls
{
    public class WaveFormScrollViewer : ScrollViewer
    {
        public WaveForm WaveFormView { get; private set; }
        public WaveFormScale WaveFormScaleView { get; private set; }

        public WaveFormScrollViewer()
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            Background = new SolidColorBrush(Colors.BlueViolet);

            WaveFormView = new WaveForm();
            WaveFormScaleView = new WaveFormScale();

            //var stackPanel = new StackPanel();
            //stackPanel.Orientation = Orientation.Horizontal;
            //stackPanel.Children.Add(WaveFormScaleView);
            //stackPanel.Children.Add(WaveFormView);
            //Content = stackPanel;

            var grid = new Grid();
            var rowScale = new RowDefinition();
            var rowWaveForm = new RowDefinition();
            rowScale.Height = new GridLength(22);
            rowWaveForm.Height = new GridLength(60);
            grid.RowDefinitions.Add(rowScale);
            grid.RowDefinitions.Add(rowWaveForm);
            grid.Children.Add(WaveFormScaleView);
            grid.Children.Add(WaveFormView);
            Grid.SetRow(WaveFormScaleView, 0);
            Grid.SetRow(WaveFormView, 1);
            Content = grid;
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
    }
}
