using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Fb2.Document.Constants;
using Fb2.Document.Models;
using Fb2.Document.WPF.Playground.Models;
using Microsoft.Win32;
using Fb2Image = Fb2.Document.Models.Image;

namespace Fb2.Document.WPF.Playground.Pages;

/// <summary>
/// Interaction logic for BookShelf.xaml
/// </summary>
public partial class BookShelfPage : Page
{
    private const int EditingDistanceThreshold = 3;

    public ObservableCollection<BookModel> Books { get; set; } = new ObservableCollection<BookModel>();


    public BookShelfPage()
    {
        InitializeComponent();
    }

    private async void AddBooks_ButtonClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Multiselect = true;
        openFileDialog.DefaultExt = ".fb2";
        openFileDialog.CheckFileExists = true;
        openFileDialog.CheckPathExists = true;

        var result = openFileDialog.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            // Open documents
            var filenames = openFileDialog.FileNames;

            var fileStreams = openFileDialog.OpenFiles();

            for (int i = 0; i < fileStreams.Length; i++)
            {
                var fileNameAndPath = filenames[i];
                var safeFileName = openFileDialog.SafeFileNames[i];
                var fileStream = fileStreams[i];

                var parsedFile = await ParseFile(fileStream, fileNameAndPath, safeFileName);

                Books.Add(parsedFile);
            }
        }
    }

    private async Task<BookModel> ParseFile(
        Stream stream,
        string fileNameAndPath,
        string safeFileName)
    {
        var bookModel = new BookModel
        {
            FilePath = fileNameAndPath,
            FileName = safeFileName,
            FileSizeInBytes = stream.Length
        };

        var fb2Doc = new Fb2Document();
        await fb2Doc.LoadAsync(stream);

        bookModel.Fb2Document = fb2Doc;

        // move to method
        var bookName = fb2Doc.Book.GetFirstDescendant<BookName>()?.Content;
        var firstBookTitle = fb2Doc.Book.GetFirstDescendant<BookTitle>()?.Content;

        if (!string.IsNullOrEmpty(firstBookTitle) || !string.IsNullOrEmpty(bookName))
            bookModel.BookName = string.IsNullOrEmpty(firstBookTitle) ? bookName : firstBookTitle;

        // move to method
        var authorString = GetStringnifiedAuthor(fb2Doc);
        if (!string.IsNullOrEmpty(authorString))
            bookModel.BookAuthor = authorString;

        // move to method
        var firstCoverpageImage = GetCoverpage(fb2Doc)?.GetFirstChild<Fb2Image>();
        if (firstCoverpageImage == null)
            return bookModel;

        if (!firstCoverpageImage.TryGetAttribute(AttributeNames.XHref, true, out var xHref)) // if value (linked image id) was String.Empty
            return bookModel;

        if (fb2Doc.BinaryImages == null || !fb2Doc.BinaryImages.Any())
            return bookModel;

        var bestMatch = GetBestMatchImage(fb2Doc.BinaryImages, xHref.Value);

        if (bestMatch == null)
            return bookModel;

        bookModel.CoverPageBase64Image = bestMatch.Content;

        return bookModel;
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

    private void OnBookClick(object sender, MouseButtonEventArgs e)
    {
        // wicked ways, why the hell not a "selectedItem" lol
        e.Handled = true;
        var originalSource = e.OriginalSource;
        var bookModel = (originalSource as FrameworkElement)?.DataContext as BookModel;

        if (bookModel == null)
            return; // to hell with ya

        Debug.WriteLine($"{bookModel.BookName}  --  {bookModel.FilePath}");

        NavigationService.Navigate(new ReadPage(bookModel));

        //var frame = this.Parent as Frame;
        //if (frame != null)
        //    frame.Navigate(new ReadPage());
    }
}
