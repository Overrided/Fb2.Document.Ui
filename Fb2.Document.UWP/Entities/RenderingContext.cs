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
    public class RenderingContext
    {
        //TODO: remove test config
        //private Config defaultConfig = new Config(poemConfig:
        //    new PoemConfig(dateHorizontalAlignment: TextAlignment.Center,
        //        textAuthorHorizontalAlignment: TextAlignment.Left));
        private Fb2MappingConfig defaultConfig = new Fb2MappingConfig();

        internal RenderingContext(IEnumerable<Fb2Node> data, Size viewPortSize, Fb2MappingConfig config = null)
        {
            Data = data;
            RenderingConfig = config ?? defaultConfig;
            ViewPortSize = viewPortSize;
        }

        // Services, half of them should not be there)
        public ElementStyler Styler => ElementStyler.Instance;

        public Utils Utils => Utils.Instance;

        public NodeProcessorFactory ProcessorFactory => NodeProcessorFactory.Instance;

        //Data
        public Fb2MappingConfig RenderingConfig { get; }

        public IEnumerable<Fb2Node> Data { get; } = null;

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
