using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Models.Base;
using Fb2.Document.Models;
using Fb2.Document.Html.Entities;
using System.Drawing;
using System.Xml.Linq;

namespace Fb2.Document.Html;

public class Fb2HtmlMapper
{
    public static List<string> MapDocument(Fb2Document document, Fb2DocumentMappingConfig? config = null)
    {
        var docConfig = config ?? new();

        var mapWholeDoc = docConfig.MapWholeDocument;

        var wholeDocNodes = new List<Fb2Node>(1) { document.Book! };
        var context = new RenderingContext(wholeDocNodes, docConfig);

        var renderableNodes = mapWholeDoc ?
            wholeDocNodes :
            GetRenderableNodes(document);

        var mappd = MapContent(renderableNodes, context);
        //var joined = string.Join(string.Empty, mappd); // TODO : stringBuilder / byte[]
        return mappd;
    }

    public static List<string> MapNode(Fb2Node node, Fb2MappingConfig? config = null)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        var nodesToMap = new List<Fb2Node>(1) { node };
        var context = new RenderingContext(nodesToMap, config);

        return MapContent(nodesToMap, context);
    }

    public static List<string> MapNodes(IEnumerable<Fb2Node> nodes, Fb2MappingConfig? config = null)
    {
        if (nodes == null || !nodes.Any())
            throw new ArgumentNullException(nameof(nodes));

        var context = new RenderingContext(nodes, config);

        return MapContent(nodes, context);
    }

    private static List<string> MapContent(IEnumerable<Fb2Node> nodes, RenderingContext renderingContext)
    {
        var buildNodes = BuildNodes(nodes, renderingContext);
        if (buildNodes == null || buildNodes.Count == 0)
        {
            return new List<string>(0);
        }

        return buildNodes;

        //var dataPages = PaginateContent(nodes);
        //var textNodes = dataPages
        //    .Select(dp =>
        //    {
        //        var buildNodes = BuildNodes(dp, renderingContext);
        //        if (buildNodes == null || buildNodes.Count == 0)
        //            return string.Empty;

        //        return string.Join(string.Empty, buildNodes);
        //    })
        //    .Where(s => !string.IsNullOrEmpty(s))
        //    .ToList();

        //return textNodes;
    }

    private static List<string> BuildNodes(IEnumerable<Fb2Node> nodes, RenderingContext context) =>
        nodes.Select(n => context.ProcessorFactory.DefaultProcessor.ElementSelector(n, context))
             //.OfType<List<string>>()
             //.SelectMany(l => l)
             .ToList();

    private static List<Fb2Node> GetRenderableNodes(Fb2Document document)
    {
        var renderableNodes = new List<Fb2Node>();

        if (document.Book?.TryGetFirstDescendant<Coverpage>(out var coverpage) ?? false)
            renderableNodes.Add(coverpage!);

        renderableNodes.AddRange(document.Bodies);
        return renderableNodes;
    }

    //private static List<List<Fb2Node>> PaginateContent(IEnumerable<Fb2Node> nodes)
    //{
    //    var result = new List<List<Fb2Node>>();
    //    var currentPage = new List<Fb2Node>();

    //    foreach (var node in nodes)
    //    {
    //        if (node is BookBody || node is BodySection || node is Coverpage)
    //        {
    //            if (currentPage.Any())
    //            {
    //                result.Add(currentPage);
    //                currentPage = new List<Fb2Node>();
    //            }

    //            var bodyContent = PaginateContent((node as Fb2Container)!.Content);
    //            if (bodyContent != null && bodyContent.Any())
    //                result.AddRange(bodyContent);
    //        }
    //        else
    //            currentPage.Add(node);
    //    }

    //    if (currentPage.Any())
    //        result.Add(currentPage);

    //    return result;
    //}
}
