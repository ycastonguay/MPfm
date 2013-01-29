using System;
using System.Collections.Generic;
using Android.App;
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
    [Activity(MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyAppTheme")]
    public class MainActivity : BaseActivity, ISplashView, IUpdateLibraryView, View.IOnClickListener
    {
        private ViewPager _viewPager;
        private TabPagerAdapter _tabPagerAdapter;
        private Dialog _splashDialog;
        private Dialog _updateLibraryDialog;
        private AndroidNavigationManager _navigationManager;
        private Button _updateLibraryDialog_button;
        private TextView _updateLibraryDialog_lblTitle;
        private TextView _updateLibraryDialog_lblSubtitle;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Get application state
            ApplicationState state = (ApplicationState) LastNonConfigurationInstance;
            if (state != null)
            {
                // Restore state here
            }

            // Load navigation manager and other important stuff before showing splash screen
            _navigationManager = new AndroidNavigationManager();
            RequestWindowFeature(WindowFeatures.ActionBar);
            //RequestWindowFeature(WindowFeatures.Progress);
            SetContentView(Resource.Layout.Main);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            //SetProgressBarVisibility(true);
            //SetProgress(5000);
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Bind this activity to splash and update library views
            _navigationManager.BindSplashView(this, ContinueInitialize);
            _navigationManager.BindUpdateLibraryView(this);

            // Show splash screen; then bind this activity to the Splash presenter
            ShowSplashScreen();
        }

        private void ContinueInitialize()
        {
            RunOnUiThread(() =>
                {
                    // Stuff has finished loading; load layout
                    _viewPager = FindViewById<ViewPager>(Resource.Id.main_pager);
                    _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _viewPager, ActionBar);
                    _viewPager.Adapter = _tabPagerAdapter;
                    _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

                    var playerTab = ActionBar.NewTab();
                    playerTab.SetTabListener(_tabPagerAdapter);
                    playerTab.SetText("Player");
                    ActionBar.AddTab(playerTab);
                    var playlistsTab = ActionBar.NewTab();
                    playlistsTab.SetTabListener(_tabPagerAdapter);
                    playlistsTab.SetText("Playlists");
                    ActionBar.AddTab(playlistsTab);
                    var artistsTab = ActionBar.NewTab();
                    artistsTab.SetTabListener(_tabPagerAdapter);
                    artistsTab.SetText("Artists");
                    ActionBar.AddTab(artistsTab);
                    var albumsTab = ActionBar.NewTab();
                    albumsTab.SetTabListener(_tabPagerAdapter);
                    albumsTab.SetText("Albums");
                    ActionBar.AddTab(albumsTab);
                    var songsTab = ActionBar.NewTab();
                    songsTab.SetTabListener(_tabPagerAdapter);
                    songsTab.SetText("Songs");
                    ActionBar.AddTab(songsTab);

                    RemoveSplashScreen();
                });
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
            else if (text.ToUpper() == "SETTINGS")
            {
                // Seperate activity?
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
