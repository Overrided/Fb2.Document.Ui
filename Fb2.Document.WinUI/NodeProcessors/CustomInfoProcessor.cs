using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.NodeProcessors.Base;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors
{
    public class CustomInfoProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            if (context.CurrentNode.TryGetAttribute(AttributeNames.InfoType, true, out var infoTypeKvp))
            {
                var attributeRun = new Run { Text = infoTypeKvp.Value };
                var baseInlines = base.Process(context);
                var allData = baseInlines.Prepend(attributeRun);
                return context.Utils.Paragraphize(allData);
            }

            return context.Utils.Paragraphize(base.Process(context));
        }
    }
}
