using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Fb2.Document.Constants;
using Fb2.Document.Models.Base;
using Fb2.Document.WPF.Entities;

namespace Fb2.Document.WPF.Services;

public class ElementStyler
{
    private static readonly Lazy<ElementStyler> instance = new Lazy<ElementStyler>(() => new ElementStyler(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static ElementStyler Instance => instance.Value;

    private const string MonotypeFontFamily = "Courier New";

    private readonly Dictionary<string, Action<RenderingContext, TextElement>> styles;

    private ElementStyler()
    {
        styles = new Dictionary<string, Action<RenderingContext, TextElement>>
            {
                { ElementNames.TableHeader, (context, el) => el.FontWeight = FontWeights.SemiBold },
                //{ ElementNames.Strikethrough, (context, el) => el.TextDecorations = TextDecorations.Strikethrough },
                { ElementNames.Author, (context, el) =>
                {
                    Debug.WriteLine("applying styles to author");

                    var baseFontSize = context.RenderingConfig.BaseFontSize;
                    var desiredLineHeight = baseFontSize * 2;

                    var p = el as Paragraph;
                    p.TextAlignment = TextAlignment.Center;
                    p.TextIndent = 0;
                    p.FontSize = baseFontSize;
                    p.LineHeight = desiredLineHeight;
                }},
                { ElementNames.BookTitle, (context, el) =>
                {
                    Debug.WriteLine("applying styles to book title");

                    var desiredFontSize = context.RenderingConfig.BaseFontSize + 6;
                    var desiredLineHeight = desiredFontSize * 2.5;

                    el.FontSize = desiredFontSize;
                    el.FontWeight = FontWeights.Bold;
                    var p = el as Paragraph;

                    p.TextAlignment = TextAlignment.Center;
                    p.TextIndent = 0;
                    p.LineHeight = desiredLineHeight;
                    p.Margin = new Thickness(0, 10, 0, 10);
                }},
                { ElementNames.BookName, (context, el) =>
                {
                    Debug.WriteLine("applying styles to book name");

                    // well, fuck.
                    // hack to apply different styles in different circumstances
                    Fb2Node lastParent = context.CurrentNode.Parent;

                    if(lastParent == null ||
                       lastParent.Name != ElementNames.PublishInfo)
                    {
                        var desiredFontSize = context.RenderingConfig.BaseFontSize + 6;
                        var desiredLineHeight = desiredFontSize * 2.5;

                        var p = el as Paragraph;
                        p.FontSize = desiredFontSize;
                        p.FontWeight = FontWeights.Bold;
                        p.TextAlignment = TextAlignment.Center;
                        p.TextIndent = 0;
                        p.LineHeight = desiredLineHeight;
                        p.Margin = new Thickness(0, 10, 0, 10);
                    }
                    else
                    {
                        el.FontSize = context.RenderingConfig.BaseFontSize - 1.5;
                    }
                }},
                { ElementNames.Sequence, (context, el) =>
                {
                    // TODO: fix
                    // well, fuck.
                    // hack to apply different styles in different circumstances
                    var lastParent = context.CurrentNode.Parent;

                    if(lastParent == null ||
                       lastParent.Name != ElementNames.PublishInfo)
                    {
                        var desiredFontSize = context.RenderingConfig.BaseFontSize + 1.5;
                        var desiredLineHeight = desiredFontSize * 2;

                        el.FontSize = desiredFontSize;
                        var p = el as Paragraph;
                        p.TextAlignment = TextAlignment.Center;
                        p.TextIndent = 0;
                        p.LineHeight = desiredLineHeight;
                    }
                    else
                    {
                        el.FontSize = context.RenderingConfig.BaseFontSize - 1.5;
                    }
                }},
                { ElementNames.Year, (context, el) =>
                {
                    var p = el as Paragraph;
                    p.FontSize = context.RenderingConfig.BaseFontSize - 1.5;
                }},
                { ElementNames.ISBN, (context, el) =>
                {
                    var p = el as Paragraph;
                    p.FontSize = context.RenderingConfig.BaseFontSize - 1.5;
                }},
                { ElementNames.Publisher, (context, el) =>
                {
                    var p = el as Paragraph;
                    p.FontSize = context.RenderingConfig.BaseFontSize - 1.5;
                }},
                { ElementNames.City, (context, el) =>
                {
                    var p = el as Paragraph;
                    p.FontSize = context.RenderingConfig.BaseFontSize - 1.5;
                }},
                { ElementNames.Code, (context, el) =>
                {
                    //el.FontStretch = FontStretch.SemiCondensed;
                    el.FontFamily = new FontFamily(MonotypeFontFamily);
                    el.Foreground = new SolidColorBrush(Colors.Gray);
                }},
                { ElementNames.Paragraph, (context, el) =>
                {
                    var p = el as Paragraph;
                    p.TextIndent = context.RenderingConfig.ParagraphIndent;
                    p.FontSize = context.RenderingConfig.BaseFontSize;
                }},
                { ElementNames.TextAuthor, (context, el) => el.FontSize = context.RenderingConfig.BaseFontSize - 2 },
                { ElementNames.Date, (context, el) => el.FontSize = context.RenderingConfig.BaseFontSize - 2 },
                { $"{ElementNames.BookBody}|{ElementNames.Title}", (context, el) =>
                {
                    var desiredFontSize = context.RenderingConfig.BaseFontSize + 4;
                    var desiredLineHeight = desiredFontSize * 3;

                    el.FontSize = desiredFontSize;
                    el.FontWeight = FontWeights.SemiBold;
                    var p = el as Paragraph;
                    p.TextAlignment = context.RenderingConfig.Body.TitleHorizontalAlignment;
                    p.TextIndent = 0;
                    p.LineHeight = desiredLineHeight;
                    p.Margin = new Thickness(0, 10, 0, 10);
                }},
                { $"{ElementNames.BookBody}|{ElementNames.Epigraph}", (context, el) => (el as Paragraph).TextAlignment = context.RenderingConfig.Body.EpigraphHorizontalAlignment },
                { $"{ElementNames.BookBodySection}|{ElementNames.Title}", (context, el) =>
                {
                    var desiredFontSize = context.RenderingConfig.BaseFontSize + 3;
                    var desiredLineHeight = desiredFontSize * 2.5;

                    el.FontSize = desiredFontSize;
                    el.FontWeight = FontWeights.Medium;
                    var p = el as Paragraph;
                    p.TextAlignment = context.RenderingConfig.Section.TitleHorizontalAlignment;
                    p.TextIndent = 0;
                    p.LineHeight = desiredLineHeight;
                    p.Margin = new Thickness(0, 10, 0, 10);
                }},
                { $"{ElementNames.BookBodySection}|{ElementNames.SubTitle}", (context, el) =>
                {
                    var desiredFontSize = context.RenderingConfig.BaseFontSize + 2;
                    var desiredLineHeight = desiredFontSize * 2.5;

                    el.FontSize = desiredFontSize;
                    el.FontWeight = FontWeights.Medium;
                    var p = el as Paragraph;
                    p.TextAlignment = context.RenderingConfig.Section.SubtitleHorizontalAlignment;
                    p.TextIndent = 0;
                    p.LineHeight = desiredLineHeight;
                    p.Margin = new Thickness(0, 10, 0, 10);
                }},
                { $"{ElementNames.BookBodySection}|{ElementNames.Epigraph}", (context, el) => (el as Paragraph).TextAlignment = context.RenderingConfig.Section.EpigraphHorizontalAlignment },
                { $"{ElementNames.Poem}|{ElementNames.Title}", (context, el) =>
                {
                    var desiredFontSize = context.RenderingConfig.BaseFontSize + 1;
                    var desiredLineHeight = desiredFontSize * 2;

                    el.FontSize = desiredFontSize;
                    el.FontWeight = FontWeights.Medium;
                    var p = el as Paragraph;
                    p.TextAlignment = context.RenderingConfig.Poem.TitleHorizontalAlignment;
                    p.TextIndent = 0;
                    p.LineHeight = desiredLineHeight;
                    p.Margin = new Thickness(0, 10, 0, 10);
                }},
                { $"{ElementNames.Poem}|{ElementNames.Epigraph}", (context, el) => (el as Paragraph).TextAlignment = context.RenderingConfig.Poem.EpigraphHorizontalAlignment },
                { $"{ElementNames.Poem}|{ElementNames.Date}", (context, el) => (el as Paragraph).TextAlignment = context.RenderingConfig.Poem.DateHorizontalAlignment },
                { $"{ElementNames.Poem}|{ElementNames.TextAuthor}", (context, el) => (el as Paragraph).TextAlignment = context.RenderingConfig.Poem.TextAuthorHorizontalAlignment },
                { $"{ElementNames.Stanza}|{ElementNames.StanzaV}",(context, el) =>
                {
                    var p = el as Paragraph;
                    p.TextAlignment = context.RenderingConfig.Poem.VerseHorizontalAlignment;
                    p.FontSize = context.RenderingConfig.BaseFontSize;
                }},
                { $"{ElementNames.Stanza}|{ElementNames.SubTitle}", (context, el) =>
                {
                    var desiredFontSize = context.RenderingConfig.BaseFontSize + 1;
                    var desiredLineHeight = desiredFontSize * 2;

                    el.FontSize = desiredFontSize;
                    el.FontWeight = FontWeights.Medium;
                    var p = el as Paragraph;

                    p.TextAlignment = context.RenderingConfig.Poem.SubtitleHorizontalAlignment;
                    p.TextIndent = 0;
                    p.LineHeight = desiredLineHeight;
                    p.Margin = new Thickness(0, 10, 0, 10);
                }},
                { $"{ElementNames.Quote}|{ElementNames.SubTitle}", (context, el) =>
                {
                    var desiredFontSize = context.RenderingConfig.BaseFontSize + 1;
                    var desiredLineHeight = desiredFontSize * 2;

                    el.FontSize = desiredFontSize;
                    el.FontWeight = FontWeights.Medium;

                    var p = el as Paragraph;
                    p.TextAlignment = context.RenderingConfig.Quote.SubtitleHorizontalAlignment;
                    p.TextIndent = 0;
                    p.LineHeight = desiredLineHeight;
                    p.Margin = new Thickness(0, 10, 0, 10);
                }},
                { $"{ElementNames.Quote}|{ElementNames.TextAuthor}", (context, el) => (el as Paragraph).TextAlignment = context.RenderingConfig.Quote.TextAuthorHorizontalAlignment },
                { $"{ElementNames.Annotation}|{ElementNames.SubTitle}", (context, el) =>
                {
                    var desiredFontSize = context.RenderingConfig.BaseFontSize + 1;
                    var desiredLineHeight = desiredFontSize * 2;

                    el.FontSize = desiredFontSize;
                    el.FontWeight = FontWeights.Medium;

                    var p = el as Paragraph;
                    p.TextAlignment = context.RenderingConfig.Annotation.SubTitleHorizontalAlignment;

                    p.TextIndent = 0;
                    p.LineHeight = desiredLineHeight;
                    p.Margin = new Thickness(0, 10, 0, 10);
                }},
                { ElementNames.CustomInfo, (context, el) =>
                {
                    // TODO : configurable
                    el.FontSize = context.RenderingConfig.BaseFontSize - 1.5;
                }}
            };
    }

    public void ApplyStyle(RenderingContext context, List<TextElement> elements)
    {
        var currentNode = context.CurrentNode;
        //var parent = context.ParentNodes.Any() ? context.ParentNodes.Peek() : null;
        var parent = currentNode.Parent;

        if (parent != null && styles.ContainsKey($"{parent.Name}|{currentNode.Name}")) // TODO: can be extended to any parent up there
        {
            var parentStylePredicate = styles[$"{parent.Name}|{currentNode.Name}"];
            elements.ForEach(te => parentStylePredicate(context, te));
        }

        if (styles.ContainsKey(currentNode.Name))
        {
            var ownStylePredicate = styles[currentNode.Name];
            elements.ForEach(te => ownStylePredicate(context, te));
        }

        if (context.RenderingConfig.HighlightUnsafe && currentNode.IsUnsafe)
            elements.ForEach(te => te.Foreground = new SolidColorBrush(Colors.Red));
    }
}

