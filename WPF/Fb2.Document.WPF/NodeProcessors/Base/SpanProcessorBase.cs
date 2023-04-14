using System.Collections.Generic;
using System.Windows.Documents;
using Fb2.Document.WPF.Entities;

namespace Fb2.Document.WPF.NodeProcessors.Base;

public abstract class SpanProcessorBase<T> : DefaultNodeProcessor where T : Span, new()
{
    public override List<TextElement> Process(RenderingContext context)
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
