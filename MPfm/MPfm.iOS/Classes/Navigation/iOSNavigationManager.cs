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

using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.iOS.Classes.Delegates;
using MPfm.iOS.Classes.Controllers;
using MonoTouch.UIKit;

namespace MPfm.iOS.Classes.Navigation
{
	public sealed class iOSNavigationManager : MobileNavigationManager
	{
		public AppDelegate AppDelegate { get; set; }
		
		public override void ShowSplash(ISplashView view)
		{
			AppDelegate.ShowSplash((SplashViewController)view);
		}
		
		public override void HideSplash()
		{
			AppDelegate.HideSplash();
		}
		
		public override void AddTab(MobileNavigationTabType type, string title, IBaseView view)
		{
            AppDelegate.AddTab(type, title, (UIViewController)view);
		}
		
		public override void PushTabView(MobileNavigationTabType type, IBaseView view)
		{
			AppDelegate.PushTabView(type, (UIViewController)view);
		}
	}
}
