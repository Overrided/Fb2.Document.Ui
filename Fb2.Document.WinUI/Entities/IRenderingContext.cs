using System.Collections.Generic;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Common;
using Fb2.Document.WinUI.Services;
using Windows.Foundation;

namespace Fb2.Document.WinUI.Entities
{
    public interface IRenderingContext
    {
        DependencyPropertyManager DependencyPropertyManager { get; }
        Fb2Node Node { get; }
        Stack<Fb2Node> ParentNodes { get; }
        NodeProcessorFactory ProcessorFactory { get; }
        Fb2MappingConfig RenderingConfig { get; }
        ElementStyler Styler { get; }
        Utils Utils { get; }
        Size ViewPortSize { get; }

        void Backtrack();
        void UpdateNode(Fb2Node node);
    }
}