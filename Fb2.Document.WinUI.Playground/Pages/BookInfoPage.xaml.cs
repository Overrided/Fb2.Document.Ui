using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.UI.WinUi;
using Fb2.Document.UI.WinUi.Entities;
using Fb2.Document.UI.WinUi.NodeProcessors;
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
            this.Loaded += BookInfoPage_Loaded;
        }

        private void BookInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            BookInfoViewModel.SrcTitleInfo = bookModel?.Fb2Document?.SourceTitle;
            BookInfoViewModel.TitleInfo = bookModel?.Fb2Document?.Title;
            BookInfoViewModel.CoverpageBase64Image = bookModel.CoverpageBase64Image;
            BookInfoViewModel.PublishInfo = bookModel.Fb2Document.PublishInfo;
            BookInfoViewModel.CustomInfo = bookModel.Fb2Document.CustomInfo;
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

            //this.descriptionViewPort.Loaded -= DescriptionViewPort_Loaded;
        }

        private void OnReadButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.NavigateContentFrame(typeof(ReadPage), bookModel);
        }
    }
}
