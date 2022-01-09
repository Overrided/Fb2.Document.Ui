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
        }

        // Services, half of them should not be there)
        public ElementStyler Styler { get; }

        public Utils Utils { get; }

        public NodeProcessorFactory ProcessorFactory { get; }

        //Data
        public Fb2MappingConfig RenderingConfig { get; }

        public T Data { get; } = null;

        public Size ViewPortSize { get; } = Size.Empty;

        //State
        public Fb2Node CurrentNode { get; private set; } = null;

        public void UpdateNode(Fb2Node node)
        {
            CurrentNode = node;
        }

        public void Backtrack()
        {
            CurrentNode = CurrentNode?.Parent;
        }
    }
}
