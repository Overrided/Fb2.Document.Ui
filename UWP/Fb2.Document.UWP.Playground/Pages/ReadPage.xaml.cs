using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.NodeProcessors;
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

        private RichContent chaptersContent;

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

        public RichContent ChaptersContent
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
            RichContent chaptersContent = null,
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
        private Fb2DocumentMappingConfig defaultMappingConfig = new Fb2DocumentMappingConfig();

        public ReadViewModel ReadViewModel { get; private set; }

        public ReadPage()
        {
            this.InitializeComponent();
            this.Loaded += ReadPage_Loaded;

            ReadViewModel = new ReadViewModel
            {
                ShowBookProgress = true,
                PageMargin = new Thickness(20)
            };
        }

        private void ReadPage_Loaded(object sender, RoutedEventArgs e)
        {
            var uiContent = Fb2Mapper.Instance.MapDocument(selectedFb2Document, defaultMappingConfig);

            //var content = new ChaptersContent(UiContent, pagePadding: defaultMappingConfig.PagePadding);
            var contentPages = uiContent.Select(p => new RichContentPage(p));
            var content = new RichContent(contentPages, new HashSet<string> { defaultMappingConfig.Image.NonInlineImageTag });
            //var content = new ChaptersContent(contentPages, 71406.8);

            ReadViewModel.ChaptersContent = content;
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

            //viewPort.Loaded -= ViewPort_Loaded;
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
