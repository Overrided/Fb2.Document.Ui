using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.Factories;
using Fb2.Document.Models.Base;
using Fb2.Document.UWP.Entities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.NodeProcessors.Base
{
    public abstract class RewrapNodeProcessorBase : DefaultNodeProcessor
    {
        private readonly HashSet<string> StylingParentNodes = new HashSet<string>
        {
            ElementNames.Strong,
            ElementNames.Emphasis,
            ElementNames.Strikethrough
        };

        private readonly HashSet<string> LayoutNode = new HashSet<string>
        {
            ElementNames.Paragraph,
            ElementNames.StanzaV,
            ElementNames.SubTitle,
            ElementNames.TableHeader,
            ElementNames.TableCell,
            ElementNames.BookBodySection,
            ElementNames.BookBody
        };

        protected Fb2Node RewrapNode(IRenderingContext context)
        {
            var affectiveParents = GetAffectingLayoutParents(context);

            if (affectiveParents == null || !affectiveParents.Any())
                return null;

            var actualNode = context.Node;
            var actualNodeContent = (actualNode as Fb2Container).Content;

            for (int i = 0; i < affectiveParents.Count; i++)
            {
                var parent = affectiveParents[i];
                var parentCloneNode = Fb2NodeFactory.GetNodeByName(parent.Name) as Fb2Container;

                if (i == 0) // first parent
                    parentCloneNode.Content.AddRange(actualNodeContent);
                else
                    parentCloneNode.Content.Add(actualNode);

                actualNode = parentCloneNode;
            }

            return actualNode;
        }

        protected InlineUIContainer AddContainer(UIElement element)
        {
            // ContentPresenter prevents lines overlap
            var contentPresenter = new Border { Child = element };
            return new InlineUIContainer { Child = contentPresenter };
        }

        private List<Fb2Node> GetAffectingLayoutParents(IRenderingContext context)
        {
            if (!context.ParentNodes.Any())
                return null;

            var result = new List<Fb2Node>();

            foreach (var node in context.ParentNodes)
            {
                var nodeName = node.Name;

                if (StylingParentNodes.Contains(nodeName))
                    result.Add(node);
                else if (LayoutNode.Contains(nodeName))
                    break;
            }

            return result;
        }
    }
}
