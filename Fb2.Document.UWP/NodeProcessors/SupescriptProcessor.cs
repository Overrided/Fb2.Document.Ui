using System.Collections.Generic;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.Extensions;
using Fb2.Document.UWP.NodeProcessors.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.NodeProcessors
{
    public class SupescriptProcessor : RewrapNodeProcessorBase
    {
        public override List<TextElement> Process(RenderingContext context)
        {
            var normalizedContent = base.Process(context);

            var txtb = new RichTextBlock
            {
                FontSize = context.RenderingConfig.BaseFontSize,
                Margin = new Thickness(0, 0, 0, 4)
            };
            txtb.Blocks.AddRange(normalizedContent);

            var inlineContainer = AddContainer(txtb);

            return new List<TextElement>(1) { inlineContainer };
        }
    }
}
