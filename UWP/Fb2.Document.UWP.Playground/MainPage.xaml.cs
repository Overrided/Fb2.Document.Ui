using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Fb2.Document.UWP.Playground.Pages;
using Fb2.Document.UWP.Playground.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Fb2.Document.UWP.Playground
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
            //Application.Current.DebugSettings.IsTextPerformanceVisualizationEnabled = true;

            NavigationService.Instance.Init(ContentFrame);

            NavView.IsPaneOpen = false;
            NavView.SelectedItem = NavView.MenuItems.First(); // tune this up
            NavView.ItemInvoked += NavView_ItemInvoked;
            NavView.BackRequested += NavView_BackRequested;

            ContentFrame.CacheSize = 5;
            NavigationService.Instance.ContentFrameNavigated += Instance_ContentFrameNavigated;

            NavigationService.Instance.NavigateContentFrame(typeof(BookshelfPage));
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.Instance.TryGoBack();
        }

        private void Instance_ContentFrameNavigated(object sender, bool e)
        {
            NavView.IsBackEnabled = e;
            NavView.IsBackButtonVisible = e ? NavigationViewBackButtonVisible.Visible : NavigationViewBackButtonVisible.Collapsed;
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer == null)
                return;

            if (args.IsSettingsInvoked)
            {
                if (ContentFrame.CurrentSourcePageType != typeof(SettingsPage))
                    NavigationService.Instance.NavigateContentFrame(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);

                return;
            }

            var navItemTag = args.InvokedItemContainer.Tag.ToString();

            Type pageType = null;

            if (navItemTag == "Bookshelf")
                pageType = typeof(BookshelfPage);
            else
                throw new NotSupportedException($"Not supported page type : {pageType.FullName}");

            if (ContentFrame.CurrentSourcePageType == pageType)
                return;

            NavigationService.Instance.NavigateContentFrame(pageType, null, args.RecommendedNavigationTransitionInfo);
        }
    }
}
