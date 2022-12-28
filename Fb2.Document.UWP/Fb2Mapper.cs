using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Models.Base;
using Fb2.Document.Models;
using Windows.UI.Xaml.Documents;
using Fb2.Document.UWP.Entities;
using Windows.Foundation;
using Fb2.Document.UWP.Common;

namespace Fb2.Document.UWP
{
    public class Fb2Mapper
    {
        public List<Fb2ContentPage> MapDocument(Fb2Document document, Size viewPortSize, Fb2MappingConfig config = null)
        {
            var context = new RenderingContext<Fb2Document>(document, viewPortSize, config);

            var contentWatch = Stopwatch.StartNew();

            var renderableNodes = GetRenderableNodes(context);

            // TODO: look into it better, it's another way of paginating content
            //var prePaginateContent = PaginateContent(renderableNodes);
            //var preBuildNodes = prePaginateContent.Select((page) => // works but breaks styles (((
            //{
            //    //if (context.ParentNodes.Any())
            //    //    while (context.ParentNodes.Any())
            //    //        context.Backtrack();

            //    //context.UpdateNode(new BodySection());

            //    var pageTe = BuildNodes(page, context);

            //    //context.Backtrack();

            //    return new Fb2ContentPage(pageTe);
            //}).ToList();

            //return preBuildNodes;

            var buildNodes = BuildNodes(renderableNodes, context);

            var dataPages = PaginateContent(buildNodes, context);

            Debug.WriteLine($"Data preparations: {contentWatch.Elapsed}");

            return dataPages;
        }

        public List<Fb2ContentPage> MapNode(Fb2Node node, Size viewPortSize, Fb2MappingConfig config = null)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var context = new RenderingContext<Fb2Node>(node, viewPortSize, config);

            var textNodes = BuildNode(node, context);
            var dataPages = PaginateContent(textNodes, context);
            return dataPages;
        }

        public List<Fb2ContentPage> MapNodes(IEnumerable<Fb2Node> nodes, Size viewPortSize, Fb2MappingConfig config = null)
        {
            if (nodes == null || !nodes.Any())
                throw new ArgumentNullException(nameof(nodes));

            var context = new RenderingContext<IEnumerable<Fb2Node>>(nodes, viewPortSize, config);

            var buildNodes = BuildNodes(nodes, context);
            var dataPages = PaginateContent(buildNodes, context);
            return dataPages;
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

            // TODO : use TryGetFirstDescendant once new version of lib is compatible
            var coverpage = context.Data.Book.GetFirstDescendant<Coverpage>();
            if (coverpage != null)
                renderableNodes.Add(coverpage);

            renderableNodes.AddRange(context.Data.Bodies);

            return renderableNodes;
        }

        // TODO : take a look into it, alternating way of "content pagination"
        // TODO : pass "parent" of element as a parameter, and return tuple if element matches?
        // so each time we need to render we know what parent it was and can establish styles
        //private List<IEnumerable<Fb2Node>> PaginateContent(List<Fb2Node> nodes) // is not compatible with newest fb2.document
        //{
        //    var result = new List<IEnumerable<Fb2Node>>();
        //    var currentPage = new List<Fb2Node>();

        //    for (int i = 0; i < nodes.Count; i++)
        //    {
        //        var node = nodes[i];

        //        if (node is BookBody || node is BodySection)
        //        {
        //            if (currentPage.Any())
        //            {
        //                result.Add(currentPage);
        //                currentPage = new List<Fb2Node>();
        //            }

        //            var bodyContent = PaginateContent((node as Fb2Container).Content);
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

        private List<Fb2ContentPage> PaginateContent(List<TextElement> elements, IRenderingContext context)
        {
            var result = new List<Fb2ContentPage>();

            var currentPageData = new Fb2ContentPage();

            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];

                if (i == 0)
                {
                    currentPageData.Add(element);
                    continue;
                }

                var containerType = context.DependencyPropertyManager.GetProperty(element, Fb2UIConstants.ContainerTypeAttributeName);

                if (!string.IsNullOrWhiteSpace(containerType))
                {
                    result.Add(currentPageData);
                    currentPageData = new Fb2ContentPage { element };
                }
                else  // not a container at all
                    currentPageData.Add(element);
            }

            if (currentPageData.Any())
                result.Add(currentPageData);

            return result;
        }
    }
}
