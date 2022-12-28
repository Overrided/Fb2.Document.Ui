using System.Collections.Generic;
using Fb2.Document.Constants;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.NodeProcessors.Base;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.NodeProcessors
{
    public class SequenceProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var node = context.Node;

            var result = node.TryGetAttribute(AttributeNames.Name, true, out var seqNameKvp) ?
                    seqNameKvp.Value :
                    string.Empty; ;

            if (node.TryGetAttribute(AttributeNames.Number, true, out var seqNumberKvps))
                result = string.IsNullOrEmpty(result) ? seqNumberKvps.Value : $"{result} {seqNumberKvps.Value}";

            return string.IsNullOrEmpty(result) ?
                context.Utils.Paragraphize(base.Process(context)) :
                context.Utils.Paragraphize(new Run() { Text = result });
        }
    }
}
