using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using Fb2.Document.WPF.Entities;
using Fb2.Document.WPF.NodeProcessors.Base;

namespace Fb2.Document.WPF.NodeProcessors;

public class SupescriptProcessor : DefaultNodeProcessor
{
    public override List<TextElement> Process(RenderingContext context)
    {
        var normalizedContent = base.Process(context);

        var span = new Span();
        span.Inlines.AddRange(normalizedContent);
        span.BaselineAlignment = BaselineAlignment.Superscript;

        return new List<TextElement>(1) { span };
    }
}
