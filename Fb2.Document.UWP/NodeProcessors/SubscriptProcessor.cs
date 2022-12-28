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
