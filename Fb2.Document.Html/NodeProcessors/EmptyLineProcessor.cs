using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Html.Entities;
using Fb2.Document.Html.NodeProcessors.Base;
using Fb2.Document.Models.Base;

namespace Fb2.Document.Html.NodeProcessors;

public class EmptyLineProcessor : DefaultFb2HtmlNodeProcessor
{
    public override string Process(RenderingContext context)
    {
        //var currentNode = context.CurrentNode as Fb2Element;
        //var content = currentNode?.Content ?? string.Empty;
        //return content;
        return $"</br>{Environment.NewLine}";

        //return base.Process(context);
    }
}
