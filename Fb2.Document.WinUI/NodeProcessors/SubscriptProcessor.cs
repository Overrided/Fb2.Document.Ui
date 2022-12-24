using System.Collections.Generic;
using Fb2.Document.UI.Entities;
using Fb2.Document.UI.Extensions;
using Fb2.Document.UI.NodeProcessors.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.NodeProcessors
{
    public class SubscriptProcessor : RewrapNodeProcessorBase
    {
        public override List<TextElement> Process(RenderingContext context)
        {
            var normalizedContent = base.Process(context);

            var txtb = new RichTextBlock
            {
                FontSize = context.RenderingConfig.BaseFontSize,
                Margin = new Thickness(0, 1, 0, -10),
                Padding = new Thickness(0, 0, 0, 1)
            };
            txtb.Blocks.AddRange(normalizedContent);

            var inlineContainer = AddContainer(txtb);

            return new List<TextElement>(1) { inlineContainer };
        }
    }
}
