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
using Gdk;
using MPfm.MVP;
using MPfm.MVP.Views;

namespace MPfm.GTK
{
	public partial class SplashWindow : BaseWindow, ISplashView
	{
		public SplashWindow(Action<IBaseView> onViewReady) : 
		base(Gtk.WindowType.Toplevel, onViewReady)
		{
			this.Build();
		
			Initialize();	
			Console.WriteLine("SplashWindow - Constructor ended");
		}
		
		private void Initialize()
		{
			this.ModifyBg(Gtk.StateType.Normal, new Color(0, 0, 0));
			this.lblStatus.ModifyFg(Gtk.StateType.Normal, new Color(255, 255, 255));
			
			// Set image background
			Pixbuf imageCover = new Pixbuf("Splash.png");
			imageBackground.Pixbuf = imageCover;
			
			//splashPresenter.BindView(this);
			//splashPresenter.Initialize();
			Console.WriteLine("SplashWindow - Invoking OnViewReady...");
			OnViewReady.Invoke(this);
		}
		
		protected override void OnRealized()
		{
			Console.WriteLine("SplashWindow - OnRealized");
			base.OnRealized();			
		}
		
		protected override void OnShown()
		{
			Console.WriteLine("SplashWindow - OnShown");
			base.OnShown();
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
				this.Destroy();
			});					
		}

		#endregion

	}
}
