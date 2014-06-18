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

using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using Sessions.MVP.Config;
using Sessions.MVP.Config.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Sessions.Core;
using TinyMessenger;
using Sessions.MVP.Messages;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// General preferences presenter.
	/// </summary>
    public class GeneralPreferencesPresenter : BasePresenter<IGeneralPreferencesView>, IGeneralPreferencesPresenter
	{
        private readonly ITinyMessengerHub _messageHub;
        private string _peakFolderSize;

        public GeneralPreferencesPresenter(ITinyMessengerHub messageHub)
		{
            _messageHub = messageHub;
		}

        public override void BindView(IGeneralPreferencesView view)
        {
            view.OnSetGeneralPreferences = SetGeneralPreferences;
            view.OnDeletePeakFiles = DeletePeakFiles;
            base.BindView(view);

            RefreshPreferences();
        }

        private void SetGeneralPreferences(GeneralAppConfig config)
        {
            try
            {
//                // Validate preferences before saving them
//                // Check if custom peak folder exists
//                if (config.UseCustomPeakFileFolder && !Directory.Exists(config.CustomPeakFileFolder))
//                {
//                    View.GeneralPreferencesError(new Exception("The custom peak file folder you have entered is invalid or the folder does not exist."));
//                    return;
//                }

                AppConfigManager.Instance.Root.General = config;
                AppConfigManager.Instance.Save();
                RefreshPreferences();
                _messageHub.PublishAsync<GeneralAppConfigChangedMessage>(new GeneralAppConfigChangedMessage(this, config));
            }
            catch (Exception ex)
            {
                Tracing.Log("GeneralPreferencesPresenter - SetGeneralPreferences - Failed to set preferences: {0}", ex);
                View.GeneralPreferencesError(ex);
            }
        }

        private void RefreshPreferences()
        {
            try
            {
                if(!string.IsNullOrEmpty(_peakFolderSize))
                {
                    View.RefreshGeneralPreferences(AppConfigManager.Instance.Root.General, _peakFolderSize);
                }
                else
                {
                    View.RefreshGeneralPreferences(AppConfigManager.Instance.Root.General, "Calculating...");
                    Task.Factory.StartNew(() =>
                    {
                        string peakFileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PeakFiles");
                        var info = new DirectoryInfo(peakFileFolder);
                        if (info.Exists)
                        {
                            var files = info.GetFiles();
                            float length = (float) files.Sum(x => x.Length);
                            _peakFolderSize = string.Format("{0:0.00} MB", length/1000000f);
                            View.RefreshGeneralPreferences(AppConfigManager.Instance.Root.General, _peakFolderSize);
                        }
                    });
                }
            } 
            catch (Exception ex)
            {
                Tracing.Log("GeneralPreferencesPresenter - RefreshPreferences - Failed to refresh preferences: {0}", ex);
                View.GeneralPreferencesError(ex);
            }
        }

        private void DeletePeakFiles()
        {
            Task.Factory.StartNew(() => 
            {
                try
                {
                    string peakFileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PeakFiles");
                    Directory.Delete(peakFileFolder, true);
                    Directory.CreateDirectory(peakFileFolder);
                    _peakFolderSize = "0 MB";
                    RefreshPreferences();
                }
                catch (Exception ex)
                {
                    Tracing.Log("GeneralPreferencesPresenter - DeletePeakFiles - Failed to delete peak files: {0}", ex);
                    View.GeneralPreferencesError(ex);
                }
            });
        }
	}
}
