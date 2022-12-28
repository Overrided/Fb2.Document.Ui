using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Models.Base;
using Fb2.Document.UWP.Common;
using Fb2.Document.UWP.Services;
using Windows.Foundation;

namespace Fb2.Document.UWP.Entities
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
