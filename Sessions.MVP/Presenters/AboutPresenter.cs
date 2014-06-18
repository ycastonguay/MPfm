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

using System.Diagnostics;
using System.Reflection;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// About preferences presenter.
	/// </summary>
    public class AboutPresenter : BasePresenter<IAboutView>, IAboutPresenter
	{
        public AboutPresenter()
		{	
		}

        public override void BindView(IAboutView view)
        {
            base.BindView(view);
            string content = "Sessions is © 2008-2014 Yanick Castonguay and is released under the GPLv3 license.";
            string version = string.Format("Version {0}", GetAssemblyVersion());
            view.RefreshAboutContent(version, content);
        }

	    private string GetAssemblyVersion()
	    {
            var assembly = Assembly.GetExecutingAssembly();
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);
            return info.FileVersion;
	    }
	}
}
