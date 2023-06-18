using System;
using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.Playground.Common;
using Fb2.Document.UWP.Playground.Models;
using Fb2.Document.UWP.Playground.Services;
using RichTextView.UWP.DTOs;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fb2.Document.UWP.Playground.Pages
{
    public class BinaryImageViewModel
    {
        public string Content { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public override bool Equals(object obj)
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

        public override bool Equals(object obj)
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

    // TODO : move to view models
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

        private TitleInfoBase titleInfo;

        public TitleInfoBase TitleInfo
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

        private TitleInfoBase srcTitleInfo;

        public TitleInfoBase SrcTitleInfo
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

        private PublishInfo publishInfo;

        public PublishInfo PublishInfo
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

        private DocumentInfo documentInfo;

        public DocumentInfo DocumentInfo
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


        private CustomInfo customInfo;

        public CustomInfo CustomInfo
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

        private List<BinaryImageViewModel> binaryImages = null;

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

        private FileInfoViewModel fileInfo;
        public FileInfoViewModel FileInfo
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

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookInfoPage : Page
    {
        private Dictionary<string, string> imageSignatures = new Dictionary<string, string>()
        {
            ["R0lGODdh"] = "image/gif",
            ["R0lGODlh"] = "image/gif",
            ["iVBORw0KGgo"] = "image/png",
            ["/9j/"] = "image/jpeg",
            ["SUkqAA"] = "image/tiff",
            ["TU0AKg"] = "image/tiff",
            ["Qk0"] = "image/bmp"
        };

        public BookInfoViewModel BookInfoViewModel { get; private set; } = new BookInfoViewModel();
        private BookModel bookModel = null;

        public BookInfoPage()
        {
            this.InitializeComponent();
            this.Loaded += BookInfoPage_Loaded;
            this.SizeChanged += BookInfoPage_SizeChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var model = e.Parameter as BookModel;
            //if (model == null)
            //    return;

            bookModel = model;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void BookInfoPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (StandardPopup.IsOpen)
            {
                PopupContentContainer.Width = OuterGrid.ActualWidth;
                PopupContentContainer.Height = OuterGrid.ActualHeight;
            }
        }

        private void BookInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            var fb2Document = bookModel?.Fb2Document;

            BookInfoViewModel.TitleInfo = GetFb2NodeOrDefault(fb2Document?.Title);
            BookInfoViewModel.SrcTitleInfo = GetFb2NodeOrDefault(fb2Document?.SourceTitle);
            BookInfoViewModel.CoverpageBase64Image = bookModel?.CoverpageBase64Image;
            BookInfoViewModel.PublishInfo = GetFb2NodeOrDefault(fb2Document?.PublishInfo);
            BookInfoViewModel.DocumentInfo = GetFb2NodeOrDefault(fb2Document?.DocumentInfo);
            BookInfoViewModel.CustomInfo = GetFb2NodeOrDefault(fb2Document?.CustomInfo);

            BookInfoViewModel.BookImages = fb2Document?.BinaryImages?.Select(bi =>
            {
                var id = bi.TryGetAttribute(AttributeNames.Id, true, out var idAttr) ?
                            idAttr.Value : string.Empty;

                var contentType = bi.TryGetAttribute(AttributeNames.ContentType, out var contentTypeAttr) ?
                                contentTypeAttr.Value : string.Empty;

                var vm = new BinaryImageViewModel
                {
                    Content = bi.Content,
                    Id = id,
                    ContentType = contentType
                };

                return vm;
            })?.ToList();

            BookInfoViewModel.FileInfo = new FileInfoViewModel
            {
                FileName = bookModel.FileName,
                FilePath = bookModel.FilePath,
                FileSizeInBytes = bookModel.FileSizeInBytes
            };
        }

        private T GetFb2NodeOrDefault<T>(T instance) where T : Fb2Node
        {
            if (instance == null)
                return null;

            if (!instance.HasContent && !instance.HasAttributes)
                return null;

            return instance;
        }

        private void OnReadButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.NavigateContentFrame(typeof(ReadPage), bookModel);
        }

        private void OnEnlargeImageClick(object sender, ItemClickEventArgs e)
        {
            if (!StandardPopup.IsOpen)
            {
                var imageInfo = e.ClickedItem as BinaryImageViewModel;

                var index = imageInfo != null ? BookInfoViewModel.BookImages.IndexOf(imageInfo) : 0;
                FullScreenImagesContainer.SelectedIndex = index;

                PopupContentContainer.Width = OuterGrid.ActualWidth;
                PopupContentContainer.Height = OuterGrid.ActualHeight;
                StandardPopup.IsOpen = true;
            }
        }

        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            if (StandardPopup.IsOpen)
            {
                StandardPopup.IsOpen = false;
            }
        }

        private async void ExportImageButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedItem = FullScreenImagesContainer.SelectedItem as BinaryImageViewModel;
            if (selectedItem == null ||
                string.IsNullOrEmpty(selectedItem.Content))
            {
                return;
            }

            var contentType = string.IsNullOrEmpty(selectedItem.ContentType) ?
                TryGetContentTypeFromBase64Content(selectedItem.Content) :
                selectedItem.ContentType;

            var fileExtension = contentType.Split('/').Last();
            var normalizedFileExtension = $".{fileExtension}";

            var suggestedFileName = selectedItem.Id.EndsWith(normalizedFileExtension) ?
                selectedItem.Id :
                $"{selectedItem.Id}{normalizedFileExtension}";

            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeChoices.Add("Images", new List<string>() { normalizedFileExtension });
            picker.SuggestedFileName = suggestedFileName;
            //picker.SettingsIdentifier = "settingsIdentifier";
            picker.DefaultFileExtension = normalizedFileExtension;

            var saveFile = await picker.PickSaveFileAsync();

            if (saveFile == null)
            {
                // operation cancelled
                return;
            }

            //using (var sst = await saveFile.OpenStreamForWriteAsync())
            //{
            //    var writer = new StreamWriter(sst)
            //    {
            //        AutoFlush = true
            //    };
            //    await writer.WriteAsync(selectedItem.Content);
            //    await writer.FlushAsync();
            //}

            var bytes = Convert.FromBase64String(selectedItem.Content);
            await FileIO.WriteBytesAsync(saveFile, bytes);
        }

        private string TryGetContentTypeFromBase64Content(string base64Content)
        {
            var mime = imageSignatures.FirstOrDefault(k => base64Content.StartsWith(k.Key)).Value;
            if (string.IsNullOrEmpty(mime))
                mime = "application/octet-stream";

            return mime;
        }

        private void ImagesThumbnailContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ImagesThumbnailContainer.ScrollIntoView(ImagesThumbnailContainer.SelectedItem);
        }
    }
}
