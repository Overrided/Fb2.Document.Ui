using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models;
using Fb2.Document.WinUI.Playground.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RichTextView.WinUI.DTOs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground.Controls;

public class PublishInfoRendererViewModel : ObservableObject
{
    private RichContent publishInfoContent;

    public RichContent PublishInfoContent
    {
        get { return publishInfoContent; }
        set
        {
            OnPropertyChanging();
            publishInfoContent = value;
            OnPropertyChanged();
        }
    }
}

// TODO : create generic renderer - like BookInfoRenderer<T> where T : PublishInfo, TitleInfoBase, etc??
// can we have `public T ` dependency property??

public sealed class PublishInfoRenderer : Control
{
    public PublishInfoRendererViewModel ViewModel { get; set; }

    public PublishInfoRenderer()
    {
        this.DefaultStyleKey = typeof(PublishInfoRenderer);
        ViewModel = new PublishInfoRendererViewModel();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    public PublishInfo PublishInfo
    {
        get { return (PublishInfo)GetValue(PublishInfoProperty); }
        set { SetValue(PublishInfoProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PublishInfoProperty =
        DependencyProperty.Register(
            nameof(PublishInfo),
            typeof(PublishInfo),
            typeof(PublishInfoRenderer),
            new PropertyMetadata(null, new PropertyChangedCallback(OnPublishInfoPropertyChangedCallback)));

    private static void OnPublishInfoPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = d as PublishInfoRenderer;
        if (sender == null)
        {
            return;
        }

        var publishInfo = sender.PublishInfo;
        if (publishInfo == null)
        {
            return;
        }

        //var mappedNodes = new Fb2Mapper().MapNode(publishInfo, Size.Empty);

        var mappedNodes = Fb2Mapper.Instance.MapNode(
            publishInfo,
            new(useStyles: false));

        var normalizedContent = mappedNodes.SelectMany(uic => uic);

        var contentPage = new RichContentPage(normalizedContent);
        var content = new RichContent(new List<RichContentPage>(1) { contentPage });
        sender.ViewModel.PublishInfoContent = content;
    }
}
