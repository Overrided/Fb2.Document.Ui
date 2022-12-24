using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fb2.Document.Constants;
using Fb2.Document.LoadingOptions;
using Fb2.Document.MAUI.Playground.Pages;
using Fb2.Document.Models;
using Microsoft.Maui.Controls;
using Fb2Image = Fb2.Document.Models.Image;

namespace Fb2.Document.MAUI.Playground;

public class BookModel
{
    //private const string system = "book_model_system";

    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSizeInBytes { get; set; }
    public string CoverpageBase64Image { get; set; } = string.Empty;
    public string BookName { get; set; }
    public string BookAuthor { get; set; }
    public Fb2Document Fb2Document { get; set; }

    //public static BookModel AddBookModel;

    //static BookModel()
    //{
    //    AddBookModel = new BookModel
    //    {
    //        FileName = system,
    //        FilePath = system,
    //        FileSizeInBytes = -1,
    //        BookName = system,
    //        BookAuthor = system
    //    };
    //}

    public override bool Equals(object? obj)
    {
        return obj is BookModel model &&
               FileName == model.FileName &&
               FilePath == model.FilePath &&
               FileSizeInBytes == model.FileSizeInBytes &&
               CoverpageBase64Image == model.CoverpageBase64Image &&
               BookName == model.BookName &&
               BookAuthor == model.BookAuthor &&
               (Fb2Document == null && model.Fb2Document == null || (Fb2Document?.Equals(model.Fb2Document) ?? false));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FileName, FilePath, FileSizeInBytes, CoverpageBase64Image, BookName, BookAuthor, Fb2Document);
    }
}

public partial class MainPage : ContentPage
{
    //int count = 0;

    private const int EditingDistanceThreshold = 3;
    private bool isAddingBooks = false;

    public ObservableCollection<BookModel> Books { get; private set; } = new();

    public MainPage()
    {
        InitializeComponent();
        BooksCollectionView.ItemsSource = Books;
        BooksCollectionView.ItemsLayout = new GridItemsLayout(5, ItemsLayoutOrientation.Vertical);
        //BooksCollectionView.ItemsLayout = new GridItemsLayout(ItemsLayoutOrientation.Horizontal);
        //BooksCollectionView.ItemsLayout = new GridItemsLayout(4, ItemsLayoutOrientation.Horizontal);
    }

    private async void AddBooks_Button_Clicked(object sender, EventArgs e)
    {
        var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    //{ DevicePlatform.iOS, new[] { ".fb2" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/xml", "text/xml", "application/octet-stream" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".fb2" } }, // file extension
                    //{ DevicePlatform.Tizen, new[] { ".fb2" } },
                    //{ DevicePlatform.macOS, new[] { ".fb2" } }, // UTType values
                });

        var booksPickingOptions = new PickOptions
        {
            PickerTitle = "Select book(s)",
            FileTypes = customFileType
        };

        var files = await PickBooks(booksPickingOptions);
        if (files == null || !files.Any())
            return;

        emptyStateLabel.IsVisible = false;

        activityIndicator.IsRunning = true;
        activityIndicator.IsVisible = true;

        var models = (await Task.WhenAll(files.Select(ParseFile))).ToList();

        foreach (var book in models)
        {
            Books.Add(book);
            Debug.WriteLine($"Added book '{book.BookName}'");
        }

        activityIndicator.IsRunning = false;
        activityIndicator.IsVisible = false;
    }

    public async Task<IEnumerable<FileResult>> PickBooks(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickMultipleAsync(options);
            if (result == null || !result.Any())
                return null;

            result = result.Where(r => r.FileName.EndsWith("fb2", StringComparison.OrdinalIgnoreCase));

            if (result == null || !result.Any())
                return null;

            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        return null;
    }

    private async Task<BookModel> ParseFile(FileResult storageFile)
    {
        var fb2Doc = new Fb2Document();
        long fileSizeBytes = 0;

        using (var dataStream = await storageFile.OpenReadAsync())
        {
            fileSizeBytes = dataStream.Length;
            await fb2Doc.LoadAsync(dataStream, new Fb2StreamLoadingOptions { CloseInputStream = true });

            dataStream.Close();
        }

        //var html = Fb2HtmlMapper.MapDocument(fb2Doc);

        var result = new BookModel
        {
            FileName = storageFile.FileName,
            FilePath = storageFile.FullPath,
            FileSizeInBytes = fileSizeBytes,
            Fb2Document = fb2Doc,
        };

        // move to method
        var bookName = fb2Doc.Book.GetFirstDescendant<BookName>()?.Content;
        var firstBookTitle = fb2Doc.Book.GetFirstDescendant<BookTitle>()?.Content;

        if (!string.IsNullOrEmpty(firstBookTitle) || !string.IsNullOrEmpty(bookName))
            result.BookName = string.IsNullOrEmpty(firstBookTitle) ? bookName : firstBookTitle;

        // move to method
        var authorString = GetStringnifiedAuthor(fb2Doc);
        if (!string.IsNullOrEmpty(authorString))
            result.BookAuthor = authorString;

        // move to method
        var firstCoverpageImage = GetCoverpage(fb2Doc)?.GetFirstChild<Fb2Image>();
        if (firstCoverpageImage == null)
            return result;

        if (!firstCoverpageImage.TryGetAttribute(AttributeNames.XHref, true, out var xHref)) // if value (linked image id) was String.Empty
            return result;

        if (fb2Doc.BinaryImages == null || !fb2Doc.BinaryImages.Any())
            return result;

        var bestMatch = GetBestMatchImage(fb2Doc.BinaryImages, xHref.Value);

        if (bestMatch == null)
            return result;

        result.CoverpageBase64Image = bestMatch.Content;

        return result;
    }

    private Coverpage GetCoverpage(Fb2Document document)
    {
        var coverpage = document.Title?.GetFirstDescendant<Coverpage>();
        return coverpage;
    }

    private string GetStringnifiedAuthor(Fb2Document fb2Document)
    {
        var authors = fb2Document.Title?.GetChildren<Author>() ?? fb2Document.SourceTitle?.GetChildren<Author>();

        if (authors == null || !authors.Any())
            return string.Empty;

        return string.Join(", ", authors.Select(a =>
        {
            var sb = new StringBuilder();

            var fName = a.GetFirstChild<FirstName>();
            if (fName != null)
                sb.Append(fName.Content);

            var mName = a.GetFirstChild<MiddleName>();
            if (mName != null)
                sb.Append($" {mName.Content}");

            var lName = a.GetFirstChild<LastName>();
            if (lName != null)
                sb.Append($" {lName.Content}");

            return sb.ToString();
        }));
    }

    private BinaryImage GetBestMatchImage(IEnumerable<BinaryImage> linkedBinaries, string xHref)
    {
        var imageDistances = linkedBinaries.Select(im =>
                new
                {
                    Image = im,
                    Distance = GetEditingDistance(xHref, im.GetAttribute(AttributeNames.Id, true).Value)
                });

        // check for distinction
        var distinctDistances = imageDistances.Select(t => t.Distance).Distinct();

        // we have 12 images, with same distances, so distinct instance.count wil be 1.
        // so if we have only 1 image at all it will still work
        if (distinctDistances.Count() == 1 && linkedBinaries.Count() != 1)
            return null; // we are fucked up - all referenes are equally good or bad at same time

        var bestMatch = imageDistances
            .OrderBy(t => t.Distance)
            .FirstOrDefault(); // choose the shortest distance

        if (bestMatch.Distance > EditingDistanceThreshold)
            return null;

        return bestMatch.Image;
    }

    private static int GetEditingDistance(string source, string target)
    {
        // corner cases
        if (source == target ||
            string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(target)) return 0;

        if (source.Length == 0) return target.Length;
        if (target.Length == 0) return source.Length;

        int sourceCharCount = source.Length;
        int targetCharCount = target.Length;

        int[,] distance = new int[sourceCharCount + 1, targetCharCount + 1];

        // Step 2
        for (int i = 0; i <= sourceCharCount; distance[i, 0] = i++) ;
        for (int j = 0; j <= targetCharCount; distance[0, j] = j++) ;

        for (int i = 1; i <= sourceCharCount; i++)
        {
            for (int j = 1; j <= targetCharCount; j++)
            {
                // Step 3
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                // Step 4
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }

        return distance[sourceCharCount, targetCharCount];
    }

    private async void OnSelectedBook_ChangedEvent(object sender, SelectionChangedEventArgs e)
    {
        var a = sender;
        var allSelected = e.CurrentSelection;
        var first = allSelected.FirstOrDefault((object)null);
        if (first == null)
            return;

        await Shell.Current.GoToAsync("Read", true, new Dictionary<string, object>
        {
            ["Book"] = first
        });

        this.BooksCollectionView.SelectedItem = null;
        this.BooksCollectionView.SelectedItems.Clear();

        //await Shell.Current.GoToAsync(new ShellNavigationState("MainPage/Read"), true);
        //await Shell.Current.GoToAsync("../Read", true);
        //Shell.Current.CurrentItem = new ReadPage();

        //Shell.Current.CurrentItem = 
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}