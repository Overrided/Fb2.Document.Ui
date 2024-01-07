using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fb2.Document.Constants;
using Fb2.Document.Models;
using Fb2.Document.WPF.Playground.Models;
using Fb2.Document.WPF.Playground.Pages;
using Microsoft.Win32;
using Fb2Image = Fb2.Document.Models.Image;

namespace Fb2.Document.WPF.Playground;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Page? CurrentPage = null;
    private readonly BookShelfPage bookShelfPage = new();
    private readonly SettingsPage settingsPage = new();

    public MainWindow()
    {
        InitializeComponent();
        this.mainFrame.Navigated += MainFrame_Navigated;
        this.Loaded += MainWindow_Loaded;
    }

    private void MainFrame_Navigated(object sender, NavigationEventArgs e)
    {
        CurrentPage = e.Content as Page;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        NavigateToBookshelf();
    }

    private void Bookshelf_MenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToBookshelf();
    }

    private void Settings_MenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToSettings();
    }

    private void NavigateToBookshelf()
    {
        if (CurrentPage is not BookShelfPage)
            mainFrame.Navigate(bookShelfPage);
    }

    private void NavigateToSettings()
    {
        if (CurrentPage is not SettingsPage)
            mainFrame.Navigate(settingsPage);
    }
}
