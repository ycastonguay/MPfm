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
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Sessions.MVP.Views;
using Sessions.MVP.Models;
using Sessions.OSX.Classes.Objects;

namespace Sessions.OSX
{
    public partial class CloudConnectWindowController : BaseWindowController, ICloudConnectView
    {
        private bool _canCheckForAuthentication = false;
        private NSColor _uncompletedStepColor = NSColor.FromDeviceRgba(0.75f, 0.75f, 0.75f, 1);

        public CloudConnectWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public CloudConnectWindowController(Action<IBaseView> onViewReady)
            : base ("CloudConnectWindow", onViewReady)
        {
            Initialize();
        }

        private void Initialize()
        {
            Console.WriteLine("CloudConnectWindowController - Initialize");
            progressBar.IsIndeterminate = true;
            btnOK.OnButtonSelected += (button) => CloseWindow();
            btnCancel.OnButtonSelected += (button) => CloseWindow();

            LoadFontsAndImages();
            ShowWindowCentered();
            Window.DidBecomeKey += HandleWindowDidBecomeKey; // this blocks the delegate used for detecting window closing
            Console.WriteLine("CloudConnectWindowController - Initialize finished!");
        }

        private void CloseWindow()
        {
            //Window.DidBecomeKey -= HandleWindowDidBecomeKey;
            Close();
        }

        private void HandleWindowDidBecomeKey(object sender, EventArgs e)
        {
            Console.WriteLine("CloudConnectWindowController - HandleWindowDidBecomeKey");
            if (!_canCheckForAuthentication)
                return;

            OnCheckIfAccountIsLinked();
        }

        public override void WindowDidLoad()
        {
            Console.WriteLine("CloudConnectWindowController - WindowDidLoad");
            base.WindowDidLoad();
            OnViewReady(this);
            Console.WriteLine("CloudConnectWindowController - WindowDidLoad finished!");
        }

        private void LoadFontsAndImages()
        {
            var titleFont = NSFont.FromFontName("Roboto Medium", 14f);
            var minorStepFont = NSFont.FromFontName("Roboto", 11f);
            var majorStepFont = NSFont.FromFontName("Roboto", 12f);
            lblTitle.Font = titleFont;
            lblStep1.Font = majorStepFont;
            lblStep1.TextColor = _uncompletedStepColor;
            lblStep2.Font = majorStepFont;
            lblStep2.TextColor = _uncompletedStepColor;
            lblStep2B.Font = minorStepFont;
            lblStep2B.TextColor = _uncompletedStepColor;
            lblStep3.Font = majorStepFont;
            lblStep3.TextColor = _uncompletedStepColor;
            lblStep4.Font = majorStepFont;
            lblStep4.TextColor = _uncompletedStepColor;

            btnOK.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_ok");
            btnCancel.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_cancel");
        }

        #region ICloudConnectView implementation

        public Action OnCheckIfAccountIsLinked { get; set; }

        public void CloudConnectError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshStatus(CloudConnectEntity entity)
        {
            InvokeOnMainThread(() =>
            {
                string title = string.Format("Connect to {0}", entity.CloudServiceName);
                Window.Title = title;
                lblTitle.StringValue = title;

                _canCheckForAuthentication = entity.CurrentStep > 1;
                lblStep1.TextColor = entity.CurrentStep > 1 ? NSColor.White : _uncompletedStepColor;
                lblStep2.TextColor = entity.CurrentStep > 2 ? NSColor.White : _uncompletedStepColor;
                lblStep2B.TextColor = entity.CurrentStep > 2 ? NSColor.White : _uncompletedStepColor;
                lblStep3.TextColor = entity.CurrentStep > 3 ? NSColor.White : _uncompletedStepColor;
                lblStep4.TextColor = entity.IsAuthenticated ? NSColor.White : _uncompletedStepColor;
                btnOK.Enabled = entity.IsAuthenticated;
                btnCancel.Enabled = !entity.IsAuthenticated;

                if (entity.IsAuthenticated)
                {
                    progressBar.Value = 1;
                    progressBar.IsIndeterminate = false;
                }
            });
        }

        #endregion

    }
}
