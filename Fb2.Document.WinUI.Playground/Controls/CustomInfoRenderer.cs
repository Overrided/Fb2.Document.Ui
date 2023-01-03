﻿using System.Collections.Generic;
using Fb2.Document.Models;
using Fb2.Document.WinUI.Playground.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using RichTextView.WinUI.DTOs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground.Controls;

public class CustomInfoRendererViewModel : ObservableObject
{
    private RichContent customInfoContent;

    public RichContent CustomInfoContent
    {
        get { return customInfoContent; }
        set
        {
            OnPropertyChanging();
            customInfoContent = value;
            OnPropertyChanged();
        }
    }
}

public sealed class CustomInfoRenderer : Control
{
    public CustomInfoRendererViewModel ViewModel { get; set; }

    public CustomInfoRenderer()
    {
        this.DefaultStyleKey = typeof(CustomInfoRenderer);
        ViewModel = new CustomInfoRendererViewModel();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }


    public CustomInfo CustomInfo
    {
        get { return (CustomInfo)GetValue(CustomInfoProperty); }
        set { SetValue(CustomInfoProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CustomInfo.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CustomInfoProperty =
        DependencyProperty.Register(
            nameof(CustomInfo),
            typeof(CustomInfo),
            typeof(CustomInfoRenderer),
            new PropertyMetadata(null, new PropertyChangedCallback(OnCustomInfoPropertyChangedCallback)));

    private static void OnCustomInfoPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = d as CustomInfoRenderer;

        if (sender == null)
            return;

        var customInfo = sender.CustomInfo;
        if (customInfo == null)
            return;

        var customInfoContent = customInfo.Content.Trim();
        var run = new Run { Text = customInfoContent };
        var paragraph = new Microsoft.UI.Xaml.Documents.Paragraph();
        paragraph.Inlines.Add(run); // dirty trick

        var contentPage = new RichContentPage(new List<TextElement>(1) { paragraph });
        var content = new RichContent(new List<RichContentPage>(1) { contentPage });
        sender.ViewModel.CustomInfoContent = content;

        //var mappedNodes = Fb2Mapper.Instance.MapNode(
        //    customInfo,
        //    Size.Empty,
        //    new(useStyles: false));

        //var normalizedContent = mappedNodes.SelectMany(uic => uic);

        //var contentPage = new RichContentPage(normalizedContent);
        //var content = new RichContent(new List<RichContentPage>(1) { contentPage });
        //sender.ViewModel.CustomInfoContent = content;
    }
}
