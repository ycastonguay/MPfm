using System;
using Android.App;
using Android.Runtime;

namespace MPfm.Android.Classes
{
    [Application (Name="my.App", Debuggable=true, Label="MPfm: Music Player for Musicians")]
    public class MPfmApplication : Application
    {
        public MPfmApplication(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer) 
        { 
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //Bootstrapper.GetContainer();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
        }
    }
}