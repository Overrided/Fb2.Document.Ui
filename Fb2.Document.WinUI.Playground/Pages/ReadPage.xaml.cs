using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Fb2.Document.WinUI.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RichTextView.DTOs;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Fb2.Document.WinUI.Playground.ViewModels;
using Windows.Storage;
using System.Threading.Tasks;
using Fb2.Document.LoadingOptions;
using Fb2.Document.WinUI.Playground.Models;

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
        private Fb2Mapper fb2MappingService;
        private Fb2MappingConfig defaultMappingConfig = new Fb2MappingConfig();

        public ReadViewModel ReadViewModel { get; }

        public ReadPage()
        {
            this.InitializeComponent();
            viewPort.Loaded += ViewPort_Loaded;

            fb2MappingService = new Fb2Mapper();

            ReadViewModel = new ReadViewModel
            {
                ShowBookProgress = true,
                PageMargin = new Thickness(20)
            };
        }

        private void ViewPort_Loaded(object sender, RoutedEventArgs e)
        {
            var stop = Stopwatch.StartNew();

            var actualViewHostSize = viewPort.GetViewHostSize();
            //var smallFontConfig = new Fb2MappingConfig(14);
            //var unsafeMappingConfig = new Fb2MappingConfig(highlightUnsafe: true);
            var uiContent = fb2MappingService.MapDocument(selectedFb2Document, actualViewHostSize, defaultMappingConfig);

            stop.Stop();

            Debug.WriteLine($"UI Mapping elapsed: {stop.Elapsed}");

            //var content = new ChaptersContent(UiContent, pagePadding: defaultMappingConfig.PagePadding);
            var contentPages = uiContent.Select(p => new RichContentPage(p));
            var content = new ChaptersContent(contentPages);
            //var content = new ChaptersContent(contentPages, 71406.8);

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
            viewPort.Loaded -= ViewPort_Loaded;
        }

        private void OnBookRendered(object sender, EventArgs e)
        {
            Debug.WriteLine("Book rendered");
        }

        private void RichTextView_OnProgress(object sender, RichTextView.EventArguments.BookProgressChangedEventArgs e)
        {
            Debug.WriteLine($"Book current position: {e.VerticalOffset}, vOffset: {e.ScrollableOffset}");
        }

        private async void RichTextView_HyperlinkActivated(object sender, RichTextView.EventArguments.RichHyperlinkActivatedEventArgs e)
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
    }
}
