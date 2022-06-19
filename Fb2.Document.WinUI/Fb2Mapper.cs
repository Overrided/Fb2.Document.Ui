using System;
using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Entities;
using Microsoft.UI.Xaml.Documents;
using Windows.Foundation;

namespace Fb2.Document.WinUI
{
    // TODO : make static? 
    // TODO : add table of contents?? (or on app level?)
    public class Fb2Mapper
    {
        public IEnumerable<Fb2ContentPage> MapDocument(Fb2Document document, Size viewPortSize, Fb2MappingConfig config = null)
        {
            var context = new RenderingContext<Fb2Document>(document, viewPortSize, config);

            var renderableNodes = GetRenderableNodes(context);

            return MapContent(renderableNodes, context);
        }

        public IEnumerable<Fb2ContentPage> MapNodes(IEnumerable<Fb2Node> nodes, Size viewPortSize, Fb2MappingConfig config = null)
        {
            if (nodes == null || !nodes.Any())
                throw new ArgumentNullException(nameof(nodes));

            var context = new RenderingContext<IEnumerable<Fb2Node>>(nodes, viewPortSize, config);

            return MapContent(nodes, context);
        }

        public IEnumerable<Fb2ContentPage> MapNode(Fb2Node node, Size viewPortSize, Fb2MappingConfig config = null)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var context = new RenderingContext<Fb2Node>(node, viewPortSize, config);

            return MapContent(new Fb2Node[1] { node }, context);
        }

        private IEnumerable<Fb2ContentPage> MapContent(IEnumerable<Fb2Node> nodes, IRenderingContext renderingContext)
        {
            var dataPages = PaginateContent(nodes);
            var textNodes = dataPages.Select(dp =>
            {
                var buildNodes = BuildNodes(dp, renderingContext);
                return new Fb2ContentPage(buildNodes);
            });

            return textNodes;
        }

        private List<TextElement> BuildNodes(IEnumerable<Fb2Node> nodes, IRenderingContext context) =>
            nodes.Select(n => BuildNode(n, context))
                 .OfType<List<TextElement>>()
                 .SelectMany(l => l).ToList();

        private List<TextElement> BuildNode(Fb2Node node, IRenderingContext context) =>
            context.ProcessorFactory.DefaultProcessor.ElementSelector(node, context);

        private List<Fb2Node> GetRenderableNodes(RenderingContext<Fb2Document> context)
        {
            var renderableNodes = new List<Fb2Node>();

            if (context.Data.Book.TryGetFirstDescendant<Coverpage>(out var coverpage))
                renderableNodes.Add(coverpage);

            renderableNodes.AddRange(context.Data.Bodies);
            return renderableNodes;
        }

        private List<List<Fb2Node>> PaginateContent(IEnumerable<Fb2Node> nodes)
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

                    var bodyContent = PaginateContent((node as Fb2Container).Content);
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
