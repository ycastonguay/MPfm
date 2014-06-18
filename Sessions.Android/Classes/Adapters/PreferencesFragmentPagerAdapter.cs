//// Copyright © 2011-2013 Yanick Castonguay
////
//// This file is part of MPfm.
////
//// MPfm is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
////
//// MPfm is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Android.App;
//using Android.Runtime;
//using Android.Support.V13.App;
//using Android.Support.V4.View;
//using MPfm.Android.Classes.Fragments;
//using MPfm.MVP.Navigation;

//namespace MPfm.Android.Classes.Adapters
//{
//    public class PreferencesFragmentPagerAdapter : FragmentPagerAdapter
//    {
//        public PreferencesFragmentPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
//            : base(javaReference, transfer)
//        {
//        }

//        public PreferencesFragmentPagerAdapter(FragmentManager fm)
//            : base(fm)
//        {
//        }

//        public override Fragment GetItem(int index)
//        {
//            switch (index)
//            {
//                case 0:
//                    return new 
//                    break;
//            }
//        }

//        public override int Count
//        {
//            get { return 3; }
//        }

//        public override void DestroyItem(global::Android.Views.ViewGroup container, int position, Java.Lang.Object obj)
//        {
//            Console.WriteLine("PreferencesFragmentPagerAdapter - Destroy item - index: {0}", position);
//            //if (position >= Count)
//            //{
//            //    var fragmentManager = ((Fragment) obj).FragmentManager;
//            //    var transaction = fragmentManager.BeginTransaction();
//            //    transaction.Remove((Fragment) obj);
//            //    transaction.Commit();
//            //}
//        }
//    }
//}
