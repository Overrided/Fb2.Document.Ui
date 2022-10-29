using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.Factories;
using Fb2.Document.Models.Base;
using Fb2.Document.UI.WinUi.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.WinUi.NodeProcessors.Base
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

        public override List<TextElement> Process(IRenderingContext context)
        {
            var originalNode = context.CurrentNode;

            var rewrappedNode = RewrapNode(context);
            //if 'rewrappedNode' != null => backtrack in ElementSelector will go back 1 node too far, restore state later

            var inlines = rewrappedNode != null ? ElementSelector(rewrappedNode, context) : base.Process(context);
            var normalizedContent = context.Utils.Paragraphize(inlines);

            context.UpdateNode(originalNode); // restoring current node state after what Rewrap could have done

            return normalizedContent;
        }

        protected Fb2Node RewrapNode(IRenderingContext context)
        {
            // use node not context
            var affectiveParents = GetAffectingLayoutParents(context);

            if (affectiveParents == null || !affectiveParents.Any())
                return null;

            var actualNode = context.CurrentNode;
            var actualNodeContent = (actualNode as Fb2Container).Content;

            for (int i = 0; i < affectiveParents.Count; i++)
            {
                var parent = affectiveParents[i];
                var parentCloneNode = Fb2NodeFactory.GetNodeByName(parent.Name) as Fb2Container;

                if (i == 0) // first parent
                    parentCloneNode.AddContent(actualNodeContent);
                else
                    parentCloneNode.AddContent(actualNode);

                actualNode = parentCloneNode;
            }

            return actualNode;
        }

        protected InlineUIContainer AddContainer(UIElement element)
        {
            // prevents lines overlap
            var innerContainer = new Border { Child = element };
            return new InlineUIContainer { Child = innerContainer };
        }

        private List<Fb2Container> GetAffectingLayoutParents(IRenderingContext context)
        {
            var ancestors = context.CurrentNode.GetAncestors();

            if (!ancestors.Any())
                return null;

            var result = new List<Fb2Container>();

            foreach (var node in ancestors)
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
