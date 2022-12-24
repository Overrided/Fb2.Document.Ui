using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Html.Entities;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;

namespace Fb2.Document.Html.NodeProcessors.Base;

public abstract class Fb2HtmlNodeProcessorBase
{
    //private const string TagSeparator = "|";

    //public abstract string CorrespondingHtmlTag { get; protected set; }
    public abstract string CorrespondingHtmlTag { get; }

    // later reimplement into byte[]
    //public abstract string Process(RenderingContext context);
    public abstract string Process(RenderingContext context);

    protected abstract string ProcessAttributes(RenderingContext context, string htmlTag);

    // later reimplement into byte[]
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
