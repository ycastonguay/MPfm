using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MPfm.Android
{
    [Activity(MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Splash")]
    public class SplashActivity : Activity
    {
        //TextView textViewStatus;

        public SplashActivity()
        {
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //SetContentView(Resource.Layout.Splash);
            //textViewStatus = FindViewById<TextView>(Resource.Id.textViewStatus);

            StartActivity(typeof(MainActivity));
        }

        //public void ShowError(Exception ex)
        //{
        //    RunOnUiThread(() =>
        //    {
        //        textViewStatus.Text = "Error: " + ex.Message;
        //    });
        //}

        //public void RefreshMessage(string message)
        //{
        //    RunOnUiThread(() =>
        //    {
        //        textViewStatus.Text = message;
        //    });
        //}

        //public void InitializeDone()
        //{
        //    RunOnUiThread(() =>
        //    {
        //        textViewStatus.Text = "Chargement complet.";
        //        StartActivity(typeof(MainActivity));
        //    });
        //}
    }
}

