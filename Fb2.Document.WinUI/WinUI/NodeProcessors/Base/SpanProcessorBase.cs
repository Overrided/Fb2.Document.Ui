using System.Collections.Generic;
using Fb2.Document.UI.WinUi.Entities;
using Fb2.Document.UI.WinUi.Extensions;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.WinUi.NodeProcessors.Base
{
    public abstract class SpanProcessorBase<T> : DefaultNodeProcessor where T : Span, new()
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var inlines = base.Process(context);

            var result = new T();
            result.Inlines.AddRange(inlines);

            return new List<TextElement>(1) { result };
        }
    }

    public class BoldProcessor : SpanProcessorBase<Bold> { }
    public class ItalicProcessor : SpanProcessorBase<Italic> { }
    public class SpanProcessor : SpanProcessorBase<Span> { }
}
