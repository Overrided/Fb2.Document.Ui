using System;
using Fb2.Document.Html.Entities;
using Fb2.Document.Html.NodeProcessors.Base;

namespace Fb2.Document.Html.NodeProcessors;

public class EmptyLineProcessor : DefaultFb2HtmlNodeProcessor
{
    public override string Process(RenderingContext context) => $"<br>{Environment.NewLine}";
}
