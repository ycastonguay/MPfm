﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using MPfm.MVP.Views;
using MPfm.WPF.Classes.Windows.Base;
using Sessions.Sound.AudioFiles;

namespace MPfm.WPF.Classes.Windows
{
    public partial class EditSongMetadataWindow : BaseWindow, IEditSongMetadataView
    {
        public EditSongMetadataWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        #region IEditSongMetadataView implementation

        public Action<AudioFile> OnSaveAudioFile { get; set; }

        public void EditSongMetadataError(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in EditSongMetadata: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void RefreshAudioFile(AudioFile audioFile)
        {
        }

        #endregion
    }
}
