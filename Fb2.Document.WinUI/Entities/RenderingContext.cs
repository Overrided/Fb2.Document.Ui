using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Common;
using Fb2.Document.WinUI.Services;
using Windows.Foundation;

namespace Fb2.Document.WinUI.Entities
{
    internal class RenderingContext<T> : IRenderingContext where T : class
    {
        //TODO: remove test config
        //private Config defaultConfig = new Config(poemConfig:
        //    new PoemConfig(dateHorizontalAlignment: TextAlignment.Center,
        //        textAuthorHorizontalAlignment: TextAlignment.Left));
        private Fb2MappingConfig defaultConfig = new Fb2MappingConfig();

        internal RenderingContext(T data, Size viewPortSize, Fb2MappingConfig config = null)
        {
            Data = data;
            RenderingConfig = config ?? defaultConfig;
            ViewPortSize = viewPortSize;

            Styler = new ElementStyler();
            Utils = new Utils();

            // test part begins here
            ProcessorFactory = new NodeProcessorFactory();
            DependencyPropertyManager = new DependencyPropertyManager();
        }

        // Services, half of them should not be there)
        public ElementStyler Styler { get; }

        public Utils Utils { get; }

        public NodeProcessorFactory ProcessorFactory { get; }

        public DependencyPropertyManager DependencyPropertyManager { get; }

        //Data
        public Fb2MappingConfig RenderingConfig { get; }

        public T Data { get; } = null;

        public Size ViewPortSize { get; } = Size.Empty;

        //State
        public Fb2Node Node { get; private set; } = null;

        public Stack<Fb2Node> ParentNodes { get; private set; } = new Stack<Fb2Node>();

        #region State Methods

        public void UpdateNode(Fb2Node node)
        {
            if (Node != null)
                ParentNodes.Push(Node);

            Node = node;
        }

        public void Backtrack()
        {
            if (ParentNodes.Any())
                Node = ParentNodes.Pop();
            else if (Node != null)
                Node = null;
        }

        #endregion
    }
}
