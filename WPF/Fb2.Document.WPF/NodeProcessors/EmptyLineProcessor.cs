using System.Collections.Generic;
using System.Windows.Documents;
using Fb2.Document.WPF.Entities;
using Fb2.Document.WPF.NodeProcessors.Base;

namespace Fb2.Document.WPF.NodeProcessors;

public class EmptyLineProcessor : DefaultNodeProcessor
{
    public override List<TextElement> Process(RenderingContext context) =>
        new List<TextElement>(1) { new LineBreak() };
}
