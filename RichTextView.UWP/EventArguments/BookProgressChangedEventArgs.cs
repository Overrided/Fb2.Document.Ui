using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RichTextView.UWP.EventArguments
{
    public class BookProgressChangedEventArgs
    {
        public double VerticalOffset { get; }

        public double ScrollableHeight { get; }

        public BookProgressChangedEventArgs(double verticalOffset, double scrollableHeight)
        {
            VerticalOffset = verticalOffset;
            ScrollableHeight = scrollableHeight;
        }
    }
}
