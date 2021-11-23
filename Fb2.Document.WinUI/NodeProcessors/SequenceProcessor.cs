using System.Collections.Generic;
using Fb2.Document.Constants;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.NodeProcessors.Base;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors
{
    public class SequenceProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var node = context.Node;

            var result = node.TryGetAttribute(AttributeNames.Name, out var seqNameKvp, true) ?
                    seqNameKvp.Value :
                    string.Empty;

            if (node.TryGetAttribute(AttributeNames.Number, out var seqNumberKvps, true))
                result = string.IsNullOrEmpty(result) ? seqNumberKvps.Value : $"{result} {seqNumberKvps.Value}";

            return string.IsNullOrEmpty(result) ?
                context.Utils.Paragraphize(base.Process(context)) :
                context.Utils.Paragraphize(new Run() { Text = result });
        }
    }
}
