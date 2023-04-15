using System.Collections.Generic;
using System.Windows.Documents;
using Fb2.Document.Constants;
using Fb2.Document.WPF.Entities;
using Fb2.Document.WPF.NodeProcessors.Base;

namespace Fb2.Document.WPF.NodeProcessors;

public class HyperlinkProcessor : DefaultNodeProcessor
{
    public override List<TextElement> Process(RenderingContext context)
    {
        var normalizedContent = base.Process(context);

        var hyperlink = new Hyperlink();
        hyperlink.Inlines.AddRange(normalizedContent);

        if (context.CurrentNode!.TryGetAttribute(AttributeNames.XHref, true, out var xHrefAttr))
        {
            var linkValue = xHrefAttr!.Value;
            SetTooltip(hyperlink, linkValue);
            hyperlink.Tag = linkValue;
        }

        return new List<TextElement>(1) { hyperlink };
    }
}
