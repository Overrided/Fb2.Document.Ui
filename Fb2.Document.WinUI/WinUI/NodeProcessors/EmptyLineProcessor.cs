using System.Collections.Generic;
using Fb2.Document.UI.WinUi.Entities;
using Fb2.Document.UI.WinUi.NodeProcessors.Base;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.WinUi.NodeProcessors
{
    public class EmptyLineProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(IRenderingContext context) => new List<TextElement>(1) { new LineBreak() };
    }
}
