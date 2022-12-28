using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.NodeProcessors.Base;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.NodeProcessors
{
    public class CustomInfoProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            if (context.Node.TryGetAttribute(AttributeNames.InfoType, true, out var infoTypeKvp))
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
