using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Fb2.Document.Constants;
using Fb2.Document.Models.Base;
using Fb2.Document.WPF.Playground.Components.ImageViewModalDialog;
using Fb2.Document.WPF.Playground.Models;
using Fb2.Document.WPF.Playground.ViewModels;

namespace Fb2.Document.WPF.Playground.Pages;

/// <summary>
/// Interaction logic for BookInfoPage.xaml
/// </summary>
public partial class BookInfoPage : Page
{
    private bool isContentRendered = false;
    private readonly BookModel? BookModel = null;

    public BookInfoViewModel BookInfoViewModel { get; private set; } = new BookInfoViewModel();

    public string CoverpageBase64Image
    {
        get { return (string)GetValue(CoverpageBase64ImageProperty); }
        set { SetValue(CoverpageBase64ImageProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CoverpageBase64Image.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CoverpageBase64ImageProperty =
        DependencyProperty.Register(
            "CoverpageBase64Image",
            typeof(string),
            typeof(BookInfoPage));

    public BookInfoPage(BookModel model)
    {
        InitializeComponent();
        BookModel = model;

        this.Loaded += BookInfoPage_Loaded;
    }

    private void BookInfoPage_Loaded(object sender, RoutedEventArgs e)
    {
        var fb2Document = BookModel!.Fb2Document;

        BookInfoViewModel.TitleInfo = GetFb2NodeOrDefault(fb2Document?.Title);
        BookInfoViewModel.SrcTitleInfo = GetFb2NodeOrDefault(fb2Document?.SourceTitle);
        BookInfoViewModel.CoverpageBase64Image = BookModel.CoverPageBase64Image;
        BookInfoViewModel.PublishInfo = GetFb2NodeOrDefault(fb2Document?.PublishInfo);
        BookInfoViewModel.DocumentInfo = GetFb2NodeOrDefault(fb2Document?.DocumentInfo);
        BookInfoViewModel.CustomInfo = GetFb2NodeOrDefault(fb2Document?.CustomInfo);

        var binaryImages = fb2Document?.BinaryImages;

        if (binaryImages != null && !binaryImages.IsEmpty)
        {
            BookInfoViewModel.BookImages = binaryImages.Select(bi =>
            {
                var id = bi.TryGetAttribute(AttributeNames.Id, true, out var idAttr) ?
                            idAttr!.Value : string.Empty;

                var contentType = bi.TryGetAttribute(AttributeNames.ContentType, out var contentTypeAttr) ?
                                contentTypeAttr!.Value : string.Empty;

                var vm = new BinaryImageViewModel
                {
                    Content = bi.Content,
                    Id = id,
                    ContentType = contentType
                };

                return vm;
            }).ToList();
        }

        BookInfoViewModel.FileInfo = new FileInfoViewModel
        {
            FileName = BookModel.FileName,
            FilePath = BookModel.FilePath,
            FileSizeInBytes = BookModel.FileSizeInBytes
        };
    }

    private T? GetFb2NodeOrDefault<T>(T? instance) where T : Fb2Node
    {
        if (instance == null)
            return null;

        if (!instance.HasContent && !instance.HasAttributes)
            return null;

        return instance;
    }

    private void ReadButton_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        if (BookModel == null)
            return; // to hell with ya

        Debug.WriteLine($"{BookModel.BookName}  --  {BookModel.FilePath}");

        NavigationService.Navigate(new ReadPage(BookModel));
    }

    private void ListViewImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Debug.WriteLine("Image list image selection changed event");

        e.Handled = true;

        var addedItems = e.AddedItems;
        var hasItems = addedItems is { Count: > 0 };
        var first = (BinaryImageViewModel)addedItems[0];

        Window.GetWindow(new ImageViewModalDialog(this.BookInfoViewModel.BookImages, first)).ShowDialog();
    }
}
