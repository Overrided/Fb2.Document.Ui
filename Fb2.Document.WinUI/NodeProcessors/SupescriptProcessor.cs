using System.Collections.Generic;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.Extensions;
using Fb2.Document.WinUI.NodeProcessors.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors
{
    public class SupescriptProcessor : RewrapNodeProcessorBase
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var rewrappedNode = RewrapNode(context);

            var inlines = rewrappedNode != null ? ElementSelector(rewrappedNode, context) : base.Process(context);
            var normalizedInlines = context.Utils.Paragraphize(inlines);

            var txtb = new RichTextBlock
            {
                FontSize = context.RenderingConfig.BaseFontSize,
                Margin = new Thickness(0, 0, 0, 5)
            };
            txtb.Blocks.AddRange(normalizedInlines);

            var inlineContainer = AddContainer(txtb);

            return new List<TextElement>(1) { inlineContainer };
        }
    }
}
