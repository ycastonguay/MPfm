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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using MPfm.Library.Objects;
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    public partial class frmResumePlayback : BaseForm, IResumePlaybackView
    {
        bool _isDiscovering;

        public frmResumePlayback(Action<IBaseView> onViewReady)
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void btnResumePlayback_Click(object sender, EventArgs e)
        {

        }

        public Action<CloudDeviceInfo> OnResumePlayback { get; set; }

        public void ResumePlaybackError(Exception ex)
        {
        }

        public void RefreshDevices(IEnumerable<CloudDeviceInfo> devices)
        {
        }
    }
}