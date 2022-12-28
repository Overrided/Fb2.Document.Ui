using System;
using System.Diagnostics;
using System.Linq;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.Playground.Common;
using Fb2.Document.UWP.Playground.Models;
using RichTextView.UWP.DTOs;
using RichTextView.UWP.EventArguments;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Fb2.Document.UWP.Playground.Pages
{

    public class ReadViewModel : ObservableObject
    {
        private bool showBookProgress;

        private Thickness pageMargin;

        private ChaptersContent chaptersContent;

        public bool ShowBookProgress
        {
            get { return showBookProgress; }
            set
            {
                if (showBookProgress != value)
                {
                    OnPropertyChanging();
                    showBookProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public Thickness PageMargin
        {
            get { return pageMargin; }
            set
            {
                if (pageMargin != value)
                {
                    OnPropertyChanging();
                    pageMargin = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public ReadViewModel(
            ChaptersContent chaptersContent = null,
            Thickness? suggestedPageMargin = null,
            bool? showBookProgress = null)
        {
            if (chaptersContent != null)
                ChaptersContent = chaptersContent;

            if (suggestedPageMargin.HasValue)
                PageMargin = suggestedPageMargin.Value;

            if (showBookProgress.HasValue)
                ShowBookProgress = showBookProgress.Value;
        }
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReadPage : Page
    {
        private Fb2Document selectedFb2Document = null;
        //private Fb2Mapper fb2MappingService = null;
        private Fb2MappingConfig defaultMappingConfig = new Fb2MappingConfig();

        public ReadViewModel ReadViewModel { get; private set; }

        public ReadPage()
        {
            this.InitializeComponent();

            viewPort.Loaded += ViewPort_Loaded;

            //fb2MappingService = new Fb2Mapper();

            ReadViewModel = new ReadViewModel
            {
                ShowBookProgress = true,
                PageMargin = new Thickness(20)
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var bookModel = e.Parameter as BookModel;
            if (bookModel == null || bookModel.Fb2Document == null || !bookModel.Fb2Document.IsLoaded)
                return;

            selectedFb2Document = bookModel.Fb2Document;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            viewPort.Loaded -= ViewPort_Loaded;
            viewPort.HyperlinkActivated -= RichTextView_HyperlinkActivated;
            viewPort.BookProgressChanged -= RichTextView_OnProgress;
            viewPort.BookRendered -= OnBookRendered;
            viewPort = null;

            if (ReadViewModel?.ChaptersContent != null)
            {
                foreach (var page in ReadViewModel.ChaptersContent.RichContentPages)
                {
                    page.Clear();
                }

                ReadViewModel.ChaptersContent = null;
                ReadViewModel = null;
            }

            //fb2MappingService = null;
            selectedFb2Document = null;
        }

        private void ViewPort_Loaded(object sender, RoutedEventArgs e)
        {
            var stop = Stopwatch.StartNew();

            var actualViewHostSize = viewPort.GetViewHostSize();
            //var smallFontConfig = new Fb2MappingConfig(14);
            //var unsafeMappingConfig = new Fb2MappingConfig(highlightUnsafe: true);
            var uiContent = Fb2Mapper.Instance.MapDocument(selectedFb2Document, actualViewHostSize, new Fb2DocumentMappingConfig());

            stop.Stop();

            Debug.WriteLine($"UI Mapping elapsed: {stop.Elapsed}");

            //var content = new ChaptersContent(UiContent, pagePadding: defaultMappingConfig.PagePadding);
            var contentPages = uiContent.Select(p => new RichContentPage(p));
            var content = new ChaptersContent(contentPages);
            //var content = new ChaptersContent(contentPages, 71406.8);

            ReadViewModel.ChaptersContent = content;
        }

        private async void RichTextView_HyperlinkActivated(object sender, RichHyperlinkActivatedEventArgs e)
        {
            string testMessage;

            if (e.OriginalSender is Hyperlink hyperlink)
                testMessage = hyperlink.NavigateUri?.ToString() ?? "No Hyperlink";
            else if (e.OriginalSender is HyperlinkButton hyperlinkButton)
                testMessage = hyperlinkButton.Tag.ToString();
            else
                throw new Exception("unexpected hyprlink btn");

            var messageDialog = new MessageDialog($"test inline hyperlink click: {testMessage}");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Close"));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;
            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 0;

            await messageDialog.ShowAsync();
        }

        private void RichTextView_OnProgress(object sender, BookProgressChangedEventArgs e)
        {
            Debug.WriteLine($"Book current position: {e.VerticalOffset}, vOffset: {e.ScrollableHeight}");
        }

        private void OnBookRendered(object sender, bool e)
        {
            Debug.WriteLine($"Book rendered : {e}");
        }
    }
}
