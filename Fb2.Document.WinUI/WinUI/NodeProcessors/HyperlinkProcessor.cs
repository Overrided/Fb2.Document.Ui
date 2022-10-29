using System.Collections.Generic;
using Fb2.Document.Constants;
using Fb2.Document.UI.WinUi.Entities;
using Fb2.Document.UI.WinUi.Extensions;
using Fb2.Document.UI.WinUi.NodeProcessors.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.WinUi.NodeProcessors
{
    public class HyperlinkProcessor : RewrapNodeProcessorBase
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var normalizedContent = base.Process(context);

            var richContentWrapper = new RichTextBlock();
            richContentWrapper.Blocks.AddRange(normalizedContent);

            var hyperlinkButton = new HyperlinkButton
            {
                Margin = new Thickness(0, 2, 0, -5.5),
                Padding = new Thickness(0),
                FontSize = context.RenderingConfig.BaseFontSize,
                Content = richContentWrapper
            };

            if (context.CurrentNode.TryGetAttribute(AttributeNames.XHref, true, out var xHrefAttr))
            {
                var linkValue = xHrefAttr.Value;
                SetTooltip(hyperlinkButton, linkValue);
                hyperlinkButton.Tag = linkValue;
            }

            var inlineContainer = AddContainer(hyperlinkButton);

            return new List<TextElement>(1) { inlineContainer };
        }
    }
}
