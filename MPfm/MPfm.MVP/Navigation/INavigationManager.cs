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
using MPfm.MVP.Views;

namespace MPfm.MVP.Navigation
{
    /// <summary>
    /// Interface defining how views can be created and bound (i.e. NavigationManager and MobileNavigationManager).
    /// This interface is used by the presenters so they don't have to know on which platform type the view is created (i.e. desktop vs mobile).
    /// </summary>
    public interface INavigationManager
    {
        void CreateSplashView();
        void CreateMainView();
        void CreateMobileMainView();
        void BindSplashView(ISplashView view);
        void BindMainView(IMainView view); // Has to be implemented on mobile, but the implementation can be empty.
        void BindMobileMainView(IMobileMainView view);
    }
}
