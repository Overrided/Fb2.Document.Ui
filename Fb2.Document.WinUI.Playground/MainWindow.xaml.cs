using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Fb2.Document.LoadingOptions;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.Playground.PageNavigation;
using Fb2.Document.WinUI.Playground.Pages;
using Fb2.Document.WinUI.Playground.Services;
using Fb2.Document.WinUI.Playground.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using RichTextView.DTOs;
using RichTextView.EventArguments;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground
{
    //[ComImport]
    //[Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface IInitializeWithWindow
    //{
    //    void Initialize(IntPtr hwnd);
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
    //internal interface IWindowNative
    //{
    //    IntPtr WindowHandle { get; }
    //}

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //private Fb2Document selectedFb2Document = null;
        private Fb2Mapper fb2MappingService = new();
        private Fb2MappingConfig defaultMappingConfig = new();

        public ReadViewModel ReadViewModel { get; }

        public MainWindow()
        {
            this.Activated += MainWindow_Activated;
            this.InitializeComponent();

            ReadViewModel = new()
            {
                ShowBookProgress = true,
                PageMargin = new Thickness(20)
            };
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (!PopupInitializerService.Instance.IsServiceInitialized)
                PopupInitializerService.Instance.Initialize(this);

            if (ContentFrame.Content == null || ContentFrame.Content?.GetType() != typeof(BookshelfPage))
                ContentFrame.Navigate(typeof(BookshelfPage));
        }

        //private async void OpenBookClick(object sender, RoutedEventArgs e)
        //{
        //    var picker = new FileOpenPicker { ViewMode = PickerViewMode.List };
        //    var pickerTwo = new FolderPicker();

        //    //Get the Window's HWND
        //    var hwnd = this.As<IWindowNative>().WindowHandle;

        //    var initializeWithWindow = picker.As<IInitializeWithWindow>();
        //    initializeWithWindow.Initialize(hwnd);

        //    picker.FileTypeFilter.Add(".fb2");

        //    StorageFile file = await picker.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        // Application now has read/write access to the picked file
        //        Debug.WriteLine("Picked file: " + file.Name);

        //        var doc = await LoadDocument(file);

        //        var actualViewHostSize = viewPort.GetViewHostSize();
        //        var uiContent = fb2MappingService.MapDocument(doc, actualViewHostSize, defaultMappingConfig);

        //        Debug.WriteLine($"UI Mapping done");

        //        var contentPages = uiContent.Select(p => new RichContentPage(p));
        //        var content = new ChaptersContent(contentPages);

        //        ReadViewModel.ChaptersContent = content;
        //    }
        //}

        private async Task<Fb2Document> LoadDocument(StorageFile storageFile)
        {
            var fb2Doc = new Fb2Document();
            using (var dataStream = await storageFile.OpenStreamForReadAsync())
            {
                await fb2Doc.LoadAsync(dataStream, new Fb2StreamLoadingOptions(true));
            }

            return fb2Doc;
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

            MessageDialog messageDialog = new($"test inline hyperlink click: {testMessage}")
            {
                DefaultCommandIndex = 0,
                CancelCommandIndex = 0
            };

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Close"));

            var hwnd = this.As<IWindowNative>().WindowHandle;

            var initializeWithWindow = messageDialog.As<IInitializeWithWindow>();
            initializeWithWindow.Initialize(hwnd);

            await messageDialog.ShowAsync();
        }

        private void RichTextView_OnProgress(object sender, BookProgressChangedEventArgs e)
        {
            Debug.WriteLine($"Book current position: {e.VerticalOffset}, vOffset: {e.ScrollableOffset}");
        }

        private void OnBookRendered(object sender, EventArgs e)
        {
            Debug.WriteLine("Book rendered");
        }
    }
}
