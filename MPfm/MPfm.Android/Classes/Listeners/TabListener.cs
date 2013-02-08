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