using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Entities;
using Microsoft.UI.Xaml.Documents;
using Windows.Foundation;

namespace Fb2.Document.WinUI
{
    // TODO : make static/singletone? 
    // TODO : add table of contents?? (or on app level?)
    public class Fb2Mapper
    {
        private static readonly Lazy<Fb2Mapper> instance = new(() => new Fb2Mapper(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static Fb2Mapper Instance => instance.Value;

        private Fb2Mapper() { }

        public IEnumerable<Fb2ContentPage> MapDocument(Fb2Document document, Size viewPortSize, Fb2DocumentMappingConfig? config = null)
        {
            var docConfig = config ?? new();

            var mapWholeDoc = docConfig.MapWholeDocument;

            var wholeDocNodes = new List<Fb2Node>(1) { document.Book! };
            var context = new RenderingContext(wholeDocNodes, viewPortSize, docConfig);

            var renderableNodes = mapWholeDoc ?
                wholeDocNodes :
                GetRenderableNodes(document);

            return MapContent(renderableNodes, context);
        }

        public IEnumerable<Fb2ContentPage> MapNodes(IEnumerable<Fb2Node> nodes, Size viewPortSize, Fb2MappingConfig? config = null)
        {
            if (nodes == null || !nodes.Any())
                throw new ArgumentNullException(nameof(nodes));

            var context = new RenderingContext(nodes, viewPortSize, config);

            return MapContent(nodes, context);
        }

        public IEnumerable<Fb2ContentPage> MapNode(Fb2Node node, Size viewPortSize, Fb2MappingConfig? config = null)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var nodesToMap = new List<Fb2Node>(1) { node };
            var context = new RenderingContext(nodesToMap, viewPortSize, config);

            return MapContent(nodesToMap, context);
        }

        private static IEnumerable<Fb2ContentPage> MapContent(IEnumerable<Fb2Node> nodes, RenderingContext renderingContext)
        {
            var dataPages = PaginateContent(nodes);
            var textNodes = dataPages.Select(dp =>
            {
                var buildNodes = BuildNodes(dp, renderingContext);
                return new Fb2ContentPage(buildNodes);
            });

            return textNodes;
        }

        private static List<TextElement> BuildNodes(IEnumerable<Fb2Node> nodes, RenderingContext context) =>
            nodes.Select(n => context.ProcessorFactory.DefaultProcessor.ElementSelector(n, context))
                 .OfType<List<TextElement>>()
                 .SelectMany(l => l).ToList();

        private static List<Fb2Node> GetRenderableNodes(Fb2Document document)
        {
            var renderableNodes = new List<Fb2Node>();

            if (document.Book?.TryGetFirstDescendant<Coverpage>(out var coverpage) ?? false)
                renderableNodes.Add(coverpage!);

            renderableNodes.AddRange(document.Bodies);
            return renderableNodes;
        }

        private static List<List<Fb2Node>> PaginateContent(IEnumerable<Fb2Node> nodes)
        {
            var result = new List<List<Fb2Node>>();
            var currentPage = new List<Fb2Node>();

            foreach (var node in nodes)
            {
                if (node is BookBody || node is BodySection || node is Coverpage)
                {
                    if (currentPage.Any())
                    {
                        result.Add(currentPage);
                        currentPage = new List<Fb2Node>();
                    }

                    var bodyContent = PaginateContent((node as Fb2Container)!.Content);
                    if (bodyContent != null && bodyContent.Any())
                        result.AddRange(bodyContent);
                }
                else
                    currentPage.Add(node);
            }

            if (currentPage.Any())
                result.Add(currentPage);

            return result;
        }
    }
}
