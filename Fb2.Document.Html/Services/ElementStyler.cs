﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
