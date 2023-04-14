using System.Collections.Generic;
using Fb2.Document.Constants;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.NodeProcessors.Base;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.NodeProcessors
{
    public class CustomInfoProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(RenderingContext context)
        {
            var processedInlines = base.Process(context);

            var currentNode = context.CurrentNode;

            if ((currentNode?.TryGetAttribute(AttributeNames.InfoType, true, out var infoTypeKvp) ?? false) &&
                !string.IsNullOrEmpty(infoTypeKvp?.Value))
            {
                var attributeRun = new Run { Text = infoTypeKvp.Value };
                processedInlines.Insert(0, attributeRun);
            }

            return context.Utils.Paragraphize(processedInlines);
        }
    }
}
