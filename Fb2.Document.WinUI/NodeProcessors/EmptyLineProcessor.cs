using System.Collections.Generic;
using Fb2.Document.UI.Entities;
using Fb2.Document.UI.NodeProcessors.Base;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.NodeProcessors
{
    public class EmptyLineProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(RenderingContext context) => new List<TextElement>(1) { new LineBreak() };
    }
}
