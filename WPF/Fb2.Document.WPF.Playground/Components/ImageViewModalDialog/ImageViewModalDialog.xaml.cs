using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Fb2.Document.WPF.Playground.ViewModels;

namespace Fb2.Document.WPF.Playground.Components.ImageViewModalDialog;

/// <summary>
/// Interaction logic for ImageViewModalDialog.xaml
/// </summary>
public partial class ImageViewModalDialog : Window
{
    public List<BinaryImageViewModel> ImagesProperty
    {
        get { return (List<BinaryImageViewModel>)GetValue(ImagesPropertyProperty); }
        set { SetValue(ImagesPropertyProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ImagesProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ImagesPropertyProperty =
        DependencyProperty.Register(
            "ImagesProperty",
            typeof(List<BinaryImageViewModel>),
            typeof(ImageViewModalDialog),
            new PropertyMetadata(null));

    public BinaryImageViewModel SelectedImageProperty
    {
        get { return (BinaryImageViewModel)GetValue(SelectedImagePropertyProperty); }
        set { SetValue(SelectedImagePropertyProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedImageProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedImagePropertyProperty =
        DependencyProperty.Register(
            "SelectedImageProperty",
            typeof(BinaryImageViewModel),
            typeof(ImageViewModalDialog),
            new PropertyMetadata(null));



    public bool IsPrevButtonEnabled
    {
        get { return (bool)GetValue(IsPrevButtonEnabledProperty); }
        set { SetValue(IsPrevButtonEnabledProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IsPrevButtonEnabled.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsPrevButtonEnabledProperty =
        DependencyProperty.Register("IsPrevButtonEnabled", typeof(bool), typeof(ImageViewModalDialog), new PropertyMetadata(false));


    public bool IsNextButtonEnabled
    {
        get { return (bool)GetValue(IsNextButtonEnabledProperty); }
        set { SetValue(IsNextButtonEnabledProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IsNextButtonEnabled.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsNextButtonEnabledProperty =
        DependencyProperty.Register("IsNextButtonEnabled", typeof(bool), typeof(ImageViewModalDialog), new PropertyMetadata(false));


    public ImageViewModalDialog(List<BinaryImageViewModel> bookImages, BinaryImageViewModel selectedImage)
    {
        InitializeComponent();

        ImagesProperty = new(bookImages);
        SelectedImageProperty = selectedImage ?? ImagesProperty.First();

        this.BookImagesList.ScrollIntoView(SelectedImageProperty);
        if (this.BookImagesList.Focusable)
            this.BookImagesList.Focus();

        if (ImagesProperty.Count == 1)
        {
            this.IsPrevButtonEnabled = false;
            this.IsNextButtonEnabled = false;
        }
    }

    private void ListViewImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        e.Handled = true;

        var addedItems = e.AddedItems;
        var hasItems = addedItems is { Count: > 0 };
        if (hasItems)
        {
            var first = (BinaryImageViewModel)addedItems[0];
            this.SelectedImageProperty = first;

            var index = this.ImagesProperty.IndexOf(first);
            this.IsPrevButtonEnabled = index > 0;
            this.IsNextButtonEnabled = index < this.ImagesProperty.Count - 1;
        }
    }

    private void PrevImageBtn_Click(object sender, RoutedEventArgs e)
    {
        var index = BookImagesList.SelectedIndex;
        if (index == 0)
            return;

        index--;
        BookImagesList.SelectedIndex = index;
        this.BookImagesList.ScrollIntoView(SelectedImageProperty);
    }

    private void NextImageBtn_Click(object sender, RoutedEventArgs e)
    {
        var index = BookImagesList.SelectedIndex;
        if (index == this.ImagesProperty.Count - 1)
            return;

        index++;
        BookImagesList.SelectedIndex = index;
        this.BookImagesList.ScrollIntoView(SelectedImageProperty);
    }
}
