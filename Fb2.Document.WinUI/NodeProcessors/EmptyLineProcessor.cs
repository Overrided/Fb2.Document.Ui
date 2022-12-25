using System.Collections.Generic;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.NodeProcessors.Base;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors
{
    public class EmptyLineProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(RenderingContext context) => new List<TextElement>(1) { new LineBreak() };
    }
}
