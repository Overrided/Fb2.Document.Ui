using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace RichTextView.Extensions
{
    public static class DependencyObjExtensions
    {
        public static T FindVisualChild<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj is T tObj)
                return tObj;

            var childrenCount = VisualTreeHelper.GetChildrenCount(depObj);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T tChild)
                    return tChild;

                var result = child.FindVisualChild<T>();

                if (result != null)
                    return result;
            }

            return null;
        }

        public static List<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            var result = new List<T>();

            if (depObj is T tObj)
                result.Add(tObj);

            var childrenCount = VisualTreeHelper.GetChildrenCount(depObj);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var subChildren = child.FindVisualChildren<T>();

                if (subChildren != null && subChildren.Any())
                    result.AddRange(subChildren);
            }

            return result;
        }

        public static List<T> GetAllTextElements<T>(this RichTextBlock rtb) where T : TextElement
        {
            var result = new List<T>();

            var blocks = rtb.Blocks;

            foreach (var block in blocks)
            {
                if (block is T blockT)
                {
                    result.Add(blockT);
                    continue;
                }

                var inlines = ((Paragraph)block).Inlines;

                var res = TraverseInline<T>(inlines);
                if (res != null && res.Any())
                    result.AddRange(res);
            }

            return result;
        }

        public static List<T> GetAllTextElements<T>(this TextElement textElement) where T : TextElement
        {
            var result = new List<T>();

            if (textElement is T typeElement)
                result.Add(typeElement);

            if (textElement is Block blockElement)
            {
                var inlines = ((Paragraph)blockElement).Inlines;
                foreach (var inline in inlines)
                {
                    var subNodes = inline.GetAllTextElements<T>();
                    if (subNodes != null && subNodes.Count > 0)
                        result.AddRange(subNodes);
                }
            }

            if (textElement is Span spanElement)
            {
                var spanInlines = spanElement.Inlines;
                foreach (var inline in spanInlines)
                {
                    var subNodes = inline.GetAllTextElements<T>();
                    if (subNodes != null && subNodes.Count > 0)
                        result.AddRange(subNodes);
                }
            }

            return result;
        }

        private static List<T> TraverseInline<T>(IEnumerable<Inline> inlines) where T : TextElement
        {
            var result = new List<T>();

            foreach (var item in inlines)
            {
                if (item is T tItem)
                {
                    result.Add(tItem);
                    continue;
                }
                else if (item is Span spanItem)
                {
                    var spanInlines = spanItem.Inlines;
                    var results = TraverseInline<T>(spanInlines);

                    if (results != null && results.Any())
                        result.AddRange(results);
                }
            }

            return result;
        }

        // This helper function is essentially the same as this answer:
        // https://stackoverflow.com/a/14132711/4184842
        //
        // It adds an additional forced 1ms delay to let the UI thread
        // catch up.
        public static Task FinishLayoutAsync(this FrameworkElement element)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            // Setup handler that will be raised when layout has completed.
            EventHandler<object> handler = null;
            handler = (s, a) =>
            {
                element.LayoutUpdated -= handler;
                tcs.SetResult(true);
            };
            element.LayoutUpdated += handler;

            // Await at least 1 ms (to force UI pump) and until the Task is completed
            // If you don't wait the 1ms then you can get a 'layout cycle detected' error
            // from the XAML runtime.
            return Task.WhenAll(new[] { Task.Delay(1), tcs.Task });
        }

        public static IEnumerable<DependencyObject> GetVisualParents(this DependencyObject element)
        {
            var result = new List<DependencyObject>();

            var parent = VisualTreeHelper.GetParent(element);
            if (parent == null || parent is RichTextBlock)
                return result;

            result.Add(parent);

            var higherParents = parent.GetVisualParents();

            if (higherParents != null && higherParents.Any())
                result.AddRange(higherParents);

            return result;
        }
    }
}
