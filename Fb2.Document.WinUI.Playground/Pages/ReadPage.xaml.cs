using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.Playground.Models;
using Fb2.Document.WinUI.Playground.Services;
using Fb2.Document.WinUI.Playground.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using RichTextView.WinUI.DTOs;
using RichTextView.WinUI.EventArguments;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReadPage : Page
    {
        private Fb2Document selectedFb2Document = null;
        private Fb2DocumentMappingConfig defaultMappingConfig = new Fb2DocumentMappingConfig(false);

        public ReadViewModel ReadViewModel { get; }

        public ReadPage()
        {
            this.InitializeComponent();
            this.Loaded += ReadPage_Loaded;

            ReadViewModel = new ReadViewModel
            {
                ShowBookProgress = true,
                PageMargin = new Thickness(20, 40, 20, 40)
            };
        }

        private void ReadPage_Loaded(object sender, RoutedEventArgs e)
        {
            var uiContent = Fb2Mapper.Instance.MapDocument(selectedFb2Document, defaultMappingConfig);

            var resplitContent = uiContent
                .SelectMany(uic => uic.Chunk(50))
                .Select(rp => new RichContentPage(rp))
                .ToList();

            var content = new RichContent(resplitContent, new HashSet<string> { defaultMappingConfig.Image.NonInlineImageTag });

            ReadViewModel.ChaptersContent = content;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter == null)
                return;

            var bookModel = e.Parameter as BookModel;
            if (bookModel == null || bookModel.Fb2Document == null)
                return;

            //if(e.NavigationMode != NavigationMode.Back)

            selectedFb2Document = bookModel.Fb2Document;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void RichTextView_OnProgress(object sender, BookProgressChangedEventArgs e)
        {
            Debug.WriteLine($"Book current position: {e.ScrollableHeight}, vOffset: {e.VerticalOffset}");
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
            PopupInitializerService.Instance.InitializePopup(messageDialog);

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Close"));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;
            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 0;

            await messageDialog.ShowAsync();
        }

        private void viewPort_BookRendered(object sender, bool e)
        {
            Debug.WriteLine("Book rendered");
        }
    }
}
