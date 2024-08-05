using System.Collections.ObjectModel;
using System.Linq;
using Fb2.Document.Html;
using static System.Reflection.Metadata.BlobBuilder;

namespace Fb2.Document.MAUI.Playground.Pages;

[QueryProperty(nameof(Book), "Book")]
public partial class ReadPage : ContentPage
{
    public BookModel? Book { get; set; }

    //public ObservableCollection<string> MappedBookParts { get; private set; } = new();

    public ReadPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var docment = Book?.Fb2Document;

        if (docment == null)
            return;

        try
        {
            var htmlSectionString = Fb2HtmlMapper
                .MapDocument(docment)
                .Select(s =>
@$"<div>
{s}
</div>")
                .ToList();

            //var webViews = htmlSectionString
            //    .Select(s => new WebView()
            //    {
            //        MinimumHeightRequest = 500,
            //        WidthRequest = BookViewportContainer.Width,
            //        MaximumWidthRequest = BookViewportContainer.Width,
            //        MinimumWidthRequest = BookViewportContainer.Width,
            //        VerticalOptions = LayoutOptions.FillAndExpand,
            //        HorizontalOptions = LayoutOptions.FillAndExpand,
            //        Margin = new Thickness(8),
            //        Source = new HtmlWebViewSource
            //        {
            //            Html = @$"<document>
            //{s}
            //</document>"
            //        }
            //    })
            //    .ToList();

            //for (int i = 0; i < webViews.Count; i++)
            //{
            //    var webView = webViews[i];
            //    BookViewportContainer.AddRowDefinition(new RowDefinition(GridLength.Auto));
            //    Grid.SetRow(webView, i);
            //    BookViewportContainer.Add(webView);
            //}

            var htmlBookString = string.Join(Environment.NewLine, htmlSectionString);

            HtmlWebView.Source = new HtmlWebViewSource
            {
                Html =
        @$"<document>
            {htmlBookString}
            </document>"
            };

        }
        catch (Exception)
        {
            //throw;
        }
    }
}