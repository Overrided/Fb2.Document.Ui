using System;
using System.Linq;
using System.Net;
using System.Text;
using Fb2.Document.Constants;
using Fb2.Document.Html.Entities;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;

namespace Fb2.Document.Html.NodeProcessors.Base;

public class DefaultFb2HtmlNodeProcessor : Fb2HtmlNodeProcessorBase
{
    public override string CorrespondingHtmlTag
    {
        get => string.Empty;
    }

    public override string Process(RenderingContext context)
    {
        var currentNode = context.CurrentNode;
        if (currentNode == null)
            return string.Empty;

        var sb = new StringBuilder();

        var htmlTag = string.IsNullOrEmpty(CorrespondingHtmlTag) ? currentNode.Name : CorrespondingHtmlTag;

        var attributesString = ProcessAttributes(context, htmlTag);

        var openingTag = string.IsNullOrEmpty(attributesString) ? $"<{htmlTag}>" : $"<{htmlTag} {attributesString}>";
        var closingTag = $"</{htmlTag}>";

        if (currentNode is Fb2Container containerNode)
        {
            if (!containerNode.HasContent && !containerNode.HasAttributes)
                return string.Empty;

            if (!string.IsNullOrEmpty(openingTag))
                sb.Append(openingTag);

            var childStrings = containerNode.Content
                .Select(n => ElementSelector(n, context))
                //.OfType<List<TextElement>>() //    .Where(n => n != null && n.Any())
                .Where(ns => !string.IsNullOrEmpty(ns)); // emmmm, Empty Line? <br /> ?

            sb.AppendJoin(string.Empty, childStrings);

            if (!string.IsNullOrEmpty(closingTag))
                sb.AppendLine(closingTag);
        }

        if (currentNode is Fb2Element elementNode && (elementNode.HasContent || elementNode.HasAttributes))
        {
            var isTextElement = elementNode is TextItem;
            var htmlEncoded = WebUtility.HtmlEncode(elementNode.Content);
            var elementLine = isTextElement ?
                htmlEncoded :
                $"{openingTag}{htmlEncoded}{closingTag}";

            if (elementNode.IsInline)
                sb.Append(elementLine);
            else
                sb.AppendLine(elementLine);
        }

        return sb.ToString();
    }

    protected override string ProcessAttributes(
        RenderingContext context,
        string htmlTag,
        Func<Fb2Attribute, bool>? attributePredicate = null)
    {
        var currentNode = context.CurrentNode;
        if (currentNode == null)
            return string.Empty;

        var sb = new StringBuilder();
        var style = context.ElementStyler.GetInlineStyles(context, htmlTag);

        if (!string.IsNullOrEmpty(style))
            sb.Append(style);

        var hasAtributes = currentNode.HasAttributes;
        if (!hasAtributes)
            return sb.ToString();

        var nodeAttributes = attributePredicate != null ?
            currentNode.Attributes.Where(a => attributePredicate(a)).ToList() :
            currentNode.Attributes.ToList();

        var attributeStrings = nodeAttributes
            .Select(a => $"{a.Key}=\"{a.Value}\"")
            .ToList();

        sb.AppendJoin(' ', attributeStrings);

        return sb.ToString();
    }
}
