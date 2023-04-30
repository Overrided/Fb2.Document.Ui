using System;
using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Playground.Models;
using Fb2.Document.WinUI.Playground.Services;
using Fb2.Document.WinUI.Playground.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookInfoPage : Page
    {
        private Dictionary<string, string> imageSignatures = new()
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
        private BookModel? bookModel = null;

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
            var fb2Document = bookModel!.Fb2Document;

            BookInfoViewModel.TitleInfo = GetFb2NodeOrDefault(fb2Document?.Title);
            BookInfoViewModel.SrcTitleInfo = GetFb2NodeOrDefault(fb2Document?.SourceTitle);
            BookInfoViewModel.CoverpageBase64Image = bookModel.CoverpageBase64Image;
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
                FileName = bookModel.FileName,
                FilePath = bookModel.FilePath,
                FileSizeInBytes = bookModel.FileSizeInBytes
            };
        }

        private void OnReadButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.NavigateContentFrame(typeof(ReadPage), bookModel);
        }

        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            if (StandardPopup.IsOpen)
            {
                StandardPopup.IsOpen = false;
            }
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

        private async void ExportImageButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedItem = FullScreenImagesContainer.SelectedItem as BinaryImageViewModel;
            if (selectedItem == null || string.IsNullOrEmpty(selectedItem.Content))
                return;

            var contentType = string.IsNullOrEmpty(selectedItem.ContentType) ?
                TryGetContentTypeFromBase64Content(selectedItem.Content) :
                selectedItem.ContentType;

            var fileExtension = contentType.Split('/').Last();
            var normalizedFileExtension = $".{fileExtension}";

            var suggestedFileName = selectedItem.Id.EndsWith(normalizedFileExtension) ?
                selectedItem.Id :
                $"{selectedItem.Id}{normalizedFileExtension}";

            FileSavePicker picker = new();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeChoices.Add("Images", new List<string>() { normalizedFileExtension });
            picker.SuggestedFileName = suggestedFileName;
            picker.DefaultFileExtension = normalizedFileExtension;
            PopupInitializerService.Instance.InitializePopup(picker);

            var saveFile = await picker.PickSaveFileAsync();

            if (saveFile == null)
            {
                // operation cancelled
                return;
            }

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

        private T? GetFb2NodeOrDefault<T>(T? instance) where T : Fb2Node
        {
            if (instance == null)
                return null;

            if (!instance.HasContent && !instance.HasAttributes)
                return null;

            return instance;
        }
    }
}
