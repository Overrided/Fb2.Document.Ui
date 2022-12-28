using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Constants;
using Fb2.Document.Models.Base;
using Fb2.Document.UWP.Entities;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Fb2.Document.UWP.Services
{
    public class ElementStyler
    {
        private const string MonotypeFontFamily = "Courier New";

        private readonly Dictionary<string, Action<IRenderingContext, TextElement>> styles = new Dictionary<string, Action<IRenderingContext, TextElement>>
        {
            { ElementNames.TableHeader, (context, el) => el.FontWeight = FontWeights.SemiBold },
            { ElementNames.Strikethrough, (context, el) => el.TextDecorations = TextDecorations.Strikethrough },
            { ElementNames.Author, (context, el) =>
            {
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
                // well, fuck.
                // hack to apply different styles in different circumstances
                Fb2Node lastParent = null;

                var canPeek = (context.ParentNodes?.Count ?? 0) > 0;
                if(canPeek)
                    lastParent = context.ParentNodes.Peek();

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
                // well, fuck.
                // hack to apply different styles in different circumstances
                Fb2Node lastParent = null;

                var canPeek = (context.ParentNodes?.Count ?? 0) > 0;
                if(canPeek)
                    lastParent = context.ParentNodes.Peek();

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
                el.FontStretch = FontStretch.SemiCondensed;
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

        public void ApplyStyle(IRenderingContext context, List<TextElement> elements)
        {
            var currentNode = context.Node;
            var parent = context.ParentNodes.Any() ? context.ParentNodes.Peek() : null;

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
}
