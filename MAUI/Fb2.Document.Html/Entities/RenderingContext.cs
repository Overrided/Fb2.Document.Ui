using System.Collections.Generic;
using Fb2.Document.Html.Services;
using Fb2.Document.Models.Base;

namespace Fb2.Document.Html.Entities;

//public class RenderingContext<T> : IRenderingContext where T : class
public class RenderingContext
{
    //TODO: remove test config
    //private Config defaultConfig = new Config(poemConfig:
    //    new PoemConfig(dateHorizontalAlignment: TextAlignment.Center,
    //        textAuthorHorizontalAlignment: TextAlignment.Left));
    private Fb2MappingConfig defaultConfig = new Fb2MappingConfig();

    internal RenderingContext(IEnumerable<Fb2Node> data, Fb2MappingConfig? config = null)
    {
        Data = data;
        RenderingConfig = config ?? defaultConfig;
    }

    // Services, half of them should not be there)
    public NodeProcessorFactory ProcessorFactory => NodeProcessorFactory.Instance;

    public ElementStyler ElementStyler => ElementStyler.Instance;

    //Data
    public Fb2MappingConfig RenderingConfig { get; }

    public IEnumerable<Fb2Node> Data { get; }

    //State
    public Fb2Node? CurrentNode { get; private set; } = null;

    public void UpdateNode(Fb2Node node)
    {
        CurrentNode = node;
    }

    public void Backtrack()
    {
        CurrentNode = CurrentNode?.Parent;
    }
}
