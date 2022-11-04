using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.UI.WinUi;
using Fb2.Document.WinUI.Playground.Models;
using Fb2.Document.WinUI.Playground.Services;
using Fb2.Document.WinUI.Playground.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookInfoPage : Page
    {
        private Fb2Mapper fb2MappingService = new Fb2Mapper();
        public BookInfoViewModel BookInfoViewModel { get; private set; } = new BookInfoViewModel();
        private BookModel? bookModel = null;

        public BookInfoPage()
        {
            this.InitializeComponent();
            this.Loaded += BookInfoPage_Loaded;
            this.SizeChanged += BookInfoPage_SizeChanged;
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
            BookInfoViewModel.SrcTitleInfo = bookModel?.Fb2Document?.SourceTitle;
            BookInfoViewModel.TitleInfo = bookModel?.Fb2Document?.Title;
            BookInfoViewModel.CoverpageBase64Image = bookModel.CoverpageBase64Image;
            BookInfoViewModel.PublishInfo = bookModel.Fb2Document.PublishInfo;
            BookInfoViewModel.CustomInfo = bookModel.Fb2Document.CustomInfo;
            BookInfoViewModel.BookImages = bookModel.Fb2Document.BinaryImages.Select(bi =>
            {
                var id = bi.TryGetAttribute(AttributeNames.Id, true, out var idAttr) ?
                            idAttr.Value :
                            string.Empty;

                var contentType = bi.TryGetAttribute(AttributeNames.ContentType, out var contentTypeAttr) ?
                                contentTypeAttr.Value :
                                string.Empty;

                var vm = new BinaryImageViewModel
                {
                    Content = bi.Content,
                    Id = id,
                    ContentType = contentType
                };

                return vm;
            }).ToList();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var model = e.Parameter as BookModel;
            if (model == null)
                return;

            bookModel = model;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
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

        private void ImagesThumbnailContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ImagesThumbnailContainer.ScrollIntoView(ImagesThumbnailContainer.SelectedItem);
        }
    }
}
