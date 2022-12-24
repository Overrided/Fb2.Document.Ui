using Fb2.Document.Html;

namespace Fb2.Document.MAUI.Playground.Pages;

[QueryProperty(nameof(Book), "Book")]
public partial class ReadPage : ContentPage
{
    public BookModel Book { get; set; }

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

        //activityIndicator.IsRunning = true;
        //activityIndicator.IsVisible = true;

        try
        {
            var htmlBookString = Fb2HtmlMapper.MapDocument(docment);
            var normalizedString = @$"<document>
{htmlBookString}
</document>";

            HtmlWebView.Source = new HtmlWebViewSource
            {
                Html = htmlBookString
            };
        }
        catch (Exception)
        {
            //throw;
        }
    }
}