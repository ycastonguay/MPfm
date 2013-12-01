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

using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.MVP.Config;
using MPfm.MVP.Config.Models;
using MPfm.Core;
using System;
using MPfm.Sound.PeakFiles;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// General preferences presenter.
	/// </summary>
    public class GeneralPreferencesPresenter : BasePresenter<IGeneralPreferencesView>, IGeneralPreferencesPresenter
	{
        private string _peakFolderSize;

        public GeneralPreferencesPresenter()
		{	
		}

        public override void BindView(IGeneralPreferencesView view)
        {
            view.OnSetGeneralPreferences = SetGeneralPreferences;
            view.OnDeletePeakFiles = DeletePeakFiles;
            base.BindView(view);

            RefreshPreferences();
        }

        private void SetGeneralPreferences(GeneralAppConfig generalAppConfig)
        {
            try
            {
                AppConfigManager.Instance.Root.General = generalAppConfig;
                AppConfigManager.Instance.Save();
                RefreshPreferences();
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
                        float length = (float)info.GetFiles().Sum(x => x.Length);
                        _peakFolderSize = string.Format("{0:0.00} MB", length / 1000000f);
                        View.RefreshGeneralPreferences(AppConfigManager.Instance.Root.General, _peakFolderSize);
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
