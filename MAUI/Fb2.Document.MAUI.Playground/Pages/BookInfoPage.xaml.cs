using Fb2.Document.MAUI.Playground.Common;
using Fb2.Document.Models.Base;
using Fb2.Document.Models;
using Fb2.Document.Constants;
using Fb2.Document.Html;

namespace Fb2.Document.MAUI.Playground.Pages;

public class BinaryImageViewModel
{
    public string Content { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is BinaryImageViewModel model &&
            Content == model.Content &&
            Id == model.Id &&
            ContentType == model.ContentType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Content, Id, ContentType);
    }
}

public class FileInfoViewModel
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeInBytes { get; set; } = 0;

    public override bool Equals(object? obj)
    {
        return obj is FileInfoViewModel model &&
               FilePath == model.FilePath &&
               FileName == model.FileName &&
               FileSizeInBytes == model.FileSizeInBytes;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FilePath, FileName, FileSizeInBytes);
    }
}

public class BookInfoViewModel : ObservableObject
{
    private string coverpageBase64Image = string.Empty;

    public string CoverpageBase64Image
    {
        get { return coverpageBase64Image; }
        set
        {
            if (coverpageBase64Image != value)
            {
                OnPropertyChanging();
                coverpageBase64Image = value;
                OnPropertyChanged();
            }
        }
    }

    private HtmlWebViewSource? titleInfo;

    public HtmlWebViewSource? TitleInfo
    {
        get { return titleInfo; }
        set
        {
            if (titleInfo != value)
            {
                OnPropertyChanging();
                titleInfo = value;
                OnPropertyChanged();
            }
        }
    }

    private TitleInfoBase? srcTitleInfo;

    public TitleInfoBase? SrcTitleInfo
    {
        get { return srcTitleInfo; }
        set
        {
            if (srcTitleInfo != value)
            {
                OnPropertyChanging();
                srcTitleInfo = value;
                OnPropertyChanged();
            }
        }
    }

    private PublishInfo? publishInfo;

    public PublishInfo? PublishInfo
    {
        get { return publishInfo; }
        set
        {
            if (publishInfo != value)
            {
                OnPropertyChanging();
                publishInfo = value;
                OnPropertyChanged();
            }
        }
    }

    private DocumentInfo? documentInfo;

    public DocumentInfo? DocumentInfo
    {
        get { return documentInfo; }
        set
        {
            if (documentInfo != value)
            {
                OnPropertyChanging();
                documentInfo = value;
                OnPropertyChanged();
            }
        }
    }


    private CustomInfo? customInfo;

    public CustomInfo? CustomInfo
    {
        get { return customInfo; }
        set
        {
            if (customInfo != value)
            {
                OnPropertyChanging();
                customInfo = value;
                OnPropertyChanged();
            }
        }
    }

    private List<BinaryImageViewModel> binaryImages = new();

    public List<BinaryImageViewModel> BookImages
    {
        get { return binaryImages; }
        set
        {
            OnPropertyChanging();
            binaryImages = value;
            OnPropertyChanged();
        }
    }


    private FileInfoViewModel? fileInfo;
    public FileInfoViewModel? FileInfo
    {
        get { return fileInfo; }
        set
        {
            if (fileInfo != value)
            {
                OnPropertyChanging();
                fileInfo = value;
                OnPropertyChanged();
            }
        }
    }
}

[QueryProperty(nameof(Book), "Book")]
public partial class BookInfoPage : ContentPage
{
    public BookModel? Book { get; set; }

    public BookInfoViewModel BookInfoViewModel { get; set; } = new BookInfoViewModel();

    public BookInfoPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (Book == null || Book?.Fb2Document == null)
            return;

        var fb2Document = Book.Fb2Document;

        if (GetFb2NodeOrDefault(fb2Document?.Title, out var titleNode))
        {
            var mappedTitle = Fb2HtmlMapper.MapNode(titleNode);
            var joinedTitle = string.Join(Environment.NewLine, mappedTitle);
            BookInfoViewModel.TitleInfo = new HtmlWebViewSource
            {
                Html = $"<document>{Environment.NewLine}{joinedTitle}{Environment.NewLine}</document>"
            };

            //TitleWebView.MinimumHeightRequest = 300;
            TitleWebView.HeightRequest = double.MaxValue;
            //TitleWebView.MaximumHeightRequest = double.MaxValue;
        }

        if (GetFb2NodeOrDefault(fb2Document?.SourceTitle, out var srcTitleNode))
            BookInfoViewModel.SrcTitleInfo = srcTitleNode;

        BookInfoViewModel.CoverpageBase64Image = Book.CoverpageBase64Image;

        if (GetFb2NodeOrDefault(fb2Document?.PublishInfo, out var publishInfoNode))
            BookInfoViewModel.PublishInfo = publishInfoNode;

        if (GetFb2NodeOrDefault(fb2Document?.DocumentInfo, out var documentInfo))
            BookInfoViewModel.DocumentInfo = documentInfo;

        if (GetFb2NodeOrDefault(fb2Document?.CustomInfo, out var customInfo))
            BookInfoViewModel.CustomInfo = customInfo;

        var binaryImages = fb2Document?.BinaryImages;

        if (binaryImages != null && !binaryImages.IsEmpty)
        {
            BookInfoViewModel.BookImages = binaryImages.Select(bi =>
            {
                var id = bi.TryGetAttribute(AttributeNames.Id, true, out var idAttr) ?
                            idAttr!.Value : string.Empty;

                var contentType = bi.TryGetAttribute(
                    AttributeNames.ContentType,
                    out var contentTypeAttr) ? contentTypeAttr!.Value : string.Empty;

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
            FileName = Book.FileName,
            FilePath = Book.FilePath,
            FileSizeInBytes = Book.FileSizeInBytes
        };
    }

    private bool GetFb2NodeOrDefault<T>(T? instance, out T? result) where T : Fb2Node
    {
        result = null;

        if (instance == null)
            return false;

        if (!instance.HasContent && !instance.HasAttributes)
            return false;

        result = instance;
        return true;
    }
}