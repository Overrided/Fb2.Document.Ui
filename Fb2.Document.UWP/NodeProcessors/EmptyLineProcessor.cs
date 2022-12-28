using System.Collections.Generic;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.NodeProcessors.Base;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.NodeProcessors
{
    public class EmptyLineProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(RenderingContext context) => new List<TextElement>(1) { new LineBreak() };
    }
}
