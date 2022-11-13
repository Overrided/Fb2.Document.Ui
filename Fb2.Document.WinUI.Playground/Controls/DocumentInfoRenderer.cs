using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models;
using Fb2.Document.UI.WinUi;
using Fb2.Document.WinUI.Playground.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RichTextView.DTOs;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground.Controls;

public class DocumentInfoRendererViewModel : ObservableObject
{
    private RichContent documentInfoContent;

    public RichContent DocumentInfoContent
    {
        get { return documentInfoContent; }
        set
        {
            OnPropertyChanging();
            documentInfoContent = value;
            OnPropertyChanged();
        }
    }
}

public sealed class DocumentInfoRenderer : Control
{
    public DocumentInfoRendererViewModel ViewModel { get; set; }

    public DocumentInfoRenderer()
    {
        this.DefaultStyleKey = typeof(DocumentInfoRenderer);
        ViewModel = new DocumentInfoRendererViewModel();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    public DocumentInfo DocumentInfo
    {
        get { return (DocumentInfo)GetValue(DocumentInfoProperty); }
        set { SetValue(DocumentInfoProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DocumentInfo.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DocumentInfoProperty =
        DependencyProperty.Register(
            nameof(DocumentInfo),
            typeof(DocumentInfo),
            typeof(DocumentInfoRenderer),
            new PropertyMetadata(null, new PropertyChangedCallback(OnDocumentInfoPropertyChangedCallback)));

    private static void OnDocumentInfoPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = d as DocumentInfoRenderer;
        if (sender == null)
        {
            return;
        }

        var documentInfo = sender.DocumentInfo;
        if (documentInfo == null)
        {
            return;
        }

        var mappedNodes = new Fb2Mapper().MapNode(
            documentInfo,
            Size.Empty,
            new(useStyles: false));

        var normalizedContent = mappedNodes
                .SelectMany(uic => uic)
                .ToList();

        var paragr = new Fb2.Document.UI.WinUi.Common.Utils().Paragraphize(normalizedContent);

        var contentPage = new RichContentPage(paragr);
        var content = new RichContent(new List<RichContentPage>(1) { contentPage });
        sender.ViewModel.DocumentInfoContent = content;
    }
}
