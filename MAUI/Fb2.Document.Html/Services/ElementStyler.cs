using System;
using System.Collections.Generic;
using System.Threading;
using Fb2.Document.Constants;
using Fb2.Document.Html.Entities;

namespace Fb2.Document.Html.Services;

public class ElementStyler
{
    private static readonly Lazy<ElementStyler> instance = new Lazy<ElementStyler>(() => new ElementStyler(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static ElementStyler Instance => instance.Value;

    private const string MonotypeFontFamily = "Courier New";

    private Dictionary<string, Func<RenderingContext, string, string>> styleMap = new Dictionary<string, Func<RenderingContext, string, string>>
    {
        { ElementNames.Paragraph, (context, htmlTag) =>
        {
            return "style=\"text-indent: 20px;\"";
        }},
        { ElementNames.Title, (context, htmlTag) =>
        {
            return "style=\"text-align: center;\"";
        }},
        { ElementNames.SubTitle, (context, htmlTag) =>
        {
            return "style=\"text-align: center;\"";
        }},
        { ElementNames.Image, (context, tag) =>
        {
            var node = context.CurrentNode;
            var isInlineImage = node.IsInline;
            if(isInlineImage)
                return string.Empty;

            return "style=\"margin:auto;display:block;max-width:100%\"";
        }},
        { ElementNames.Table, (context, htmlTag) =>
        {
            return "class=\"table table-bordered\"";
        }}
    };

    public string GetInlineStyles(RenderingContext context, string htmlTag)
    {
        var currentNode = context.CurrentNode;
        if (currentNode == null)
            return string.Empty;

        var nodeName = currentNode.Name;

        if (!styleMap.TryGetValue(nodeName, out var initFunc))
            return string.Empty;

        var style = initFunc(context, htmlTag);
        return style;
    }
}
