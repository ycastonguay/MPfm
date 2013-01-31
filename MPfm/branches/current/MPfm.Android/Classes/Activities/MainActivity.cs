using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.OS;
using Android.Widget;
using MPfm.Android.Classes;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Listeners;
using MPfm.Android.Classes.Objects;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using Environment = Android.OS.Environment;

namespace MPfm.Android
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyAppTheme")]
    public class MainActivity : BaseActivity, ISplashView, IUpdateLibraryView, View.IOnClickListener
    {
        private ViewPager _viewPager;
        private TabPagerAdapter _tabPagerAdapter;
        private Dialog _splashDialog;
        private Dialog _updateLibraryDialog;
        private Button _updateLibraryDialog_button;
        private TextView _updateLibraryDialog_lblTitle;
        private TextView _updateLibraryDialog_lblSubtitle;
        private List<Fragment> _fragments;
        private ProgressBar _updateLibraryDialog_progressBar;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("MainActivity - OnCreate");
            base.OnCreate(bundle);

            // Get application state
            ApplicationState state = (ApplicationState) LastNonConfigurationInstance;
            if (state != null)
            {
                // Restore state here
            }

            // Load navigation manager and other important stuff before showing splash screen
            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Main);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            // Bind this activity to splash and update library views
            AndroidNavigationManager.Instance.BindSplashView(this, ContinueInitialize);
            AndroidNavigationManager.Instance.BindUpdateLibraryView(this);
            //AndroidNavigationManager.Instance.StartMobile();

            // Show splash screen; then bind this activity to the Splash presenter
            ShowSplashScreen();
        }

        private void ContinueInitialize()
        {
            // This is called after ISplashView.OnInitDone
            RunOnUiThread(() =>
                {
                    // Create fragments
                    _fragments = new List<Fragment>
                        {
                            (Fragment)AndroidNavigationManager.Instance.CreateMobileLibraryBrowserView(MobileLibraryBrowserType.Playlists),
                            (Fragment)AndroidNavigationManager.Instance.CreateMobileLibraryBrowserView(MobileLibraryBrowserType.Artists),
                            (Fragment)AndroidNavigationManager.Instance.CreateMobileLibraryBrowserView(MobileLibraryBrowserType.Albums),
                            (Fragment)AndroidNavigationManager.Instance.CreateMobileLibraryBrowserView(MobileLibraryBrowserType.Songs)
                        };

                    // Create view
                    _viewPager = FindViewById<ViewPager>(Resource.Id.main_pager);
                    _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _fragments, _viewPager, ActionBar);
                    _viewPager.Adapter = _tabPagerAdapter;
                    _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

                    // Create tabs
                    AddTab("Playlists");
                    AddTab("Artists");
                    AddTab("Albums");
                    AddTab("Songs");

                    // Finished loading; hide splash screen
                    RemoveSplashScreen();
                });
        }

        private void AddTab(string title)
        {
            var tab = ActionBar.NewTab();
            tab.SetTabListener(_tabPagerAdapter);
            tab.SetText(title);
            ActionBar.AddTab(tab);
        }

        protected override void OnStart()
        {
            Console.WriteLine("MainActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("MainActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("MainActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("MainActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("MainActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("MainActivity - OnDestroy");
            base.OnDestroy();
        }

        public override Java.Lang.Object OnRetainNonConfigurationInstance()
        {
            // Save stuff here
            ApplicationState state = new ApplicationState();
            return state;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            string text = menuItem.TitleFormatted.ToString();

            if (text.ToUpper() == "EFFECTS")
            {
                ProgressDialog progressDialog = ProgressDialog.Show(this, "Update Library", "Updating library...", true);                
            }
            else if (text.ToUpper() == "UPDATE LIBRARY")
            {
                ShowUpdateLibrary();
            }
            else if (text.ToUpper() == "PREFERENCES")
            {
                Intent intent = new Intent(this, typeof(PreferencesActivity));
                //intent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(intent);
                //StartActivityForResult(intent, 0);
            }
            else if (text.ToUpper() == "ABOUT MPFM")
            {
                ShowSplashScreen();
            }
            return base.OnOptionsItemSelected(menuItem);
        }

        private void ShowSplashScreen()
        {
            _splashDialog = new Dialog(this, Resource.Style.SplashTheme);
            _splashDialog.SetContentView(Resource.Layout.Splash);
            _splashDialog.SetCancelable(false);            
            _splashDialog.Show();
        }

        private void RemoveSplashScreen()
        {
            if (_splashDialog != null)
            {
                _splashDialog.Dismiss();
                _splashDialog = null;
            }
        }

        private void ShowUpdateLibrary()
        {
            // Configure dialog for Update Library
            _updateLibraryDialog = new Dialog(this, Resource.Style.UpdateLibraryTheme);
            _updateLibraryDialog.SetContentView(Resource.Layout.Fragment_UpdateLibrary);
            _updateLibraryDialog.SetCancelable(false);
            _updateLibraryDialog.SetTitle("Update Library");

            // Get controls from dialog
            _updateLibraryDialog_button = (Button)_updateLibraryDialog.FindViewById(Resource.Id.fragment_updateLibrary_button);
            _updateLibraryDialog_button.SetOnClickListener(this);
            _updateLibraryDialog_lblTitle = (TextView) _updateLibraryDialog.FindViewById(Resource.Id.fragment_updateLibrary_lblTitle);
            _updateLibraryDialog_lblSubtitle = (TextView)_updateLibraryDialog.FindViewById(Resource.Id.fragment_updateLibrary_lblSubtitle);
            _updateLibraryDialog_progressBar = (ProgressBar) _updateLibraryDialog.FindViewById(Resource.Id.fragment_updateLibrary_progressBar);
            _updateLibraryDialog.Show();

            // Start update library process
            string musicPath = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMusic).ToString();
            OnStartUpdateLibrary(UpdateLibraryMode.SpecificFolder, null, musicPath);
            //OnStartUpdateLibrary(UpdateLibraryMode.WholeLibrary, null, null);
        }

        private void RemoveUpdateLibrary()
        {
            if (_updateLibraryDialog != null)
            {
                _updateLibraryDialog.Dismiss();
                _updateLibraryDialog = null;
            }
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
            RunOnUiThread(() =>
                {
                    TextView splashTextView = (TextView)_splashDialog.FindViewById(Resource.Id.splash_text);
                    splashTextView.Text = message;
                });
        }

        public void InitDone()
        {
        }

        #endregion

        #region IUpdateLibraryView implementation

        public Action<UpdateLibraryMode, List<string>, string> OnStartUpdateLibrary { get; set; }

        public void RefreshStatus(UpdateLibraryEntity entity)
        {
            RunOnUiThread(() =>
            {                
                _updateLibraryDialog_lblTitle.Text = entity.Title;
                _updateLibraryDialog_lblSubtitle.Text = entity.Subtitle;
            });
        }

        public void AddToLog(string entry)
        {
        }

        public void ProcessEnded(bool canceled)
        {
            RunOnUiThread(() =>
            {                
                _updateLibraryDialog_lblTitle.Text = "Update successful.";
                _updateLibraryDialog_lblSubtitle.Text = string.Empty;
                _updateLibraryDialog_button.Text = "OK";
                _updateLibraryDialog_progressBar.Visibility = ViewStates.Gone;
            });
        }

        #endregion

        public void OnClick(View v)
        {
            if (_updateLibraryDialog_button.Text == "Cancel")
            {
                // TODO: Cancel update
            }

            RemoveUpdateLibrary();
        }
    }
}
