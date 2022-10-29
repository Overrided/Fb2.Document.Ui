using System.Collections.Generic;
using Fb2.Document.Models.Base;
using Fb2.Document.UI.WinUi.Common;
using Fb2.Document.UI.WinUi.Services;
using Windows.Foundation;

namespace Fb2.Document.UI.WinUi.Entities
{
    public interface IRenderingContext
    {
        Fb2Node CurrentNode { get; }
        NodeProcessorFactory ProcessorFactory { get; }
        Fb2MappingConfig RenderingConfig { get; }
        ElementStyler Styler { get; }
        Utils Utils { get; }
        Size ViewPortSize { get; }

        void Backtrack();
        void UpdateNode(Fb2Node node);
    }
}