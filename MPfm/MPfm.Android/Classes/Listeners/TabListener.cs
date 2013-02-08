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
using Android.App;

namespace MPfm.Android.Classes.Listeners
{
    public class TabListener<T> : Java.Lang.Object, ActionBar.ITabListener
        where T : Fragment, new()
    {
        private T _fragment;

        public TabListener()
        {
            _fragment = new T();
        }

        protected TabListener(T fragment)
        {
            _fragment = fragment;
        }

        public void OnTabReselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            Console.WriteLine("TabListener - " + _fragment.GetType().FullName + " - OnTabReselected");
        }

        public void OnTabSelected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            Console.WriteLine("TabListener - " + _fragment.GetType().FullName + " - OnTabSelected");
            ft.Replace(Resource.Id.main_layout, _fragment, typeof(T).FullName);

            //if (ft.IsAddToBackStackAllowed)
            //{
            //    ft.AddToBackStack(null);
            //    ft.Commit();
            //}
        }

        public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            Console.WriteLine("TabListener - " + _fragment.GetType().FullName + " - OnTabUnselected");
            //ft.Remove(_fragment);
            //ft.Commit();
        }
    }
}
