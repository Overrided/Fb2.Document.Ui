using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Fb2.Document.LoadingOptions;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.Playground.Pages;
using Fb2.Document.WinUI.Playground.Services;
using Fb2.Document.WinUI.Playground.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using RichTextView.DTOs;
using RichTextView.EventArguments;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Activated += MainWindow_Activated;
            this.InitializeComponent();
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (!PopupInitializerService.Instance.IsServiceInitialized)
                PopupInitializerService.Instance.Initialize(this);

            if (!NavigationService.Instance.IsInitialized)
            {
                NavView.BackRequested += NavView_BackRequested;
                NavView.ItemInvoked += NavView_ItemInvoked;
                NavigationService.Instance.Init(ContentFrame);
                NavigationService.Instance.ContentFrameNavigated += OnContentFrameNavigated;
                NavigationService.Instance.NavigateContentFrame(typeof(BookshelfPage));
            }

            if (!MainWindowsService.Instance.IsInitialized)
            {
                MainWindowsService.Instance.Init(this, ContentFrame);
            }
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

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.Instance.TryGoBack();
        }

        private void OnContentFrameNavigated(object? sender, bool e)
        {
            NavView.IsBackButtonVisible = e ? NavigationViewBackButtonVisible.Visible : NavigationViewBackButtonVisible.Collapsed;
            NavView.IsBackEnabled = e;

            if (ContentFrame.SourcePageType != typeof(SettingsPage))
                BookshelfPageViewItem.IsSelected = true;
        }
    }
}
