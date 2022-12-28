using System.Collections.Generic;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.Extensions;
using Fb2.Document.UWP.NodeProcessors.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.NodeProcessors
{
    public class SubscriptProcessor : RewrapNodeProcessorBase
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var rewrappedNode = RewrapNode(context);

            var inlines = rewrappedNode != null ? ElementSelector(rewrappedNode, context) : base.Process(context);
            var normalizedInlines = context.Utils.Paragraphize(inlines);

            var txtb = new RichTextBlock
            {
                FontSize = context.RenderingConfig.BaseFontSize,
                Margin = new Thickness(0, 9, 0, -9)
            };
            txtb.Blocks.AddRange(normalizedInlines);

            var inlineContainer = AddContainer(txtb);

            return new List<TextElement>(1) { inlineContainer };
        }
    }
}
