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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fb2.Document.UWP.Playground.Pages
{
    // TODO : move to view models
    public class BookInfoViewModel : ObservableObject
    {
        private string coverpageBase64Image;

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

        private ChaptersContent chaptersContent;

        public ChaptersContent ChaptersContent
        {
            get { return chaptersContent; }
            set
            {
                OnPropertyChanging();
                chaptersContent = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookInfoPage : Page
    {
        //private Fb2Mapper fb2MappingService = new Fb2Mapper();
        private Fb2MappingConfig defaultMappingConfig = new Fb2MappingConfig();
        public BookInfoViewModel BookInfoViewModel { get; private set; } = new BookInfoViewModel();
        private BookModel bookModel = null;

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
            BookInfoViewModel.CoverpageBase64Image = bookModel.CoverpageBase64Image;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            descriptionViewPort.Loaded -= DescriptionViewPort_Loaded;

            if (BookInfoViewModel?.ChaptersContent != null)
            {
                BookInfoViewModel.CoverpageBase64Image = null;
                foreach (var page in BookInfoViewModel?.ChaptersContent?.RichContentPages)
                {
                    page.Clear();
                }

                BookInfoViewModel = null;
            }

            //fb2MappingService = null;
            bookModel = null;
        }

        private void DescriptionViewPort_Loaded(object sender, RoutedEventArgs e)
        {
            Fb2Container titleInfo = (Fb2Container)bookModel.Fb2Document.Title ?? (Fb2Container)bookModel.Fb2Document.SourceTitle;

            var titleInfoAuthors = titleInfo?.GetDescendants<Author>();

            var titleInfoBookName = titleInfo?.GetFirstDescendant<BookTitle>() as Fb2Node;
            var publishInfoBookName = bookModel.Fb2Document.PublishInfo?.GetFirstChild<BookName>() as Fb2Node;
            var bookTitle = titleInfoBookName ?? publishInfoBookName;
            var annotation = titleInfo?.GetFirstDescendant<Annotation>();
            var tiltleSequences = titleInfo?.GetDescendants<SequenceInfo>();
            var publishInfoSequences = bookModel.Fb2Document.PublishInfo?.GetDescendants(ElementNames.Sequence);

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
            var descriptionText = Fb2Mapper.Instance.MapNodes(nodes, size).ToList();

            // we have had sequence info mapped before
            // use Remove api once new lib is hooked
            var publishInfo = bookModel.Fb2Document.PublishInfo;
            if (publishInfo != null && publishInfo.Content.Any())
            {
                publishInfo.Content.RemoveAll(n => n is SequenceInfo);
                var publishInfoPage = Fb2Mapper.Instance.MapNode(publishInfo, size);
                if (publishInfoPage != null && publishInfoPage.Any())
                    descriptionText.AddRange(publishInfoPage);
            }

            var customInfo = bookModel.Fb2Document.CustomInfo;
            if (customInfo != null)
            {
                var customInfoPage = Fb2Mapper.Instance.MapNode(customInfo, size);
                if (customInfoPage != null && customInfoPage.Any())
                    descriptionText.AddRange(customInfoPage);
            }

            var contentPages = descriptionText.Select(p => new RichContentPage(p));
            var content = new ChaptersContent(contentPages);
            BookInfoViewModel.ChaptersContent = content;
        }

        private void OnReadButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.NavigateContentFrame(typeof(ReadPage), bookModel);
        }

        private async void BookInfo_HyperlinkActivated(object sender, RichTextView.UWP.EventArguments.RichHyperlinkActivatedEventArgs e)
        {
            string testMessage;

            if (e.OriginalSender is Hyperlink hyperlink)
                testMessage = hyperlink.NavigateUri?.ToString() ?? "No Hyperlink";
            else if (e.OriginalSender is HyperlinkButton hyperlinkButton)
                testMessage = hyperlinkButton.Tag.ToString();
            else
                throw new Exception("unexpected hyprlink btn");

            var messageDialog = new MessageDialog($"book info hyperlink click: {testMessage}");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Close"));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;
            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 0;

            await messageDialog.ShowAsync();
        }
    }
}
