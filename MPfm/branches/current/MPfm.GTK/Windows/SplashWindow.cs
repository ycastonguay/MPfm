//  
//  SplashWindow.cs
//  
//  Author:
//       Yanick Castonguay <ycastonguay@mp4m.org>
// 
//  Copyright (c) 2012 2011 - 2012 Yanick Castonguay
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Gdk;
using MPfm.MVP;

namespace MPfm.GTK
{
	public partial class SplashWindow : Gtk.Window, ISplashView
	{
		readonly ISplashPresenter splashPresenter;
		
		public SplashWindow(ISplashPresenter splashPresenter) : 
		base(Gtk.WindowType.Toplevel)
		{
			this.Build();
			this.splashPresenter = splashPresenter;
		
			this.ModifyBg(Gtk.StateType.Normal, new Color(0, 0, 0));
			this.lblStatus.ModifyFg(Gtk.StateType.Normal, new Color(255, 255, 255));
			
			// Set image background
			Pixbuf imageCover = new Pixbuf("Splash.png");
			imageBackground.Pixbuf = imageCover;
			
			splashPresenter.BindView(this);
			splashPresenter.Initialize();
		}

		#region ISplashView implementation

		public void RefreshStatus(string message)
		{
			Gtk.Application.Invoke(delegate{
				lblStatus.Text = message;
			});
		}

		public void InitDone()
		{
			Gtk.Application.Invoke(delegate{
				MainClass.mainWindow.ShowAll();
				this.Destroy();
			});
		}

		#endregion
	}
}
