using System.Collections.Generic;
using Fb2.Document.UI.WinUi.Entities;
using Fb2.Document.UI.WinUi.Extensions;
using Fb2.Document.UI.WinUi.NodeProcessors.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.WinUi.NodeProcessors
{
    public class SupescriptProcessor : RewrapNodeProcessorBase
    {
        public override List<TextElement> Process(IRenderingContext context)
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
