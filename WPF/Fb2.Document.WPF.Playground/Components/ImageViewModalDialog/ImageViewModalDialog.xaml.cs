using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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


    public ImageViewModalDialog(List<BinaryImageViewModel> bookImages)
    {
        InitializeComponent();

        ImagesProperty = new(bookImages);
        SelectedImageProperty = ImagesProperty.First();
    }

    private void ListViewImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Debug.WriteLine("ImageModal image_list selection changed event");
    }
}
