using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.NodeProcessors;
using Fb2.Document.WinUI.Playground.Models;
using Fb2.Document.WinUI.Playground.Services;
using Fb2.Document.WinUI.Playground.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using RichTextView.DTOs;

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
            this.descriptionViewPort.Loaded += DescriptionViewPort_Loaded;
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

            this.descriptionViewPort.Loaded -= DescriptionViewPort_Loaded;
        }

        private void DescriptionViewPort_Loaded(object sender, RoutedEventArgs e)
        {
            TitleInfoBase? titleInfo = (TitleInfoBase?)bookModel?.Fb2Document?.Title ?? bookModel?.Fb2Document?.SourceTitle;

            var titleInfoAuthors = titleInfo?.GetDescendants<Author>();

            var titleInfoBookName = titleInfo?.GetFirstDescendant<BookTitle>() as Fb2Node;
            var publishInfoBookName = bookModel.Fb2Document.PublishInfo?.GetFirstChild<BookName>() as Fb2Node;
            var bookTitle = titleInfoBookName ?? publishInfoBookName;
            var annotation = titleInfo?.GetFirstDescendant<Annotation>();
            var tiltleSequences = titleInfo?.GetDescendants<SequenceInfo>();
            var publishInfoSequences = bookModel.Fb2Document.PublishInfo?.GetDescendants<SequenceInfo>();

            var nodes = new List<Fb2Node>();
            if (titleInfoAuthors != null && titleInfoAuthors.Any())
                nodes.AddRange(titleInfoAuthors);

            // TODO : TryGetFirstDescendant once using new lib
            if (bookTitle != null)
                nodes.Add(bookTitle);

            if (tiltleSequences != null && tiltleSequences.Any())
                nodes.AddRange(tiltleSequences);

            if (publishInfoSequences != null && publishInfoSequences.Any())
                nodes.AddRange(publishInfoSequences);

            // TODO : distinct different sequences once using new version of a lib (if ever)

            if (annotation != null)
                nodes.Add(annotation);

            var size = descriptionViewPort.GetViewHostSize();
            var descriptionText = new List<Fb2ContentPage>();

            descriptionText.AddRange(fb2MappingService.MapNodes(nodes, size));

            // we have had sequence info mapped before
            // use Remove api once new lib is hooked
            var publishInfo = bookModel.Fb2Document.PublishInfo;
            if (publishInfo != null && publishInfo.Content.Any())
            {
                publishInfo.Content.RemoveAll(n => n is SequenceInfo);
                var publishInfoPage = fb2MappingService.MapNode(publishInfo, size);
                if (publishInfoPage != null && publishInfoPage.Any())
                    descriptionText.AddRange(publishInfoPage);
            }

            var customInfo = bookModel.Fb2Document.CustomInfo;
            if (customInfo != null)
            {
                var customInfoPage = fb2MappingService.MapNode(customInfo, size);
                if (customInfoPage != null && customInfoPage.Any())
                    descriptionText.AddRange(customInfoPage);
            }

            var contentPages = descriptionText.Select(p => new RichContentPage(p)).ToList();
            var content = new RichContent(contentPages, new HashSet<string> { ImageProcessor.NotInlineImageTag });
            BookInfoViewModel.ChaptersContent = content;

            BookInfoViewModel.CoverpageBase64Image = bookModel.CoverpageBase64Image;
        }

        private void OnReadButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.NavigateContentFrame(typeof(ReadPage), bookModel);
        }
    }
}
