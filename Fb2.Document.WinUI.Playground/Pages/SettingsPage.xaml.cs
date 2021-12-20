using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Fb2.Document.WinUI.Playground.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.Loaded += SettingsPage_Loaded;
            this.InitializeComponent();
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            var actualTheme = MainWindowsService.Instance.ContentFrame.RequestedTheme;
            var actualThemeName = actualTheme.ToString();
            ThemePanel.Children.Cast<RadioButton>().FirstOrDefault(c => c?.Tag?.ToString() == actualThemeName).IsChecked = true;
        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();

            if (selectedTheme != null && Enum.TryParse<ElementTheme>(selectedTheme, out var convertedTheme))
                MainWindowsService.Instance.ContentFrame.RequestedTheme = convertedTheme;
        }

        //private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        //{
        //    var isOn = ((ToggleSwitch)sender).IsOn;

        //    ApplicationData.Current.LocalSettings.Values["themeSetting"] = isOn ? 1 : 0;

        //    // kills the app - tho it shoundt have had
        //    //App.Current.RequestedTheme = isOn ? ApplicationTheme.Light : ApplicationTheme.Dark;

        //    var framwContent = MainWindowsService.Instance.ContentFrame;
        //    if (framwContent != null)
        //    {
        //        var selectedTheme = isOn ? ElementTheme.Light : ElementTheme.Dark;
        //        framwContent.RequestedTheme = selectedTheme;
        //    }
        //}

        //private void ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //((ToggleSwitch)sender).IsOn = App.Current.RequestedTheme == ApplicationTheme.Light;

        //    ((ToggleSwitch)sender).IsOn = MainWindowsService.Instance.ContentFrame.RequestedTheme == ElementTheme.Light;
        //}
    }
}
