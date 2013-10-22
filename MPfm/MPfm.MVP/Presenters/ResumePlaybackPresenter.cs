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
using MPfm.Library.Objects;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.Services.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Resume Playback view presenter.
	/// </summary>
    public class ResumePlaybackPresenter : BasePresenter<IResumePlaybackView>, IResumePlaybackPresenter
	{
        private readonly ICloudLibraryService _cloudLibrary;

        public ResumePlaybackPresenter(ICloudLibraryService cloudLibrary)
		{
            _cloudLibrary = cloudLibrary;
            _cloudLibrary.OnDropboxDataChanged += (data) => {
                Task.Factory.StartNew(() => {
                    Console.WriteLine("ResumePlaybackPresenter - OnDropboxDataChanged - Sleeping 1 second...");
                    Thread.Sleep(1000);
                    Console.WriteLine("ResumePlaybackPresenter - OnDropboxDataChanged - Fetching device infos...");
                    RefreshDevices();
                });
            };
		}

        public override void BindView(IResumePlaybackView view)
        {
            base.BindView(view);

            view.OnResumePlayback = ResumePlayback;

            Task.Factory.StartNew(() => {
                RefreshDevices();
            });
        }     

        private void RefreshDevices()
        {
            try
            {
                var devices = _cloudLibrary.PullDeviceInfos();
                View.RefreshDevices(devices);
            } 
            catch (Exception ex)
            {
                View.ResumePlaybackError(ex);
            }
        }

        private void ResumePlayback(CloudDeviceInfo device)
        {



        }
	}
}
