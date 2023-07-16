using System;
using Fb2.Document.Html.Entities;
using Fb2.Document.Models.Base;

namespace Fb2.Document.Html.NodeProcessors.Base;

public abstract class Fb2HtmlNodeProcessorBase
{
    //private const string TagSeparator = "|";

    public abstract string CorrespondingHtmlTag { get; }

    public abstract string Process(RenderingContext context);

    protected abstract string ProcessAttributes(
        RenderingContext context,
        string htmlTag,
        Func<Fb2Attribute, bool>? attributePredicate = null);

    public string ElementSelector(Fb2Node node, RenderingContext context)
    {
        context.UpdateNode(node);

        var processor = context.ProcessorFactory.GetNodeProcessor(node);
        var result = processor.Process(context);

        //var shouldApplyStyles = context.RenderingConfig.UseStyles && (result?.Any() ?? false);
        //if (shouldApplyStyles)
        //    context.Styler.ApplyStyle(context, result!);

        context.Backtrack();

        return result;
    }
}
