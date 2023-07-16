using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Constants;
using Fb2.Document.LoadingOptions;
using Fb2.Document.Models;
using Fb2.Document.WinUI.Playground.Models;
using Fb2.Document.WinUI.Playground.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Fb2Image = Fb2.Document.Models.Image;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookshelfPage : Page
    {
        private const int EditingDistanceThreshold = 3;
        private bool isAddingBooks = false;

        public ObservableCollection<BookModel> selectedBooks = new();

        public ObservableCollection<BookModel> SelectedBooks { get { return selectedBooks; } }

        public BookshelfPage()
        {
            this.InitializeComponent();
        }

        private async Task<BookModel> ParseFile(StorageFile storageFile)
        {
            var fb2Doc = new Fb2Document();
            long fileSizeBytes = 0;
            using (var dataStream = await storageFile.OpenStreamForReadAsync())
            {
                fileSizeBytes = dataStream.Length;
                await fb2Doc.LoadAsync(dataStream, new Fb2StreamLoadingOptions { CloseInputStream = true });

                dataStream.Close();
            }

            var result = new BookModel
            {
                FileName = storageFile.Name,
                FilePath = storageFile.Path,
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
                return null; // we are fucked up - all references are equally good or bad at same time

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

        private void Book_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                var bookModel = (BookModel)e.ClickedItem;
                NavigationService.Instance.NavigateContentFrame(typeof(BookInfoPage), bookModel);
                UpdateLayout();
            }
            catch (Exception)
            {
                //ErrorHelper.ShowErrorMessagebox();
            }
            //finally
            //{
            //VisualStateManager.GoToState(this, FreeStateName, false);
            //}
        }

        private async Task AddBooks()
        {
            var picker = new FileOpenPicker();
            PopupInitializerService.Instance.InitializePopup(picker);

            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".fb2");

            var files = await picker.PickMultipleFilesAsync();

            if (files == null || !files.Any())
                return;

            var fileTasks = files.Select(async f =>
            {
                var book = await ParseFile(f);
                SelectedBooks.Add(book);
                return book;
            });

            //var books = await Task.WhenAll(fileTasks);
            await Task.WhenAll(fileTasks);
        }

        private void OnBookReadButtonClick(object sender, RoutedEventArgs e)
        {
            var bookModel = ((sender as Button)?.DataContext as BookModel);
            if (bookModel == null)
                return;

            NavigationService.Instance.NavigateContentFrame(typeof(ReadPage), bookModel);
        }

        private async void AddBooksButtonClick(object sender, RoutedEventArgs e)
        {
            if (isAddingBooks)
                return;

            isAddingBooks = true;
            await AddBooks();
            isAddingBooks = false;
        }
    }
}
