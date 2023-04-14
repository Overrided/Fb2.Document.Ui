using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Constants;
using System.Windows.Documents;
using System.Windows;
using Fb2.Document.WPF.NodeProcessors.Base;
using Fb2.Document.WPF.Entities;
using System.Windows.Controls;

namespace Fb2.Document.WPF.NodeProcessors;

public class HyperlinkProcessor : RewrapNodeProcessorBase
{
    public override List<TextElement> Process(RenderingContext context)
    {
        var normalizedContent = base.Process(context);

        //var richContentWrapper = new RichTextBlock();

        //var txtb = new FlowDocument();
        //txtb.Blocks.AddRange(normalizedContent);

        //var txtbContainer = new FlowDocumentReader();
        //txtbContainer.Document = txtb;

        //richContentWrapper.Blocks.AddRange(normalizedContent);

        //var hyperlinkButton = new Button
        //{
        //    Margin = new Thickness(0, 2, 0, -5.5),
        //    Padding = new Thickness(0),
        //    FontSize = context.RenderingConfig.BaseFontSize,
        //    Content = txtbContainer,
        //    MaxHeight = 60
        //};

        var hyperlink = new Hyperlink();
        hyperlink.Inlines.AddRange(normalizedContent);

        if (context.CurrentNode.TryGetAttribute(AttributeNames.XHref, true, out var xHrefAttr))
        {
            var linkValue = xHrefAttr.Value;
            SetTooltip(hyperlink, linkValue);
            hyperlink.Tag = linkValue;
        }

        //var inlineContainer = AddContainer(hyperlinkButton);

        return new List<TextElement>(1) { hyperlink };
    }
}
