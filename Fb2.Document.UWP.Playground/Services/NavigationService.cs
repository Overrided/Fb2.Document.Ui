using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.UWP.Playground.Pages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Fb2.Document.UWP.Playground.Services
{
    public class NavigationService
    {
        private readonly List<Type> pagesToGoBackFrom = new List<Type>
        {
            typeof(ReadPage),
            typeof(BookInfoPage),
            typeof(SettingsPage)
        };

        private static NavigationService instance = new NavigationService();

        public static NavigationService Instance { get { return instance; } }

        public Frame ContentFrame { get; private set; }

        private bool isInitialized = false;

        private NavigationService() { }

        public event EventHandler<bool> ContentFrameNavigated;

        public void Init(Frame navFrame)
        {
            ContentFrame = navFrame;
            ContentFrame.Navigated += ContentFrame_Navigated;
            isInitialized = true;
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("Content frame navigated");

            var sourcePageType = e.SourcePageType;

            var shouldBackButtonBeVisible = pagesToGoBackFrom.Contains(sourcePageType);

            ContentFrameNavigated?.Invoke(this, shouldBackButtonBeVisible);
        }

        public void NavigateContentFrame(Type pageType, object param = null, NavigationTransitionInfo transitionInfo = null)
        {
            if (!isInitialized)
                return;

            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = transitionInfo ?? new EntranceNavigationTransitionInfo();
            navOptions.IsNavigationStackEnabled = true;

            ContentFrame.NavigateToType(pageType, param, navOptions);
        }

        public void TryGoBack()
        {
            if (!isInitialized || !ContentFrame.CanGoBack)
                return;

            ContentFrame.GoBack();
        }
    }
}
