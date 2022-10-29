﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.UI.WinUi;
using Fb2.Document.WinUI.Playground.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using RichTextView.DTOs;
using Windows.Foundation;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fb2.Document.WinUI.Playground.Controls;

public class TitleInfoBaseRendererViewModel : ObservableObject
{
    private RichContentPage titleInfoContent;

    public RichContentPage TitleInfoContent
    {
        get { return titleInfoContent; }
        set
        {
            OnPropertyChanging();
            titleInfoContent = value;
            OnPropertyChanged();
        }
    }

}

public sealed class TitleInfoBaseRenderer : Control
{
    public TitleInfoBaseRendererViewModel ViewModel { get; set; }

    public TitleInfoBaseRenderer()
    {
        this.DefaultStyleKey = typeof(TitleInfoBaseRenderer);
        ViewModel = new TitleInfoBaseRendererViewModel();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    public TitleInfoBase TitleInfo
    {
        get { return (TitleInfoBase)GetValue(TitleInfoProperty); }
        set { SetValue(TitleInfoProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TitleInfo.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TitleInfoProperty =
        DependencyProperty.Register(
            "TitleInfo",
            typeof(TitleInfoBase),
            typeof(TitleInfoBaseRenderer),
            new PropertyMetadata(null, new PropertyChangedCallback(OnTitleInfoProperyChangedCallback)));

    private static void OnTitleInfoProperyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = d as TitleInfoBaseRenderer;
        if (sender == null)
        {
            return;
        }

        var titleInfo = sender.TitleInfo;
        if (titleInfo == null)
        {
            return;
        }

        var authors = titleInfo.GetDescendants<Author>();
        var titleInfoBookName = titleInfo.GetFirstDescendant<BookTitle>();
        var subTitle = titleInfo.GetFirstDescendant<SubTitle>();
        var genres = titleInfo.GetDescendants<BookGenre>();
        var annotation = titleInfo.GetFirstDescendant<Annotation>();
        var sequences = titleInfo.GetDescendants<SequenceInfo>();
        var keywords = titleInfo.GetDescendants<Keywords>();

        var nodes = new List<Fb2Node>();
        nodes.AddRange(authors.Where(a => a != null && !a.IsEmpty));

        if (titleInfoBookName != null)
            nodes.Add(titleInfoBookName);

        if (subTitle != null)
            nodes.Add(subTitle);

        nodes.AddRange(genres.Where(g => g != null && !g.IsEmpty));

        if (annotation != null)
            nodes.Add(annotation);

        nodes.AddRange(sequences.Where(s => s != null && !s.IsEmpty));
        nodes.AddRange(keywords.Where(k => k != null && !k.IsEmpty));

        var mappedNodes = new Fb2Mapper().MapNodes(nodes, Size.Empty);
        //var contentPage = new RichContentPage(mappedNodes);

        var normalizedContent = mappedNodes
                //.SelectMany(uic => uic.Chunk(50))
                .SelectMany(uic => uic)
                //.Select(rp => new RichContentPage(rp))
                .ToList();

        var paragr = new Fb2.Document.UI.WinUi.Common.Utils().Paragraphize(normalizedContent);

        var contentPage = new RichContentPage(paragr);
        sender.ViewModel.TitleInfoContent = contentPage;
    }
}