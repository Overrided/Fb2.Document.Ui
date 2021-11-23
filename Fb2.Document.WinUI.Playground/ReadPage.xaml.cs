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
using Fb2.Document.WinUI.Playground.PageNavigation;
using Fb2.Document.WinUI.Playground.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground
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
            fb2MappingService = new Fb2Mapper();

            ReadViewModel = new ReadViewModel
            {
                ShowBookProgress = true,
                PageMargin = new Thickness(20)
            };
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var prm = e.Parameter as ReadPageNavigationParams;
            var window = prm.BaseWindow;

            //var picker = new FileOpenPicker();

            ////Get the Window's HWND
            //var hwnd = Window.Current.As<IWindowNative>().WindowHandle;

            ////Make folder Picker work in Win32
            //var initializeWithWindow = picker.As<IInitializeWithWindow>();
            //initializeWithWindow.Initialize(hwnd);

            //picker.ViewMode = PickerViewMode.Thumbnail;
            //picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            //picker.FileTypeFilter.Add(".fb2");

            //var selectedFile = await picker.PickSingleFileAsync();


            //var fb2Doc = new Fb2Document();

            //using (var dataStream = await selectedFile.OpenStreamForReadAsync())
            //{
            //    await fb2Doc.LoadAsync(dataStream);
            //}

            //selectedFb2Document = fb2Doc;
            //var actualViewHostSize = viewPort.GetViewHostSize();

            //var uiContent = fb2MappingService.MapDocument(selectedFb2Document, actualViewHostSize, defaultMappingConfig);
            //var contentPages = uiContent.Select(p => new RichContentPage(p));
            //var content = new ChaptersContent(contentPages);
            ////var content = new ChaptersContent(contentPages, 71406.8);

            //ReadViewModel.ChaptersContent = content;


            //Get the Window's HWND
            //var hwnd = window.As<IWindowNative>().WindowHandle;

            //var filePicker = new FileOpenPicker();

            ////Make folder Picker work in Win32
            //var initializeWithWindow = filePicker.As<IInitializeWithWindow>();
            //initializeWithWindow.Initialize(hwnd);

            //filePicker.FileTypeFilter.Add("*");

            //var folder = await filePicker.PickSingleFileAsync();
            //var a = 1;



            var picker = new Windows.Storage.Pickers.FileOpenPicker();

            //Get the Window's HWND
            var hwnd = window.As<IWindowNative>().WindowHandle;

            //Make folder Picker work in Win32
            var initializeWithWindow = picker.As<IInitializeWithWindow>();
            initializeWithWindow.Initialize(hwnd);

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                Debug.WriteLine("Picked photo: " + file.Name);
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }

        }


        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            IntPtr WindowHandle { get; }
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
