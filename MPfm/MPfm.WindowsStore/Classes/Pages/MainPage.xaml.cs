using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.WindowsStore.Classes.Navigation;
using MPfm.WindowsStore.Classes.Pages.Base;
using MPfm.WindowsStore.Data;

namespace MPfm.WindowsStore.Classes.Pages
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainPage : BasePage, IMobileOptionsMenuView
    {
        private readonly WindowsStoreNavigationManager _navigationManager;

        public MainPage()
        {
            this.InitializeComponent();

            Tracing.Log("MainPage - Ctor - Starting navigation manager...");
            _navigationManager = (WindowsStoreNavigationManager)Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            _navigationManager.BindOptionsMenuView(this);
            _navigationManager.Start();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TO DO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
            Debug.WriteLine("MainPage - LoadState");
            var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Items"] = sampleDataGroups;            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnItemClick(MobileOptionsMenuType.SyncLibrary);            
            //this.Frame.Navigate(typeof(SplashPage));
        }

        #region IMobileOptionsMenuView implementation

        public Action<MobileOptionsMenuType> OnItemClick { get; set; }

        public void RefreshMenu(List<KeyValuePair<MobileOptionsMenuType, string>> options)
        {
            Debug.WriteLine("MainPage - RefreshMenu - options.Count: {0}", options.Count);
        }

        #endregion

    }
}
