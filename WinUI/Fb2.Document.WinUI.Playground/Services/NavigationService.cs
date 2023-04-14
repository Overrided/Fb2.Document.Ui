using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.WinUI.Playground.Pages;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace Fb2.Document.WinUI.Playground.Services
{
    public class NavigationService
    {
        private readonly HashSet<Type> pagesToGoBackFrom = new HashSet<Type>
        {
            typeof(ReadPage),
            typeof(BookInfoPage),
            typeof(SettingsPage)
        };

        private static NavigationService instance = new NavigationService();

        public static NavigationService Instance { get { return instance; } }

        private Frame contentFrame { get; set; }

        public bool IsInitialized = false;

        private NavigationService() { }

        public event EventHandler<bool> ContentFrameNavigated;

        public void Init(Frame navFrame)
        {
            contentFrame = navFrame;
            contentFrame.Navigated += ContentFrame_Navigated;
            IsInitialized = true;
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("Content frame navigated");

            var sourcePageType = e.SourcePageType;

            var shouldBackButtonBeVisible = pagesToGoBackFrom.Contains(sourcePageType);

            ContentFrameNavigated?.Invoke(this, shouldBackButtonBeVisible);
        }

        public void NavigateContentFrame(Type pageType, object? param = null, NavigationTransitionInfo? transitionInfo = null)
        {
            if (!IsInitialized)
                return;

            // due to some inner wierdness this is needed not to crash the whole app
            var dispatch = contentFrame.DispatcherQueue;
            dispatch.TryEnqueue(DispatcherQueuePriority.Normal, () =>
            {
                FrameNavigationOptions navOptions = new FrameNavigationOptions();
                navOptions.TransitionInfoOverride = transitionInfo ?? new EntranceNavigationTransitionInfo();
                navOptions.IsNavigationStackEnabled = true;

                contentFrame.NavigateToType(pageType, param, navOptions);
            });
        }

        public void TryGoBack()
        {
            if (!IsInitialized || !contentFrame.CanGoBack)
                return;

            contentFrame.GoBack();
        }
    }
}
